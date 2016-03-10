
CREATE PROCEDURE [dbo].[app_AvatarPartrGetAll] @TenID INT = NULL
AS
SELECT *
FROM [AvatarPart]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
