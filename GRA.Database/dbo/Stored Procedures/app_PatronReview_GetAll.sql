
/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReview_GetAll] @pid INT = 0
AS
SELECT *
FROM [PatronReview]
WHERE @PID = PID
