
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAllFromPatron]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetAllFromPatron] @PID INT
AS
SELECT *
FROM [Notifications]
WHERE PID_From = @PID
ORDER BY AddedDate DESC
