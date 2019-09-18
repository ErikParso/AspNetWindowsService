using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace AspWinService.Services
{
    public class CurrentUserService
    {
        #region .dll imports

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

        [DllImport("kernel32.dll")]
        private static extern UInt32 WTSGetActiveConsoleSessionId();

        enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin
        }

        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }

        // Using IntPtr for pSID insted of Byte[]
        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ConvertSidToStringSid(
            IntPtr pSID,
            out IntPtr ptrSid);

        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            int TokenInformationLength,
            out int ReturnLength);

        [DllImport("shell32.dll")]
        static extern int SHGetFolderLocation(IntPtr hwndOwner, int nFolder,
            IntPtr hToken, uint dwReserved, out IntPtr ppidl);

        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        #endregion


        public string Account()
        {
            IntPtr token = IntPtr.Zero;
            String account = String.Empty;

            if (WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out token))
            {
                String sid = GetSID(token);
                return new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
            }
            else
            {
                return ProcessLastError();
            }
        }

        private string GetSID(IntPtr token)
        {
            bool Result;

            int TokenInfLength = 0;
            string sidAsString = String.Empty;

            // first call gets lenght of TokenInformation
            Result = GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, TokenInfLength, out TokenInfLength);

            IntPtr TokenInformation = Marshal.AllocHGlobal(TokenInfLength);
            Result = GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, TokenInformation, TokenInfLength, out TokenInfLength);

            if (Result)
            {
                TOKEN_USER TokenUser = (TOKEN_USER)Marshal.PtrToStructure(TokenInformation, typeof(TOKEN_USER));

                IntPtr pstr = IntPtr.Zero;
                Boolean ok = ConvertSidToStringSid(TokenUser.User.Sid, out pstr);

                sidAsString = Marshal.PtrToStringAuto(pstr);
                LocalFree(pstr);
            }

            Marshal.FreeHGlobal(TokenInformation);

            return sidAsString;
        }

        public string GetUserPath(int csidl)
        {
            IntPtr token = IntPtr.Zero;
            String account = String.Empty;

            if (WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out token))
            {
                IntPtr pidlist = IntPtr.Zero;
                StringBuilder sb = new StringBuilder(260);

                SHGetFolderLocation(IntPtr.Zero, csidl, token, 0, out pidlist);
                SHGetPathFromIDListW(pidlist, sb);

                return sb.ToString();
            }
            else
            {
                return ProcessLastError();
            }
        }

        private static string ProcessLastError()
        {
            int err = Marshal.GetLastWin32Error();
            switch (err)
            {
                case 5: return "ERROR_ACCESS_DENIED";
                case 87: return "ERROR_INVALID_PARAMETER";
                case 1008: return "ERROR_NO_TOKEN";
                case 1314: return "ERROR_PRIVILEGE_NOT_HELD";
                case 7022: return "ERROR_CTX_WINSTATION_NOT_FOUND";
                default: return String.Format("ERROR_{0}", err.ToString());
            }
        }
    }
}
