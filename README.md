# API SMe(ShopeeMe) (Đồ án tốt nghiệp) (v1.0.1)
# Công nghệ: ASP.NET Core MVC  7.0
# API là gì?
API là các phương thức, giao thức kết nối với các thư viện và ứng dụng khác. Nó là viết tắt của Application Programming Interface – giao diện lập trình ứng dụng. API cung cấp khả năng truy xuất đến một tập các hàm hay dùng. Và từ đó có thể trao đổi dữ liệu giữa các ứng dụng.
![image](https://github.com/user-attachments/assets/eac50dc0-8fef-4229-a6cc-432bb84cfe62)
# API Quản lý một số module
## 1. Quản lý tài khoản
### 1.1. Lấy tất cả tài khoản
http://shopeeme.somee.com/user/
### 1.2. Lấy tài khoản với mã
http://shopeeme.somee.com/user?userID=1
### 1.3. Đăng nhập tài khoản
http://shopeeme.somee.com/user/login?email=cong@gmail.com&password=Cong23012002$
### 1.4. Lấy mật khẩu tài khoản với email
http://shopeeme.somee.com/user/forgot?email=cong@gmail.com
### 1.5. Hồ sơ tài khoản
http://shopeeme.somee.com/user/profile?userID=1
### 1.6. Lấy tất cả tài khoản người bán
http://shopeeme.somee.com/seller
### 1.7. Lấy tài khoản người bán với mã
http://shopeeme.somee.com/seller?sellerID=6
### 1.8. Đăng nhập tài khoản người bán
http://shopeeme.somee.com/seller/login?phone=23012002&password=1
### 1.9. Đăng ký tài khoản người bán
http://shopeeme.somee.com/seller/register?phone=23012002&username=laneige.vn&password=1
### 1.10. Lấy lại mật khẩu tài khoản người mua với số điện thoại
http://shopeeme.somee.com/seller/forgot?phone=23012002
### 1.11. Đổi mật khẩy tài khoản người mua
http://shopeeme.somee.com/seller/change?sellerID=6&oldPassword=10&newPassword=20
## 2. Quản lý cửa hàng
### 2.1. Cửa hàng theo tên đăng nhập
http://shopeeme.somee.com/shop?shopUsername=laneige.vn&userID=1
## 3. Quản lý ngành hàng
### 3.1. Tìm ngành hàng/thể loại theo từ khoá
http://shopeeme.somee.com/home/search?keyword=c
## 4. Quản lý sản phẩm
### 4.1. Sản phẩm gợi ý
http://shopeeme.somee.com/home/suggest
### 4.2 Sản phẩm theo mã ngành
http://shopeeme.somee.com/product?parentCategoryID=1
### 4.3 Chi tiết sản phẩm
http://shopeeme.somee.com/product/detail?productID=2
### 4.4. Sản phẩm theo mã danh mục và sắp xếp theo giá tăng dần
http://shopeeme.somee.com/product/sort/?categoryID=1&sortType=asc
### 4.5. Sản phẩm theo mã danh mục và sắp xếp theo giá giảm dần
http://shopeeme.somee.com/product/sort/?categoryID=1&sortType=desc
## 5. Quản lý mua hàng
### 5.1. Giỏ hàng theo mã tài khoản
http://shopeeme.somee.com/cart?userID=1
















