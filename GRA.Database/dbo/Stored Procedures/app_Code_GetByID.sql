
/****** Object:  StoredProcedure [dbo].[app_Code_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Code_GetByID] @CID INT
AS
SELECT *
FROM [Code]
WHERE CID = @CID
