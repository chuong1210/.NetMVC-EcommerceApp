using DoAnLapTrinhWeb.Helper;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.ViewComponents
{
    public class CartViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<CartItem> carts = HttpContext.User.Identity.IsAuthenticated
              ? HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>()
              : new List<CartItem>();
           // var carts = HttpContext.Session.Get <List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();
            CartVM rs = new CartVM
            {
                //  Quantity = carts.Sum(ct => ct.SoLuong),
                 Quantity = carts.Count,

                Total = carts.Sum(ct => ct.ThanhTien),

            };
            return View(rs);
        }
    }
}
