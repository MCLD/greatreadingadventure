
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Get]    Script Date: 01/05/2015 14:43:27 ******/
/*
procedure [DAL].[Applicationuser_IO]
@Action		int			= 0
, @AuditpointID
as
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
declare @RowCount int
@@IDENTITY
@RowCount = @@ROWCOUNT
raiserror('UDE-CONCURRENCY',11,11) with SETERROR
set @intErrFlag = 11
*/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Get] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPUser
WHERE UID = @UID
