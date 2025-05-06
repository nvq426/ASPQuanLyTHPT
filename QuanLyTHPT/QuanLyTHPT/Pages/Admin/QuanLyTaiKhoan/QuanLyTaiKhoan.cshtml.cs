using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace QuanLyTHPT.Pages.Admin
{
    public class QuanLyTaiKhoanModel : PageModel
    {
        public List<TaiKhoanNguoiDung> TaiKhoanList { get; set; }

        public void OnGet()
        {
            LoadTaiKhoanList();
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Admin/TrangChu");
        }

        public IActionResult OnPostAddTaiKhoan(string tenNguoiDung, string matKhau, string quyenTruyCap, string email)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem tên người dùng đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM takhoan_nguoidung WHERE tenNguoiDung = @tenNguoiDung";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@tenNguoiDung", tenNguoiDung);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Tên người dùng đã tồn tại! Vui lòng nhập tên khác.";
                            return RedirectToPage();
                        }
                    }

                    // Thêm tài khoản mới
                    string query = "INSERT INTO takhoan_nguoidung (tenNguoiDung, matKhau, quyenTruyCap, email) VALUES (@tenNguoiDung, @matKhau, @quyenTruyCap, @email)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tenNguoiDung", tenNguoiDung);
                        command.Parameters.AddWithValue("@matKhau", matKhau);
                        command.Parameters.AddWithValue("@quyenTruyCap", quyenTruyCap);
                        command.Parameters.AddWithValue("@email", email);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Thêm tài khoản thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể thêm tài khoản!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi thêm tài khoản: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEditTaiKhoan(string tenNguoiDung, string matKhau, string quyenTruyCap, string email)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Cập nhật thông tin tài khoản
                    string query = "UPDATE takhoan_nguoidung SET matKhau = @matKhau, quyenTruyCap = @quyenTruyCap, email = @email WHERE tenNguoiDung = @tenNguoiDung";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tenNguoiDung", tenNguoiDung);
                        command.Parameters.AddWithValue("@matKhau", matKhau);
                        command.Parameters.AddWithValue("@quyenTruyCap", quyenTruyCap);
                        command.Parameters.AddWithValue("@email", email);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Cập nhật tài khoản thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể cập nhật tài khoản!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật tài khoản: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteTaiKhoan(string tenNguoiDung)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM takिनाhoan_nguoidung WHERE tenNguoiDung = @tenNguoiDung";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tenNguoiDung", tenNguoiDung);
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Xóa tài khoản thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể xóa tài khoản!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa tài khoản: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        private void LoadTaiKhoanList()
        {
            TaiKhoanList = new List<TaiKhoanNguoiDung>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM takhoan_nguoidung";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TaiKhoanList.Add(new TaiKhoanNguoiDung
                                {
                                    TenNguoiDung = reader["tenNguoiDung"].ToString(),
                                    MatKhau = reader["matKhau"].ToString(),
                                    QuyenTruyCap = reader["quyenTruyCap"].ToString(),
                                    Email = reader["email"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách tài khoản: {ex.Message}";
                }
            }
        }
    }
}