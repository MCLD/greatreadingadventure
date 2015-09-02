
CREATE PROCEDURE [dbo].[app_Notifications_Insert] (
	@PID_To INT,
	@PID_From INT,
	@isQuestion BIT,
	@Subject VARCHAR(150),
	@Body TEXT,
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@isUnread BIT = 0,
	@NID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Notifications (
		PID_To,
		PID_From,
		isQuestion,
		Subject,
		Body,
		AddedDate,
		AddedUser,
		LastModDate,
		LastModUser,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		isUnread
		)
	VALUES (
		@PID_To,
		@PID_From,
		@isQuestion,
		@Subject,
		@Body,
		@AddedDate,
		@AddedUser,
		@LastModDate,
		@LastModUser,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@isUnread
		)

	SELECT @NID = SCOPE_IDENTITY()
END
