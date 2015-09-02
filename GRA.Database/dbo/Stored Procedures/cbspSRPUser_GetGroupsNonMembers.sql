
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroupsNonMembers]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetGroupsNonMembers] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @UID AS UID,
	dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPGroups
WHERE dbo.SRPGroups.GID NOT IN (
		SELECT dbo.SRPGroups.GID
		FROM dbo.SRPGroups
		INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
		INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
		WHERE dbo.SRPUserGroups.UID = @UID
		)
