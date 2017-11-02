if exists (select 1 from sysobjects where  id = object_id('p56_aftersave') and type = 'P')
 drop procedure p56_aftersave
GO





CREATE    PROCEDURE [dbo].[p56_aftersave]
@p56id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu projektu
declare @p56code varchar(50),@x38id int

select @p56code=p56Code,@x38id=p57.x38ID
FROM
p56Task a INNER JOIN p57TaskType p57 ON a.p57ID=p57.p57ID
WHERE a.p56ID=@p56id

if left(@p56code,4)='TEMP' OR @p56code is null
 begin
  set @p56code=dbo.x38_get_freecode(@x38id,356,@p56id,1)
  if @p56code<>''
   UPDATE p56Task SET p56Code=@p56code WHERE p56ID=@p56id 
 end 


declare @j02id int
select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys

exec j03_recovery_cache @j03id_sys,@j02id




GO