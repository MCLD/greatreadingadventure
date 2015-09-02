
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetByID] @NID INT
AS
SELECT *
FROM [Notifications]
WHERE NID = @NID
