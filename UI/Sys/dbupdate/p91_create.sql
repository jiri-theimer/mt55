if exists (select 1 from sysobjects where  id = object_id('p91_create') and type = 'P')
 drop procedure p91_create
GO






CREATE procedure [dbo].[p91_create]
@guid varchar(50)
,@j03id_sys int
,@p28id int
,@p92id int
,@p91isdraft bit
,@p91date datetime
,@p91datematurity datetime
,@p91datesupply datetime
,@p91datep31_from datetime
,@p91datep31_until datetime
,@p91text1 nvarchar(2000)
,@err_ret varchar(1000) OUTPUT
,@ret_p91id int OUTPUT

AS

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')
set @p28id=isnull(@p28id,0)
set @p92id=isnull(@p92id,0)


if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if @p92id=0
  set @err_ret='Chybí typ faktury!'

if @p28id=0
  set @err_ret='Chybí klient (odbìratel) faktury!'

if @err_ret<>''
 return

declare @login nvarchar(50),@j02id_owner int
set @login=dbo.j03_getlogin(@j03id_sys)
select @j02id_owner=j02ID FROM j03User WHERE j03ID=@j03id_sys


declare @j27id int,@j19id int,@x15id int,@j17id int


select @j27id=j27id,@j19id=j19id,@x15id=x15id,@j17id=j17ID
from p92InvoiceType where p92id=@p92id  

if isnull(@j27id,0)=0
 begin
  if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Invoice')
   select @j27id=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Invoice'
  else
   set @j27id=2
 end

declare @code_temp varchar(50)
set @code_temp='TEMP'+@guid  

insert into p91invoice(p91code,p91dateinsert,p91userinsert,p91Date,p91DateSupply,p91DateMaturity,j02ID_Owner,p28ID,p92ID,j27ID) values(@code_temp,getdate(),@login,@p91date,@p91datesupply,@p91datematurity,@j02id_owner,@p28id,@p92id,@j27id)

SELECT @ret_p91id=@@IDENTITY

	
update p91invoice set p91IsDraft=@p91isdraft,j17ID=@j17id
,p91userupdate=@login,p91dateupdate=getdate()
,p91Text1=@p91text1
,p91Datep31_From=@p91datep31_from,p91Datep31_Until=@p91datep31_until,j19id=@j19id
FROM p91invoice
where p91id=@ret_p91id


declare @o38id_primary int,@o38id_delivery int,@p41id_first int

select top 1 @p41id_first=p41ID FROM p31Worksheet WHERE p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

select top 1 @o38id_primary=o38ID from o37Contact_Address WHERE o36ID=1 AND p28ID=@p28id
   
select top 1 @o38id_delivery=o38ID from o37Contact_Address WHERE o36ID=2 AND p28ID=@p28id

if @o38id_delivery is null
 set @o38id_delivery=@o38id_primary

update p91Invoice set o38ID_Primary=@o38id_primary,@o38id_delivery=o38id_delivery,p41ID_First=@p41id_first
where p91ID=@ret_p91id

 

declare @p91text2 nvarchar(1000)

select @p91text2=p41InvoiceDefaultText2 from p41Project WHERE p41ID=@p41id_first

if @p91text2 is null
 select @p91text2=p28InvoiceDefaultText2 from p28Contact where p28ID=@p28id

if @p91text2 is not null
 update p91Invoice set p91Text2=@p91text2 where p91ID=@ret_p91id

update p31worksheet set p91ID=@ret_p91id,p70id=p72ID_AfterApprove
WHERE p71id=1 and isnull(p72ID_AfterApprove,0)<>0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p91ID=@ret_p91id,p70id=(case when isnull(p31amount_withoutvat_approved,0)<>0 then 4 else 2 end)
WHERE p71id=1 and isnull(p72ID_AfterApprove,0)=0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p31Minutes_Invoiced=p31Minutes_Approved_Billing,p31Hours_Invoiced=p31Hours_Approved_Billing,p31HHMM_Invoiced=p31HHMM_Approved_Billing
,p31Value_Invoiced=p31Value_Approved_Billing
,p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved,p31VatRate_Invoiced=p31VatRate_Approved
,p31Rate_Billing_Invoiced=p31Rate_Billing_Approved
where p91ID=@ret_p91id



exec p91_recalc_amount @ret_p91id

if isnull(@x15id,0)<>0
 begin	---faktura má být kompletnì pøevedena do jednotné DPH
  declare @explicit_vatrate float,@j18id int

  
  set @explicit_vatrate=dbo.p91_get_vatrate(@x15id,@j27id,@j17id,@p91datesupply)
  
  
  exec p91_change_vat @ret_p91id,@j03id_sys,@x15id,@explicit_vatrate,null
  
 end



 exec [p91_update_code] @ret_p91id,@j03id_sys

 exec [p91_aftersave] @ret_p91id,@j03id_sys,0





























GO