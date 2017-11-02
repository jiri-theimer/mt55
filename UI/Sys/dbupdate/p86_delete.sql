if exists (select 1 from sysobjects where  id = object_id('p86_delete') and type = 'P')
 drop procedure p86_delete
GO







CREATE   procedure [dbo].[p86_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p86id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu bankovního úètu z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p88ID from p88InvoiceHeader_BankAccount WHERE p86ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna hlavièka vystavovatele faktur obsahuje vazbu na tento úèet.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	

	DELETE from p86BankAccount where p86ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO