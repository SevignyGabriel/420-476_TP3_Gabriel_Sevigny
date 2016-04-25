using System.Web;
using System.Web.Mvc;

namespace _420_476_TP3_Gabriel_Sevigny
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
