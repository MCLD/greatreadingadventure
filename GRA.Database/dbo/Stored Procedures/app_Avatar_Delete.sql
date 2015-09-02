
CREATE PROCEDURE [dbo].[app_Avatar_Delete] @AID INT,
	@TenID INT = NULL
AS
DELETE
FROM [Avatar]
WHERE AID = @AID
	AND TenID = @TenID
