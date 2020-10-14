using RestSharp;
using RestSharp.Authenticators;
using TCAdminSubdomain.Configurations;
using TCAdminSubdomain.Models.Objects;

namespace TCAdminSubdomain.Dns.Clients
{
    public class Dynv6Client
    {
        public readonly RestClient RestClient;
        private const string BaseUrl = "https://dynv6.com/api/v2";
        private readonly DnsProviderType _dnsProviderType = new DnsProviderType(2);
        private readonly Dynv6Configuration _configuration;

        public Dynv6Client()
        {
            RestClient = new RestClient(BaseUrl);
            _configuration = _dnsProviderType.Configuration.Parse<Dynv6Configuration>();
            RestClient.Authenticator = new JwtAuthenticator(_configuration.Token);
        }
    }
}