using System.Collections.Generic;
using System.Linq;
using Alexr03.Common.TCAdmin.Objects;
using TCAdmin.Interfaces.Database;
using TCAdminSubdomain.Configurations;
using TCAdminSubdomain.Dns;

namespace TCAdminSubdomain.Models.Objects
{
    public class DnsProviderType : DynamicTypeBase
    {
        public DnsProviderType() : base("tcmodule_dns_providers")
        {
        }

        public DnsProviderType(int id) : this()
        {
            this.SetValue("id", id);
            this.ValidateKeys();
            if (!this.Find())
            {
                throw new KeyNotFoundException("Cannot find DNS Provider with ID: " + id);
            }
        }

        public int DnsProviderId
        {
            get => this.GetIntegerValue("id");
            set => this.SetValue("id", value);
        }
        
        public DnsProvider DnsProvider => DnsProvider.GetDnsProvider(TypeName);

        public DnsProviderConfiguration DnsProviderConfiguration => Configuration.Parse<DnsProviderConfiguration>();

        public static List<DnsProviderType> GetDnsProviderTypes()
        {
            return new DnsProviderType().GetObjectList(new WhereList()).Cast<DnsProviderType>().ToList();
        }

        public static List<DnsProvider> GetDnsProviders()
        {
            var dnsProviderTypes = GetDnsProviderTypes();
            return dnsProviderTypes
                .Select(x => x.DnsProvider).ToList();
        }

        public static DnsProviderType GetDnsProviderTypeByName(string typeName)
        {
            typeName = $"{typeName}%";
            var whereList = new WhereList
            {
                {"typeName", ColumnOperator.Like, typeName}
            };
            var dnsProviderTypes = new DnsProviderType().GetObjectList(whereList).Cast<DnsProviderType>().ToList();
            if (dnsProviderTypes.Any())
            {
                return dnsProviderTypes[0];
            }

            return null;
        }
    }
}