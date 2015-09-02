
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_GetByID] @CAID INT
AS
SELECT *
FROM [MGChooseAdv]
WHERE CAID = @CAID
