namespace CorpoGameApp.Properties
{
    public class ApplicationSettings
    {
        public string SendGridUsername { get; set; }
        public string SendGridPassword { get; set; }
        public string SendGridFromEmail { get; set; }
        public string SendGridFromName { get; set; }
        public string SendGridApiKey { get; internal set; }
    }
}