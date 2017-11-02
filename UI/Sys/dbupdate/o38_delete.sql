if exists (select 1 from sysobjects where  id = object_id('o38_delete') and type = 'P')
 drop procedure o38_delete
GO









CREATE   procedure [dbo].[o38_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--a38id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu adresy z tabulky o38Address
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE o38ID_Primary=@pid or o38ID_Delivery=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna klientská faktura má vazbu na tuto adresu ('+dbo.GetObjectAlias('p91',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 


BEGIN TRANSACTION

BEGIN TRY

	delete from o38Address where o38ID=@pid
	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  





GO