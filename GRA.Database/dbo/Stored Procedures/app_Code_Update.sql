
/****** Object:  StoredProcedure [dbo].[app_Code_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Code_Update] (
	@CID INT,
	@CTID INT,
	@Code VARCHAR(25),
	@Description VARCHAR(80),
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
UPDATE Code
SET CTID = @CTID,
	Code = @Code,
	Description = @Description,
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
WHERE CID = @CID
	AND TenID = @TenID
