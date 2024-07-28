using System.ComponentModel.DataAnnotations;
namespace DoAnLapTrinhWeb.ViewModels
{
    public class CheckOutVM
    {
      //  [Required(ErrorMessage = "Vui lòng nhập tên người nhận hàng")]
        public string? HoTen { get; set; }
     //   [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        public string? DiaChi { get; set; }
    //    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string? DienThoai { get; set; }
        public string? GhiChu { get; set; }
        public bool GiongKhachHang { get; set; } = false;
    }
}