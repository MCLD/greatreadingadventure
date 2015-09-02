
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_GetByID] @PRCID INT
AS
SELECT *
FROM [PatronRewardCodes]
WHERE PRCID = @PRCID
