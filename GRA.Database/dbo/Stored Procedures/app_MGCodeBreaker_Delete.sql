
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_Delete] @CBID INT
AS
DELETE
FROM [MGCodeBreaker]
WHERE CBID = @CBID
