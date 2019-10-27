namespace ClientManagerService.Model
{
    /// <summary>
    /// Client Manager versions info.
    /// </summary>
    public class ClientManagerInfo
    {
        /// <summary>
        /// Latest available Client Manager version.
        /// </summary>
        public string Latest { get; set; }

        /// <summary>
        /// Current Client Manager version. 
        /// </summary>
        public string Local { get; set; }
    }
}
