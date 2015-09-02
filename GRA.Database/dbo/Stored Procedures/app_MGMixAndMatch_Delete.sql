
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_Delete] @MMID INT
AS
DELETE
FROM [MGMixAndMatch]
WHERE MMID = @MMID
