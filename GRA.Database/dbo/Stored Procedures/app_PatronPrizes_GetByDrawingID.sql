
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetByDrawingID]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPrizes_GetByDrawingID] @DrawingID INT
AS
SELECT *
FROM [PatronPrizes]
WHERE DrawingID = @DrawingID
