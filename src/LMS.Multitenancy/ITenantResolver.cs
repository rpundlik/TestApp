using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LMS.Multitenancy
{
    public interface ITenantResolver<TTenant>
    {
        Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);
    }
}