if exists (select 1 from sysobjects where  id = object_id('p41_aftersave') and type = 'P')
 drop procedure p41_aftersave
GO




CREATE    PROCEDURE [dbo].[p41_aftersave]
@p41id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu projektu
declare @p41code varchar(50),@x38id int,@p51id_billing int,@name nvarchar(200),@p28id_client int

select @p41code=p41Code,@x38id=p42.x38ID,@p51id_billing=a.p51ID_Billing,@name=a.p41Name,@p28id_client=a.p28ID_Client
FROM
p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID
WHERE a.p41ID=@p41id

if left(@p41code,4)='TEMP' OR @p41code is null
 begin
  set @p41code=dbo.x38_get_freecode(@x38id,141,@p41id,1)
  if @p41code<>''
   UPDATE p41Project SET p41Code=@p41code WHERE p41ID=@p41id 
 end 

if @p51id_billing is not null	---aktualizace názvu pøípadného ceníku sazeb, který je nastaven na míru pro daný projekt
 begin
   if @p28id_client is not null
    select @name=p28Name+' - '+@name FROM p28Contact WHERE p28ID=@p28id_client

   if exists(select p51ID FROM p51PriceList WHERE p51IsCustomTailor=1 and p51ID=@p51id_billing)
    update p51PriceList set p51Name=@name WHERE p51ID=@p51id_billing

 end


exec [x90_appendlog] 141,@p41id,@j03id_sys


GO