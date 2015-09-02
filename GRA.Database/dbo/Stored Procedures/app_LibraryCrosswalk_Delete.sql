
CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_Delete] @ID INT
AS
DELETE
FROM [LibraryCrosswalk]
WHERE ID = @ID
