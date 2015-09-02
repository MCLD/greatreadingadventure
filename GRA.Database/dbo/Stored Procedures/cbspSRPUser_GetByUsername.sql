
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetByUsername]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetByUsername] @Username VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPUser
WHERE Username = @Username
