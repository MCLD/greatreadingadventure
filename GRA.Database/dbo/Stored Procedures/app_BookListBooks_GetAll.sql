
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetAll]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_GetAll] @BLID INT
AS
SELECT *
FROM [BookListBooks]
WHERE BLID = @BLID
