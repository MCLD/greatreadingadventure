
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllTypeID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Code_GetAllTypeID] @ID INT
AS
SELECT *
FROM [Code]
WHERE CTID = @ID
