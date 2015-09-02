
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetAll]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_GetAll] @PGID INT = 0
AS
SELECT *
FROM [ProgramGamePointConversion]
WHERE PGID = @PGID
