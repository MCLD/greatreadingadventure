
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetByID] @HPBID INT
AS
SELECT *
FROM [MGHiddenPicBk]
WHERE HPBID = @HPBID
