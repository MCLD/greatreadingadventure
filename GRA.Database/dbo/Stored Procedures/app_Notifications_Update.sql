
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Notifications_Update] (
	@NID INT,
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
	@isUnread BIT = 0
	)
AS
UPDATE Notifications
SET PID_To = @PID_To,
	PID_From = @PID_From,
	isQuestion = @isQuestion,
	Subject = @Subject,
	Body = @Body,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	isUnread = @isUnread
WHERE NID = @NID
	AND TenID = @TenID
