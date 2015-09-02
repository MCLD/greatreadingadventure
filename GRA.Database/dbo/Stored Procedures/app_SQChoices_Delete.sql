
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SQChoices_Delete] @SQCID INT
AS
DECLARE @QID INT;

SELECT @QID = QID
FROM [SQChoices]
WHERE SQCID = @SQCID

DELETE
FROM [SQChoices]
WHERE SQCID = @SQCID

EXEC app_SQChoices_Reorder @QID
