
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_GetByID] @PDWID INT
AS
SELECT *
FROM [PrizeDrawingWinners]
WHERE PDWID = @PDWID
