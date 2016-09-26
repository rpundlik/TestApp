using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Model
{
    public class AppTenant
    {
        public String ClientName { get; set; }
        public String ConnectionString { get; set; }
        public int LanguageID { get; set; }
        public int UserID { get; set; }
        public String Theme { get; set; }
        public string[] Hostnames { get; set; }
        public String LoggedInCulture { get; set; }
        public string VideoFilePath { get; set; }
        public string DefaultCulture { get; set; }
    }
}
