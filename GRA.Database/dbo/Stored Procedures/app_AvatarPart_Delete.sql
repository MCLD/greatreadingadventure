
CREATE PROCEDURE [dbo].[app_AvatarPart_Delete] @APID INT,
	@TenID INT = NULL
AS
DELETE
FROM [AvatarPart]
WHERE APID = @APID
	AND TenID = @TenID
