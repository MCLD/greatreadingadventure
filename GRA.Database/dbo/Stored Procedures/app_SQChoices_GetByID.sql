
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQChoices_GetByID] @SQCID INT
AS
SELECT *
FROM [SQChoices]
WHERE SQCID = @SQCID
