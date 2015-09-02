
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_GetAll] @MGID INT
AS
SELECT *
FROM [MGWordMatchItems]
WHERE MGID = @MGID
