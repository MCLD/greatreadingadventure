
/****** Object:  StoredProcedure [dbo].[app_BookList_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_BookList_GetByID] @BLID INT
AS
SELECT *
FROM [BookList]
WHERE BLID = @BLID
