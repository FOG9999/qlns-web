

### LOG DEBUG

-------
 
#### 19.04.2018

- **BUG**
  + #capphat: lỗi không có chỉ tiêu, không nhập được cấp phát cho ngân sách đặc biệt  3010000.

- **FIX**:
  + #dtbs:
    + Sửa biểu quyết toán năm quý, chỉ tiêu 1040100 tách ra ngoại tệ, hàng mua
  + #quyettoan: 
    + sửa lỗi không xem được danh sách chứng từ
    + mặc định lúc đầu hiển thị tất cả chứng từ (không lọc, theo mã trạng thái)
  + fix lỗi logout ra bị lỗi -> đổi master page, không có *LogonUserControl*
  
- **ADD**:
  + #dtbs:
    + Biểu tổng hợp chỉ tiêu, tách ra các loại: tổng hợp, đầu năm và bổ sung
    + Thêm function: f_ns_chitieu
 
---

#### 18.04.2018

- **fix**:
  + #capphat: không nhập được cột rPhanCap, rDuPhong do mlns chưa cho quyền nhập
    + vào mục lục ngân sách -> cấu hình
    + tích và bấm Lưu để cập nhật cấu hình cho lns cần chỉnh sửa.
    + => #bug
  + #dtbs: fix báo cáo tổng hợp chỉ tiêu đơn vị và báo cáo quyết toán năm quý -> xong
- **add**:
  + thêm **markdown** log file
 



