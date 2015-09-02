
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Insert] (
	@OBID INT,
	@MGID INT,
	@PageNumber INT,
	@TextEasy TEXT,
	@TextMedium TEXT,
	@TextHard TEXT,
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@OBPGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGOnlineBookPages (
		OBID,
		MGID,
		PageNumber,
		TextEasy,
		TextMedium,
		TextHard,
		AudioEasy,
		AudioMedium,
		AudioHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@OBID,
		@MGID,
		(
			SELECT isnull(Max(PageNumber), 0) + 1
			FROM MGOnlineBookPages
			WHERE MGID = @MGID
			),
		@TextEasy,
		@TextMedium,
		@TextHard,
		@AudioEasy,
		@AudioMedium,
		@AudioHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @OBPGID = SCOPE_IDENTITY()
END
