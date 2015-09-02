
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UpdateSpecialUserPermissions]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_UpdateSpecialUserPermissions] @UID INT,
	@PermissionID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPUserPermissions
WHERE UID = @UID
	AND PermissionID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)

INSERT INTO dbo.SRPUserPermissions
SELECT @UID,
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
		FROM dbo.SRPUserPermissions
		WHERE UID = @UID
		)
