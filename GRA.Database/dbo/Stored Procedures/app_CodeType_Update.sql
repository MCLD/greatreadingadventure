
/****** Object:  StoredProcedure [dbo].[app_CodeType_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_CodeType_Update] (
	@CTID INT,
	@isSystem BIT,
	@CodeTypeName VARCHAR(50),
	@Description TEXT,
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
UPDATE CodeType
SET isSystem = @isSystem,
	CodeTypeName = @CodeTypeName,
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
WHERE CTID = @CTID
	AND TenID = @TenID
