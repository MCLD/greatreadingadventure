
/****** Object:  StoredProcedure [dbo].[app_PatronReview_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronReview_Delete] @PRID INT
AS
DELETE
FROM [PatronReview]
WHERE PRID = @PRID
