
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsersFlagged]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetUsersFlagged] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	dbo.SRPUser.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser,
	CASE 
		WHEN dbo.SRPUserGroups.AddedDate IS NULL
			THEN 0 --'False'
		ELSE 1 --'True'
		END AS isMember
FROM dbo.SRPUser
LEFT JOIN dbo.SRPUserGroups ON dbo.SRPUser.UID = dbo.SRPUserGroups.UID
	AND dbo.SRPUserGroups.GID = @GID
LEFT JOIN dbo.SRPGroups ON dbo.SRPGroups.GID = @GID
WHERE (
		dbo.SRPUserGroups.GID = @GID
		OR dbo.SRPUserGroups.GID IS NULL
		)
	AND dbo.SRPUser.IsDeleted = 0
	AND dbo.SRPUser.TenID = dbo.SRPGroups.TenID
