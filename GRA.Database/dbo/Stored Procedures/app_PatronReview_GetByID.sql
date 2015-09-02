
/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReview_GetByID] @PRID INT
AS
SELECT *
FROM [PatronReview]
WHERE PRID = @PRID
