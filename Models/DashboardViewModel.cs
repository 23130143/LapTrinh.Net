namespace Bookstore.Models
{
    // ===========================================================================
    // CLASS: DashboardViewModel
    // ===========================================================================
    // Mục đích: Chứa tất cả dữ liệu cần thiết để hiển thị trang Dashboard Admin
    // ViewModel là một class dùng để truyền dữ liệu từ Controller sang View
    // Nó không lưu vào Database, chỉ dùng để truyền dữ liệu tạm thời
    // ===========================================================================

    public class DashboardViewModel
    {
        // -----------------------------------------------------------------------
        // NHÓM 1: THỐNG KÊ DOANH THU
        // -----------------------------------------------------------------------
        // Doanh thu là tổng tiền của các đơn hàng (KHÔNG bao gồm đơn đã hủy)
        // Kiểu decimal: Dùng cho số tiền vì chính xác hơn double/float
        // -----------------------------------------------------------------------

        // Doanh thu hôm nay (từ 00:00 đến 23:59 ngày hiện tại)
        // Ví dụ: Nếu hôm nay có 5 đơn hàng, tổng 500.000đ → TodayRevenue = 500000
        public decimal TodayRevenue { get; set; }

        // Doanh thu tháng này (từ ngày 1 đến cuối tháng hiện tại)
        // Ví dụ: Tháng 1/2026 có tổng 50 đơn, tổng 10.000.000đ → MonthRevenue = 10000000
        public decimal MonthRevenue { get; set; }

        // Tổng doanh thu từ khi hệ thống bắt đầu (tất cả đơn hàng đã giao/xác nhận)
        // Ví dụ: Tổng cộng 200 đơn từ trước đến nay → TotalRevenue = 50000000
        public decimal TotalRevenue { get; set; }

        // -----------------------------------------------------------------------
        // NHÓM 2: THỐNG KÊ ĐƠN HÀNG THEO TRẠNG THÁI
        // -----------------------------------------------------------------------
        // Mỗi đơn hàng có 1 trạng thái: Pending, Confirmed, Shipping, Delivered, Cancelled
        // Ta đếm số lượng đơn hàng ở từng trạng thái để biết tình hình kinh doanh
        // Kiểu int: Vì đếm số lượng (số nguyên)
        // -----------------------------------------------------------------------

        // Tổng số tất cả đơn hàng (bao gồm cả đơn đã hủy)
        public int TotalOrders { get; set; }

        // Số đơn hàng đang CHỜ XỬ LÝ (vừa đặt, chưa xác nhận)
        // Admin cần vào xử lý những đơn này
        public int PendingOrders { get; set; }

        // Số đơn hàng đã XÁC NHẬN (Admin đã duyệt, chuẩn bị giao)
        public int ConfirmedOrders { get; set; }

        // Số đơn hàng đang GIAO HÀNG (shipper đang giao)
        public int ShippingOrders { get; set; }

        // Số đơn hàng đã GIAO THÀNH CÔNG (khách đã nhận hàng)
        public int DeliveredOrders { get; set; }

        // Số đơn hàng đã BỊ HỦY (khách hủy hoặc admin hủy)
        // Đơn hủy KHÔNG tính vào doanh thu
        public int CancelledOrders { get; set; }

        // -----------------------------------------------------------------------
        // NHÓM 3: THỐNG KÊ CHUNG
        // -----------------------------------------------------------------------

        // Tổng số sản phẩm đang có trong hệ thống
        public int TotalProducts { get; set; }

        // Tổng số khách hàng (User có Role = "Customer")
        // KHÔNG tính Admin
        public int TotalCustomers { get; set; }

        // -----------------------------------------------------------------------
        // NHÓM 4: DỮ LIỆU CHO BIỂU ĐỒ
        // -----------------------------------------------------------------------
        // Biểu đồ doanh thu 7 ngày gần nhất (vẽ bằng Chart.js bên View)
        // List<T>: Danh sách có thể chứa nhiều phần tử
        // -----------------------------------------------------------------------

        // Danh sách doanh thu của 7 ngày (từ 6 ngày trước đến hôm nay)
        // Ví dụ: [100000, 200000, 150000, 300000, 250000, 400000, 350000]
        // Phần tử đầu tiên (index 0) là 6 ngày trước, phần tử cuối (index 6) là hôm nay
        public List<decimal> Last7DaysRevenue { get; set; } = new List<decimal>();

        // Danh sách nhãn ngày tương ứng (để hiển thị trên trục X của biểu đồ)
        // Ví dụ: ["20/01", "21/01", "22/01", "23/01", "24/01", "25/01", "26/01"]
        public List<string> Last7DaysLabels { get; set; } = new List<string>();

        // Giải thích: = new List<decimal>()
        // Nghĩa là khởi tạo sẵn một danh sách rỗng khi tạo object
        // Nếu không làm vậy, biến này sẽ null và gây lỗi khi truy cập

        // -----------------------------------------------------------------------
        // NHÓM 5: DANH SÁCH ĐƠN HÀNG MỚI NHẤT
        // -----------------------------------------------------------------------

        // Danh sách 10 đơn hàng mới nhất để hiển thị nhanh trên Dashboard
        // List<Order>: Danh sách các object Order (Model)
        // Order là class đại diện cho 1 đơn hàng trong Database
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
