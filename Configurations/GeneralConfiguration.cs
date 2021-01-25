using System.ComponentModel;
using TCAdminSubdomain.Helpers;

namespace TCAdminSubdomain.Configurations
{
    public class GeneralConfiguration
    {
        public string AllowedGameVariable { get; set; } = "SD::ALLOW";
        public string Prefix { get; set; } = "";
        public string Postfix { get; set; } = "";
        public int MinimumLength { get; set; } = 4;
        public int MaximumLength { get; set; } = 12;
        public AfterServiceAction AfterServiceMoveAction { get; set; } = AfterServiceAction.SetSubDomainToNewIpAddress;
        public AfterServiceAction AfterServiceDeletionAction { get; set; } = AfterServiceAction.DeleteSubDomain;
        public string IllegalSubdomains { get; set; } = Constants.DefaultIllegalSubdomains;
    }

    public enum AfterServiceAction
    {
        [Description("Set Sub Domain to new IP Address")]
        SetSubDomainToNewIpAddress,
        [Description("Delete Sub Domain")]
        DeleteSubDomain,
        [Description("Do Nothing")]
        DoNothing
    }
}