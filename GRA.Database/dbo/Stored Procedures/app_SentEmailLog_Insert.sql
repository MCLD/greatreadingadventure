
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Insert]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_Insert] (
	@SentDateTime DATETIME,
	@SentFrom VARCHAR(150),
	@SentTo VARCHAR(150),
	@Subject VARCHAR(150),
	@Body TEXT,
	@EID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SentEmailLog (
		SentDateTime,
		SentFrom,
		SentTo,
		Subject,
		Body
		)
	VALUES (
		@SentDateTime,
		@SentFrom,
		@SentTo,
		@Subject,
		@Body
		)

	SELECT @EID = SCOPE_IDENTITY()
END
