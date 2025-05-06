using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace QuanLyTHPT.Pages.Admin
{
    public class QuanLyLopHocModel : PageModel
    {
        public List<LopHoc> LopHocList { get; set; }
        public Dictionary<int, List<HocSinh>> StudentsByLopHoc { get; set; } = new Dictionary<int, List<HocSinh>>();
        [BindProperty]
        public string BoLoc { get; set; } = "0";

        public void OnGet()
        {
            LoadLopHocList();
            LoadStudentsByLopHoc();
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Admin/TrangChu");
        }

        public IActionResult OnPostFilter()
        {
            if (string.IsNullOrEmpty(BoLoc))
            {
                BoLoc = "0";
            }
            LoadLopHocList();
            LoadStudentsByLopHoc();
            return Page();
        }

        public IActionResult OnPostExport()
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID Lớp học,Tên lớp,Sĩ số lớp,Tên giáo viên chủ nhiệm,Mã giáo viên chủ nhiệm");

            LoadLopHocList();
            foreach (var lop in LopHocList)
            {
                csv.AppendLine($"{lop.IdLopHoc},{lop.TenLop},{lop.SiSoLop},{lop.TenGiaoVien},{lop.IdGiaoVien}");
            }

            var fileName = $"DanhSachLopHoc_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var fileContent = Encoding.UTF8.GetBytes(csv.ToString());
            return File(fileContent, "text/csv", fileName);
        }

        public IActionResult OnPostAddLopHoc(int idLopHoc, string tenLop, int idGiaoVien)
        {
            if (string.IsNullOrWhiteSpace(tenLop))
            {
                TempData["ErrorMessage"] = "Tên lớp không được để trống!";
                return RedirectToPage();
            }

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM lopHoc WHERE idLopHoc = @idLopHoc OR tenLop = @tenLop";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@idLopHoc", idLopHoc);
                        checkCommand.Parameters.AddWithValue("@tenLop", tenLop);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Lớp học đã tồn tại! Vui lòng kiểm tra lại thông tin.";
                            return RedirectToPage();
                        }
                    }

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

                    string query = "INSERT INTO lopHoc (idLopHoc, tenLop, idGiaoVien) VALUES (@idLopHoc, @tenLop, @idGiaoVien)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idLopHoc", idLopHoc);
                        command.Parameters.AddWithValue("@tenLop", tenLop);
                        command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Thêm lớp học thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể thêm lớp học!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi thêm lớp học: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEditLopHoc(int idLopHoc, string tenLop, int idGiaoVien)
        {
            if (string.IsNullOrWhiteSpace(tenLop))
            {
                TempData["ErrorMessage"] = "Tên lớp không được để trống!";
                return RedirectToPage();
            }

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

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

                    string checkTenLopQuery = "SELECT COUNT(*) FROM lopHoc WHERE tenLop = @tenLop AND idLopHoc != @idLopHoc";
                    using (var checkTenLopCommand = new SQLiteCommand(checkTenLopQuery, connection))
                    {
                        checkTenLopCommand.Parameters.AddWithValue("@tenLop", tenLop);
                        checkTenLopCommand.Parameters.AddWithValue("@idLopHoc", idLopHoc);
                        int count = Convert.ToInt32(checkTenLopCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Tên lớp đã được sử dụng cho một lớp khác! Vui lòng chọn tên khác.";
                            return RedirectToPage();
                        }
                    }

                    string query = "UPDATE lopHoc SET tenLop = @tenLop, idGiaoVien = @idGiaoVien WHERE idLopHoc = @idLopHoc";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idLopHoc", idLopHoc);
                        command.Parameters.AddWithValue("@tenLop", tenLop);
                        command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Cập nhật lớp học thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể cập nhật lớp học!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật lớp học: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteLopHoc(int idLopHoc)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string checkHocSinhQuery = "SELECT COUNT(*) FROM hocSinh WHERE maLop = @idLopHoc";
                    using (var checkHocSinhCommand = new SQLiteCommand(checkHocSinhQuery, connection))
                    {
                        checkHocSinhCommand.Parameters.AddWithValue("@idLopHoc", idLopHoc);
                        int hocSinhCount = Convert.ToInt32(checkHocSinhCommand.ExecuteScalar());

                        if (hocSinhCount > 0)
                        {
                            TempData["ErrorMessage"] = "Lớp học hiện đang có học sinh, không thể xóa!";
                            return RedirectToPage();
                        }
                    }

                    string query = "DELETE FROM lopHoc WHERE idLopHoc = @idLopHoc";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idLopHoc", idLopHoc);
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Xóa lớp học thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể xóa lớp học!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa lớp học: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostAddHocSinh(int idHocSinh, string hoVaTen, string gioiTinh, string ngaySinh, int namNhapHoc, int maLop, string sdt, string diaChi)
        {
            if (string.IsNullOrWhiteSpace(hoVaTen) || string.IsNullOrWhiteSpace(gioiTinh) || string.IsNullOrWhiteSpace(ngaySinh) || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(diaChi))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ các trường thông tin bắt buộc!";
                return RedirectToPage();
            }

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string checkLopHocQuery = "SELECT COUNT(*) FROM lopHoc WHERE idLopHoc = @maLop";
                    using (var checkLopHocCommand = new SQLiteCommand(checkLopHocQuery, connection))
                    {
                        checkLopHocCommand.Parameters.AddWithValue("@maLop", maLop);
                        int lopHocCount = Convert.ToInt32(checkLopHocCommand.ExecuteScalar());

                        if (lopHocCount == 0)
                        {
                            TempData["ErrorMessage"] = "Lớp học với mã này không tồn tại! Vui lòng kiểm tra lại.";
                            return RedirectToPage();
                        }
                    }

                    string checkQuery = "SELECT COUNT(*) FROM hocSinh WHERE idHocSinh = @idHocSinh";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Học sinh đã tồn tại! Vui lòng kiểm tra lại thông tin.";
                            return RedirectToPage();
                        }
                    }

                    string query = "INSERT INTO hocSinh (idHocSinh, hoVaTen, gioiTinh, ngaySinh, namNhapHoc, maLop, sdt, diaChi) " +
                                   "VALUES (@idHocSinh, @hoVaTen, @gioiTinh, @ngaySinh, @namNhapHoc, @maLop, @sdt, @diaChi)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        command.Parameters.AddWithValue("@hoVaTen", hoVaTen);
                        command.Parameters.AddWithValue("@gioiTinh", gioiTinh);
                        command.Parameters.AddWithValue("@ngaySinh", ngaySinh);
                        command.Parameters.AddWithValue("@namNhapHoc", namNhapHoc);
                        command.Parameters.AddWithValue("@maLop", maLop);
                        command.Parameters.AddWithValue("@sdt", sdt);
                        command.Parameters.AddWithValue("@diaChi", diaChi);

                        int result = command.ExecuteNonQuery();
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
                    TempData["ErrorMessage"] = $"Lỗi khi thêm học sinh: {ex.Message}";
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }

        private void LoadLopHocList()
        {
            LopHocList = new List<LopHoc>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = BoLoc == "0" ? "SELECT * FROM lopHoc" : "SELECT * FROM lopHoc WHERE tenLop LIKE @tenLop";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        if (BoLoc != "0")
                        {
                            command.Parameters.AddWithValue("@tenLop", $"{BoLoc}%");
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LopHocList.Add(new LopHoc
                                {
                                    IdLopHoc = Convert.ToInt32(reader["idLopHoc"]),
                                    TenLop = reader["tenLop"].ToString(),
                                    IdGiaoVien = Convert.ToInt32(reader["idGiaoVien"])
                                });
                            }
                        }
                    }

                    foreach (var lop in LopHocList)
                    {
                        string countQuery = "SELECT COUNT(*) FROM hocSinh WHERE maLop = @maLop";
                        using (var countCommand = new SQLiteCommand(countQuery, connection))
                        {
                            countCommand.Parameters.AddWithValue("@maLop", lop.IdLopHoc);
                            lop.SiSoLop = Convert.ToInt32(countCommand.ExecuteScalar());
                        }

                        string giaoVienQuery = "SELECT hoVaTen FROM giaoVien WHERE idGiaoVien = @idGiaoVien";
                        using (var giaoVienCommand = new SQLiteCommand(giaoVienQuery, connection))
                        {
                            giaoVienCommand.Parameters.AddWithValue("@idGiaoVien", lop.IdGiaoVien);
                            var tenGiaoVien = giaoVienCommand.ExecuteScalar();
                            lop.TenGiaoVien = tenGiaoVien != null ? tenGiaoVien.ToString() : "Không xác định";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách lớp học: {ex.Message}";
                }
            }
        }

        private void LoadStudentsByLopHoc()
        {
            StudentsByLopHoc = new Dictionary<int, List<HocSinh>>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    foreach (var lop in LopHocList)
                    {
                        var students = new List<HocSinh>();
                        string query = "SELECT * FROM hocSinh WHERE maLop = @idLopHoc";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@idLopHoc", lop.IdLopHoc);
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    students.Add(new HocSinh
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
                        StudentsByLopHoc[lop.IdLopHoc] = students;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách học sinh: {ex.Message}";
                }
            }
        }
    }
}