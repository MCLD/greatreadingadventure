
CREATE PROCEDURE [dbo].[app_Code_GetAllTypeName] @name VARCHAR(50),
	@TenID INT = NULL
AS
SELECT *
FROM [Code]
WHERE CTID = (
		SELECT CTID
		FROM dbo.CodeType
		WHERE CodeTypeName = @name
			AND (
				TenID = @TenID
				OR @TenID IS NULL
				)
		)
ORDER BY Code
