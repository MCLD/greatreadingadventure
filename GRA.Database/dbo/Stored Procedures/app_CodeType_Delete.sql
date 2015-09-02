
/****** Object:  StoredProcedure [dbo].[app_CodeType_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_CodeType_Delete] @CTID INT
AS
DELETE
FROM [CodeType]
WHERE CTID = @CTID
