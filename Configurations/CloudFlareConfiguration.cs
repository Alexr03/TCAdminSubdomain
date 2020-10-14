namespace TCAdminSubdomain.Configurations
{
    public class CloudFlareConfiguration : DnsProviderConfiguration
    {
        public string Token { get; set; } = "";
    }
}