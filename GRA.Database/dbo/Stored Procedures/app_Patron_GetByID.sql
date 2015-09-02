
/****** Object:  StoredProcedure [dbo].[app_Patron_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Patron_GetByID] @PID INT
AS
SELECT *
FROM [Patron]
WHERE PID = @PID
