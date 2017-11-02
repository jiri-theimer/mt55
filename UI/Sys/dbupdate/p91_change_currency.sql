if exists (select 1 from sysobjects where  id = object_id('p91_change_currency') and type = 'P')
 drop procedure p91_change_currency
GO






CREATE     procedure [dbo].[p91_change_currency]
@p91id int
,@j03id_sys int
,@j27id int
,@err_ret varchar(1000) OUTPUT

AS

set @p91id=isnull(@p91id,0)

declare @login nvarchar(50)

set @login=dbo.j03_getlogin(@j03id_sys)

set @err_ret=''

if @j27id is null or @p91id is null
  set @err_ret='@j27id or @p91id is missing!'

if @j03id_sys=0
 set @err_ret='@j03id_sys is missing!'


if @err_ret<>''
  return


update p91Invoice set j27ID=@j27id,p91UserUpdate=@login,p91DateUpdate=getdate() WHERE p91ID=@p91id

exec p91_recalc_amount @p91id

































GO