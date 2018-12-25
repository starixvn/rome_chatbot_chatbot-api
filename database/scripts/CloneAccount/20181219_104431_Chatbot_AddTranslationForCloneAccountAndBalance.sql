/*
	CreatedDate: 20181219@Alvin
	Task: Add translations for Clone Account & Balance
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

INSERT INTO dbo.[Resource] ([Key], [Type], [LanguageCode], [Value])
SELECT 9, 'Function', 'zh-CN', N'从现有的账号复制新的会员。'
UNION ALL
SELECT 9, 'Function', 'zh-TW', N'從現有的賬號復制新的會員。'
UNION ALL
SELECT 9, 'Function', 'th-TH', N'สร้างสมาชิกใหม่จากบัญชีที่มีอยู่'
UNION ALL
SELECT 7, 'Action', 'zh-CN', N'从现有的账号复制新的总代理'
UNION ALL
SELECT 7, 'Action', 'zh-TW', N'從現有的賬號復制新的總代理'
UNION ALL
SELECT 7, 'Action', 'th-TH', N'สร้างบัญชีมาสเตอร์ใหม่จากบัญชีที่มีอยู่'
UNION ALL
SELECT 8, 'Action', 'zh-CN', N'请输入现有账号。'
UNION ALL
SELECT 8, 'Action', 'zh-TW', N'請輸入現有賬號。'
UNION ALL
SELECT 8, 'Action', 'th-TH', N'โปรดกรอกบัญชีที่มีอยู่'
UNION ALL
SELECT 9, 'Action', 'zh-CN', N'请为新的总代理输入信用额度。'+ CHAR(13) + CHAR(10) + N'您最高可以给予 {0}'
UNION ALL
SELECT 9, 'Action', 'zh-TW', N'請為新的總代理輸入信用額度。'+ CHAR(13) + CHAR(10) + N'您最高可以給予 {0}'
UNION ALL
SELECT 9, 'Action', 'th-TH', N'โปรดกรอกวงเงินเครดิตสำหรับมาสเตอร์ใหม่'+ CHAR(13) + CHAR(10) + N'คุณสามารถให้ได้ถึง {0}'
UNION ALL
SELECT 10, 'Action', 'zh-CN', N'复制 #actionid_8, 信用额度 #actionid_9'
UNION ALL
SELECT 10, 'Action', 'zh-TW', N'復制 #actionid_8, 信用額度 #actionid_9'
UNION ALL
SELECT 10, 'Action', 'th-TH', N'คัดลอก #actionid_8, วงเงินเครดิต #actionid_9'
UNION ALL
SELECT 11, 'Action', 'zh-CN', N'请为复制中的 #actionid_8 再输入信用额度'
UNION ALL
SELECT 11, 'Action', 'zh-TW', N'請為復制中的 #actionid_8 再輸入信用額度'
UNION ALL
SELECT 11, 'Action', 'th-TH', N'ป้อนวงเงินเครดิตอีกครั้ง สำหรับคัดลอก #actionid_8'
UNION ALL
SELECT 12, 'Action', 'zh-CN', N'从现有的账号复制新的代理'
UNION ALL
SELECT 12, 'Action', 'zh-TW', N'從現有的賬號復制新的代理'
UNION ALL
SELECT 12, 'Action', 'th-TH', N'สร้างบัญชีเอเย่นต์ใหม่จากบัญชีที่มีอยู่'
UNION ALL
SELECT 13, 'Action', 'zh-CN', N'请输入现有账号。'
UNION ALL
SELECT 13, 'Action', 'zh-TW', N'請輸入現有賬號。'
UNION ALL
SELECT 13, 'Action', 'th-TH', N'โปรดกรอกบัญชีที่มีอยู่'
UNION ALL
SELECT 14, 'Action', 'zh-CN', N'请为新的代理输入信用额度。'+ CHAR(13) + CHAR(10) + N'您最高可以给予 {0}'
UNION ALL
SELECT 14, 'Action', 'zh-TW', N'請為新的代理輸入信用額度。'+ CHAR(13) + CHAR(10) + N'您最高可以給予  {0}'
UNION ALL
SELECT 14, 'Action', 'th-TH', N'โปรดกรอกวงเงินเครดิตสำหรับเอเย่นต์ใหม่'+ CHAR(13) + CHAR(10) + N'คุณสามารถให้ได้ถึง {0}'
UNION ALL
SELECT 15, 'Action', 'zh-CN', N'请输入代理可以提供给会员的最高信用额度。'+ CHAR(13) + CHAR(10) + N'会员最高信用额度为 {0}。'
UNION ALL
SELECT 15, 'Action', 'zh-TW', N'請輸入代理可以提供給會員的最高信用額度。'+ CHAR(13) + CHAR(10) + N'會員最高信用額度為 {0}。'
UNION ALL
SELECT 15, 'Action', 'th-TH', N'โปรดระบุวงเงินเครดิตสูงสุดที่เอเย่นสามารถให้สมาชิก. '+ CHAR(13) + CHAR(10) + N'วงเงินเครดิตสูงสุดของสมาชิกคือ {0}.'
UNION ALL
SELECT 16, 'Action', 'zh-CN', N'复制 #actionid_13, 信用额度 #actionid_14, 会员最高信用额 #actionid_15'
UNION ALL
SELECT 16, 'Action', 'zh-TW', N'復制 #actionid_13, 信用額度 #actionid_14,  會員最高信用額 #actionid_15'
UNION ALL
SELECT 16, 'Action', 'th-TH', N'คัดลอก #actionid_13, เครดิต #actionid_14, เครดิตสมาชิกสูงสุด #actionid_15'
UNION ALL
SELECT 17, 'Action', 'zh-CN', N'请为复制中的 #actionid_13 再输入信用额度'
UNION ALL
SELECT 17, 'Action', 'zh-TW', N'請為復制中的 #actionid_13 再輸入信用額度'
UNION ALL
SELECT 17, 'Action', 'th-TH', N'ป้อนวงเงินเครดิตอีกครั้ง สำหรับคัดลอก #actionid_13'
UNION ALL
SELECT 19, 'Action', 'zh-CN', N'从现有的账号复制新的会员。'
UNION ALL
SELECT 19, 'Action', 'zh-TW', N'從現有的賬號復制新的會員。'
UNION ALL
SELECT 19, 'Action', 'th-TH', N'สร้างสมาชิกใหม่จากบัญชีที่มีอยู่'
UNION ALL
SELECT 20, 'Action', 'zh-CN', N'请输入现有账号。'
UNION ALL
SELECT 20, 'Action', 'zh-TW', N'請輸入現有賬號。'
UNION ALL
SELECT 20, 'Action', 'th-TH', N'โปรดกรอกบัญชีที่มีอยู่'
UNION ALL
SELECT 21, 'Action', 'zh-CN', N'请为新的会员输入信用额度。'+ CHAR(13) + CHAR(10) + N'您最高可以给予 {0}'
UNION ALL
SELECT 21, 'Action', 'zh-TW', N'請為新的會員輸入信用額度。'+ CHAR(13) + CHAR(10) + N'您最高可以給予  {0}'
UNION ALL
SELECT 21, 'Action', 'th-TH', N'โปรดกรอกเครดิตสำหรับสมาชิกใหม่'+ CHAR(13) + CHAR(10) + N'คุณสามารถให้ได้ถึง {0}'
UNION ALL
SELECT 22, 'Action', 'zh-CN', N'复制 #actionid_20, 信用额度 #actionid_21'
UNION ALL
SELECT 22, 'Action', 'zh-TW', N'復制 #actionid_20, 信用額度 #actionid_21'
UNION ALL
SELECT 22, 'Action', 'th-TH', N'คัดลอก #actionid_20, วงเงินเครดิต #actionid_21'
UNION ALL
SELECT 23, 'Action', 'zh-CN', N'请为复制中的 #actionid_20 再输入信用额度'
UNION ALL
SELECT 23, 'Action', 'zh-TW', N'請為復制中的 #actionid_20 再輸入信用額度'
UNION ALL
SELECT 23, 'Action', 'th-TH', N'ป้อนวงเงินเครดิตอีกครั้ง สำหรับคัดลอก #actionid_20'
UNION ALL
SELECT 25, 'Action', 'zh-CN', N'我的余额资料'
UNION ALL
SELECT 25, 'Action', 'zh-TW', N'我的余額資料'
UNION ALL
SELECT 25, 'Action', 'th-TH', N'ข้อมูลยอดคงเหลือของฉัน'
UNION ALL
SELECT 26, 'Action', 'zh-CN', N'前200名 余额资料'
UNION ALL
SELECT 26, 'Action', 'zh-TW', N'前200名 余額資料'
UNION ALL
SELECT 26, 'Action', 'th-TH', N'ข้อมูลยอดคงเหลือ 200 อันดับแรก'
