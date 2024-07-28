using DoAnLapTrinhWeb.ViewModels;

namespace DoAnLapTrinhWeb.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
