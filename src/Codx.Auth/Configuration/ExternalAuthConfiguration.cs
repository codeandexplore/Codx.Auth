using Microsoft.Extensions.Configuration;

namespace Codx.Auth.Configuration
{
    public class ExternalAuthConfiguration
    {
        public GoogleAuthSettings Google { get; set; } = new();
        public FacebookAuthSettings Facebook { get; set; } = new();
        public MicrosoftAuthSettings Microsoft { get; set; } = new();
    }

    public class GoogleAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool IsConfigured => !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret);
    }

    public class FacebookAuthSettings
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public bool IsConfigured => !string.IsNullOrEmpty(AppId) && !string.IsNullOrEmpty(AppSecret);
    }

    public class MicrosoftAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool IsConfigured => !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret);
    }
}