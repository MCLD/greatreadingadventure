
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissions]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetSpecialUserPermissions] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPUser.UID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPUserPermissions.AddedDate,
	dbo.SRPUserPermissions.AddedUser
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
