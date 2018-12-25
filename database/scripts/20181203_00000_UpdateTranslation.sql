/*
	CreatedDate: 20181203@Alvin
	Task: Insert multiple languages into Resource table
	DB: DBBOT/bodb_ChatBot
*/

USE [bodb_ChatBot]
GO

--vi-VN
INSERT INTO dbo.Resource([Key], [Type], LanguageCode, [Value])
SELECT 1, 'Action', 'vi-VN', N'Xem Báo Cáo Tiền Chưa Xử Lý'
UNION ALL
SELECT 2, 'Action', 'vi-VN', N'Xem Thắng Thua Trong Ngày'
UNION ALL
SELECT 3, 'Action', 'vi-VN', N'Xem Tài Khoản'
UNION ALL
SELECT 4, 'Action', 'vi-VN', N'Cấp Dưới Trực Tiếp Cụ Thể'
UNION ALL
SELECT 5, 'Action', 'vi-VN', N'Top 200 Tiền Chưa Xử Lý'
UNION ALL
SELECT 6, 'Action', 'vi-VN', N'Vui lòng nhập tên tài khoản'
