
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_GetByID] @PBID INT
AS
SELECT *
FROM [PatronBadges]
WHERE PBID = @PBID
