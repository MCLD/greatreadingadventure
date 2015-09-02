
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissions]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetPermissions] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPGroups.GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPGroupPermissions.AddedDate,
	dbo.SRPGroupPermissions.AddedUser
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
WHERE dbo.SRPGroups.GID = @GID
