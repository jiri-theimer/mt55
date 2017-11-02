if exists (select 1 from sysobjects where  id = object_id('p91_proforma_save') and type = 'P')
 drop procedure p91_proforma_save
GO




CREATE     procedure [dbo].[p91_proforma_save]
@p91id int
,@p90id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT

AS

set @p91id=isnull(@p91id,0)

set @p90id=isnull(@p90id,0)

declare @login nvarchar(50)

set @login=dbo.j03_getlogin(@j03id_sys)

set @err_ret=''

if @p91id=0
 set @err_ret='@p91id is missing!'

if @p90id=0
 set @err_ret='@p90id is missing!'

if @j03id_sys=0
 set @err_ret='@j03id_sys is missing!'

if exists(select p99ID from p99Invoice_Proforma WHERE p90ID=@p90id)
 set @err_ret='Zálohová faktura je již svázaná s daòovou fakturou!'

if @err_ret<>''
  return

if not exists(select p99ID from p99Invoice_Proforma WHERE p91ID=@p91id AND p90ID=@p90id)
 INSERT INTO p99Invoice_Proforma(p91ID,p90ID,p99UserInsert,p99UserUpdate,p99DateUpdate) values(@p91id,@p90id,@login,@login,getdate())





exec p91_recalc_amount @p91id

































GO