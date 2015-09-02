
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetBySurveyAndSource] @PID INT,
	@SID INT,
	@SrcType VARCHAR(150),
	@SrcID INT
AS
SELECT TOP 1 *
FROM [SurveyResults]
WHERE PID = @PID
	AND SID = @SID
	AND Source = @SrcType
	AND SourceID = @SrcID
ORDER BY StartDate DESC
