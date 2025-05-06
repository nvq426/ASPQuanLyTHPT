using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyTHPT;

namespace QuanLyTHPT
{
    public static class KeyUser
    {
        public static string KeyUserSave { get; set; } 
    }
    public static class DatabaseConfig
    {
        //public static string ConnectionString { get; } = "Data Source=C:\\3.MEGANZ.nguyenvanquoc426\\30.TaiLieu\\301.TaiLieuChuyenNganh\\#BAITHI HKI 2024-2025\\LapTrinhCSharp.Net\\K68_HTTT.237480104020.NguyenVanQuoc.LTNET\\SOURCE CODE\\QuanLyTHPT\\dataTHPT.db;Version=3;";
        //public static string ConnectionString { get; } = "Data Source=dataTHPT.db;Version=3;";
        public static string ConnectionString { get; } = "Data Source=dataTHPT.db;Version=3;";
    }
    public class DatabaseHelper
    {

        public List<HocSinh> GetHocSinhList()
        {
            List<HocSinh> hocSinhList = new List<HocSinh>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM hocSinh";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hocSinhList.Add(new HocSinh
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
            return hocSinhList;
        }
        public List<TaiKhoanNguoiDung> GetTaiKhoanList()
        {
            List<TaiKhoanNguoiDung> taiKhoanList = new List<TaiKhoanNguoiDung>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM takhoan_nguoidung";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            taiKhoanList.Add(new TaiKhoanNguoiDung
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
            return taiKhoanList;
        }

        public List<LopHoc> GetLopHocList(string boloc)
        {
            List<LopHoc> lopHocList = new List<LopHoc>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                string query;
                connection.Open();

                // Câu truy vấn SQL sử dụng JOIN để lấy thông tin tên học sinh
                if (boloc == "Tất cả")
                {
                    query = @"SELECT lopHoc.idLopHoc, lopHoc.tenLop, hocSinh.hoVaTen AS tenHocSinh, lopHoc.idGiaoVien 
                      FROM lopHoc 
                      INNER JOIN hocSinh ON lopHoc.idHocSinh = hocSinh.idHocSinh";
                }
                else
                {
                    query = @"SELECT lopHoc.idLopHoc, lopHoc.tenLop, hocSinh.hoVaTen AS tenHocSinh, lopHoc.idGiaoVien 
                      FROM lopHoc 
                      INNER JOIN hocSinh ON lopHoc.idHocSinh = hocSinh.idHocSinh
                      WHERE lopHoc.tenLop = @boloc";
                }

                using (var command = new SQLiteCommand(query, connection))
                {
                    if (boloc != "Tất cả")
                    {
                        command.Parameters.AddWithValue("@boloc", boloc);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lopHocList.Add(new LopHoc
                            {
                                IdLopHoc = Convert.ToInt32(reader["idLopHoc"]),
                                TenLop = reader["tenLop"].ToString(),
                                TenHocSinh = reader["tenHocSinh"].ToString(), // Lấy tên học sinh từ bảng hocSinh
                                IdGiaoVien = Convert.ToInt32(reader["idGiaoVien"])
                            });
                        }
                    }
                }
            }
            return lopHocList;
        }
        public List<MonHoc> GetMonHocList()
        {
            List<MonHoc> monHocList = new List<MonHoc>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM monHoc";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            monHocList.Add(new MonHoc
                            {
                                MaMonHoc = Convert.ToInt32(reader["maMonHoc"]),
                                TenMonHoc = reader["tenMonHoc"].ToString(),
                                IdGiaoVien = Convert.ToInt32(reader["idGiaoVien"])
                            });
                        }
                    }
                }
            }
            return monHocList;
        }
        public List<GiaoVien> GetGiaoVienList()
        {
            List<GiaoVien> giaoVienList = new List<GiaoVien>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM giaoVien";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            giaoVienList.Add(new GiaoVien
                            {
                                IdGiaoVien = Convert.ToInt32(reader["idGiaoVien"]),
                                HoVaTen = reader["hoVaTen"].ToString(),
                                GioiTinh = reader["gioiTinh"].ToString(),
                                NgaySinh = reader["ngaySinh"].ToString(),
                                TrinhDo = reader["trinhDo"].ToString(),
                                Sdt = reader["sdt"].ToString(),
                                Mail = reader["mail"].ToString(),
                                DiaChi = reader["diaChi"].ToString(),
                                BoMonGiangDay = reader["boMonGiangDay"].ToString(),
                                ChucVu = reader["chucVu"].ToString(),
                            });
                        }
                    }
                }
            }
            return giaoVienList;
        }
        public List<DiemSo> GetDiemSoList()
        {
            List<DiemSo> diemSoList = new List<DiemSo>();
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM diemSo";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diemSoList.Add(new DiemSo
                            {
                                MaMonHoc = Convert.ToInt32(reader["maMonHoc"]),
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
                                DiemTB = Convert.ToDouble(reader["diemTB"]),
                            });
                        }
                    }
                }
            }
            return diemSoList;
        }
        public List<DiemSo> GetDiemSoHocSinhList(string lopFilter, int hocKyFilter)
        {
            List<DiemSo> diemSoList = new List<DiemSo>();

            // Lấy giá trị mã học sinh từ KeyUser
            string idHocSinh = KeyUser.KeyUserSave;

            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                // Câu truy vấn với INNER JOIN và bộ lọc động
                string query = @"
            SELECT 
                ds.*, 
                mh.tenMonHoc 
            FROM 
                diemSo ds 
            INNER JOIN 
                monHoc mh 
            ON 
                ds.maMonHoc = mh.maMonHoc
            WHERE 
                ds.idHocSinh = @idHocSinh";

                // Thêm các điều kiện bộ lọc nếu có giá trị
                if (!string.IsNullOrEmpty(lopFilter))
                {
                    query += " AND ds.lop = @lopFilter";
                }

                if (hocKyFilter > 0)
                {
                    query += " AND ds.hocKy = @hocKyFilter";
                }

                using (var command = new SQLiteCommand(query, connection))
                {
                    // Gán tham số
                    command.Parameters.AddWithValue("@idHocSinh", idHocSinh);

                    if (!string.IsNullOrEmpty(lopFilter))
                    {
                        command.Parameters.AddWithValue("@lopFilter", lopFilter);
                    }

                    if (hocKyFilter > 0)
                    {
                        command.Parameters.AddWithValue("@hocKyFilter", hocKyFilter);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diemSoList.Add(new DiemSo
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
                                DiemTB = Convert.ToDouble(reader["diemTB"]),
                            });
                        }
                    }
                }
            }

            return diemSoList;
        }


    }

    public class HocSinh
    {
        public int IdHocSinh { get; set; }
        public string HoVaTen { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; } // Dạng chuỗi
        public int NamNhapHoc { get; set; }
        public int MaLop { get; set; }
        public string Sdt { get; set; }
        public string DiaChi { get; set; }
    }
    public class GiaoVien
    {
        public int IdGiaoVien { get; set; }
        public string HoVaTen { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; } // Dạng chuỗi
        public string TrinhDo { get; set; }
        public string Sdt { get; set; }
        public string Mail { get; set; }
        public string DiaChi { get; set; }
        public string BoMonGiangDay { get; set; }
        public string ChucVu { get; set; }
    }

    public class MonHoc
    {
        public int MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }
        public int IdGiaoVien { get; set; } // Khóa ngoại từ GiaoVien
        public string TenGiaoVien { get; set; }
    }
    public class LopHoc
    {
        public int IdLopHoc { get; set; }
        public string TenLop { get; set; }
        public int? IdHocSinh { get; set; } // Khóa ngoại từ HocSinh (nullable)
        public string TenHocSinh { get; set; }
        public int IdGiaoVien { get; set; } // Khóa ngoại từ GiaoVien
        public int SiSoLop { get; set; }
        public string TenGiaoVien { get; set; }
    }
    public class DiemSo
    {
        public int MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }// Khóa ngoại từ MonHoc
        public int IdHocSinh { get; set; } // Khóa ngoại từ HocSinh
        public string TenHocSinh { get; set; }
        public string Lop { get; set; }
        public int HocKy { get; set; }
        public double? DiemThuongXuyen1 { get; set; }
        public double? DiemThuongXuyen2 { get; set; }
        public double? DiemThuongXuyen3 { get; set; }
        public double? DiemThuongXuyen4 { get; set; }
        public double? DiemThuongXuyen5 { get; set; }

        public double? DiemGiuaKy { get; set; }
        public double? DiemThi { get; set; }
        public double? DiemTB { get; set; }
    }
    public class TaiKhoanNguoiDung
    {
        public string TenNguoiDung { get; set; }
        public string MatKhau { get; set; }
        public string QuyenTruyCap { get; set; } // admin, student, teacher
        public string Email { get; set; }
    }
    public class UserLog
    {
        public int Id { get; set; }
        public string TenNguoiDung { get; set; } // Khóa ngoại từ TaiKhoanNguoiDung
        public string NoiDung { get; set; }
        public string DauThoiGian { get; set; } // Dạng chuỗi
    }

}
