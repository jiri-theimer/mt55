if exists (select 1 from sysobjects where  id = object_id('p90_update_code') and type = 'P')
 drop procedure p90_update_code
GO




create    PROCEDURE [dbo].[p90_update_code]
@p90id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu zálohy
declare @p90code varchar(50),@x38id int,@isdraft bit

select @p90code=p90Code,@x38id=p89.x38ID,@isdraft=a.p90IsDraft
FROM
p90Proforma a INNER JOIN p89ProformaType p89 ON a.p89ID=p89.p89ID
WHERE a.p90ID=@p90id


if @isdraft=1
 select @x38id=x38ID FROM x38CodeLogic WHERE x29ID=390 AND x38IsDraft=1


if left(@p90code,4)='TEMP' OR @p90code is null
 begin


  set @p90code=dbo.x38_get_freecode(@x38id,390,@p90id,1)
  if @p90code<>''
   UPDATE p90Proforma SET p90Code=@p90code WHERE p90ID=@p90id 

  
 end 







GO