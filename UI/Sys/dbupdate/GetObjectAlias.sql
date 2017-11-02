if exists (select 1 from sysobjects where  id = object_id('GetObjectAlias') and type = 'FN')
 drop function GetObjectAlias
GO



CREATE FUNCTION [dbo].[GetObjectAlias] (@prefix varchar(10),@pid int)
RETURNS nvarchar(300) AS  
BEGIN 

if @pid is null or @prefix is null
  return null

declare @ret nvarchar(300),@refp28id int,@refp41id int,@refp91id int,@refj02id int,@refpid int,@refx29id int
	
if @prefix='p28'
 select @ret=p28name FROM p28contact where p28id=@pid

if @prefix='p41'
  select @ret=p41Name+isnull(' | '+b.p28name,'') FROM p41Project a LEFT OUTER JOIN p28Contact b on a.p28ID_Client=b.p28ID where a.p41id=@pid

if @prefix='p91'
  select @ret=p91code+isnull(' | '+b.p28name,'') FROM p91invoice a LEFT OUTER JOIN p28Contact b ON a.p28ID=b.p28ID where a.p91id=@pid

if @prefix='p90'
  select @ret=p90code+isnull(' ['+b.p28name+']','') FROM p90Proforma a LEFT OUTER JOIN p28Contact b ON a.p28ID=b.p28ID where a.p90ID=@pid


if @prefix='p31'
  select @ret=p41Name+' ['+p34name+'] '+convert(varchar(20),p31date,104)+' ['+j02LastName+']' FROM p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34activitygroup c on b.p34id=c.p34id inner join j02Person d on a.j02ID=d.j02ID inner join p41Project p41 on a.p41id=p41.p41id where a.p31id=@pid


if @prefix='j03'
  select @ret=j03Login+isnull(' | '+j02LastName+' '+j02Firstname,'') from j03user a LEFT OUTER JOIN j02Person b ON a.j02ID=b.j02ID where a.j03id=@pid 

if @prefix='j02'
  select @ret=j02LastName+' '+j02FirstName FROM j02Person WHERE j02ID=@pid

if @prefix='j23'
  select @ret=j23Name+isnull(' ('+j23code+')','') FROM j23NonPerson WHERE j23ID=@pid

if @prefix='j24'
  select @ret=j24Name FROM j24NonPersonType WHERE j24ID=@pid

if @prefix='o22'
 select @ret=o22Name+' - '+[dbo].GetDDMMYYYYHHMM(o22DateUntil)+' ('+o21Name+')',@refp28id=p28id,@refp41id=p41id,@refp91id=p91id,@refj02id=j02id FROM o22Milestone a inner join o21MilestoneType b ON a.o21ID=b.o21ID WHERE a.o22ID=@pid

 
if @prefix='o23'
 select @ret=case when o23IsBillingMemo=0 then isnull(o23Name,'Poznámka bez názvu') else isnull(o23Name+' (fakt.poznámka)','Fakturaèní poznámka bez názvu') end,@refp28id=p28id,@refp41id=p41id,@refp91id=p91id,@refj02id=j02id FROM o23Notepad WHERE o23ID=@pid
  
if @prefix='b07'
 select @ret='Komentáø od: '+b.j02Firstname+' '+b.j02LastName, @refpid=a.b07RecordPID,@refx29id=a.x29ID FROM b07Comment a INNER JOIN j02Person b ON a.j02ID_Owner=b.j02ID WHERE a.b07ID=@pid
 

if @prefix='p32'
  select @ret=p32Name+' | '+isnull(p34name,'') FROM p32Activity a LEFT OUTER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE a.p32ID=@pid

if @prefix='p34'
  select @ret=p34Name FROM p34ActivityGroup WHERE p34ID=@pid

if @prefix='p51'
  select @ret=p51Name+' ('+j27Code+')' from p51PriceList a INNER JOIN j27Currency b ON a.j27ID=b.j27ID WHERE a.p51ID=@pid

if @prefix='p36'
 select @ret=convert(varchar(20),p36DateFrom,104)+' - '+convert(varchar(20),p36DateUntil,104) from p36LockPeriod WHERE p36ID=@pid

if @prefix='p56'
  select @ret=c.p57Name+' - '+isnull(p56Name,'Úkol')+' ('+p56Code+') | '+isnull(b.p41NameShort,b.p41Name) from p56Task a inner join p41Project b on a.p41ID=b.p41id INNER JOIN p57TaskType c ON a.p57ID=c.p57ID where a.p56id=@pid 

if @prefix='c21'
  select @ret=c21Name FROM c21FondCalendar WHERE c21ID=@pid

if @prefix='c26'
  select @ret=c26Name FROM c26Holiday WHERE c26ID=@pid

if @ret is null
 set @ret='Pro prefix '+@prefix+' objekt nenalezen'


if @refpid is not null
 begin
   if @refx29id=141
    set @refp41id=@refpid

   if @refx29id=328
    set @refp28id=@refpid

   if @refx29id=391
    set @refp91id=@refpid

   if @refj02id=102
    set @refj02id=@refpid
 end

if @refp28id is not null
 select @ret=@ret+' | '+p28name FROM p28Contact WHERE p28ID=@refp28id

if @refp41id is not null
 select @ret=@ret+' | '+p41name+isnull(' ('+b.p28name+')','') FROM p41Project a LEFT OUTER JOIN p28Contact b ON a.p28ID_Client=b.p28ID WHERE a.p41ID=@refp41id

if @refp91id is not null
 select @ret=@ret+' | '+p91Code FROM p91Invoice WHERE p91ID=@refp91id

if @refj02id is not null
 select @ret=@ret+' | '+j02FirstName+' '+j02LastName FROM j02Person WHERE j02ID=@refj02id

RETURN(@ret)


END


































GO