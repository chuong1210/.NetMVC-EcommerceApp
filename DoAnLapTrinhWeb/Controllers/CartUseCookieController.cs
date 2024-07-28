using System.Text.Json;
using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.Helper;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
    public class CartUseCookieController : Controller
    {
        private readonly ShopApp2024Context db;

        public CartUseCookieController(ShopApp2024Context context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var user = User;
            if (!user.IsInRole("User"))
            {
                return Unauthorized();
            }

            if (quantity <= 0)
            {
                TempData["messageCart"] = "Số lượng không hợp lệ. Vui lòng chọn số lượng lớn hơn 0.";
                return RedirectToAction("Detail", "HangHoa", new { id });
            }

            var gioHang = GetCartFromCookie();
            var item = gioHang.SingleOrDefault(gh => gh.MaHh == id);

            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(gh => gh.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["message"] = $"Không tìm thấy mã {id} trong giỏ hàng";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            SetCartCookie(gioHang);

            return RedirectToAction("Index");
        }

        private List<CartItem>? GetCartFromCookie()
        {
            var cookieValue = Request.Cookies[StaticMethod.CART_KEY];
            return string.IsNullOrEmpty(cookieValue)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cookieValue);
        }

        private void SetCartCookie(List<CartItem> cart)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7) // Set cookie expiration as needed
            };
            var cookieValue = JsonSerializer.Serialize(cart);
            Response.Cookies.Append(StaticMethod.CART_KEY, cookieValue, cookieOptions);
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = GetCartFromCookie();
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                SetCartCookie(gioHang);
            }
            return RedirectToAction("Index");
        }


    }
}
