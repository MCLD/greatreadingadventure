
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetAllPermissions]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_GetAllPermissions] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
WHERE dbo.SRPGroups.GID IN (
		SELECT GID
		FROM SRPUserGroups
		WHERE UID = @UID
		)

UNION

SELECT dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
