if exists (select 1 from sysobjects where  id = object_id('x38_get_freecode') and type = 'FN')
 drop function x38_get_freecode
GO





CREATE FUNCTION [dbo].[x38_get_freecode](@x38id int,@x29id int,@datapid int,@attempt_number int)
RETURNS varchar(50) AS  
BEGIN 

declare @mask varchar(200),@x38ExplicitIncrementStart int,@code_new varchar(50),@val int,@code_max_used varchar(50)
declare @x38Scale int,@x38ConstantBeforeValue varchar(50),@pid_last int,@x38isdraft bit,@x38ConstantAfterValue varchar(40)

set @val=0

if @x38id is null and @x29id is not null
 select top 1 @x38id=x38ID FROM x38CodeLogic WHERE x29ID=@x29id AND getdate() between x38ValidFrom AND x38ValidUntil

if @x38id is null
 RETURN('')


select @mask=x38MaskSyntax,@x29id=x29ID,@x38ExplicitIncrementStart=isnull(x38ExplicitIncrementStart,0)
,@x38ConstantBeforeValue=isnull(x38ConstantBeforeValue,''),@x38Scale=x38Scale,@x38isdraft=x38IsDraft
,@x38ConstantAfterValue=isnull(x38ConstantAfterValue,'')
FROM x38CodeLogic
WHERE x38ID=@x38id

if isnull(@x38Scale,0)=0
 set @x38Scale=4

if @mask is null
 begin			---kód se generuje automatikou bez explicitní masky
  if @x29id=141	---projekt
   select @pid_last=max(p41ID),@code_max_used=max(dbo.remove_alphacharacters(p41code,@x38Scale)) FROM p41Project where p41Code NOT LIKE 'TEMP%' AND p42ID IN (SELECT p42ID FROM p42ProjectType WHERE x38ID=@x38id or x38ID IS NULL)

  if @x29id=328
   begin
     declare @p29id int
	 select @p29id=p29ID FROM p28Contact WHERE p28ID=@datapid

	 if @p29id is not null
	  select @pid_last=max(p28ID),@code_max_used=max(dbo.remove_alphacharacters(p28code,@x38Scale)) FROM p28Contact where p28Code NOT LIKE 'TEMP%' AND p29ID IN (SELECT p29ID FROM p29ContactType WHERE x38ID=@x38id or x38ID IS NULL)
	 else
	  select @pid_last=max(p28ID),@code_max_used=max(dbo.remove_alphacharacters(p28code,@x38Scale)) FROM p28Contact where p28Code NOT LIKE 'TEMP%'
   end
   

  if @x29id=356	---úkol
   select @pid_last=max(p56ID),@code_max_used=max(dbo.remove_alphacharacters(p56code,@x38Scale)) FROM p56Task where p56Code NOT LIKE 'TEMP%' AND p57ID IN (SELECT p57ID FROM p57TaskType WHERE x38ID=@x38id or x38ID IS NULL)

  if @x29id=391	---faktura
   begin
    if @x38isdraft=0
     select @pid_last=max(p91ID),@code_max_used=max(dbo.remove_alphacharacters(p91code,@x38Scale)) FROM p91Invoice where p91Code NOT LIKE 'TEMP%' AND p92ID IN (SELECT p92ID FROM p92InvoiceType WHERE x38ID=@x38id or x38ID IS NULL)

	if @x38isdraft=1
	 select @pid_last=max(p91ID),@code_max_used=max(dbo.remove_alphacharacters(p91code,@x38Scale)) FROM p91Invoice where p91Code NOT LIKE 'TEMP%' AND p91IsDraft=1
   end

  if @x29id=390	---záloha
   begin
    if @x38isdraft=0
     select @pid_last=max(p90ID),@code_max_used=max(dbo.remove_alphacharacters(p90code,@x38Scale)) FROM p90Proforma where p90Code NOT LIKE 'TEMP%' AND p89ID IN (SELECT p89ID FROM p89ProformaType WHERE x38ID=@x38id or x38ID IS NULL)

	 if @x38isdraft=1
	  select @pid_last=max(p90ID),@code_max_used=max(dbo.remove_alphacharacters(p90code,@x38Scale)) FROM p90Proforma where p90Code NOT LIKE 'TEMP%' AND p90IsDraft=1

   end
 
  if @code_max_used is not null
   begin    
	 
    if ISNUMERIC(@code_max_used)=1
	 set @val=convert(int,@code_max_used)
   end

  set @val=@val+1		---nový kód bude o jednièku vìtší


  if @val<@x38ExplicitIncrementStart
   set @val=@x38ExplicitIncrementStart

  set @code_new=@x38ConstantBeforeValue+right('0000000000'+convert(varchar(10),@val),@x38Scale)+@x38ConstantAfterValue
  
 end


if @x29id=141 and @code_new<>''	---projekt
begin
	if exists(select p41ID FROM p41Project WHERE p41Code LIKE @code_new)
	set @code_new=''	---v tabulce již je uložená hodnota klíèe @code_new
end

if @x29id=356 and @code_new<>''	---úkol
begin
	if exists(select p56ID FROM p56Task WHERE p56Code LIKE @code_new)
	set @code_new=''
end

if @x29id=328 and @code_new<>''	---kontakt
begin
 return(@code_new)
 if exists(select p28ID FROM p28Contact WHERE p28Code LIKE @code_new)
  set @code_new=''
end

if @x29id=391 and @code_new<>''	---faktura
begin
 if exists(select p91ID FROM p91Invoice WHERE p91Code LIKE @code_new)
  set @code_new=''
end

if @x29id=390 and @code_new<>''	---záloha
begin
 if exists(select p90ID FROM p90Proforma WHERE p90Code LIKE @code_new)
  set @code_new=''
end


if @code_new='' and @attempt_number<=1
begin
 set @code_new=dbo.x38_get_freecode(@x38id,@x29id,@datapid,2)	---druhý pokus, když se nepodaøilo zjistit hodnotu kódu napoprvé

end


 RETURN(@code_new)

END


















GO