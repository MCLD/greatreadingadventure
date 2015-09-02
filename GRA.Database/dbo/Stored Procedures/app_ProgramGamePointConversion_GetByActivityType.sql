
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetByActivityType]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_GetByActivityType] @PGID INT,
	@ActivityTypeID INT
AS
SELECT *
FROM [ProgramGamePointConversion]
WHERE ActivityTypeId = @ActivityTypeID
	AND PGID = @PGID
