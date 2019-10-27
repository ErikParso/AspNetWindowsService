namespace ClientManagerService.Model
{
    /// <summary>
    /// Client Manager Settings model.
    /// Model to file ClientManagerSetting.json
    /// </summary>
    public class ClientManagerSettings
    {
        /// <summary>
        /// Auto actualization interval in minutes.
        /// </summary>
        public int AutoActualizationIntervalMin { get; set; } = 5;
    }
}
