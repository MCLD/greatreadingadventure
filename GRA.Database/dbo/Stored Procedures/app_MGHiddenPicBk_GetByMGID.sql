
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetByMGID] @MGID INT
AS
SELECT *
FROM MGHiddenPicBk
WHERE MGID = @MGID
