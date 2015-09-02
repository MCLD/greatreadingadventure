
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_GetByID] @MMIID INT
AS
SELECT *
FROM [MGMixAndMatchItems]
WHERE MMIID = @MMIID
