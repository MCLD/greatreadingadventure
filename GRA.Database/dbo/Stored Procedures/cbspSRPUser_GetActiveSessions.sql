
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetActiveSessions]    Script Date: 01/05/2015 14:43:27 ******/
----------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetActiveSessions]
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPUserLoginHistory.UIDLH,
	dbo.SRPUserLoginHistory.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	dbo.SRPUserLoginHistory.SessionsID,
	dbo.SRPUserLoginHistory.StartDateTime,
	dbo.SRPUserLoginHistory.IP,
	dbo.SRPUserLoginHistory.MachineName,
	dbo.SRPUserLoginHistory.Browser,
	dbo.SRPUserLoginHistory.EndDateTime
FROM dbo.SRPUser
INNER JOIN dbo.SRPUserLoginHistory ON dbo.SRPUser.UID = dbo.SRPUserLoginHistory.UID
WHERE dbo.SRPUserLoginHistory.EndDateTime IS NULL
