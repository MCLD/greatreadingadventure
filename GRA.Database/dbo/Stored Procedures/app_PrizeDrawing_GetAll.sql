
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_GetAll] @TenID INT = NULL
AS
SELECT pd.*,
	t.TName
FROM [PrizeDrawing] pd
LEFT JOIN PrizeTemplate t ON pd.TID = t.TID
	AND pd.TenID = t.TenID
WHERE pd.TenID = @TenID
ORDER BY PDID DESC
