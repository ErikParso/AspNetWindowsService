using CredentialsListener.Model;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CredentialsListener
{
    public class Credentials
    {
        public static async Task<WindowsCredentials> PromptCredentials(RpcLoginRequest inputData)
        {
            string userName = inputData.UserName;
            string password = string.Empty;

            Credentials.Info ci = new Credentials.Info(inputData.Server, "Logon");
            int netError = 0;
            bool ciSave = true;

            Credentials.SystemFlags flags =
                Credentials.SystemFlags.GENERIC_CREDENTIALS |
                //Srv.Credentials.SystemFlags.SHOW_SAVE_CHECK_BOX |
                Credentials.SystemFlags.DO_NOT_PERSIST |
                //Srv.Credentials.SystemFlags.EXPECT_CONFIRMATION |
                Credentials.SystemFlags.ALWAYS_SHOW_UI;

            var result = await PromptForCredentials(ci, inputData.Server, netError, userName, password, ciSave, flags);
            if (Credentials.ResultCode.NO_ERROR == result.Item1)
            {
                return await Task.FromResult(new WindowsCredentials()
                {
                    UserName = result.Item2,
                    Password = result.Item3,
                    Domain = inputData.Server
                });
            }
            else
            {
                return await Task.FromResult(new WindowsCredentials()
                {
                    UserName = string.Empty,
                    Password = string.Empty,
                    Domain = string.Empty
                });
            }
        }

        public const int MAX_USER_NAME = 256;
        public const int MAX_PASSWORD = 256;
        public const int MAX_DOMAIN = 256;

        /// <summary>
		/// Prompt for credentials dialog
		/// </summary>
		/// <param name="creditUI"></param>
		/// <param name="targetName"></param>
		/// <param name="netError"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="save"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public async static Task<(ResultCode, string, string, bool)> PromptForCredentials(
            Info infoUI, string targetName, int netError,
            string userName, string password, bool save, SystemFlags flags)
        {
            ResultCode result = ResultCode.NO_ERROR;
            string retUserName = string.Empty;
            string retPassword = string.Empty;
            bool retSave = true;

            await Task.Run(() =>
            {
                StringBuilder user = new System.Text.StringBuilder(userName, MAX_USER_NAME);
                StringBuilder pwd = new System.Text.StringBuilder(password, MAX_PASSWORD);
                int saveCredentials = Convert.ToInt32(save);
                _Info creditUI = new _Info();
                creditUI.cbSize = Marshal.SizeOf(creditUI);
                creditUI.pszMessageText = infoUI.Message;
                creditUI.pszCaptionText = infoUI.Caption;
                if (infoUI.Banner != null)
                {   //create GDI Bitmap handle
                    creditUI.hbmBanner = infoUI.Banner.GetHbitmap();
                }
                result = CredUIPromptForCredentialsW(ref creditUI, new System.Text.StringBuilder(targetName), IntPtr.Zero, netError, user, user.Capacity, pwd, pwd.Capacity, ref saveCredentials, flags);
                if (infoUI.Banner != null)
                {   //destroy GDI Bitmap handle
                    DeleteObject(creditUI.hbmBanner);
                }
                retSave = Convert.ToBoolean(saveCredentials);
                retUserName = user.ToString();
                retPassword = pwd.ToString();              
            });

            return (result, retUserName, retPassword, retSave);
        }

        /// <summary>
        /// Parse username
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPart"></param>
        /// <param name="domainPart"></param>
        /// <returns></returns>
        public static ResultCode ParseUserName(string userName, out string userPart, out string domainPart)
        {
            StringBuilder user = new StringBuilder(MAX_USER_NAME);
            StringBuilder domain = new StringBuilder(MAX_DOMAIN);
            ResultCode result = CredUIParseUserNameW(userName, user, MAX_USER_NAME, domain, MAX_DOMAIN);
            userPart = user.ToString();
            domainPart = domain.ToString();
            return result;
        }

        /// <summary>
        /// Confirm credentials (true = success, false = failure)
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        public static ResultCode ConfirmCredentials(string targetName, bool confirm)
        {
            return CredUIConfirmCredentialsW(new System.Text.StringBuilder(targetName), confirm);
        }

        //Private Declare Unicode _
        [DllImport("credui", CharSet = CharSet.Unicode)] //Lib "credui" Alias "CredUIPromptForCredentialsW"
        private static extern ResultCode CredUIPromptForCredentialsW
            (ref _Info creditUR, System.Text.StringBuilder targetName, IntPtr reserved1, int iError,
            System.Text.StringBuilder userName, int maxUserName, System.Text.StringBuilder password, int maxPassword,
            ref int iSave, SystemFlags flags);

        //Private Declare Unicode
        [DllImport("credui", CharSet = CharSet.Unicode)] //Lib "credui" Alias "CredUIParseUserNameW"
        private static extern ResultCode CredUIParseUserNameW(string userName, StringBuilder user, int userMaxChars, StringBuilder domain, int domainMaxChars);

        //Private Declare Unicode
        [DllImport("credui", CharSet = CharSet.Unicode)] //Lib "credui" Alias "CredUIConfirmCredentialsW"
        private static extern ResultCode CredUIConfirmCredentialsW(System.Text.StringBuilder targetName, bool confirm);

        //Public Declare Auto
        [DllImport("Gdi32", CharSet = CharSet.Unicode)/*, CharSet.Auto*/] //Lib "Gdi32"
        private static extern bool DeleteObject(IntPtr hObject);


        [Flags]
        public enum SystemFlags
        {
            INCORRECT_PASSWORD = 0x0001,
            DO_NOT_PERSIST = 0x0002,
            REQUEST_ADMINISTRATOR = 0x0004,
            EXCLUDE_CERTIFICATES = 0x0008,
            REQUIRE_CERTIFICATE = 0x0010,
            SHOW_SAVE_CHECK_BOX = 0x0040,
            ALWAYS_SHOW_UI = 0x0080,
            REQUIRE_SMARTCARD = 0x0100,
            PASSWORD_ONLY_OK = 0x0200,
            VALIDATE_USERNAME = 0x0400,
            COMPLETE_USERNAME = 0x0800,
            PERSIST = 0x1000,
            SERVER_CREDENTIAL = 0x4000,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
            USERNAME_TARGET_CREDENTIALS = 0x80000,
            KEEP_USERNAME = 0x100000
        }

        public enum ResultCode
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004
        }

        public class Info
        {
            /// <summary>
            /// Message text
            /// </summary>
            public string Message;
            /// <summary>
            /// Window caption
            /// </summary>
            public string Caption;
            /// <summary>
            /// Custom Bitmap 320 x 60 px.
            /// </summary>
            public Bitmap Banner = null;

            public Info(string message, string caption) : base()
            {
                this.Message = message;
                this.Caption = caption;
            }
        }

        private struct _Info
        {
            public int cbSize;
            public IntPtr hwndParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

    }

}
