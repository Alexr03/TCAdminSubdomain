using Alexr03.Common.TCAdmin.Configuration;
using Alexr03.Common.TCAdmin.Extensions;
using TCAdmin.GameHosting.SDK.Objects;
using TCAdmin.SDK.Integration;
using TCAdmin.SDK.Objects;
using TCAdminSubdomain.Configurations;
using TCAdminSubdomain.Dns;
using TCAdminSubdomain.Helpers;
using TCAdminSubdomain.Models.Objects;
using Service = TCAdmin.GameHosting.SDK.Objects.Service;

namespace TCAdminSubdomain.Events.Commands
{
    public class ServiceCommands : CommandBase
    {
        private readonly GeneralConfiguration _configuration =
            new DatabaseConfiguration<GeneralConfiguration>(Globals.ModuleId, "GeneralConfiguration")
                .GetConfiguration();

        public override CommandResponse ProcessCommand(object sender, IntegrationEventArgs args)
        {
            if (args.Command != "AfterMove")
            {
                return new CommandResponse(Globals.ModuleId, ReturnStatus.Ok);
            }

            var service = (Service) sender;
            var subdomainSet = service.Variables.HasValueAndSet("SD-FullSubDomain");
            if (!subdomainSet)
            {
                return new CommandResponse(Globals.ModuleId, "Subdomain was not set.",
                    ReturnStatus.SafeError);
            }

            if (args.Command == ServiceEvent.AfterMove.ToString())
            {
                return AfterMove(service);
            }

            if (args.Command == ServiceEvent.AfterDelete.ToString())
            {
                return AfterDelete(service);
            }

            return new CommandResponse(Globals.ModuleId, "Unknown Service Event",
                ReturnStatus.SafeError);
        }

        private CommandResponse AfterMove(Service service)
        {
            if (_configuration.AfterServiceMoveAction == AfterServiceAction.DoNothing)
            {
                return new CommandResponse(Globals.ModuleId, "Doing nothing.", ReturnStatus.SafeError);
            }

            var domainName = Globals.DomainParser.Get(service.Variables["SD-FullSubDomain"].ToString());
            var dnsProvider = new DnsProviderType(int.Parse(service.Variables["SD-DnsProvider"].ToString()))
                .DnsProvider;
            var zoneExists = dnsProvider.ZoneExists(domainName.RegistrableDomain, ZoneSearchType.Name);
            if (!zoneExists)
            {
                return new CommandResponse(Globals.ModuleId, "Zone does not exist", ReturnStatus.SafeError);
            }

            var dnsZone = dnsProvider.GetZone(domainName.RegistrableDomain, ZoneSearchType.Name);
            switch (_configuration.AfterServiceMoveAction)
            {
                case AfterServiceAction.DeleteSubDomain:
                    SubdomainHelper.DeleteSubdomainForService(dnsProvider, service);
                    break;
                case AfterServiceAction.SetSubDomainToNewIpAddress:
                    SubdomainHelper.DeleteSubdomainForService(dnsProvider, service);
                    SubdomainHelper.CreateSubdomainForService(dnsProvider, service, domainName.SubDomain, dnsZone.Id);
                    break;
            }

            return new CommandResponse(Globals.ModuleId, "Successfully reset subdomain",
                ReturnStatus.Ok);
        }

        private CommandResponse AfterDelete(Service service)
        {
            if (_configuration.AfterServiceDeletionAction == AfterServiceAction.DoNothing)
            {
                return new CommandResponse(Globals.ModuleId, "Doing nothing.",
                    ReturnStatus.SafeError);
            }

            var domainName = Globals.DomainParser.Get(service.Variables["SD-FullSubDomain"].ToString());
            var dnsProvider = new DnsProviderType(int.Parse(service.Variables["SD-DnsProvider"].ToString()))
                .DnsProvider;
            var zoneExists = dnsProvider.ZoneExists(domainName.RegistrableDomain, ZoneSearchType.Name);
            if (!zoneExists)
            {
                return new CommandResponse(Globals.ModuleId, "Zone does not exist",
                    ReturnStatus.SafeError);
            }

            var dnsZone = dnsProvider.GetZone(domainName.RegistrableDomain, ZoneSearchType.Name);

            switch (_configuration.AfterServiceDeletionAction)
            {
                case AfterServiceAction.DeleteSubDomain:
                    SubdomainHelper.DeleteSubdomainForService(dnsProvider, service);
                    break;
                case AfterServiceAction.SetSubDomainToNewIpAddress:
                    SubdomainHelper.DeleteSubdomainForService(dnsProvider, service);
                    SubdomainHelper.CreateSubdomainForService(dnsProvider, service, domainName.SubDomain, dnsZone.Id);
                    break;
            }

            return new CommandResponse(Globals.ModuleId, "Successfully reset subdomain",
                ReturnStatus.Ok);
        }
    }
}