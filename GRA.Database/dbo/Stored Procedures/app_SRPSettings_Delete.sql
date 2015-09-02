
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_Delete] @SID INT,
	@TenID INT = NULL
AS
DELETE
FROM [SRPSettings]
WHERE SID = @SID
	AND TenID = @TenID
