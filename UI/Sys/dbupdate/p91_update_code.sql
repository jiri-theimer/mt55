if exists (select 1 from sysobjects where  id = object_id('p91_update_code') and type = 'P')
 drop procedure p91_update_code
GO




CREATE    PROCEDURE [dbo].[p91_update_code]
@p91id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu projektu
declare @p91code varchar(50),@x38id int,@isdraft bit

select @p91code=p91Code,@x38id=p92.x38ID,@isdraft=a.p91IsDraft
FROM
p91Invoice a INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
WHERE a.p91ID=@p91id


if @isdraft=1
 select @x38id=x38ID FROM x38CodeLogic WHERE x29ID=391 AND x38IsDraft=1


if left(@p91code,4)='TEMP' OR @p91code is null
 begin


  set @p91code=dbo.x38_get_freecode(@x38id,391,@p91id,1)
  if @p91code<>''
   UPDATE p91Invoice SET p91Code=@p91code WHERE p91ID=@p91id 

  
 end 







GO