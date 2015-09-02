
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_GetByID] @PPID INT
AS
SELECT *
FROM [PatronPrizes]
WHERE PPID = @PPID
