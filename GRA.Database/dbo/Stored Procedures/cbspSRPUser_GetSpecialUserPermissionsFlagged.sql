
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @UID AS UID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPUserPermissions.AddedDate,
	dbo.SRPUserPermissions.AddedUser,
	CASE 
		WHEN dbo.SRPUserPermissions.AddedDate IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.SRPPermissionsMaster
LEFT JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
	AND dbo.SRPUserPermissions.UID = @UID
WHERE UID = @UID
	OR UID IS NULL
