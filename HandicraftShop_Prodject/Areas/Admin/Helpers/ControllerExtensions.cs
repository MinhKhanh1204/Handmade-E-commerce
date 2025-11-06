using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HandicraftShop_Prodject.Areas.Admin.Helpers
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(
            this Controller controller,
            string viewName,
            TModel model,
            bool partial = false)
        {
            controller.ViewData.Model = model;
            await using var writer = new StringWriter();
            var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            var viewResult = viewEngine?.FindView(controller.ControllerContext, viewName, !partial);
            if (viewResult == null || !viewResult.Success)
                throw new FileNotFoundException($"View '{viewName}' not found.");

            var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
                                              controller.TempData, writer, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }
}