# Sàn TMĐT ShopeeMe(Shopee) (Đồ án tốt nghiệp) (v2.0.1)
# Công nghệ: ASP.NET Core MVC  7.0
- Luồng sử lý dữ liệu cơ bản: 
![ShopeeMe-SequenceDiagram-Software_Architecture drawio](https://github.com/user-attachments/assets/fd93c90b-e2d6-4118-b80e-6d50222115ce)
## Thành phần chính
### Routing
- Dựa vào các request Để gọi các Controller

### Controller
- Thực hiện các logic code từ yêu cầu cảu request và trả về response

### View
- Giao diện HTML, CSS được trả về theo logic của controller

### Model
- Được sử dụng để tương tác với các trường dữ liệu của bảng (định nghĩa các field, primary keys, foreign key,...)
- Khi tương tác với các dữ liệu có thể viết vào 1 file Repository riêng

## Khác
### Repository 
- Nơi tương tác với các dữ liệu của thực thể
- Được sử dụng để lấy dữ liệu và tương tác với dữ liệu của table (create, read, update, delete)

### Viết thủ tục lưu trong CSDL
  - Thủ tục được viết trên Server và khi truy vấn ta chỉ cần gọi thủ tục đó
### Quy tắc đặt tên trong CSDL
 - Tên Database: db_ (Ví dụ: db_F4_Shop)
 - Tên bảng: tbl_ (Ví dụ tbl_Categories)
 - Tên thủ tục: sp_ (Ví dụL sp_GetCategories)
 - ...
## Unit Testing (Kiểm thử đơn vị)
https://docs.google.com/spreadsheets/d/1ZXDqi9M9C59Rs9ZT_QM6zz7HuXFDN4jg/edit?gid=1570539229#gid=1570539229
## Kiến trúc phần mềm
![shopeeme-archi](https://github.com/user-attachments/assets/c461be70-c30e-4a33-b126-2302a792ae36)
## Kết quả thực hiện
### Trang chủ
![image](https://github.com/user-attachments/assets/eb06a394-b0ed-4570-bcb8-45f8e745fd63)
![image](https://github.com/user-attachments/assets/1cd4f0a6-0858-4344-a288-956671d47b4f)
![image](https://github.com/user-attachments/assets/5ff66ac7-e7e1-4d55-90d1-05fdfbca763b)
### Trang gợi ý
![image](https://github.com/user-attachments/assets/d65e204f-f6eb-460a-9474-bff4341ee124)
### Trang sản phẩm tương tự
![image](https://github.com/user-attachments/assets/fdb3fbc0-7f50-4dbf-b9c6-441d5061aebe)
### Trang sản phẩm
![image](https://github.com/user-attachments/assets/36781f9f-c2f7-4400-9d6c-90cf147400e2)
![image](https://github.com/user-attachments/assets/ba9e0646-d0a3-4858-87d5-df3d0c62dcc7)
### Trang cửa hàng
![image](https://github.com/user-attachments/assets/22da4dad-30e7-48e8-b229-652f06a0c0af)
![image](https://github.com/user-attachments/assets/7ae8e03e-8cf0-4f54-b682-9599475933bc)
### Chi tiết sản phẩm
![image](https://github.com/user-attachments/assets/5e4b3e4f-5d63-4b39-a626-c11df3d01db1)
![image](https://github.com/user-attachments/assets/8e5175c0-1441-47ef-be3e-5e6c12cc964d)
### Bình luận, đánh giá sản phẩm
![image](https://github.com/user-attachments/assets/98af65e6-b4b5-4be1-bbc9-ee21637a61bd)
![image](https://github.com/user-attachments/assets/0a17ec32-d067-4a57-980a-5e67cb23eac2)
### Giỏ hàng
![image](https://github.com/user-attachments/assets/a6c856b1-3982-4768-b432-0f95de7b5271)
![image](https://github.com/user-attachments/assets/7f609383-7b89-4666-852b-804b629dd700)
### Trạng thái đơn hàng
![image](https://github.com/user-attachments/assets/37ef7700-8ea7-4218-8c29-1796e4306f65)
![image](https://github.com/user-attachments/assets/7bf21a29-a781-44a6-933d-3535515946ac)
### Đơn mua
![image](https://github.com/user-attachments/assets/cfb3d74c-5037-4621-a277-1d1959bae48b)
### Kênh Sàn TMĐT
![image](https://github.com/user-attachments/assets/618f19b2-acfb-49a2-94c2-2870303803f4)
### Kênh người bán
![image](https://github.com/user-attachments/assets/414ed5a5-8e36-4e4c-b1bc-96d7897405c5)
### Kênh vận chuyển
![image](https://github.com/user-attachments/assets/57f5e4c1-e31c-45e8-8e7c-2f1432b45427)
![image](https://github.com/user-attachments/assets/646d0fb5-4030-4b81-bcc4-8d72456302c2)
![image](https://github.com/user-attachments/assets/02cad0c9-e2e3-4461-981b-cd18cc955e09)
![image](https://github.com/user-attachments/assets/aa972d6c-f0a6-4c47-b2e0-f9aac96c70d7)

















