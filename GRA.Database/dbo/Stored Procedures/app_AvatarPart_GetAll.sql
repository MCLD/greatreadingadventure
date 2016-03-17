
CREATE PROCEDURE [dbo].[app_AvatarPart_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [AvatarPart]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
