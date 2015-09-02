
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetRandom3]    Script Date: 01/05/2015 14:43:22 ******/
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_GetRandom3] @MMID INT
AS
SELECT TOP 3 NEWID() AS id,
	*
FROM dbo.MGMixAndMatchItems
WHERE MMID = @MMID
ORDER BY id
