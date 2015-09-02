
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_Delete] @GPSID INT
AS
DELETE
FROM [GamePlayStats]
WHERE GPSID = @GPSID
