
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_GetByID] @MMID INT
AS
SELECT *
FROM [MGMixAndMatch]
WHERE MMID = @MMID
