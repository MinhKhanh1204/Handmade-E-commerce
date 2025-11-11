using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApprovalController : Controller
    {
        private readonly IApprovalService _service;
        private readonly IHubContext<ApprovalHub> _hubContext;

        public ApprovalController(IApprovalService service, IHubContext<ApprovalHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // 📋 Hiển thị danh sách chờ phê duyệt
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _service.GetPendingCategoriesAsync();
            var products = await _service.GetPendingProductsAsync();
            var articles = await _service.GetPendingArticlesAsync();
            var vouchers = await _service.GetPendingVouchersAsync();

            var model = new
            {
                Categories = categories,
                Products = products,
                Articles = articles,
                Vouchers = vouchers
            };

            return View(model); // => Views/Approval/Index.cshtml
        }

        // ✅ PHÊ DUYỆT
        [HttpPost]
        public async Task<IActionResult> Approve(string type, string id)
        {
            var user = User.Identity?.Name ?? "System";
            await _service.ApproveAsync(type, id, user);

            TempData["Message"] = $"{type} #{id} approved successfully!";
            return RedirectToAction(nameof(Index));
        }


        // ❌ TỪ CHỐI
        [HttpPost]
        public async Task<IActionResult> Reject(string type, string id)
        {
            var user = User.Identity?.Name ?? "System";
            await _service.RejectAsync(type, id, user);

            await _hubContext.Clients.All.SendAsync("ReceiveApproval", type, id, "Rejected", user);
            TempData["Message"] = $"{type} #{id} rejected.";
            return RedirectToAction(nameof(Index));
        }

    }
}

