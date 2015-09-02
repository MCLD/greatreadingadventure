
CREATE PROCEDURE [dbo].[app_BookList_GetAll] @TenID INT = NULL
AS
SELECT bl.*,
	isnull(p.AdminName, '') AS ProgName,
	isnull(c.Code, '') AS Library
FROM [BookList] bl
LEFT JOIN Programs p ON bl.ProgID = p.PID
	AND bl.TenID = p.TenID
LEFT JOIN Code c ON bl.LibraryID = c.cid
WHERE bl.TenID = @TenID
