using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace QuanLyTHPT.Pages.Admin
{
    public class QuanLyMonHocModel : PageModel
    {
        public List<MonHoc> MonHocList { get; set; }
        public List<MonHoc> OriginalMonHocList { get; set; }
        [BindProperty]
        public string SearchText { get; set; }

        public void OnGet()
        {
            LoadMonHocList();
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Admin/TrangChu");
        }

        public IActionResult OnPostSearch()
        {
            LoadMonHocList();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                MonHocList = MonHocList
                    .Where(mh => mh.TenMonHoc.ToLower().Contains(SearchText.Trim().ToLower()))
                    .ToList();
            }
            return Page();
        }

        public IActionResult OnPostAddMonHoc(int maMonHoc, string tenMonHoc, int idGiaoVien)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem mã môn học đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM monHoc WHERE maMonHoc = @maMonHoc";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Mã môn học đã tồn tại! Vui lòng nhập mã khác.";
                            return RedirectToPage();
                        }
                    }

                    // Kiểm tra xem giáo viên có tồn tại không
                    string checkGiaoVienQuery = "SELECT COUNT(*) FROM giaoVien WHERE idGiaoVien = @idGiaoVien";
                    using (var checkGiaoVienCommand = new SQLiteCommand(checkGiaoVienQuery, connection))
                    {
                        checkGiaoVienCommand.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);
                        int giaoVienCount = Convert.ToInt32(checkGiaoVienCommand.ExecuteScalar());

                        if (giaoVienCount == 0)
                        {
                            TempData["ErrorMessage"] = "Giáo viên với mã này không tồn tại! Vui lòng kiểm tra lại.";
                            return RedirectToPage();
                        }
                    }

                    // Thêm môn học mới
                    string query = "INSERT INTO monHoc (maMonHoc, tenMonHoc, idGiaoVien) VALUES (@maMonHoc, @tenMonHoc, @idGiaoVien)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        command.Parameters.AddWithValue("@tenMonHoc", tenMonHoc);
                        command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Thêm môn học thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể thêm môn học!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi thêm môn học: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEditMonHoc(int maMonHoc, string tenMonHoc, int idGiaoVien)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem giáo viên có tồn tại không
                    string checkGiaoVienQuery = "SELECT COUNT(*) FROM giaoVien WHERE idGiaoVien = @idGiaoVien";
                    using (var checkGiaoVienCommand = new SQLiteCommand(checkGiaoVienQuery, connection))
                    {
                        checkGiaoVienCommand.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);
                        int giaoVienCount = Convert.ToInt32(checkGiaoVienCommand.ExecuteScalar());

                        if (giaoVienCount == 0)
                        {
                            TempData["ErrorMessage"] = "Giáo viên với mã này không tồn tại! Vui lòng kiểm tra lại.";
                            return RedirectToPage();
                        }
                    }

                    // Cập nhật thông tin môn học
                    string query = "UPDATE monHoc SET tenMonHoc = @tenMonHoc, idGiaoVien = @idGiaoVien WHERE maMonHoc = @maMonHoc";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        command.Parameters.AddWithValue("@tenMonHoc", tenMonHoc);
                        command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Cập nhật môn học thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể cập nhật môn học!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật môn học: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteMonHoc(int maMonHoc)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM monHoc WHERE maMonHoc = @maMonHoc";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Xóa môn học thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể xóa môn học!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa môn học: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        private void LoadMonHocList()
        {
            OriginalMonHocList = new List<MonHoc>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM monHoc";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OriginalMonHocList.Add(new MonHoc
                                {
                                    MaMonHoc = Convert.ToInt32(reader["maMonHoc"]),
                                    TenMonHoc = reader["tenMonHoc"].ToString(),
                                    IdGiaoVien = Convert.ToInt32(reader["idGiaoVien"])
                                });
                            }
                        }
                    }

                    // Lấy tên giáo viên cho từng môn học
                    foreach (var monHoc in OriginalMonHocList)
                    {
                        string giaoVienQuery = "SELECT hoVaTen FROM giaoVien WHERE idGiaoVien = @idGiaoVien";
                        using (var giaoVienCommand = new SQLiteCommand(giaoVienQuery, connection))
                        {
                            giaoVienCommand.Parameters.AddWithValue("@idGiaoVien", monHoc.IdGiaoVien);
                            var tenGiaoVien = giaoVienCommand.ExecuteScalar();
                            monHoc.TenGiaoVien = tenGiaoVien != null ? tenGiaoVien.ToString() : "Không xác định";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách môn học: {ex.Message}";
                }
            }
            MonHocList = OriginalMonHocList;
        }
    }
}