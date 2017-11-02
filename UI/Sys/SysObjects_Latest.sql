----------FN---------------convert_to_dateserial-------------------------

if exists (select 1 from sysobjects where  id = object_id('convert_to_dateserial') and type = 'FN')
 drop function convert_to_dateserial
GO




CREATE function [dbo].[convert_to_dateserial](@dat datetime)
RETURNS datetime
AS
BEGIN
	declare @year int,@month int,@day int

	set @year=year(@dat)
	set @month=month(@dat)
	set @day=day(@dat)

	return dbo.get_dateserial(@year,@month,@day,0,0)
END



GO

----------FN---------------get_datename-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_datename') and type = 'FN')
 drop function get_datename
GO







create FUNCTION [dbo].[get_datename] (@d datetime,@langindex int)
--@langindex=0 -> èesky, 1 -> anglicky
RETURNS varchar (3) AS  
BEGIN 

	
	declare @s varchar(3)
	set @s=left(DATENAME(weekday,@d),3)
	
	
	if @langindex=0
	  begin
		if @s='Mon'
		  set @s='Po'
		if @s='Tue'
		  set @s='Ut'
		if @s='Wed'
		  set @s='St'
		if @s='Thu'
		  set @s='Ct'
		if @s='Fri'
		  set @s='Pa'
		if @s='Sat'
		  set @s='So'
		if @s='Sun'
		  set @s='Ne'
	  end

	RETURN(@s)

	
END













GO

----------FN---------------get_dateserial-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_dateserial') and type = 'FN')
 drop function get_dateserial
GO




CREATE FUNCTION [dbo].[get_dateserial](
    @year int,
    @month int,
    @day int,
    @hour int,
    @minute int
	)
RETURNS datetime2(7)
AS
BEGIN
    RETURN
        DATEADD(MINUTE, @minute, 
        DATEADD(HOUR, @hour, 
        DATEADD(DAY, @day-1, 
        DATEADD(MONTH, @month-1, 
        DATEADD(YEAR, @year-1900, 
        CAST(CAST(0 AS datetime) AS datetime2(7)))))));
END



GO

----------FN---------------get_exchange_rate-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_exchange_rate') and type = 'FN')
 drop function get_exchange_rate
GO





CREATE FUNCTION [dbo].[get_exchange_rate](@ratetype int,@dat datetime,@j27id_source int,@j27id_dest int)
RETURNS float
AS
BEGIN
  set @ratetype=isnull(@ratetype,1)		---@ratetype=1 - fakturaèní kurz, @ratetype=2 - fixní kurz

  if @j27id_source is null or @j27id_dest is null
    return(1)

  if @j27id_source=@j27id_dest
    RETURN(1)

  declare @j27id_domestic int	--domácí mìna, která definuje mìnové kurzy vùèi ostatním mìnám, výchozí je CZK

  if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Domestic')
   select @j27id_domestic=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Domestic'
  else
   set @j27id_domestic=2

   
  declare @ret float
  


 if @j27id_source<>@j27id_domestic
  begin
  	select TOP 1 @ret=m62rate/m62units
  	FROM m62ExchangeRate
  	WHERE j27id_slave=@j27id_source and m62date<=@dat and j27id_master=@j27id_domestic AND m62RateType=@ratetype
  	ORDER BY m62date desc

  end
else
  begin
	select TOP 1 @ret=(1/m62rate)*m62units
  	FROM m62ExchangeRate
  	WHERE j27id_slave=@j27id_dest and m62date<=@dat and j27id_master=@j27id_domestic AND m62RateType=@ratetype
  	ORDER BY m62date desc

  end


  set @ret=isnull(@ret,1)

  if @j27id_dest=@j27id_domestic or @j27id_source=@j27id_domestic
    RETURN(@ret)	--pøevod do nebo z domácí mìny



  declare @ret2 float
  select @ret2=dbo.get_exchange_rate(@ratetype,@dat,@j27id_dest,@j27id_domestic)
  
  set @ret=@ret/@ret2
  
  

  RETURN(@ret)

END



































GO

----------FN---------------get_hhmm_to_minutes-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_hhmm_to_minutes') and type = 'FN')
 drop function get_hhmm_to_minutes
GO






CREATE  FUNCTION [dbo].[get_hhmm_to_minutes] (@hhmm varchar(50))
RETURNS int AS  
BEGIN 
	declare @i int

	set @hhmm=replace(@hhmm,' ','')

	set @i=PATINDEX('%:%',@hhmm)
	
	if @i<=0
	  begin
	     if isnumeric(@hhmm)=1
	 	return(60*convert(int,@hhmm))
	     else
	        return(0)
	  end

	declare @hours int,@minutes int

	if isnumeric(left(@hhmm,@i-1))=1
	  set @hours=left(@hhmm,@i-1)
	else
	  set @hours=0
       

	if isnumeric(right(@hhmm,len(@hhmm)-@i))=1
	  set @minutes=right(@hhmm,len(@hhmm)-@i)
	else
	  set @minutes=0

	
	

	RETURN(@hours*60+@minutes)
END










GO

----------FN---------------get_hours_to_hhmm-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_hours_to_hhmm') and type = 'FN')
 drop function get_hours_to_hhmm
GO







CREATE   FUNCTION [dbo].[get_hours_to_hhmm] (@hours float)
RETURNS varchar (8) AS  
BEGIN 
	declare @bitNegative bit

	if @hours<0
	 begin
	  set @bitNegative=1
 	  set @hours=@hours*-1
	 end
	else
	 set @bitNegative=0

	declare @decMinutes decimal(11,2)
	set @decMinutes=@hours*60

	declare @str varchar(8)
	declare @intHours int
	declare @intMinutes int

	

	set @intHours=convert(int,@decMinutes/60)
	set @intMinutes=convert(int,@decMinutes)-@intHours*60
	
	if @intHours<10
		begin
			set @str=right('0'+convert(varchar(2),@intHours),2)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end
	else
		begin
			set @str=convert(varchar(4),@intHours)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end

	if @bitNegative=1
	  set @str='-'+@str

	RETURN(@str)
END







GO

----------FN---------------GetDDMMYYYYHHMM-------------------------

if exists (select 1 from sysobjects where  id = object_id('GetDDMMYYYYHHMM') and type = 'FN')
 drop function GetDDMMYYYYHHMM
GO






create   FUNCTION [dbo].[GetDDMMYYYYHHMM] (@d datetime)
RETURNS varchar (30) AS  
BEGIN 
	declare @s varchar(30)

	set @s=convert(varchar(10),@d,104)+' '+right('0'+convert(varchar(10),datepart(hour,@d)),2)+':'+right('0'+convert(varchar(10),datepart(minute,@d)),2)

	RETURN(@s)
END







GO

----------FN---------------getfieldsize-------------------------

if exists (select 1 from sysobjects where  id = object_id('getfieldsize') and type = 'FN')
 drop function getfieldsize
GO








CREATE FUNCTION [dbo].[getfieldsize] (@field varchar(50),@table varchar(50))
RETURNS int AS  
BEGIN 

---vrací maximální délku pole @field v tabulce @table
	
declare @id int

select @id=[id]
from sysobjects
where [name] like @table

if isnull(@id,0)=0
  return(0)

declare @size int,@type int,@xtype int

select @size=[length],@type=[type],@xtype=[xtype]
from
dbo.syscolumns
where [name] like @field and [id]=@id

if @xtype=231
  set @size=@size/2	--nvarchar typ dìlíme 2

if @type=35
  return(0)	--memo


RETURN(isnull(@size,0))


END








GO

----------FN---------------GetObjectAlias-------------------------

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

----------FN---------------Hours2HHMM-------------------------

if exists (select 1 from sysobjects where  id = object_id('Hours2HHMM') and type = 'FN')
 drop function Hours2HHMM
GO







CREATE   FUNCTION [dbo].[Hours2HHMM] (@hours float)
RETURNS varchar (8) AS  
BEGIN 
	declare @bitNegative bit

	if @hours<0
	 begin
	  set @bitNegative=1
 	  set @hours=@hours*-1
	 end
	else
	 set @bitNegative=0

	declare @decMinutes decimal(11,2)
	set @decMinutes=@hours*60

	declare @str varchar(8)
	declare @intHours int
	declare @intMinutes int

	

	set @intHours=convert(int,@decMinutes/60)
	set @intMinutes=convert(int,@decMinutes)-@intHours*60
	
	if @intHours<10
		begin
			set @str=right('0'+convert(varchar(2),@intHours),2)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end
	else
		begin
			set @str=convert(varchar(4),@intHours)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end

	if @bitNegative=1
	  set @str='-'+@str

	RETURN(@str)
END







GO

----------FN---------------j02_teams_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_teams_inline') and type = 'FN')
 drop function j02_teams_inline
GO




CREATE    FUNCTION [dbo].[j02_teams_inline](@j02id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené názvy týmù, v kterých je osoba @j02id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+b.j11Name
  FROM j12Team_Person a INNER JOIN j11Team b ON a.j11ID=b.j11ID
  WHERE a.j02ID=@j02id AND b.j11IsAllPersons=0
  ORDER BY j11Name


RETURN(@s)
   
END




GO

----------FN---------------j03_getj02id-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03_getj02id') and type = 'FN')
 drop function j03_getj02id
GO










CREATE    FUNCTION [dbo].[j03_getj02id](@j03id int)
RETURNS int
AS
BEGIN
  ---vrací j02ID uživatele @j03id

 

  RETURN(select j02ID FROM j03user where j03id=@j03id)
   
END























GO

----------FN---------------j03_getlogin-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03_getlogin') and type = 'FN')
 drop function j03_getlogin
GO










CREATE    FUNCTION [dbo].[j03_getlogin](@j03id int)
RETURNS nvarchar(50)
AS
BEGIN
  ---vrací login uživatele @j03id

 

  RETURN(select j03login FROM j03user where j03id=@j03id)
   
END






















GO

----------FN---------------j03_test_permission_global-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03_test_permission_global') and type = 'FN')
 drop function j03_test_permission_global
GO











CREATE    FUNCTION [dbo].[j03_test_permission_global](@j03id int,@x53value int)
RETURNS BIT
AS
BEGIN
  ---vrací 1, pokud uživatel @j03id disponuje oprávnìním v jeho globální roli (j04)
  declare @ret bit,@rolevalue varchar(50)
  set @ret=0

  select @rolevalue=x67.x67RoleValue
  FROM j03User a INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID INNER JOIN x67EntityRole x67 ON j04.x67ID=x67.x67ID
  WHERE a.j03ID=@j03id

  if SUBSTRING(@rolevalue,@x53value,1)='1'
   set @ret=1
  
 
  
  RETURN(@ret)
   
END























GO

----------FN---------------j05_slaves_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('j05_slaves_inline') and type = 'FN')
 drop function j05_slaves_inline
GO




CREATE    FUNCTION [dbo].[j05_slaves_inline](@j02id_master int)
RETURNS varchar(6000)
AS
BEGIN
  ---vrací èárkou oddìlené názvy podøízených osob a týmù

 DECLARE @s varchar(6000)

select @s=COALESCE(@s + ', ', '')+a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')
  FROM j02Person a INNER JOIN j05MasterSlave b ON a.j02ID=b.j02ID_Slave
  WHERE b.j02ID_Master=@j02id_master
  ORDER BY a.j02LastName


RETURN(@s)
   
END





GO

----------FN---------------Minutes2HHMM-------------------------

if exists (select 1 from sysobjects where  id = object_id('Minutes2HHMM') and type = 'FN')
 drop function Minutes2HHMM
GO










CREATE FUNCTION [dbo].[Minutes2HHMM] (@decMinutes decimal (11,2))
RETURNS varchar (7) AS  
BEGIN 
	declare @str varchar(7)
	declare @intHours int
	declare @intMinutes int

	set @intHours=convert(int,@decMinutes/60)
	set @intMinutes=convert(int,@decMinutes)-@intHours*60
	
	if @intHours<10
		begin
			set @str=right('0'+convert(varchar(2),@intHours),2)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end
	else
		begin
			set @str=convert(varchar(4),@intHours)+':'+right('0'+convert(varchar(2),@intMinutes),2)
		end

	RETURN(@str)
END








GO

----------FN---------------o28_get_permflag-------------------------

if exists (select 1 from sysobjects where  id = object_id('o28_get_permflag') and type = 'FN')
 drop function o28_get_permflag
GO






CREATE  FUNCTION [dbo].[o28_get_permflag](@j02id int,@p41id int,@j18id int,@p34id int,@minpermflag int,@maxpermflag int)
RETURNS int
AS
BEGIN
   
declare @o28id int,@o28permflag int	---0-pouze vlastní worksheet,1-Èíst vše v rámci projektu, 2-Èíst a upravovat vše v rámci projektu,3-Èíst a schvalovat vše v rámci projektu 4 - Èíst, upravovat a schvalovat vše v rámci projektu

select TOP 1 @o28id=a.o28id,@o28permflag=a.o28PermFlag
from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
where a.p34ID=@p34id AND x69.x69RecordPID=@p41id AND x67.x29ID=141
and (isnull(x69.j02ID,0)=@j02id OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))
AND a.o28entryflag>=@minpermflag AND a.o28PermFlag<=@maxpermflag
ORDER BY a.o28PermFlag DESC
	
if @o28id is null and @j18id is not null
	begin ----------oprávnìní k projektu podle projektové skupiny (regionu)
		select TOP 1 @o28id=a.o28id,@o28permflag=a.o28PermFlag
		from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
		inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
		where a.p34ID=@p34id AND x69.x69RecordPID=@j18id AND x67.x29ID=118
		and (isnull(x69.j02ID,0)=@j02id OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))
		AND a.o28entryflag>=@minpermflag AND a.o28PermFlag<=@maxpermflag
		ORDER BY a.o28PermFlag DESC
  
	end


RETURN isnull(@o28permflag,0)

END






GO

----------FN---------------p28_addresses_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_addresses_inline') and type = 'FN')
 drop function p28_addresses_inline
GO




CREATE    FUNCTION [dbo].[p28_addresses_inline](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené adresy z profilu kontaktu @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+c.o36Name+' ['+isnull(o38street+', ','')+isnull(o38city+', ','')+isnull(o38zip,'')+']'
from
o38Address a INNER JOIN o37Contact_Address b ON a.o38ID=b.o38ID
INNER JOIN o36AddressType c ON b.o36ID=c.o36ID
WHERE b.p28ID=@p28id


RETURN(@s)
   
END



GO

----------FN---------------p28_medias_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_medias_inline') and type = 'FN')
 drop function p28_medias_inline
GO




CREATE    FUNCTION [dbo].[p28_medias_inline](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené kontaktní média z profilu kontaktu @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+'['+b.o33Name+'] '+isnull(a.o32Value,'')+isnull(' ('+a.o32description+')','')
from
o32Contact_Medium a INNER JOIN o33MediumType b ON a.o33ID=b.o33ID
WHERE a.p28ID=@p28id

RETURN(@s)
   
END



GO

----------FN---------------p31_testvat-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_testvat') and type = 'FN')
 drop function p31_testvat
GO






CREATE   FUNCTION [dbo].[p31_testvat](@vatrate float,@p41id int,@dat datetime,@j27id_explicit int)
RETURNS bit
AS
BEGIN
  if @j27id_explicit=0
   set @j27id_explicit=null

  declare @ret bit,@p53id int,@j18id int,@p51id int,@j17id int

  select @j18id=j18id,@p51id=p51ID_Billing from p41project where p41id=@p41id

  if @j18id is not null
   select @j17id=j17ID FROM j18Region WHERE j18ID=@j18id

  if @p51id is not null and @j27id_explicit is null
   select @j27id_explicit=j27ID FROM p51PriceList WHERE p51ID=@p51id
  
  if @j27id_explicit is null
   select @j27id_explicit=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic' AND ISNUMERIC(x35Value)=1


  if @j17id is not null
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id_explicit AND p53validfrom<=@dat AND p53validuntil>=@dat AND (j17ID=@j17id or j17ID is null) and p53value=@vatrate
  else
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id_explicit AND p53validfrom<=@dat AND p53validuntil>=@dat AND p53value=@vatrate

  if @p53id is null
    set @ret=0
  else
    set @ret=1



  RETURN(@ret)
   
END





GO

----------FN---------------p32_get_vatrate-------------------------

if exists (select 1 from sysobjects where  id = object_id('p32_get_vatrate') and type = 'FN')
 drop function p32_get_vatrate
GO




CREATE  FUNCTION [dbo].[p32_get_vatrate](@p32id int,@p41id int,@dat datetime)
RETURNS float
AS
BEGIN
   
  declare @ret float,@x15id int,@j18id int,@p53id int,@p51id int,@j27id int,@j17id int

  if ISNULL(@p41id,0)<>0
   begin
    select @j18id=j18id,@p51id=p51ID_Billing from p41project where p41id=@p41id

	if @p51id is not null
	 select @j27id=j27ID FROM p51PriceList WHERE p51ID=@p51id

	if @j18id is not null
	 select @j17id=j17ID FROM j18Region WHERE j18ID=@j18id

   end

  if @j27id is null
   select @j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic' AND ISNUMERIC(x35Value)=1

  if exists(select p32id from p32Activity where p32ID=@p32id and x15ID is not null)
    select @x15id=isnull(x15ID,@x15id) from p32activity where p32id=@p32id

  if @x15id is null
   select @x15id=convert(int,x35Value) from x35GlobalParam where x35Key LIKE 'x15ID'
  
  set @x15id=isnull(@x15id,3)

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID=@j17id and x15id=@x15id and p53validfrom<=@dat AND p53validuntil>=@dat ORDER BY p53ValidFrom DESC
  

  if @p53id is null
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID is null and x15id=@x15id and p53validfrom<=@dat AND p53validuntil>=@dat ORDER BY p53ValidFrom DESC

  
  if @p53id is not null
    RETURN(@ret)
    
  if @ret is null
   set @ret=0

  RETURN(@ret)
   
END





GO

----------FN---------------p41_getroles_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_getroles_inline') and type = 'FN')
 drop function p41_getroles_inline
GO












CREATE    FUNCTION [dbo].[p41_getroles_inline](@p41id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené obsazení projektových rolí v projektu @p41id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))+' ('+x67Name+')'
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p41id AND x67.x29ID=141
  ORDER BY x67Ordinary


RETURN(@s)
   
END



























GO

----------FN---------------p56_getroles_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_getroles_inline') and type = 'FN')
 drop function p56_getroles_inline
GO







CREATE    FUNCTION [dbo].[p56_getroles_inline](@p56id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené obsazení  rolí v úkolu @p56id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))+' ('+x67Name+')'
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p56id AND x67.x29ID=356
  ORDER BY x67Ordinary


RETURN(@s)
   
END




























GO

----------FN---------------p91_get_p86id-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_get_p86id') and type = 'FN')
 drop function p91_get_p86id
GO








CREATE  FUNCTION [dbo].[p91_get_p86id](@p91id int)
RETURNS int
AS
BEGIN
  ---vrací ID bankovního úètu pro fakturu @p91id

  declare @p86id int,@j27id int,@p93id int

  select @j27id=a.j27ID,@p93id=b.p93ID
  FROM p91Invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
  WHERE a.p91ID=@p91id

  RETURN(select p86ID FROM p88InvoiceHeader_BankAccount WHERE j27ID=@j27id AND p93ID=@p93id)

 
   
END




GO

----------FN---------------p91_get_vatrate-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_get_vatrate') and type = 'FN')
 drop function p91_get_vatrate
GO






CREATE  FUNCTION [dbo].[p91_get_vatrate](@x15id int,@j27id int,@j17id int,@dat datetime)
RETURNS float
AS
BEGIN
  ---2: snížená, 3: standardní, 4: special

  declare @ret float,@p53id int

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID=@j17id and x15id=@x15id and @dat BETWEEN p53validfrom AND p53validuntil ORDER BY p53ValidFrom DESC
  

  if @p53id is null
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID is null and x15id=@x15id and @dat BETWEEN p53validfrom AND p53validuntil ORDER BY p53ValidFrom DESC

  
  if @p53id is not null
    RETURN(@ret)
    
  
  RETURN(0)
   
END



GO

----------FN---------------p91_test_vat-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_test_vat') and type = 'FN')
 drop function p91_test_vat
GO






CREATE  FUNCTION [dbo].[p91_test_vat](@vatrate float,@j27id int,@j17id int,@dat datetime)
RETURNS bit
AS
BEGIN
  ---2: snížená, 3: standardní, 4: special

  declare @p53id int

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id AND j17ID=@j17id and p53Value=@vatrate and @dat BETWEEN p53validfrom AND p53validuntil
  

  if @p53id is null
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id AND j17ID is null and p53Value=@vatrate and @dat BETWEEN p53validfrom AND p53validuntil

  
  if @p53id is null
    RETURN(0)
  

  RETURN(1)


   
   
END



GO

----------FN---------------parse_errinfo-------------------------

if exists (select 1 from sysobjects where  id = object_id('parse_errinfo') and type = 'FN')
 drop function parse_errinfo
GO





CREATE    FUNCTION [dbo].[parse_errinfo](@ERROR_PROCEDURE nvarchar(500),@ERROR_LINE int,@ERROR_MESSAGE nvarchar(3000))
RETURNS nvarchar(4000)
AS
BEGIN
  ---vrací login uživatele @j03id
 declare @s nvarchar(4000)
 set @s='Procedure: '+@ERROR_PROCEDURE+char(13)+char(10)+'<hr>Line: '+convert(varchar(10),@ERROR_LINE)+char(13)+char(10)+'<hr>'+@ERROR_MESSAGE

 return(@s)
   
END

























GO

----------FN---------------remove_alphacharacters-------------------------

if exists (select 1 from sysobjects where  id = object_id('remove_alphacharacters') and type = 'FN')
 drop function remove_alphacharacters
GO




CREATE FUNCTION [dbo].[remove_alphacharacters](@InputString VARCHAR(1000),@leading_zeros_scale int)
RETURNS VARCHAR(1000)
AS
BEGIN
  WHILE PATINDEX('%[^0-9]%',@InputString)>0
        SET @InputString = STUFF(@InputString,PATINDEX('%[^0-9]%',@InputString),1,'')  		  
		
    
  
  IF @leading_zeros_scale>0
   begin
    
	 RETURN right('00000000000000000000000'+@InputString,@leading_zeros_scale)
   end

   RETURN @InputString
END




GO

----------FN---------------x28_getFirstUsableField-------------------------

if exists (select 1 from sysobjects where  id = object_id('x28_getFirstUsableField') and type = 'FN')
 drop function x28_getFirstUsableField
GO






CREATE FUNCTION [dbo].[x28_getFirstUsableField] (@entityprefix varchar(50),@x28datatype varchar(50))
RETURNS varchar(50) AS  
BEGIN 
	
declare @id int,@table varchar(50),@mask varchar(50),@ret varchar(50),@x29id int

select @x29id=x29id,@table=x29TableName+'_FreeField' from x29Entity where left(x29TableName,3)=@entityprefix

if @x28datatype='string'
  set @mask=@entityprefix+'freetext%'

if @x28datatype='boolean'
  set @mask=@entityprefix+'freeboolean%'

if @x28datatype='date' or @x28datatype='datetime' or @x28datatype='time'
  set @mask=@entityprefix+'freedate%'

if @x28datatype='decimal' or @x28datatype='integer'
  set @mask=@entityprefix+'freenumber%'


select @id=[id]
from sysobjects
where [name] like @table

if isnull(@id,0)=0
  return(null)

select top 1 @ret=[name]
from
dbo.syscolumns
where [name] like @mask and [id]=@id and upper([name]) not in (select upper(x28field) from x28EntityField where x29id=@x29id and x28Field is not null)

RETURN(@ret)


END




























GO

----------FN---------------x38_get_freecode-------------------------

if exists (select 1 from sysobjects where  id = object_id('x38_get_freecode') and type = 'FN')
 drop function x38_get_freecode
GO







CREATE FUNCTION [dbo].[x38_get_freecode](@x38id int,@x29id int,@datapid int,@attempt_number int)
RETURNS varchar(50) AS  
BEGIN 

declare @mask varchar(200),@x38ExplicitIncrementStart int,@code_new varchar(50),@val int,@code_max_used varchar(50)
declare @x38Scale int,@x38ConstantBeforeValue varchar(50),@pid_last int,@x38isdraft bit

set @val=0

if @x38id is null and @x29id is not null
 select top 1 @x38id=x38ID FROM x38CodeLogic WHERE x29ID=@x29id AND getdate() between x38ValidFrom AND x38ValidUntil

if @x38id is null
 RETURN('')


select @mask=x38MaskSyntax,@x29id=x29ID,@x38ExplicitIncrementStart=isnull(x38ExplicitIncrementStart,0)
,@x38ConstantBeforeValue=isnull(x38ConstantBeforeValue,''),@x38Scale=x38Scale,@x38isdraft=x38IsDraft
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

  set @code_new=@x38ConstantBeforeValue+right('0000000000'+convert(varchar(10),@val),@x38Scale)
  
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

----------P---------------c11_insertrec-------------------------

if exists (select 1 from sysobjects where  id = object_id('c11_insertrec') and type = 'P')
 drop procedure c11_insertrec
GO




CREATE procedure [dbo].[c11_insertrec]
@c11level int,@c11datefrom datetime,@c11dateuntil datetime,@week int,@month int,@quarter int,@year int
as


SET DATEFIRST 1

declare @code varchar(20),@id int,@parentid int,@name varchar(50),@s varchar(200)
declare @day int,@strid varchar(20)

if @year=-1
  set @year=year(@c11datefrom)

if @quarter=-1
  set @quarter=datepart(quarter,@c11datefrom)

if @month=-1
  set @month=month(@c11datefrom)


set @day=day(@c11datefrom)

if @week=-1
  set @week=datepart(week,@c11datefrom)

if @c11level=1
  begin
  	set @code=convert(varchar(4),@year)+'-00-00-000000-00'
	set @name='Rok '+convert(varchar(4),@year)
	set @strid=convert(varchar(10),@year-2000)+'0000000'
	set @quarter=0
	set @month=0
	set @week=0
	set @day=0
  end
if @c11level=2
  begin
  	set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-00-000000-00'
	set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+'000000'
	set @name='Ètvrtletí '+convert(varchar(4),@year)+'-'+convert(char(1),@quarter)
  end

if @c11level=3
  begin
  	set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-000000-00'
	set @name='Mìsíc '+convert(varchar(4),@year)+'-'+convert(varchar(2),@month)
	set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+right('0'+convert(varchar(2),@month),2)+'0000'
  end

if @c11level=4
  begin
    
    set @s=convert(varchar(4),@year)+right('0'+convert(varchar(2),@week),2)
    set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-'+@s+'-00'
    set @name='Týden '+convert(varchar(4),@year)+'-'+convert(varchar(2),@week)
    set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+right('0'+convert(varchar(2),@month),2)+right('0'+convert(varchar(2),@week),2)+'00'
    
  end

if @c11level=5
  begin
    set @s=convert(varchar(4),@year)+right('0'+convert(varchar(2),@week),2)
    set @s=@s+'-'+right('0'+convert(varchar(2),@day),2)
    set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-'+@s
    set @name=convert(varchar(10),@c11datefrom,104)+' '+dbo.get_datename(@c11datefrom,0)
    set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+right('0'+convert(varchar(2),@month),2)+right('0'+convert(varchar(2),@week),2)+right('0'+convert(varchar(2),@day),2)
  end



set @id=convert(int,@strid)

declare @ordinary int


set @ordinary=@id

select top 1 @parentid=c11id from c11statperiod where c11level=@c11level-1 and c11id<=@id and c11y=@year order by c11id desc

if @c11level=1
  set @parentid=0


insert into c11statperiod
(
c11id,c11parentid,c11name,c11code,c11ordinary,c11level,c11validfrom,c11validuntil,c11datefrom,c11dateuntil,c11y,c11q,c11m,c11w,c11d
)
values
(
@id,@parentid,@name,@code,@ordinary,@c11level,getdate(),convert(datetime,'01.01.3000',104),@c11datefrom,@c11dateuntil,@year,@year*100+@quarter,@year*100+@month,@year*100+@week,@day
)






GO

----------P---------------c11_yearrecovery-------------------------

if exists (select 1 from sysobjects where  id = object_id('c11_yearrecovery') and type = 'P')
 drop procedure c11_yearrecovery
GO




CREATE  procedure [dbo].[c11_yearrecovery]
@year int
as


SET DATEFIRST 1

delete from c11statperiod where c11y=@year

declare @level int,@y int,@i int,@dats varchar(10),@d1 datetime,@d2 datetime,@firstmonday datetime,@firstthurday datetime

set @i=0
set @d1=convert(datetime,'01.01.'+convert(varchar(4),@year))
set @d2=dateadd(day,-1,dateadd(year,1,@d1))
set @firstmonday=@d1


EXEC c11_insertrec 1, @d1, @d2,-1,-1,-1,-1


while year(@d1)=@year
  begin
    if day(@d1)=1 and (month(@d1)=1 or month(@d1)=4 or month(@d1)=7 or month(@d1)=10)
      begin	---kvartály
	set @d2=dateadd(day,-1,dateadd(quarter,1,@d1))
    	EXEC c11_insertrec 2, @d1, @d2,-1,-1,-1,-1
      end

    if day(@d1)=1
      begin	---mìsíce
	set @d2=dateadd(day,-1,dateadd(month,1,@d1))
	EXEC c11_insertrec 3, @d1, @d2,-1,-1,-1,-1
      end

    
    set @d1=dateadd(day,1,@d1)
    set @i=@i+1
    
  end


---generovat týdny----
set @firstthurday=convert(datetime,'01.01.'+convert(varchar(4),@year)) 

while datepart(weekday,@firstthurday)<>4
  begin
	set @firstthurday=dateadd(day,1,@firstthurday)
  end

set @firstmonday=dateadd(day,-3,@firstthurday)


set @d1=@firstmonday
set @i=1
declare @q int,@m int

while year(@d1)=@year or year(@d2)=@year
  begin
    set @d2=dateadd(day,6,@d1)

    if year(@d2)=@year
     begin
    	set @q=datepart(quarter,@d2)
    	set @m=month(@d2)
     end
    else
     begin
    	set @q=datepart(quarter,@d1)
    	set @m=month(@d1)
     end

    EXEC c11_insertrec 4, @d1, @d2,@i,@m,@q,@year

    set @d1=dateadd(day,7,@d1)
    set @i=@i+1
  end

---generování dnù---------
set @d1=convert(datetime,'01.01.'+convert(varchar(4),@year))

while year(@d1)=@year
  begin
    
    EXEC c11_insertrec 5, @d1, @d1,-1,-1,-1,-1
    set @d1=dateadd(day,1,@d1)
  end


---narovnání c11parentid dnù vùèi týdnùm---
declare @w int

DECLARE curW CURSOR FOR 
	select c11id,c11datefrom,c11dateuntil,c11w from c11statperiod
	where (c11level=4 and c11y=@year) or (c11id in (select max(c11id) from c11statperiod where c11level=4 and c11y=@year-1))
	
	OPEN curW
	FETCH NEXT FROM curW 
	INTO @i,@d1,@d2,@w
	WHILE @@FETCH_STATUS = 0
	BEGIN		
	    update c11statperiod set c11parentid=@i,c11w=@w where c11level=5 and c11datefrom>=@d1 and c11datefrom<=@d2

   	  FETCH NEXT FROM curW 
   	  INTO @i,@d1,@d2,@w
	END
	CLOSE curW
	DEALLOCATE curW

---závìreèné èištìní
update c11statperiod set c11q=0,c11m=0,c11w=0,c11d=0 where c11y=@year and c11level=1
update c11statperiod set c11m=0,c11w=0,c11d=0 where c11y=@year and c11level=2
update c11statperiod set c11w=0,c11d=0 where c11y=@year and c11level=3
update c11statperiod set c11d=0 where c11y=@year and c11level=4




GO

----------P---------------c21_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('c21_aftersave') and type = 'P')
 drop procedure c21_aftersave
GO








CREATE   procedure [dbo].[c21_aftersave]
@c21id int
,@j03id_sys int
AS
---aktualizovat rozpis dnù ve fondu do regionálních instancí kalendáøù
declare @count int
select @count=count(*) from c21FondCalendar

if @count=1
 begin
   truncate table c22FondCalendar_Date
 end

exec c21_recovery @c21id,null

declare @j17id int

DECLARE curJ17 CURSOR FOR 
select j17id from j17Country
OPEN curJ17
FETCH NEXT FROM curJ17  INTO @j17id
WHILE @@FETCH_STATUS = 0
BEGIN

 exec c21_recovery @c21id,@j17id

FETCH NEXT FROM curJ17 INTO @j17id
END

CLOSE curJ17
DEALLOCATE curJ17








GO

----------P---------------c21_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('c21_delete') and type = 'P')
 drop procedure c21_delete
GO








CREATE   procedure [dbo].[c21_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--c21id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu pracovního kalendáøe z tabulky c21FondCalendar
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE c21ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna osoba má vazbu na tento pracovní kalendáø ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	delete from c22FondCalendar_Date where c21ID=@pid
	
	delete from c21FondCalendar where c21ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------c21_recovery-------------------------

if exists (select 1 from sysobjects where  id = object_id('c21_recovery') and type = 'P')
 drop procedure c21_recovery
GO








CREATE  procedure [dbo].[c21_recovery]
@c21id int
,@j17id int

as

declare @po_hours float,@ut_hours float,@st_hours float,@ct_hours float,@pa_hours float,@so_hours float,@ne_hours float
declare @c21scopeflag int

select @po_hours=c21day1_hours,@ut_hours=c21day2_hours,@st_hours=c21day3_hours,@ct_hours=c21day4_hours,@pa_hours=c21day5_hours
,@so_hours=c21day6_hours,@ne_hours=c21day7_hours,@c21scopeflag=c21ScopeFlag
FROM c21FondCalendar
where c21id=@c21id

declare @datMonthFrom datetime,@datMonthTo datetime
declare @decHoursPerDay as decimal(11,2)

set @datMonthFrom=convert(datetime,'01.01.2014',104)

select @datMonthTo=max(c11dateuntil) from c11statperiod

set @decHoursPerDay=8


delete from c22FondCalendar_Date where c21id=@c21id and isnull(j17ID,0)=isnull(@j17id,0)

insert into c22FondCalendar_Date
(c11id,c22date,c22Hours_Potencial,c21id,c22Hours_Work,j17id)
select c11id,c11datefrom,@decHoursPerDay,@c21id,@decHoursPerDay,@j17id
FROM c11StatPeriod
WHERE c11level=5 and c11datefrom>=@datMonthFrom

SET DATEFIRST 1

  update c22FondCalendar_Date set c22Hours_Work=@po_hours
  WHERE datepart(weekday,c22date)=1 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)
  

  update c22FondCalendar_Date set c22Hours_Work=@ut_hours
  WHERE datepart(weekday,c22date)=2 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)


  update c22FondCalendar_Date set c22Hours_Work=@st_hours
  WHERE datepart(weekday,c22date)=3 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)


  update c22FondCalendar_Date set c22Hours_Work=@ct_hours
  WHERE datepart(weekday,c22date)=4 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)

  
  update c22FondCalendar_Date set c22Hours_Work=@pa_hours
  WHERE datepart(weekday,c22date)=5 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)


  update c22FondCalendar_Date set c22Hours_Work=@so_hours
  WHERE datepart(weekday,c22date)=6 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)

  update c22FondCalendar_Date set c22Hours_Work=@ne_hours
  WHERE datepart(weekday,c22date)=7 and c21id=@c21id and c22date>=@datMonthFrom and isnull(j17id,0)=isnull(@j17id,0)

  

if @c21scopeflag=3  ---fond hodin je shodný s vykázaným timesheet
 begin
  update c22FondCalendar_Date set c22Hours_Work=8 where c21id=@c21id and isnull(j17id,0)=isnull(@j17id,0)

  
 end


--svátky mají totální prioritu nepracování, kontroluje se stát
UPDATE c22FondCalendar_Date set c22Hours_Work=0,c26ID=b.c26ID
FROM
c22FondCalendar_Date a INNER JOIN c26Holiday b ON a.c22Date=b.c26Date AND isnull(a.j17ID,0)=isnull(b.j17ID,0)
WHERE c22date>=@datMonthFrom



























GO

----------P---------------c26_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('c26_aftersave') and type = 'P')
 drop procedure c26_aftersave
GO







CREATE   procedure [dbo].[c26_aftersave]
@c26id int
,@j03id_sys int
AS
---aktualizovat rozpis dnù ve fondu, kam má vliv den svátku

declare @j17id int

select @j17id=j17ID FROM c26Holiday WHERE c26ID=@c26id


declare @c21id int


DECLARE curC21 CURSOR FOR 
select c21ID from c21FondCalendar
OPEN curC21
FETCH NEXT FROM curC21  INTO @c21id
WHILE @@FETCH_STATUS = 0
BEGIN

 exec c21_recovery @c21id,null

FETCH NEXT FROM curC21 INTO @c21id
END

CLOSE curC21
DEALLOCATE curC21





DECLARE curC17 CURSOR FOR 
select j17id from j17Country
OPEN curC17
FETCH NEXT FROM curC17  INTO @j17id
WHILE @@FETCH_STATUS = 0
BEGIN

 exec c21_recovery @c21id,@j17id

FETCH NEXT FROM curC17 INTO @j17id
END

CLOSE curC17
DEALLOCATE curC17








GO

----------P---------------c26_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('c26_delete') and type = 'P')
 drop procedure c26_delete
GO








CREATE   procedure [dbo].[c26_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--c26id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu dnes svátku z tabulky c21FondCalendar


BEGIN TRANSACTION

BEGIN TRY
	delete from c22FondCalendar_Date where c26ID=@pid
	
	delete from c26Holiday where c26ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j02_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_aftersave') and type = 'P')
 drop procedure j02_aftersave
GO








CREATE    PROCEDURE [dbo].[j02_aftersave]
@j02id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu osoby

exec [x90_appendlog] 102,@j02id,@j03id_sys


exec [j02_recovery]






GO

----------P---------------j02_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_delete') and type = 'P')
 drop procedure j02_delete
GO







CREATE   procedure [dbo].[j02_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p32id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu osoby z tabulky j02Person
declare @ref_pid int

if exists(select p31ID FROM p31Worksheet where j02ID=@pid)
 set @err_ret='Minimálnì jeden worksheet záznam má vazbu na tuto osobu.'

if isnull(@err_ret,'')<>''
 return 

set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu worksheet úkonu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu projektu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p41',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu záznamu kontaktu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì v jednomu záznamu faktury je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	UPDATE j03User SET j02ID=NULL WHERE j02ID=@pid

	if exists(SELECT x69ID FROM x69EntityRole_Assign WHERE j02ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE j02ID=@pid

	if exists(select o27ID FROM o27Attachment where j02ID=@pid)
	 DELETE FROM o27Attachment WHERE j02ID=@pid

	if exists(select o23ID FROM o23Notepad where j02ID=@pid)
	 DELETE FROM o23Notepad WHERE j02ID=@pid

	if exists(select j12ID FROM j12Team_Person where j02ID=@pid)
	 DELETE FROM j12Team_Person WHERE j02ID=@pid

	if exists(SELECT p30ID FROM p30Contact_Person WHERE j02ID=@pid)
	 DELETE FROM p30Contact_Person WHERE j02ID=@pid

	if exists(SELECT p46ID FROM p46Project_Person WHERE j02ID=@pid)
	 DELETE FROM p46Project_Person WHERE j02ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE j02ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE j02ID=@pid

	if exists(SELECT j05ID FROM j05MasterSlave WHERE j02ID_Master=@pid OR j02ID_Slave=@pid)
	 DELETE FROM j05MasterSlave WHERE j02ID_Master=@pid OR j02ID_Slave=@pid


	DELETE FROM x90EntityLog WHERE x29ID=102 AND x90RecordPID=@pid


	DELETE FROM j02Person WHERE j02ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------j02_recovery-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_recovery') and type = 'P')
 drop procedure j02_recovery
GO










CREATE PROCEDURE [dbo].[j02_recovery]
AS

declare @j11id_all int

select @j11id_all=j11ID FROM j11Team WHERE j11IsAllPersons=1

if @j11id_all is null
 BEGIN
  INSERT INTO j11Team(j11IsAllPersons,j11Name,j11UserInsert,j11UserUpdate,j11DateInsert,j11DateUpdate) VALUES(1,'Všechny osoby','recovery','recovery',getdate(),getdate())

  set @j11id_all=@@IDENTITY
 END


 INSERT INTO j12Team_Person(j11ID,j02ID)
 SELECT @j11id_all,j02ID
 FROM j02Person
 WHERE j02IsIntraPerson=1 AND j02ID NOT IN (SELECT j02ID FROM j12Team_Person where j11ID=@j11id_all)



	
	






GO

----------P---------------j03_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03_delete') and type = 'P')
 drop procedure j03_delete
GO








CREATE   procedure [dbo].[j03_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j03id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu uživatele z tabulky j03User


BEGIN TRANSACTION

BEGIN TRY

	if exists(select j90ID FROM j90LoginAccessLog where j03ID=@pid)
      DELETE FROM j90LoginAccessLog where j03ID=@pid 

	if exists(select x47ID FROM x47EventLog where j03ID=@pid)
      DELETE FROM x47EventLog where j03ID=@pid 

	if exists(select x37ID FROM x37SavedDockState WHERE j03ID=@pid)
	 DELETE FROM x37SavedDockState WHERE j03ID=@pid

	delete from j03User where j03ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------j03user_load_sysuser-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03user_load_sysuser') and type = 'P')
 drop procedure j03user_load_sysuser
GO



CREATE   procedure [dbo].[j03user_load_sysuser]
@login nvarchar(50)

AS

declare @j03id int,@j02id int,@j04id int,@personal_page varchar(200),@personal_page_mobile varchar(200),@x67id int

select @j03id=a.j03ID,@j02id=a.j02ID,@j04id=a.j04id,@personal_page=b.j04Aspx_PersonalPage,@personal_page_mobile=b.j04Aspx_PersonalPage_Mobile
,@x67id=b.x67ID
FROM j03user a INNER JOIN j04UserRole b on a.j04ID=b.j04ID
WHERE a.j03Login=@login

declare @appscope varchar(255),@cache_x36id int,@cache_value varchar(10),@is_dropbox bit

select @appscope=convert(varchar(255),x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'AppScope'

set @is_dropbox=0
if exists(select x35ID FROM x35GlobalParam WHERE x35Key='Dropbox_IsUse' AND x35Value='1')
 set @is_dropbox=1

declare @is_approve bit,@is_master bit
set @is_approve=0
set @is_master=0

if exists(SELECT x68ID FROM x68EntityRole_Permission WHERE x67ID=@x67id AND x53ID=58)
 set @is_approve=1	--mùže paušálnì schvalovat veškerý worksheet, oprávnìní x53ID=58: Oprávnìní schvalovat všechny worksheet úkony v databázi

if @is_approve=0
begin
 if exists(SELECT TOP 1 a.x67ID FROM x67EntityRole a INNER JOIN x69EntityRole_Assign x69 ON a.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON a.x67ID=o28.x67ID WHERE getdate() BETWEEN a.x67ValidFrom AND a.x67ValidUntil AND a.x29ID=141 AND o28.o28PermFlag IN (3,4) AND (x69.j02ID=@j02id OR x69.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id)))
  set @is_approve=1	---má oprávnìní schvalovat worksheet v minimálnì jednom projektu

end

if exists(select j05ID FROM j05MasterSlave WHERE j02ID_Master=@j02id)
 set @is_master=1

select a.*,a.j03id as _pid
,j04.j04name as _j04Name,j02.j02LastName as _j02LastName,j02.j02FirstName as _j02FirstName,j02.j02TitleBeforeName as _j02TitleBeforeName,j02.j02Email as _j02Email
,a.j03dateupdate as _dateupdate,a.j03dateinsert as _dateinsert,a.j03userupdate as _userupdate,a.j03userinsert as _userinsert
,a.j03validfrom as _validfrom,a.j03validuntil as _validuntil,a.j03IsLiveChatSupport,a.j03SiteMenuSkin,a.j03IsSiteMenuOnClick
,@appscope as _AppScopeCrypted,x67.x67RoleValue as _RoleValue
,j02.j02Email as _j02Email,@is_approve as _IsApprovingPerson,@is_master as _IsMasterPerson
,@is_dropbox as _IsDropbox
,case when a.j03Aspx_PersonalPage IS NULL THEN @personal_page ELSE a.j03Aspx_PersonalPage END as _PersonalPage
,j04.j04IsMenu_Worksheet,j04.j04IsMenu_Report,j04.j04IsMenu_Project,j04.j04IsMenu_People,j04.j04IsMenu_Contact,j04.j04IsMenu_Invoice,j04.j04IsMenu_Proforma
FROM j03user a INNER JOIN j04userrole j04 on a.j04id=j04.j04id LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id
INNER JOIN x67EntityRole x67 ON j04.x67ID=x67.x67ID
WHERE a.j03ID=@j03id























GO

----------P---------------j04_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j04_delete') and type = 'P')
 drop procedure j04_delete
GO









CREATE   procedure [dbo].[j04_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j04id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu role z tabulky j04UserRole
declare @ref_pid int,@x67id int

select @x67id=x67ID FROM j04UserRole WHERE j04ID=@pid

if @x67id is null
 begin
  set @err_ret='x67id missing'
  return
 end

SELECT TOP 1 @ref_pid=j03ID from j03User WHERE j04ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden uživatelský úèet má vazbu na tuto aplikaèní roli ('+dbo.GetObjectAlias('j03',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM x68EntityRole_Permission WHERE x67ID=@x67id

	DELETE FROM x69EntityRole_Assign WHERE x67ID=@x67id

	DELETE FROM x67EntityRole WHERE x67ID=@x67id

	delete from j04UserRole where j04ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------j05_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j05_delete') and type = 'P')
 drop procedure j05_delete
GO









CREATE   procedure [dbo].[j05_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j05id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu MASTERSLAVE z tabulky j05MasterSlave



BEGIN TRANSACTION

BEGIN TRY
	
	delete from j05MasterSlave where j05ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------j07_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j07_delete') and type = 'P')
 drop procedure j07_delete
GO






CREATE   procedure [dbo].[j07_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j07id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu pozice z tabulky j07PersonPosition
declare @ref_pid int

SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE j07ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna osoba má vazbu na tuto pozici ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select p52ID FROM p52PriceList_Item where j07ID=@pid)
	 DELETE FROM p52PriceList_Item WHERE j07ID=@pid

	if exists(select x69ID FROM x69EntityRole_Assign where j07ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE j07ID=@pid


	delete from j07PersonPosition where j07ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------j11_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j11_delete') and type = 'P')
 drop procedure j11_delete
GO





CREATE   procedure [dbo].[j11_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j11id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu týmu osob z tabulky j11Team
declare @ref_pid int,@x29id int,@x69recordpid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=a.x69ID,@x29id=b.x29ID,@x69recordpid=a.x69recordpid
from x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID
WHERE a.j11ID=@pid

if @ref_pid is not null and @x29id=141
 set @err_ret='Tento tým je obsazen pøes projektovou roli minimálnì v jednom projektu ('+dbo.GetObjectAlias('p41',@x69recordpid)+')'

if @ref_pid is not null and @x29id=328
 set @err_ret='Tento tým je obsazen rolí minimálnì v jednom záznamu kontaktu ('+dbo.GetObjectAlias('p28',@x69recordpid)+')'

if @ref_pid is not null and @x29id=391
 set @err_ret='Tento tým je obsazen rolí minimálnì v jedné faktuøe ('+dbo.GetObjectAlias('p91',@x69recordpid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select o20ID FROM o20Milestone_Receiver WHERE j11ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE j11ID=@pid

	DELETE FROM x69EntityRole_Assign WHERE j11ID=@pid

	if exists(select j05ID FROM j05MasterSlave WHERE j11ID_Slave=@pid)
	 DELETE FROM j05MasterSlave WHERE j11ID_Slave=@pid


	DELETE FROM j12Team_Person WHERE j11ID=@pid

	DELETE FROM j11Team WHERE j11ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j17_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j17_delete') and type = 'P')
 drop procedure j17_delete
GO








CREATE   procedure [dbo].[j17_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j17id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu státu z tabulky j17Country
declare @ref_pid int

SELECT TOP 1 @ref_pid=c26ID from c26Holiday WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden den svátku je svázaný s tímto státem ('+dbo.GetObjectAlias('c26',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna osoba má vazbu na tento stát ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j17Country where j17ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j18_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j18_delete') and type = 'P')
 drop procedure j18_delete
GO






CREATE   procedure [dbo].[j18_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j18id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu regionu z tabulky j18Region
declare @ref_pid int

SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE j18ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden projekt je svázaný s tímto regionem ('+dbo.GetObjectAlias('p41',@ref_pid)+')'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j18Region where j18ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------j23_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j23_delete') and type = 'P')
 drop procedure j23_delete
GO








CREATE   procedure [dbo].[j23_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j23id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu nepersonálního zdroje z tabulky j23NonPerson
declare @ref_pid int

SELECT TOP 1 @ref_pid=o22ID from o19Milestone_NonPerson WHERE j23ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna rezervaèní událost má vazbu na tento zdroj ('+dbo.GetObjectAlias('o22',@ref_pid)+')'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j23NonPerson where j23ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j24_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j24_delete') and type = 'P')
 drop procedure j24_delete
GO








CREATE   procedure [dbo].[j24_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j24id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu nepersonálního zdroje z tabulky j24NonPersonType
declare @ref_pid int

SELECT TOP 1 @ref_pid=j23ID from j23NonPerson WHERE j24ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden nepersonální zdroj je svázaný s tímto typem ('+dbo.GetObjectAlias('j23',@ref_pid)+')'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j24NonPersonType where j24ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j25_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j25_delete') and type = 'P')
 drop procedure j25_delete
GO







CREATE   procedure [dbo].[j25_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j25ID
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu kategorie z tabulky j25ReportCategory
declare @ref_pid int

SELECT TOP 1 @ref_pid=x31ID from x31Report WHERE x31ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna šablona sestavy nebo pluginu má vazbu na tuto kategorii ('+dbo.GetObjectAlias('x31',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	

	delete from j25ReportCategory where j25ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j74_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j74_delete') and type = 'P')
 drop procedure j74_delete
GO










CREATE   procedure [dbo].[j74_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--j74id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky j74SavedGridColTemplate


if exists(select j74ID FROM j74SavedGridColTemplate WHERE j74ID=@pid AND j74IsSystem=1)
 set @err_ret='Výchozí šablonu sloupcù nelze odstranit.'


if isnull(@err_ret,'')<>''
 return 

delete from j74SavedGridColTemplate where j74ID=@pid

















GO

----------P---------------m62_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('m62_delete') and type = 'P')
 drop procedure m62_delete
GO







CREATE   procedure [dbo].[m62_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--m62id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu mìnového kurzu z tabulky m62ExchangeRate


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from m62ExchangeRate where m62ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------o21_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o21_delete') and type = 'P')
 drop procedure o21_delete
GO







CREATE   procedure [dbo].[o21_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o21id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu milníku z tabulky o21MilestoneType
declare @ref_pid int

SELECT TOP 1 @ref_pid=o22ID from o22Milestone WHERE o21ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden milník/termín/událost je svázaný s tímto typem ('+dbo.GetObjectAlias('o22',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	


	delete from o21MilestoneType where o21ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------o22_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o22_delete') and type = 'P')
 drop procedure o22_delete
GO








CREATE   procedure [dbo].[o22_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o22id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu milníku z tabulky o22Milestone


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select o19ID FROM o19Milestone_NonPerson WHERE o22ID=@pid)
	 DELETE FROM o19Milestone_NonPerson WHERE o22ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE o22ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE o22ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid)

    if exists(select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid

	delete from o22Milestone where o22ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------o23_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o23_delete') and type = 'P')
 drop procedure o23_delete
GO












CREATE   procedure [dbo].[o23_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o23ID
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu adresy z tabulky o38Address

BEGIN TRANSACTION

BEGIN TRY

	if exists(select o27ID FROM o27Attachment WHERE o23ID=@pid)
	 DELETE FROM o27Attachment WHERE o23ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)

    if exists(select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid

	delete from o23Notepad where o23ID=@pid
	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  








GO

----------P---------------o27_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o27_delete') and type = 'P')
 drop procedure o27_delete
GO








CREATE   procedure [dbo].[o27_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o27id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu dokumentu z tabulky o27Attachment


BEGIN TRANSACTION

BEGIN TRY
	


	delete from o27Attachment where o27ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------o32_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o32_delete') and type = 'P')
 drop procedure o32_delete
GO












CREATE   procedure [dbo].[o32_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o32id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu kontaktního média z tabulky o32Contact_Medium



delete from o32Contact_Medium where o32ID=@pid










GO

----------P---------------o38_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o38_delete') and type = 'P')
 drop procedure o38_delete
GO











CREATE   procedure [dbo].[o38_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--a38id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu adresy z tabulky o38Address
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE o38ID_Primary=@pid or o38ID_Delivery=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna klientská faktura má vazbu na tuto adresu ('+dbo.GetObjectAlias('p91',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 


BEGIN TRANSACTION

BEGIN TRY

	delete from o38Address where o38ID=@pid
	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  







GO

----------P---------------p28_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_aftersave') and type = 'P')
 drop procedure p28_aftersave
GO






CREATE    PROCEDURE [dbo].[p28_aftersave]
@p28id int
,@j03id_sys int

AS

declare @p28code varchar(10),@p29id int,@x38id int,@p28name nvarchar(255),@iscompany bit
declare @p28companyname nvarchar(255),@p28companyshortname nvarchar(50)

select @p28code=p28code,@p29id=a.p29id,@iscompany=p28IsCompany
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


exec [x90_appendlog] 328,@p28id,@j03id_sys
 
 






GO

----------P---------------p28_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_delete') and type = 'P')
 drop procedure p28_delete
GO







CREATE   procedure [dbo].[p28_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p28id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu kontaktu z tabulky p28Contact
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE p28ID_Client=@pid OR p28ID_Billing=@pid
if @ref_pid is not null
 set @err_ret='Tento kontakt je klientem nebo odbìratelem minimálnì jednoho projektu ('+dbo.GetObjectAlias('p41',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p28ID=@pid
if @ref_pid is not null
 set @err_ret='Tento kontakt je klientem v minimálnì jedné faktuøe ('+dbo.GetObjectAlias('p91',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p90ID from p90Proforma WHERE p28ID=@pid
if @ref_pid is not null
 set @err_ret='Tento kontakt je klientem v minimálnì jedné zálohové faktuøe ('+dbo.GetObjectAlias('p90',@ref_pid)+')'



if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p28ID=@pid)
	 DELETE FROM o27Attachment WHERE p28ID=@pid

	if exists(SELECT o32ID FROM o32Contact_Medium WHERE p28ID=@pid)
	 DELETE FROM o32Contact_Medium WHERE p28ID=@pid

	if exists(SELECT o23ID FROM o23Notepad WHERE p28ID=@pid)
	 DELETE FROM o23Notepad WHERE p28ID=@pid

	if exists(select o37ID FROM o37Contact_Address WHERE p28ID=@pid)
	 begin
	  DELETE FROM o37Contact_Address WHERE p28ID=@pid

	  DELETE FROM o38Address WHERE o38ID IN (select o38ID FROM o37Contact_Address WHERE p28ID=@pid)
	 end

	if exists(select p28ID FROM p28Contact_FreeField WHERE p28ID=@pid)
	 DELETE FROM p28Contact_FreeField WHERE p28ID=@pid

	if exists(select p30ID FROM p30Contact_Person where p28ID=@pid)
	 DELETE FROM p30Contact_Person where p28ID=@pid

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=328)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=328


	DELETE FROM x90EntityLog WHERE x29ID=328 AND x90RecordPID=@pid

	delete from p28Contact WHERE p28ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p29_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p29_delete') and type = 'P')
 drop procedure p29_delete
GO









CREATE   procedure [dbo].[p29_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p29id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p29contacttype
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE p29ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden kontakt má vazbu na tento typ ('+dbo.GetObjectAlias('p28',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY


	delete from p29ContactType where p29ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p31_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_aftersave') and type = 'P')
 drop procedure p31_aftersave
GO





CREATE    PROCEDURE [dbo].[p31_aftersave]
@p31id int
,@j03id_sys int
,@x45ids varchar(50) OUTPUT	---pøípadné události, které se mají notifikovat (èárkou oddìlené x45id)

AS

---automaticky se spouští po uložení worksheet záznamu
set @x45ids=''

declare @p31date datetime,@p32id int,@p41id int,@p34id int,@p71id int,@p70id int,@c11id int,@p33id int,@j02id_rec int
declare @j27id_billing_orig int,@j27id_internal int,@p31rate_billing_orig float,@p31rate_internal_orig float
declare @p31value_orig float,@p31amount_withoutvat_orig float,@p31vatrate_orig float,@p31amount_withvat_orig float
declare @p31amount_internal float,@p31amount_vat_orig float,@p91id int

select @c11id=a.c11ID,@p32id=a.p32ID,@p34id=p32.p34ID,@p71id=a.p71ID,@p70id=a.p70ID
,@p33id=p34.p33ID,@j02id_rec=a.j02ID,@p31date=a.p31date,@p41id=a.p41ID
,@p31value_orig=a.p31value_orig,@p91id=a.p91ID
FROM
p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p31ID=@p31id

if isnull(@p71id,0)<>0 or ISNULL(@p70id,0)<>0 or ISNULL(@p91id,0)<>0
 return	---pokud úkon prošel schvalováním nebo fakturací, není možné mìnit jeho atributy!!!!!
 

declare @c11id_find int

select top 1 @c11id_find=c11id from c11statperiod where c11level=5 and c11datefrom=@p31date  

-----statistické období c11-------------------------
if @c11id is null OR @c11id_find is null OR isnull(@c11id_find,0)<>isnull(@c11id,0)
 BEGIN  
  if @c11id_find is null
    begin
      if year(@p31date)>1950 and year(@p31date)<year(getdate())+40
        begin
		  declare @year int
		  set @year=year(@p31date)
          exec c11_yearrecovery @year

		  select top 1 @c11id_find=c11id from c11statperiod where c11level=5 and c11datefrom=@p31date
        end
    end
    
   update p31worksheet set c11ID=@c11id_find WHERE p31ID=@p31id
 END

if @p33id=1 or @p33id=3	---1 - èas, 3 - kusovník
 BEGIN        
	exec p31_getrate_tu 1, @p41id, @j02id_rec, @p32id, @j27id_billing_orig OUTPUT , @p31rate_billing_orig OUTPUT

	exec p31_getrate_tu 2, @p41id, @j02id_rec, @p32id, @j27id_internal OUTPUT , @p31rate_internal_orig OUTPUT  
	
	select @p31vatrate_orig=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
	
	set @p31amount_withoutvat_orig=@p31value_orig*@p31rate_billing_orig
	set @p31amount_vat_orig=@p31amount_withoutvat_orig*@p31vatrate_orig/100
	set @p31amount_withvat_orig=@p31amount_withoutvat_orig+@p31amount_vat_orig
	set @p31amount_internal=@p31value_orig*@p31rate_internal_orig
	
	update p31WorkSheet set p31amount_withoutvat_orig=@p31amount_withoutvat_orig,p31amount_vat_orig=@p31amount_vat_orig
	,p31amount_withvat_orig=@p31amount_withvat_orig,p31VatRate_Orig=@p31vatrate_orig
	,p31Amount_Internal=@p31amount_internal
	,p31Rate_Billing_Orig=@p31rate_billing_orig,p31Rate_Internal_Orig=@p31rate_internal_orig
	,j27ID_Billing_Orig=@j27id_billing_orig,j27ID_Internal=@j27id_internal
	WHERE p31ID=@p31id
	
 END
 

 ----otestování limitù k notifikaci
 declare @limit_hours float,@limit_fee float,@real_hours float,@real_fee float,@p28id int

 select @p28id=p28ID_Client,@limit_hours=convert(float,p41LimitHours_Notification),@limit_fee=convert(float,p41LimitFee_Notification)
 FROM p41Project
 WHERE p41ID=@p41id

 if @limit_hours>0 OR @limit_fee>0	---notifikaèní limity nastavené u projektu
  begin
    select @real_hours=sum(p31Hours_Orig),@real_fee=sum(p31Hours_Orig*p31Rate_Billing_Orig)
	FROM p31Worksheet
	where p41ID=@p41id AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

	if @real_hours>@limit_hours and @limit_hours>0
	 set @x45ids=@x45ids+',14110'

	if @real_fee>@limit_fee and @limit_fee>0
	 set @x45ids=@x45ids+',14111'
  end

set @limit_hours=0
set @limit_fee=0

if @p28id is not null
 select @limit_hours=convert(float,p28LimitHours_Notification),@limit_fee=convert(float,p28LimitFee_Notification) FROM p28Contact WHERE p28ID=@p28id

if @limit_hours>0 OR @limit_fee>0	---notifikaèní limit nastavené u klienta projektu
  begin
    select @real_hours=sum(p31Hours_Orig),@real_fee=sum(p31Hours_Orig*p31Rate_Billing_Orig)
	FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID
	where b.p28ID_Client=@p28id AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

	if @real_hours>@limit_hours and @limit_hours>0
	 set @x45ids=@x45ids+',32810'

	if @real_fee>@limit_fee and @limit_fee>0
	 set @x45ids=@x45ids+',32811'
  end

if @x45ids<>''
 set @x45ids=right(@x45ids,len(@x45ids)-1)



GO

----------P---------------p31_append_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_append_invoice') and type = 'P')
 drop procedure p31_append_invoice
GO






CREATE procedure [dbo].[p31_append_invoice]
@p91id int
,@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---vložení schválených worksheet záznamù do uložené faktury @p91id
---vstupní úkony musí být schváleny a uloženy v TEMPu - p85TempBox
---p85Prefix='p31'
---p31id - p85DataPID

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')



if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if @p91id=0
  set @err_ret='Chybí faktura @p91id!'

if @err_ret<>''
 return

declare @login nvarchar(50)
set @login=dbo.j03_getlogin(@j03id_sys)


declare @j27id int,@x15id int,@p91fixedvatrate float


select @j27id=j27ID,@x15id=x15ID,@p91fixedvatrate=p91FixedVatRate
from p91Invoice
where p91ID=@p91id  




update p31worksheet set p91ID=@p91id,p70id=p72ID_AfterApprove
WHERE p91ID is null AND p71id=1 and isnull(p72ID_AfterApprove,0)<>0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p91ID=@p91id,p70id=(case when isnull(p31amount_withoutvat_approved,0)<>0 then 4 else 2 end)
WHERE p91ID is null AND p71id=1 and isnull(p72ID_AfterApprove,0)=0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p31Minutes_Invoiced=p31Minutes_Approved_Billing,p31Hours_Invoiced=p31Hours_Approved_Billing,p31HHMM_Invoiced=p31HHMM_Approved_Billing
,p31Value_Invoiced=p31Value_Approved_Billing
,p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved,p31VatRate_Invoiced=case when @x15id is not null then @p91fixedvatrate else p31VatRate_Approved end
,p31Rate_Billing_Invoiced=p31Rate_Billing_Approved
where p91ID=@p91id AND p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')



exec p91_recalc_amount @p91id

































GO

----------P---------------p31_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_delete') and type = 'P')
 drop procedure p31_delete
GO





CREATE   procedure [dbo].[p31_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p31id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu worksheet úkonu z tabulky p31Worksheet

declare @islocked bit,@p34id int,@isplan bit,@p31date datetime,@p33id int,@j02id_rec int,@p91id int,@p71id int,@p41id int


select @p34id=p32.p34id,@p33id=p34.p33id,@isplan=a.p31IsPlanRecord,@p31date=a.p31Date,@j02id_rec=a.j02ID,@p91id=a.p91ID,@p71id=a.p71ID,@p41id=a.p41ID
from p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
 inner join p34ActivityGroup p34 on p32.p34ID=p34.p34id
where a.p31ID=@pid

if @p71id is not null
 set @err_ret='Tento worksheet úkon již prošel schvalovacím procesem.'

if @p91id is not null
 set @err_ret='Tento worksheet úkon patøí do faktury ('+dbo.GetObjectAlias('p91',@p91id)+').'


if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND (p41ValidFrom>getdate() OR p41ValidUntil<getdate()))
 set @err_ret='Projekt byl pøesunut do koše, nelze v nìm upravovat úkony.'

if isnull(@err_ret,'')<>''
 return 


if @isplan=0
 begin
  --test uzamèeného období-----------
  exec p31_test_lockedperiod @j03id_sys,@p31date,@j02id_rec, @p34id, @islocked OUTPUT 
      

  if @islocked=1
    set @err_ret='Datum ['+convert(varchar(30),@p31date,104)+'] patøí do uzamèeného období, úkon nelze odstranit!'

 end

if isnull(@err_ret,'')<>''
 return 



BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p31ID=@pid)
	 DELETE FROM o27Attachment WHERE p31ID=@pid

	if exists(select p31ID FROM p31worksheet_FreeField WHERE p31ID=@pid)
	 DELETE FROM p31WorkSheet_FreeField WHERE p31ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=331 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=331 AND b07RecordPID=@pid)

    if exists(select b07ID FROM b07Comment WHERE x29ID=331 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=331 AND b07RecordPID=@pid

	DELETE FROM p31Worksheet WHERE p31ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------p31_getrate_tu-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_getrate_tu') and type = 'P')
 drop procedure p31_getrate_tu
GO







CREATE procedure [dbo].[p31_getrate_tu]
@pricelisttype int,@p41id int,@j02id int,@p32id int
,@ret_j27id int OUTPUT,@ret_rate float OUTPUT

---@pricelisttype=1 - fakturaèní ceník
---@pricelisttype=2 - nákladový ceník
AS

  set @pricelisttype=isnull(@pricelisttype,1)
   
  declare @p51id int,@p34id int,@j07id int,@isbillable bit,@p28id int,@p33id int
    
  set @ret_rate=0
  
  if @pricelisttype=1	--fakturaèní ceník
   begin
    select @p51id=p51ID_Billing,@p28id=p28id_client from p41Project where p41id=@p41id
    
    if @p51id is null and @p28id is not null
     select @p51id=p51ID_Billing from p28contact where p28id=@p28id
   end


  if @pricelisttype=2	--nákladový ceník
   begin
     select @p51id=p51ID_Internal,@p28id=p28id_client from p41Project where p41id=@p41id
     
     if @p51id is null and @p28id is not null
      select @p51id=p51ID_Internal from p28contact where p28id=@p28id
   end
    
  
  if @p51id is null
   begin
    select @ret_j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Invoice'

    return	--není ceník u projektu ani u klienta
   end
  

  select @j07id=isnull(j07ID,0) FROM j02Person where j02ID=@j02id
  
  select @p34id=a.p34id,@isbillable=a.p32IsBillable,@p33id=b.p33id
  FROM p32Activity a inner join p34activitygroup b on a.p34id=b.p34id
  where a.p32id=@p32id
  
  if @isbillable=0 and @pricelisttype=1
   begin
     set @ret_rate=0

     select @ret_j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Invoice'

     return	--nefakturovatelná aktivita -> nulová sazba (pravidlo platí u fakt.ceníku)
   end

   
  declare @p52id int,@rate_default_amount float,@p51id_master int

  select @ret_j27id=j27id,@rate_default_amount=case when @p33id=1 THEN p51DefaultRateT else isnull(p51DefaultRateU,0) END,@p51id_master=isnull(p51id_master,0)
  FROM p51PriceList where p51id=@p51id


  --------sazba podle aktivita+uživatel napøímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j02ID=@j02id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return



  --------sazba podle aktivita+pozice osoby napøímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j07ID=@j07id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return

  --------sazba podle aktivita--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return
    
    
--------sazba podle uživatel bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null
  and j02ID=@j02id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return


--------sazba podle pozice osoby bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null
  and j07ID=@j07id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return

--------sazba podle sheet bez aktivity i personálního zdroje--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null and j02ID is null AND j07id is null
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return

----zde už se vrací výchozí sazba ceníku-----------
set @ret_rate=@rate_default_amount




















GO

----------P---------------p31_change_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_change_invoice') and type = 'P')
 drop procedure p31_change_invoice
GO






CREATE procedure [dbo].[p31_change_invoice]
@p91id int
,@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---zmìna vyfakturovaných úkonù ve faktuøe @p91id
---vstupní úkony musí být již obsaženy ve faktuøe a uloženy v TEMPu - p85TempBox
---p85Prefix='p31'
---p31id - p85DataPID
---p70id - p85OtherKey1
---p31Text - p85Message
---p31Value_Invoiced - p85FreeFloat01  (èástka bez DPH u penìz nebo hodiny u èasu)
---p31Rate_Billing_Invoiced - p85FreeFloat02 (hodinová nebo úkonová sazba)
---p31VatRate_Invoiced - p85FreeFloat03  (explicitní sazba DPH)

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')



if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if isnull(@p91id,0)=0
  set @err_ret='Chybí faktura @p91id!'

if exists(select p85ID FROM p85TempBox WHERE p85GUID=@guid AND (p85OtherKey1 IS NULL OR p85DataPID IS NULL))
 set @err_ret='TEMP data p85GUID or p85OtherKey1 missing.'

if @err_ret<>''
 return

declare @login nvarchar(50),@j02id_sys int

select @j02id_sys=j02ID,@login=j03Login FROM j03User WHERE j03ID=@j03id_sys


declare @j27id int,@x15id int,@p91fixedvatrate float


select @j27id=j27ID,@x15id=x15ID,@p91fixedvatrate=p91FixedVatRate
from p91Invoice
where p91ID=@p91id  

declare @p31id int,@p70id_edit int,@vatrate_edit float,@value_edit float,@text_edit nvarchar(2000),@rate_edit float
declare @p33id int
declare @p31amount_withoutvat_invoiced float,@p31amount_vat_invoiced float,@p31amount_withvat_invoiced float

DECLARE curP31 CURSOR FOR 
select p85DataPID,p85OtherKey1,p85Message,p85FreeFloat01,p85FreeFloat02,p85FreeFloat03 from p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31'
OPEN curP31
FETCH NEXT FROM curP31  INTO @p31id,@p70id_edit,@text_edit,@value_edit,@rate_edit,@vatrate_edit
WHILE @@FETCH_STATUS = 0
BEGIN

 select @p33id=c.p33ID FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
 WHERE a.p31ID=@p31id


 if @x15id is not null
  set @vatrate_edit=@p91fixedvatrate	---DPH se pøebírá z jednotné (fixní) dph faktury
 

 if @p70id_edit=2 or @p70id_edit=3 or @p70id_edit=6	---odpis nebo paušál
  begin
   set @value_edit=0
   set @rate_edit=0
  end

 if @p33id=1	---èas
  begin
    UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Rate_Billing_Invoiced=@rate_edit
	,p31Hours_Invoiced=@value_edit,p31Minutes_Invoiced=@value_edit*60,p31HHMM_Invoiced=dbo.get_hours_to_hhmm(@value_edit)
	WHERE p31ID=@p31id
  end

 if @p33id=3	---kusovník
  begin
    UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Rate_Billing_Invoiced=@rate_edit
	WHERE p31ID=@p31id
  end
 
 if @p33id=2 or @p33id=5
  begin
   UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Amount_WithoutVat_Invoiced=@value_edit
   WHERE p31ID=@p31id
  end

 if @p33id=1 OR @p33id=3
  begin
   set @p31amount_withoutvat_invoiced=@rate_edit*@value_edit

   if @x15id is null
	 select @vatrate_edit=p31VatRate_Approved FROM p31Worksheet WHERE p31ID=@p31id	---pokud DPH není ve faktuøe fixní, pak to brát z úkonu
  end
  
  
  if @p33id=2 or @p33id=5
   set @p31amount_withoutvat_invoiced=@value_edit


  set @p31amount_vat_invoiced=@p31amount_withoutvat_invoiced*@vatrate_edit/100
  set @p31amount_withvat_invoiced=@p31amount_withoutvat_invoiced+@p31amount_vat_invoiced

  UPDATE p31Worksheet set p70ID=@p70id_edit,p31IsInvoiceManual=1,j02ID_InvoiceManual=@j02id_sys,p31DateUpdate_InvoiceManual=getdate()
  ,p31Amount_WithoutVat_Invoiced=@p31amount_withoutvat_invoiced,p31Amount_WithVat_Invoiced=@p31amount_withvat_invoiced
  ,p31Amount_Vat_Invoiced=@p31amount_vat_invoiced,p31VatRate_Invoiced=@vatrate_edit,j27ID_Billing_Invoiced=@j27id
  WHERE p31ID=@p31id

  if @text_edit is not null
   UPDATE p31Worksheet set p31Text=@text_edit WHERE p31ID=@p31id

FETCH NEXT FROM curP31 INTO @p31id,@p70id_edit,@text_edit,@value_edit,@rate_edit,@vatrate_edit
END

CLOSE curP31
DEALLOCATE curP31





exec p91_recalc_amount @p91id

































GO

----------P---------------p31_inhale_disposition-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_inhale_disposition') and type = 'P')
 drop procedure p31_inhale_disposition
GO






CREATE procedure [dbo].[p31_inhale_disposition]
@j03id_sys int
,@pid int	---p31id
,@record_disposition int OUTPUT	--_NoAccess = 0, CanRead = 1, CanEdit = 2, CanApprove = 3, CanApproveAndEditable = 4
,@record_state int OUTPUT		--_NotExists=0, Editing=1, Locked=2, Approveded=5, Invoiced=7
,@msg_locked varchar(1000) OUTPUT
AS

set @record_disposition=0
set @record_state=1

declare @is_access_edit bit,@is_access_read bit,@is_access_approve bit
set @is_access_edit=0
set @is_access_approve=0
set @is_access_read=0


declare @p34id int,@isplan bit,@p91id int,@p71id int,@p31date datetime,@j02id_rec int,@j18id int,@p41id int,@p31id int
declare @p41validfrom datetime,@p41validuntil datetime,@p41WorksheetOperFlag int,@j02id_sys int,@j02id_owner int

select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

select @p31id=a.p31ID,@isplan=a.p31IsPlanRecord,@p91id=a.p91ID,@p41validfrom=p41.p41ValidFrom,@p41validuntil=p41.p41ValidUntil
,@p71id=a.p71ID,@p31date=a.p31Date,@j02id_rec=a.j02ID,@p34id=p32.p34ID
,@p41id=a.p41ID,@p41WorksheetOperFlag=p41.p41WorksheetOperFlag,@j18id=p41.j18ID
,@j02id_owner=a.j02ID_Owner
FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
WHERE a.p31ID=@pid

if @p31id is null
 begin
  set @record_state=0	---record not exists
  return
 end

if @p91id is not null
 begin
  set @record_state=7	---invoiced
  set @msg_locked='Vyfakturovaný úkon'
 end

if isnull(@p71id,0)>0 and @record_state=1
 set @record_state=5	--approved


if (@p41validfrom>getdate() OR @p41validuntil<getdate()) and @record_state=1
 begin
  set @record_state=2	---locked
  set @msg_locked='Projekt byl pøesunut do koše.'
 end

if @p41WorksheetOperFlag=1 and @record_state=1
 begin
  set @record_state=2		---locked, p41WorksheetOperFlagEnum=NoEntryData
  set @msg_locked='Projekt je uzavøený pro zapisování úkonù'
 end
if @isplan=0 and @record_state=1
 begin
  --test uzamèeného období-----------
  declare @islocked bit
  set @islocked=0

  exec p31_test_lockedperiod @j03id_sys,@p31date,@j02id_rec, @p34id, @islocked OUTPUT 

  if @islocked=1
   begin
    set @record_state=2	---locked
	set @msg_locked='Úkon spadá do uzamknutého období.'
   end
 end
 
if @j02id_rec=@j02id_sys
 set @is_access_read=1	---osoba záznamu má vždy minimálnì právo na ètení

if @j02id_owner=@j02id_sys
 begin
  set @is_access_edit=1
  set @is_access_read=1
 end

if @j02id_rec=@j02id_sys AND @j02id_rec<>@j02id_owner AND @is_access_edit=0
 begin
	---osoba záznamu není vlatníkem záznamu - natypoval ho nìkdo jiný, globální oprávnìní GR_P31_EditAsNonOwner=25
	if dbo.j03_test_permission_global(@j03id_sys,25)=1
	 begin
		set @is_access_edit=1
		set @is_access_read=1
	 end
 end


if @is_access_edit=0
 begin
  if dbo.j03_test_permission_global(@j03id_sys,22)=1
   begin
    set @is_access_edit=1	----globální právo být vlastníkem pro veškerý worksheet, GR_P31_Owner=22
	set @is_access_read=1
   end
 end

if @is_access_read=0
 begin
   if dbo.j03_test_permission_global(@j03id_sys,21)=1
    set @is_access_read=1	----globální právo èíst veškerý worksheet, GR_P31_Reader=21
 end

if dbo.j03_test_permission_global(@j03id_sys,23)=1
 set @is_access_approve=1	----globální právo schvalovat veškerý worksheet, GR_P31_Approver=23

if @is_access_approve=0 or @is_access_read=0 or @is_access_edit=0
begin  ---ovìøování oprávnìní podle vztahu nadøízený x podøízený
 if exists(select j05ID FROM j05MasterSlave WHERE j02ID_Master=@j02id_sys AND (j02ID_Slave=@j02id_rec OR @j02id_rec IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_sys)))
  set @is_access_read=1

 if exists(select j05ID FROM j05MasterSlave WHERE j05Disposition_p31 IN (2,4) AND j02ID_Master=@j02id_sys AND (j02ID_Slave=@j02id_rec OR @j02id_rec IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_sys)))
  set @is_access_edit=1
 
 if exists(select j05ID FROM j05MasterSlave WHERE j05Disposition_p31 IN (3,4) AND j02ID_Master=@j02id_sys AND (j02ID_Slave=@j02id_rec OR @j02id_rec IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_sys)))
  set @is_access_approve=1
end


if @is_access_approve=0 or @is_access_read=0 or @is_access_edit=0
begin	---ovìøování oprávnìní podle projektové role
	---test manažerského oprávnìní do projektového worksheet---------
	declare @o28permflag int	---0-pouze vlastní worksheet,1-Èíst vše v rámci projektu, 2-Èíst a upravovat vše v rámci projektu,3-Èíst a schvalovat vše v rámci projektu,Èíst, upravovat a schvalovat vše v rámci projektu

	SELECT @o28permflag=dbo.o28_get_permflag(@j02id_sys,@p41id,@j18id,@p34id,1,4)


	if @o28permflag>0
	 set @is_access_read=1

	if @o28permflag IN (2,4)
	 set @is_access_edit=1
	 
	if @o28permflag IN (3,4)
	 set @is_access_approve=1

	if @is_access_read=1 AND @is_access_edit=0
	 begin
		---zjistit, zda má manažerské právo k editaci
		SELECT @o28permflag=dbo.o28_get_permflag(@j02id_sys,@p41id,@j18id,@p34id,2,2)

		if @o28permflag=2
		 set @is_access_edit=1
	 end

	if @is_access_read=1 AND @is_access_approve=0 and @isplan=0
	 begin
	  ---zjistit, zda má manažerské právo ke schvalování
	  SELECT @o28permflag=dbo.o28_get_permflag(@j02id_sys,@p41id,@j18id,@p34id,3,3)

	  if @o28permflag=3
		set @is_access_approve=1
	 end
end	----konec ovìøování práv podle projektové role

if @is_access_read=1
 set @record_disposition=1	---právo èíst záznam


if @is_access_edit=1
 set @record_disposition=2	---právo èíst + editovat záznam


if @is_access_approve=1
 set @record_disposition=3	---právo èíst + schvalovat záznam


if @is_access_edit=1 and @is_access_approve=1
 set @record_disposition=4	---nejvyšší právo: èíst + editovat + schvalovat záznam



GO

----------P---------------p31_remove_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_remove_invoice') and type = 'P')
 drop procedure p31_remove_invoice
GO







CREATE procedure [dbo].[p31_remove_invoice]
@p91id int
,@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---vyjmutí worksheet záznamù z faktury @p91id
---vstupní úkony musí být uloženy v TEMPu - p85TempBox
---p85Prefix='p31'
---p31id - p85DataPID

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')



if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if @p91id=0
  set @err_ret='Chybí faktura @p91id!'

if @err_ret<>''
 return


update p31WorkSheet set p91id=null,p70id=null
,p31Rate_Billing_Invoiced=null,p31Minutes_Invoiced=null,p31Hours_Invoiced=null,p31HHMM_Invoiced=null
,p31Value_Invoiced=null,p31Amount_WithoutVat_Invoiced=null,p31Amount_WithVat_Invoiced=null
,p31Amount_Vat_Invoiced=null,p31VatRate_Invoiced=null
,p31ExchangeRate_Domestic=null,p31ExchangeRate_Invoice=null,p31ExchangeRate_InvoiceManual=null
,j27ID_Billing_Invoiced=null,j27ID_Billing_Invoiced_Domestic=null
,p31IsInvoiceManual=0,j02ID_InvoiceManual=null
where p31ID IN (select p85DataPID FROM p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31')

exec p91_recalc_amount @p91id


































GO

----------P---------------p31_save_approving-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_save_approving') and type = 'P')
 drop procedure p31_save_approving
GO





CREATE  procedure [dbo].[p31_save_approving]
@p31id int
,@j03id_sys int
,@p71id int
,@p72id int
,@value_approved_internal float
,@value_approved_billing float
,@rate_billing_approved float
,@rate_internal_approved float
,@p31text nvarchar(2000)
,@vatrate_approved float
,@err_ret varchar(1000) OUTPUT

AS

set @err_ret=''
---exec p31_testbeforesave_approving @p31id,@j03id_sys,@p71id,@p72id,@hours_approved,@value_approved_billing,@amount_withoutvat_approved,@rate_billing_approved,@err_ret OUTPUT

if @err_ret<>''
 return


set @vatrate_approved=isnull(@vatrate_approved,0)

----validace sazby dph------------------
declare @vatisok bit,@p31date datetime,@p33code varchar(10),@p41id int,@j27id_orig int,@j02id_sys int,@p32id int

select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

select @p31date=p31date,@p33code=p33Code,@p41id=a.p41id,@j27id_orig=a.j27ID_Billing_Orig,@p32id=p32.p32ID
from p31worksheet a inner join p41project b on a.p41id=b.p41id
left outer join p32activity p32 on a.p32id=p32.p32id
left outer join p34activitygroup p34 on p32.p34id=p34.p34id
left outer join p33ActivityInputType p33 on p34.p33id=p33.p33id
where a.p31id=@p31id

if @p33code='M'
 begin
  select @vatrate_approved=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
 end

if @p33code='MV'	--DPH sazba se u schvalování testuje jenom u finanèních worksheetù
 begin
  select @vatisok=dbo.p31_testvat(@vatrate_approved,@p41id,@p31date,@j27id_orig)
  if @vatisok=0
    set @err_ret='Sazba DPH ['+convert(varchar(10),@vatrate_approved)+'%] není platná pro dané období, projekt a mìnu!'
 end
-----------------------------------------

if @err_ret<>''
  return


if @p72id=2 or @p72id=6 or @p72id=3
 begin
  set @rate_billing_approved=0		---odpis nebo paušál má fakturaèní sazbu celkovou cenu nulovou

   set @value_approved_billing=0
 end
 
if @p72id=6		--zahrnout do paušálu - schválená hodnota je pùvodní hodnota
 begin
  select @value_approved_billing=p31value_approved_billing from p31WorkSheet where p31ID=@p31id
 end 
 
if @p72id=2 or @p72id=3		--odpis nuluje schválené hodnoty
 begin
   set @value_approved_billing=0
 end 

declare @minutes int,@hours float,@hours_internal float,@minutes_internal int
set @hours=0
set @hours_internal=0
set @minutes_internal=0

if @p33code='T'
 begin
  set @hours=@value_approved_billing
  set @minutes=@value_approved_billing*60
  
  set @hours_internal=@value_approved_internal
  set @minutes_internal=@value_approved_internal*60
 end
 
 
update p31worksheet set p71id=@p71id,p72ID_AfterApprove=@p72id,j02ID_ApprovedBy=@j02id_sys,p31Approved_When=getdate()
where p31id=@p31id

if @p71id=2 or @p71id=3
  begin	---neschváleno - vynulovat hodnoty
	set @hours=0
	set @minutes=0
	set @value_approved_billing=0
	set @hours_internal=0
	set @minutes_internal=0
  end 


if @p71id is not null
 begin
   if @p33code='T'
 	update p31worksheet set p31Minutes_Approved_Billing=@minutes,p31HHMM_Approved_Billing=dbo.Minutes2HHMM(@minutes),p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@hours,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@hours*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@hours+@rate_billing_approved*@hours*@vatrate_approved/100
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id=4 then @hours else 0 end
	,p31Hours_Approved_Billing=@hours,p31Hours_Approved_Internal=@hours_internal
	where p31id=@p31id

   if @p33code='U'
 	update p31worksheet set p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@value_approved_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@value_approved_billing,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@value_approved_billing+@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id =4 then @value_approved_billing else 0 end
	where p31id=@p31id

   if @p33code='M' or @p33code='MV'
   begin
 	update p31worksheet set p31Amount_WithoutVat_Approved=@value_approved_billing,p31vatrate_approved=@vatrate_approved
	,p31amount_withvat_approved=(case when p31vatrate_orig<>@vatrate_approved or @value_approved_billing<>p31amount_withoutvat_orig then @value_approved_billing+@value_approved_billing*@vatrate_approved/100 else p31amount_withvat_orig end)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id=4 then @value_approved_billing else 0 end
	where p31id=@p31id
   end

   update p31worksheet set p31Amount_Vat_Approved=p31amount_withvat_approved-p31amount_withoutvat_approved
   where p31id=@p31id

   if isnull(@p31text,'')<>''
    	update p31worksheet set p31text=@p31text where p31id=@p31id
 end
else
 begin
 	update p31worksheet set p31Value_Approved_Billing=null,p31Value_Approved_Internal=null
	,p31Minutes_Approved_Billing=null,p31HHMM_Approved_Billing=null,p31Hours_Approved_Billing=null,p31Hours_Approved_Internal=null
	,p31Rate_Billing_Approved=null,p31Rate_Internal_Approved=null
 	,p31Amount_WithoutVat_Approved=null,p31Amount_WithVat_Approved=null,p31Amount_Vat_Approved=null,p31VatRate_Approved=null
 	,j02ID_ApprovedBy=null,p31approved_when=null
 	where p31id=@p31id
 end

































































GO

----------P---------------p31_save_approving_temp-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_save_approving_temp') and type = 'P')
 drop procedure p31_save_approving_temp
GO







CREATE  procedure [dbo].[p31_save_approving_temp]
@guid varchar(50)
,@p31id int
,@j03id_sys int
,@p71id int
,@p72id int
,@value_approved_internal float
,@value_approved_billing float
,@rate_billing_approved float
,@rate_internal_approved float
,@p31text nvarchar(2000)
,@vatrate_approved float
,@err_ret varchar(1000) OUTPUT

AS

set @err_ret=''
---exec p31_testbeforesave_approving @p31id,@j03id_sys,@p71id,@p72id,@hours_approved,@value_approved_billing,@amount_withoutvat_approved,@rate_billing_approved,@err_ret OUTPUT
if isnull(@guid,'')='' or @p31id is null
 set @err_ret='Na vstupu je prázdný @guid nebo @p31id.'

if @err_ret<>''
 return

exec [p31_setup_temp] @p31id,@guid

set @vatrate_approved=isnull(@vatrate_approved,0)

----validace sazby dph------------------
declare @vatisok bit,@p31date datetime,@p33code varchar(10),@p41id int,@j27id_orig int,@j02id_sys int,@p32id int

select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

select @p31date=p31date,@p33code=p33Code,@p41id=a.p41id,@j27id_orig=a.j27ID_Billing_Orig,@p32id=p32.p32ID
from p31worksheet a inner join p41project b on a.p41id=b.p41id
left outer join p32activity p32 on a.p32id=p32.p32id
left outer join p34activitygroup p34 on p32.p34id=p34.p34id
left outer join p33ActivityInputType p33 on p34.p33id=p33.p33id
where a.p31id=@p31id

if @p33code='M'
 begin
  select @vatrate_approved=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
 end

if @p33code='MV'	--DPH sazba se u schvalování testuje jenom u finanèních worksheetù
 begin
  select @vatisok=dbo.p31_testvat(@vatrate_approved,@p41id,@p31date,@j27id_orig)
  if @vatisok=0
    set @err_ret='Sazba DPH ['+convert(varchar(10),@vatrate_approved)+'%] není platná pro dané období, projekt a mìnu!'
 end
-----------------------------------------

if @err_ret<>''
  return


if @p72id=2 or @p72id=6 or @p72id=3
 begin
  set @rate_billing_approved=0		---odpis nebo paušál má fakturaèní sazbu celkovou cenu nulovou

   set @value_approved_billing=0
 end
 
------if @p72id=6		--zahrnout do paušálu - schválená hodnota je pùvodní hodnota
------ begin
------  select @value_approved_billing=p31value_approved_billing from p31WorkSheet where p31ID=@p31id
------ end 
 
------if @p72id=2 or @p72id=3		--odpis nuluje schválené hodnoty
------ begin
------   set @value_approved_billing=0
------ end 

declare @minutes int,@hours float,@hours_internal float,@minutes_internal int
set @hours=0
set @hours_internal=0
set @minutes_internal=0

if @p33code='T'
 begin
  set @hours=@value_approved_billing
  set @minutes=@value_approved_billing*60
  
  set @hours_internal=@value_approved_internal
  set @minutes_internal=@value_approved_internal*60
 end
 
 
update p31worksheet_temp set p71id=@p71id,p72ID_AfterApprove=@p72id,j02ID_ApprovedBy=@j02id_sys,p31Approved_When=getdate()
where p31GUID=@guid AND p31id=@p31id

if @p71id=2 or @p71id=3
  begin	---neschváleno - vynulovat hodnoty
	set @hours=0
	set @minutes=0
	set @value_approved_billing=0
	set @hours_internal=0
	set @minutes_internal=0
  end 



if @p71id is not null
 begin
   if @p33code='T'
 	update p31worksheet_Temp set p31Minutes_Approved_Billing=@minutes,p31HHMM_Approved_Billing=dbo.Minutes2HHMM(@minutes),p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@hours,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@hours*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@hours+@rate_billing_approved*@hours*@vatrate_approved/100
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id=4 then @hours else 0 end
	,p31Hours_Approved_Billing=@hours,p31Hours_Approved_Internal=@hours_internal
	where p31GUID=@guid AND p31id=@p31id

   if @p33code='U'
 	update p31Worksheet_Temp set p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@value_approved_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@value_approved_billing,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@value_approved_billing+@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id =4 then @value_approved_billing else 0 end
	where p31GUID=@guid AND p31id=@p31id

   if @p33code='M' or @p33code='MV'
   begin
 	update p31worksheet_Temp set p31Amount_WithoutVat_Approved=@value_approved_billing,p31vatrate_approved=@vatrate_approved
	,p31amount_withvat_approved=(case when p31vatrate_orig<>@vatrate_approved or @value_approved_billing<>p31amount_withoutvat_orig then @value_approved_billing+@value_approved_billing*@vatrate_approved/100 else p31amount_withvat_orig end)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id=4 then @value_approved_billing else 0 end
	where p31GUID=@guid AND p31id=@p31id
   end

   update p31Worksheet_Temp set p31Amount_Vat_Approved=p31amount_withvat_approved-p31amount_withoutvat_approved
   where p31GUID=@guid AND  p31id=@p31id

   if isnull(@p31text,'')<>''
    	update p31Worksheet_Temp set p31text=@p31text where p31GUID=@guid AND p31id=@p31id
 end
else
 begin
 	update p31Worksheet_Temp set p31Value_Approved_Billing=null,p31Value_Approved_Internal=null
	,p31Minutes_Approved_Billing=null,p31HHMM_Approved_Billing=null,p31Hours_Approved_Billing=null,p31Hours_Approved_Internal=null
	,p31Rate_Billing_Approved=null,p31Rate_Internal_Approved=null
 	,p31Amount_WithoutVat_Approved=null,p31Amount_WithVat_Approved=null,p31Amount_Vat_Approved=null,p31VatRate_Approved=null
 	,j02ID_ApprovedBy=null,p31approved_when=null
 	where p31GUID=@guid AND p31id=@p31id
 end


































































GO

----------P---------------p31_setup_temp-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_setup_temp') and type = 'P')
 drop procedure p31_setup_temp
GO








CREATE procedure [dbo].[p31_setup_temp]
@p31id int	---p31id
,@guid varchar(50)
AS

if exists(select p31ID FROM p31Worksheet_Temp WHERE p31GUID=@guid AND p31ID=@p31id)
 return ---temp data se na úvod plní pouze jednou

INSERT INTO p31Worksheet_Temp
(
[p31GUID]
	  ,[p31ID]
      ,[p41ID]
      ,[j02ID]
      ,[p32ID]
      ,[p56ID]
      ,[j02ID_Owner]
      ,[j02ID_ApprovedBy]
      ,[p31Code]
      ,[p70ID]
      ,[p71ID]
      ,[p72ID_AfterApprove]
      ,[p72ID_AfterTrimming]
      ,[j27ID_Billing_Orig]
      ,[j27ID_Billing_Invoiced]
      ,[j27ID_Billing_Invoiced_Domestic]
      ,[j27ID_Internal]
      ,[p91ID]
      ,[c11ID]
      ,[p31Date]
      ,[p31DateUntil]
      ,[p31HoursEntryFlag]
      ,[p31Approved_When]
      ,[p31IsPlanRecord]
      ,[p31Text]
      ,[p31Value_Orig]
      ,[p31Value_Trimmed]
      ,[p31Value_Approved_Billing]
      ,[p31Value_Approved_Internal]
      ,[p31Value_Invoiced]
      ,[p31Amount_WithoutVat_Orig]
      ,[p31Amount_WithVat_Orig]
      ,[p31Amount_Vat_Orig]
      ,[p31VatRate_Orig]
      ,[p31Amount_WithoutVat_FixedCurrency]
      ,[p31Amount_WithoutVat_Invoiced]
      ,[p31Amount_WithVat_Invoiced]
      ,[p31Amount_Vat_Invoiced]
      ,[p31VatRate_Invoiced]
      ,[p31Amount_WithoutVat_Invoiced_Domestic]
      ,[p31Amount_WithVat_Invoiced_Domestic]
      ,[p31Amount_Vat_Invoiced_Domestic]
      ,[p31Minutes_Orig]
      ,[p31Minutes_Trimmed]
      ,[p31Minutes_Approved_Billing]
      ,[p31Minutes_Approved_Internal]
      ,[p31Minutes_Invoiced]
      ,[p31HHMM_Orig]
      ,[p31HHMM_Trimmed]
      ,[p31HHMM_Approved_Billing]
      ,[p31HHMM_Approved_Internal]
      ,[p31HHMM_Invoiced]
      ,[p31Rate_Billing_Orig]
      ,[p31Rate_Internal_Orig]
      ,[p31Rate_Billing_Approved]
      ,[p31Rate_Internal_Approved]
      ,[p31Rate_Billing_Invoiced]
      ,[p31Amount_WithoutVat_Approved]
      ,[p31Amount_WithVat_Approved]
      ,[p31Amount_Vat_Approved]
      ,[p31VatRate_Approved]
      ,[p31Amount_Internal]
      ,[p31Amount_Internal_Approved]
      ,[p31ExchangeRate_Domestic]
      ,[p31ExchangeRate_Invoice]
      ,[p31DateTimeFrom_Orig]
      ,[p31DateTimeUntil_Orig]
      ,[p31Value_Orig_Entried]
      ,[p31DateInsert]
      ,[p31UserInsert]
      ,[p31DateUpdate]
      ,[p31UserUpdate]
      ,[p31ValidFrom]
      ,[p31ValidUntil]
	  ,[p31Hours_Orig]
      ,[p31Hours_Trimmed]
      ,[p31Hours_Approved_Billing]
      ,[p31Hours_Approved_Internal]
      ,[p31Hours_Invoiced]
)
SELECT @guid
	  ,[p31ID]
      ,[p41ID]
      ,[j02ID]
      ,[p32ID]
      ,[p56ID]
      ,[j02ID_Owner]
      ,[j02ID_ApprovedBy]
      ,[p31Code]
      ,[p70ID]
      ,[p71ID]
      ,[p72ID_AfterApprove]
      ,[p72ID_AfterTrimming]
      ,[j27ID_Billing_Orig]
      ,[j27ID_Billing_Invoiced]
      ,[j27ID_Billing_Invoiced_Domestic]
      ,[j27ID_Internal]
      ,[p91ID]
      ,[c11ID]
      ,[p31Date]
      ,[p31DateUntil]
      ,[p31HoursEntryFlag]
      ,[p31Approved_When]
      ,[p31IsPlanRecord]
      ,[p31Text]
      ,[p31Value_Orig]
      ,[p31Value_Trimmed]
      ,[p31Value_Approved_Billing]
      ,[p31Value_Approved_Internal]
      ,[p31Value_Invoiced]
      ,[p31Amount_WithoutVat_Orig]
      ,[p31Amount_WithVat_Orig]
      ,[p31Amount_Vat_Orig]
      ,[p31VatRate_Orig]
      ,[p31Amount_WithoutVat_FixedCurrency]
      ,[p31Amount_WithoutVat_Invoiced]
      ,[p31Amount_WithVat_Invoiced]
      ,[p31Amount_Vat_Invoiced]
      ,[p31VatRate_Invoiced]
      ,[p31Amount_WithoutVat_Invoiced_Domestic]
      ,[p31Amount_WithVat_Invoiced_Domestic]
      ,[p31Amount_Vat_Invoiced_Domestic]
      ,[p31Minutes_Orig]
      ,[p31Minutes_Trimmed]
      ,[p31Minutes_Approved_Billing]
      ,[p31Minutes_Approved_Internal]
      ,[p31Minutes_Invoiced]
      ,[p31HHMM_Orig]
      ,[p31HHMM_Trimmed]
      ,[p31HHMM_Approved_Billing]
      ,[p31HHMM_Approved_Internal]
      ,[p31HHMM_Invoiced]
      ,[p31Rate_Billing_Orig]
      ,[p31Rate_Internal_Orig]
      ,[p31Rate_Billing_Approved]
      ,[p31Rate_Internal_Approved]
      ,[p31Rate_Billing_Invoiced]
      ,[p31Amount_WithoutVat_Approved]
      ,[p31Amount_WithVat_Approved]
      ,[p31Amount_Vat_Approved]
      ,[p31VatRate_Approved]
      ,[p31Amount_Internal]
      ,[p31Amount_Internal_Approved]
      ,[p31ExchangeRate_Domestic]
      ,[p31ExchangeRate_Invoice]
      ,[p31DateTimeFrom_Orig]
      ,[p31DateTimeUntil_Orig]
      ,[p31Value_Orig_Entried]
      ,[p31DateInsert]
      ,[p31UserInsert]
      ,[p31DateUpdate]
      ,[p31UserUpdate]
      ,[p31ValidFrom]
      ,[p31ValidUntil]
	  ,[p31Hours_Orig]
      ,[p31Hours_Trimmed]
      ,[p31Hours_Approved_Billing]
      ,[p31Hours_Approved_Internal]
      ,[p31Hours_Invoiced]
  FROM [dbo].[p31Worksheet]
  WHERE p31ID=@p31id



GO

----------P---------------p31_test_beforesave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_test_beforesave') and type = 'P')
 drop procedure p31_test_beforesave
GO





CREATE procedure [dbo].[p31_test_beforesave]
@j03id_sys int
,@j02id_rec int
,@p41id int
,@p56id int
,@p31date datetime
,@p32id int
,@p31vatrate_orig float
,@j27id_explicit int
,@p31text nvarchar(2000)
,@value_orig float
,@err varchar(1000) OUTPUT
,@round2minutes int OUTPUT
,@j27id_domestic int OUTPUT
,@p33id int OUTPUT
,@vatrate float OUTPUT
AS

set @err=''
set @p31vatrate_orig=ISNULL(@p31vatrate_orig,0)
set @vatrate=0

if @p32id is null
 set @err='Na vstupu chybí aktivita.'

if @p41id is null
 set @err='Na vstupu chybí projekt.'

if @j02id_rec is null
 set @err='Na vstupu chybí osoba.'

if @err<>''
 return

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND p41WorksheetOperFlag=1)
 set @err='Projekt je uzavøený pro zapisování worksheet úkonù.'

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND (p41ValidFrom>getdate() OR p41ValidUntil<getdate()))
 set @err='Projekt byl pøesunut do koše, nelze do nìj zapisovat worksheet úkony.'

if @err<>''
 return

select @round2minutes=convert(int,x35Value)
FROM x35GlobalParam WHERE x35Key LIKE 'Round2Minutes' AND ISNUMERIC(x35Value)=1

select @j27id_domestic=convert(int,x35Value)
FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic' AND ISNUMERIC(x35Value)=1


declare @islocked bit,@p34id int,@isplan bit,@person nvarchar(300),@j07id_rec int,@p32IsTextRequired bit,@p32name nvarchar(200)
declare @p32Value_Maximum float,@p32Value_Minimum float

select @person=j02LastName+' '+isnull(j02FirstName,''),@j07id_rec=isnull(j07id,-1) from j02Person where j02ID=@j02id_rec

select @p34id=a.p34id,@p33id=b.p33id,@isplan=b.p34iscapacityplan,@p32IsTextRequired=a.p32IsTextRequired,@p32name=a.p32Name
,@p32Value_Maximum=isnull(a.p32Value_Maximum,0),@p32Value_Minimum=isnull(a.p32Value_Minimum,0)
from p32Activity a inner join p34ActivityGroup b on a.p34ID=b.p34id
where a.p32ID=@p32id

if isnull(ltrim(rtrim(@p31text)),'')='' and @p32IsTextRequired=1
 begin
  set @err='Pro aktivitu ['+@p32name+'] je povinné zadávat podrobný popis úkonu.'
  return
 end

if @p32Value_Minimum<>0 and @value_orig<=@p32Value_Minimum
 begin
  set @err='Pro aktivitu ['+@p32name+'] musí být vykázaná hodnota vìtší než: '+convert(varchar(10),@p32Value_Minimum)
  if @p33id=1
   set @err=@err+'h.'

  return
 end

if @p32Value_Maximum<>0 and @value_orig>=@p32Value_Maximum
 begin
  set @err='Pro aktivitu ['+@p32name+'] musí být vykázaná hodnota menší než: '+convert(varchar(10),@p32Value_Maximum)
  if @p33id=1
   set @err=@err+'h.'

  return
 end

---test sazby DPH------------
if @p33id=5 ----testuje se pouze money sešit s plným rozpisem DPH
 begin
  declare @vatisok bit
  
  select @vatisok=dbo.p31_testvat(@p31vatrate_orig,@p41id,@p31date,@j27id_explicit)
  
  if @vatisok=0
   set @err='Sazba DPH ['+convert(varchar(30),@p31vatrate_orig)+'%] není povolena pro tento projekt, mìnu a období!'
 

 end

if @err<>''
 return

if @p33id=1 or @p33id=3 or @p33id=2  ----pro èas a kusovník a èástku bez rozpisu
 select @vatrate=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)



if @isplan=0
 begin
  --test uzamèeného období-----------
  exec p31_test_lockedperiod @j03id_sys,@p31date,@j02id_rec, @p34id, @islocked OUTPUT 


  if @islocked=1
    set @err='Datum ['+convert(varchar(30),@p31date,104)+'] patøí do uzamèeného období!'
 end
 
if @err<>''
 return
   
---test oprávnìní zapisovat do projektu worksheet---------
declare @o28id int,@o28entryflag int,@x69id int

select @o28id=a.o28id,@o28entryflag=a.o28entryflag
from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
where a.p34ID=@p34id AND x69.x69RecordPID=@p41id AND x67.x29ID=141
and (isnull(x69.j02ID,0)=@j02id_rec OR isnull(x69.j07ID,0)=@j07id_rec OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec))
AND (a.o28entryflag>0)
ORDER BY a.o28entryflag

declare @j18id int
select @j18id=j18ID FROM p41Project WHERE p41ID=@p41id

if @o28id is null and @j18id is not null
 begin ----------oprávnìní k projektu podle projektové skupiny (regionu)
  select @o28id=a.o28id,@o28entryflag=a.o28entryflag
  from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
  inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  where a.p34ID=@p34id AND x69.x69RecordPID=@j18id AND x67.x29ID=118
  and (isnull(x69.j02ID,0)=@j02id_rec OR isnull(x69.j07ID,0)=@j07id_rec OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec))
  AND (a.o28entryflag>0)
  
 end

if @o28id is null
 begin  
  set @err='Osoba ['+@person+'] nemá v tomto projektu nebo v pøíslušné projektové skupinì pøiøazenou roli k zapisování worksheet úkonù do sešitu ['+dbo.GetObjectAlias('p34',@p34id)+']'

 end
   
if @err<>''
 return  
 
if @o28entryflag=1
 return	--OK - právo zapisovat do projektu i všech úloh 
 
if isnull(@o28entryflag,0)=0
 begin
   set @err='Projektová role osoby ['+@person+'] nemá povoleno zapisovat worksheet do zvoleného sešitu'
   return
 end
 
if @o28entryflag=2 and @p56id is null
 return	  --OK - právo zapisovat do projektu pøímo nebo do úloh s WR
 
if @o28entryflag=3 and @p56id is null
 begin
   set @err='Projektová role osoby ['+@person+'] má povoleno zapisovat pouze do projektových úloh.'
   return
 end 
 
if @p56id is null
 begin
   set @err='Musíte vybrat úlohu.'
   return
 end 

--situace, kdy se zapisuje úkon do úlohy a osoba nemá právo zapisovat do projektu 
SELECT @x69id=a.x69ID
from x69EntityRole_Assign a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
WHERE a.x69RecordPID=@p56id and x67.x29ID=356
AND (
 isnull(a.j02ID,0)=@j02id_rec
 OR isnull(a.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec)
 OR isnull(a.j07ID,0)=@j07id_rec
)
  

if @x69id is null 
  set @err='Nejste øešitelem úlohy ['+dbo.GetObjectAlias('p56',@p56id)+'] a proto nemùžete zapisovat worksheet do zvoleného sešitu.'
  
 




GO

----------P---------------p31_test_lockedperiod-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_test_lockedperiod') and type = 'P')
 drop procedure p31_test_lockedperiod
GO







CREATE procedure [dbo].[p31_test_lockedperiod]
@j03id_sys int,@dat datetime,@j02id_rec int,@p34id int,@islocked bit OUTPUT

AS
   
  declare @p36id int,@j02id_sys int
  
  select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

  set @islocked=1
    
  if not exists(select p36id from p36LockPeriod)
   begin
    set @islocked=0
    return
   end


  ---zamknuto pro všechny osoby a všechny sheety-----
  select @p36id=p36id FROM p36LockPeriod
  where @dat BETWEEN p36DateFrom AND p36DateUntil AND p36IsAllSheets=1

  if isnull(@p36id,0)<>0
     return


  ---zamknuto pro konkrétní osobu nebo tým a všechny sheety-------
  select @p36id=p36id FROM p36LockPeriod
  where @dat BETWEEN p36DateFrom AND p36DateUntil and p36IsAllSheets=1
  and (j02ID=@j02id_sys OR j02ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_sys))

  if isnull(@p36id,0)<>0
     return


---zamknuto pro všechny uživatele a konkrétní sheety-----
  select @p36id=p36id FROM p36LockPeriod
  where @dat BETWEEN p36DateFrom AND p36DateUntil AND p36IsAllPersons=1
  and p36id in (select p36id from p37LockPeriod_Sheet where p34id=@p34id)

  if isnull(@p36id,0)<>0
     return


---zamknuto pro konkrétní osobu/tým a konkrétní sheety-------
  select @p36id=p36id FROM p36LockPeriod
  where @dat BETWEEN p36DateFrom AND p36DateUntil
  and (j02ID=@j02id_sys OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_sys))
  AND p36ID IN (select p36id from p37LockPeriod_Sheet where p34id=@p34id)

  if isnull(@p36id,0)<>0
     return
  

  ---zde už je jasné, že období zamknuté není
  set @islocked=0











GO

----------P---------------p32_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p32_delete') and type = 'P')
 drop procedure p32_delete
GO







CREATE   procedure [dbo].[p32_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p32id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p51PriceList
declare @ref_pid int

if exists(select p31ID FROM p31Worksheet where p32ID=@pid)
 set @err_ret='Minimálnì jeden worksheet záznam má vazbu na tuto aktivitu.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p51ID from p52PriceList_Item WHERE p32ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden ceník sazeb má vazbu na tuto aktivitu ('+dbo.GetObjectAlias('p51',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY

	delete from p32Activity WHERE p32ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p34_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p34_delete') and type = 'P')
 drop procedure p34_delete
GO








CREATE   procedure [dbo].[p34_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p34id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky p34activitygroup
declare @ref_pid int

SELECT TOP 1 @ref_pid=p32ID from p32Activity WHERE p34ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna aktivita má vazbu na tento sešit ('+dbo.GetObjectAlias('p32',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p43ProjectType_Workload where p34ID=@pid

	DELETE FROM o28ProjectRole_Workload WHERE p34ID=@pid

	delete from p34ActivityGroup where p34ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p36_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p36_delete') and type = 'P')
 drop procedure p36_delete
GO








CREATE   procedure [dbo].[p36_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p36id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu  z tabulky p36LockPeriod


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p37LockPeriod_Sheet WHERE p36ID=@pid

	delete from p36LockPeriod where p36ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p41_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_aftersave') and type = 'P')
 drop procedure p41_aftersave
GO






CREATE    PROCEDURE [dbo].[p41_aftersave]
@p41id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu projektu
declare @p41code varchar(50),@x38id int

select @p41code=p41Code,@x38id=p42.x38ID
FROM
p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID
WHERE a.p41ID=@p41id

if left(@p41code,4)='TEMP' OR @p41code is null
 begin
  set @p41code=dbo.x38_get_freecode(@x38id,141,@p41id,1)
  if @p41code<>''
   UPDATE p41Project SET p41Code=@p41code WHERE p41ID=@p41id 
 end 

exec [x90_appendlog] 141,@p41id,@j03id_sys




GO

----------P---------------p41_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_delete') and type = 'P')
 drop procedure p41_delete
GO








CREATE   procedure [dbo].[p41_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p41id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu projektu z tabulky p41Project
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE p41ID=@pid
if @ref_pid is not null
 set @err_ret='Do projektu byl zapsán minimálnì jeden worksheet úkon.'


set @ref_pid=null
SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p41ID=@pid
if @ref_pid is not null
 set @err_ret='K projektu je vytvoøen minimálnì jeden úkol ('+dbo.GetObjectAlias('p56',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p41ID=@pid)
	 DELETE FROM o27Attachment WHERE p41ID=@pid

	if exists(SELECT o23ID FROM o23Notepad WHERE p41ID=@pid)
	 DELETE FROM o23Notepad WHERE p41ID=@pid

	if exists(SELECT o22ID FROM o22Milestone WHERE p41ID=@pid)
	 DELETE FROM o22Milestone WHERE p41ID=@pid

	if exists(select p41ID FROM p41Project_FreeField WHERE p41ID=@pid)
	 DELETE FROM p41Project_FreeField WHERE p41ID=@pid

	if exists(select o39ID FROM o39Project_Address WHERE p41ID=@pid)
	 DELETE FROM o39Project_Address WHERE p41ID=@pid

	if exists(select p46ID FROM p46Project_Person WHERE p41ID=@pid)
	 DELETE FROM p46Project_Person WHERE p41ID=@pid

	if exists(select p47ID FROM p47CapacityPlan WHERE p41ID=@pid)
	 DELETE FROM p47CapacityPlan WHERE p41ID=@pid

	if exists(select p48ID FROM p48OperativePlan WHERE p41ID=@pid)
	 DELETE FROM p48OperativePlan WHERE p41ID=@pid

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=141)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=141

	
	DELETE FROM x90EntityLog WHERE x29ID=141 AND x90RecordPID=@pid


	delete from p41Project WHERE p41ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p41_inhale_sumrow-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_inhale_sumrow') and type = 'P')
 drop procedure p41_inhale_sumrow
GO





CREATE procedure [dbo].[p41_inhale_sumrow]
@j03id_sys int
,@pid int						---p41id
,@p56_actual_count int OUTPUT	--_poèet otevøených úkolù v projektu
,@o22_actual_count int OUTPUT	--poèet otevøených termínù v projektu
,@p91_count int OUTPUT			--poèet vystavených faktur
AS

set @p56_actual_count=0
set @o22_actual_count=0
set @p91_count=0


SELECT @p56_actual_count=COUNT(*)
FROM p56Task
WHERE p41ID=@pid AND getdate() BETWEEN p56ValidFrom AND p56ValidUntil

SELECT @o22_actual_count=COUNT(*)
FROM o22Milestone
WHERE p41ID=@pid AND o22DateUntil>=dateadd(day,-2,getdate()) AND getdate() BETWEEN o22ValidFrom AND o22ValidUntil

SELECT @p91_count=COUNT(*)
from p91Invoice
WHERE p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p41ID=@pid)

GO

----------P---------------p42_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p42_delete') and type = 'P')
 drop procedure p42_delete
GO








CREATE   procedure [dbo].[p42_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p42id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p42projecttype
declare @ref_pid int

SELECT TOP 1 @ref_pid=p41ID from  p41Project WHERE p42ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden projekt má vazbu na tento typ ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p43ProjectType_Workload WHERE p42ID=@pid

	DELETE from p42ProjectType where p42ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p48_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p48_delete') and type = 'P')
 drop procedure p48_delete
GO









CREATE   procedure [dbo].[p48_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p48id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu operativního plánu z tabulky p48OperativePlan


BEGIN TRANSACTION

BEGIN TRY

	delete from p48OperativePlan WHERE p48ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------p51_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p51_delete') and type = 'P')
 drop procedure p51_delete
GO







CREATE   procedure [dbo].[p51_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p51id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p51PriceList
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE p51ID_Billing=@pid OR p51ID_Internal=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden klient má vazbu na tento ceník ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE p51ID_Billing=@pid OR p51ID_Internal=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden projekt má vazbu na tento ceník ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY

	delete from p52PriceList_Item WHERE p51ID=@pid

	delete from p51PriceList where p51ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p53_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p53_delete') and type = 'P')
 drop procedure p53_delete
GO







CREATE   procedure [dbo].[p53_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p53id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu dph sazby z tabulky p53VatRate


BEGIN TRANSACTION

BEGIN TRY

	delete from p53VatRate WHERE p53ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p56_aftersave-------------------------

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








GO

----------P---------------p56_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_delete') and type = 'P')
 drop procedure p56_delete
GO






CREATE   procedure [dbo].[p56_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p56id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu úkolu z tabulky p56Task
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE p56ID=@pid
if @ref_pid is not null
 set @err_ret='K úkolu má vazbu minimálnì jeden worksheet úkon.'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(SELECT o27ID FROM o27Attachment WHERE p56ID=@pid)
	 DELETE FROM o27Attachment WHERE p56ID=@pid

	if exists(SELECT o23ID FROM o23Notepad WHERE p56ID=@pid)
	 DELETE FROM o23Notepad WHERE p56ID=@pid

	if exists(SELECT o22ID FROM o22Milestone WHERE p56ID=@pid)
	 DELETE FROM o22Milestone WHERE p56ID=@pid

	if exists(select p56ID FROM p56Task_FreeField WHERE p56ID=@pid)
	 DELETE FROM p56Task_FreeField WHERE p56ID=@pid

	

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=356)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=356

	
	DELETE FROM x90EntityLog WHERE x29ID=356 AND x90RecordPID=@pid


	delete from p56Task WHERE p56ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------p57_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p57_delete') and type = 'P')
 drop procedure p57_delete
GO








CREATE   procedure [dbo].[p57_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p57id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu instituce z tabulky p57TaskType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p56ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden úkol má vazbu na tento typ ('+dbo.GetObjectAlias('p56',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE from p57TaskType where p57ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p86_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p86_delete') and type = 'P')
 drop procedure p86_delete
GO









CREATE   procedure [dbo].[p86_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p86id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu bankovního úètu z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p88ID from p88InvoiceHeader_BankAccount WHERE p86ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna hlavièka vystavovatele faktur obsahuje vazbu na tento úèet.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	

	DELETE from p86BankAccount where p86ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------p91_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_aftersave') and type = 'P')
 drop procedure p91_aftersave
GO









CREATE    PROCEDURE [dbo].[p91_aftersave]
@p91id int
,@j03id_sys int
,@recalc_amount bit

AS

if exists(select p91ID FROM p91Invoice WHERE p91ID=@p91id AND (p91Code LIKE 'TEMP%' OR p91Code IS NULL))
 exec p91_update_code @p91id,@j03id_sys

---automaticky se spouští po uložení záznamu faktury
if @recalc_amount=1
 exec p91_recalc_amount @p91id









GO

----------P---------------p91_convertdraft-------------------------

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

----------P---------------p91_create-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_create') and type = 'P')
 drop procedure p91_create
GO








CREATE procedure [dbo].[p91_create]
@guid varchar(50)
,@j03id_sys int
,@p28id int
,@p92id int
,@p91isdraft bit
,@p91date datetime
,@p91datematurity datetime
,@p91datesupply datetime
,@p91datep31_from datetime
,@p91datep31_until datetime
,@p91text1 nvarchar(2000)
,@err_ret varchar(1000) OUTPUT
,@ret_p91id int OUTPUT

AS

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @guid=isnull(@guid,'')
set @p28id=isnull(@p28id,0)
set @p92id=isnull(@p92id,0)


if @j03id_sys=0 or @guid=''
 begin
  set @err_ret='@guid or @j03id_sys missing!'
  return
 end


if @p92id=0
  set @err_ret='Chybí typ faktury!'

if @p28id=0
  set @err_ret='Chybí klient (odbìratel) faktury!'

if @err_ret<>''
 return

declare @login nvarchar(50),@j02id_owner int
set @login=dbo.j03_getlogin(@j03id_sys)
select @j02id_owner=j02ID FROM j03User WHERE j03ID=@j03id_sys


declare @j27id int,@j19id int,@x15id int,@j17id int


select @j27id=j27id,@j19id=j19id,@x15id=x15id,@j17id=j17ID
from p92InvoiceType where p92id=@p92id  

if isnull(@j27id,0)=0
 begin
  if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Invoice')
   select @j27id=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Invoice'
  else
   set @j27id=2
 end

declare @code_temp varchar(50)
set @code_temp='TEMP'+@guid  

insert into p91invoice(p91code,p91dateinsert,p91userinsert,p91Date,p91DateSupply,p91DateMaturity,j02ID_Owner,p28ID,p92ID,j27ID) values(@code_temp,getdate(),@login,@p91date,@p91datesupply,@p91datematurity,@j02id_owner,@p28id,@p92id,@j27id)

SELECT @ret_p91id=@@IDENTITY

	
update p91invoice set p91IsDraft=@p91isdraft,j17ID=@j17id
,p91userupdate=@login,p91dateupdate=getdate()
,p91Text1=@p91text1
,p91Datep31_From=@p91datep31_from,p91Datep31_Until=@p91datep31_until,j19id=@j19id
FROM p91invoice
where p91id=@ret_p91id


declare @o38id_primary int,@o38id_delivery int,@p41id_first int

select top 1 @p41id_first=p41ID FROM p31Worksheet WHERE p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

select top 1 @o38id_primary=o38ID from o37Contact_Address WHERE o36ID=1 AND p28ID=@p28id
   
select top 1 @o38id_delivery=o38ID from o37Contact_Address WHERE o36ID=2 AND p28ID=@p28id

if @o38id_delivery is null
 set @o38id_delivery=@o38id_primary

update p91Invoice set o38ID_Primary=@o38id_primary,@o38id_delivery=o38id_delivery,p41ID_First=@p41id_first
where p91ID=@ret_p91id

 

declare @p91text2 nvarchar(1000)

select @p91text2=p41InvoiceDefaultText2 from p41Project WHERE p41ID=@p41id_first

if @p91text2 is null
 select @p91text2=p28InvoiceDefaultText2 from p28Contact where p28ID=@p28id

if @p91text2 is not null
 update p91Invoice set p91Text2=@p91text2 where p91ID=@ret_p91id

update p31worksheet set p91ID=@ret_p91id,p70id=p72ID_AfterApprove
WHERE p71id=1 and isnull(p72ID_AfterApprove,0)<>0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p91ID=@ret_p91id,p70id=(case when isnull(p31amount_withoutvat_approved,0)<>0 then 4 else 2 end)
WHERE p71id=1 and isnull(p72ID_AfterApprove,0)=0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')

update p31worksheet set p31Minutes_Invoiced=p31Minutes_Approved_Billing,p31Hours_Invoiced=p31Hours_Approved_Billing,p31HHMM_Invoiced=p31HHMM_Approved_Billing
,p31Value_Invoiced=p31Value_Approved_Billing
,p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved,p31VatRate_Invoiced=p31VatRate_Approved
,p31Rate_Billing_Invoiced=p31Rate_Billing_Approved
where p91ID=@ret_p91id



exec p91_recalc_amount @ret_p91id

if isnull(@x15id,0)<>0
 begin	---faktura má být kompletnì pøevedena do jednotné DPH
  declare @explicit_vatrate float,@j18id int

  
  set @explicit_vatrate=dbo.p91_get_vatrate(@x15id,@j27id,@j17id,@p91datesupply)
  
  
  exec p91_change_vat @ret_p91id,@j03id_sys,@x15id,@explicit_vatrate,null
  
 end



 exec [p91_update_code] @ret_p91id,@j03id_sys

 exec [p91_aftersave] @ret_p91id,@j03id_sys,0































GO

----------P---------------p91_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_delete') and type = 'P')
 drop procedure p91_delete
GO








CREATE   procedure [dbo].[p91_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p91id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu vystavené faktury z tabulky p91Invoice
declare @ref_pid int


if isnull(@err_ret,'')<>''
 return
 
declare @p92invoicetype int,@p92id int,@p91id_bind int,@p32id_overhead int

if exists(select x35ID FROM x35GlobalParam WHERE x35Key LIKE 'p32ID_Overhead')
  select @p32id_overhead=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'p32ID_Overhead'

select @p92invoicetype=p92invoicetype,@p91id_bind=p91ID_CreditNoteBind
FROM p91Invoice a inner join p92InvoiceType b on a.p92id=b.p92id
where a.p91id=@pid

BEGIN TRANSACTION

BEGIN TRY


if @p92invoicetype=2
 delete from p31WorkSheet where p91id=@pid	--u dobropisu se mažou zdrojové worksheet záznamy
 
if @p32id_overhead is not null
 begin
  if exists(select p31ID FROM p31Worksheet WHERE p91ID=@pid AND p32id=@p32id_overhead)
    delete FROM p31worksheet WHERE p91id=@pid AND p32id=@p32id_overhead
 end

update p31Worksheet set p91ID=null,p70ID=null
,j27ID_Billing_Invoiced=null,j27ID_Billing_Invoiced_Domestic=null,p31Value_Invoiced=null,p31Rate_Billing_Invoiced=null
,p31Amount_WithoutVat_Invoiced=null,p31Amount_WithVat_Invoiced=null,p31Amount_Vat_Invoiced=null,p31VatRate_Invoiced=null
,p31Amount_WithoutVat_Invoiced_Domestic=null,p31Amount_WithVat_Invoiced_Domestic=null,p31Amount_Vat_Invoiced_Domestic=null
,p31Minutes_Invoiced=null,p31HHMM_Invoiced=null,p31ExchangeRate_Invoice=null
where p91ID=@pid

if exists(select p94ID FROM p94Invoice_Payment where p91ID=@pid)
  delete from p94Invoice_Payment where p91id=@pid

if exists(select p96ID FROM p96Invoice_ExchangeRate where p91ID=@pid)
  delete from p96Invoice_ExchangeRate where p91id=@pid

if exists(select p91ID FROM p91Invoice_FreeField WHERE p91ID=@pid)
  delete from p91Invoice_FreeField where p91id=@pid

if exists(select p99ID FROM p99Invoice_Proforma WHERE p91ID=@pid)
  delete from p99Invoice_Proforma where p91id=@pid

if exists(SELECT o27ID FROM o27Attachment WHERE p91ID=@pid)
  DELETE FROM o27Attachment WHERE p91ID=@pid

if exists(SELECT o23ID FROM o23Notepad WHERE p91ID=@pid)
  DELETE FROM o23Notepad WHERE p91ID=@pid

if exists(SELECT o22ID FROM o22Milestone WHERE p91ID=@pid)
  DELETE FROM o22Milestone WHERE p91ID=@pid

if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=391)
  DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=391


DELETE FROM x90EntityLog WHERE x29ID=391 AND x90RecordPID=@pid


delete from p91Invoice where p91id=@pid

	
COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

--if @p91id_bind is not null
-- exec p91_recalc_amount @p91id_bind,0

















GO

----------P---------------p91_change_currency-------------------------

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

----------P---------------p91_change_vat-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_change_vat') and type = 'P')
 drop procedure p91_change_vat
GO







CREATE     procedure [dbo].[p91_change_vat]
@p91id int
,@j03id_sys int
,@x15id int
,@newvatrate float
,@err_ret varchar(1000) OUTPUT

AS



set @p91id=isnull(@p91id,0)

declare @login nvarchar(50)

set @login=dbo.j03_getlogin(@j03id_sys)

set @err_ret=''

if @newvatrate is null
  set @err_ret='New VAT rate must be number!'

if @j03id_sys=0
 set @err_ret='@j03id_sys is missing!'


if @err_ret<>''
  return

----validace sazby dph------------------
declare @vatisok bit,@p91DateSupply datetime,@j17id int,@j27id int

select @p91DateSupply=p91DateSupply,@j17id=j17id,@j27id=j27id from p91invoice where p91id=@p91id

select @vatisok=dbo.p91_test_vat(@newvatrate,@j27id,@j17id,@p91DateSupply)

if @vatisok=0
  set @err_ret='Sazba DPH ['+convert(varchar(10),@newvatrate)+'%] není platná pro dané datum plnìní, mìnu a zemi!'
-----------------------------------------

if @err_ret<>''
  return

update p91Invoice SET x15ID=@x15id,p91FixedVatRate=@newvatrate WHERE p91ID=@p91id

update p31worksheet set p31vatrate_approved=@newvatrate,p31vatrate_invoiced=@newvatrate
where p91id=@p91id

update p31worksheet set p31amount_vat_approved=p31amount_withoutvat_approved*p31vatrate_approved/100
where p91id=@p91id

update p31worksheet set p31amount_withvat_approved=p31amount_withoutvat_approved+p31amount_vat_approved
where p91id=@p91id

update p31worksheet set p31amount_vat_invoiced=p31amount_withoutvat_invoiced*p31vatrate_invoiced/100
where p91id=@p91id

update p31worksheet set p31amount_withvat_invoiced=p31amount_withoutvat_invoiced+p31amount_vat_invoiced
where p91id=@p91id

exec p91_recalc_amount @p91id


































GO

----------P---------------p91_recalc_amount-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_recalc_amount') and type = 'P')
 drop procedure p91_recalc_amount
GO







CREATE procedure [dbo].[p91_recalc_amount]
@p91id int

AS

declare @j27id_dest int,@datSupply datetime,@j27id_domestic int,@p92invoicetype int,@p92id int,@p41id_first int,@j17id int

select @j27id_dest=a.j27id,@datSupply=a.p91DateSupply,@p92id=a.p92id,@p92invoicetype=b.p92InvoiceType,@p41id_first=a.p41ID_First,@j17id=a.j17ID
from p91invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
where a.p91id=@p91id

declare @p41id_test int

select top 1 @p41id_test=p41id from p31worksheet where p91id=@p91id


--*****************mìnové kurzy*************************---------------
if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Domestic')
 select @j27id_domestic=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Domestic'
else
 set @j27id_domestic=2


declare @exchangedate datetime
if @j27id_domestic<>@j27id_dest
 begin
  select TOP 1 @exchangedate=m62date FROM m62ExchangeRate where j27id_master=@j27id_domestic and m62date<=@datSupply and j27id_slave=@j27id_dest order by m62date desc

  update p91invoice set p91DateExchange=@exchangedate,p91ExchangeRate=dbo.get_exchange_rate(1,p91DateSupply,j27id,@j27id_domestic)
  where p91id=@p91id
 end
else
 update p91invoice set p91DateExchange=null,p91ExchangeRate=1 where p91id=@p91id


----výchozí mìnový kurz pocházející z j27id worksheet záznamu-----------
update p31worksheet set p31ExchangeRate_Invoice=dbo.get_exchange_rate(1,@datSupply,j27ID_Billing_Orig,@j27id_dest)
WHERE p91id=@p91id

----mìnový kurz pro manuálnì upravované èástky ve faktuøe-----------
update p31worksheet set p31ExchangeRate_InvoiceManual=dbo.get_exchange_rate(1,@datSupply,j27ID_Billing_Invoiced,@j27id_dest)
WHERE p91id=@p91id and p31IsInvoiceManual=1


update p31worksheet set p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved*p31ExchangeRate_Invoice
,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved*p31ExchangeRate_Invoice
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved*p31ExchangeRate_Invoice
,p31rate_billing_invoiced=p31rate_billing_approved*p31ExchangeRate_Invoice
where p91id=@p91id and p31IsInvoiceManual=0

update p31worksheet set p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Invoiced*p31ExchangeRate_InvoiceManual
,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Invoiced*p31ExchangeRate_InvoiceManual
,p31Amount_Vat_Invoiced=p31Amount_Vat_Invoiced*p31ExchangeRate_InvoiceManual
,p31rate_billing_invoiced=p31rate_billing_invoiced*p31ExchangeRate_InvoiceManual
where p91id=@p91id and p31IsInvoiceManual=1

if exists(select p31id from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34activitygroup c on b.p34id=c.p34id where a.p91id=@p91id AND p31amount_withoutvat_invoiced*isnull(p31vatrate_invoiced,0)/100<>isnull(p31amount_vat_invoiced,0))
 begin  ---existují fakturované úkony, u kterých nesedí výpoèet DPH
  update p31worksheet set p31amount_vat_invoiced=p31amount_withoutvat_invoiced*p31vatrate_invoiced/100
  ,p31amount_withvat_invoiced=p31amount_withoutvat_invoiced+(p31amount_withoutvat_invoiced*p31vatrate_invoiced/100)
  WHERE p91id=@p91id
    
 end

update p31WorkSheet set p31Value_Invoiced=p31Amount_WithoutVat_Invoiced
where p91ID=@p91id and p32ID in (select p32ID from p32Activity a inner join p34ActivityGroup b on a.p34ID=b.p34ID where b.p33ID in (2,5))

update p31worksheet set j27ID_Billing_Invoiced=@j27id_dest
where p91id=@p91id and isnull(j27ID_Billing_Invoiced,0)<>@j27id_dest

----mìnový kurz z fakturaèní mìny do domácí mìny----------
update p31worksheet set j27ID_Billing_Invoiced_Domestic=@j27id_domestic, p31ExchangeRate_Domestic=dbo.get_exchange_rate(1,@datSupply,j27ID_Billing_Invoiced,@j27id_domestic)
WHERE p91id=@p91id

update p31worksheet set p31Amount_WithoutVat_Invoiced_Domestic=p31Amount_WithoutVat_Invoiced*p31ExchangeRate_Domestic
,p31Amount_WithVat_Invoiced_Domestic=p31Amount_WithVat_Invoiced*p31ExchangeRate_Domestic
,p31Amount_Vat_Invoiced_Domestic=p31Amount_Vat_Invoiced*p31ExchangeRate_Domestic
where p91id=@p91id

--***************èástky DPH***********************************--
declare @p91amount_withoutvat_none float

declare @p91amount_withoutVat_low float,@p91amount_withoutVat_standard float,@p91amount_withoutVat_special float
declare @p91amount_withVat_low float,@p91amount_withVat_standard float,@p91amount_withVat_special float
declare @p91amount_Vat_low float,@p91amount_Vat_standard float,@p91amount_Vat_special float
declare @p91vatrate_low float,@p91vatrate_standard float,@p91vatrate_special float


select @p91vatrate_low=dbo.p91_get_vatrate(2,@j27id_dest,@j17id,@datSupply)
select @p91vatrate_standard=dbo.p91_get_vatrate(3,@j27id_dest,@j17id,@datSupply)
select @p91vatrate_special=dbo.p91_get_vatrate(4,@j27id_dest,@j17id,@datSupply)


if @p92invoicetype=2	--dobropis
 begin  ---u dobropisu brát sazby DPH z pùvodní faktury   
   select @p91vatrate_low=p91vatrate_low,@p91vatrate_standard=p91vatrate_standard,@p91vatrate_special=p91vatrate_special
   FROM
   p91Invoice WHERE p91ID in (select p91ID_CreditNoteBind FROM p91invoice where p91id=@p91id)
 end

select @p91amount_withoutvat_none=sum(p31Amount_WithoutVat_Invoiced)
FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=0

if @p91vatrate_low>0
 begin
  select @p91amount_withoutVat_low=sum(p31Amount_WithoutVat_Invoiced),@p91amount_withVat_low=sum(p31Amount_WithVat_Invoiced),@p91amount_Vat_low=sum(p31Amount_Vat_Invoiced)
  FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=@p91vatrate_low
 end

if @p91vatrate_standard>0
 begin
  select @p91amount_withoutVat_standard=sum(p31Amount_WithoutVat_Invoiced),@p91amount_withVat_standard=sum(p31Amount_WithVat_Invoiced),@p91amount_Vat_standard=sum(p31Amount_Vat_Invoiced)
  FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=@p91vatrate_standard
 end


if @p91vatrate_special>0
 begin
  select @p91amount_withoutVat_special=sum(p31Amount_WithoutVat_Invoiced),@p91amount_withVat_special=sum(p31Amount_WithVat_Invoiced),@p91amount_Vat_special=sum(p31Amount_Vat_Invoiced)
  FROM p31worksheet where p91id=@p91id and p31VatRate_Invoiced=@p91vatrate_special
 end

set @p91amount_withoutvat_none=ROUND(@p91amount_withoutvat_none,2)
set @p91amount_withoutVat_low=ROUND(@p91amount_withoutVat_low,2)
set @p91amount_withVat_low=ROUND(@p91amount_withVat_low,2)
set @p91amount_Vat_low=ROUND(@p91amount_Vat_low,2)
set @p91amount_withoutVat_standard=ROUND(@p91amount_withoutVat_standard,2)
set @p91amount_withVat_standard=ROUND(@p91amount_withVat_standard,2)
set @p91amount_Vat_standard=ROUND(@p91amount_Vat_standard,2)

declare @p91amount_withoutVat float,@p91amount_withVat float,@p91amount_Vat float

set @p91amount_withoutvat_none=isnull(@p91amount_withoutvat_none,0)
set @p91amount_withoutVat_low=isnull(@p91amount_withoutVat_low,0)
set @p91amount_withoutVat_standard=isnull(@p91amount_withoutVat_standard,0)
set @p91amount_withoutVat_special=isnull(@p91amount_withoutVat_special,0)
set @p91amount_withVat_low=isnull(@p91amount_withVat_low,0)
set @p91amount_withVat_standard=isnull(@p91amount_withVat_standard,0)
set @p91amount_withVat_special=isnull(@p91amount_withVat_special,0)
set @p91amount_Vat_low=isnull(@p91amount_Vat_low,0)
set @p91amount_Vat_standard=isnull(@p91amount_Vat_standard,0)
set @p91amount_Vat_special=isnull(@p91amount_Vat_special,0)

set @p91amount_withoutVat=@p91amount_withoutvat_none+@p91amount_withoutVat_low+@p91amount_withoutVat_standard+@p91amount_withoutVat_special
set @p91amount_withVat=@p91amount_withoutvat_none+@p91amount_withVat_low+@p91amount_withVat_standard+@p91amount_withVat_special
set @p91amount_Vat=@p91amount_Vat_low+@p91amount_Vat_standard+@p91amount_Vat_special

--************************zaokrouhlování************************************----
declare @p97id int,@p97scale int,@p91roundfitamount float
declare @p97amountflag int	---jaká èástka je pøedmìtem zaokrouhlování: 1-èástka DPH, 2-èástka bez DPH (základ), 3-èástka vè. DPH

set @p91roundfitamount=0

if @j17id is not null
 select top 1 @p97id=p97id,@p97amountflag=p97AmountFlag,@p97scale=p97Scale from p97Invoice_Round_Setting where j17ID=@j17id and j27id=@j27id_dest

if @p97id is null
 select top 1 @p97id=p97id,@p97amountflag=p97AmountFlag,@p97scale=p97Scale from p97Invoice_Round_Setting where j17ID is null and j27id=@j27id_dest



if @p97id is not null
  begin  ----dojde k zaokrouhlování
    if @p97amountflag=3		---zaokrouhluje se celková èástka faktury
	begin
	  set @p91roundfitamount=round(@p91amount_withVat,@p97scale)-@p91amount_withVat
	  set @p91amount_withVat=@p91amount_withVat+@p91roundfitamount
	  
	end
	if @p97amountflag=2		---zaokrouhluje se celkový základ danì
	 begin
	   declare @xx float
	   set @xx=0
	   set @p91roundfitamount=0
	   
	   set @xx=round(@p91amount_withoutVat_none,@p97scale)-@p91amount_withoutVat_none
	   set @p91amount_withoutVat_none=@p91amount_withoutVat_none+@xx
	   set @p91roundfitamount=@p91roundfitamount+@xx
	   
	   set @xx=round(@p91amount_withoutVat_low,@p97scale)-@p91amount_withoutVat_low
	   set @p91amount_withoutVat_low=@p91amount_withoutVat_low+@xx
	   set @p91amount_Vat_low=@p91amount_withoutVat_low*@p91vatrate_low/100
	   set @p91amount_withVat_low=@p91amount_withoutVat_low+@p91amount_Vat_low
	   set @p91roundfitamount=@p91roundfitamount+@xx
	   
	   set @xx=round(@p91amount_withoutVat_standard,@p97scale)-@p91amount_withoutVat_standard
	   set @p91amount_withoutVat_standard=@p91amount_withoutVat_standard+@xx
	   set @p91amount_Vat_standard=@p91amount_withoutVat_standard*@p91vatrate_standard/100
	   set @p91amount_withVat_standard=@p91amount_withoutVat_standard+@p91amount_Vat_standard
	   set @p91roundfitamount=@p91roundfitamount+@xx
	   
	   set @p91amount_withoutVat=@p91amount_withoutvat_none+@p91amount_withoutVat_low+@p91amount_withoutVat_standard
	   set @p91amount_withVat=@p91amount_withoutvat_none+@p91amount_withVat_low+@p91amount_withVat_standard
	   set @p91amount_Vat=0+@p91amount_Vat_low+@p91amount_Vat_standard
	   --set @p91roundfitamount=0	---zaokrouhlení je nula, protože už je zahrnuté v èástce bez DPH
	   
	 end

    if @p97amountflag=1		---zaokrouhluje se výsledná èástka DPH
	begin
	  set @p91roundfitamount=round(@p91amount_Vat_low,@p97scale)-@p91amount_Vat_low
	  set @p91amount_Vat_low=@p91amount_Vat_low+@p91roundfitamount
	  set @p91amount_withVat_low=@p91amount_Vat_low+@p91amount_withoutVat_low
	
	  set @p91roundfitamount=round(@p91amount_Vat_standard,@p97scale)-@p91amount_Vat_standard
	  set @p91amount_Vat_standard=@p91amount_Vat_standard+@p91roundfitamount
	  set @p91amount_withVat_standard=@p91amount_Vat_standard+@p91amount_withoutVat_standard

	  set @p91roundfitamount=round(@p91amount_Vat_special,@p97scale)-@p91amount_Vat_special
	  set @p91amount_Vat_special=@p91amount_Vat_special+@p91roundfitamount
	  set @p91amount_withVat_special=@p91amount_Vat_special+@p91amount_withoutVat_special
	
	  set @p91amount_Vat=0+@p91amount_Vat_low+@p91amount_Vat_standard+@p91amount_Vat_special
	
	  set @p91amount_withVat=@p91amount_withoutvat_none+@p91amount_withVat_low+@p91amount_withVat_standard+@p91amount_withVat_special

	  set @p91roundfitamount=0	---zaokrouhlení je nula, protože už je zahrnuté v èástce DPH
	  
	end
  end


---*************- celkový dluh k zaplacení************************************
declare @p91amount_billed float,@p91amount_debt float,@datLastBilled datetime
declare @p91proformaamount float,@p91proformabilledamount float



select @p91proformaamount=sum(a.p90amount),@p91proformabilledamount=sum(a.p90Amount_Billed) FROM p90Proforma a inner join p99Invoice_Proforma b on a.p90id=b.p90id where b.p91id=@p91id

set @p91proformabilledamount=isnull(@p91proformabilledamount,0)
set @p91proformaamount=isnull(@p91proformaamount,0)

select @p91amount_billed=sum(p94Amount),@datLastBilled=max(p94Date) FROM p94Invoice_Payment where p91id=@p91id

set @p91amount_billed=isnull(@p91amount_billed,0)+@p91proformabilledamount


set @p91amount_debt=(@p91amount_withVat)-@p91amount_billed

if @p92invoicetype=2	---dobropis
 begin
  set @p91amount_debt=0
  set @p91amount_billed=@p91amount_withVat
 end

if exists(select a.p91id from p91invoice a inner join p92InvoiceType b on a.p92id=b.p92id where b.p92invoicetype=2 and a.p91ID_CreditNoteBind=@p91id)
 begin
   ---faktura je dobropisována
   declare @amount_dobropis float
   select @amount_dobropis=sum(p91amount_withVat) from p91invoice a inner join p92InvoiceType b on a.p92id=b.p92id where b.p92invoicetype=2 and a.p91ID_CreditNoteBind=@p91id
   set @p91amount_debt=@p91amount_debt-abs(@amount_dobropis)
   set @p91amount_billed=@p91amount_billed+abs(@amount_dobropis)
 end

----****závìreèný update**********************
update p91invoice set p91amount_withoutvat_none=@p91amount_withoutvat_none,p91amount_withoutVat_low=@p91amount_withoutVat_low,p91amount_withoutVat_standard=@p91amount_withoutVat_standard,p91amount_withoutVat_special=@p91amount_withoutVat_special
,p91amount_withVat_low=@p91amount_withVat_low,p91amount_withVat_standard=@p91amount_withVat_standard,p91amount_withVat_special=@p91amount_withVat_special
,p91amount_Vat_low=@p91amount_Vat_low,p91amount_Vat_standard=@p91amount_Vat_standard,p91amount_Vat_special=@p91amount_Vat_special
,p91vatrate_low=@p91vatrate_low,p91vatrate_standard=@p91vatrate_standard,p91vatrate_special=@p91vatrate_special
,p91amount_withoutVat=@p91amount_withoutVat,p91amount_withVat=@p91amount_withVat,p91amount_Vat=@p91amount_Vat
,p91amount_billed=@p91amount_billed,p91amount_debt=@p91amount_debt,p91DateBilled=@datLastBilled
,p91roundfitamount=@p91roundfitamount
,p91proformaamount=@p91proformaamount,p91proformabilledamount=@p91proformabilledamount
,p91Amount_TotalDue=@p91amount_withVat-@p91proformabilledamount
,p41id_first=@p41id_first
WHERE p91id=@p91id


---rozúètování faktury
--exec p91_invoice_statement @p91id

if @p92invoicetype=2
 begin
   --pøepoèítat ještì svázaný doklad k dobropisu
   declare @p91id_bound int
   select TOP 1 @p91id_bound=p91ID_CreditNoteBind from p91invoice where p91id=@p91id

   if @p91id_bound is not null
     exec p91_recalc_amount @p91id_bound
 end





























GO

----------P---------------p91_update_code-------------------------

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

----------P---------------p92_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p92_delete') and type = 'P')
 drop procedure p92_delete
GO







CREATE   procedure [dbo].[p92_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p92id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu typu faktury z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p92ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jedna faktura má vazbu na tento typ ('+dbo.GetObjectAlias('91',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE from p92InvoiceType where p92ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p93_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p93_delete') and type = 'P')
 drop procedure p93_delete
GO







CREATE   procedure [dbo].[p93_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--p93id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu hlavièky dodavatele z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE p93ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ faktury má vazbu na tento záznam.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select p88ID FROM p88InvoiceHeader_BankAccount where p93ID=@pid)
	 DELETE FROM p88InvoiceHeader_BankAccount where p93ID=@pid

	DELETE from p93InvoiceHeader where p93ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------recovery_clear_temp-------------------------

if exists (select 1 from sysobjects where  id = object_id('recovery_clear_temp') and type = 'P')
 drop procedure recovery_clear_temp
GO










CREATE PROCEDURE [dbo].[recovery_clear_temp]
AS



truncate table p85TempBox


	
	






GO

----------P---------------x27_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x27_delete') and type = 'P')
 drop procedure x27_delete
GO












CREATE   procedure [dbo].[x27_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x27id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu skupiny uživatelských polí z tabulky x27EntityFieldGroup
if exists(select x28ID FROM x28EntityField WHERE x27ID=@pid)
 set @err_ret='Minimálnì jedno uživatelské pole je svázané s touto skupinou.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x27EntityFieldGroup where x27ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------x28_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x28_delete') and type = 'P')
 drop procedure x28_delete
GO






CREATE   procedure [dbo].[x28_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x28id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu uživatelského pole z tabulky x28EntityField

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x28EntityField where x28ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------x31_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x31_delete') and type = 'P')
 drop procedure x31_delete
GO








CREATE   procedure [dbo].[x31_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x31id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu sestavy z tabulky x31Report





BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=931)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=931

	if exists(select o27ID FROM o27Attachment WHERE x31ID=@pid)
	 DELETE FROM o27Attachment WHERE x31ID=@pid


	delete from x31Report where x31ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------x36userparam_get_mytag-------------------------

if exists (select 1 from sysobjects where  id = object_id('x36userparam_get_mytag') and type = 'P')
 drop procedure x36userparam_get_mytag
GO





CREATE   procedure [dbo].[x36userparam_get_mytag]
@j03id int					--id uživatele
,@x36key varchar(50)		--název klíèe
,@clear_after_read bit		--1 - ihned po pøeètení vyèistit hodnotu
,@x36value nvarchar(500) OUTPUT	--hodnota klíèe

AS

--Uložení záznamu uživatelského parametru do tabulky x36UserParam
set @x36value=null

if isnull(@j03id,0)=0
  return

if isnull(@x36key,'')=''
  return

declare @pid int


select top 1 @pid=x36id,@x36value=x36Value from x36UserParam where j03id=@j03id and x36key like @x36key

if @clear_after_read=1 AND @x36value is not null
 UPDATE x36UserParam SET x36value=NULL WHERE x36ID=@pid
































GO

----------P---------------x36userparam_save-------------------------

if exists (select 1 from sysobjects where  id = object_id('x36userparam_save') and type = 'P')
 drop procedure x36userparam_save
GO








CREATE   procedure [dbo].[x36userparam_save]
@j03id int					--id uživatele
,@x36key varchar(50)		--název klíèe
,@x36value nvarchar(500)	--hodnota klíèe

AS

--Uložení záznamu uživatelského parametru do tabulky x36UserParam

if isnull(@j03id,0)=0
  return

if isnull(@x36key,'')=''
  return

declare @pid int,@login nvarchar(50),@x36validuntil datetime

set @login=dbo.j03_getlogin(@j03id)
set @x36validuntil=convert(datetime,'01.01.3000',104)

select top 1 @pid=x36id from x36UserParam where j03id=@j03id and x36key like @x36key

if @pid is not null
   update x36UserParam set x36value=@x36value,x36dateupdate=getdate(),x36validuntil=@x36validuntil,x36userupdate=@login where x36id=@pid
else
   insert into x36UserParam(j03id,x36value,x36key,x36dateinsert,x36dateupdate,x36userinsert,x36userupdate,x36validfrom,x36validuntil) values(@j03id,@x36value,@x36key,getdate(),getdate(),@login,@login,getdate(),@x36validuntil)































GO

----------P---------------x37_save-------------------------

if exists (select 1 from sysobjects where  id = object_id('x37_save') and type = 'P')
 drop procedure x37_save
GO










CREATE   procedure [dbo].[x37_save]
@j03id int					--id uživatele
,@page varchar(50)		--název aspx stránky
,@dockstate nvarchar(MAX)	--uložený dockstate

AS

--Uložení záznamu uživatelem nastaveného stavu RadDock na stránce @page

if @j03id is null
  return

if @page is null
  return

if not exists(SELECT x37ID FROM x37SavedDockState WHERE j03ID=@j03id AND x37Page=@page)
 INSERT INTO x37SavedDockState(j03ID,x37Page,x37DockState,x37UserInsert) VALUES(@j03id,@page,@dockstate,dbo.j03_getlogin(@j03id))
else
 UPDATE x37SavedDockState set x37DockState=@dockstate,x37DateUpdate=getdate(),x37UserUpdate=dbo.j03_getlogin(@j03id) WHERE j03ID=@j03id AND x37Page=@page

































GO

----------P---------------x38_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x38_delete') and type = 'P')
 drop procedure x38_delete
GO











CREATE   procedure [dbo].[x38_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x38id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu èíselné øady z tabulky x38CodeLogic
declare @ref_pid int



SELECT TOP 1 @ref_pid=p42ID from p42ProjectType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ projektu je svázaný s touto èíselnou øadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p29ID from p29ContactType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ kontaktu je svázaný s touto èíselnou øadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ faktury je svázaný s touto èíselnou øadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p89ID from p89ProformaType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálnì jeden typ zálohy je svázaný s touto èíselnou øadou.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x38CodeLogic where x38ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------x46_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x46_delete') and type = 'P')
 drop procedure x46_delete
GO









CREATE   procedure [dbo].[x46_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x46id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu notifikaèního nastavení z tabulky x46EventNotification


BEGIN TRANSACTION

BEGIN TRY

	delete from x46EventNotification where x46ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  




















GO

----------P---------------x50_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x50_delete') and type = 'P')
 drop procedure x50_delete
GO







CREATE   procedure [dbo].[x50_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x50id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu pole z tabulky x50Help

if exists(select o27ID FROM o27Attachment where x50ID=@pid)
 DELETE FROM o27Attachment where x50ID=@pid

delete from x50Help where x50ID=@pid































GO

----------P---------------x55_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x55_delete') and type = 'P')
 drop procedure x55_delete
GO







CREATE   procedure [dbo].[x55_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x55id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu z tabulky x55HtmlSnippet

if exists(select x57ID FROM x57UserPageBinding WHERE x55ID=@pid)
 set @err_ret='Minimálnì jedna osobní stránka využívá tento BOX.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	
	DELETE FROM x56SnippetProperty WHERE x55ID=@pid

	delete from x55HtmlSnippet where x55ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------x67_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x67_delete') and type = 'P')
 drop procedure x67_delete
GO








CREATE   procedure [dbo].[x67_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--x67id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu role z tabulky x67EntityRole
declare @ref_pid int,@x29id int,@x69recordpid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=a.x69ID,@x29id=b.x29ID,@x69recordpid=a.x69recordpid
from x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID
WHERE b.x67ID=@pid

if @ref_pid is not null and @x29id=141
 set @err_ret='Tato role je již obsazena v minimálnì jednom projektu ('+dbo.GetObjectAlias('p41',@x69recordpid)+')'

if @ref_pid is not null and @x29id=328
 set @err_ret='Tato role je již obsazena v minimálnì jednom záznamu kontaktu ('+dbo.GetObjectAlias('p28',@x69recordpid)+')'

if @ref_pid is not null and @x29id=356
 set @err_ret='Tato role je již obsazena v minimálnì jedné úloze ('+dbo.GetObjectAlias('p56',@x69recordpid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select x67id from x67EntityRole where x67ID=@pid and x29ID=141)
	 begin
		--odstranit navíc naklonovanou roli pro entitu j18Region (projektová skupina)
		declare @x67id_cloned int
		
		select @x67id_cloned=x67ID FROM x67EntityRole WHERE x67ParentID=@pid

		delete from x68EntityRole_Permission WHERE x67ID=@x67id_cloned

		if exists(select o28ID FROM o28ProjectRole_Workload WHERE x67ID=@x67id_cloned)
		  DELETE FROM o28ProjectRole_Workload WHERE x67ID=@x67id_cloned

		delete from x67EntityRole where x67ID=@x67id_cloned
	 end

	delete from x68EntityRole_Permission WHERE x67ID=@pid

	if exists(select o28ID FROM o28ProjectRole_Workload WHERE x67ID=@pid)
	 DELETE FROM o28ProjectRole_Workload WHERE x67ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE x67ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE x67ID=@pid

	if exists(select x69ID FROM x69EntityRole_Assign WHERE x67ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE x67ID=@pid

	delete from x67EntityRole where x67ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------x90_appendlog-------------------------

if exists (select 1 from sysobjects where  id = object_id('x90_appendlog') and type = 'P')
 drop procedure x90_appendlog
GO






CREATE PROCEDURE [dbo].[x90_appendlog]
@x29id int
,@pid int
,@j03id_sys int

AS

declare @j02id_sys int,@validfrom datetime,@validuntil datetime,@dateinsert datetime,@x90id_last int,@validfrom_last datetime,@validuntil_last datetime

select @j02id_sys=dbo.j03_getj02id(@j03id_sys)

if @x29id=141
 select @validfrom=p41ValidFrom,@validuntil=p41ValidUntil,@dateinsert=p41DateInsert FROM p41Project WHERE p41ID=@pid

if @x29id=328
 select @validfrom=p28ValidFrom,@validuntil=p28ValidUntil,@dateinsert=p28DateInsert FROM p28Contact WHERE p28ID=@pid

if @x29id=102
 select @validfrom=j02ValidFrom,@validuntil=j02ValidUntil,@dateinsert=j02DateInsert FROM j02Person WHERE j02ID=@pid


declare @flag int
set @flag=2

select TOP 1 @x90id_last=x90ID,@validfrom_last=x90RecordValidFrom,@validuntil_last=x90RecordValidUntil
FROM x90EntityLog
WHERE x29ID=@x29id AND x90RecordPID=@pid
ORDER BY x90ID DESC

if @validuntil<>@validuntil_last AND getdate()>=@validuntil
 set @flag=3	---záznam byl pøesunut do koše

if @validuntil<>@validuntil_last AND getdate()<@validuntil
 set @flag=4	---záznam byl obnoven z koše

if not exists(select x90ID FROM x90EntityLog WHERE x29ID=@x29id AND x90RecordPID=@pid AND x90EventFlag=1)
BEGIN
 INSERT INTO x90EntityLog(x29ID,x90RecordPID,j03ID_Author,x90Date,x90EventFlag,j02ID_Author,x90RecordValidFrom,x90RecordValidUntil) VALUES(@x29id,@pid,@j03id_sys,@dateinsert,1,@j02id_sys,@dateinsert,convert(datetime,'01.01.3000',104))
END



INSERT INTO x90EntityLog(x29ID,x90RecordPID,j03ID_Author,x90Date,x90EventFlag,j02ID_Author,x90RecordValidFrom,x90RecordValidUntil)
VALUES(@x29id,@pid,@j03id_sys,getdate(),@flag,@j02id_sys,@validfrom,@validuntil)







GO

