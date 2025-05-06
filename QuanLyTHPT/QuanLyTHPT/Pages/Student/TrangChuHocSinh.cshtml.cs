using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace QuanLyTHPT.Pages.Student
{
    public class TrangChuHocSinhModel : PageModel
    {
        public HocSinh HocSinh { get; set; }
        public List<DiemSo> DiemSoList { get; set; }
        public List<MonHoc> MonHocList { get; set; }
        public string LopTen { get; set; }
        [BindProperty]
        public int MaMonHocFilter { get; set; }
        [BindProperty]
        public string LopFilter { get; set; }
        [BindProperty]
        public string HocKyFilter { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(KeyUser.KeyUserSave))
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToPage("/Index");
            }

            LoadHocSinh();
            LoadMonHocList();
            LoadDiemSoList();
            LoadLopTen();
            MaMonHocFilter = 0;
            LopFilter = "";
            HocKyFilter = "";
            return Page();
        }

        public IActionResult OnPostFilter(int maMonHocFilter, string lopFilter, string hocKyFilter)
        {
            if (string.IsNullOrEmpty(KeyUser.KeyUserSave))
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToPage("/Index");
            }

            MaMonHocFilter = maMonHocFilter;
            LopFilter = lopFilter;
            HocKyFilter = hocKyFilter;

            LoadHocSinh();
            LoadMonHocList();
            LoadDiemSoList();
            LoadLopTen();

            // Lọc theo mã môn học
            if (MaMonHocFilter != 0)
            {
                DiemSoList = DiemSoList.Where(ds => ds.MaMonHoc == MaMonHocFilter).ToList();
            }

            // Lọc theo lớp
            if (!string.IsNullOrEmpty(LopFilter))
            {
                DiemSoList = DiemSoList.Where(ds => ds.Lop == LopFilter).ToList();
            }

            // Lọc theo học kỳ
            if (!string.IsNullOrEmpty(HocKyFilter))
            {
                int hocKy = int.Parse(HocKyFilter);
                DiemSoList = DiemSoList.Where(ds => ds.HocKy == hocKy).ToList();
            }

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            KeyUser.KeyUserSave = null;
            return RedirectToPage("/Index");
        }

        private void LoadHocSinh()
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM hocSinh WHERE idHocSinh = @idHocSinh";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idHocSinh", KeyUser.KeyUserSave);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                HocSinh = new HocSinh
                                {
                                    IdHocSinh = Convert.ToInt32(reader["idHocSinh"]),
                                    HoVaTen = reader["hoVaTen"].ToString(),
                                    GioiTinh = reader["gioiTinh"].ToString(),
                                    NgaySinh = reader["ngaySinh"].ToString(),
                                    NamNhapHoc = Convert.ToInt32(reader["namNhapHoc"]),
                                    MaLop = Convert.ToInt32(reader["maLop"]),
                                    Sdt = reader["sdt"].ToString(),
                                    DiaChi = reader["diaChi"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải thông tin học sinh: {ex.Message}";
                }
            }
        }

        private void LoadDiemSoList()
        {
            DiemSoList = new List<DiemSo>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ds.*, mh.tenMonHoc " +
                                   "FROM diemSo ds " +
                                   "INNER JOIN monHoc mh ON ds.maMonHoc = mh.maMonHoc " +
                                   "WHERE ds.idHocSinh = @idHocSinh";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idHocSinh", KeyUser.KeyUserSave);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DiemSoList.Add(new DiemSo
                                {
                                    MaMonHoc = Convert.ToInt32(reader["maMonHoc"]),
                                    TenMonHoc = reader["tenMonHoc"].ToString(),
                                    IdHocSinh = Convert.ToInt32(reader["idHocSinh"]),
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

        private void LoadLopTen()
        {
            if (HocSinh == null) return;

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT tenLop FROM lopHoc WHERE idLopHoc = @maLop";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maLop", HocSinh.MaLop);
                        var result = command.ExecuteScalar();
                        LopTen = result != null ? result.ToString() : "Không xác định";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi tải tên lớp: {ex.Message}";
                }
            }
        }
    }
}