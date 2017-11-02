if exists (select 1 from sysobjects where  id = object_id('p91_convertdraft') and type = 'P')
 drop procedure p91_convertdraft
GO






CREATE    PROCEDURE [dbo].[p91_convertdraft]
@p91id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---konverze dokladu DRAFT->OSTRÁ faktura
set @err_ret=''

declare @p91code varchar(50),@x38id int,@p91isdraft bit

select @x38id=p92.x38ID,@p91isdraft=a.p91IsDraft
FROM
p91Invoice a INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
WHERE a.p91ID=@p91id

if @p91isdraft=0
 begin
  set @err_ret='Faktura není v àežimu DRAFT.'
  return	--faktura není v draftu
 end
 
set @p91code=dbo.x38_get_freecode(@x38id,391,@p91id,1)

if @p91code<>''
   UPDATE p91Invoice SET p91Code=@p91code,p91IsDraft=0 WHERE p91ID=@p91id 

  





GO