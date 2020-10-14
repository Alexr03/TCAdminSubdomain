using System.Collections.Generic;

namespace TCAdminSubdomain.Configurations
{
    public class DnsProviderConfiguration
    {
        public bool Enabled { get; set; } = false;
        
        public List<string> AllowedDomains { get; set; } = new List<string>();
    }
}