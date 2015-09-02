
CREATE PROCEDURE [dbo].[app_Notifications_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Notifications]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY AddedDate DESC
