using System.Collections.ObjectModel;
using LMS.Model;
namespace LMS.Web.Core
{
    public class MultitenancyOptions
    {
        public Collection<AppTenant> Tenants { get; set; }
    }
}
