
/****** Object:  StoredProcedure [dbo].[app_Programs_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Programs_GetByID] @PID INT
AS
SELECT *
FROM [Programs]
WHERE PID = @PID
