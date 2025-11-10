// modified DeleteConfirmed to only soft-delete (set status = Inactive)
// replace existing DeleteConfirmed implementation with this:
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(string id)
{
    if (id == null) return NotFound();

    var product = _productService.GetProductById(id);
    if (product == null) return NotFound();

    // Soft-delete: set status to "Inactive" via service/repository.
    // Do NOT delete uploaded files here so files stay in storage.
    _productService.DeleteProduct(id);
    return RedirectToAction(nameof(Index));
}