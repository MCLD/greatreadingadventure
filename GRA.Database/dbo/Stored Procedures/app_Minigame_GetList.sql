
CREATE PROCEDURE [dbo].[app_Minigame_GetList] @IDList VARCHAR(1000) = ''
AS
BEGIN
	DECLARE @tmp AS TABLE (
		Num INT identity,
		Value INT
		)

	INSERT INTO @tmp (value)
	SELECT Value
	FROM [dbo].[fnSplitBigInt](@IDList)

	--select * from @tmp
	SELECT m.*
	FROM Minigame m
	INNER JOIN @tmp t ON m.MGID = t.Value
	ORDER BY Num DESC
END
