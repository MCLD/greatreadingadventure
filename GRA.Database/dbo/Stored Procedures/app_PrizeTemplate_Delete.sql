
/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_Delete] @TID INT
AS
DELETE
FROM [PrizeTemplate]
WHERE TID = @TID
