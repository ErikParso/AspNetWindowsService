using System;

namespace AspWinService.Services
{
    public class ProgressService
    {
        internal string WriteLog(int imageIndex, string fileName, int downloaded, int total, string status)
        {
            Console.WriteLine($"{fileName} {downloaded}/{total} {status}");
            return Guid.NewGuid().ToString();
        }

        internal void UpdateLogItemImageIndex(string logItemKey, int imageIndex)
        {

        }

        internal void UpdateLogItemSubItem(string logItemKey, string subItemKey, string newValue)
        {

        }
    }
}
