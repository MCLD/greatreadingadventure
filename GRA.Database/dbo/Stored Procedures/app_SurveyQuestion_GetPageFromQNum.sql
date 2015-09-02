
CREATE PROCEDURE [dbo].[app_SurveyQuestion_GetPageFromQNum] @SID INT = NULL,
	@QNum INT = 0
AS
DECLARE @StopQID INT
DECLARE @StopQNum INT

SELECT @StopQID = NULL,
	@StopQNum = NULL

SELECT TOP 1 @StopQID = QID,
	@StopQNum = QNumber
FROM [SurveyQuestion]
WHERE SID = @SID
	AND QNumber > @QNum
	AND QType IN (
		5,
		6
		)
ORDER BY QNumber

--select @StopQID, @StopQNum
SELECT *,
	(
		SELECT isnull(Max(QNumber), 0)
		FROM SurveyQuestion
		WHERE SID = @SID
		) AS MAX
FROM [SurveyQuestion]
WHERE SID = @SID
	AND (QNumber > @QNum)
	AND (
		QNumber <= @StopQNum
		OR @StopQNum IS NULL
		)
ORDER BY QNumber
