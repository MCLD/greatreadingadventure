
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetByMGID] @MGID INT
AS
SELECT *
FROM MGHiddenPic
WHERE MGID = @MGID
