﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.Multitenancy
{
    public abstract class MemoryCacheTenantResolver<TTenant> : ITenantResolver<TTenant>
    {
        protected readonly IMemoryCache cache;
        protected readonly ILogger log;

        public MemoryCacheTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory)
        {
            Ensure.Argument.NotNull(cache, nameof(cache));
            Ensure.Argument.NotNull(loggerFactory, nameof(loggerFactory));

            this.cache = cache;
            this.log = loggerFactory.CreateLogger<MemoryCacheTenantResolver<TTenant>>();
        }

        protected virtual MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(new TimeSpan(1, 0, 0)); // Default 1 hour lifetime
        }

        protected abstract string GetContextIdentifier(HttpContext context);
        protected abstract IEnumerable<string> GetTenantIdentifiers(TenantContext<TTenant> context);
        protected abstract Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);

        async Task<TenantContext<TTenant>> ITenantResolver<TTenant>.ResolveAsync(HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            // Obtain the key used to identify cached tenants from the current request
            var cacheKey = GetContextIdentifier(context);

            var tenantContext = cache.Get(cacheKey) as TenantContext<TTenant>;

            if (tenantContext == null)
            {
                log.LogDebug("TenantContext not present in cache with key \"{cacheKey}\". Attempting to resolve.", cacheKey);
                tenantContext = await ResolveAsync(context);

                if (tenantContext != null)
                {
                    var tenantIdentifiers = GetTenantIdentifiers(tenantContext);
                    var cacheEntryOptions = CreateCacheEntryOptions();

                    log.LogDebug("TenantContext resolved. Caching with keys \"{tenantIdentifiers}\".", tenantIdentifiers);

                    foreach (var identifier in tenantIdentifiers)
                    {
                        cache.Set(identifier, tenantContext, cacheEntryOptions);
                    }
                }
            }
            else
            {
                log.LogDebug("TenantContext retrieved from cache with key \"{cacheKey}\".", cacheKey);
            }

            return tenantContext;
        }
    }
}
