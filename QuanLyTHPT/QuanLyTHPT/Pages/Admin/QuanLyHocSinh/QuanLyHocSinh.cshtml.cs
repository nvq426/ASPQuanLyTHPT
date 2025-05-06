using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace QuanLyTHPT.Pages.Admin
{
    public class QuanLyHocSinhModel : PageModel
    {
        public List<HocSinh> HocSinhList { get; set; }
        public List<HocSinh> OriginalHocSinhList { get; set; }
        public List<LopHoc> LopHocList { get; set; }
        [BindProperty]
        public string SearchText { get; set; }
        [BindProperty]
        public int MaLopFilter { get; set; }

        public void OnGet()
        {
            LoadLopHocList();
            LoadHocSinhList();
            SearchText = string.Empty;
            MaLopFilter = 0;
        }

        public IActionResult OnPostSearch(string searchText)
        {
            SearchText = searchText ?? string.Empty;
            LoadLopHocList();
            LoadHocSinhList();

            if (!string.IsNullOrEmpty(SearchText))
            {
                HocSinhList = HocSinhList
                    .Where(hs => hs.HoVaTen.ToLower().Contains(SearchText.ToLower()) ||
                                 hs.Sdt.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
            }

            return Page();
        }

        public IActionResult OnPostFilter(int maLopFilter)
        {
            MaLopFilter = maLopFilter;
            LoadLopHocList();
            LoadHocSinhList();

            if (MaLopFilter != 0)
            {
                HocSinhList = HocSinhList.Where(hs => hs.MaLop == MaLopFilter).ToList();
            }

            return Page();
        }

        public IActionResult OnPostExport()
        {
            if (HocSinhList == null)
            {
                LoadHocSinhList();
            }

            if (!HocSinhList.Any())
            {
                TempData["ErrorMessage"] = "Không có dữ liệu để xuất!";
                LoadLopHocList();
                return Page();
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ID Học sinh,Họ và tên,Giới tính,Ngày sinh,Năm nhập học,Mã lớp,Số điện thoại,Địa chỉ");

            foreach (var hocSinh in HocSinhList)
            {
                string hoVaTen = $"\"{hocSinh.HoVaTen}\"";
                string gioiTinh = $"\"{hocSinh.GioiTinh}\"";
                string ngaySinh = $"\"{hocSinh.NgaySinh}\"";
                string sdt = $"\"{hocSinh.Sdt}\"";
                string diaChi = $"\"{hocSinh.DiaChi}\"";

                sb.AppendLine($"{hocSinh.IdHocSinh},{hoVaTen},{gioiTinh},{ngaySinh},{hocSinh.NamNhapHoc},{hocSinh.MaLop},{sdt},{diaChi}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(bytes);

            TempData["Message"] = "Dữ liệu đã được xuất thành công!";
            LoadLopHocList();
            return File(stream, "text/csv", "DanhSachHocSinh.csv");
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Admin/TrangChu");
        }

        public IActionResult OnPostAddStudent(int idHocSinh, string hoVaTen, string gioiTinh, string ngaySinh, int namNhapHoc, int maLop, string sdt, string diaChi)
        {
            if (idHocSinh <= 0 || string.IsNullOrWhiteSpace(hoVaTen) || string.IsNullOrWhiteSpace(gioiTinh) ||
                string.IsNullOrWhiteSpace(ngaySinh) || namNhapHoc <= 0 || maLop <= 0 || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(diaChi))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ các trường thông tin bắt buộc!";
                LoadLopHocList();
                LoadHocSinhList();
                return Page();
            }

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem học sinh đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM hocSinh WHERE idHocSinh = @idHocSinh";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Học sinh đã tồn tại! Vui lòng kiểm tra lại thông tin.";
                            LoadLopHocList();
                            LoadHocSinhList();
                            return Page();
                        }
                    }

                    // Kiểm tra xem mã lớp có tồn tại không
                    string checkLopQuery = "SELECT COUNT(*) FROM lopHoc WHERE idLopHoc = @maLop";
                    using (var checkLopCommand = new SQLiteCommand(checkLopQuery, connection))
                    {
                        checkLopCommand.Parameters.AddWithValue("@maLop", maLop);
                        int lopCount = Convert.ToInt32(checkLopCommand.ExecuteScalar());

                        if (lopCount == 0)
                        {
                            TempData["ErrorMessage"] = "Mã lớp không tồn tại! Vui lòng kiểm tra lại.";
                            LoadLopHocList();
                            LoadHocSinhList();
                            return Page();
                        }
                    }

                    // Thêm học sinh vào cơ sở dữ liệu
                    string insertQuery = "INSERT INTO hocSinh (idHocSinh, hoVaTen, gioiTinh, ngaySinh, namNhapHoc, maLop, sdt, diaChi) " +
                                        "VALUES (@idHocSinh, @hoVaTen, @gioiTinh, @ngaySinh, @namNhapHoc, @maLop, @sdt, @diaChi)";
                    using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        insertCommand.Parameters.AddWithValue("@hoVaTen", hoVaTen);
                        insertCommand.Parameters.AddWithValue("@gioiTinh", gioiTinh);
                        insertCommand.Parameters.AddWithValue("@ngaySinh", ngaySinh);
                        insertCommand.Parameters.AddWithValue("@namNhapHoc", namNhapHoc);
                        insertCommand.Parameters.AddWithValue("@maLop", maLop);
                        insertCommand.Parameters.AddWithValue("@sdt", sdt);
                        insertCommand.Parameters.AddWithValue("@diaChi", diaChi);

                        int result = insertCommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Thêm học sinh thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể thêm học sinh!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                }
            }

            LoadLopHocList();
            LoadHocSinhList();
            return Page();
        }

        public IActionResult OnPostEditStudent(int idHocSinh, string hoVaTen, string gioiTinh, string ngaySinh, int namNhapHoc, int maLop, string sdt, string diaChi)
        {
            if (idHocSinh <= 0 || string.IsNullOrWhiteSpace(hoVaTen) || string.IsNullOrWhiteSpace(gioiTinh) ||
                string.IsNullOrWhiteSpace(ngaySinh) || namNhapHoc <= 0 || maLop <= 0 || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(diaChi))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ các trường thông tin bắt buộc!";
                LoadLopHocList();
                LoadHocSinhList();
                return Page();
            }

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem mã lớp có tồn tại không
                    string checkLopQuery = "SELECT COUNT(*) FROM lopHoc WHERE idLopHoc = @maLop";
                    using (var checkLopCommand = new SQLiteCommand(checkLopQuery, connection))
                    {
                        checkLopCommand.Parameters.AddWithValue("@maLop", maLop);
                        int lopCount = Convert.ToInt32(checkLopCommand.ExecuteScalar());

                        if (lopCount == 0)
                        {
                            TempData["ErrorMessage"] = "Mã lớp không tồn tại! Vui lòng kiểm tra lại.";
                            LoadLopHocList();
                            LoadHocSinhList();
                            return Page();
                        }
                    }

                    // Cập nhật thông tin học sinh
                    string updateQuery = "UPDATE hocSinh SET hoVaTen = @hoVaTen, gioiTinh = @gioiTinh, ngaySinh = @ngaySinh, namNhapHoc = @namNhapHoc, maLop = @maLop, sdt = @sdt, diaChi = @diaChi " +
                                        "WHERE idHocSinh = @idHocSinh";
                    using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        updateCommand.Parameters.AddWithValue("@hoVaTen", hoVaTen);
                        updateCommand.Parameters.AddWithValue("@gioiTinh", gioiTinh);
                        updateCommand.Parameters.AddWithValue("@ngaySinh", ngaySinh);
                        updateCommand.Parameters.AddWithValue("@namNhapHoc", namNhapHoc);
                        updateCommand.Parameters.AddWithValue("@maLop", maLop);
                        updateCommand.Parameters.AddWithValue("@sdt", sdt);
                        updateCommand.Parameters.AddWithValue("@diaChi", diaChi);

                        int result = updateCommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Cập nhật thông tin học sinh thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể cập nhật thông tin học sinh!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                }
            }

            LoadLopHocList();
            LoadHocSinhList();
            return Page();
        }

        public IActionResult OnPostDeleteStudent(int idHocSinh)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem học sinh có điểm số trong hệ thống không
                    string checkDiemSoQuery = "SELECT COUNT(*) FROM diemSo WHERE idHocSinh = @idHocSinh";
                    using (var checkDiemSoCommand = new SQLiteCommand(checkDiemSoQuery, connection))
                    {
                        checkDiemSoCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int diemSoCount = Convert.ToInt32(checkDiemSoCommand.ExecuteScalar());

                        if (diemSoCount > 0)
                        {
                            TempData["ErrorMessage"] = "Học sinh có điểm số trong hệ thống, không thể xóa!";
                            LoadLopHocList();
                            LoadHocSinhList();
                            return Page();
                        }
                    }

                    // Xóa học sinh
                    string deleteQuery = "DELETE FROM hocSinh WHERE idHocSinh = @idHocSinh";
                    using (var deleteCommand = new SQLiteCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int result = deleteCommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Xóa học sinh thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể xóa học sinh!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                }
            }

            LoadLopHocList();
            LoadHocSinhList();
            return Page();
        }

        private void LoadHocSinhList()
        {
            HocSinhList = new List<HocSinh>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM hocSinh";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HocSinhList.Add(new HocSinh
                                {
                                    IdHocSinh = Convert.ToInt32(reader["idHocSinh"]),
                                    HoVaTen = reader["hoVaTen"].ToString(),
                                    GioiTinh = reader["gioiTinh"].ToString(),
                                    NgaySinh = reader["ngaySinh"].ToString(),
                                    NamNhapHoc = Convert.ToInt32(reader["namNhapHoc"]),
                                    MaLop = Convert.ToInt32(reader["maLop"]),
                                    Sdt = reader["sdt"].ToString(),
                                    DiaChi = reader["diaChi"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách học sinh: {ex.Message}";
                }
            }
            OriginalHocSinhList = HocSinhList;
        }

        private void LoadLopHocList()
        {
            LopHocList = new List<LopHoc>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM lopHoc";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LopHocList.Add(new LopHoc
                                {
                                    IdLopHoc = Convert.ToInt32(reader["idLopHoc"]),
                                    TenLop = reader["tenLop"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách lớp học: {ex.Message}";
                }
            }
        }
    }
}