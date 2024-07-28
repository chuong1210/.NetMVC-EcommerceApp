using System.Security.Claims;
using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.Helper;
using DoAnLapTrinhWeb.Service;
using DoAnLapTrinhWeb.Services;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace DoAnLapTrinhWeb.Controllers
{
	public class CartController : Controller
	{
        private readonly PaypalService _paypal;
        private readonly ShopApp2024Context db;
        private readonly IVNPayService _vnpayService;

        public CartController(ShopApp2024Context context,PaypalService paypal,IVNPayService vnpayService)
        {
            _paypal=paypal;
			db = context;
            _vnpayService = vnpayService;
        }
		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();
		[Authorize]
        public IActionResult Index()
		{
			return View(Cart);
		}

		// [Authorize(Roles = "User")]
		//[Authorize]
		[Authorize(Policy="UserPolicy")]
        public IActionResult AddToCart(int id,int quantity=1)
		
	{
            var user = User;
            if (!user.IsInRole("User"))
            {
                // Trả về lỗi hoặc chuyển hướng đến trang không được phép
                return Unauthorized();
            }
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

            HttpContext.Session.Set(StaticMethod.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(StaticMethod.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

		[Authorize]
		[HttpGet]
        public IActionResult CheckOut()
        {
			if(Cart.Count==0)
			{
				return Redirect("/"); // chuyển đến 1 url cụ thể
			}
            ViewBag.PaypalClientId = _paypal.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CheckOut(CheckOutVM model,string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                if (payment == "Thanh toán VNPay")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = Cart.Sum(p => p.ThanhTien),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.HoTen} {model.DienThoai}",
                        FullName = model.HoTen,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnpayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                var customerId =User.
                    Claims.SingleOrDefault(p => p.Type == StaticMethod.CLAIM_CUSTOMERID).Value;
             //  ClaimTypes.NameIdentifier
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }
                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    SoDienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };
				db.Database.BeginTransaction();
				try
				{
					db.Database.CommitTransaction();
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }

                    db.SaveChanges();
                    HttpContext.Session.Set<List<CartItem>>(StaticMethod.CART_KEY,new List<CartItem>());
                    return View("Succcess");

                }
                catch
				{
                    db.Database.RollbackTransaction();

                }

            }
            return View(Cart);
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Thông tin đơn hàng gửi qua Paypal
            var tongTien = Cart.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypal.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypal.CaptureOrder(orderID);

                // Lưu database đơn hàng của mình

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }
        // call back giao dich
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnpayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }


            // Lưu đơn hàng vô database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
       
	}
}
