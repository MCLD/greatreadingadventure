
CREATE PROCEDURE [dbo].[app_ProgramCodes_Insert] (
	@PID INT,
	@CodeNumber INT,
	@CodeValue UNIQUEIDENTIFIER,
	@ShortCode VARCHAR(20) = '',
	@isUsed BIT,
	@DateCreated DATETIME,
	@DateUsed DATETIME,
	@PatronId INT,
	@PCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramCodes (
		PID,
		CodeNumber,
		CodeValue,
		ShortCode,
		isUsed,
		DateCreated,
		DateUsed,
		PatronId
		)
	VALUES (
		@PID,
		@CodeNumber,
		@CodeValue,
		@ShortCode,
		@isUsed,
		@DateCreated,
		@DateUsed,
		@PatronId
		)

	SELECT @PCID = SCOPE_IDENTITY()
END
