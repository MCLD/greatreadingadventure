
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [CustomEventFields]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
