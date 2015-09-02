
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetAllWinners]    Script Date: 01/05/2015 14:43:24 ******/
CREATE PROCEDURE [dbo].[app_PrizeDrawing_GetAllWinners] @PDID INT = 0
AS
SELECT pdw.*,
	p.Username,
	p.FirstName,
	p.LastName
FROM dbo.PrizeDrawingWinners pdw
LEFT JOIN Patron p ON pdw.PatronID = p.PID
WHERE PDID = @PDID
ORDER BY PDID DESC
