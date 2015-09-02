
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetAllExpanded] @TenID INT = 0,
	@PID INT = NULL,
	@SID INT = NULL
AS
SELECT *
FROM [SurveyResults]
WHERE TenID = @TenID
	AND (
		PID = @PID
		OR @PID IS NULL
		)
	AND (
		SID = @SID
		OR @SID IS NULL
		)
