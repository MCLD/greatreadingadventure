
/****** Object:  StoredProcedure [dbo].[app_Patron_CanManageSubAccount]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_Patron_CanManageSubAccount] @MainAccount INT = 0,
	@SubAccount INT = 0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @Count INT,
	@UID INT

SELECT @Count = isnull(Count(*), 0)
FROM dbo.Patron
WHERE PID = @SubAccount
	AND MasterAcctPID = @MainAccount
GROUP BY PID

IF @Count = 0
	OR @Count IS NULL
BEGIN
	SELECT 0
END
ELSE
BEGIN
	SELECT 1
END

RETURN 0
