
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQChoices_GetAll] @QID INT,
	@Echo INT = 0
AS
SELECT *,
	(
		SELECT isnull(Max(ChoiceOrder), 0)
		FROM [SQChoices]
		WHERE QID = @QID
		) AS MAX,
	@Echo AS Echo
FROM [SQChoices]
WHERE QID = @QID
ORDER BY ChoiceOrder
