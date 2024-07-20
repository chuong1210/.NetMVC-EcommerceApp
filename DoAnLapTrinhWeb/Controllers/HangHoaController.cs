using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DoAnLapTrinhWeb.Controllers
{
    public class HangHoaController : Controller
    {
		private readonly ShopApp2024Context db;

		public HangHoaController(ShopApp2024Context context)
        {
            db = context;
        }
        public IActionResult Index(int ? Loai)
        {
            var hanghoas=db.HangHoas.AsQueryable();
            if(Loai.HasValue)
            {
                hanghoas = hanghoas.Where(h =>  h.MaLoai==Loai.Value );
            }
            var rs = hanghoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai

            });
            return View(rs);
        }
        public IActionResult Search(string? query)
        {
			var hanghoas = db.HangHoas.AsQueryable();
			if (query !=null)
			{
				hanghoas = hanghoas.Where(h => h.TenHh.Contains(query));
			}
			var rs = hanghoas.Select(p => new HangHoaVM
			{
				MaHh = p.MaHh,
				TenHH = p.TenHh,
				DonGia = p.DonGia ?? 0,
				Hinh = p.Hinh ?? "",
				MoTaNgan = p.MoTaDonVi ?? "",
				TenLoai = p.MaLoaiNavigation.TenLoai

			}).ToList();
			return View(rs);


		}
		public IActionResult Detail(int id)
		{
			var hanghoa = db.HangHoas.Include(p=>p.MaLoaiNavigation).
				SingleOrDefault(p=>p.MaHh==id);
			if (hanghoa == null)
			{
				TempData["message"] = $"Không tìm thấy sản phẩm có mã ${id}";
				return Redirect("/404");
			}
            var result = new ChiTietHangHoaVM
            {
                MaHh = hanghoa.MaHh,
                TenHH = hanghoa.TenHh,
                DonGia = hanghoa.DonGia ?? 0,
                ChiTiet = hanghoa.MoTa ?? string.Empty,
                Hinh = hanghoa.Hinh ?? string.Empty,
                MoTaNgan = hanghoa.MoTaDonVi ?? string.Empty,
                TenLoai = hanghoa.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,//tính sau
                DiemDanhGia = 5,//check sau
            };
            return View(result);

		}

	}
}
