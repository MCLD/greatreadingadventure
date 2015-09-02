
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetLoginHistory]    Script Date: 01/05/2015 14:43:27 ******/
---------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetLoginHistory] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT h.*,
	u.Username,
	u.FirstName + ' ' + u.LastName AS NAME
FROM dbo.SRPUserLoginHistory h,
	dbo.SRPUser u
WHERE u.UID = @UID
	AND u.UID = h.UID
ORDER BY StartDateTime DESC
