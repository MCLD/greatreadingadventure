
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_Delete] @CAID INT
AS
DELETE
FROM [MGChooseAdv]
WHERE CAID = @CAID
