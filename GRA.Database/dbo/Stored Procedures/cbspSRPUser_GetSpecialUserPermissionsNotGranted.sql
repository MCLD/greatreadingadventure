
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @UID AS UID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPPermissionsMaster
WHERE dbo.SRPPermissionsMaster.PermissionID NOT IN (
		SELECT dbo.SRPPermissionsMaster.PermissionID
		FROM dbo.SRPPermissionsMaster
		INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
		INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
		WHERE dbo.SRPUserPermissions.UID = @UID
		)
