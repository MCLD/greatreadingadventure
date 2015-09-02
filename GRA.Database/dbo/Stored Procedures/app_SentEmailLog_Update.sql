
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Update]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_Update] (
	@EID INT,
	@SentDateTime DATETIME,
	@SentFrom VARCHAR(150),
	@SentTo VARCHAR(150),
	@Subject VARCHAR(150),
	@Body TEXT
	)
AS
UPDATE SentEmailLog
SET SentDateTime = @SentDateTime,
	SentFrom = @SentFrom,
	SentTo = @SentTo,
	Subject = @Subject,
	Body = @Body
WHERE EID = @EID
