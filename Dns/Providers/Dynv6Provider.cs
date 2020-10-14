using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using TCAdminSubdomain.Dns.Clients;
using TCAdminSubdomain.Models;
using TCAdminSubdomain.Models.Dynv6;

namespace TCAdminSubdomain.Dns.Providers
{
    public class Dynv6Provider : DnsProvider
    {
        public Dynv6Client Client;

        public Dynv6Provider()
        {
            Client = new Dynv6Client();
        }

        public override string Name { get; set; } = "Dynv6";

        public override bool AddZone(DnsZone zone, string subdomain, string ipAddress)
        {
            var request = new RestRequest("zones");
            request.AddParameter("name", $"{subdomain}.{zone.Id}");
            request.AddParameter("ipv4address", $"{ipAddress}");
            var restResponse = Client.RestClient.Post(request);
            return restResponse.IsSuccessful;
        }

        public override bool RemoveZone(DnsZone zone, string subdomain)
        {
            var dynv6Zone = GetRegisteredZones().FirstOrDefault(x =>
                string.Equals(x.Name, $"{subdomain}.{zone.Id}", StringComparison.CurrentCultureIgnoreCase));
            if (dynv6Zone == null)
            {
                return false;
            }

            var deleteRequest = new RestRequest($"zones/{dynv6Zone.Id}");
            var restResponse2 = Client.RestClient.Delete(deleteRequest);

            return restResponse2.IsSuccessful;
        }

        public override IEnumerable<DnsZone> GetZones()
        {
            var dnsZones = new List<DnsZone>
            {
                new DnsZone {Id = "dns.army", Name = "dns.army"},
                new DnsZone {Id = "dns.navy", Name = "dns.navy"},
                new DnsZone {Id = "dynv6.net", Name = "dynv6.net"},
                new DnsZone {Id = "v6.army", Name = "v6.army"},
                new DnsZone {Id = "v6.navy", Name = "v6.navy"},
                new DnsZone {Id = "v6.rocks", Name = "v6.rocks"},
            };

            return dnsZones;
        }

        public override bool SubdomainAvailable(DnsZone zone, string subdomain)
        {
            return !GetRegisteredZones().Any(x =>
                string.Equals(x.Name, $"{subdomain}.{zone.Id}", StringComparison.CurrentCultureIgnoreCase));
        }

        private List<Dynv6Zone> GetRegisteredZones()
        {
            var request = new RestRequest("zones");
            return Client.RestClient.Get<List<Dynv6Zone>>(request).Data;
        }
    }
}