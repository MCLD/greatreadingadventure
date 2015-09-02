
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_Delete] @WMID INT
AS
DELETE
FROM [MGWordMatch]
WHERE WMID = @WMID
