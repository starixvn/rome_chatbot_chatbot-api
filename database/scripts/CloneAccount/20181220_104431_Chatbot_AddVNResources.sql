INSERT INTO dbo.[Resource] ([Key], [Type], [LanguageCode], [Value])
SELECT 9, 'Function', 'vi-VN', N'Tạo mới tài khoản Member dựa trên thiết lập của tài khoản có sẵn.'
UNION ALL
SELECT 7, 'Action', 'vi-VN', N'Tạo mới tài khoản Master dựa trên thiết lập của tài khoản có sẵn.'
UNION ALL
SELECT 8, 'Action', 'vi-VN', N'Vui lòng nhập tên tài khoản muốn sao chép'
UNION ALL
SELECT 9, 'Action', 'vi-VN', N'Vui lòng nhập mức tín dụng cho tài khoản Master được tạo.'+ CHAR(13) + CHAR(10) + N'Mức tín dụng có thể cho cấp dưới lên tới {0}'
UNION ALL
SELECT 10, 'Action', 'vi-VN', N'Sao chép từ #actionid_8, mức tín dụng #actionid_9'
UNION ALL
SELECT 11, 'Action', 'vi-VN', N'Nhập lại mức tín dụng cho tài khoản #actionid_8'
UNION ALL
SELECT 12, 'Action', 'vi-VN', N'Tạo mới tài khoản Agent dựa trên thiết lập của tài khoản có sẵn.'
UNION ALL
SELECT 13, 'Action', 'vi-VN', N'Vui lòng nhập tên tài khoản muốn sao chép'
UNION ALL
SELECT 14, 'Action', 'vi-VN', N'Vui lòng nhập mức tín dụng cho tài khoản Agent được tạo.'+ CHAR(13) + CHAR(10) + N'Mức tín dụng có thể cho cấp dưới lên tới {0}'
UNION ALL
SELECT 15, 'Action', 'vi-VN', N'Vui lòng nhập mức tín dụng tối đa mà Agent có thể cho Member.'+ CHAR(13) + CHAR(10) + N'Mức tín dụng tối đa cho Member lên tới {0}。'
UNION ALL
SELECT 16, 'Action', 'vi-VN', N'Sao chép từ #actionid_13, mức tín dụng #actionid_14, mức tín dụng tối đa có thể cấp cho Member là #actionid_15'
UNION ALL
SELECT 17, 'Action', 'vi-VN', N'Nhập lại mức tín dụng cho tài khoản #actionid_13'
UNION ALL
SELECT 19, 'Action', 'vi-VN', N'Tạo mới tài khoản Member dựa trên thiết lập của tài khoản có sẵn.'
UNION ALL
SELECT 20, 'Action', 'vi-VN', N'Vui lòng nhập tên tài khoản muốn sao chép'
UNION ALL
SELECT 21, 'Action', 'vi-VN', N'Vui lòng nhập mức tín dụng cho tài khoản Member được tạo.'+ CHAR(13) + CHAR(10) + N'Mức tín dụng có thể cho cấp dưới lên tới {0}'
UNION ALL
SELECT 22, 'Action', 'vi-VN', N'Sao chép từ #actionid_20, mức tín dụng #actionid_21'
UNION ALL
SELECT 23, 'Action', 'vi-VN', N'Nhập lại mức tín dụng cho tài khoản #actionid_20'
UNION ALL
SELECT 25, 'Action', 'vi-VN', N'Thông tin tài khoản cá nhân'
UNION ALL
SELECT 26, 'Action', 'vi-VN', N'Thông tin Top 200 tài khoản'
