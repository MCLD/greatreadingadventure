
CREATE PROCEDURE [dbo].[cbspSRPUser_GetLoginNow]
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT h.*,
	u.Username,
	u.FirstName + ' ' + u.LastName AS NAME,
	t.AdminName AS Tenant
FROM dbo.SRPUserLoginHistory h,
	dbo.SRPUser u
INNER JOIN dbo.Tenant t ON u.TenID = t.TenID
WHERE EndDateTime IS NULL
	AND u.UID = h.UID
ORDER BY StartDateTime DESC
