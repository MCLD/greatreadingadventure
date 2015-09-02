
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_GetAll] @PID INT
AS
SELECT *
FROM [PatronRewardCodes]
WHERE PID = @PID
