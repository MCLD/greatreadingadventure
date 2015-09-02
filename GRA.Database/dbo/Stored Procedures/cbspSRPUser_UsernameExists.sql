
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UsernameExists]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_UsernameExists] @Username VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @RowCount INT

SELECT @RowCount = Count(*)
FROM dbo.SRPUser
WHERE Username = @Username

IF @RowCount > 0
	RETURN 1
ELSE
	RETURN 0
