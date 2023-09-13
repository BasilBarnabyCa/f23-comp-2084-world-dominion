using Microsoft.AspNetCore.Mvc;

namespace WorldDominion
{
	public class DepartmentsController : Controller
	{
		public IActionResult Index()
		{ 
			List<Dictionary<string, object>> departments = new()
			{
					new () { {"ID", 1}, {"Name", "Dry Goods"} },
					new () { {"ID", 2}, {"Name", "Baking"} },
					new () { {"ID", 3}, {"Name", "Meats"} },
			};
			return View(departments);
		}



	}

}