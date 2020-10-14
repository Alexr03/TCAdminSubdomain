using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentCloudflare;
using FluentCloudflare.Abstractions.Builders;
using FluentCloudflare.Api;
using FluentCloudflare.Api.Entities;
using FluentCloudflare.Extensions;
using TCAdminSubdomain.Configurations;
using TCAdminSubdomain.Models;
using TCAdminSubdomain.Models.Objects;

namespace TCAdminSubdomain.Dns.Providers
{
    public class CloudFlareProvider : DnsProvider
    {
        private readonly ITokenAuthorizedSyntax _cloudFlareContext;

        public CloudFlareProvider()
        {
            var config = new DnsProviderType(1).Configuration.Parse<CloudFlareConfiguration>();
            _cloudFlareContext = Cloudflare.WithToken(config.Token);
        }

        public override string Name { get; set; } = "CloudFlare";

        public override bool AddZone(DnsZone zone, string subdomain, string ipAddress)
        {
            try
            {
                if (!SubdomainAvailable(zone, subdomain))
                {
                    return false;
                }

                using (var wc = new HttpClient())
                {
                    var zoneSyntax = _cloudFlareContext.Zone(zone.Id);
                    var dnsCreateSyntax = zoneSyntax.Dns.Create(DnsRecordType.A, subdomain, ipAddress);
                    var _ = dnsCreateSyntax.CallAsync(wc).Result;

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public override bool RemoveZone(DnsZone zone, string subdomain)
        {
            using (var wc = new HttpClient())
            {
                var dns = _cloudFlareContext.Zone(zone.Id).Dns;
                var dnsListSyntax = dns.List().CallAsync(wc).Result;
                foreach (var dnsRecord in dnsListSyntax)
                {
                    var name = dnsRecord.Name.Replace(zone.Name, "").Trim('.');
                    if (name == subdomain)
                    {
                        var _ = dns.Delete(dnsRecord).CallAsync(wc).Result;
                    }
                }

                return true;
            }
        }

        public override IEnumerable<DnsZone> GetZones()
        {
            try
            {
                using (var wc = new HttpClient())
                {
                    var zones = _cloudFlareContext.Zones.List().PerPage(50).CallAsync(wc).Result.Select(x => new DnsZone()
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();
                    return zones;
                }
            }
            catch
            {
                return new List<DnsZone>();
            }
        }

        public override bool SubdomainAvailable(DnsZone zone, string subdomain)
        {
            using (var wc = new HttpClient())
            {
                var dns = _cloudFlareContext.Zone(zone.Id).Dns;
                var dnsListSyntax = dns.List().CallAsync(wc).Result;
                if (dnsListSyntax.Select(dnsRecord => dnsRecord.Name.Replace(zone.Name, ""))
                    .Any(name => name.Trim('.') == subdomain.Trim('.')))
                {
                    return false;
                }
            }

            return true;
        }
    }
}