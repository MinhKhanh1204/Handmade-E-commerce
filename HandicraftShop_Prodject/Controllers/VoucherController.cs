using Microsoft.AspNetCore.Mvc;
using Services;

namespace HandicraftShop_Prodject.Controllers
{
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // UC_13: View vouchers - Xem danh sách voucher
        public IActionResult Index()
        {
            var vouchers = _voucherService.GetActiveVouchers();
            return View(vouchers);
        }

        // UC_14: View voucher detail - Xem chi tiết voucher
        public IActionResult Detail(int id)
        {
            var voucher = _voucherService.GetVoucherById(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }
    }
}

