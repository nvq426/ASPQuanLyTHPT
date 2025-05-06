using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyTHPT.Pages.Admin
{
    public class TrangChuModel : PageModel
    {
        public void OnGet()
        {
            // Logic khi trang được tải (nếu cần)
        }

        public IActionResult OnPostQuanLyHocSinh()
        {
            return RedirectToPage("/Admin/QuanLyHocSinh/QuanLyHocSinh");
        }

        public IActionResult OnPostQuanLyLopHoc()
        {
            return RedirectToPage("/Admin/QuanLyLopHoc/QuanLyLopHoc");
        }

        public IActionResult OnPostQuanLyTaiKhoan()
        {
            return RedirectToPage("/Admin/QuanLyTaiKhoan/QuanLyTaiKhoan");
        }

        public IActionResult OnPostQuanLyMonHoc()
        {
            return RedirectToPage("/Admin/QuanLyMonHoc/QuanLyMonHoc");
        }

        public IActionResult OnPostQuanLyGiaoVien()
        {
            return RedirectToPage("/Admin/QuanLyGiaoVien/QuanLyGiaoVien");
        }

        public IActionResult OnPostQuanLyDiemSo()
        {
            return RedirectToPage("/Admin/QuanLyDiemSo/QuanLyDiemSo");
        }

        public IActionResult OnPostLogout()
        {
            // Xử lý đăng xuất: Có thể xóa session hoặc thông tin đăng nhập (nếu có)
            // Ví dụ: HttpContext.Session.Clear(); (nếu dùng session)

            // Điều hướng về trang đăng nhập
            return RedirectToPage("/Index");
        }
    }
}