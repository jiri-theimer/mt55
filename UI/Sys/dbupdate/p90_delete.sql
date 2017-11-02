if exists (select 1 from sysobjects where  id = object_id('p90_delete') and type = 'P')
 drop procedure p90_delete
GO



CREATE   procedure [dbo].[p90_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p90id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu vystavené faktury z tabulky p91Invoice
declare @ref_pid int


if isnull(@err_ret,'')<>''
 return
 

declare @p91id_bind int

BEGIN TRANSACTION

BEGIN TRY

if exists(select p99ID FROM p99Invoice_Proforma WHERE p91ID=@pid)
 begin
  select @p91id_bind=p91ID FROM p99Invoice_Proforma WHERE p90ID=@pid

  delete from p99Invoice_Proforma where p90id=@pid
 end

if exists(SELECT o27ID FROM o27Attachment WHERE p90ID=@pid)
  DELETE FROM o27Attachment WHERE p90ID=@pid

if exists(select p90ID FROM p90Proforma_FreeField WHERE p90ID=@pid)
  delete from p90Proforma_FreeField where p90id=@pid

if exists(SELECT o22ID FROM o22Milestone WHERE p90ID=@pid)
  DELETE FROM o22Milestone WHERE p90ID=@pid

if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=390)
  DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=390


DELETE FROM x90EntityLog WHERE x29ID=390 AND x90RecordPID=@pid


delete from p90Proforma where p90ID=@pid

	
COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

if @p91id_bind is not null
 exec p91_recalc_amount @p91id_bind,0
















GO