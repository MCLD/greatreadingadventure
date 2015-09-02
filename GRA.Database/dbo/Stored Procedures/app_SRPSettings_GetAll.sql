
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [SRPSettings]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
