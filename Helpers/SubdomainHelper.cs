using System.Collections.Generic;
using System.Linq;
using Alexr03.Common.TCAdmin.Configuration;
using Alexr03.Common.TCAdmin.Permissions;
using Kendo.Mvc.UI;
using TCAdmin.GameHosting.SDK.Objects;
using TCAdminSubdomain.Configurations;
using TCAdminSubdomain.Dns;
using TCAdminSubdomain.Exceptions;
using TCAdminSubdomain.Models.Objects;

namespace TCAdminSubdomain.Helpers
{
    public static class SubdomainHelper
    {
        public static void CreateSubdomainForService(DnsProvider dnsProvider, Service service, string subdomain,
            string baseDomain)
        {
            subdomain = ParseSubdomain(subdomain);
            if (IsIllegal(subdomain))
            {
                throw new SubdomainException($"<strong>Illegal Subdomain - {subdomain}</strong>");
            }

            if (!dnsProvider.ZoneExists(baseDomain, ZoneSearchType.Id))
            {
                throw new SubdomainException($"<strong>Invalid Zone</strong>");
            }

            var zone = dnsProvider.GetZone(baseDomain, ZoneSearchType.Id);
            if (!dnsProvider.SubdomainAvailable(zone, subdomain))
            {
                throw new SubdomainException($"<strong>{subdomain}.{zone.Name}</strong> is already taken!");
            }

            var created = dnsProvider.AddZone(zone, subdomain, service.PublicIpAddress);
            if (created)
            {
                using (var securityBypass = new SecurityBypass(service))
                {
                    service.Variables["SD-FullSubDomain"] = $"{subdomain}.{zone.Name}";
                    service.Variables["SD-ZoneId"] = $"{zone.Id}";
                    service.Variables["SD-DnsProvider"] = dnsProvider.DnsProviderType.DnsProviderId;
                    service.Save();
                }

                return;
            }

            throw new SubdomainException($"Failed to create <strong>{subdomain}.{zone.Name}</strong> subdomain.");
        }

        public static void DeleteSubdomainForService(DnsProvider dnsProvider, Service service)
        {
            var zone = dnsProvider.GetZone(service.Variables["SD-ZoneId"].ToString(), ZoneSearchType.Id);
            var domainName = Globals.DomainParser.Get(service.Variables["SD-FullSubDomain"].ToString());
            var removed = dnsProvider.RemoveZone(zone, domainName.SubDomain);
            if (removed)
            {
                using (var securityBypass = new SecurityBypass(service))
                {
                    service.Variables.RemoveValue("SD-FullSubDomain");
                    service.Variables.RemoveValue("SD-ZoneId");
                    service.Variables.RemoveValue("SD-DnsProvider");
                    service.Save();
                }

                return;
            }

            throw new SubdomainException($"Failed to delete <strong>{domainName.Hostname}</strong> subdomain.");
        }

        public static string ParseSubdomain(string subdomain)
        {
            return subdomain.Replace(" ", "").Trim();
        }

        public static bool IsIllegal(string subdomain)
        {
            var generalConfiguration = new DatabaseConfiguration<GeneralConfiguration>(Globals.ModuleId, "GeneralConfiguration")
                .GetConfiguration();
            if (subdomain.Length < generalConfiguration.MinimumLength ||
                subdomain.Length > generalConfiguration.MaximumLength)
            {
                return true;
            }

            var illegalSubdomains = generalConfiguration.IllegalSubdomains.Split(',');
            return illegalSubdomains.Select(x => x.Trim().ToLower())
                .Any(illegalSubdomain => illegalSubdomain.Equals(subdomain));
        }

        public static List<DropDownListItem> GetBaseDomains()
        {
            return (from dnsProvider in DnsProviderType.GetDnsProviders()
                let config = dnsProvider.DnsProviderType.DnsProviderConfiguration
                where config.Enabled
                from selectListItem in dnsProvider.GetAllowedZones()
                select new DropDownListItem {Selected = false, Text = selectListItem.Text, Value = selectListItem.Value}).ToList();
        }
    }
}