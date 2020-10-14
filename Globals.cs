using Nager.PublicSuffix;

namespace TCAdminSubdomain
{
    public static class Globals
    {
        public const string ModuleId = "ceb0b7e0-59f6-4290-991d-b766b30f1ff1";
        public static readonly DomainParser DomainParser = new DomainParser(new WebTldRuleProvider());
    }
}