
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAllForPatron]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAllForPatron] @PID INT
AS
SELECT *
FROM [ProgramCodes]
WHERE PatronId = @PID
