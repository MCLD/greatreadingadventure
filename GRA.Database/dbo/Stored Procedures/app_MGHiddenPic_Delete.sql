
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_Delete] @HPID INT
AS
DELETE
FROM [MGHiddenPic]
WHERE HPID = @HPID
