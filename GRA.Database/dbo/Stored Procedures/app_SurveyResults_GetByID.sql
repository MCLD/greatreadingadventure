
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetByID] @SRID INT
AS
SELECT *
FROM [SurveyResults]
WHERE SRID = @SRID
