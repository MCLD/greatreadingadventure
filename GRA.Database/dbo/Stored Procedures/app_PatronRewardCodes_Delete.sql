
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_Delete] @PRCID INT
AS
DELETE
FROM [PatronRewardCodes]
WHERE PRCID = @PRCID
