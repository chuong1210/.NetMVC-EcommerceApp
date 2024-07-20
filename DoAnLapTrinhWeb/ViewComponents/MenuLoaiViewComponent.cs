using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.ViewComponents
{
    public class MenuLoaiViewComponent:ViewComponent
    {
        private readonly ShopApp2024Context db;

        public MenuLoaiViewComponent(ShopApp2024Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(l => new MenuLoaiVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                SoLuong = l.HangHoas.Count
            }).OrderBy(l=>l.TenLoai) ;
            return View(data); //để ko v là defaullt.cshtml
           // return View("Default",data)
        }
    }
}
