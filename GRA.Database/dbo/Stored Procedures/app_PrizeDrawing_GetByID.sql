
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_GetByID] @PDID INT
AS
SELECT *
FROM [PrizeDrawing]
WHERE PDID = @PDID
