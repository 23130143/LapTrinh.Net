using Bookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Controllers
{
    // ===========================================================================
    // CONTROLLER: AdminDashboardController
    // ===========================================================================
    // Mục đích: Quản lý trang Admin Dashboard và các chức năng admin
    // 
    // [Authorize(Roles = "Admin")] là một Attribute (thuộc tính)
    // Nó bảo ASP.NET Core: CHỈ cho phép User có Role="Admin" truy cập controller này
    // Nếu user chưa đăng nhập hoặc không phải Admin → tự động chuyển đến trang Login
    // ===========================================================================
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        // -----------------------------------------------------------------------
        // BIẾN THÀNH VIÊN: _context
        // -----------------------------------------------------------------------
        // _context là đối tượng để kết nối và làm việc với Database
        // QuanlybansachContext được tạo từ Entity Framework Core
        // Thông qua _context, ta có thể truy vấn tới các bảng: Orders, Products, Users...
        // 
        // readonly: Chỉ được gán giá trị 1 lần (ở constructor), sau đó không đổi được
        // private: Chỉ dùng trong class này, không class khác truy cập được
        // -----------------------------------------------------------------------
        private readonly QuanlybansachContext _context;

        // -----------------------------------------------------------------------
        // CONSTRUCTOR (Hàm khởi tạo)
        // -----------------------------------------------------------------------
        // Constructor là hàm đặc biệt, tự động chạy khi tạo object AdminDashboardController
        // ASP.NET Core tự động "inject" (tiêm) QuanlybansachContext vào đây
        // Đây gọi là Dependency Injection (DI) - một pattern quan trọng trong .NET
        // 
        // Cách hoạt động:
        // 1. Khi user vào trang /AdminDashboard/Index
        // 2. ASP.NET tạo object AdminDashboardController mới
        // 3. Tự động tìm QuanlybansachContext đã đăng ký trong Program.cs
        // 4. Truyền vào constructor này qua tham số context
        // 5. Ta gán context vào biến _context để dùng trong các method khác
        // -----------------------------------------------------------------------
        public AdminDashboardController(QuanlybansachContext context)
        {
            _context = context; // Lưu database context vào biến thành viên
        }

        // =======================================================================
        // ACTION: Index
        // =======================================================================
        // Đây là trang Dashboard chính của Admin
        // URL: /AdminDashboard/Index hoặc /AdminDashboard
        // Nhiệm vụ: Tính toán thống kê và hiển thị trang dashboard
        // =======================================================================
        public IActionResult Index()
        {
            // -------------------------------------------------------------------
            // BƯỚC 1: Tạo ViewModel
            // -------------------------------------------------------------------
            // ViewModel là object chứa dữ liệu sẽ truyền sang View
            // Lúc này viewModel là một object rỗng, các property đều = 0 hoặc empty
            var viewModel = new DashboardViewModel();

            // -------------------------------------------------------------------
            // BƯỚC 2: TÍNH DOANH THU HÔM NAY
            // -------------------------------------------------------------------

            // DateTime.Today: Lấy ngày hôm nay, giờ = 00:00:00
            // Ví dụ: 26/01/2026 00:00:00
            var today = DateTime.Today;

            // Truy vấn LINQ để tính doanh thu:
            // _context.Orders: Truy cập bảng Orders trong database
            // .Where(): Lọc các đơn hàng thỏa mãn điều kiện
            viewModel.TodayRevenue = _context.Orders
                .Where(o =>
                    o.CreateDate.HasValue &&                    // Đơn hàng phải có ngày tạo (không null)
                    o.CreateDate.Value.Date == today &&         // Ngày tạo phải là hôm nay
                    o.Status != "Cancelled"                     // Trạng thái không phải "Cancelled" (đã hủy)
                )
                .Sum(o => o.TotalAmount ?? 0);                  // Cộng tổng TotalAmount, nếu null thì lấy 0

            // Giải thích Sum(o => o.TotalAmount ?? 0):
            // o => là lambda expression (hàm ẩn danh)
            // o.TotalAmount ?? 0 nghĩa là: nếu TotalAmount null thì lấy 0
            // Sum() sẽ cộng tất cả các TotalAmount lại

            // -------------------------------------------------------------------
            // BƯỚC 3: TÍNH DOANH THU THÁNG NÀY
            // -------------------------------------------------------------------

            // Lấy tháng hiện tại (1-12)
            var currentMonth = DateTime.Now.Month;

            // Lấy năm hiện tại (ví dụ: 2026)
            var currentYear = DateTime.Now.Year;

            // Truy vấn tương tự như trên, nhưng điều kiện lọc theo tháng và năm
            viewModel.MonthRevenue = _context.Orders
                .Where(o =>
                    o.CreateDate.HasValue &&
                    o.CreateDate.Value.Month == currentMonth && // CreateDate phải cùng tháng
                    o.CreateDate.Value.Year == currentYear &&   // CreateDate phải cùng năm
                    o.Status != "Cancelled"
                )
                .Sum(o => o.TotalAmount ?? 0);

            // -------------------------------------------------------------------
            // BƯỚC 4: TÍNH TỔNG DOANH THU (TẤT CẢ THỜI GIAN)
            // -------------------------------------------------------------------

            // Không lọc theo ngày/tháng, chỉ loại trừ đơn đã hủy
            viewModel.TotalRevenue = _context.Orders
                .Where(o => o.Status != "Cancelled")
                .Sum(o => o.TotalAmount ?? 0);

            // -------------------------------------------------------------------
            // BƯỚC 5: ĐẾM SỐ LƯỢNG ĐƠN HÀNG
            // -------------------------------------------------------------------
            // Count() đếm số lượng record thỏa mãn điều kiện

            viewModel.TotalOrders = _context.Orders.Count(); // Đếm tất cả đơn hàng

            // Count với lambda: chỉ đếm đơn có Status = "Pending"
            viewModel.PendingOrders = _context.Orders.Count(o => o.Status == "Pending");

            viewModel.ConfirmedOrders = _context.Orders.Count(o => o.Status == "Confirmed");
            viewModel.ShippingOrders = _context.Orders.Count(o => o.Status == "Shipping");
            viewModel.DeliveredOrders = _context.Orders.Count(o => o.Status == "Delivered");
            viewModel.CancelledOrders = _context.Orders.Count(o => o.Status == "Cancelled");

            // -------------------------------------------------------------------
            // BƯỚC 6: ĐẾM SẢN PHẨM VÀ KHÁCH HÀNG
            // -------------------------------------------------------------------

            viewModel.TotalProducts = _context.Products.Count(); // Đếm tất cả sản phẩm

            // Đếm users có Role = "Customer" (không tính Admin)
            viewModel.TotalCustomers = _context.Users.Count(u => u.Role == "Customer");

            // -------------------------------------------------------------------
            // BƯỚC 7: TÍNH DOANH THU 7 NGÀY GẦN NHẤT (cho biểu đồ)
            // -------------------------------------------------------------------

            // Vòng lặp for từ 6 về 0 (tổng 7 lần)
            // i=6: 6 ngày trước
            // i=5: 5 ngày trước
            // ...
            // i=0: hôm nay
            for (int i = 6; i >= 0; i--)
            {
                // Tính ngày cần lấy doanh thu
                // DateTime.Today.AddDays(-i):
                // - Nếu i=6: lấy ngày 6 ngày trước
                // - Nếu i=0: lấy ngày hôm nay
                var date = DateTime.Today.AddDays(-i);

                // Tính doanh thu của ngày đó (tương tự như tính doanh thu hôm nay)
                var revenue = _context.Orders
                    .Where(o =>
                        o.CreateDate.HasValue &&
                        o.CreateDate.Value.Date == date &&  // Chỉ lấy đơn của ngày này
                        o.Status != "Cancelled"
                    )
                    .Sum(o => o.TotalAmount ?? 0);

                // Thêm doanh thu vào danh sách
                viewModel.Last7DaysRevenue.Add(revenue);

                // Thêm nhãn ngày (format dd/MM: 26/01)
                viewModel.Last7DaysLabels.Add(date.ToString("dd/MM"));
            }
            // Sau vòng lặp, Last7DaysRevenue sẽ có 7 phần tử
            // Last7DaysLabels cũng có 7 phần tử tương ứng

            // -------------------------------------------------------------------
            // BƯỚC 8: LẤY 10 ĐƠN HÀNG MỚI NHẤT
            // -------------------------------------------------------------------

            viewModel.RecentOrders = _context.Orders
                // Include(): Eager loading - load luôn dữ liệu liên quan
                // Nếu không Include, DeliveryInformation sẽ null khi truy cập
                .Include(o => o.DeliveryInformation)

                // OrderByDescending: Sắp xếp giảm dần theo CreateDate
                // Đơn mới nhất sẽ ở đầu danh sách
                .OrderByDescending(o => o.CreateDate)

                // Take(10): Chỉ lấy 10 phần tử đầu tiên
                .Take(10)

                // ToList(): Thực thi query và chuyển kết quả thành List<Order>
                .ToList();

            // -------------------------------------------------------------------
            // BƯỚC 9: TRẢ VỀ VIEW
            // -------------------------------------------------------------------
            // View(viewModel): Truyền viewModel sang file Index.cshtml
            // Bên View sẽ dùng @Model để truy cập dữ liệu
            return View(viewModel);
        }

        // =======================================================================
        // ACTION: Products
        // =======================================================================
        // Hiển thị danh sách tất cả sản phẩm
        // URL: /AdminDashboard/Products
        // =======================================================================
        public IActionResult Products()
        {
            // Lấy tất cả sản phẩm, kèm theo thông tin Category và Brand
            var products = _context.Products
                .Include(p => p.Categories)     // Load Category của sản phẩm
                .Include(p => p.Brand)          // Load Brand của sản phẩm
                .OrderByDescending(p => p.Id)   // Sắp xếp theo Id giảm dần (mới nhất trước)
                .ToList();

            // Trả về View với danh sách products
            return View(products);
        }

        // =======================================================================
        // ACTION: Orders
        // =======================================================================
        // Hiển thị danh sách đơn hàng, có thể lọc theo trạng thái
        // URL: /AdminDashboard/Orders?status=Pending
        // Tham số: string? status (? nghĩa là có thể null, không bắt buộc)
        // =======================================================================
        public IActionResult Orders(string? status)
        {
            // Bắt đầu query từ bảng Orders
            // AsQueryable(): Cho phép thêm điều kiện lọc động
            var orders = _context.Orders
                .Include(o => o.DeliveryInformation) // Load thông tin giao hàng
                .AsQueryable();

            // Kiểm tra nếu có tham số status được truyền vào
            if (!string.IsNullOrEmpty(status))
            {
                // Thêm điều kiện lọc: chỉ lấy đơn có Status = tham số truyền vào
                orders = orders.Where(o => o.Status == status);
            }
            // Nếu không có status, lấy tất cả đơn hàng

            // Sắp xếp mới nhất trước
            orders = orders.OrderByDescending(o => o.CreateDate);

            // Execute query và chuyển sang List
            return View(orders.ToList());
        }

        // =======================================================================
        // ACTION: OrderDetails
        // =======================================================================
        // Xem chi tiết một đơn hàng cụ thể
        // URL: /AdminDashboard/OrderDetails/5 (5 là orderId)
        // Tham số: int id (Id của đơn hàng)
        // =======================================================================
        public IActionResult OrderDetails(int id)
        {
            // Tìm đơn hàng theo Id, kèm theo các thông tin liên quan
            var order = _context.Orders
                .Include(o => o.DeliveryInformation)    // Thông tin giao hàng
                .Include(o => o.Orderdetails)           // Chi tiết đơn hàng (các sản phẩm)
                    .ThenInclude(od => od.Product)      // ThenInclude: Load sản phẩm trong từng OrderDetail
                .FirstOrDefault(o => o.Id == id);       // Lấy đơn hàng đầu tiên có Id khớp, null nếu không tìm thấy

            // Giải thích ThenInclude:
            // Include(o => o.Orderdetails): Load danh sách OrderDetail
            // ThenInclude(od => od.Product): Với mỗi OrderDetail, load luôn Product

            // Kiểm tra nếu không tìm thấy đơn hàng
            if (order == null)
            {
                return NotFound(); // Trả về HTTP 404
            }

            // Trả về View với object order
            return View(order);
        }

        // =======================================================================
        // ACTION: UpdateOrderStatus
        // =======================================================================
        // Cập nhật trạng thái đơn hàng
        // URL: POST /AdminDashboard/UpdateOrderStatus
        // Tham số: orderId (Id đơn hàng), status (trạng thái mới)
        // 
        // [HttpPost]: Chỉ chấp nhận HTTP POST (không được GET)
        // Dùng POST vì đây là thao tác thay đổi dữ liệu
        // =======================================================================
        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            // Tìm đơn hàng theo Id
            // Find(): Tìm theo Primary Key, nhanh hơn FirstOrDefault
            var order = _context.Orders.Find(orderId);

            // Nếu tìm thấy đơn hàng
            if (order != null)
            {
                // Cập nhật trạng thái mới
                order.Status = status;

                // Lưu thay đổi vào database
                // SaveChanges(): Thực thi câu lệnh SQL UPDATE
                _context.SaveChanges();

                // TempData: Lưu thông báo tạm thời, hiển thị 1 lần ở trang tiếp theo
                TempData["Success"] = "Đã cập nhật trạng thái đơn hàng!";
            }

            // Chuyển hướng về trang chi tiết đơn hàng
            // RedirectToAction("ActionName", new { tham số })
            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        // =======================================================================
        // ACTION: Users
        // =======================================================================
        // Hiển thị danh sách người dùng
        // URL: /AdminDashboard/Users
        // =======================================================================
        public IActionResult Users()
        {
            // Lấy tất cả user, sắp xếp theo Id giảm dần
            var users = _context.Users
                .OrderByDescending(u => u.Id)
                .ToList();

            return View(users);
        }
    }
}
