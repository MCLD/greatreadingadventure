
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQChoices_GetAllInList] @List VARCHAR(2000)
AS
SELECT *
FROM [SQChoices]
WHERE SQCID IN (
		SELECT *
		FROM dbo.fnSplitString(@List, ',')
		)
ORDER BY ChoiceOrder
