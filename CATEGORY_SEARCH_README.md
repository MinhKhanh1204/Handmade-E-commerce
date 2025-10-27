# Category Search Functionality - Admin Panel

## 📋 Tổng Quan
Chức năng tìm kiếm Category dành cho Staff trong Admin Panel của hệ thống Handmade E-commerce.

## 🚀 Tính Năng Đã Implement

### ✅ **Search Functionality**
- **Real-time Search**: Tìm kiếm theo tên category và mô tả
- **Case-insensitive**: Không phân biệt hoa thường
- **Partial Match**: Tìm kiếm theo từ khóa con
- **Clear Search**: Xóa bộ lọc tìm kiếm
- **Search Status**: Hiển thị trạng thái tìm kiếm

### ✅ **UI/UX Features**
- **Search Form**: Form tìm kiếm với placeholder hướng dẫn
- **Search Button**: Nút tìm kiếm với icon
- **Clear Button**: Nút xóa tìm kiếm (chỉ hiện khi có kết quả)
- **Search Badge**: Badge hiển thị từ khóa đang tìm kiếm
- **Empty State**: Thông báo khi không tìm thấy kết quả
- **Responsive Design**: Giao diện responsive

## 🏗️ **Kiến Trúc**

### **Search Flow**
```
User Input → Controller → Service → Repository → Database
     ↓
Search Results → View → User Interface
```

### **Files Modified**

#### **Repository Layer:**
- `Repositories/ICategoryRepository.cs` - Added Search method
- `Repositories/CategoryRepository.cs` - Implemented Search with LINQ

#### **Service Layer:**
- `Services/ICategoryService.cs` - Added SearchCategories method
- `Services/CategoryService.cs` - Implemented SearchCategories

#### **Controller Layer:**
- `HandicraftShop_Prodject/Areas/Admin/Controllers/CategoryController.cs` - Added search parameter

#### **View Layer:**
- `HandicraftShop_Prodject/Areas/Admin/Views/Category/Index.cshtml` - Added search form and UI

## 🎯 **Cách Sử Dụng**

### **1. Truy Cập Search**
```
URL: /Admin/Category
```

### **2. Thực Hiện Tìm Kiếm**

#### **Tìm Kiếm Cơ Bản:**
1. Nhập từ khóa vào ô "Search categories by name or description..."
2. Click nút "Search" hoặc nhấn Enter
3. Kết quả sẽ hiển thị ngay lập tức

#### **Tìm Kiếm Nâng Cao:**
- **Tìm theo tên**: Nhập tên category (ví dụ: "Handmade")
- **Tìm theo mô tả**: Nhập từ khóa trong mô tả (ví dụ: "ceramic")
- **Tìm kết hợp**: Tìm kiếm trong cả tên và mô tả

#### **Xóa Tìm Kiếm:**
1. Click nút "Clear" (màu đỏ)
2. Hoặc click "Clear search" trong kết quả trống

### **3. Kết Quả Tìm Kiếm**

#### **Có Kết Quả:**
- Hiển thị danh sách categories phù hợp
- Badge hiển thị từ khóa tìm kiếm
- Sắp xếp theo tên category

#### **Không Có Kết Quả:**
- Thông báo "No categories found matching [keyword]"
- Nút "Clear search" để xóa bộ lọc
- Icon tìm kiếm để dễ nhận biết

## 🔧 **Technical Details

### **Search Implementation**
```csharp
public IEnumerable<Category> Search(string searchTerm)
{
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
        return GetAll();
    }

    var term = searchTerm.ToLower().Trim();
    
    return _context.Categories
        .Where(c => c.Status != "Deleted" && 
                   (c.CategoryName.ToLower().Contains(term) || 
                    (c.Description != null && c.Description.ToLower().Contains(term))))
        .OrderBy(c => c.CategoryName)
        .ToList();
}
```

### **Controller Logic**
```csharp
public IActionResult Index(string searchTerm)
{
    IEnumerable<Category> categories;
    
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        categories = _categoryService.SearchCategories(searchTerm);
        ViewBag.SearchTerm = searchTerm;
    }
    else
    {
        categories = _categoryService.GetAllCategories();
    }
    
    return View(categories);
}
```

### **Search Features**
- **Case-insensitive**: Sử dụng `.ToLower()`
- **Trim whitespace**: Loại bỏ khoảng trắng thừa
- **Null-safe**: Kiểm tra null cho Description
- **Soft delete aware**: Chỉ tìm trong records chưa bị xóa
- **Ordered results**: Sắp xếp theo CategoryName

## 🎨 **UI Components**

### **Search Form**
```html
<form asp-action="Index" method="get" class="d-flex">
    <div class="input-group">
        <input type="text" name="searchTerm" class="form-control" 
               placeholder="Search categories by name or description..." 
               value="@ViewBag.SearchTerm">
        <button class="btn btn-outline-secondary" type="submit">
            <i class="fas fa-search"></i> Search
        </button>
        @if (!string.IsNullOrEmpty(ViewBag.SearchTerm))
        {
            <a asp-action="Index" class="btn btn-outline-danger">
                <i class="fas fa-times"></i> Clear
            </a>
        }
    </div>
</form>
```

### **Search Status Badge**
```html
@if (!string.IsNullOrEmpty(ViewBag.SearchTerm))
{
    <span class="badge bg-info">
        <i class="fas fa-search"></i> 
        Search results for: "@ViewBag.SearchTerm"
    </span>
}
```

### **Empty State**
```html
@if (!string.IsNullOrEmpty(ViewBag.SearchTerm))
{
    <div class="text-muted">
        <i class="fas fa-search fa-2x mb-2"></i>
        <br>
        No categories found matching "@ViewBag.SearchTerm"
        <br>
        <a asp-action="Index" class="btn btn-link">Clear search</a>
    </div>
}
```

## 🚦 **Search Examples**

### **Tìm Kiếm Theo Tên:**
- Input: "Handmade" → Tìm categories có tên chứa "handmade"
- Input: "Ceramic" → Tìm categories có tên chứa "ceramic"

### **Tìm Kiếm Theo Mô Tả:**
- Input: "traditional" → Tìm categories có mô tả chứa "traditional"
- Input: "artisan" → Tìm categories có mô tả chứa "artisan"

### **Tìm Kiếm Kết Hợp:**
- Input: "pottery" → Tìm trong cả tên và mô tả
- Input: "wood" → Tìm tất cả categories liên quan đến "wood"

## 📝 **Performance Notes**

- **Database Query**: Sử dụng LINQ to Entities cho hiệu suất tốt
- **Indexing**: Nên tạo index cho CategoryName và Description
- **Caching**: Có thể implement caching cho kết quả tìm kiếm
- **Pagination**: Có thể thêm phân trang cho kết quả lớn

## 🔄 **Future Enhancements**

1. **Advanced Search**: Tìm kiếm theo nhiều tiêu chí
2. **Search Filters**: Lọc theo status, date range
3. **Search History**: Lưu lịch sử tìm kiếm
4. **Auto-complete**: Gợi ý khi nhập
5. **Search Analytics**: Thống kê từ khóa tìm kiếm
6. **Export Results**: Xuất kết quả tìm kiếm

## ✅ **Testing Scenarios**

1. **Empty Search**: Tìm kiếm với ô trống → Hiển thị tất cả
2. **Valid Search**: Tìm kiếm có kết quả → Hiển thị danh sách
3. **No Results**: Tìm kiếm không có kết quả → Hiển thị empty state
4. **Clear Search**: Xóa tìm kiếm → Quay về danh sách đầy đủ
5. **Special Characters**: Tìm kiếm với ký tự đặc biệt
6. **Case Sensitivity**: Test không phân biệt hoa thường

Chức năng Search Category đã sẵn sàng để sử dụng! Staff có thể tìm kiếm categories một cách nhanh chóng và hiệu quả. 🎉
