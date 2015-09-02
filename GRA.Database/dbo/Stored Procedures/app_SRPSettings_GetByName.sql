
CREATE PROCEDURE [dbo].[app_SRPSettings_GetByName] @Name VARCHAR(50),
	@TenID INT = NULL
AS
SELECT *
FROM [SRPSettings]
WHERE NAME = @Name
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
