
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_GetAll] @MGID INT = 0
AS
SELECT *
FROM [MGMixAndMatchItems]
WHERE MGID = @MGID
