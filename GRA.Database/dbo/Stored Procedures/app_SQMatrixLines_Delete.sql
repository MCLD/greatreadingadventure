
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_Delete] @SQMLID INT
AS
DECLARE @QID INT;

SELECT @QID = QID
FROM [SQMatrixLines]
WHERE SQMLID = @SQMLID

DELETE
FROM [SQMatrixLines]
WHERE SQMLID = @SQMLID

EXEC app_SQMatrixLines_Reorder @QID
