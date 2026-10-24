GO
/****** Object:  StoredProcedure [dbo].[DI_SP_GetAuditTableWithouFile]    Script Date: 4/10/2017 1:20:46 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DI_SP_GetAuditTableWithouFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DI_SP_GetAuditTableWithouFile]
GO
/****** Object:  StoredProcedure [dbo].[DI_SP_GetAuditTableWithouFile]    Script Date: 4/10/2017 1:20:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAuditTableWithouFile]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create procedure [dbo].[DI_SP_GetAuditTableWithouFile] @tableName varchar(max), @id bigint
as
Begin
	
	create table #columnNames (COLUMN_NAME varchar(25)) 
	insert into #columnNames 
	select  COLUMN_NAME
	from INFORMATION_SCHEMA.COLUMNS
	where TABLE_NAME=@tableName and DATA_TYPE <> ''varbinary''
	declare @tmp varchar(250)
	SET @tmp = '''' 
	select @tmp = @tmp + COLUMN_NAME + '', '' from #columnNames
	select @tmp = SUBSTRING(@tmp, 0, LEN(@tmp))
	Exec(''select '' +@tmp+ '' from ''+ @tableName +'' where id=''+ @id)
	drop table #columnNames
End	' 
END
GO
