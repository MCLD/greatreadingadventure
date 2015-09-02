
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetAllByDifficulty]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetAllByDifficulty] @MGID INT = 0,
	@Diff INT = 1
AS
SELECT *,
	(
		SELECT MAX(StepNumber)
		FROM MGChooseAdvSlides
		WHERE MGID = @MGID
			AND Difficulty = @Diff
		) AS MAX
FROM MGChooseAdvSlides
WHERE MGID = @MGID
	AND Difficulty = @Diff
ORDER BY StepNumber
