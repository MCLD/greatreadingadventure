
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetGroupsFlagged] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @TenID INT

SELECT @TenID = TenID
FROM SRPUser
WHERE UID = @UID

SELECT @UID,
	dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser,
	CASE 
		WHEN dbo.SRPUserGroups.AddedDate IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.SRPGroups
LEFT JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
	AND dbo.SRPUserGroups.UID = @UID
WHERE UID = @UID
	OR UID IS NULL
	AND SRPGroups.TenID = @TenID
