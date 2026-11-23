# CleanLink (v1.0.0)

Công cụ nhỏ gọn viết bằng **C# (.NET Framework 3.5)** giúp xóa toàn bộ khoảng trắng khỏi các đường link bị chèn dấu cách (ví dụ: `https:/ /drive . google. com/...`) và tự động mở bằng trình duyệt mặc định.

## Tính năng chính
- **Độc lập hoàn toàn**: Không phụ thuộc file `.bat` hay script ngoài, chạy trực tiếp bằng `CleanLink.exe`.
- **Xóa khoảng trắng & Mở**: Dán link có khoảng trắng, nhấn `Enter` hoặc bấm nút để làm sạch và mở ngay trong trình duyệt.
- **Dán từ Clipboard & Mở**: Đọc nhanh link từ Clipboard, làm sạch và truy cập chỉ với 1 click.
- **Hỗ trợ dòng lệnh (CLI)**: Có thể truyền trực tiếp link qua tham số dòng lệnh (`CleanLink.exe "https:/ /example . com"`).
- **Kiểm tra URL an toàn**: Chỉ mở các đường dẫn hợp lệ có giao thức `http://` hoặc `https://`.

## Biên dịch (.NET Framework 3.5)
```powershell
C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe CleanLink.csproj /p:Configuration=Release
```
