if exists (select 1 from sysobjects where  id = object_id('j04_delete') and type = 'P')
 drop procedure j04_delete
GO







CREATE   procedure [dbo].[j04_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j04id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu role z tabulky j04UserRole
declare @ref_pid int,@x67id int

select @x67id=x67ID FROM j04UserRole WHERE j04ID=@pid

if @x67id is null
 begin
  set @err_ret='x67id missing'
  return
 end

SELECT TOP 1 @ref_pid=j03ID from j03User WHERE j04ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden uživatelský úèet má vazbu na tuto aplikaèní roli ('+dbo.GetObjectAlias('j03',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM x68EntityRole_Permission WHERE x67ID=@x67id

	DELETE FROM x69EntityRole_Assign WHERE x67ID=@x67id

	DELETE FROM x67EntityRole WHERE x67ID=@x67id

	delete from j04UserRole where j04ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO