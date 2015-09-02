
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_Delete] @BLBID INT,
	@TenID INT = NULL
AS
DELETE
FROM [BookListBooks]
WHERE BLBID = @BLBID
	AND TenID = @TenID
