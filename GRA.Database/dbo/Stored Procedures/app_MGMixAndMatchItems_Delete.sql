
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_Delete] @MMIID INT
AS
DELETE
FROM [MGMixAndMatchItems]
WHERE MMIID = @MMIID
