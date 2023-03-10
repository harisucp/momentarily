USE [Momentarily]
GO
DELETE FROM [dbo].[s_settings]
GO
SET IDENTITY_INSERT [dbo].[s_settings] ON 

--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (1, N'SMTP_HOST', N'smtp.sendgrid.net')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (2, N'SMTP_PORT', N'587')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (3, N'SMTP_USER', N'momentarily')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (4, N'SMTP_PWD', N'Telekinesis101')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (5, N'SMTP_FROM', N'no-reply@momentarily.com')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (6, N'SMTP_FROM_NAME', N'momentarily team')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (7, N'EMAIL_CONTACTUS', N'hello@momentarily.com')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (8, N'EMAIL_ADMIN', N'admin@momentarily.com')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (9, N'IS_PRODUCTION', N'false')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (10, N'AWS_S3_BUCKET_NAME', N'')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (11, N'AWS_S3_ACCESS_KEY', N'')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (12, N'AWS_S3_SECRET_KEY', N'')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (13, N'AWS_S3_PRE_SIGNED_URL_TIMEOUT', N'0')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (14, N'IMG_FILE_SERVER_URL', N'/Content/Images')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (15, N'SMS_PROVIDER_LOGIN', N'')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (16, N'SMS_PROVIDER_PWD', N'')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (17, N'SMS_PROVIDER_LOGIN_ALT', N'')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (18, N'SMS_PROVIDER_PWD_ALT', N'')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (19, N'PAYPAL_CLIENT_ID', N'AeQA8K5V8HbiCNEbpR2cbgbBT9QEeZzvKueji0cYUPE70megD_CfYnE7txnLDfA9Ut025lMK6O7nVd2O')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (20, N'PAYPAL_CLIENT_SECRET', N'EOWu5TU_sQo0LPgJoHVw68NJpTSZntCH4JeLAcdSqB_IQc-7NOzLPvw5xBf0nFkz36VqG9hUxMKmGxAE')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (21, N'PAYPAL_MODE', N'sandbox')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (22, N'PAYPAL_CONNECTION_TIMEOUT', N'360000')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (23, N'PAYPAL_REQUEST_RETRIES', N'5')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (24, N'PAYPAL_ACCOUNT1_API_USERNAME', N'dkosyak-facilitator_api1.gmail.com')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (25, N'PAYPAL_ACCOUNT1_API_PASSWORD', N'U59DQXJS7CYFN272')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (26, N'PAYMENT_TRANSACTION_COMMISION', N'0.1')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (27, N'PAYPAL_ACCOUNT1_API_SIGNATURE', N'AFcWxV21C7fd0v3bYYYRCpSSRl31Az6JOZdxVltFQYGogKR.9LL.IMT.')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (28, N'PAYPAL_ACCOUNT1_API_APP_ID', N'APP-80W284485P519543T')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (29, N'PAYPAL_EMAIL_ID', N'dkosyak-facilitator@gmail.com')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (30, N'PAYMENT_TRANSACTION_CURRENCY', N'AUD')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (31, N'GOOGLE_CLIENT_ID', N'')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (32, N'GOOGLE_CLIENT_SECRET', N'')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (33, N'FACEBOOK_CLIENT_ID', N'')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (34, N'FACEBOOK_CLIENT_SECRET', N'')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (35, N'PIN_TEST_MODE', N'true')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (36, N'PIN_LIVE_SECRET_KEY', N'lEdSpqfMouB13SX7ZipelQ')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (37, N'PIN_LIVE_PUBLISH_KEY', N'pk_qanH2jKnFKA9_BakD60YkA')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (38, N'PIN_TEST_SECRET_KEY', N'ud7wRxnycUx7-1Pz3wKYIw')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (39, N'PIN_TEST_PUBLISH_KEY', N'pk_FeEXQYCYjUavmVHxUG4TwQ')
--INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (40, N'HOST', N'http://momentarily.empeek.net/')
SET IDENTITY_INSERT [dbo].[s_settings] OFF


GO
DELETE FROM [dbo].[s_dns]
GO
SET IDENTITY_INSERT [dbo].[s_dns] ON 


INSERT [dbo].[s_dns] ([dns_id],[dns], [default_lang_id], [is_default]) VALUES (1, N'info', 1,1)
SET IDENTITY_INSERT [dbo].[s_dns] OFF
