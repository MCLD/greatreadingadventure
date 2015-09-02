
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetForPatronDisplay]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_BookListBooks_GetForPatronDisplay] @BLID INT = 0,
	@PID INT = 0
AS
SELECT isnull(p.HasReadFlag, 0) AS HasRead,
	isnull(p.PBLBID, 0) AS PBLBID,
	b.*
FROM BookListBooks b
LEFT JOIN PatronBookLists p ON b.BLBID = p.BLBID
	AND b.BLID = p.BLID
	AND p.PID = @PID
WHERE b.BLID = @BLID
