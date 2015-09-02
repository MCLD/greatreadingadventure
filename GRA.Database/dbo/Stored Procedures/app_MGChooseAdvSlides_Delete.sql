
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Delete]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Delete] @CASID INT
AS
DECLARE @MGID INT

SELECT @MGID = MGID
FROM MGChooseAdvSlides
WHERE CASID = @CASID

DELETE
FROM MGChooseAdvSlides
WHERE CASID = @CASID

EXEC app_MGChooseAdvSlides_Reorder @MGID
