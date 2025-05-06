using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using QuanLyTHPT;

namespace QuanLyTHPT.Pages.Admin
{
    public class QuanLyGiaoVienModel : PageModel
    {
        public List<GiaoVien> GiaoVienList { get; set; }
        public List<GiaoVien> OriginalGiaoVienList { get; set; }
        [BindProperty]
        public string SearchText { get; set; }

        public void OnGet()
        {
            LoadGiaoVienList();
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Admin/TrangChu");
        }

        public IActionResult OnPostSearch()
        {
            LoadGiaoVienList();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                GiaoVienList = GiaoVienList
                    .Where(gv => gv.HoVaTen.ToLower().Contains(SearchText.Trim().ToLower()) ||
                                 gv.Sdt.ToLower().Contains(SearchText.Trim().ToLower()))
                    .ToList();
            }
            return Page();
        }

        public IActionResult OnPostExport()
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID Giáo viên,Họ và tên,Giới tính,Ngày sinh,Trình độ,Số điện thoại,Email,Địa chỉ,Bộ môn giảng dạy,Chức vụ");

            LoadGiaoVienList();
            foreach (var gv in GiaoVienList)
            {
                csv.AppendLine($"{gv.IdGiaoVien},{gv.HoVaTen},{gv.GioiTinh},{gv.NgaySinh},{gv.TrinhDo},{gv.Sdt},{gv.Mail},{gv.DiaChi},{gv.BoMonGiangDay},{gv.ChucVu}");
            }

            var fileName = $"DanhSachGiaoVien_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var fileContent = Encoding.UTF8.GetBytes(csv.ToString());
            return File(fileContent, "text/csv", fileName);
        }

        public IActionResult OnPostAddGiaoVien(int idGiaoVien, string hoVaTen, string gioiTinh, string ngaySinh, string trinhDo, string sdt, string mail, string diaChi, string boMonGiangDay, string chucVu)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                // Kiểm tra xem idGiaoVien đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM giaoVien WHERE idGiaoVien = @idGiaoVien";
                using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        TempData["ErrorMessage"] = "Giáo viên với ID này đã tồn tại. Vui lòng nhập ID khác!";
                        return RedirectToPage();
                    }
                }

                // Thêm giáo viên mới
                string query = "INSERT INTO giaoVien (idGiaoVien, hoVaTen, gioiTinh, ngaySinh, trinhDo, sdt, mail, diaChi, boMonGiangDay, chucVu) " +
                               "VALUES (@idGiaoVien, @hoVaTen, @gioiTinh, @ngaySinh, @trinhDo, @sdt, @mail, @diaChi, @boMonGiangDay, @chucVu)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);
                    command.Parameters.AddWithValue("@hoVaTen", hoVaTen);
                    command.Parameters.AddWithValue("@gioiTinh", gioiTinh);
                    command.Parameters.AddWithValue("@ngaySinh", ngaySinh);
                    command.Parameters.AddWithValue("@trinhDo", trinhDo);
                    command.Parameters.AddWithValue("@sdt", sdt);
                    command.Parameters.AddWithValue("@mail", mail);
                    command.Parameters.AddWithValue("@diaChi", diaChi);
                    command.Parameters.AddWithValue("@boMonGiangDay", boMonGiangDay);
                    command.Parameters.AddWithValue("@chucVu", chucVu);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        TempData["Message"] = "Thêm giáo viên thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không thể thêm giáo viên!";
                    }
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEditGiaoVien(int idGiaoVien, string hoVaTen, string gioiTinh, string ngaySinh, string trinhDo, string sdt, string mail, string diaChi, string boMonGiangDay, string chucVu)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                // Cập nhật thông tin giáo viên
                string query = "UPDATE giaoVien SET hoVaTen = @hoVaTen, gioiTinh = @gioiTinh, ngaySinh = @ngaySinh, trinhDo = @trinhDo, sdt = @sdt, mail = @mail, diaChi = @diaChi, boMonGiangDay = @boMonGiangDay, chucVu = @chucVu WHERE idGiaoVien = @idGiaoVien";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);
                    command.Parameters.AddWithValue("@hoVaTen", hoVaTen);
                    command.Parameters.AddWithValue("@gioiTinh", gioiTinh);
                    command.Parameters.AddWithValue("@ngaySinh", ngaySinh);
                    command.Parameters.AddWithValue("@trinhDo", trinhDo);
                    command.Parameters.AddWithValue("@sdt", sdt);
                    command.Parameters.AddWithValue("@mail", mail);
                    command.Parameters.AddWithValue("@diaChi", diaChi);
                    command.Parameters.AddWithValue("@boMonGiangDay", boMonGiangDay);
                    command.Parameters.AddWithValue("@chucVu", chucVu);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        TempData["Message"] = "Cập nhật giáo viên thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không thể cập nhật giáo viên!";
                    }
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteGiaoVien(int idGiaoVien)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                string query = "DELETE FROM giaoVien WHERE idGiaoVien = @idGiaoVien";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idGiaoVien", idGiaoVien);
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        TempData["Message"] = "Xóa giáo viên thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không thể xóa giáo viên!";
                    }
                }
            }
            return RedirectToPage();
        }

        private void LoadGiaoVienList()
        {
            OriginalGiaoVienList = new List<GiaoVien>();
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
                            OriginalGiaoVienList.Add(new GiaoVien
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
                                ChucVu = reader["chucVu"].ToString()
                            });
                        }
                    }
                }
            }
            GiaoVienList = OriginalGiaoVienList;
        }
    }
}