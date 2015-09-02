
CREATE PROCEDURE [dbo].[app_Survey_GetNumQuestions] @SID INT = NULL
AS
SELECT isnull(Max(QNumber), 0) AS NumQuestions
FROM SurveyQuestion
WHERE SID = @SID
