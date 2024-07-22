using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
	[Authorize(Roles = "Admin")] // Chỉ cho phép người dùng thuộc vai trò "Admin" truy cập
	public class AdminController : Controller
	{
		private readonly ShopApp2024Context db;

		public AdminController(ShopApp2024Context context)
		{
			db = context;
		}

		public IActionResult Index()
		{
			var users = db.KhachHangs.ToList();
			return View(users);
		}

		public IActionResult Dashboard()
		{
			return View();
		}

	}
}