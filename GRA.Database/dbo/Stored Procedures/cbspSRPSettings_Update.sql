
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_Update]    Script Date: 01/05/2015 14:43:27 ******/
-- Updates a record in the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_Update]
	-- The rest of writeable parameters
	@Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT,
	-- Primary key parameters
	@SID INT
AS
UPDATE [dbo].[SRPSettings]
SET [Name] = @Name,
	[Value] = @Value,
	[StorageType] = @StorageType,
	[EditType] = @EditType,
	[ModID] = @ModID,
	[Label] = @Label,
	[Description] = @Description,
	[ValueList] = @ValueList,
	[DefaultValue] = @DefaultValue
WHERE [SID] = @SID
