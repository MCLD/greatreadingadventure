
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_Insert]    Script Date: 01/05/2015 14:43:27 ******/
-- Inserts a new record into the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_Insert] @Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT
AS
INSERT INTO [dbo].[SRPSettings] (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue]
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
	@DefaultValue
	)

SELECT @@IDENTITY
