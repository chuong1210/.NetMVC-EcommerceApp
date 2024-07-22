using System.Security.Claims;
using DoAnLapTrinhWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
	public class UserController : Controller
	{
		private readonly ShopApp2024Context db;

		public UserController(ShopApp2024Context context)
		{
			db = context;
		}

		[Authorize] // Yêu cầu người dùng đăng nhập
		public IActionResult Profile()
		{
			// Lấy thông tin người dùng hiện tại từ HttpContext.User
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = db.KhachHangs.Find(userId);

			return View(user);
		}

		// Các action ChangePassword, Orders tương tự
	}
}