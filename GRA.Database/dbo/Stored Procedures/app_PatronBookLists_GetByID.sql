
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_GetByID] @PBLBID INT
AS
SELECT *
FROM [PatronBookLists]
WHERE PBLBID = @PBLBID
