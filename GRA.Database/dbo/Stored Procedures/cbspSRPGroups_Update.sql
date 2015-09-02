
-- Updates a record in the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_Update]
	-- The rest of writeable parameters
	@GroupName VARCHAR(50),
	@GroupDescription VARCHAR(255),
	@ActionUsername VARCHAR(50),
	-- Primary key parameters
	@GID INT,
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
AS
UPDATE [dbo].[SRPGroups]
SET [GroupName] = @GroupName,
	[GroupDescription] = @GroupDescription,
	[LastModDate] = GETDATE(),
	[LastModUser] = @ActionUsername,
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
WHERE [GID] = @GID
	AND TenID = @TenID
