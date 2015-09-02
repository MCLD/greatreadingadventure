
/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetByLogID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReview_GetByLogID] @PRLID INT
AS
SELECT *
FROM [PatronReview]
WHERE PRLID = @PRLID
