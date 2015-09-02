
/****** Object:  StoredProcedure [dbo].[app_CodeType_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_CodeType_Insert] (
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
	@FldText3 TEXT = '',
	@CTID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO CodeType (
		isSystem,
		CodeTypeName,
		Description,
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
		@isSystem,
		@CodeTypeName,
		@Description,
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

	SELECT @CTID = SCOPE_IDENTITY()
END
