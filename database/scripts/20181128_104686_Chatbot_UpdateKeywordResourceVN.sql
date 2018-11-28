/*
	CreatedDate: 20181128@Alvin
	Task: Update Keyword's Resources vi-VN
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

UPDATE dbo.[Resource]
SET Value = N'chưa xử lý,báo cáo,đang diễn ra,chưa kết thúc,đang chạy,cược,bao cao,baocao,bc,chua xu ly,dang dien ra,chua ket thuc,dang chay,cuoc,kết quả,ket qua,kq'
WHERE [Key] = 1
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'vi-VN'

UPDATE dbo.[Resource]
SET Value = N'thắng,thua,báo cáo,hôm nay,thang,thua,baocao,bao cao,hom nay,homnay'
WHERE [Key] = 2
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'vi-VN'

UPDATE dbo.[Resource]
SET Value = N'số dư,tài khoản,báo cáo,hạn mức,còn lại,so du,sodu,tk,tai khoan,taikhoan,han muc,hanmuc,con lai'
WHERE [Key] = 3
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'vi-VN'
