
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_Update] (
	@SID INT,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE SRPSettings
SET NAME = @Name,
	Value = @Value,
	StorageType = @StorageType,
	EditType = @EditType,
	ModID = @ModID,
	Label = @Label,
	Description = @Description,
	ValueList = @ValueList,
	DefaultValue = @DefaultValue,
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
