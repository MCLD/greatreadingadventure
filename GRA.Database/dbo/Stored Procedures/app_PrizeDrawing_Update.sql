
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_Update] (
	@PDID INT,
	@PrizeName VARCHAR(250),
	@TID INT,
	@DrawingDateTime DATETIME,
	@NumWinners INT,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE PrizeDrawing
SET PrizeName = @PrizeName,
	TID = @TID,
	DrawingDateTime = @DrawingDateTime,
	NumWinners = @NumWinners,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
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
WHERE PDID = @PDID
	AND TenID = @TenID
