namespace ClientManagerService.Model
{
    /// <summary>
    /// Validation result.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Whether item in request is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Validation message.
        /// </summary>
        public string Message { get; set; }
    }
}
