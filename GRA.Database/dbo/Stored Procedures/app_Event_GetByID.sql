
/****** Object:  StoredProcedure [dbo].[app_Event_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Event_GetByID] @EID INT
AS
SELECT *,
	(
		SELECT Code
		FROM dbo.Code
		WHERE CID = BranchID
		) AS Branch
FROM [Event]
WHERE EID = @EID
