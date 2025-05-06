# ASPQuanLyTHPT

## Tổng quan

**ASPQuanLyTHPT** là một ứng dụng web được xây dựng bằng **ASP.NET Core Razor Pages**, hỗ trợ quản lý trường học THPT. Dự án cung cấp các chức năng như quản lý học sinh, điểm số, lớp học, môn học, giáo viên và tài khoản người dùng. Ngoài ra, còn có giao diện riêng dành cho học sinh để xem điểm cá nhân.

Ứng dụng sử dụng **SQLite (dataTHPT.db)** làm cơ sở dữ liệu và được thiết kế với giao diện trực quan, thân thiện, hỗ trợ thêm/sửa/xóa, lọc dữ liệu và xuất CSV.

---

## Tính năng chính

### 1. Đăng nhập
- Hỗ trợ đăng nhập cho **Admin** và **Học sinh**.
- Xác thực từ bảng `takhoan_nguoidung`.

### 2. Quản lý Admin
#### Quản lý học sinh
- Xem danh sách, lọc theo lớp, tìm kiếm theo tên/số điện thoại.
- Thêm, sửa, xóa, xuất CSV.

#### Quản lý điểm số
- Lọc theo mã môn, khối lớp, mã học sinh, học kỳ.
- Thêm, sửa, xóa điểm.
- Tính điểm trung bình:  
  `((TX1 + TX2 + TX3 + TX4 + TX5) + (Giữa kỳ * 2) + (Thi * 3)) / 10`

#### Quản lý dự kiến
- **Lớp học**: Xem/thêm/sửa/xóa, lọc theo tên lớp.
- **Môn học**: Xem/thêm/sửa/xóa, lọc theo tên môn hoặc giáo viên.
- **Giáo viên**: Xem/thêm/sửa/xóa, lọc theo bộ môn hoặc chức vụ.
- **Tài khoản người dùng**: Quản lý tài khoản admin, học sinh, giáo viên.

### 3. Trang học sinh
- Xem thông tin cá nhân.
- Xem điểm, lọc theo môn, lớp (10/11/12), và học kỳ (HK1, HK2, Cả năm).

---

## Công nghệ sử dụng
- **ASP.NET Core Razor Pages** – Framework chính.
- **SQLite** – Cơ sở dữ liệu.
- **HTML/CSS/JavaScript** – Giao diện người dùng.
- **Git/GitHub** – Quản lý mã nguồn.

---

## Cài đặt và chạy dự án

### 1. Yêu cầu
- **.NET SDK** 6.0 trở lên.
- **Git** để clone mã nguồn.
- **SQLite** để chạy cơ sở dữ liệu.

### 2. Clone repository
```bash
git clone https://github.com/nvq426/ASPQuanLyTHPT.git
cd ASPQuanLyTHPT
