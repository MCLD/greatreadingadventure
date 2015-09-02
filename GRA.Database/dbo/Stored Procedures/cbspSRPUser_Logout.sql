
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Logout]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_Logout] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUserLoginHistory
SET EndDateTime = getdate()
WHERE UID = @UID
	AND EndDateTime IS NULL
