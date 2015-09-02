
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetAll] @MGID INT = 0
AS
SELECT *
FROM MGHiddenPicBk
WHERE MGID = @MGID
