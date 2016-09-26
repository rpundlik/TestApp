using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Multitenancy;
using LMS.Model;


namespace LMS.Web.Core
{
    public class CachingAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;

        public CachingAppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            this.tenants = options.Value.Tenants;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();

        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.Hostnames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            String strPath = String.Empty;

            TenantContext<AppTenant> tenantContext = context.Session.GetObjectFromJson<TenantContext<AppTenant>>("TenantContext");

            if (context.Request.Path.Value.ToString().ToLower().Contains("/account/login")|| context.Request.Path.Value.ToString().ToLower().Contains("/admin/login"))
            {
                if (!context.Request.Query.ContainsKey("ReturnUrl"))
                    tenantContext = null;
            }

            if (tenantContext == null)
            {
                strPath = context.Request.Path.Value;

                if (strPath.StartsWith("/"))
                {
                    strPath = context.Request.Path.Value.Remove(context.Request.Path.Value.IndexOf("/"), 1);
                }

                if (strPath.EndsWith("/"))
                {
                    strPath = strPath.Remove(strPath.LastIndexOf("/"), 1);
                }

                var tenant = tenants.FirstOrDefault(t => t.Hostnames.Any(h => h.Equals(strPath, StringComparison.OrdinalIgnoreCase)));

                if (tenant != null)
                {
                    tenantContext = new TenantContext<AppTenant>(tenant);
                    context.Session.SetObjectAsJson("TenantContext", tenantContext);
                }
                else
                {
                    tenant = new AppTenant { ClientName = "DefaultTenant", Hostnames = new string[0] };
                    tenantContext = new TenantContext<AppTenant>(tenant);
                }

                //if (tenant == null)
                //{
                //    tenant = new AppTenant { ClientName = "DefaultTenant", Hostnames = new string[0] };
                //}

                //tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }
    }
}
