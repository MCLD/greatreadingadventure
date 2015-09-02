
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetByID] @HPID INT
AS
SELECT *
FROM [MGHiddenPic]
WHERE HPID = @HPID
