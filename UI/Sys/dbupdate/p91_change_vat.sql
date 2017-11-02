if exists (select 1 from sysobjects where  id = object_id('p91_change_vat') and type = 'P')
 drop procedure p91_change_vat
GO





CREATE     procedure [dbo].[p91_change_vat]
@p91id int
,@j03id_sys int
,@x15id int
,@newvatrate float
,@err_ret varchar(1000) OUTPUT

AS



set @p91id=isnull(@p91id,0)

declare @login nvarchar(50)

set @login=dbo.j03_getlogin(@j03id_sys)

set @err_ret=''

if @newvatrate is null
  set @err_ret='New VAT rate must be number!'

if @j03id_sys=0
 set @err_ret='@j03id_sys is missing!'


if @err_ret<>''
  return

----validace sazby dph------------------
declare @vatisok bit,@p91DateSupply datetime,@j17id int,@j27id int

select @p91DateSupply=p91DateSupply,@j17id=j17id,@j27id=j27id from p91invoice where p91id=@p91id

select @vatisok=dbo.p91_test_vat(@newvatrate,@j27id,@j17id,@p91DateSupply)

if @vatisok=0
  set @err_ret='Sazba DPH ['+convert(varchar(10),@newvatrate)+'%] není platná pro dané datum plnìní, mìnu a zemi!'
-----------------------------------------

if @err_ret<>''
  return

update p91Invoice SET x15ID=@x15id,p91FixedVatRate=@newvatrate WHERE p91ID=@p91id

update p31worksheet set p31vatrate_approved=@newvatrate,p31vatrate_invoiced=@newvatrate
where p91id=@p91id

update p31worksheet set p31amount_vat_approved=p31amount_withoutvat_approved*p31vatrate_approved/100
where p91id=@p91id

update p31worksheet set p31amount_withvat_approved=p31amount_withoutvat_approved+p31amount_vat_approved
where p91id=@p91id

update p31worksheet set p31amount_vat_invoiced=p31amount_withoutvat_invoiced*p31vatrate_invoiced/100
where p91id=@p91id

update p31worksheet set p31amount_withvat_invoiced=p31amount_withoutvat_invoiced+p31amount_vat_invoiced
where p91id=@p91id

exec p91_recalc_amount @p91id
































GO