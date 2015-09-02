
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetPlaySlide]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetPlaySlide] @CAID INT,
	@Step INT = 1,
	@Difficulty INT = 1
AS
SELECT *
FROM [MGChooseAdvSlides]
WHERE CAID = @CAID
	AND StepNumber = @Step
	AND Difficulty = @Difficulty
