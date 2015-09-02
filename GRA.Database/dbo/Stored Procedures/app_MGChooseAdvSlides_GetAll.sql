
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetAll] @MGID INT = 0
AS
SELECT *
FROM MGChooseAdvSlides
WHERE MGID = @MGID
