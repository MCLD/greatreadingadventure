
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Reorder]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Reorder] @MGID INT
AS
UPDATE MGChooseAdvSlides
SET StepNumber = rowNumber
FROM MGChooseAdvSlides
INNER JOIN (
	SELECT CASID,
		StepNumber,
		MGID,
		Difficulty,
		row_number() OVER (
			ORDER BY StepNumber ASC
			) AS rowNumber
	FROM MGChooseAdvSlides
	WHERE MGID = @MGID
		AND Difficulty = 1
	) drRowNumbers ON drRowNumbers.CASID = MGChooseAdvSlides.CASID
	AND drRowNumbers.MGID = @MGID
	AND drRowNumbers.Difficulty = 1

UPDATE MGChooseAdvSlides
SET StepNumber = rowNumber
FROM MGChooseAdvSlides
INNER JOIN (
	SELECT CASID,
		StepNumber,
		MGID,
		Difficulty,
		row_number() OVER (
			ORDER BY StepNumber ASC
			) AS rowNumber
	FROM MGChooseAdvSlides
	WHERE MGID = @MGID
		AND Difficulty = 2
	) drRowNumbers ON drRowNumbers.CASID = MGChooseAdvSlides.CASID
	AND drRowNumbers.MGID = @MGID
	AND drRowNumbers.Difficulty = 2

UPDATE MGChooseAdvSlides
SET StepNumber = rowNumber
FROM MGChooseAdvSlides
INNER JOIN (
	SELECT CASID,
		StepNumber,
		MGID,
		Difficulty,
		row_number() OVER (
			ORDER BY StepNumber ASC
			) AS rowNumber
	FROM MGChooseAdvSlides
	WHERE MGID = @MGID
		AND Difficulty = 3
	) drRowNumbers ON drRowNumbers.CASID = MGChooseAdvSlides.CASID
	AND drRowNumbers.MGID = @MGID
	AND drRowNumbers.Difficulty = 3
