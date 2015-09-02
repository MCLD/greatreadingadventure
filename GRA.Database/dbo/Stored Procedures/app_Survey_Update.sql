
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Survey_Update] (
	@SID INT,
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
	@FldText3 TEXT
	)
AS
UPDATE Survey
SET NAME = @Name,
	LongName = @LongName,
	Description = @Description,
	Preamble = @Preamble,
	STATUS = @Status,
	TakenCount = @TakenCount,
	PatronCount = @PatronCount,
	CanBeScored = @CanBeScored,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SID = @SID
	AND TenID = @TenID
