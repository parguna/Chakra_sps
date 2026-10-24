--- Adding Section mapping in Discovery

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=3

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'SectionMapping' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'SectionMapping',N'System', GETDATE(), N'System', GETDATE())

Go


IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName] = 29 )
	 INSERT INTO [dbo].[DI_M_EndpointSystemType]
           ([EndpointSystemTypeName]
           ,[DisplayName]
           ,[Description]
           ,[IsActive]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[ObjectName]
           ,[CanBeTrustedSource])
     VALUES
           (29
           ,N'EAM NOVA' 
           ,N'EAM NOVA' 
           ,1
           ,N'System' 
           ,GETDATE()
           ,N'System' 
           ,GETDATE()
           ,NULL
		   ,0)
GO



