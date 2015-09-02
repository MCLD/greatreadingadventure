
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsers]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetUsers] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPUser.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser
FROM dbo.SRPGroups
INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
WHERE dbo.SRPGroups.GID = @GID
	AND dbo.SRPUser.IsDeleted = 0
