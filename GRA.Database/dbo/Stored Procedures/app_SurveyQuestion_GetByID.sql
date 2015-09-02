
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyQuestion_GetByID] @QID INT
AS
SELECT *
FROM [SurveyQuestion]
WHERE QID = @QID
