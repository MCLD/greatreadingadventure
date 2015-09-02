
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_Delete] @PBLBID INT
AS
DELETE
FROM [PatronBookLists]
WHERE PBLBID = @PBLBID
