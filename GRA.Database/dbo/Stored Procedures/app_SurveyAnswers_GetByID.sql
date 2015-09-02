
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_GetByID] @SAID INT
AS
SELECT *
FROM [SurveyAnswers]
WHERE SAID = @SAID
