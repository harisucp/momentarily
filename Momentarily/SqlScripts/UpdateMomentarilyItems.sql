USE [MomentarilyDB]
GO
ALTER TABLE [dbo].[c_good_property_value_definition] NOCHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[d_good_property] NOCHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[d_good_property_type] NOCHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[c_good_property_value] NOCHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[c_good_category] NOCHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[c_request_status] NOCHECK CONSTRAINT ALL
GO

DELETE FROM [dbo].[c_good_property_value_definition]
GO
DELETE FROM [dbo].[d_good_property]
GO
DELETE FROM [dbo].[d_good_property_type]
GO
DELETE FROM [dbo].[c_category]
GO
DELETE FROM [dbo].[c_category_tree]
GO
DELETE FROM [dbo].[c_request_status]
GO

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
--INSERT [dbo].[c_good_property_value_definition] ([id], [good_property_id], [name], [value], [create_date], [mod_date], [create_by], [mod_by]) VALUES (2, 1, N'Lesson', N'Lesson', GETDATE(), GETDATE(), NULL, NULL)
--INSERT [dbo].[c_good_property_value_definition] ([id], [good_property_id], [name], [value], [create_date], [mod_date], [create_by], [mod_by]) VALUES (3, 1, N'Experience', N'Experience', GETDATE(), GETDATE(), NULL, NULL)
SET IDENTITY_INSERT [dbo].[c_good_property_value_definition] OFF

SET IDENTITY_INSERT [dbo].[c_category] ON 
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (1, NULL, 1, N'Momentarily', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (2, 1, 0, N'Surfing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (3, 1, 0, N'Canoeing/Kayaking', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (4, 1, 0, N'Backpacking', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (5, 1, 0, N'Camping', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (6, 1, 0, N'Climbing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (7, 1, 0, N'Biking', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (8, 1, 0, N'Fishing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (9, 1, 0, N'Golf', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (10, 1, 0, N'Kites', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (11, 1, 0, N'Kite-surfing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (12, 1, 0, N'Wakeboarding', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (13, 1, 0, N'SUP boarding', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (14, 1, 0, N'Sailing/Windsurfing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (15, 1, 0, N'Scuba diving', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (16, 1, 0, N'Snorkeling', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (17, 1, 0, N'Skateboarding', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (18, 1, 0, N'Skiing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (19, 1, 0, N'Snowboarding', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (20, 1, 0, N'Spearfishing', GETDATE(), GETDATE(), NULL, NULL)
INSERT [dbo].[c_category] ([id], [parent_id], [is_root], [name], [create_date], [mod_date], [create_by], [mod_by]) VALUES (21, 1, 0, N'Walking/Hiking', GETDATE(), GETDATE(), NULL, NULL)

SET IDENTITY_INSERT [dbo].[c_category] OFF

INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 2, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 3, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 4, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 5, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 6, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 7, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 8, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 9, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 10, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 11, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 12, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 13, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 14, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 15, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 16, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 17, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 18, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 19, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 20, 0)
INSERT [dbo].[c_category_tree] ([parent_id], [child_id], [depth]) VALUES (1, 21, 0)
GO
ALTER TABLE [dbo].[c_good_property_value_definition] WITH CHECK CHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[d_good_property] WITH CHECK CHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[d_good_property_type] WITH CHECK CHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[c_good_property_value] WITH CHECK CHECK CONSTRAINT ALL
GO
ALTER TABLE [dbo].[c_good_category] WITH CHECK CHECK CONSTRAINT ALL
GO


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
GO

ALTER TABLE [dbo].[c_request_status] WITH CHECK CHECK CONSTRAINT ALL
GO