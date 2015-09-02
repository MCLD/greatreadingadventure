
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [CustomRegistrationFields]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
