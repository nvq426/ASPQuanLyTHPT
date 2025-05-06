using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace QuanLyTHPT.Pages.Admin
{
    public class QuanLyDiemSoModel : PageModel
    {
        public List<DiemSo> DiemSoList { get; set; }
        public List<MonHoc> MonHocList { get; set; } = new List<MonHoc>();
        [BindProperty]
        public int MaMonHocFilter { get; set; }
        [BindProperty]
        public string KhoiLopFilter { get; set; }
        [BindProperty]
        public string IdHocSinhFilter { get; set; }
        [BindProperty]
        public int HocKyFilter { get; set; }

        public void OnGet()
        {
            LoadMonHocList();
            LoadDiemSoList();
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Admin/TrangChu");
        }

        public IActionResult OnPostFilter()
        {
            LoadMonHocList();
            LoadDiemSoList();

            // Lọc theo mã môn học
            if (MaMonHocFilter != 0)
            {
                DiemSoList = DiemSoList.Where(ds => ds.MaMonHoc == MaMonHocFilter).ToList();
            }

            // Lọc theo khối lớp
            if (!string.IsNullOrEmpty(KhoiLopFilter))
            {
                DiemSoList = DiemSoList.Where(ds => ds.Lop == KhoiLopFilter).ToList();
            }

            // Lọc theo mã học sinh
            if (!string.IsNullOrEmpty(IdHocSinhFilter))
            {
                int idHocSinh = int.Parse(IdHocSinhFilter);
                DiemSoList = DiemSoList.Where(ds => ds.IdHocSinh == idHocSinh).ToList();
            }

            // Lọc theo học kỳ
            if (HocKyFilter != 0)
            {
                DiemSoList = DiemSoList.Where(ds => ds.HocKy == HocKyFilter).ToList();
            }

            return Page();
        }

        public IActionResult OnPostAddDiemSo(int maMonHoc, int idHocSinh, string lop, int hocKy, double diemTX1, double diemTX2, double diemTX3, double diemTX4, double diemTX5, double diemGiuaKy, double diemThi)
        {
            double diemTB = ((diemTX1 + diemTX2 + diemTX3 + diemTX4 + diemTX5) + (diemGiuaKy * 2) + (diemThi * 3)) / 10;

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Kiểm tra xem mã môn học có tồn tại không
                    string checkMonHocQuery = "SELECT COUNT(*) FROM monHoc WHERE maMonHoc = @maMonHoc";
                    using (var checkMonHocCommand = new SQLiteCommand(checkMonHocQuery, connection))
                    {
                        checkMonHocCommand.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        int monHocCount = Convert.ToInt32(checkMonHocCommand.ExecuteScalar());

                        if (monHocCount == 0)
                        {
                            TempData["ErrorMessage"] = "Mã môn học không tồn tại! Vui lòng kiểm tra lại.";
                            LoadMonHocList();
                            return RedirectToPage();
                        }
                    }

                    // Kiểm tra xem học sinh có tồn tại không
                    string checkHocSinhQuery = "SELECT COUNT(*) FROM hocSinh WHERE idHocSinh = @idHocSinh";
                    using (var checkHocSinhCommand = new SQLiteCommand(checkHocSinhQuery, connection))
                    {
                        checkHocSinhCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int hocSinhCount = Convert.ToInt32(checkHocSinhCommand.ExecuteScalar());

                        if (hocSinhCount == 0)
                        {
                            TempData["ErrorMessage"] = "Học sinh không tồn tại! Vui lòng kiểm tra lại.";
                            LoadMonHocList();
                            return RedirectToPage();
                        }
                    }

                    // Kiểm tra xem điểm số đã tồn tại chưa (dựa trên mã môn học và mã học sinh)
                    string checkDiemSoQuery = "SELECT COUNT(*) FROM diemSo WHERE maMonHoc = @maMonHoc AND idHocSinh = @idHocSinh";
                    using (var checkDiemSoCommand = new SQLiteCommand(checkDiemSoQuery, connection))
                    {
                        checkDiemSoCommand.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        checkDiemSoCommand.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int diemSoCount = Convert.ToInt32(checkDiemSoCommand.ExecuteScalar());

                        if (diemSoCount > 0)
                        {
                            TempData["ErrorMessage"] = "Điểm số cho môn học và học sinh này đã tồn tại! Vui lòng kiểm tra lại.";
                            LoadMonHocList();
                            return RedirectToPage();
                        }
                    }

                    // Thêm điểm số mới
                    string query = "INSERT INTO diemSo (maMonHoc, idHocSinh, lop, hocKy, diemThuongXuyen1, diemThuongXuyen2, diemThuongXuyen3, diemThuongXuyen4, diemThuongXuyen5, diemGiuaKy, diemThi, diemTB) " +
                                   "VALUES (@maMonHoc, @idHocSinh, @lop, @hocKy, @diemTX1, @diemTX2, @diemTX3, @diemTX4, @diemTX5, @diemGiuaKy, @diemThi, @diemTB)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        command.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        command.Parameters.AddWithValue("@lop", lop);
                        command.Parameters.AddWithValue("@hocKy", hocKy);
                        command.Parameters.AddWithValue("@diemTX1", diemTX1);
                        command.Parameters.AddWithValue("@diemTX2", diemTX2);
                        command.Parameters.AddWithValue("@diemTX3", diemTX3);
                        command.Parameters.AddWithValue("@diemTX4", diemTX4);
                        command.Parameters.AddWithValue("@diemTX5", diemTX5);
                        command.Parameters.AddWithValue("@diemGiuaKy", diemGiuaKy);
                        command.Parameters.AddWithValue("@diemThi", diemThi);
                        command.Parameters.AddWithValue("@diemTB", diemTB);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Nhập điểm thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể nhập điểm!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi nhập điểm: {ex.Message}";
                }
            }
            LoadMonHocList();
            return RedirectToPage();
        }

        public IActionResult OnPostEditDiemSo(int maMonHoc, int idHocSinh, string lop, int hocKy, double diemTX1, double diemTX2, double diemTX3, double diemTX4, double diemTX5, double diemGiuaKy, double diemThi)
        {
            double diemTB = ((diemTX1 + diemTX2 + diemTX3 + diemTX4 + diemTX5) + (diemGiuaKy * 2) + (diemThi * 3)) / 10;

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Cập nhật điểm số
                    string query = "UPDATE diemSo SET lop = @lop, hocKy = @hocKy, diemThuongXuyen1 = @diemTX1, diemThuongXuyen2 = @diemTX2, diemThuongXuyen3 = @diemTX3, diemThuongXuyen4 = @diemTX4, diemThuongXuyen5 = @diemTX5, diemGiuaKy = @diemGiuaKy, diemThi = @diemThi, diemTB = @diemTB " +
                                  "WHERE maMonHoc = @maMonHoc AND idHocSinh = @idHocSinh";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        command.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        command.Parameters.AddWithValue("@lop", lop);
                        command.Parameters.AddWithValue("@hocKy", hocKy);
                        command.Parameters.AddWithValue("@diemTX1", diemTX1);
                        command.Parameters.AddWithValue("@diemTX2", diemTX2);
                        command.Parameters.AddWithValue("@diemTX3", diemTX3);
                        command.Parameters.AddWithValue("@diemTX4", diemTX4);
                        command.Parameters.AddWithValue("@diemTX5", diemTX5);
                        command.Parameters.AddWithValue("@diemGiuaKy", diemGiuaKy);
                        command.Parameters.AddWithValue("@diemThi", diemThi);
                        command.Parameters.AddWithValue("@diemTB", diemTB);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Cập nhật điểm số thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể cập nhật điểm số!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật điểm số: {ex.Message}";
                }
            }
            LoadMonHocList();
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteDiemSo(int maMonHoc, int idHocSinh)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM diemSo WHERE maMonHoc = @maMonHoc AND idHocSinh = @idHocSinh";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maMonHoc", maMonHoc);
                        command.Parameters.AddWithValue("@idHocSinh", idHocSinh);
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["Message"] = "Xóa điểm số thành công!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể xóa điểm số!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa điểm số: {ex.Message}";
                }
            }
            LoadMonHocList();
            return RedirectToPage();
        }

        private void LoadDiemSoList()
        {
            DiemSoList = new List<DiemSo>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ds.*, mh.tenMonHoc, hs.hoVaTen AS tenHocSinh " +
                                   "FROM diemSo ds " +
                                   "INNER JOIN monHoc mh ON ds.maMonHoc = mh.maMonHoc " +
                                   "INNER JOIN hocSinh hs ON ds.idHocSinh = hs.idHocSinh";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DiemSoList.Add(new DiemSo
                                {
                                    MaMonHoc = Convert.ToInt32(reader["maMonHoc"]),
                                    TenMonHoc = reader["tenMonHoc"].ToString(),
                                    IdHocSinh = Convert.ToInt32(reader["idHocSinh"]),
                                    TenHocSinh = reader["tenHocSinh"].ToString(),
                                    Lop = reader["lop"].ToString(),
                                    HocKy = Convert.ToInt32(reader["hocKy"]),
                                    DiemThuongXuyen1 = Convert.ToDouble(reader["diemThuongXuyen1"]),
                                    DiemThuongXuyen2 = Convert.ToDouble(reader["diemThuongXuyen2"]),
                                    DiemThuongXuyen3 = Convert.ToDouble(reader["diemThuongXuyen3"]),
                                    DiemThuongXuyen4 = Convert.ToDouble(reader["diemThuongXuyen4"]),
                                    DiemThuongXuyen5 = Convert.ToDouble(reader["diemThuongXuyen5"]),
                                    DiemGiuaKy = Convert.ToDouble(reader["diemGiuaKy"]),
                                    DiemThi = Convert.ToDouble(reader["diemThi"]),
                                    DiemTB = Convert.ToDouble(reader["diemTB"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách điểm số: {ex.Message}";
                }
            }
        }

        private void LoadMonHocList()
        {
            MonHocList = new List<MonHoc>();
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
                                MonHocList.Add(new MonHoc
                                {
                                    MaMonHoc = Convert.ToInt32(reader["maMonHoc"]),
                                    TenMonHoc = reader["tenMonHoc"].ToString(),
                                    IdGiaoVien = Convert.ToInt32(reader["idGiaoVien"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải danh sách môn học: {ex.Message}";
                }
            }
        }
    }
}