using AspWinService.Services;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspWinService.Tools
{
    public abstract class FileDownloadProvider
    {
        protected const string BaseFlags = "zip=2"; // vzdy chci dostat zip verze 2
        protected const string ChunkFlag = "Chunk=1";
        protected const string ChunkIndexFlagKey = "ChunkIndex=";
        protected const string ChunkSizeFlagKey = "ChunkSize=";
        protected const string ChunkStartKey = "ChunkStart=";
        protected const string ChunkContinueKey = "ChunkContinue=";
        protected const string ChunkEndKey = "ChunkEnd=";
        protected const int DownloadBufferSize = 3 * 1024 * 1024; // 3MB

        private readonly ProgressService progressService;

        public string TargetFileName { get; }
        public string DownloadTargetFileName { get; }
        public UpdateProcessorService.ExecutePlanItem ExecutePlanItem { get; }
        public string LogItemKey { get; }

        public FileDownloadProvider(
            string targetFileName,
            UpdateProcessorService.ExecutePlanItem executePlanItem, 
            string logItemKey,
            ProgressService progressService)
        {
            TargetFileName = targetFileName;
            ExecutePlanItem = executePlanItem;
            LogItemKey = logItemKey;
            this.progressService = progressService;
            DownloadTargetFileName = TargetFileName + ".part";
        }

        internal async Task<bool> DownloadAsync()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(ExecutePlanItem.TargetFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(TargetFileName));
            var indetermineStatusText = string.Empty;
            bool indetermineStatusEnabled = true;
            var _indetermineIndicatorAnimationTask = System.Threading.Tasks.Task.Run(() =>
            {
                var pointChar = (Char)0x25CF;
                do
                {
                    if (indetermineStatusText.Length > 10)
                        indetermineStatusText = string.Empty;
                    indetermineStatusText += pointChar;
                    if (!indetermineStatusEnabled) break;
                    progressService.UpdateLogItemSubItem(LogItemKey, DownloadService.LogItemSubKey_Status, indetermineStatusText);
                    Task.Delay(500).Wait();
                }
                while (indetermineStatusEnabled);
                progressService.UpdateLogItemSubItem(LogItemKey, DownloadService.LogItemSubKey_Status, string.Empty);
            });

            var currentChunkIndex = 0;
            var success = true;
            long downloaded = 0;
            long total = 0;
            bool eof = false;
            bool zipUsed = false;
            System.IO.Stream downloadStream = null;
            string timeCheck = null;
            try
            {
                do
                {
                    var flags = $"{BaseFlags} {ChunkFlag} {ChunkIndexFlagKey}{currentChunkIndex} {ChunkSizeFlagKey}{DownloadBufferSize}";

                    var fd = await CallDownloadRequest(flags);


                    if (fd.ErrorMessage != null && fd.ErrorMessage != "")
                    {
                        success = false;
                        break;
                    }

                    if (fd == null || fd.Name == string.Empty)
                    {
                        success = false;
                        break;
                    }

                    if (fd == null)
                    {
                        success = false;
                        break;
                    }


                    bool chunkResponse = false;
                    if (fd.Flags.Contains($" {ChunkStartKey}")) // this is first part of requested file
                    {// new application server with support chunks download
                        timeCheck = _readTimeCheck(fd.Flags, ChunkStartKey);
                        chunkResponse = true;
                        var innerStream = System.IO.File.Open(DownloadTargetFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                        downloadStream = new System.IO.BufferedStream(innerStream, DownloadBufferSize);
                        currentChunkIndex++;
                        zipUsed = fd.ZipUsed;
                        downloadStream.Write(fd.Data, 0, fd.Data.Length);
                    }
                    else if (fd.Flags.Contains($" {ChunkContinueKey}")) // this is second but not last part of requested file
                    {// new application server with support chunks download
                        chunkResponse = true;
                        var nCheck = _readTimeCheck(fd.Flags, ChunkContinueKey);
                        if (nCheck == timeCheck)
                        {
                            currentChunkIndex++;
                            zipUsed = fd.ZipUsed;
                            downloadStream.Write(fd.Data, 0, fd.Data.Length);
                        }
                        else
                        {//server file was modified, restart download
                            currentChunkIndex = 0;
                            downloadStream.Dispose();
                            downloadStream = null;
                            System.IO.File.Delete(DownloadTargetFileName);
                        }
                    }

                    // END muze prijit rovnou se START pokud je soubor mensi nez buffer
                    if (fd.Flags.Contains($" {ChunkEndKey}")) // this is last part of requested file
                    {// new application server with support chunks download
                        chunkResponse = true;
                        var nCheck = _readTimeCheck(fd.Flags, ChunkEndKey);
                        if (nCheck == timeCheck)
                        {
                            if (!fd.Flags.Contains($" {ChunkStartKey}")) // pokud to nebyl stav kdy start byl zaroven i end, pri kombinaci start-end v jedne odpovedi provadi zapis uz if pro start segment
                                downloadStream.Write(fd.Data, 0, fd.Data.Length);
                            else
                            {
                                // write already in if (fd.Flags.Contains($" {ChunkStartKey}")... branche in this method
                                // read first part of file is already EOF, no need call server again for next part
                            }

                            downloadStream.Flush();
                            downloadStream.Dispose();
                            downloadStream = null;
                            eof = true;
                            zipUsed = fd.ZipUsed;
                        }
                        else
                        {//server file was modified, restart download
                            currentChunkIndex = 0;
                            downloadStream.Dispose();
                            downloadStream = null;
                            System.IO.File.Delete(DownloadTargetFileName);
                        }
                    }
                    else if (!chunkResponse)
                    {// old application server without support chunks download, fd.Data contains all content
                        var innerStream = System.IO.File.Open(DownloadTargetFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                        downloadStream = new System.IO.BufferedStream(innerStream, DownloadBufferSize);
                        downloadStream.Write(fd.Data, 0, fd.Data.Length);
                        eof = true;
                        zipUsed = fd.ZipUsed;
                        total = downloaded;
                    }

                    downloaded += fd.Data.Length;

                    //DCH 0059422 16.07.2018 Pokud bude v budoucnu server posilat jednotlive casti jako samostatne ZIP soubory a ne jako casti velkeho ZIP souboru jako ted tak zde by se mel udelat Unzip a patricne upravit Total
                    total = downloaded;

                    //downloaded size
                    progressService.UpdateLogItemSubItem(LogItemKey, DownloadService.LogItemSubKey_Total, _humanizeFileSize(total));
                    progressService.UpdateLogItemSubItem(LogItemKey, DownloadService.LogItemSubKey_Downloaded, _humanizeFileSize(downloaded));
                }
                while (!eof);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                indetermineStatusEnabled = false;
                downloadStream?.Dispose();
                downloadStream = null;
            }

            // when fail we only delete downloaded file
            if (!success && System.IO.File.Exists(DownloadTargetFileName))
            {
                System.IO.File.Delete(DownloadTargetFileName);
                return success;
            }

            // delete original target file before move/rename downloaded file to target file
            if (success)
            {
                //downloaded size
                progressService.UpdateLogItemSubItem(LogItemKey, DownloadService.LogItemSubKey_Downloaded, _humanizeFileSize(new System.IO.FileInfo(DownloadTargetFileName).Length));

                if (fi.Exists)
                {
                    try
                    {
                        System.IO.FileAttributes fa = fi.Attributes & ~System.IO.FileAttributes.ReadOnly;
                        System.IO.File.SetAttributes(fi.FullName, fa); //reset readonly attribute
                        fi.Delete();
                    }
                    catch
                    {   //error? wait a while and try again
                        System.Threading.Thread.Sleep(3000);
                        try
                        {
                            System.IO.FileAttributes fa = fi.Attributes & ~System.IO.FileAttributes.ReadOnly;
                            System.IO.File.SetAttributes(fi.FullName, fa); //reset readonly attribute
                            fi.Delete();
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            success = false;
                        }
                    }
                }
            }

            // if all ok then move/rename downloaded file to targetfile
            if (success)
            {
                if (zipUsed)
                {
                    using (var sourceFs = File.Open(DownloadTargetFileName, FileMode.Open, FileAccess.Read))
                    using (var sourceBs = new System.IO.BufferedStream(sourceFs, 1024 * 1024))
                    using (var targetFs = System.IO.File.Open(TargetFileName, FileMode.Create, FileAccess.Write))
                    using (var targetBs = new System.IO.BufferedStream(targetFs, 1024 * 1024))
                    {
                        UnzipFromStreamToStream(sourceBs, targetBs);
                    }
                    System.IO.File.Delete(DownloadTargetFileName);
                }
                else
                {
                    File.Move(DownloadTargetFileName, TargetFileName);
                }

                //total size
                progressService.UpdateLogItemSubItem(LogItemKey, DownloadService.LogItemSubKey_Total, _humanizeFileSize(new System.IO.FileInfo(TargetFileName).Length));
            }
            return success;
        }

        private void UnzipFromStreamToStream(Stream inputStream, Stream outputStream)
        {
            // Refer to #ziplib documentation for more info on this
            ZipInputStream zipIn = new ZipInputStream(inputStream);
            ZipEntry theEntry = zipIn.GetNextEntry();
            byte[] buffer = new byte[2048];
            int size = 2048;
            while (true)
            {
                size = zipIn.Read(buffer, 0, buffer.Length);
                if (size > 0)
                {
                    outputStream.Write(buffer, 0, size);
                }
                else
                {
                    break;
                }
            }
        }

        private string _readTimeCheck(string flags, string chunkTypeKey)
        {
            var keyLength = chunkTypeKey.Length;
            var startKey = flags.IndexOf(chunkTypeKey);
            var startValue = startKey + keyLength - 1;
            var endValue = flags.IndexOf(' ', startValue);
            if (endValue == -1)
                return flags.Substring(startValue);
            else
                return flags.Substring(startValue, endValue - startValue);
        }

        private string _humanizeFileSize(long fileSizeBytes)
        {
            var units = new[] { "kB", "MB", "GB" };
            var nextValue = fileSizeBytes;
            if (fileSizeBytes < 1000) return string.Format("{0} {1}", fileSizeBytes, "B");
            var unitIndex = 0;
            for (var i = 0; i < units.Length; i++)
            {
                nextValue = nextValue / 1024;
                if (nextValue <= 1000)
                {
                    unitIndex = i;
                    break;
                }
            }
            var result = string.Format("{0} {1}", nextValue, units[unitIndex]);
            return result;
        }

        protected abstract Task<WSUpdate.LoadClientFileDescriptor> CallDownloadRequest(string flags);
    }
}
