
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_UpdatePermissions]    Script Date: 01/05/2015 14:43:28 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_UpdatePermissions] @GID INT,
	@PermissionID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPGroupPermissions
WHERE GID = @GID
	AND PermissionID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)

INSERT INTO dbo.SRPGroupPermissions
SELECT @GID,
	PermissionID,
	getdate(),
	@ActionUsername
FROM dbo.SRPPermissionsMaster
WHERE PermissionID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)
	AND PermissionID NOT IN (
		SELECT PermissionID
		FROM dbo.SRPGroupPermissions
		WHERE GID = @GID
		)
