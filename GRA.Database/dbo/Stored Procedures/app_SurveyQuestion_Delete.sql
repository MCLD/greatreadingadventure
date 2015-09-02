
CREATE PROCEDURE [dbo].[app_SurveyQuestion_Delete] @QID INT
AS
DECLARE @SID INT;

SELECT @SID = SID
FROM [SurveyQuestion]
WHERE QID = @QID

DELETE
FROM [SurveyQuestion]
WHERE QID = @QID

EXEC app_SurveyQuestion_Reorder @SID
