using AutoMapper;
using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.Helper;
using DoAnLapTrinhWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
/*	[Route("api/[controller]")]
	[ApiController]*/
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
			if (!ModelState.IsValid)
			{
				foreach (var key in ModelState.Keys)
				{
					var errors = ModelState[key].Errors;
					foreach (var error in errors)
					{
						Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
					}
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
	}
}
