
/****** Object:  StoredProcedure [dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPPermissionsMaster
WHERE dbo.SRPPermissionsMaster.PermissionID NOT IN (
		SELECT dbo.SRPPermissionsMaster.PermissionID
		FROM dbo.SRPPermissionsMaster
		INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
		INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
		WHERE dbo.SRPGroupPermissions.GID = @GID
		)
