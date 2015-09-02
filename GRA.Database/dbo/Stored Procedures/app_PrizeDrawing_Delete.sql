
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_Delete] @PDID INT
AS
DELETE
FROM [PrizeDrawing]
WHERE PDID = @PDID
