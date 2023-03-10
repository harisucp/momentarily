--USE [master]
--GO

--/****** Object:  Database [DoodjaDB] ******/
--IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'DoodjaDB')
--BEGIN
--CREATE DATABASE [DoodjaDB]
-- CONTAINMENT = NONE
-- ON  PRIMARY 
--( NAME = N'DoodjaDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\DoodjaDB.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
-- LOG ON 
--( NAME = N'DoodjaDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\DoodjaDB_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
--ALTER DATABASE [DoodjaDB] SET COMPATIBILITY_LEVEL = 110
--IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
--BEGIN
--EXEC [DoodjaDB].[dbo].[sp_fulltext_database] @action = 'enable'
--END
--END
--GO

USE [Momentarily]
GO

/****** Object:  Table [dbo].[c_category] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_category]'))
BEGIN
CREATE TABLE [dbo].[c_category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[parent_id] [int] NULL,
	[is_root] [bit] NOT NULL,
	[name] [varchar](250) NOT NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
CONSTRAINT [PK_c_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good]'))
BEGIN
CREATE TABLE [dbo].[c_good](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](250) NULL,
	[description] [varchar](4000) NULL,
	[price] [float] NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_good] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_category] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_category]'))
BEGIN
CREATE TABLE [dbo].[c_good_category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[good_id] [int] NOT NULL FOREIGN KEY REFERENCES c_good(id),
	[category_id] [int] NOT NULL FOREIGN KEY REFERENCES c_category(id),
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL
CONSTRAINT [PK_c_good_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user]'))
BEGIN
CREATE TABLE [dbo].[c_user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](100) NULL,
	[last_name] [varchar](100) NULL,
	[url] [varchar](800) NULL,
	[full_name] [varchar](256) NULL,
	[verified] [tinyint] NULL CONSTRAINT [DF_c_user_verified]  DEFAULT ((0)),
	[verification_code] [varchar](255) NULL,
	[email] [varchar](255) NULL,
	[description] [varchar](1500) NULL,
	[pwd] [varchar](255) NULL,
	[temp_pwd] [varchar](6) NULL,
	[temp_pwd_create_date] [datetime] NULL,
	[free_text] [varchar](200) NULL,
	[last_visit_date] [datetime] NULL,
	[website_url] [varchar](255) NULL,
	[create_date] [float] NULL,
	[create_by] [int] NULL,
	[mod_date] [float] NULL,
	[mod_by] [int] NULL,
	[date_of_birth] [date] NULL,
 CONSTRAINT [PK_c_user] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_img] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_img]'))
BEGIN
CREATE TABLE [dbo].[c_user_img](
	[user_img_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[type] [int] NULL,
	[sequence] [int] NULL,
	[file_name] [varchar](250) NULL,
 CONSTRAINT [PK_c_user_img] PRIMARY KEY CLUSTERED 
(
	[user_img_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_security_data_change_request] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_security_data_change_request]'))
BEGIN
CREATE TABLE [dbo].[c_user_security_data_change_request](
	[user_security_data_change_request_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[data_type] [int] NULL,
	[new_value] [varchar](255) NULL,
	[old_value] [varchar](255) NULL,
	[verification_code] [varchar](255) NULL,
	[create_date] [date] NULL,
	[verified] [bit] NULL,
 CONSTRAINT [PK_c_user_security_data_change_request] PRIMARY KEY CLUSTERED 
(
	[user_security_data_change_request_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_service_img] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_img]'))
BEGIN
CREATE TABLE [dbo].[c_good_img](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[good_id] [int] NULL,
	[user_id] [int] NULL,
	[type] [int] NULL,
	[sequence] [int] NULL,
	[file_name] [varchar](250) NULL,
	[folder] [varchar](250) NULL,
 CONSTRAINT [PK_c_good_img] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_verify_user_email] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_verify_user_email]'))
BEGIN
CREATE TABLE [dbo].[c_verify_user_email](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [varchar](255) NULL,
	[verification_code] [varchar](255) NULL,
	[verified] [bit] NOT NULL,
	[create_date] [datetime] NOT NULL,
 CONSTRAINT [PK_c_verify_user_email] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[d_good_property_type] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[d_good_property_type]'))
BEGIN
CREATE TABLE [dbo].[d_good_property_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](250) NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_d_good_property_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[d_good_property] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[d_good_property]'))
BEGIN
CREATE TABLE [dbo].[d_good_property](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](250) NULL,
	[type_id] [int] NOT NULL FOREIGN KEY REFERENCES d_good_property_type(id),
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_d_good_property] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[d_translate] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[d_translate]'))
BEGIN
CREATE TABLE [dbo].[d_translate](
	[translate_id] [int] IDENTITY(1,1) NOT NULL,
	[lang_id] [int] NOT NULL,
	[translate_key] [varchar](250) NULL,
	[translate_val] [varchar](250) NULL,
 CONSTRAINT [PK_d_translate] PRIMARY KEY CLUSTERED 
(
	[translate_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[d_translate_case] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[d_translate_case]'))
BEGIN
CREATE TABLE [dbo].[d_translate_case](
	[translate_case_id] [int] IDENTITY(1,1) NOT NULL,
	[lang_id] [int] NULL,
	[translate_key] [varchar](50) NULL,
	[translate_val] [varchar](50) NULL,
	[case] [varchar](10) NULL,
 CONSTRAINT [PK_d_translate_case] PRIMARY KEY CLUSTERED 
(
	[translate_case_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[s_dns] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[s_dns]'))
BEGIN
CREATE TABLE [dbo].[s_dns](
	[dns_id] [int] IDENTITY(1,1) NOT NULL,
	[dns] [varchar](255) NULL,
	[default_lang_id] [int] NOT NULL,
	[is_default] [bit] NOT NULL,
 CONSTRAINT [PK_s_dns] PRIMARY KEY CLUSTERED 
(
	[dns_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_property_value_definition] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_property_value_definition]'))
BEGIN
CREATE TABLE [dbo].[c_good_property_value_definition](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[good_property_id] [int] NOT NULL FOREIGN KEY REFERENCES d_good_property(id),
	[name] [varchar](250) NULL,
	[value] [varchar](250) NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_product_property_type_value] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[s_log] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[s_log]'))
BEGIN
CREATE TABLE [dbo].[s_log](
	[log_id] [int] IDENTITY(1,1) NOT NULL,
	[application_name] [varchar](50) NOT NULL,
	[source_name] [varchar](50) NOT NULL,
	[severity] [varchar](50) NOT NULL,
	[create_date] [datetime] NOT NULL,
	[user_id] [int] NULL,
	[session_id] [varchar](50) NULL,
	[ipaddress] [varchar](50) NULL,
	[message] [text] NOT NULL,
	[app_version] [varchar](50) NULL,
 CONSTRAINT [PK_s_log] PRIMARY KEY CLUSTERED 
(
	[log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_property_value] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_property_value]'))
BEGIN
CREATE TABLE [dbo].[c_good_property_value](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[good_id] [int] NOT NULL FOREIGN KEY REFERENCES c_good(id),
	[good_property_id] [int] NOT NULL FOREIGN KEY REFERENCES d_good_property(id),
	[property_value_definition_id] [int] NULL FOREIGN KEY REFERENCES c_good_property_value_definition(id),
	[value] [varchar](250) NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL
CONSTRAINT [PK_c_good_property_value] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[s_settings] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[s_settings]'))
BEGIN
CREATE TABLE [dbo].[s_settings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[key] [varchar](50) NOT NULL,
	[value] [varchar](500) NULL,
 CONSTRAINT [PK_s_settings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_s_settings] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_s_settings_1] UNIQUE NONCLUSTERED 
(
	[key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[s_user_privilege] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[s_user_privilege]'))
BEGIN
CREATE TABLE [dbo].[s_user_privilege](
	[privilege_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[privilege_name] [varchar](255) NOT NULL,
	[has_access] [bit] NOT NULL,
 CONSTRAINT [PK_s_user_privilege] PRIMARY KEY CLUSTERED 
(
	[privilege_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_good] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_good]'))
BEGIN
CREATE TABLE [dbo].[c_user_good](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[good_id] [int] NOT NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_user_good_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_category_tree] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_category_tree]'))
BEGIN
CREATE TABLE [dbo].[c_category_tree](
	[parent_id] [int] NOT NULL,
	[child_id] [int] NOT NULL,
	[depth] [int] NOT NULL
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_location] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_location]'))
BEGIN
CREATE TABLE [dbo].[c_good_location](
	[good_id] [int] NOT NULL FOREIGN KEY REFERENCES c_good(id),
	[latitude] [float] NULL,
	[longitude] [float] NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_good_location] PRIMARY KEY CLUSTERED 
(
	[good_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_start_end_date] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_start_end_date]'))
BEGIN
CREATE TABLE [dbo].[c_good_start_end_date](
	[good_id] [int] NOT NULL FOREIGN KEY REFERENCES c_good(id),
	[date_start] [datetime] NULL,
	[date_end] [datetime] NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_good_start_end_date] PRIMARY KEY CLUSTERED 
(
	[good_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_phone_number] ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_phone_number]'))
--BEGIN
--CREATE TABLE [dbo].[c_user_phone_number](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[user_id] [int] NOT NULL FOREIGN KEY REFERENCES c_user(id),
--	[phone_number] [varchar](15) NOT NULL,
--	[create_date] [datetime] NULL,
--	[mod_date] [datetime] NULL,
--	[create_by] [int] NULL,
--	[mod_by] [int] NULL,
--	CONSTRAINT [PK_c_user_phone_number] PRIMARY KEY CLUSTERED ([id] ASC)
--	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO

/****** Object:  Table [dbo].[c_user_message] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_message]'))
BEGIN
CREATE TABLE [dbo].[c_user_message](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[opposed_user_id] [int] NOT NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_user_message] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_message_detail] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_message_detail]'))
BEGIN
CREATE TABLE [dbo].[c_user_message_detail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_message_id] [int] NOT NULL,
	[receiver_id] [int] NOT NULL,
	[author_id] [int] NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[is_read] [bit] NOT NULL,
	[is_system] [bit] NOT NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_user_notification] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_notification]'))
BEGIN
CREATE TABLE [dbo].[c_user_notification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[text] [varchar](250) NOT NULL,
	[url] [varchar](250) NOT NULL,
	[is_viewed] [bit] NOT NULL,
	[create_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_date] [datetime] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_user_notification] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_request] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_request]'))
BEGIN
CREATE TABLE [dbo].[c_good_request](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[good_id] [int] NOT NULL,
	[status_id] [int] NOT NULL,
	[price] [float] NOT NULL,
	[create_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_date] [datetime] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_good_request] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_good_booking] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_booking]'))
BEGIN
CREATE TABLE [dbo].[c_good_booking](
	[good_request_id] [int] NOT NULL,
	[start_date] [datetime] NOT NULL,
	[end_date] [datetime] NOT NULL,
	[create_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_date] [datetime] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_good_booking_1] PRIMARY KEY CLUSTERED 
(
	[good_request_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

--allow null for c_user_phone_number.phone_number
--ALTER TABLE [dbo].[c_user_phone_number]
--ALTER COLUMN [phone_number] VARCHAR(15) NULL

--add c_good table.is_archive
IF EXISTS (SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'c_good'))
BEGIN
	IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'is_archive' AND OBJECT_ID = OBJECT_ID(N'c_good'))
	ALTER TABLE [dbo].[c_good] ADD is_archive BIT NOT NULL DEFAULT 0
END
GO

/****** Object:  Table [dbo].[c_request_status] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_request_status]'))
BEGIN
CREATE TABLE [dbo].[c_request_status](
	[id] [int] NOT NULL,
	[status_name] [varchar](250) NOT NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_status] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[c_payment_transaction] ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_payment_transaction]'))
BEGIN
CREATE TABLE [dbo].[c_payment_transaction](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[transaction_id] [varchar](250) NOT NULL,
	[payer_id] [varchar](250) NOT NULL,
	[good_request_id] [int] NOT NULL,
	[type] [varchar](250) NOT NULL,
	[cost] [float] NOT NULL,
	[commision] [float] NOT NULL,
	[status_id] [int] NOT NULL,
	[create_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_date] [datetime] NULL,
	[mod_by] [int] NULL,
 CONSTRAINT [PK_c_payment_transaction] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_address]'))
BEGIN
CREATE TABLE [dbo].[c_user_address](
	[id] [int] NOT NULL,
	[addressLine1] [nvarchar](200) NOT NULL,
	[addressLine2] [nvarchar](200) NULL,
	[addressLine3] [nvarchar](200) NULL,
	[postalCode] [nchar](10) NULL,
	[location_id] [int] NULL,
	[region] [nvarchar](50) NULL,
	[country] [nvarchar](50) NULL,
	[person_Id] [int] NOT NULL,
 CONSTRAINT [PK_c_user_address] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO



IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'dbo.FK_c_user_address_c_user') AND parent_object_id = OBJECT_ID(N'dbo.c_user_address'))
BEGIN
  ALTER TABLE [dbo].[c_user_address] DROP CONSTRAINT [FK_c_user_address_c_user]
END
GO
ALTER TABLE [dbo].[c_user_address]  WITH CHECK ADD  CONSTRAINT [FK_c_user_address_c_user] FOREIGN KEY([person_Id])
REFERENCES [dbo].[c_user] ([id])
GO
ALTER TABLE [dbo].[c_user_address] CHECK CONSTRAINT [FK_c_user_address_c_user]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_phone_number]'))
BEGIN
CREATE TABLE [dbo].[c_phone_number](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[address_id] [int] NULL,
	[country_code] [nchar](5) NOT NULL,
	[phone_number] [nvarchar](15) NOT NULL,
 CONSTRAINT [PK_c_phoneNumber] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'dbo.FK_c_phoneNumber_c_user') AND parent_object_id = OBJECT_ID(N'dbo.c_phone_number'))
BEGIN
  ALTER TABLE [dbo].[c_phone_number] DROP CONSTRAINT [FK_c_phoneNumber_c_user]
END
GO

ALTER TABLE [dbo].[c_phone_number]  WITH CHECK ADD  CONSTRAINT [FK_c_phoneNumber_c_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[c_user] ([id])
GO
ALTER TABLE [dbo].[c_phone_number] CHECK CONSTRAINT [FK_c_phoneNumber_c_user]
GO

--ALTER TABLE [dbo].[c_phone_number]  WITH CHECK ADD  CONSTRAINT [FK_c_phoneNumber_c_user] FOREIGN KEY([user_id])
--REFERENCES [dbo].[c_user] ([id])
--GO

--ALTER TABLE [dbo].[c_phone_number] CHECK CONSTRAINT [FK_c_phoneNumber_c_user]
--GO




IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'dbo.FK_c_phoneNumber_c_user_address') AND parent_object_id = OBJECT_ID(N'dbo.c_phone_number'))
BEGIN
  ALTER TABLE [dbo].[c_phone_number] DROP CONSTRAINT [FK_c_phoneNumber_c_user_address]
END
GO

ALTER TABLE [dbo].[c_phone_number]  WITH CHECK ADD  CONSTRAINT [FK_c_phoneNumber_c_user_address] FOREIGN KEY([address_id])
REFERENCES [dbo].[c_user_address] ([id])
GO

ALTER TABLE [dbo].[c_phone_number] CHECK CONSTRAINT [FK_c_phoneNumber_c_user_address]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'c_user'))
BEGIN
	IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'address_id' AND OBJECT_ID = OBJECT_ID(N'c_user'))
	ALTER TABLE [dbo].[c_user] ADD address_id int NULL

	IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'account_association_id' AND OBJECT_ID = OBJECT_ID(N'c_user'))
	ALTER TABLE [dbo].[c_user] ADD account_association_id int NULL
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'dbo.FK_c_user_c_user_address') AND parent_object_id = OBJECT_ID(N'dbo.c_user'))
BEGIN
  ALTER TABLE [dbo].[c_user] DROP CONSTRAINT [FK_c_user_c_user_address]
END
GO

ALTER TABLE [dbo].[c_user]  WITH CHECK ADD  CONSTRAINT [FK_c_user_c_user_address] FOREIGN KEY([address_id])
REFERENCES [dbo].[c_user_address] ([id])
GO

ALTER TABLE [dbo].[c_user] CHECK CONSTRAINT [FK_c_user_c_user_address]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_user_account_association]'))
BEGIN
CREATE TABLE [dbo].[c_user_account_association](
	[id] [int]  IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[external_id] [nvarchar](50) NOT NULL,
	[provider_name] [nvarchar](50) NOT NULL,
	[extra_data] [nvarchar](max) NULL,
	[image_url] [nvarchar](256) NULL,
 CONSTRAINT [PK_c_user_account_association] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


--IF EXISTS (SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'c_payment_transaction'))
--BEGIN
--	IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'capture_id' AND OBJECT_ID = OBJECT_ID(N'c_payment_transaction'))
--	ALTER TABLE [dbo].[c_payment_transaction] ADD capture_id varchar(250) NULL
--END
--GO

--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_pinpayment_card]'))
--BEGIN
--CREATE TABLE [dbo].[p_pinpayment_card](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[user_id] [int] NOT NULL,
--	[card_token] [nvarchar](250) NOT NULL,
--	[create_date] [datetime] NULL,
--	[mod_date] [datetime] NULL,
--	[create_by] [int] NULL,
--	[mod_by] [int] NULL,
--	[display_number] [nvarchar](250) NOT NULL,
-- CONSTRAINT [PK_c_pinpayment_card] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO

--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_pinpayment_customer]'))
--BEGIN
--CREATE TABLE [dbo].[p_pinpayment_customer](
--	[user_id] [int] NOT NULL,
--	[customer_token] [nvarchar](250) NOT NULL,
--	[create_date] [datetime] NULL,
--	[mod_date] [datetime] NULL,
--	[create_by] [int] NULL,
--	[mod_by] [int] NULL,
--PRIMARY KEY CLUSTERED 
--(
--	[user_id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO

----ALTER TABLE [dbo].[p_pinpayment_customer] ADD  DEFAULT (NULL) FOR [create_date]
----GO


--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_pinpayment_recipient]'))
--BEGIN
--CREATE TABLE [dbo].[p_pinpayment_recipient](
--	[user_id] [int] NOT NULL,
--	[recipient_token] [nvarchar](250) NOT NULL,
--	[create_date] [datetime] NULL,
--	[mod_date] [datetime] NULL,
--	[create_by] [int] NULL,
--	[mod_by] [int] NULL,
--PRIMARY KEY CLUSTERED 
--(
--	[user_id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[c_good_booking_rank]'))
BEGIN
CREATE TABLE [dbo].[c_good_booking_rank](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[good_request_id] [int] NOT NULL,
	[good_id] [int] NOT NULL,
	[sharer_id] [int] NOT NULL,
	[seeker_id] [int] NOT NULL,
	[reviewer_id] [int] NOT NULL,
	[rank] [int] NOT NULL,
	[message] [nvarchar](1000) NULL,
	[create_date] [datetime] NULL,
	[mod_date] [datetime] NULL,
	[create_by] [int] NULL,
	[mod_by] [int] NULL,
CONSTRAINT [PK_c_good_booking_rank] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


ALTER TABLE [dbo].[c_good_booking_rank]  WITH CHECK ADD  CONSTRAINT [FK_c_good_booking_rank_c_user] FOREIGN KEY([reviewer_id])
REFERENCES [dbo].[c_user] ([id])
GO

ALTER TABLE [dbo].[c_good_booking_rank] CHECK CONSTRAINT [FK_c_good_booking_rank_c_user]
GO