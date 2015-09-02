
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_Delete] @PDWID INT
AS
DELETE
FROM [PrizeDrawingWinners]
WHERE PDWID = @PDWID
