
CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetByLibraryID] @ID INT = 0
AS
SELECT *
FROM LibraryCrosswalk
WHERE BranchID = @ID
