
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissionsFlagged]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetPermissionsFlagged] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPGroupPermissions.AddedDate,
	dbo.SRPGroupPermissions.AddedUser,
	CASE 
		WHEN dbo.SRPGroupPermissions.AddedDate IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.SRPPermissionsMaster
LEFT JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
	AND dbo.SRPGroupPermissions.GID = @GID
WHERE GID = @GID
	OR GID IS NULL
