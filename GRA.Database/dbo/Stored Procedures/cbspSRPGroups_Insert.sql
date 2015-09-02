
-- Inserts a new record into the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_Insert] @GroupName VARCHAR(50),
	@GroupDescription VARCHAR(255),
	@ActionUsername VARCHAR(50),
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
INSERT INTO [dbo].[SRPGroups] (
	[GroupName],
	[GroupDescription],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
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
	@GroupName,
	@GroupDescription,
	GETDATE(),
	@ActionUsername,
	GETDATE(),
	@ActionUsername,
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

SELECT @@IDENTITY
