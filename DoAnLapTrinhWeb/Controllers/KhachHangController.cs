using AutoMapper;
using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.Helper;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DoAnLapTrinhWeb.Controllers
{
	/*	[Route("api/[controller]")]
		[ApiController]*/
	//[Authorize(Roles = "User,Admin")]
	public class KhachHangController : Controller
    {
        private readonly ShopApp2024Context db;
		private readonly IMapper _mapper;

		public KhachHangController(ShopApp2024Context context,IMapper mapper)
        {
            db = context;
			_mapper = mapper;

        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
		[HttpPost]
		public IActionResult DangKy(RegisterVM khSU,IFormFile urlImage)
		{
			if(ModelState.IsValid)
			{
				try
				{
					KhachHang kh = _mapper.Map<KhachHang>(khSU);
					kh.RandomKey = MyUtil.GenerateRandomKey();
					kh.MatKhau = khSU.MatKhau.ToMd5Hash(kh.RandomKey);
					kh.HieuLuc = true;
					kh.VaiTro = 0;
					if (urlImage != null)
					{
						kh.Hinh = MyUtil.UploadImage(urlImage, "KhachHang");
				}
					db.Add(kh);
					db.SaveChanges();
					return RedirectToAction("Index", "HangHoa");

				}
				catch (Exception ex)
				{
					var mess = $"{ex.Message} shh";
				}
			}
			
			return View();
		}
		/*
				[HttpGet]
				public string Get()
				{
					return "Hello World!";
				}*/
		[HttpGet]
		public IActionResult DangNhap(string? ReturnUrl = null)
		{
			
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> DangNhap(LoginVM loginVM, string? ReturnUrl = null)
		{
			if (ModelState.IsValid)
			{
				var user = db.KhachHangs.FirstOrDefault(u => u.MaKh == loginVM.UserName);
				if(user == null)
				{
					ModelState.AddModelError("loi", "Tên đăng nhập hoặc mật khẩu không chính xác.");
				}
				
				else
				{
					if(!user.HieuLuc)
					{
						ModelState.AddModelError("loi", "Tài khoản đã không còn hiệu lực.");

					}
					else
					{

						if ( user.MatKhau == loginVM.Password.ToMd5Hash(user.RandomKey))
						{
							var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.HoTen),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim("CustomerID", user.MaKh),

				new Claim(ClaimTypes.Role, user.VaiTro == 0 ? "User" : "Admin") // Assuming VaiTro is the role identifier
            };

							var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
							var authProperties = new AuthenticationProperties
							{
								IsPersistent = true,
								RedirectUri = ReturnUrl ?? "/"
							};

							await HttpContext.SignInAsync(
								CookieAuthenticationDefaults.AuthenticationScheme,
								new ClaimsPrincipal(claimsIdentity),
								authProperties);

						/*	if (user.VaiTro == 0)
							{
								return RedirectToAction("Index", "Home"); // Redirect to user home
							}
							else
							{
								return RedirectToAction("Dashboard", "Admin"); // Redirect to admin home
							}*/

							if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
							{
								return Redirect(ReturnUrl);
								// Quay trở lại trang ban đầu, 2 cách
								//return Redirect(Request.Headers["Referer"].ToString());
							}
							else
							{
								return RedirectToAction("Index", "Home");
							}
						}
						else
						{
							ModelState.AddModelError("loi", "Sai mật khẩu");
						}
					}	
				}
				
			
			}
			return View(loginVM);
		}

		[Authorize]
		public IActionResult Profile()
		{
			return View();
		}

		
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}
		/*[HttpGet]
		public IActionResult Profile()
		{
			// Retrieve the current user's username
			var username = User.Identity.Name;

			// Fetch the user's details from the database
			var user = db.KhachHangs.FirstOrDefault(u => u.MaKh == username);

			if (user == null)
			{
				return NotFound(); // Or redirect to an error page
			}

			// Map user data to a view model if necessary
			var profileViewModel = new ProfileVM
			{
				UserName = user.MaKh,
				Email = user.Email,
				FullName = user.FullName,
				// Add other properties as needed
			};

			return View(profileViewModel);
		}*/


	}

}
