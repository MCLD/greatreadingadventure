
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_GetByID] @TenID INT
AS
SELECT *
FROM [CustomEventFields]
WHERE TenID = @TenID
