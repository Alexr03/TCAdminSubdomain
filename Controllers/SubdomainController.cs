using System;
using System.Web.Mvc;
using Alexr03.Common.TCAdmin.Configuration;
using Alexr03.Common.TCAdmin.Extensions;
using Alexr03.Common.TCAdmin.Objects;
using Alexr03.Common.Web.Extensions;
using Alexr03.Common.Web.HttpResponses;
using Newtonsoft.Json.Linq;
using TCAdmin.SDK.Web.MVC.Controllers;
using TCAdminSubdomain.Configurations;
using TCAdminSubdomain.Dns;
using TCAdminSubdomain.Exceptions;
using TCAdminSubdomain.Helpers;
using TCAdminSubdomain.Models.Objects;
using Service = TCAdmin.GameHosting.SDK.Objects.Service;

namespace TCAdminSubdomain.Controllers
{
    public class SubdomainController : BaseController
    {
        [HttpPost]
        public ActionResult Create(int id, string subdomain, string baseDomain)
        {
            var service = new Service(id);
            try
            {
                var generalConfiguration = new DatabaseConfiguration<GeneralConfiguration>(Globals.ModuleId, "GeneralConfiguration").GetConfiguration();
                var dnsProvider = DnsProvider.GetDnsProviderFromBaseDomain(baseDomain, ZoneSearchType.Id);
                subdomain = $"{generalConfiguration.Prefix}{subdomain}{generalConfiguration.Postfix}";
                subdomain = Alexr03.Common.TCAdmin.Misc.Strings.VariableReplacement.ReplaceVariables(subdomain, service, TCAdmin.SDK.Session.GetCurrentUser());
                SubdomainHelper.CreateSubdomainForService(dnsProvider, service, subdomain, baseDomain);
                var dnsZone = dnsProvider.GetZone(baseDomain, ZoneSearchType.Id);
                return this.SendSuccess($"Successfully created <strong>{subdomain}.{dnsZone.Name}</strong> subdomain.");
            }
            catch (SubdomainException subdomainException)
            {
                return this.SendException(subdomainException);
            }
        }

        [HttpPost]
        [ParentAction("Create")]
        public ActionResult Delete(int id)
        {
            var service = new Service(id);
            try
            {
                if (!service.Variables.HasValueAndSet("SD-FullSubDomain") || !service.Variables.HasValueAndSet("SD-DnsProvider"))
                    return this.SendError($"{service.GameShortNameAndConnectionInfo} does not have a subdomain set.");
                var domain = Globals.DomainParser.Get(service.Variables["SD-FullSubDomain"].ToString());
                var dnsProvider = new DnsProviderType(int.Parse(service.Variables["SD-DnsProvider"].ToString())).DnsProvider;
                SubdomainHelper.DeleteSubdomainForService(dnsProvider, service);
                return this.SendSuccess($"Successfully deleted <strong>{domain.Hostname}</strong> subdomain.");
            }
            catch (SubdomainException subdomainException)
            {
                return this.SendException(subdomainException);
            }
            catch (Exception exception)
            {
                return this.SendException(exception);
            }
        }

        public ActionResult Configure()
        {
            return View();
        }

        public ActionResult ProviderConfigure(int id)
        {
            var dnsProviderType = DynamicTypeBase.GetCurrent<DnsProviderType>();
            var configurationJObject = (JObject)dnsProviderType.Configuration.Parse<object>();
            var o = configurationJObject.ToObject(dnsProviderType.Configuration.Type);
            ViewData.TemplateInfo = new TemplateInfo
            {
                HtmlFieldPrefix = dnsProviderType.Configuration.Type.Name,
            };
            return PartialView($"{dnsProviderType.Configuration.View}", o);
        }
        
        [HttpPost]
        public ActionResult ProviderConfigure(int id, FormCollection model)
        {
            var dnsProviderType = DynamicTypeBase.GetCurrent<DnsProviderType>();
            var bindModel = model.Parse(ControllerContext, dnsProviderType.Configuration.Type);
            dnsProviderType.Configuration.SetConfiguration(bindModel);
            return PartialView($"{dnsProviderType.Configuration.View}", bindModel);
        }

        public ActionResult GeneralConfigure()
        {
            return PartialView(new DatabaseConfiguration<GeneralConfiguration>(Globals.ModuleId, "GeneralConfiguration").GetConfiguration());
        }

        [HttpPost]
        public ActionResult GeneralConfigure(GeneralConfiguration model)
        {
            new DatabaseConfiguration<GeneralConfiguration>(Globals.ModuleId, "GeneralConfiguration").SetConfiguration(model);
            return new JsonNetResult(new
            {
                Message = "General Configuration successfully saved."
            });
        }
    }
}