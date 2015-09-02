
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_Survey_Insert] (
	@Name VARCHAR(50),
	@LongName VARCHAR(150),
	@Description TEXT,
	@Preamble TEXT,
	@Status INT,
	@TakenCount INT,
	@PatronCount INT,
	@CanBeScored BIT,
	@TenID INT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Survey (
		NAME,
		LongName,
		Description,
		Preamble,
		STATUS,
		TakenCount,
		PatronCount,
		CanBeScored,
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
		@Name,
		@LongName,
		@Description,
		@Preamble,
		@Status,
		@TakenCount,
		@PatronCount,
		@CanBeScored,
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

	SELECT @SID = SCOPE_IDENTITY()
END
