using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Dashboard()
		{
            // Lấy dữ liệu từ database
            var tongSoSanPham = db.HangHoas.Count();
            var tongSoDonHang = db.HoaDons.Count();
            var doanhThu = db.ChiTietHds.Sum(dh => dh.DonGia);

            // Lấy danh sách sản phẩm mới
            var sanPhamMoi = db.HangHoas.OrderByDescending(hh => hh.NgaySx).Take(5).ToList();

            ViewBag.TongSoSanPham = tongSoSanPham;
            ViewBag.TongSoDonHang = tongSoDonHang;
            ViewBag.DoanhThu = doanhThu;
            ViewBag.SanPhamMoi = sanPhamMoi;

            return View();
		}

	}
}