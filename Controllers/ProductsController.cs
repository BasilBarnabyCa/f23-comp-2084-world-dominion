using Microsoft.AspNetCore.Mvc;

namespace WorldDominion.Controllers
{
	public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}