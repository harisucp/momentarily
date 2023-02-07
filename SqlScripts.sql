
SET IDENTITY_INSERT [dbo].[s_settings] ON
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (1, N'SMTP_HOST', N'smtp.sendgrid.net')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (2, N'SMTP_PORT', N'587')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (3, N'SMTP_USER', N'Zuber.max.v@gmail.com')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (4, N'SMTP_PWD', N'samuraj15')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (5, N'SMTP_FROM', N'no-reply@momentarily.com')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (6, N'SMTP_FROM_NAME', N'momentarily team')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (7, N'EMAIL_CONTACTUS', N'hello@momentarily.com')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (8, N'EMAIL_ADMIN', N'admin@momentarily.com')
INSERT [dbo].[s_settings] ([id], [key], [value]) VALUES (26, N'PAYMENT_TRANSACTION_COMMISION', N'0.1')
SET IDENTITY_INSERT [dbo].[s_settings] OFF

SET IDENTITY_INSERT c_category ON
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (1, NULL, 1, N'Momentarily', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (2, 1, 0, N'Clothing/Fashion', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (3, 1, 0, N'Electronics', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (4, 1, 0, N'Home', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (5, 1, 0, N'Kitchen', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (6, 1, 0, N'Leisure/Outdoors', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (7, 1, 0, N'Tools', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (8, 1, 0, N'Space', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (9, 1, 0, N'Education', GETDATE(), GETDATE(), NULL, NULL)


SET IDENTITY_INSERT c_category OFF

SET IDENTITY_INSERT [dbo].[d_good_property_type] ON 
INSERT [dbo].[d_good_property_type] ([id], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (1, N'Dropdown', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[d_good_property_type] ([id], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (2, N'Location', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[d_good_property_type] ([id], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (3, N'Value', GETDATE(), GETDATE(), NULL, NULL)
SET IDENTITY_INSERT [dbo].[d_good_property_type] OFF

SET IDENTITY_INSERT [dbo].[d_good_property] ON 
INSERT [dbo].[d_good_property] ([id], [name], [type_id], [create_date], [mod_date], [create_by], [mod_by]) VALUES (1, N'MomentarilyItem_Type', 1, GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[d_good_property] ([id], [name], [type_id], [create_date], [mod_date], [create_by], [mod_by]) VALUES (2, N'MomentarilyItem_Location', 2, GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[d_good_property] ([id], [name], [type_id], [create_date], [mod_date], [create_by], [mod_by]) VALUES (3, N'MomentarilyItem_Deposit', 3, GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[d_good_property] ([id], [name], [type_id], [create_date], [mod_date], [create_by], [mod_by]) VALUES (4, N'MomentarilyItem_PickUpLocation', 2, GETDATE(), GETDATE(), NULL, NULL)
SET IDENTITY_INSERT [dbo].[d_good_property] OFF

SET IDENTITY_INSERT [dbo].[c_good_property_value_definition] ON 
INSERT [dbo].[c_good_property_value_definition] ([id], [good_property_id], [name], [value], [create_date], [mod_date], [create_by], [mod_by]) VALUES (1, 1, N'Equipment', N'Equipment', GETDATE(), GETDATE(), NULL, NULL)
SET IDENTITY_INSERT [dbo].[c_good_property_value_definition] OFF


--SET IDENTITY_INSERT [dbo].[c_request_status] ON 
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (1, N'Pending', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (2, N'Approved', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (3, N'Declined', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (4, N'Canceled', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (5, N'Paid', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (6, N'Released', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (7, N'Closed', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (8, N'Review', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (9, N'Deposit Charged', GETDATE(), GETDATE(), 0, 0)
INSERT [dbo].[c_request_status] ([id], [status_name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (10, N'Dispute', GETDATE(), GETDATE(), 0, 0)
--SET IDENTITY_INSERT [dbo].[c_request_status] OFF

ALTER TABLE [Momentarily].[dbo].[c_good_booking] ADD shipping_address varchar(4000)

ALTER TABLE [Momentarily].[dbo].[c_good] ADD
	[price_per_week] [float],
	[price_per_month] [float]

-- start add rent_period
ALTER TABLE [Momentarily].[dbo].[c_good] ADD
	[rent_period_day] [bit] NULL,
	[rent_period_week] [bit] NULL,
	[rent_period_month] [bit] NULL
GO

UPDATE [Momentarily].[dbo].[c_good] SET
	[rent_period_day] = 1,
	[rent_period_week] = 0,
	[rent_period_month] = 0
GO

ALTER TABLE [Momentarily].[dbo].[c_good] ALTER COLUMN [rent_period_day] [bit] NOT NULL
ALTER TABLE [Momentarily].[dbo].[c_good] ALTER COLUMN [rent_period_week] [bit] NOT NULL
ALTER TABLE [Momentarily].[dbo].[c_good] ALTER COLUMN [rent_period_month] [bit] NOT NULL
GO
--  end add rent_period

-- start add column [agree_to_deliver]
ALTER TABLE [Momentarily].[dbo].[c_good] ADD
	[agree_to_deliver] [bit] NULL
GO

UPDATE [Momentarily].[dbo].[c_good] SET
	[agree_to_deliver] = 0
GO

ALTER TABLE [Momentarily].[dbo].[c_good] ALTER COLUMN [agree_to_deliver] [bit] NOT NULL
GO
-- end add column [agree_to_deliver]


-- start add column [apply_for_delivery]
ALTER TABLE [Momentarily].[dbo].[c_good_booking] ADD
	apply_for_delivery [bit] NULL
GO

UPDATE [Momentarily].[dbo].[c_good_booking] SET
	apply_for_delivery = 0
GO

ALTER TABLE [Momentarily].[dbo].[c_good_booking] ALTER COLUMN apply_for_delivery [bit] NOT NULL
GO
-- end add column [apply_for_delivery]

-- ADD Braintree Recipient
USE [Momentarily]

CREATE TABLE [dbo].[p_braintree_recipient](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[merchant_account_id] [nvarchar](250) NOT NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [Momentarily].[dbo].[c_good_request] ADD 
	[days] [int],
	[days_cost] [float],
	[customer_cost] [float],
	[customer_service_fee] [float],
	[customer_service_fee_cost] [float],
	[customer_charity] [float],
	[customer_charity_cost] [float],
	[sharer_cost] [float],
	[sharer_service_fee] [float],
	[sharer_service_fee_cost] [float],
	[sharer_charity] [float],
	[sharer_charity_cost] [float],
	[dilivery_cost] [float],
	[shipping_distance] [float],
	[dilivery_price] [float]


ALTER TABLE [Momentarily].[dbo].[c_good_request] ADD 	
	[security_deposit] [float]

INSERT [dbo].[s_settings] ([key], [value]) VALUES (N'BRAINTREE_ENVIRONMENT', N'sandbox')
INSERT [dbo].[s_settings] ([key], [value]) VALUES (N'BRAINTREE_MASTERMERCHANTACCOUNTID', N'empeek')
INSERT [dbo].[s_settings] ([key], [value]) VALUES (N'BRAINTREE_MERCHANTID', N't9xcnr5rs965njsv')
INSERT [dbo].[s_settings] ([key], [value]) VALUES (N'BRAINTREE_PUBLICKEY', N'nn27z3jvmz8hr63r')
INSERT [dbo].[s_settings] ([key], [value]) VALUES (N'BRAINTREE_PRIVATEKEY', N'd8e05e997e4b8381cfb864a1bbdea3b2')


ALTER TABLE c_category ADD 
	[image_file_name] [nvarchar](250)

-- start add column [agree_to_deliver]
ALTER TABLE [Momentarily].[dbo].[c_good] ADD
	[agree_to_share_immediately] [bit] NULL
GO

UPDATE [Momentarily].[dbo].[c_good] SET
	[agree_to_share_immediately] = 0
GO

ALTER TABLE [Momentarily].[dbo].[c_good] ALTER COLUMN [agree_to_share_immediately] [bit] NOT NULL
GO
-- end add column [agree_to_deliver]

CREATE TABLE [dbo].[c_good_share_date](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[good_id] [int] NOT NULL,
	[share_date] [date] NOT NULL
)

ALTER TABLE [dbo].[c_good_share_date]  WITH CHECK ADD  CONSTRAINT [FK_c_good] FOREIGN KEY([good_id])
REFERENCES [dbo].[c_good] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

--update sendgrid credentials
UPDATE [dbo].[s_settings]
SET [value] = N'japonki15'
WHERE [key] = N'SMTP_PWD'
GO
