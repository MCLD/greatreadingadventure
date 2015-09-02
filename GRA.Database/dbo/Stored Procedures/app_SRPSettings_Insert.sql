
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_Insert] (
	@Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT,
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
	@SID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SRPSettings (
		NAME,
		Value,
		StorageType,
		EditType,
		ModID,
		Label,
		Description,
		ValueList,
		DefaultValue,
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
		@Value,
		@StorageType,
		@EditType,
		@ModID,
		@Label,
		@Description,
		@ValueList,
		@DefaultValue,
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
