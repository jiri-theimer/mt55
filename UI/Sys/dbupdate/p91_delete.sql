if exists (select 1 from sysobjects where  id = object_id('p91_delete') and type = 'P')
 drop procedure p91_delete
GO






CREATE   procedure [dbo].[p91_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p91id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu vystavené faktury z tabulky p91Invoice
declare @ref_pid int


if isnull(@err_ret,'')<>''
 return
 
declare @p92invoicetype int,@p92id int,@p91id_bind int,@p32id_overhead int

if exists(select x35ID FROM x35GlobalParam WHERE x35Key LIKE 'p32ID_Overhead')
  select @p32id_overhead=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'p32ID_Overhead'

select @p92invoicetype=p92invoicetype,@p91id_bind=p91ID_CreditNoteBind
FROM p91Invoice a inner join p92InvoiceType b on a.p92id=b.p92id
where a.p91id=@pid

BEGIN TRANSACTION

BEGIN TRY


if @p92invoicetype=2
 delete from p31WorkSheet where p91id=@pid	--u dobropisu se mažou zdrojové worksheet záznamy
 
if @p32id_overhead is not null
 begin
  if exists(select p31ID FROM p31Worksheet WHERE p91ID=@pid AND p32id=@p32id_overhead)
    delete FROM p31worksheet WHERE p91id=@pid AND p32id=@p32id_overhead
 end

update p31Worksheet set p91ID=null,p70ID=null
,j27ID_Billing_Invoiced=null,j27ID_Billing_Invoiced_Domestic=null,p31Value_Invoiced=null,p31Rate_Billing_Invoiced=null
,p31Amount_WithoutVat_Invoiced=null,p31Amount_WithVat_Invoiced=null,p31Amount_Vat_Invoiced=null,p31VatRate_Invoiced=null
,p31Amount_WithoutVat_Invoiced_Domestic=null,p31Amount_WithVat_Invoiced_Domestic=null,p31Amount_Vat_Invoiced_Domestic=null
,p31Minutes_Invoiced=null,p31HHMM_Invoiced=null,p31ExchangeRate_Invoice=null
where p91ID=@pid

if exists(select p94ID FROM p94Invoice_Payment where p91ID=@pid)
  delete from p94Invoice_Payment where p91id=@pid

if exists(select p96ID FROM p96Invoice_ExchangeRate where p91ID=@pid)
  delete from p96Invoice_ExchangeRate where p91id=@pid

if exists(select p91ID FROM p91Invoice_FreeField WHERE p91ID=@pid)
  delete from p91Invoice_FreeField where p91id=@pid

if exists(select p99ID FROM p99Invoice_Proforma WHERE p91ID=@pid)
  delete from p99Invoice_Proforma where p91id=@pid

if exists(SELECT o27ID FROM o27Attachment WHERE p91ID=@pid)
  DELETE FROM o27Attachment WHERE p91ID=@pid

if exists(SELECT o23ID FROM o23Notepad WHERE p91ID=@pid)
  DELETE FROM o23Notepad WHERE p91ID=@pid

if exists(SELECT o22ID FROM o22Milestone WHERE p91ID=@pid)
  DELETE FROM o22Milestone WHERE p91ID=@pid

if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=391)
  DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=391


DELETE FROM x90EntityLog WHERE x29ID=391 AND x90RecordPID=@pid


delete from p91Invoice where p91id=@pid

	
COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

--if @p91id_bind is not null
-- exec p91_recalc_amount @p91id_bind,0















GO