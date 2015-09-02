
CREATE PROCEDURE [dbo].[app_SQChoices_Reorder] @QID INT
AS
UPDATE SQChoices
SET ChoiceOrder = rowNumber
FROM SQChoices
INNER JOIN (
	SELECT SQCID,
		ChoiceOrder,
		row_number() OVER (
			ORDER BY ChoiceOrder ASC
			) AS rowNumber
	FROM SQChoices
	WHERE QID = @QID
	) drRowNumbers ON drRowNumbers.SQCID = SQChoices.SQCID
	AND QID = @QID
WHERE QID = @QID
