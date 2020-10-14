using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TCAdminSubdomain.Models;
using TCAdminSubdomain.Models.Objects;

namespace TCAdminSubdomain.Dns
{
    public abstract class DnsProvider
    {
        public abstract string Name { get; set; }
        
        public abstract bool AddZone(DnsZone zone, string subdomain, string ipAddress);

        public abstract bool RemoveZone(DnsZone zone, string subdomain);

        public abstract IEnumerable<DnsZone> GetZones();

        public abstract bool SubdomainAvailable(DnsZone zone, string subdomain);

        public virtual DnsZone GetZone(string searchFor, ZoneSearchType searchType)
        {
            switch (searchType)
            {
                case ZoneSearchType.Id:
                    return GetZones().FirstOrDefault(x => x.Id == searchFor);
                case ZoneSearchType.Name:
                    return GetZones().FirstOrDefault(x => x.Name == searchFor);
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
            }
        }

        public virtual bool ZoneExists(string baseDomain, ZoneSearchType searchType)
        {
            try
            {
                return GetZone(baseDomain, searchType) != null;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool ReplaceZone(string oldName, DnsZone oldZone, string newName, DnsZone newZone,
            string ipAddress)
        {
            return RemoveZone(oldZone, oldName) && AddZone(newZone, newName, ipAddress);
        }

        public DnsProviderType DnsProviderType => DnsProviderType.GetDnsProviderTypeByName(this.GetType().ToString());

        public List<SelectListItem> GetAvailableZones()
        {
            var configuration = DnsProviderType.DnsProviderConfiguration;
            var selectListItems = this.GetZones().Select(x => new SelectListItem
            {
                Disabled = false,
                Selected = false,
                Text = x.Name,
                Value = x.Id
            }).ToList();
            selectListItems.RemoveAll(x => configuration.AllowedDomains.Contains(x.Value));
            return selectListItems;
        }

        public List<SelectListItem> GetAllowedZones()
        {
            var configuration = DnsProviderType.DnsProviderConfiguration;
            var selectListItems = this.GetZones().Where(x => configuration.AllowedDomains.Contains(x.Id))
                .Select(x => new SelectListItem
                {
                    Disabled = false,
                    Selected = false,
                    Text = x.Name,
                    Value = x.Id
                }).ToList();
            return selectListItems;
        }

        public static DnsProvider GetDnsProvider(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new Exception("Unknown type.");
            }

            return Activator.CreateInstance(type) as DnsProvider;
        }

        public static DnsProvider GetDnsProviderFromBaseDomain(string baseDomain, ZoneSearchType zoneSearchType)
        {
            var dnsProviders = DnsProviderType.GetDnsProviders();
            return dnsProviders.FirstOrDefault(dnsProvider => dnsProvider.ZoneExists(baseDomain, zoneSearchType));
        }
    }

    public enum ZoneSearchType
    {
        Id,
        Name
    }
}