
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdv_GetByMGID] @MGID INT
AS
SELECT *
FROM MGChooseAdv
WHERE MGID = @MGID
