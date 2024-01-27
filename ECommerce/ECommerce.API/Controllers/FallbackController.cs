using ECommerce.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ServiceFilter(typeof(VisitorIpAndActivity))]
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}
