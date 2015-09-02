
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetByID] @CASID INT
AS
SELECT *
FROM [MGChooseAdvSlides]
WHERE CASID = @CASID
