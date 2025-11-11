using Microsoft.AspNetCore.Mvc;
using Services;
using HandicraftShop_Prodject.Utils;
using BussinessObject;

namespace HandicraftShop_Prodject.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly FeedbackService _feedbackService;
        private readonly OrderService _orderService;

        public FeedbackController(FeedbackService feedbackService, OrderService orderService)
        {
            _feedbackService = feedbackService;
            _orderService = orderService;
        }

        // ✅ Add Feedback (UC_24)
        [HttpPost]
        public async Task<IActionResult> Add(string productId, int rating, string comment)
        {
            var account = AccountUtils.GetUserData(User);

            if (account == null)
            {
                TempData["ErrorMessage"] = "Please login to leave feedback!";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Kiểm tra xem customer đã mua sản phẩm này chưa
                var hasPurchased = await _orderService.HasCustomerPurchasedProductAsync(account.AccountId, productId);

                if (!hasPurchased)
                {
                    TempData["ErrorMessage"] = "You can only review products you have purchased!";
                    return RedirectToAction("Detail", "Product", new { id = productId });
                }

                // Kiểm tra đã feedback chưa
                var existingFeedback = await _feedbackService.GetFeedbackByCustomerAndProductAsync(account.AccountId, productId);
                if (existingFeedback != null)
                {
                    TempData["ErrorMessage"] = "You have already reviewed this product!";
                    return RedirectToAction("Detail", "Product", new { id = productId });
                }

                var feedback = new Feedback
                {
                    ProductId = productId,
                    CustomerId = account.AccountId,
                    Rating = rating,
                    Comment = comment
                };

                await _feedbackService.AddFeedbackAsync(feedback);

                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding feedback: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to submit feedback!";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }
        }

        // ✅ Edit Feedback (UC_25)
        [HttpPost]
        public async Task<IActionResult> Edit(int feedbackId, int rating, string comment)
        {
            var account = AccountUtils.GetUserData(User);

            if (account == null)
            {
                return Unauthorized();
            }

            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(feedbackId);

                if (feedback == null)
                {
                    TempData["ErrorMessage"] = "Feedback not found!";
                    return RedirectToAction("Index", "Product");
                }

                if (feedback.CustomerId != account.AccountId)
                {
                    TempData["ErrorMessage"] = "You can only edit your own feedback!";
                    return RedirectToAction("Detail", "Product", new { id = feedback.ProductId });
                }

                feedback.Rating = rating;
                feedback.Comment = comment;

                await _feedbackService.UpdateFeedbackAsync(feedback);

                TempData["SuccessMessage"] = "Feedback updated successfully!";
                return RedirectToAction("Detail", "Product", new { id = feedback.ProductId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error editing feedback: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to update feedback!";
                return RedirectToAction("Index", "Product");
            }
        }

        // ✅ Delete Feedback (UC_26)
        [HttpPost]
        public async Task<IActionResult> Delete(int feedbackId)
        {
            var account = AccountUtils.GetUserData(User);

            if (account == null)
            {
                return Unauthorized();
            }

            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(feedbackId);

                if (feedback == null)
                {
                    TempData["ErrorMessage"] = "Feedback not found!";
                    return RedirectToAction("Index", "Product");
                }

                if (feedback.CustomerId != account.AccountId)
                {
                    TempData["ErrorMessage"] = "You can only delete your own feedback!";
                    return RedirectToAction("Detail", "Product", new { id = feedback.ProductId });
                }

                var productId = feedback.ProductId;
                await _feedbackService.DeleteFeedbackAsync(feedbackId);

                TempData["SuccessMessage"] = "Feedback deleted successfully!";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting feedback: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to delete feedback!";
                return RedirectToAction("Index", "Product");
            }
        }
    }
}