
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_GetAll]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_GetAll]
AS
SELECT *
FROM [PatronBookLists]
