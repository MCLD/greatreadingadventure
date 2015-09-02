
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_GetByID] @WMIID INT
AS
SELECT *
FROM [MGWordMatchItems]
WHERE WMIID = @WMIID
