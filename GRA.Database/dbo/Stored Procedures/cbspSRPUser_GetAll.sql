
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetAll] @TenID INT = NULL
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPUser
WHERE IsDeleted = 0
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
