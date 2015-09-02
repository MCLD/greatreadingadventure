
/****** Object:  StoredProcedure [dbo].[app_Patron_GetSubAccountList]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_Patron_GetSubAccountList] @PID INT = 0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT subs.*,
	pg.AdminName AS Program
FROM dbo.Patron subs
INNER JOIN dbo.Patron mast ON subs.MasterAcctPID = mast.PID
	AND mast.PID = @PID
	AND mast.IsMasterAccount = 1
LEFT JOIN Programs pg ON subs.ProgID = pg.PID
	--order BY subs.PID desc
