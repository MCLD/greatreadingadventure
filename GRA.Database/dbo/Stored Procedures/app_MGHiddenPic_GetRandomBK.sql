
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetRandomBK]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetRandomBK] @HPID INT,
	@HPBID INT OUTPUT
AS
DECLARE @a INT

SELECT TOP 1 NEWID() AS id,
	HPBID
INTO #tmp
FROM dbo.MGHiddenPicBk
WHERE HPID = @HPID
ORDER BY id

SELECT @HPBID = HPBID
FROM #tmp

RETURN @a
