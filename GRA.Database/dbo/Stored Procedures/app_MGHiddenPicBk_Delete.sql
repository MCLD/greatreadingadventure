
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_Delete] @HPBID INT
AS
DELETE
FROM [MGHiddenPicBk]
WHERE HPBID = @HPBID
