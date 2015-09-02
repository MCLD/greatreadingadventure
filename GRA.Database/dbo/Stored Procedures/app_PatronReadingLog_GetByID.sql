
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_GetByID] @PRLID INT
AS
SELECT *
FROM [PatronReadingLog]
WHERE PRLID = @PRLID
