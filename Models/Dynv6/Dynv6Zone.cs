using System;
using Newtonsoft.Json;

namespace TCAdminSubdomain.Models.Dynv6
{
    public class Dynv6Zone
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ipv4address")]
        public string Ipv4Address { get; set; }

        [JsonProperty("ipv6prefix")]
        public string Ipv6Prefix { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}