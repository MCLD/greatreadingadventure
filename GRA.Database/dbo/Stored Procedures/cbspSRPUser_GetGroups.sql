
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroups]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetGroups] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPUser.UID,
	dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser
FROM dbo.SRPGroups
INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
