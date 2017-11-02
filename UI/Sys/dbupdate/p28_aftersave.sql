if exists (select 1 from sysobjects where  id = object_id('p28_aftersave') and type = 'P')
 drop procedure p28_aftersave
GO




CREATE    PROCEDURE [dbo].[p28_aftersave]
@p28id int
,@j03id_sys int

AS

declare @p28code varchar(10),@p29id int,@x38id int,@p28name nvarchar(255),@iscompany bit,@p51id_billing int
declare @p28companyname nvarchar(255),@p28companyshortname nvarchar(50)

select @p28code=p28code,@p29id=a.p29id,@iscompany=p28IsCompany,@p51id_billing=a.p51ID_Billing
,@p28companyname=p28CompanyName,@p28companyshortname=p28CompanyShortName,@x38id=p29.x38ID
from p28contact a LEFT OUTER JOIN p29ContactType p29 ON a.p29ID=p29.p29ID
where a.p28ID=@p28id

if @iscompany=1
 begin
  if @p28companyshortname is null
   set @p28name=@p28companyname
  else
   set @p28name=@p28companyshortname
 end
else
 begin
  select @p28name=replace(ISNULL(p28LastName,'')+' '+isnull(p28FirstName,'')+' '+isnull(p28TitleBeforeName,''),'  ',' ')
  from p28Contact where p28ID=@p28id
 end
 
set @p28name=RTRIM(@p28name)
set @p28name=LTRIM(@p28name)

update p28Contact set p28Name=@p28name where p28ID=@p28id 


if left(@p28code,4)='TEMP' OR @p28code is null
 begin
  set @p28code=dbo.x38_get_freecode(@x38id,328,@p28id,1)
  if @p28code<>''
   UPDATE p28Contact SET p28Code=@p28code WHERE p28ID=@p28id 
 end 

if @p51id_billing is not null	---aktualizace názvu pøípadného ceníku sazeb, který je nastaven na míru pro daný projekt
 begin
   
   if exists(select p51ID FROM p51PriceList WHERE p51IsCustomTailor=1 and p51ID=@p51id_billing)
    update p51PriceList set p51Name=@p28name WHERE p51ID=@p51id_billing

 end

exec [x90_appendlog] 328,@p28id,@j03id_sys
 
 




GO