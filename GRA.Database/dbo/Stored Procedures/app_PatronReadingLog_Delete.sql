
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_Delete] @PRLID INT
AS
DELETE
FROM [PatronReadingLog]
WHERE PRLID = @PRLID
