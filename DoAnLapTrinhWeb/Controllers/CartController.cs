using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.Helper;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
	public class CartController : Controller
	{
        private readonly ShopApp2024Context db;

        public CartController(ShopApp2024Context context)
        {
			db = context;
        }
		const string CART_KEY = "MY_CART";
		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY)?? new List<CartItem>();
        public IActionResult Index()
		{
			return View(Cart);
		}

		public IActionResult AddToCart(int id,int quantity=1)
		
	{
            if (quantity <= 0)
            {
                TempData["messageCart"] = "Số lượng không hợp lệ. Vui lòng chọn số lượng lớn hơn 0.";
                return RedirectToAction("Detail", "HangHoa", new { id });

            }
            var gioHang = Cart;
			var item = gioHang.SingleOrDefault(gh => gh.MaHh == id);
			if(item==null)
			{
				var hangHoa=db.HangHoas.SingleOrDefault(gh => gh.MaHh == id);
				if(hangHoa==null)
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

            HttpContext.Session.Set(CART_KEY, gioHang);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        // GET: CartController/Details/5
        public ActionResult Details(int id)
		{
			return View();
		}

		// GET: CartController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: CartController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: CartController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: CartController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: CartController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: CartController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
