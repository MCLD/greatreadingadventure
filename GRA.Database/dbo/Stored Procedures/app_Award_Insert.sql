
CREATE PROCEDURE [dbo].[app_Award_Insert] (
	@AwardName VARCHAR(80),
	@BadgeID INT,
	@NumPoints INT,
	@BranchID INT,
	@ProgramID INT,
	@District VARCHAR(50),
	@SchoolName VARCHAR(50),
	@BadgeList VARCHAR(500),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
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
	@AID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Award (
		AwardName,
		BadgeID,
		NumPoints,
		BranchID,
		ProgramID,
		District,
		SchoolName,
		BadgeList,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@AwardName,
		@BadgeID,
		@NumPoints,
		@BranchID,
		@ProgramID,
		@District,
		@SchoolName,
		@BadgeList,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @AID = SCOPE_IDENTITY()
END
