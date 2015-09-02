
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_GetAll] @PID INT = 0
AS
SELECT *
FROM [PatronReadingLog]
WHERE @PID = PID
ORDER BY LoggingDate DESC
