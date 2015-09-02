
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_GetByID] @SQMLID INT
AS
SELECT *
FROM [SQMatrixLines]
WHERE SQMLID = @SQMLID
