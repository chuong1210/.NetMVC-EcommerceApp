using AutoMapper;
using DoAnLapTrinhWeb.Data;
using DoAnLapTrinhWeb.ViewModels;

namespace DoAnLapTrinhWeb.Helper
{
	public class AutoMapperProfile:Profile
	{
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, KhachHang>()
                .ForMember(kh=>kh.HoTen,option=>
                option.MapFrom(RegisterVM=>RegisterVM.HoTen)).ReverseMap();
        }
    }
}
