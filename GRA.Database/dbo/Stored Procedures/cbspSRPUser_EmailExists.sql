
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_EmailExists]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_EmailExists] @EmailAddress VARCHAR(128)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @RowCount INT

SELECT @RowCount = Count(*)
FROM dbo.SRPUser
WHERE LOWER(EmailAddress) = LOWER(@EmailAddress)

IF @RowCount > 0
	RETURN 1
ELSE
	RETURN 0
