namespace pyttogpanne_api.Services
{
    /// <summary>Builds absolute links back into the website for emails.</summary>
    public static class SiteLinks
    {
        private const string Fallback = "https://www.pyttogpanne.no";
        private const string DefaultLocale = "no";

        public static string BaseUrl(IConfiguration config) =>
            (config["Site:BaseUrl"] ?? Fallback).TrimEnd('/');

        public static string SetPasswordPage(IConfiguration config) =>
            $"{BaseUrl(config)}/{DefaultLocale}/reset-password";
    }
}
