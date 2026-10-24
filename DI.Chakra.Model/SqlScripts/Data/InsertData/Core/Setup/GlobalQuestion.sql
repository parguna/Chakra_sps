Delete [DI_M_GlobalQuestion]
DBCC CHECKIDENT ([DI_M_GlobalQuestion], reseed, 0)

INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your Mother''s birth date?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your Father''s birth date?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your Mother''s birth place?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your Father''s birth place?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your favourite historical place?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your Father''s first occupation?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your Mother''s maiden name?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is the model and make of your first car?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your favourite sports team?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'Who is your all-time favourite film character?',  N'System', GETDATE(), N'System', GETDATE())


INSERT INTO [dbo].[DI_M_GlobalQuestion] ([Question],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'What is your driving licence number?',  N'System', GETDATE(), N'System', GETDATE())
GO

