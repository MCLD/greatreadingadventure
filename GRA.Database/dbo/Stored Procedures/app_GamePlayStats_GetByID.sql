
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_GetByID] @GPSID INT
AS
SELECT *
FROM [GamePlayStats]
WHERE GPSID = @GPSID
