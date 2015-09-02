
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAllByProgram]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAllByProgram] @PID INT
AS
SELECT *
FROM [ProgramCodes]
WHERE PID = @PID
