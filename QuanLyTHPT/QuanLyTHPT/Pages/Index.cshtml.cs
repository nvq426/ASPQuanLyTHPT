using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;

namespace QuanLyTHPT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Hiển thị form đăng nhập
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên người dùng và mật khẩu.";
                return Page();
            }

            string quyenTruyCap = CheckLogin(Username, Password);

            if (!string.IsNullOrEmpty(quyenTruyCap))
            {
                if (quyenTruyCap == "admin")
                {
                    return RedirectToPage("/Admin/TrangChu");
                }
                else if (quyenTruyCap == "student")
                {
                    KeyUser.KeyUserSave = Username; // Gán trực tiếp vào KeyUser.KeyUserSave
                    return RedirectToPage("/Student/TrangChuHocSinh");
                }
                else
                {
                    TempData["ErrorMessage"] = "Quyền truy cập không hợp lệ.";
                    return Page();
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Tên người dùng hoặc mật khẩu không đúng.";
                return Page();
            }
        }

        private string CheckLogin(string tenNguoiDung, string matKhau)
        {
            string quyenTruyCap = null;
            string connectionString = @"Data Source=dataTHPT.db;Version=3;";

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT quyenTruyCap FROM takhoan_nguoidung WHERE tenNguoiDung = @tenNguoiDung AND matKhau = @matKhau";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tenNguoiDung", tenNguoiDung);
                        cmd.Parameters.AddWithValue("@matKhau", matKhau);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                quyenTruyCap = reader["quyenTruyCap"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi kết nối cơ sở dữ liệu.");
                }
            }

            return quyenTruyCap;
        }
    }
}