if exists (select 1 from sysobjects where  id = object_id('p91_create_creditnote') and type = 'P')
 drop procedure p91_create_creditnote
GO



CREATE procedure [dbo].[p91_create_creditnote]
@j03id_sys int
,@p91id_bind int
,@p92id_creditnote int
,@err_ret varchar(1000) OUTPUT
,@ret_p91id int OUTPUT

AS

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @p91id_bind=isnull(@p91id_bind,0)
set @p92id_creditnote=isnull(@p92id_creditnote,0)


if @j03id_sys=0
 begin
  set @err_ret='@j03id_sys is missing!'
  return
 end

declare @login nvarchar(50),@j02id_sys int,@guid varchar(50)
select @login=j03login,@j02id_sys=j02ID from j03user where j03id=@j03id_sys


if @p92id_creditnote=0
  set @err_ret='@p92id_creditnote is missing!'


if @err_ret<>''
  return

declare @x38id int,@p28id int,@j27id int,@p32id_creditnote int

if not exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'p32ID_CreditNote')
 begin
  set @err_ret='V globálním nastavení chybí hodnota parametru [p32ID_CreditNote].'
  return
 end

select @p32id_creditnote=convert(int,x35value) from x35GlobalParam WHERE x35Key like 'p32ID_CreditNote'

if not exists(select p32ID FROM p32Activity WHERE p32ID=@p32id_creditnote)
 set @err_ret='Pro parametr [p32ID_CreditNote] v systému neexistuje záznam aktivity!'

select @x38id=x38id from p92InvoiceType where p92id=@p92id_creditnote

select @p28id=p28id,@j27id=j27id from p91invoice where p91id=@p91id_bind

if @x38id is null
  set @err_ret='@x38id is not found!'


if @err_ret<>''
  return

insert into p91invoice(
p92id,p91dateinsert,p91userinsert,p91Date,p91DateSupply,p91DateMaturity,j02ID_Owner,p28id,j27id,p91dateupdate,p91userupdate
,o38ID_Primary,o38ID_Delivery,x15ID,p91fixedvatrate,j02ID_ContactPerson
,j19id,p41ID_First,j17ID
,p91ID_CreditNoteBind,p91Datep31_From,p91Datep31_Until
)
select @p92id_creditnote,getdate(),@login,getdate(),getdate(),getdate(),@j02id_sys,p28id,j27id,getdate(),@login
,o38ID_Primary,o38ID_Delivery,x15ID,p91fixedvatrate,j02ID_ContactPerson
,j19id,p41ID_First,j17ID
,@p91id_bind,p91Datep31_From,p91Datep31_Until
FROM p91invoice
where p91id=@p91id_bind

SELECT @ret_p91id=@@IDENTITY



exec p91_update_code @ret_p91id,@j03id_sys

---worksheet
declare @c11id int,@p31date datetime,@p31id int,@p31text nvarchar(300)
set @p31text='Dobropisovaná èástka'
select top 1 @c11id=c11id,@p31date=c11datefrom from c11statperiod where c11level=5 and c11datefrom<=getdate() order by c11id desc

declare @amount_withoutvat decimal(18,2),@amount_withvat decimal(18,2),@vatrate decimal(18,2),@amount_vat decimal(18,2),@p41id int
DECLARE curP31 CURSOR FOR 
select p41id,p31VatRate_Invoiced,-1*sum(p31Amount_WithoutVat_Invoiced),-1*sum(p31Amount_WithVat_Invoiced),-1*sum(p31Amount_Vat_Invoiced)
from p31worksheet where p91id=@p91id_bind
GROUP BY p41id,p31VatRate_Invoiced
	
OPEN curP31
FETCH NEXT FROM curP31 
INTO @p41id,@vatrate,@amount_withoutvat,@amount_withvat,@amount_vat
WHILE @@FETCH_STATUS = 0
BEGIN
  insert into p31worksheet(j02ID,p41ID,p32ID,j27ID_Billing_Orig,c11id,p31Date,p31Amount_WithoutVat_Orig,p31Amount_WithVat_Orig,p31VatRate_Orig,j02ID_Owner,p31UserInsert,p31UserUpdate,p31DateInsert,p31DateUpdate,p31value_orig,p31text,p31HoursEntryFlag)
  values(@j02id_sys,@p41id,@p32id_creditnote,@j27id,@c11id,@p31date,@amount_withoutvat,@amount_withvat,@vatrate,@j02id_sys,@login,@login,getdate(),getdate(),@amount_withoutvat,@p31text,0)

  SELECT @p31id=@@IDENTITY

  set @guid=convert(varchar(10),@ret_p91id)+'-'+convert(varchar(10),@p31id)+'-'+convert(varchar(50),getdate())
  insert into p85TempBox(p85GUID,p85DataPID,p85Prefix) values(@guid,@p31id,'p31')

  exec p31_save_approving @p31id,@j03id_sys,1,4,null,@amount_withoutvat,null,null,@p31text,@vatrate,@err_ret OUTPUT

  print 'p31_save_approving, p31id: '+convert(varchar(10),@p31id)+', error: '+@err_ret
  

  exec p31_append_invoice @ret_p91id,@guid,@j03id_sys,@err_ret OUTPUT

  print 'p31_append_invoice, p31id: '+convert(varchar(10),@p31id)+', error: '+@err_ret

  ---exec p31_save_approving @p31id, @j03id, 1, 4, null, null, null,null, @amount_withoutvat, null, null, null, @vatrate, @err_ret OUTPUT 

  ---exec p31_save_invoice @p31id, @j03id, 4, @amount_withoutvat, @amount_withvat, @amount_vat, null, @p31text, @vatrate, @err_ret OUTPUT 

  FETCH NEXT FROM curP31 
  INTO @p41id,@vatrate,@amount_withoutvat,@amount_withvat,@amount_vat
END
CLOSE curP31
DEALLOCATE curP31


exec dbo.p91_recalc_amount @ret_p91id



GO