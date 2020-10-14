using Nager.PublicSuffix;

namespace TCAdminSubdomain
{
    public class Globals
    {
        public const string ModuleId = "d3b2aa93-7e2b-4e0d-8080-67d14b2fa8a9";
        public static readonly DomainParser DomainParser = new DomainParser(new WebTldRuleProvider());
    }
}