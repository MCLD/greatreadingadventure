
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_Insert] (
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@OBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGOnlineBook (
		MGID,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @OBID = SCOPE_IDENTITY()
END
