namespace Codx.Auth.Configuration
{
    /// <summary>
    /// Configuration settings for authentication
    /// </summary>
    public class AuthenticationSettings
    {
        public const string SectionName = "Authentication";
        
        /// <summary>
        /// Whether email verification is required before users can log in
        /// </summary>
        public bool RequireEmailVerification { get; set; } = true;
        
        /// <summary>
        /// Google OAuth settings
        /// </summary>
        public GoogleAuthSettings Google { get; set; } = new();
        
        /// <summary>
        /// Facebook OAuth settings
        /// </summary>
        public FacebookAuthSettings Facebook { get; set; } = new();
        
        /// <summary>
        /// Microsoft OAuth settings
        /// </summary>
        public MicrosoftAuthSettings Microsoft { get; set; } = new();
    }
}
