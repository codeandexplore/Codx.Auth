namespace Codx.Auth.Configuration
{
    public class ThemeOptions
    {
        public const string SectionName = "Theme";

        /// <summary>
        /// Master toggle. When false, all theming is bypassed and the source UI is used.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Theme folder name under /Themes (for view overrides) and /wwwroot/themes (for static assets).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display name shown in the navbar brand. Falls back to "Codx.Auth" when empty.
        /// </summary>
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// Path to the brand icon (relative to wwwroot). Example: "/themes/custom/brand.svg".
        /// Leave empty to show no icon.
        /// </summary>
        public string BrandIconPath { get; set; } = string.Empty;

        /// <summary>
        /// Returns true when the theme is fully configured and should be applied.
        /// </summary>
        public bool IsActive => Enabled && !string.IsNullOrWhiteSpace(Name);
    }
}
