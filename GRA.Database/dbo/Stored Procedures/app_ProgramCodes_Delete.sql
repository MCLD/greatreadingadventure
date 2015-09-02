
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_Delete] @PCID INT
AS
DELETE
FROM [ProgramCodes]
WHERE PCID = @PCID
