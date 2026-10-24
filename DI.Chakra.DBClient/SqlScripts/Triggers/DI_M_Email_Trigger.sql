
CREATE  TRIGGER [dbo].[DI_M_Email_Trigger] ON [dbo].[DI_M_Email]  AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
    
	           
	INSERT INTO [dbo].[DI_M_EmailHistory]					
			([ActivityType]
			,[Priority]
			,[From]
			,[To]
			,[Cc]
			,[Bcc]
			,[Subject]
			,[Body]
			,[IsHtml]
			,[Attachment]
			,[Status]
			,[CreatedBy]
			,[CreatedDateTime]
			,[SentDateTime]
			,[NoOfAttempts])
	SELECT   [ActivityType]
			,[Priority]
			,[From]
			,[To]
			,[Cc]
			,[Bcc]
			,[Subject]
			,[Body]
			,[IsHtml]
			,[Attachment]
			,[Status]
			,[CreatedBy]
			,[CreatedDateTime]
			,[SentDateTime]
			,[NoOfAttempts]
	FROM inserted

	DELETE DI_M_Email where Id = (Select Id from inserted)

END