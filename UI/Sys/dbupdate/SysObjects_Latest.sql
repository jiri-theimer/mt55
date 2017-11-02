----------FN---------------CleanHTMLTags-------------------------

if exists (select 1 from sysobjects where  id = object_id('CleanHTMLTags') and type = 'FN')
 drop function CleanHTMLTags
GO


CREATE FUNCTION [dbo].[CleanHTMLTags] (@HTMLText VARCHAR(MAX),@ReplaceChar char(1))
RETURNS VARCHAR(MAX)
AS
BEGIN
DECLARE @Start INT
DECLARE @End INT
DECLARE @Length INT

set @HTMLText=replace(@HTMLText,'&nbsp;',' ')
set @HTMLText=replace(@HTMLText,'<br />',char(13)+char(10))
set @HTMLText=replace(@HTMLText,'</p>',char(13)+char(10))

SET @Start = CHARINDEX('<',@HTMLText)
SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
SET @Length = (@End - @Start) + 1 
WHILE @Start > 0 AND @End > 0 AND @Length > 0
 BEGIN
  IF (UPPER(SUBSTRING(@HTMLText, @Start, 4)) <> '') AND (UPPER(SUBSTRING(@HTMLText, @Start, 5)) <>'')
   begin
    SET @HTMLText = RTRIM(LTRIM(STUFF(@HTMLText,@Start,@Length,@ReplaceChar)));
   end
  ELSE
   SET @Length = 0;
   SET @Start = CHARINDEX('<',@HTMLText, @End-@Length) 
   SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText, @Start))
   SET @Length = (@End - @Start) + 1
  END
  RETURN isnull(RTRIM(LTRIM(@HTMLText)), '')
END

GO

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

----------FN---------------ConvertNumberToRoman-------------------------

if exists (select 1 from sysobjects where  id = object_id('ConvertNumberToRoman') and type = 'FN')
 drop function ConvertNumberToRoman
GO


CREATE FUNCTION dbo.ConvertNumberToRoman (@Number INT)
  /**
 summary:   >
 This is a simple routine for converting a decimal integer into a roman numeral.
 Author: Phil Factor
 Revision: 1.2
 date: 3rd Feb 2014
 Why: converted to run on SQL Server 2008-12
 example:
      - code: Select dbo.ToRomanNumerals(187)
      - code: Select dbo.ToRomanNumerals(2011)
 returns:   >
 The Mediaeval-style 'roman' numeral as a string.
 **/   
 RETURNS varchar(100)
 AS
 BEGIN
  IF @Number<0
     BEGIN
     RETURN 'De romanorum non numero negative'
     end                          
   IF @Number> 200000
     BEGIN
     RETURN 'O Juppiter, magnus numerus'
     end                          
   DECLARE @RomanNumeral AS varchar(100)
   DECLARE @RomanSystem TABLE (symbol varchar(20) 
                                   COLLATE SQL_Latin1_General_Cp437_BIN ,
                               DecimalValue INT PRIMARY key)
    INSERT  INTO @RomanSystem (symbol, DecimalValue)
     VALUES('I', 1),
           ('IV', 4),
           ('V', 5),
           ('IX', 9),
           ('X', 10),
           ('XL', 40),
           ('L', 50),
           ('XC', 90),
           ('C', 100),
           ('CD', 400),
           ('D', 500),
           ('CM', 900),
           ('M', 1000),
           (N'|ↄↄ', 5000),
           (N'cc|ↄↄ', 10000),
           (N'|ↄↄↄ', 50000),
           (N'ccc|ↄↄↄ', 100000),
           (N'ccc|ↄↄↄↄↄↄ', 150000)
  
   WHILE @Number > 0
     SELECT  @RomanNumeral = COALESCE(@RomanNumeral, '') + symbol,
             @Number = @Number - DecimalValue
     FROM    @RomanSystem
     WHERE   DecimalValue = (SELECT  MAX(DecimalValue)
                             FROM    @RomanSystem
                             WHERE   DecimalValue <= @number)
   RETURN COALESCE(@RomanNumeral,'nulla')
 END


GO

----------FN---------------get_datename-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_datename') and type = 'FN')
 drop function get_datename
GO





create FUNCTION [dbo].[get_datename] (@d datetime,@langindex int)
--@langindex=0 -> česky, 1 -> anglicky
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

----------FN---------------get_domestic_j27code-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_domestic_j27code') and type = 'FN')
 drop function get_domestic_j27code
GO









CREATE    FUNCTION [dbo].[get_domestic_j27code]()
RETURNS varchar(10)
AS
BEGIN
   declare @s varchar(10),@j27id int,@j27code varchar(10)

  select @s=x35Value from x35GlobalParam WHERE x35key like 'j27ID_Domestic'

  if isnull(@s,'')=''
   set @j27id=2
  else
   set @j27id=convert(int,@s)

  select @j27code=j27Code FROM j27Currency WHERE j27ID=@j27id
   
  RETURN(@j27code)
END





















GO

----------FN---------------get_domestic_j27id-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_domestic_j27id') and type = 'FN')
 drop function get_domestic_j27id
GO









CREATE    FUNCTION [dbo].[get_domestic_j27id]()
RETURNS int
AS
BEGIN
   declare @s varchar(10),@j27id int

  select @s=x35Value from x35GlobalParam WHERE x35key like 'j27ID_Domestic'

  if isnull(@s,'')=''
   set @j27id=2
  else
   set @j27id=convert(int,@s)
   
  RETURN(@j27id)
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
  set @ratetype=isnull(@ratetype,1)		---@ratetype=1 - fakturační kurz, @ratetype=2 - fixní kurz

  if @j27id_source is null or @j27id_dest is null
    return(1)

  if @j27id_source=@j27id_dest
    RETURN(1)

  declare @j27id_domestic int	--domácí měna, která definuje měnové kurzy vůči ostatním měnám, výchozí je CZK

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
    RETURN(@ret)	--převod do nebo z domácí měny



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

----------FN---------------get_one_role_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_one_role_inline') and type = 'FN')
 drop function get_one_role_inline
GO






CREATE    FUNCTION [dbo].[get_one_role_inline](@recordpid int,@x67id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení projektové role @x67id v projektu @p41id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@recordpid AND x69.x67ID=@x67id
  


RETURN(@s)
   
END


























GO

----------FN---------------get_parsed_text_with_period-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_parsed_text_with_period') and type = 'FN')
 drop function get_parsed_text_with_period
GO





CREATE    FUNCTION [dbo].[get_parsed_text_with_period](@expr nvarchar(4000),@d datetime,@period_index int)
RETURNS nvarchar(4000)
AS
BEGIN
  ---podle masky [MM], [YYYY], [QQ]

if @d is null
 RETURN(@expr)

set @period_index=isnull(@period_index,0)


if @period_index=0
begin
set @expr=replace(@expr,'[MM]',right('0'+convert(varchar(10),month(@d)),2))
set @expr=replace(@expr,'[DD]',right('0'+convert(varchar(10),day(@d)),2))
set @expr=replace(@expr,'[YYYY]',convert(varchar(10),year(@d)))
set @expr=replace(@expr,'[WW]',right('0'+convert(varchar(10),datepart(week,@d)),2))
set @expr=replace(@expr,'[QQ]',right('0'+convert(varchar(10),datepart(quarter,@d)),2))
set @expr=replace(@expr,'[Q]',convert(varchar(10),datepart(quarter,@d)))
set @expr=replace(@expr,'[M]',convert(varchar(10),month(@d)))
end
if @period_index=1
begin
set @expr=replace(@expr,'[MM1]',right('0'+convert(varchar(10),month(@d)),2))
set @expr=replace(@expr,'[DD1]',right('0'+convert(varchar(10),day(@d)),2))
set @expr=replace(@expr,'[YYYY1]',convert(varchar(10),year(@d)))
set @expr=replace(@expr,'[WW1]',right('0'+convert(varchar(10),datepart(week,@d)),2))
set @expr=replace(@expr,'[QQ1]',right('0'+convert(varchar(10),datepart(quarter,@d)),2))
set @expr=replace(@expr,'[Q1]',convert(varchar(10),datepart(quarter,@d)))
set @expr=replace(@expr,'[M1]',convert(varchar(10),month(@d)))
end
if @period_index=2
begin
set @expr=replace(@expr,'[MM2]',right('0'+convert(varchar(10),month(@d)),2))
set @expr=replace(@expr,'[DD2]',right('0'+convert(varchar(10),day(@d)),2))
set @expr=replace(@expr,'[YYYY2]',convert(varchar(10),year(@d)))
set @expr=replace(@expr,'[WW2]',right('0'+convert(varchar(10),datepart(week,@d)),2))
set @expr=replace(@expr,'[QQ2]',right('0'+convert(varchar(10),datepart(quarter,@d)),2))
set @expr=replace(@expr,'[Q2]',convert(varchar(10),datepart(quarter,@d)))
set @expr=replace(@expr,'[M2]',convert(varchar(10),month(@d)))
end



RETURN(@expr)
   
END



























GO

----------FN---------------get_today-------------------------

if exists (select 1 from sysobjects where  id = object_id('get_today') and type = 'FN')
 drop function get_today
GO



CREATE FUNCTION [dbo].[get_today](
  
	)
RETURNS datetime
AS
BEGIN
	declare @d int,@m int,@s varchar(10)
	set @d=day(getdate())
	set @m=MONTH(getdate())
	
	if @d<10
	 set @s='0'+CONVERT(varchar(1),@d)
	else
	 set @s=CONVERT(varchar(2),@d)
	 
	if @m<10
	 set @s=@s+'.0'+CONVERT(varchar(1),@m)
	else
	 set @s=@s+'.'+CONVERT(varchar(2),@m)
	
	set @s=@s+'.'+CONVERT(varchar(4),year(getdate()))
	
	RETURN convert(datetime,@s,104)

    ---RETURN cast(convert(varchar(10), getdate(), 110) as datetime)
    
    
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
  set @size=@size/2	--nvarchar typ dělíme 2

if @type=35
  return(0)	--memo


RETURN(isnull(@size,0))


END






GO

----------FN---------------GetFirstMondayInWeek-------------------------

if exists (select 1 from sysobjects where  id = object_id('GetFirstMondayInWeek') and type = 'FN')
 drop function GetFirstMondayInWeek
GO


CREATE FUNCTION [dbo].[GetFirstMondayInWeek] (@d datetime)
--vrací první pondělí v týdnu, kam patří den @d
RETURNS datetime AS  
BEGIN 

set @d=dbo.convert_to_dateserial(@d)

while upper(DATENAME(weekday,@d))<>'MONDAY'
  begin
	set @d=dateadd(day,-1,@d)
  end

RETURN(@d)
	
END

GO

----------FN---------------GetNextMonth-------------------------

if exists (select 1 from sysobjects where  id = object_id('GetNextMonth') and type = 'FN')
 drop function GetNextMonth
GO





CREATE FUNCTION [dbo].[GetNextMonth] (@d datetime)
--vrací datum o měsíc posunutý vůči @d
RETURNS datetime AS  
BEGIN 

declare @dx datetime

set @dx=dateadd(day,1,@d)

if month(@d)=month(@dx)
 RETURN(dateadd(mm,1,@d))

declare @m int
set @m=month(@dx)


while month(@dx)=@m
begin
 set @dx=dateadd(day,1,@dx)
 
end

set @dx=dateadd(day,-1,@dx)

RETURN(@dx)
	
END












GO

----------FN---------------GetNextQuarter-------------------------

if exists (select 1 from sysobjects where  id = object_id('GetNextQuarter') and type = 'FN')
 drop function GetNextQuarter
GO






CREATE FUNCTION [dbo].[GetNextQuarter] (@d datetime)
--vrací datum o kvartál posunutý vůči @d
RETURNS datetime AS  
BEGIN 

declare @dx datetime


set @dx=dbo.GetNextMonth(@d)
set @dx=dbo.GetNextMonth(@dx)
set @dx=dbo.GetNextMonth(@dx)

RETURN(@dx)
	
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

if isnumeric(@prefix)=1
 select @prefix=left(x29TableName,3) FROM x29Entity WHERE x29ID=convert(int,@prefix)

declare @ret nvarchar(300),@refp28id int,@refp41id int,@refp91id int,@refj02id int,@refpid int,@refx29id int,@refp56id int
	
if @prefix='p28'
 select @ret=p28name FROM p28contact where p28id=@pid

if @prefix='p41'
 begin
  if not exists(select p41ID FROM p41Project WHERE p41ID=@pid AND p41ParentID IS NOT NULL)
   select @ret=isnull(p41NameShort,p41Name)+isnull(' | '+b.p28name,'') FROM p41Project a LEFT OUTER JOIN p28Contact b on a.p28ID_Client=b.p28ID where a.p41id=@pid
  else
   select @ret=isnull(b.p41NameShort,b.p41Name)+'->'+isnull(a.p41NameShort,a.p41name) FROM p41Project a LEFT OUTER JOIN p41Project b on a.p41ParentID=b.p41ID where a.p41id=@pid
 end
  

if @prefix='p91'
  select @ret=p91code+' | '+isnull(a.p91Client,b.p28Name) FROM p91invoice a LEFT OUTER JOIN p28Contact b ON a.p28ID=b.p28ID where a.p91id=@pid

if @prefix='p90'
  select @ret=p90code+isnull(' ['+b.p28name+']','') FROM p90Proforma a LEFT OUTER JOIN p28Contact b ON a.p28ID=b.p28ID where a.p90ID=@pid


if @prefix='p31'
  select @ret=p41Name+' ['+p34name+'] '+convert(varchar(20),p31date,104)+' ['+j02LastName+']'+case when len(a.p31Text)>50 then ' '+left(a.p31Text,50)+'...' else isnull(' '+a.p31Text,'') end+case when c.p33ID IN (2,5) then ' ['+convert(varchar(10),a.p31Amount_WithoutVat_Orig)+' '+j27.j27Code+']' else '' end FROM p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34activitygroup c on b.p34id=c.p34id inner join j02Person d on a.j02ID=d.j02ID inner join p41Project p41 on a.p41id=p41.p41id left outer join j27Currency j27 ON a.j27ID_Billing_Orig=j27.j27ID where a.p31id=@pid


if @prefix='j03'
  select @ret=j03Login+isnull(' | '+j02LastName+' '+j02Firstname,'') from j03user a LEFT OUTER JOIN j02Person b ON a.j02ID=b.j02ID where a.j03id=@pid 

if @prefix='j02'
  select @ret=j02LastName+' '+j02FirstName FROM j02Person WHERE j02ID=@pid



if @prefix='o22'
 select @ret=o22Name+' - '+[dbo].GetDDMMYYYYHHMM(o22DateUntil)+' ('+o21Name+')',@refp28id=p28id,@refp41id=p41id,@refp91id=p91id,@refj02id=j02id FROM o22Milestone a inner join o21MilestoneType b ON a.o21ID=b.o21ID WHERE a.o22ID=@pid

 
if @prefix='o23'
 select @ret=isnull(o23Code,'')+' | '+isnull(c.x18Name+': ','')+isnull(a.o23Name,'') FROM o23Doc a INNER JOIN x23EntityField_Combo b ON a.x23ID=b.x23ID INNER JOIN x18EntityCategory c ON b.x23ID=c.x23ID WHERE a.o23ID=@pid
  
if @prefix='b07'
 select @ret='Komentář od: '+b.j02Firstname+' '+b.j02LastName, @refpid=a.b07RecordPID,@refx29id=a.x29ID FROM b07Comment a INNER JOIN j02Person b ON a.j02ID_Owner=b.j02ID WHERE a.b07ID=@pid
 

if @prefix='p32'
  select @ret=p32Name+' | '+isnull(p34name,'') FROM p32Activity a LEFT OUTER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE a.p32ID=@pid

if @prefix='p34'
  select @ret=p34Name FROM p34ActivityGroup WHERE p34ID=@pid

if @prefix='p51'
  select @ret=p51Name+' ('+j27Code+')' from p51PriceList a INNER JOIN j27Currency b ON a.j27ID=b.j27ID WHERE a.p51ID=@pid

if @prefix='p49'
  select @ret=right(convert(varchar(20),a.p49DateFrom,104),7)+'/'+ convert(varchar(20),a.p49Amount)+',-'+b.j27Code+isnull('/'+p49Text,'') from p49FinancialPlan a INNER JOIN j27Currency b ON a.j27ID=b.j27ID INNER JOIN p45Budget c ON a.p45ID=c.p45ID WHERE a.p49ID=@pid

if @prefix='p36'
 select @ret=convert(varchar(20),p36DateFrom,104)+' - '+convert(varchar(20),p36DateUntil,104) from p36LockPeriod WHERE p36ID=@pid

if @prefix='p56'
 begin
  
  select @ret=p56Code+' | '+c.p57Name+': '+isnull(p56Name,'')+' | '+isnull(b.p41NameShort,b.p41Name) from p56Task a inner join p41Project b on a.p41ID=b.p41id INNER JOIN p57TaskType c ON a.p57ID=c.p57ID where a.p56id=@pid 

 end

 if @prefix='p45'
  select @ret=p45Name+' | '+isnull(b.p41NameShort,b.p41Name) from p45Budget a inner join p41Project b on a.p41ID=b.p41id where a.p45ID=@pid 



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

   if @refx29id=102
    set @refj02id=@refpid

   if @refx29id=356
    set @refp56id=@refpid
 end

if @refp28id is not null
 select @ret=@ret+' | '+p28name FROM p28Contact WHERE p28ID=@refp28id

if @refp41id is not null
 select @ret=@ret+' | '+p41name+isnull(' ('+b.p28name+')','') FROM p41Project a LEFT OUTER JOIN p28Contact b ON a.p28ID_Client=b.p28ID WHERE a.p41ID=@refp41id

if @refp91id is not null
 select @ret=@ret+' | '+p91Code FROM p91Invoice WHERE p91ID=@refp91id

if @refj02id is not null
 select @ret=@ret+' | '+j02FirstName+' '+j02LastName FROM j02Person WHERE j02ID=@refj02id

if @refp56id is not null
 select @ret=@ret+' | '+b.p57Name+': '+convert(varchar(10),a.p56Code) FROM p56Task a INNER JOIN p57TaskType b ON a.p57ID=b.p57ID WHERE a.p56ID=@refp56id

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

----------FN---------------j02_c21_get_hours_for_day-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_c21_get_hours_for_day') and type = 'FN')
 drop function j02_c21_get_hours_for_day
GO





CREATE FUNCTION [dbo].[j02_c21_get_hours_for_day] (@j02id int,@d datetime)
--vrací počet hodin z pracovního fondu osoby @j02id pro den @d
RETURNS float AS  
BEGIN 

declare @hours float,@c21id int,@j17id int

select @c21id=c21ID,@j17id=j17ID FROM j02Person WHERE j02ID=@j02id

select @hours=c22hours_work FROM c22FondCalendar_Date WHERE c21ID=@c21id AND c22Date=@d AND isnull(j17ID,0)=isnull(@j17id,0)


RETURN(isnull(@hours,0))
	
END














GO

----------FN---------------j02_clients_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_clients_inline') and type = 'FN')
 drop function j02_clients_inline
GO


CREATE    FUNCTION [dbo].[j02_clients_inline](@j02id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené názvy týmů, v kterých je osoba @j02id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+b.p28Name
  FROM p30Contact_Person a INNER JOIN p28Contact b ON a.p28ID=b.p28ID
  WHERE a.j02ID=@j02id



RETURN(@s)
   
END


GO

----------FN---------------j02_get_pid_from_login-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_get_pid_from_login') and type = 'FN')
 drop function j02_get_pid_from_login
GO








CREATE    FUNCTION [dbo].[j02_get_pid_from_login](@login varchar(50))
RETURNS INT
AS
BEGIN
  ---vrací j02ID osoby

 

  RETURN(select a.J02ID FROM j02Person a INNER JOIN j03user b ON a.j02ID=b.j02ID where b.j03Login LIKE @login)
   
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
  ---vrací čárkou oddělené názvy týmů, v kterých je osoba @j02id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+b.j11Name
  FROM j12Team_Person a INNER JOIN j11Team b ON a.j11ID=b.j11ID
  WHERE a.j02ID=@j02id AND b.j11IsAllPersons=0
  ORDER BY j11Name


RETURN(@s)
   
END


GO

----------FN---------------j03_get_pid_from_login-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03_get_pid_from_login') and type = 'FN')
 drop function j03_get_pid_from_login
GO








CREATE    FUNCTION [dbo].[j03_get_pid_from_login](@login varchar(50))
RETURNS INT
AS
BEGIN
  ---vrací j03ID uživatele

 

  RETURN(select j03ID FROM j03user where j03Login LIKE @login)
   
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
  ---vrací 1, pokud uživatel @j03id disponuje oprávněním v jeho globální roli (j04)
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
  ---vrací čárkou oddělené názvy podřízených osob a týmů

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

----------FN---------------my_iif1-------------------------

if exists (select 1 from sysobjects where  id = object_id('my_iif1') and type = 'FN')
 drop function my_iif1
GO




CREATE FUNCTION [dbo].[my_iif1](
    @compare_val1 int
	,@compare_val2 int
	,@return_true float
	,@return_false float
	)
RETURNS float
AS
BEGIN
    if @compare_val1=@compare_val2
	 RETURN @return_true
	


	 RETURN @return_false
END





GO

----------FN---------------my_iif2_bit-------------------------

if exists (select 1 from sysobjects where  id = object_id('my_iif2_bit') and type = 'FN')
 drop function my_iif2_bit
GO






create FUNCTION [dbo].[my_iif2_bit](
    @compare_val1 int
	,@compare_val2 int
	,@return_true bit
	,@return_false bit
	)
RETURNS bit
AS
BEGIN
    if @compare_val1=@compare_val2
	 RETURN @return_true
	


	 RETURN @return_false
END







GO

----------FN---------------my_iif2_date-------------------------

if exists (select 1 from sysobjects where  id = object_id('my_iif2_date') and type = 'FN')
 drop function my_iif2_date
GO





create FUNCTION [dbo].[my_iif2_date](
    @compare_val1 int
	,@compare_val2 int
	,@return_true datetime
	,@return_false datetime
	)
RETURNS datetime
AS
BEGIN
    if @compare_val1=@compare_val2
	 RETURN @return_true
	


	 RETURN @return_false
END






GO

----------FN---------------my_iif2_number-------------------------

if exists (select 1 from sysobjects where  id = object_id('my_iif2_number') and type = 'FN')
 drop function my_iif2_number
GO






CREATE FUNCTION [dbo].[my_iif2_number](
    @compare_val1 int
	,@compare_val2 int
	,@return_true float
	,@return_false float
	)
RETURNS float
AS
BEGIN
    if @compare_val1=@compare_val2
	 RETURN @return_true
	


	 RETURN @return_false
END







GO

----------FN---------------my_iif2_string-------------------------

if exists (select 1 from sysobjects where  id = object_id('my_iif2_string') and type = 'FN')
 drop function my_iif2_string
GO





CREATE FUNCTION [dbo].[my_iif2_string](
    @compare_val1 int
	,@compare_val2 int
	,@return_true nvarchar(255)
	,@return_false nvarchar(255)
	)
RETURNS nvarchar(255)
AS
BEGIN
    if @compare_val1=@compare_val2
	 RETURN @return_true
	


	 RETURN @return_false
END






GO

----------FN---------------o22_getroles_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('o22_getroles_inline') and type = 'FN')
 drop function o22_getroles_inline
GO







CREATE    FUNCTION [dbo].[o22_getroles_inline](@o22id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení  rolí v události @p56id

 DECLARE @s nvarchar(2000)


select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))
  FROM o20Milestone_Receiver o20
  LEFT OUTER JOIN j02Person j02 ON o20.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON o20.j11ID=j11.j11ID
  WHERE o20.o22ID=@o22id
  

RETURN(@s)
   
END



























GO

----------FN---------------o23_get_text-------------------------

if exists (select 1 from sysobjects where  id = object_id('o23_get_text') and type = 'FN')
 drop function o23_get_text
GO


CREATE FUNCTION [dbo].[o23_get_text](@x23id int,@o23id int)
RETURNS nvarchar(255) AS  
BEGIN 

if ISNULL(@o23id,0)=0
 RETURN(NULL)

declare @ret nvarchar(255)

select @ret=o23Name
from
o23Doc
where x23ID=@x23id AND o23ID=@o23id


RETURN(@ret)

END



GO

----------FN---------------o23_getroles_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('o23_getroles_inline') and type = 'FN')
 drop function o23_getroles_inline
GO


CREATE    FUNCTION [dbo].[o23_getroles_inline](@o23id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení  rolí v položce štítku @o23id

 DECLARE @s nvarchar(2000),@count int

 select @count=count(*) FROM x67EntityRole WHERE x29ID=223

 
 if @count>1
 begin
select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))+' ('+x67Name+')'
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@o23id AND x67.x29ID=223
  ORDER BY x67Ordinary
 end

  if @count<=1
 begin
select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@o23id AND x67.x29ID=223
  ORDER BY x67Ordinary
 end

RETURN(@s)
   
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
   
declare @o28id int,@o28permflag int	---0-pouze vlastní worksheet,1-Číst vše v rámci projektu, 2-Číst a upravovat vše v rámci projektu,3-Číst a schvalovat vše v rámci projektu 4 - Číst, upravovat a schvalovat vše v rámci projektu

select TOP 1 @o28id=a.o28id,@o28permflag=a.o28PermFlag
from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
where a.p34ID=@p34id AND x69.x69RecordPID=@p41id AND x67.x29ID=141
and (isnull(x69.j02ID,0)=@j02id OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))
AND a.o28entryflag>=@minpermflag AND a.o28PermFlag<=@maxpermflag
ORDER BY a.o28PermFlag DESC
	
if @o28id is null and @j18id is not null
	begin ----------oprávnění k projektu podle projektové skupiny (regionu)
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

----------FN---------------p11_find_p41id_default-------------------------

if exists (select 1 from sysobjects where  id = object_id('p11_find_p41id_default') and type = 'FN')
 drop function p11_find_p41id_default
GO





CREATE  FUNCTION [dbo].[p11_find_p41id_default](@j02id int)
RETURNS int
AS
BEGIN
  ---vrací ID projektu pro zapisování neproduktivních hodin v rozhraní docházky

  declare @p41id int,@p42id int,@s varchar(10)

  select @s=x35Value FROM x35GlobalParam WHERE x35Key like 'attendance_p41_default'

  if @s is not null
   select @p41id=convert(int,@s)

  if @p41id is null
   select TOP 1 @p42id=p42ID FROM p43ProjectType_Workload WHERE p34ID IN (select a.p34ID FROM p32Activity a INNER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE a.p32AttendanceFlag=1 OR a.p32AttendanceFlag=2)

  if not @p42id is null
   select @p41ID=p41ID FROM p41Project WHERE p42ID=@p42id AND getdate() between p41ValidFrom and p41ValidUntil

  
  if @p41id is null
   select TOP 1 @p41id=a.p41ID FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID WHERE a.p32ID IN (select p32ID FROM p32Activity WHERE p32AttendanceFlag=1 OR p32AttendanceFlag=2) AND getdate() between b.p41ValidFrom AND b.p41ValidUntil ORDER BY a.p31ID DESC

  
  if @p41id is null
   select TOP 1 @p41id=p41ID FROM p41Project WHERE p41Name like '%režij%' AND getdate() between p41ValidFrom and p41ValidUntil
  
  
  
  return(@p41id)
   
END



GO

----------FN---------------p28_address_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_address_inline') and type = 'FN')
 drop function p28_address_inline
GO



CREATE    FUNCTION [dbo].[p28_address_inline](@p28id int,@o36id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené adresy z profilu kontaktu @p28id pro typ adresy @o36id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+isnull(o38street+', ','')+isnull(o38city+', ','')+isnull(o38zip,'')
from
o38Address a INNER JOIN o37Contact_Address b ON a.o38ID=b.o38ID
WHERE b.p28ID=@p28id and b.o36ID=@o36id


RETURN(@s)
   
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
  ---vrací čárkou oddělené adresy z profilu kontaktu @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+c.o36Name+' ['+isnull(o38street+', ','')+isnull(o38city+', ','')+isnull(o38zip,'')+']'
from
o38Address a INNER JOIN o37Contact_Address b ON a.o38ID=b.o38ID
INNER JOIN o36AddressType c ON b.o36ID=c.o36ID
WHERE b.p28ID=@p28id


RETURN(@s)
   
END

GO

----------FN---------------p28_get_childs_inline_html-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_get_childs_inline_html') and type = 'FN')
 drop function p28_get_childs_inline_html
GO





create    FUNCTION [dbo].[p28_get_childs_inline_html](@p28id int)
RETURNS varchar(5000)
AS
BEGIN
  ---vrací čárkou html seznam pod-klientů v klientovi @p28id

 DECLARE @s nvarchar(2000) 


select top 20 @s=COALESCE(@s + ' ', '')+'<div><a href='+char(34)+'javascript: send_url_to_pane('+char(39)+'p28_framework_detail.aspx?pid='+convert(varchar(10),b.p28ID)+char(39)+')'+char(34)+'>'+left(b.p28Name,30)+'</a></div>'
FROM p28Contact a INNER JOIN p28Contact b ON a.p28ID=b.p28ParentID
where a.p28ID=@p28id
ORDER BY b.p28Name



RETURN(@s)
   
END




























GO

----------FN---------------p28_getonerole_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_getonerole_inline') and type = 'FN')
 drop function p28_getonerole_inline
GO


CREATE FUNCTION [dbo].[p28_getonerole_inline](@p28id int,@x67id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení jedné klientské role @x67id u klienta @p28id
 if @p28id is null
  RETURN(null)

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02LastName+' '+j02.j02FirstName,'')+isnull(' '+j11.j11Name,''))
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p28id AND x67.x67ID=@x67id
  


RETURN(@s)
   
END

GO

----------FN---------------p28_getroles_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_getroles_inline') and type = 'FN')
 drop function p28_getroles_inline
GO












CREATE    FUNCTION [dbo].[p28_getroles_inline](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení klientských rolí v klientovi @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))+' ('+x67Name+')'
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p28id AND x67.x29ID=328
  ORDER BY x67Ordinary


RETURN(@s)
   
END


























GO

----------FN---------------p28_ko_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_ko_inline') and type = 'FN')
 drop function p28_ko_inline
GO


CREATE    FUNCTION [dbo].[p28_ko_inline](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené názvy kontaktní osob

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+b.j02LastName+' '+b.j02FirstName
  FROM p30Contact_Person a INNER JOIN j02Person b ON a.j02ID=b.j02ID
  WHERE a.p28ID=@p28id



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
  ---vrací čárkou oddělené kontaktní média z profilu kontaktu @p28id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+'['+b.o33Name+'] '+isnull(a.o32Value,'')+isnull(' ('+a.o32description+')','')
from
o32Contact_Medium a INNER JOIN o33MediumType b ON a.o33ID=b.o33ID
WHERE a.p28ID=@p28id

RETURN(@s)
   
END

GO

----------FN---------------p28_medias_inline_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_medias_inline_invoice') and type = 'FN')
 drop function p28_medias_inline_invoice
GO



CREATE    FUNCTION [dbo].[p28_medias_inline_invoice](@p28id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené fakturační e-mail adresy

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+isnull(a.o32Value,'')+isnull(' ('+a.o32description+')','')
from
o32Contact_Medium a
WHERE a.p28ID=@p28id AND a.o32IsDefaultInInvoice=1

RETURN(@s)
   
END


GO

----------FN---------------p31_getrate_from_p51id-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_getrate_from_p51id') and type = 'FN')
 drop function p31_getrate_from_p51id
GO







CREATE function [dbo].[p31_getrate_from_p51id](@p51id int,@p31id int)
RETURNS FLOAT
AS
BEGIN


 declare @j02id int,@j07id int,@p32id int,@p34id int,@isbillable bit

 if @p51id=-1
  RETURN(select p31Rate_Billing_Orig from p31Worksheet WHERE p31ID=@p31id)

  select @j02id=a.j02ID,@j07id=isnull(j02.j07ID,0),@p32id=a.p32ID,@p34id=p32.p34ID,@isbillable=p32.p32IsBillable
  FROM p31Worksheet a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
  where a.p31ID=@p31id
  
   
  declare @p52id int,@rate_default_amount float,@p51id_master int,@ret_rate float

  select @p51id_master=isnull(p51id_master,0)
  FROM p51PriceList where p51id=@p51id


  --------sazba podle aktivita+uživatel napřímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j02ID=@j02id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return(@ret_rate)



  --------sazba podle aktivita+pozice osoby napřímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j07ID=@j07id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return(@ret_rate)

  --------sazba podle pouze aktivita--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  AND j02ID IS NULL AND j07ID IS NULL
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return(@ret_rate)
    
    
--------sazba podle uživatel bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null
  and j02ID=@j02id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return(@ret_rate)


--------sazba podle pozice osoby bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null
  and j07ID=@j07id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return(@ret_rate)

--------sazba podle sheet bez aktivity i personálního zdroje--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null and j02ID is null AND j07id is null
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return(@ret_rate)

declare @p33id int

select @p33id=b.p33ID FROM p32Activity a INNER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE a.p32ID=@p32id

if @p33id=1
 begin
-------ještě možnost, že sazba je definována pro všechny časové sešity přes volbu p52IsPlusAllTimeSheets=1
--------sazba podle osoby bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p52IsPlusAllTimeSheets=1 and p32id is null and j02ID=@j02id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return(@ret_rate)

--------sazba podle pozice bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p52IsPlusAllTimeSheets=1 and p32id is null and j07ID=@j07id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return(@ret_rate)

  --------sazba podle sheet bez aktivity i personálního zdroje--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p52IsPlusAllTimeSheets=1 and p32id is null and j02ID is null AND j07id is null
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return(@ret_rate)
 end

----zde už se vrací výchozí sazba ceníku-----------
select @ret_rate=isnull(p51DefaultRateT,0) FROM p51PriceList WHERE p51ID=@p51id

return(@ret_rate)

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

----------FN---------------p32_get_invoice_worksheet_text-------------------------

if exists (select 1 from sysobjects where  id = object_id('p32_get_invoice_worksheet_text') and type = 'FN')
 drop function p32_get_invoice_worksheet_text
GO





CREATE FUNCTION [dbo].[p32_get_invoice_worksheet_text](@p91id int,@p32id int)
RETURNS nvarchar(200) AS  
BEGIN 
---vrací název aktivity @p32id ve správném fakturačním jazyku pro fakturu @p91id

declare @p28id int,@p41id int,@p87id int,@ret nvarchar(500),@langindex int



select @p28id=p28ID,@p41id=p41ID_First FROM p91Invoice WHERE p91ID=@p91id

if @p41id is not null
 select @p87id=p87ID FROM p41Project WHERE p41ID=@p41id

if @p87id is null
 select @p87id=p87ID FROM p28Contact WHERE p28ID=@p28id

if @p87id is not null
 select @langindex=p87LangIndex FROM p87BillingLanguage WHERE p87ID=@p87id

if @langindex=1
 select @ret=isnull(p32DefaultWorksheetText_Lang1,p32Name_BillingLang1) FROM p32Activity WHERE p32ID=@p32id

if @langindex=2
 select @ret=isnull(p32DefaultWorksheetText_Lang2,p32Name_BillingLang2) FROM p32Activity WHERE p32ID=@p32id

if @langindex=3
 select @ret=isnull(p32DefaultWorksheetText_Lang3,p32Name_BillingLang3) FROM p32Activity WHERE p32ID=@p32id

if @langindex=4
 select @ret=isnull(p32DefaultWorksheetText_Lang4,p32Name_BillingLang4) FROM p32Activity WHERE p32ID=@p32id

if @ret is null
 select @ret=isnull(p32DefaultWorksheetText,p32Name) FROM p32Activity WHERE p32ID=@p32id


RETURN(@ret)

END









GO

----------FN---------------p32_get_invoicetext-------------------------

if exists (select 1 from sysobjects where  id = object_id('p32_get_invoicetext') and type = 'FN')
 drop function p32_get_invoicetext
GO




CREATE FUNCTION [dbo].[p32_get_invoicetext](@p91id int,@p32id int)
RETURNS nvarchar(200) AS  
BEGIN 
---vrací název aktivity @p32id ve správném fakturačním jazyku pro fakturu @p91id

declare @p28id int,@p41id int,@p87id int,@ret nvarchar(200),@langindex int



select @p28id=p28ID,@p41id=p41ID_First FROM p91Invoice WHERE p91ID=@p91id

if @p41id is not null
 select @p87id=p87ID FROM p41Project WHERE p41ID=@p41id

if @p87id is null
 select @p87id=p87ID FROM p28Contact WHERE p28ID=@p28id

if @p87id is not null
 select @langindex=p87LangIndex FROM p87BillingLanguage WHERE p87ID=@p87id

if @langindex=1
 select @ret=p32Name_BillingLang1 FROM p32Activity WHERE p32ID=@p32id

if @langindex=2
 select @ret=p32Name_BillingLang2 FROM p32Activity WHERE p32ID=@p32id

if @langindex=3
 select @ret=p32Name_BillingLang3 FROM p32Activity WHERE p32ID=@p32id

if @langindex=4
 select @ret=p32Name_BillingLang4 FROM p32Activity WHERE p32ID=@p32id

if @ret is null
 select @ret=p32Name FROM p32Activity WHERE p32ID=@p32id


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

----------FN---------------p41_get_childs_inline_html-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_childs_inline_html') and type = 'FN')
 drop function p41_get_childs_inline_html
GO





CREATE    FUNCTION [dbo].[p41_get_childs_inline_html](@p41id int)
RETURNS varchar(5000)
AS
BEGIN
  ---vrací čárkou html seznam pod-projektů v projektu @p41id

 DECLARE @s nvarchar(2000) 


select top 20 @s=COALESCE(@s + ' ', '')+'<div><a href='+char(34)+'javascript: send_url_to_pane('+char(39)+'p41_framework_detail.aspx?pid='+convert(varchar(10),b.p41ID)+char(39)+')'+char(34)+'>'+left(isnull(b.p41NameShort,b.p41Name),30)+'</a></div>'
FROM p41Project a INNER JOIN p41Project b ON a.p41ID=b.p41ParentID
where a.p41ID=@p41id
ORDER BY isnull(b.p41NameShort,b.p41Name)



RETURN(@s)
   
END




























GO

----------FN---------------p41_get_one_role_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_one_role_inline') and type = 'FN')
 drop function p41_get_one_role_inline
GO







CREATE    FUNCTION [dbo].[p41_get_one_role_inline](@p41id int,@x67id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení jedné projektové role @x67id v projektu @p41id
 if @p41id is null
  RETURN(NULL)

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02LastName+' '+j02.j02FirstName,'')+isnull(' '+j11.j11Name,''))
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p41id AND x67.x67ID=@x67id
  

RETURN(@s)
   
END



























GO

----------FN---------------p41_get_p41code_client_plus_ordinary-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_p41code_client_plus_ordinary') and type = 'FN')
 drop function p41_get_p41code_client_plus_ordinary
GO




CREATE    FUNCTION [dbo].[p41_get_p41code_client_plus_ordinary](@pid int)
RETURNS varchar(50)
AS
BEGIN
  ---vrací kód projektu podle logiky kód klienta + pořadové číslo projektu v rámci klienta

 DECLARE @ret varchar(50),@p28id int,@p28code varchar(50),@pocet int,@suffix varchar(10)

 select @p28id=p28ID_Client,@p28code=b.p28Code From p41Project a INNER JOIN p28Contact b ON a.p28ID_Client=b.p28ID WHERE a.p41ID=@pid

 if @p28id is null
  set @p28code='????'+convert(varchar(10),@pid)

 select @pocet=count(*) FROM p41Project WHERE p28ID_Client=@p28id and p41ID<>@pid

 set @pocet=isnull(@pocet,0)+1
 set @suffix=right('000'+convert(varchar(10),@pocet),3)
 
 set @ret=@p28code+'-'+@suffix
 
 

 WHILE (exists(select p41ID FROM p41Project WHERE p41Code like @ret))
  BEGIN
    set @pocet=@pocet+1
	set @suffix=right('000'+convert(varchar(10),@pocet),3)
	set @ret=@p28code+'-'+@suffix
	if @pocet>=999
	 BREAK
  END



RETURN(@ret)
   
END


























GO

----------FN---------------p41_get_p41code_parentproject_plus_ordinary-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_p41code_parentproject_plus_ordinary') and type = 'FN')
 drop function p41_get_p41code_parentproject_plus_ordinary
GO





CREATE    FUNCTION [dbo].[p41_get_p41code_parentproject_plus_ordinary](@pid int)
RETURNS varchar(50)
AS
BEGIN
  ---vrací kód projektu podle logiky kód nadřízeného projektu + pořadové číslo projektu v rámci nadřízeného projektu

 DECLARE @ret varchar(50),@p41id_master int,@p41code_master varchar(50),@pocet int,@suffix varchar(10)

 select @p41id_master=p41ParentID FROM p41Project WHERE p41ID=@pid

 if @p41id_master is null
  RETURN('?????'+convert(varchar(10),@pid))

 select @p41code_master=p41Code From p41Project where p41ID=@p41id_master


 select @pocet=count(*) FROM p41Project WHERE p41ParentID=@p41id_master and p41ID<>@pid

 set @pocet=isnull(@pocet,0)+1
 set @suffix=right('000'+convert(varchar(10),@pocet),3)
 
 set @ret=@p41code_master+'-'+@suffix
 
 

 WHILE (exists(select p41ID FROM p41Project WHERE p41Code like @ret))
  BEGIN
    set @pocet=@pocet+1
	set @suffix=right('000'+convert(varchar(10),@pocet),3)
	set @ret=@p41code_master+'-'+@suffix
	if @pocet>=999
	 BREAK
  END



RETURN(@ret)
   
END



























GO

----------FN---------------p41_get_p41code_yyyy_mm_dd_xxxx-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_p41code_yyyy_mm_dd_xxxx') and type = 'FN')
 drop function p41_get_p41code_yyyy_mm_dd_xxxx
GO




CREATE    FUNCTION [dbo].[p41_get_p41code_yyyy_mm_dd_xxxx](@pid int)
RETURNS varchar(50)
AS
BEGIN
  ---vrací kód projektu podle logiky YYYY_MM_DD_XXXX

 DECLARE @ret varchar(50),@d datetime,@pocet int,@suffix varchar(10),@s varchar(50)
 set @d=getdate()

 set @s=convert(varchar(10),year(@d))+'_'+right('0'+convert(varchar(10),month(@d)),2)+'_'+right('0'+convert(varchar(10),day(@d)),2)



 select @pocet=count(*) FROM p41Project WHERE p41ID<>@pid

 set @pocet=isnull(@pocet,0)+1
 set @suffix=right('000'+convert(varchar(10),@pocet),4)
 
 set @ret=@s+'_'+@suffix
 
 WHILE (exists(select p41ID FROM p41Project WHERE p41Code like @ret))
  BEGIN
    set @pocet=@pocet+1
	set @suffix=right('000'+convert(varchar(10),@pocet),4)
	set @ret=@s+'-'+@suffix
	if @pocet>=999
	 BREAK
  END



RETURN(@ret)
   
END

GO

----------FN---------------p41_get_tasks_inline_html-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_tasks_inline_html') and type = 'FN')
 drop function p41_get_tasks_inline_html
GO





CREATE    FUNCTION [dbo].[p41_get_tasks_inline_html](@p41id int)
RETURNS varchar(5000)
AS
BEGIN
  ---vrací čárkou html seznam otevřených úkolů v projektu @p41id

 DECLARE @s nvarchar(2000) 


select top 20 @s=COALESCE(@s + ' ', '')+'<div><span class='+char(34)+'badge_square'+char(34)+' style='+char(34)+case when b02.b02Color is null then '' else +'background-color:'+b02.b02Color+';' end+char(34)+'></span><a href='+char(34)+'javascript: send_url_to_pane('+char(39)+'p56_framework_detail.aspx?pid='+convert(varchar(10),a.p56ID)+char(39)+')'+char(34)+'>'+case when len(a.p56Name)>20 then left(a.p56Name,20)+'...' else a.p56Name end+'</a></div>'
FROM p56Task a INNER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID
where a.p41ID=@p41id AND getdate() between a.p56ValidFrom AND a.p56ValidUntil
ORDER BY a.p56Name



RETURN(@s)
   
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
  ---vrací čárkou oddělené obsazení projektových rolí v projektu @p41id

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

----------FN---------------p41_ko_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_ko_inline') and type = 'FN')
 drop function p41_ko_inline
GO


create    FUNCTION [dbo].[p41_ko_inline](@p41id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené názvy kontaktní osob přímo svázané s projektem

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+b.j02LastName+' '+b.j02FirstName
  FROM p30Contact_Person a INNER JOIN j02Person b ON a.j02ID=b.j02ID
  WHERE a.p41ID=@p41id



RETURN(@s)
   
END


GO

----------FN---------------p56_get_one_role_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_get_one_role_inline') and type = 'FN')
 drop function p56_get_one_role_inline
GO







CREATE    FUNCTION [dbo].[p56_get_one_role_inline](@p56id int,@x67id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené obsazení jedné  role @x67id v úkolu @p56id
 if @p56id is null
  RETURN(NULL)

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02LastName+' '+j02.j02FirstName,'')+isnull(' '+j11.j11Name,''))
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p56id AND x67.x67ID=@x67id
  

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
  ---vrací čárkou oddělené obsazení  rolí v úkolu @p56id

 DECLARE @s nvarchar(2000),@count int

 select @count=count(*) FROM x67EntityRole WHERE x29ID=356

 
 if @count>1
 begin
select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))+' ('+x67Name+')'
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p56id AND x67.x29ID=356
  ORDER BY x67Ordinary
 end

  if @count<=1
 begin
select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p56id AND x67.x29ID=356
  ORDER BY x67Ordinary
 end

RETURN(@s)
   
END


























GO

----------FN---------------p82_get_p86id-------------------------

if exists (select 1 from sysobjects where  id = object_id('p82_get_p86id') and type = 'FN')
 drop function p82_get_p86id
GO





CREATE  FUNCTION [dbo].[p82_get_p86id](@p82id int)
RETURNS int
AS
BEGIN
  ---vrací ID bankovního účtu pro úhradu zálohové faktury @p82id

  declare @p86id int,@j27id int,@p93id int

  select @j27id=p90.j27ID,@p93id=p89.p93ID
  FROM p82Proforma_Payment a INNER JOIN p90Proforma p90 ON a.p90ID=p90.p90ID INNER JOIN p89ProformaType p89 ON p90.p89ID=p89.p89ID
  WHERE a.p82ID=@p82id

  RETURN(select p86ID FROM p88InvoiceHeader_BankAccount WHERE j27ID=@j27id AND p93ID=@p93id)

 
   
END




GO

----------FN---------------p84_translate-------------------------

if exists (select 1 from sysobjects where  id = object_id('p84_translate') and type = 'FN')
 drop function p84_translate
GO



CREATE FUNCTION [dbo].[p84_translate](@default nvarchar(255),@langindex int)
RETURNS nvarchar(255) AS  
BEGIN 


if ISNULL(@default,'')=''
 RETURN('?')

if ISNULL(@langindex,0)=0
 RETURN(@default)

declare @ret nvarchar(255)

if @langindex=1
 select @ret=p84Lang1 from p84LanguageExpression where p84Default like @default

if @langindex=2
 select @ret=p84Lang2 from p84LanguageExpression where p84Default like @default

if @langindex=3
 select @ret=p84Lang3 from p84LanguageExpression where p84Default like @default

if @langindex=4
 select @ret=p84Lang4 from p84LanguageExpression where p84Default like @default


if @ret is null
 RETURN(@default)


RETURN(@ret)

END




GO

----------FN---------------p90_get_p86id-------------------------

if exists (select 1 from sysobjects where  id = object_id('p90_get_p86id') and type = 'FN')
 drop function p90_get_p86id
GO



CREATE  FUNCTION [dbo].[p90_get_p86id](@p90id int)
RETURNS int
AS
BEGIN
  ---vrací ID bankovního účtu pro zálohovou fakturu @p90id

  declare @p86id int,@j27id int,@p93id int

  select @j27id=a.j27ID,@p93id=b.p93ID
  FROM p90Proforma a INNER JOIN p89ProformaType b ON a.p89ID=b.p89ID
  WHERE a.p90ID=@p90id

  RETURN(select p86ID FROM p88InvoiceHeader_BankAccount WHERE j27ID=@j27id AND p93ID=@p93id)

 
   
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
  ---vrací ID bankovního účtu pro fakturu @p91id

  declare @p86id int,@j27id int,@p93id int

  select @j27id=a.j27ID,@p93id=b.p93ID
  FROM p91Invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
  WHERE a.p91ID=@p91id

  RETURN(select p86ID FROM p88InvoiceHeader_BankAccount WHERE j27ID=@j27id AND p93ID=@p93id)

 
   
END


GO

----------FN---------------p91_get_p86id_currency-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_get_p86id_currency') and type = 'FN')
 drop function p91_get_p86id_currency
GO






CREATE  FUNCTION [dbo].[p91_get_p86id_currency](@p91id int,@j27id int)
RETURNS int
AS
BEGIN
  ---vrací ID bankovního účtu pro fakturu @p91id a explicitně měnu @j27id

  declare @p86id int,@p93id int

  select @p93id=b.p93ID
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
  if isnull(@x15id,1)=1
   RETURN(0)

  declare @ret float,@p53id int

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID=@j17id and x15id=@x15id and @dat BETWEEN p53validfrom AND p53validuntil ORDER BY p53ValidFrom DESC
  

  if @p53id is null
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID is null and x15id=@x15id and @dat BETWEEN p53validfrom AND p53validuntil ORDER BY p53ValidFrom DESC

  
  if @p53id is not null
    RETURN(@ret)
    
  
  RETURN(-1)
   
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

----------FN---------------p95_get_invoicetext-------------------------

if exists (select 1 from sysobjects where  id = object_id('p95_get_invoicetext') and type = 'FN')
 drop function p95_get_invoicetext
GO



CREATE FUNCTION [dbo].[p95_get_invoicetext](@p91id int,@p95id int)
RETURNS nvarchar(200) AS  
BEGIN 
---vrací název fakturačního oddílu @p95id ve správném fakturačním jazyku pro fakturu @p91id

declare @p28id int,@p41id int,@p87id int,@ret nvarchar(200),@langindex int



select @p28id=p28ID,@p41id=p41ID_First FROM p91Invoice WHERE p91ID=@p91id

if @p41id is not null
 select @p87id=p87ID FROM p41Project WHERE p41ID=@p41id

if @p87id is null
 select @p87id=p87ID FROM p28Contact WHERE p28ID=@p28id

if @p87id is not null
 select @langindex=p87LangIndex FROM p87BillingLanguage WHERE p87ID=@p87id

if @langindex=1
 select @ret=p95Name_BillingLang1 FROM p95InvoiceRow WHERE p95ID=@p95id

if @langindex=2
 select @ret=p95Name_BillingLang2 FROM p95InvoiceRow WHERE p95ID=@p95id

if @langindex=3
 select @ret=p95Name_BillingLang3 FROM p95InvoiceRow WHERE p95ID=@p95id

if @langindex=4
 select @ret=p95Name_BillingLang4 FROM p95InvoiceRow WHERE p95ID=@p95id

if @ret is null
 select @ret=p95Name FROM p95InvoiceRow WHERE p95ID=@p95id


RETURN(@ret)

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



CREATE FUNCTION [dbo].[remove_alphacharacters](@InputString VARCHAR(1000),@leading_zeros_scale int,@ConstantBeforeValue varchar(40),@ConstantAfterValue varchar(40))
RETURNS VARCHAR(1000)
AS
BEGIN
  if isnull(@ConstantBeforeValue,'')<>''
   begin
    if LEFT(@InputString,LEN(@ConstantBeforeValue))=@ConstantBeforeValue
     set @InputString=SUBSTRING(@InputString,1+len(@ConstantBeforeValue),len(@InputString)-len(@ConstantBeforeValue))
   end
   
  if isnull(@ConstantAfterValue,'')<>''
   begin
    if RIGHT(@InputString,LEN(@ConstantAfterValue))=@ConstantAfterValue
     set @InputString=SUBSTRING(@InputString,1,LEN(@InputString)-LEN(@ConstantAfterValue))
   end
  
   
  WHILE PATINDEX('%[^0-9]%',@InputString)>0
        SET @InputString = STUFF(@InputString,PATINDEX('%[^0-9]%',@InputString),1,'')  		  
		
    
  
  IF @leading_zeros_scale>0
   begin
    
	 RETURN right('00000000000000000000000'+@InputString,@leading_zeros_scale)
   end

   RETURN @InputString
END

GO

----------FN---------------stitek_entity-------------------------

if exists (select 1 from sysobjects where  id = object_id('stitek_entity') and type = 'FN')
 drop function stitek_entity
GO







CREATE FUNCTION [dbo].[stitek_entity](@o23id int,@x20id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené názvy entit
  
 DECLARE @s nvarchar(2000) 


select top 10 @s=COALESCE(@s + ', ', '')+dbo.GetObjectAlias(convert(varchar(10),x20.x29ID),a.x19RecordPID)
FROM x19EntityCategory_Binding a INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID
where a.o23ID=@o23id AND a.x20ID=@x20id



RETURN(@s)
   
END



























GO

----------FN---------------stitek_entity_pid-------------------------

if exists (select 1 from sysobjects where  id = object_id('stitek_entity_pid') and type = 'FN')
 drop function stitek_entity_pid
GO




CREATE FUNCTION [dbo].[stitek_entity_pid](@o23id int,@x29id int)
RETURNS int
AS
BEGIN
  ---vrací PID entity v dokumentu
  
 DECLARE @ret int,@x20id int,@x18id int

 select @x18id=c.x18ID FROM o23Doc a INNER JOIN x23EntityField_Combo b ON a.x23ID=b.x23ID INNER JOIN x18EntityCategory c ON b.x23ID=c.x23ID WHERE a.o23ID=@o23id

 select @x20id=x20ID FROM x20EntiyToCategory WHERE x18ID=@x18id AND x29ID=@x29id


select top 1 @ret=a.x19RecordPID
FROM x19EntityCategory_Binding a INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID
where a.o23ID=@o23id AND a.x20ID=@x20id



RETURN(@ret)
   
END





























GO

----------FN---------------stitek_entity_pid_by_x20id-------------------------

if exists (select 1 from sysobjects where  id = object_id('stitek_entity_pid_by_x20id') and type = 'FN')
 drop function stitek_entity_pid_by_x20id
GO


CREATE FUNCTION [dbo].[stitek_entity_pid_by_x20id](@o23id int,@x20id int)
RETURNS int
AS
BEGIN
  ---vrací PID entity v dokumentu
  
 DECLARE @ret int


select top 1 @ret=a.x19RecordPID
FROM x19EntityCategory_Binding a INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID
where a.o23ID=@o23id AND a.x20ID=@x20id



RETURN(@ret)
   
END


GO

----------FN---------------stitek_entity_report-------------------------

if exists (select 1 from sysobjects where  id = object_id('stitek_entity_report') and type = 'FN')
 drop function stitek_entity_report
GO









CREATE FUNCTION [dbo].[stitek_entity_report](@o23id int,@x29id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené názvy entit
  
 DECLARE @s nvarchar(2000),@x20id int,@x18id int

 select @x18id=c.x18ID FROM o23Doc a INNER JOIN x23EntityField_Combo b ON a.x23ID=b.x23ID INNER JOIN x18EntityCategory c ON b.x23ID=c.x23ID WHERE a.o23ID=@o23id

 select @x20id=x20ID FROM x20EntiyToCategory WHERE x18ID=@x18id AND x29ID=@x29id


select top 10 @s=COALESCE(@s + ', ', '')+dbo.GetObjectAlias(convert(varchar(10),x20.x29ID),a.x19RecordPID)
FROM x19EntityCategory_Binding a INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID
where a.o23ID=@o23id AND a.x20ID=@x20id



RETURN(@s)
   
END




























GO

----------FN---------------stitek_hodnoty-------------------------

if exists (select 1 from sysobjects where  id = object_id('stitek_hodnoty') and type = 'FN')
 drop function stitek_hodnoty
GO







CREATE    FUNCTION [dbo].[stitek_hodnoty](@x18id int,@x29id int,@recpid int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené hodnoty štítků

 DECLARE @s nvarchar(2000) 


select top 10 @s=COALESCE(@s + ', ', '')+isnull(o23.o23Code,o23.o23Name)
FROM x19EntityCategory_Binding a INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID
where a.x19RecordPID=@recpid AND x20.x18ID=@x18id and x20.x29ID=@x29id



RETURN(@s)
   
END



























GO

----------FN---------------tag_values_inline-------------------------

if exists (select 1 from sysobjects where  id = object_id('tag_values_inline') and type = 'FN')
 drop function tag_values_inline
GO



CREATE    FUNCTION [dbo].[tag_values_inline](@x29id int,@recpid int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené hodnoty štítků

 DECLARE @s nvarchar(2000) 


select top 10 @s=COALESCE(@s + ', ', '')+o51.o51Name
FROM o52TagBinding a INNER JOIN o51Tag o51 ON a.o51ID=o51.o51ID
where a.o52RecordPID=@recpid AND a.x29ID=@x29id
ORDER BY o51.o51Name



RETURN(@s)
   
END



























GO

----------FN---------------tag_values_inline_html-------------------------

if exists (select 1 from sysobjects where  id = object_id('tag_values_inline_html') and type = 'FN')
 drop function tag_values_inline_html
GO


CREATE    FUNCTION [dbo].[tag_values_inline_html](@x29id int,@recpid int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací čárkou oddělené hodnoty štítků

 DECLARE @s nvarchar(2000) 


select top 10 @s=COALESCE(@s + ' ', '')+'<div class='+char(34)+'badge_tag'+char(34)+'style='+char(34)+case when o51.o51BackColor is null then '' else +'background-color:'+o51.o51BackColor+';' end+case when o51.o51ForeColor is null then '' else+'color:'+o51.o51ForeColor end+';'+char(34)+'>'+o51.o51Name+'</div>'
FROM o52TagBinding a INNER JOIN o51Tag o51 ON a.o51ID=o51.o51ID
where a.o52RecordPID=@recpid AND a.x29ID=@x29id
ORDER BY o51.o51Name



RETURN(@s)
   
END




























GO

----------FN---------------x28_getFirstUsableField-------------------------

if exists (select 1 from sysobjects where  id = object_id('x28_getFirstUsableField') and type = 'FN')
 drop function x28_getFirstUsableField
GO




CREATE FUNCTION [dbo].[x28_getFirstUsableField] (@entityprefix varchar(50),@x28datatype varchar(50),@x23id int)
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

if isnull(@x23id,0)<>0
 set @mask=@entityprefix+'freecombo%'

select @id=[id]
from sysobjects
where [name] like @table

if isnull(@id,0)=0
  return(null)

select top 1 @ret=[name]
from
dbo.syscolumns
where [name] like @mask and [name] not like '%FreeCombo%Text%' and [id]=@id and upper([name]) not in (select upper(x28field) from x28EntityField where x29id=@x29id and x28Field is not null)

RETURN(@ret)


END


























GO

----------FN---------------x38_get_freecode-------------------------

if exists (select 1 from sysobjects where  id = object_id('x38_get_freecode') and type = 'FN')
 drop function x38_get_freecode
GO


CREATE FUNCTION [dbo].[x38_get_freecode](@x38id int,@x29id int,@datapid int,@isdraft bit,@attempt_number int)
RETURNS varchar(50) AS  
BEGIN 

declare @x38ExplicitIncrementStart int,@code_new varchar(50),@val int,@code_max_used varchar(50)
declare @x38Scale int,@x38ConstantBeforeValue varchar(50),@pid_last int,@x38ConstantAfterValue varchar(40),@x38IsUseDbPID bit

set @val=0
set @isdraft=isnull(@isdraft,0)

if @x29id is null
 RETURN('')	----bez ID entity nelze generovat kód

if @x29id<>328
 begin		---vyjímku má pouze entita Klient
  if @x38id is null and @isdraft=0
   RETURN('')	---pokud na vstupu chybí číselná řada x38id, pak musí být záznam v režimu DRAFT. U normálních záznamů nesmí x38id chybět
 end

if @x38id is not null
 begin
	select @x38ExplicitIncrementStart=isnull(x38ExplicitIncrementStart,0)
	,@x38ConstantBeforeValue=isnull(x38ConstantBeforeValue,''),@x38Scale=x38Scale
	,@x38ConstantAfterValue=isnull(x38ConstantAfterValue,''),@x38IsUseDbPID=x38IsUseDbPID
	FROM x38CodeLogic
	WHERE x38ID=@x38id

	if @x38IsUseDbPID=1
	 RETURN(convert(varchar(10),@datapid))	---číslování podle hodnoty primárního klíče v databázi
 end
else
 begin
	set @x38ExplicitIncrementStart=0
	set @x38ConstantBeforeValue='DRAFT'
	set @x38Scale=5
	set @x38ConstantAfterValue=''	

	if @x29id=328 and @isdraft=0
	 begin	----klient bez přiřazení typu
		set @x38ExplicitIncrementStart=0
		set @x38ConstantBeforeValue='K'
		set @x38Scale=4
		set @x38ConstantAfterValue=''	
	 end
 end





if isnull(@x38Scale,0)=0
 set @x38Scale=4


if @x29id=141 and @isdraft=0	---projekt
 select @pid_last=max(p41ID),@code_max_used=max(dbo.remove_alphacharacters(p41code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p41Project where p41IsDraft=0 AND p41Code NOT LIKE 'TEMP%' and p41Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p42ID IN (SELECT p42ID FROM p42ProjectType WHERE x38ID=@x38id)

if @x29id=141 and @isdraft=1	---projekt DRAFT, x38id může být prázdné
 select @pid_last=max(p41ID),@code_max_used=max(dbo.remove_alphacharacters(p41code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p41Project where p41IsDraft=1 AND p41Code NOT LIKE 'TEMP%' and p41Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p42ID IN (SELECT p42ID FROM p42ProjectType WHERE x38ID_Draft=@x38id OR x38ID_Draft IS NULL)


if @x29id=328
 begin
	declare @p29id int
	select @p29id=p29ID FROM p28Contact WHERE p28ID=@datapid	---u klienta nemusí být povinně vyplněný TYP klienta, který s sebou nese číselnou řadu


	if @p29id is not null and @isdraft=0
	  select @pid_last=max(p28ID),@code_max_used=max(dbo.remove_alphacharacters(p28code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p28Contact where p28IsDraft=0 AND p28Code NOT LIKE 'TEMP%' and p28Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND (p29ID IS NULL OR p29ID IN (SELECT p29ID FROM p29ContactType WHERE x38ID=@x38id))

	if @p29id is null and @isdraft=0	---zde chybí zcela vazba na číselnou řadu, protože typ klienta není povinné pole
	 select @pid_last=max(p28ID),@code_max_used=max(dbo.remove_alphacharacters(p28code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p28Contact where p28IsDraft=0 AND p28Code NOT LIKE 'TEMP%' and p28Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue

	
	if @p29id is not null and @isdraft=1
	  select @pid_last=max(p28ID),@code_max_used=max(dbo.remove_alphacharacters(p28code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p28Contact where p28IsDraft=1 AND p28Code NOT LIKE 'TEMP%' and p28Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND (p29ID IS NULL OR p29ID IN (SELECT p29ID FROM p29ContactType WHERE x38ID_Draft=@x38id OR x38ID_Draft IS NULL))

	if @p29id is null and @isdraft=1
	  select @pid_last=max(p28ID),@code_max_used=max(dbo.remove_alphacharacters(p28code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p28Contact where p28IsDraft=1 AND p28Code NOT LIKE 'TEMP%' and p28Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue

	
 end
   

if @x29id=356	---úkol (technicky nemůže být DRAFT a musí existovat p57id a tím i x38id
 select @pid_last=max(p56ID),@code_max_used=max(dbo.remove_alphacharacters(p56code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p56Task where p56Code NOT LIKE 'TEMP%' and p56Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p57ID IN (SELECT p57ID FROM p57TaskType WHERE x38ID=@x38id)

if @x29id=382	---úhrada zálohové faktury
 select @pid_last=max(p82ID),@code_max_used=max(dbo.remove_alphacharacters(p82Code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p82Proforma_Payment where p82Code NOT LIKE 'TEMP%' and p82Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue


if @x29id=223 and @isdraft=0	---dokument
 select @pid_last=max(o23ID),@code_max_used=max(dbo.remove_alphacharacters(o23code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM o23Doc where o23IsDraft=0 AND o23Code NOT LIKE 'TEMP%' and o23Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND x23ID IN (SELECT b.x23ID FROM x23EntityField_Combo a INNER JOIN x18EntityCategory b ON a.x23ID=b.x23ID WHERE b.x38ID=@x38id)

if @x29id=223 and @isdraft=1	---dokument DRAFT
 select @pid_last=max(o23ID),@code_max_used=max(dbo.remove_alphacharacters(o23code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM o23Doc where o23IsDraft=1


if @x29id=391 and @isdraft=0	---faktura
 select @pid_last=max(p91ID),@code_max_used=max(dbo.remove_alphacharacters(p91code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p91Invoice where p91IsDraft=0 AND p91Code NOT LIKE 'TEMP%' AND p91Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p92ID IN (SELECT p92ID FROM p92InvoiceType WHERE x38ID=@x38id)

if @x29id=391 and @isdraft=1	---faktura DRAFT
 select @pid_last=max(p91ID),@code_max_used=max(dbo.remove_alphacharacters(p91code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p91Invoice where p91IsDraft=1 AND p91Code NOT LIKE 'TEMP%' AND p91Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p92ID IN (SELECT p92ID FROM p92InvoiceType WHERE x38ID_Draft=@x38id OR x38ID_Draft is null)

if @x29id=390 and @isdraft=0	---zálohová faktura
 select @pid_last=max(p90ID),@code_max_used=max(dbo.remove_alphacharacters(p90code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p90Proforma where p90IsDraft=0 AND p90Code NOT LIKE 'TEMP%' AND p90Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p89ID IN (SELECT p89ID FROM p89ProformaType WHERE x38ID=@x38id)

if @x29id=390 and @isdraft=1	---zálohová faktura DRAFT
 select @pid_last=max(p90ID),@code_max_used=max(dbo.remove_alphacharacters(p90code,@x38Scale,@x38ConstantBeforeValue,@x38ConstantAfterValue)) FROM p90Proforma where p90IsDraft=1 AND p90Code NOT LIKE 'TEMP%' AND p90Code LIKE @x38ConstantBeforeValue+'%'+@x38ConstantAfterValue AND p89ID IN (SELECT p89ID FROM p89ProformaType WHERE x38ID_Draft=@x38id or x38ID_Draft is null)

	
if @code_max_used is not null
begin	---existuje předchozí záznam pro navázání řady
	if ISNUMERIC(@code_max_used)=1
	 set @val=convert(int,@code_max_used)
end

set @val=@val+1		---nový kód bude o jedničku větší
  
if @val<=@x38ExplicitIncrementStart
 set @val=@x38ExplicitIncrementStart


set @code_new=@x38ConstantBeforeValue+right('0000000000'+convert(varchar(10),@val),@x38Scale)+@x38ConstantAfterValue
return(@code_new)


END


GO

----------FN---------------x69_exist_role_in_record-------------------------

if exists (select 1 from sysobjects where  id = object_id('x69_exist_role_in_record') and type = 'FN')
 drop function x69_exist_role_in_record
GO









CREATE    FUNCTION [dbo].[x69_exist_role_in_record](@x67id int,@j02id int,@record_pid int)
RETURNS BIT
AS
BEGIN
  ---vrací 1, pokud osoba @j02id má u záznamu @record_pid roli @x67id

 if exists(select x69ID FROM x69EntityRole_Assign WHERE x67ID=@x67id AND j02ID=@j02id AND x69RecordPID=@record_pid)
  RETURN(1)
 
 if exists(select x69ID FROM x69EntityRole_Assign WHERE x67ID=@x67id AND j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id) AND x69RecordPID=@record_pid)
  RETURN(1)

 RETURN(0)
   
end






















GO

----------IF---------------SplitString-------------------------

if exists (select 1 from sysobjects where  id = object_id('SplitString') and type = 'IF')
 drop function SplitString
GO





create function [dbo].[SplitString] 
    (
        @str nvarchar(4000), 
        @separator char(1)
    )
    returns table
    AS
    return (
        with tokens(p, a, b) AS (
            select 
                1, 
                1, 
                charindex(@separator, @str)
            union all
            select
                p + 1, 
                b + 1, 
                charindex(@separator, @str, b + 1)
            from tokens
            where b > 0
        )
        select
            p-1 zeroBasedOccurance,
            substring(
                @str, 
                a, 
                case when b > 0 then b-a ELSE 4000 end) 
            AS s
        from tokens
      )



GO

----------IF---------------tview_j02_notinvoiced-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_j02_notinvoiced') and type = 'IF')
 drop function tview_j02_notinvoiced
GO



CREATE function [dbo].[tview_j02_notinvoiced] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select a.j02ID
,sum(case when a.p71ID IS NULL AND p32.p32IsBillable=1 THEN a.p31Hours_Orig when a.p71ID=1 THEN a.p31Hours_Approved_Billing end) as Hodiny
,sum(case when a.p71ID IS NULL AND a.p31Amount_WithoutVat_Orig<>0 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Castka_Celkem
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 then p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Honorar
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_EUR
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN a.p31Amount_WithoutVat_Approved end) as Vydaje
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Vydaje_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Vydaje_EUR
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Odmeny_EUR
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.j02ID

      )



GO

----------IF---------------tview_j02_wip-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_j02_wip') and type = 'IF')
 drop function tview_j02_wip
GO



CREATE function [dbo].[tview_j02_wip] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select a.j02ID
,sum(a.p31Hours_Orig) as Hodiny
,sum(case when a.p31Amount_WithoutVat_Orig<>0 then a.p31Amount_WithoutVat_Orig end) as Castka_Celkem
,sum(case when p34.p33ID=1 then p31Amount_WithoutVat_Orig end) as Honorar
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Honorar_CZK
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Honorar_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then a.p31Amount_WithoutVat_Orig end) as Vydaje
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Vydaje_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Vydaje_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig end) as Odmeny
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Odmeny_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Odmeny_EUR
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p71ID IS NULL and a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.j02ID

      )



GO

----------IF---------------tview_p28_approved-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p28_approved') and type = 'IF')
 drop function tview_p28_approved
GO






CREATE function [dbo].[tview_p28_approved] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select p41.p28ID_Client as p28ID
,sum(a.p31Hours_Approved_Billing) as Hodiny
,sum(a.p31Amount_WithoutVat_Approved) as Castka_Celkem
,sum(case when a.p71ID=1 AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Honorar
,sum(case when a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_CZK
,sum(case when a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_EUR
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN a.p31Amount_WithoutVat_Approved end) as Vydaje
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Vydaje_CZK
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Vydaje_EUR
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny_CZK
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Odmeny_EUR
,count(a.p41ID) as PocetProjektu
from
p31WorkSheet a
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE p41.p28ID_Client IS NOT NULL AND a.p71ID=1 AND a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY p41.p28ID_Client

      )





GO

----------IF---------------tview_p28_notinvoiced-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p28_notinvoiced') and type = 'IF')
 drop function tview_p28_notinvoiced
GO





CREATE function [dbo].[tview_p28_notinvoiced] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select p41.p28ID_Client as p28ID
,sum(case when a.p71ID IS NULL AND p32.p32IsBillable=1 THEN a.p31Hours_Orig when a.p71ID=1 THEN a.p31Hours_Approved_Billing end) as Hodiny
,sum(case when a.p71ID IS NULL AND a.p31Amount_WithoutVat_Orig<>0 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Castka_Celkem
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 then p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Honorar
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_EUR
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN a.p31Amount_WithoutVat_Approved end) as Vydaje
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Vydaje_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Vydaje_EUR
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Odmeny_EUR
,count(a.p41ID) as PocetProjektu
from
p31WorkSheet a
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE p41.p28ID_Client IS NOT NULL AND a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY p41.p28ID_Client

      )




GO

----------IF---------------tview_p28_wip-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p28_wip') and type = 'IF')
 drop function tview_p28_wip
GO




CREATE function [dbo].[tview_p28_wip] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select p41.p28ID_Client as p28ID
,sum(a.p31Hours_Orig) as Hodiny
,sum(case when a.p31Amount_WithoutVat_Orig<>0 then a.p31Amount_WithoutVat_Orig end) as Castka_Celkem
,sum(case when p34.p33ID=1 then p31Amount_WithoutVat_Orig end) as Honorar
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Honorar_CZK
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Honorar_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then a.p31Amount_WithoutVat_Orig end) as Vydaje
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig end) as Odmeny
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Odmeny_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Odmeny_EUR
,count(a.p41ID) as PocetProjektu
from
p31WorkSheet a
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE p41.p28ID_Client IS NOT NULL AND a.p71ID IS NULL AND a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY p41.p28ID_Client

      )



GO

----------IF---------------tview_p28_worksheet-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p28_worksheet') and type = 'IF')
 drop function tview_p28_worksheet
GO



CREATE function [dbo].[tview_p28_worksheet] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select p41.p28ID_Client as p28ID
,sum(a.p31Hours_Orig) as Vykazano_Hodiny
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN a.p31Amount_WithoutVat_Orig END) as Vykazano_Vydaje
,sum(a.p31Hours_Invoiced) as Vyfakturovano_Hodiny
,sum(a.p31Amount_WithoutVat_Invoiced_Domestic) as Vyfakturovano_Celkem_Domestic
,sum(case when p34.p33ID=1 THEN a.p31Amount_WithoutVat_Invoiced_Domestic END) as Vyfakturovano_Hodiny_Domestic
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN a.p31Amount_WithoutVat_Invoiced_Domestic END) as Vyfakturovano_Odmeny_Domestic
,max(p91.p91Date) as Vyfakturovano_Naposledy_Kdy
,min(p91.p91Date) as Vyfakturovano_Poprve_Kdy
,count(distinct p91.p91ID) as Vyfakturovano_PocetFaktur
from
p31WorkSheet a
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
LEFT OUTER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
WHERE p41.p28ID_Client IS NOT NULL AND p31Date BETWEEN @d1 AND @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY p41.p28ID_Client

      )


GO

----------IF---------------tview_p41_approved-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p41_approved') and type = 'IF')
 drop function tview_p41_approved
GO



CREATE function [dbo].[tview_p41_approved] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select a.p41ID
,sum(a.p31Hours_Approved_Billing) as Hodiny
,sum(a.p31Amount_WithoutVat_Approved) as Castka_Celkem
,sum(case when a.p71ID=1 AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Honorar
,sum(case when a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_CZK
,sum(case when a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_EUR
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN a.p31Amount_WithoutVat_Approved end) as Vydaje
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Vydaje_CZK
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Vydaje_EUR
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny_CZK
,sum(case when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Odmeny_EUR
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p71ID=1 AND a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.p41ID

      )



GO

----------IF---------------tview_p41_notinvoiced-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p41_notinvoiced') and type = 'IF')
 drop function tview_p41_notinvoiced
GO


CREATE function [dbo].[tview_p41_notinvoiced] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select a.p41ID
,sum(case when a.p71ID IS NULL AND p32.p32IsBillable=1 THEN a.p31Hours_Orig when a.p71ID=1 THEN a.p31Hours_Approved_Billing end) as Hodiny
,sum(case when a.p71ID IS NULL AND a.p31Amount_WithoutVat_Orig<>0 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Castka_Celkem
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 then p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Approved end) as Honorar
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID=1 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID=1 AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Approved end) as Honorar_EUR
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then a.p31Amount_WithoutVat_Orig WHEN a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN a.p31Amount_WithoutVat_Approved end) as Vydaje
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Vydaje_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Vydaje_EUR
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Approved end) as Odmeny_CZK
,sum(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig when a.p71ID=1 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Approved end) as Odmeny_EUR
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.p41ID

      )


GO

----------IF---------------tview_p41_wip-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p41_wip') and type = 'IF')
 drop function tview_p41_wip
GO


CREATE function [dbo].[tview_p41_wip] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select a.p41ID
,sum(a.p31Hours_Orig) as Hodiny
,sum(case when a.p31Amount_WithoutVat_Orig<>0 then a.p31Amount_WithoutVat_Orig end) as Castka_Celkem
,sum(case when p34.p33ID=1 then p31Amount_WithoutVat_Orig end) as Honorar
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Honorar_CZK
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Honorar_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then a.p31Amount_WithoutVat_Orig end) as Vydaje
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Vydaje_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Vydaje_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig end) as Odmeny
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=2 then a.p31Amount_WithoutVat_Orig end) as Odmeny_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=3 then a.p31Amount_WithoutVat_Orig end) as Odmeny_EUR
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p71ID IS NULL and a.p91ID IS NULL AND p31Date between @d1 and @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.p41ID

      )


GO

----------IF---------------tview_p41_worksheet-------------------------

if exists (select 1 from sysobjects where  id = object_id('tview_p41_worksheet') and type = 'IF')
 drop function tview_p41_worksheet
GO


CREATE function [dbo].[tview_p41_worksheet] 
    (
        @d1 datetime,
		@d2 datetime
    )
    returns table
    AS
    return (
        select a.p41ID
,sum(a.p31Hours_Orig) as Vykazano_Hodiny
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN a.p31Amount_WithoutVat_Orig END) as Vykazano_Vydaje
,sum(a.p31Hours_Invoiced) as Vyfakturovano_Hodiny
,sum(a.p31Amount_WithoutVat_Invoiced_Domestic) as Vyfakturovano_Celkem_Domestic
,sum(case when p34.p33ID=1 THEN a.p31Amount_WithoutVat_Invoiced_Domestic END) as Vyfakturovano_Hodiny_Domestic
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN a.p31Amount_WithoutVat_Invoiced_Domestic END) as Vyfakturovano_Odmeny_Domestic
,max(p91.p91Date) as Vyfakturovano_Naposledy_Kdy
,min(p91.p91Date) as Vyfakturovano_Poprve_Kdy
,count(distinct p91.p91ID) as Vyfakturovano_PocetFaktur
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
LEFT OUTER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
WHERE p31Date BETWEEN @d1 AND @d2 AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.p41ID

      )

GO

----------P---------------b01_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('b01_delete') and type = 'P')
 drop procedure b01_delete
GO






CREATE   procedure [dbo].[b01_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--b01id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu workflow šablony z tabulky b01workflowtemplate
 
if exists(select b01ID from p42ProjectType where b01id=@pid)
 set @err_ret='Minimálně jeden typ projektu má vazbu na tuto šablonu.'

if exists(select b01ID from p57TaskType where b01id=@pid)
 set @err_ret='Minimálně jeden typ úkolu má vazbu na tuto šablonu.'

if exists(select b01ID from x18EntityCategory where b01id=@pid)
 set @err_ret='Minimálně jeden štítek má vazbu na tuto šablonu.'
  
if isnull(@err_ret,'')<>''
 return 
 
declare @err_b02 varchar(500),@b02id int
 
DECLARE cur2 CURSOR FOR 
SELECT b02ID FROM b02WorkflowStatus WHERE b01ID=@pid

OPEN cur2
FETCH NEXT FROM cur2 
INTO @b02id
WHILE @@FETCH_STATUS = 0
BEGIN
  
  EXEC b02_delete @j03id_sys,@b02id,@err_b02 OUTPUT

  if @err_b02 is not null
   begin
    set @err_ret=@err_b02
    return
   end
   
  FETCH NEXT FROM cur2 
  INTO @b02id
END
CLOSE cur2
DEALLOCATE cur2   

if exists(select b65ID FROM b65WorkflowMessage where b01ID=@pid)
 DELETE FROM b65WorkflowMessage WHERE b01ID=@pid

delete from b01WorkflowTemplate where b01id=@pid









GO

----------P---------------b02_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('b02_delete') and type = 'P')
 drop procedure b02_delete
GO






CREATE   procedure [dbo].[b02_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--b02id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu statusu z tabulky b02WorkflowStatus

if exists(select p41ID from p41Project where b02ID=@pid)
begin
 declare @s varchar(20)
 select TOP 1 @s=isnull(p41Code,'') FROM p41Project WHERE b02ID=@pid
 
 set @err_ret='Minimálně jeden projekt ('+@s+') má vazbu na tento workflow stav.'
 return
end

if exists(select p56ID from p56Task where b02ID=@pid)
begin
 set @err_ret='Minimálně jeden úkol má vazbu na tento workflow stav.'
 return
end

if exists(select o23ID from o23Doc where b02ID=@pid)
begin
 set @err_ret='Minimálně jeden dokument má vazbu na tento workflow stav.'
 return
end


declare @b06id int,@err_b06 varchar(500)

DECLARE cur1 CURSOR FOR 
SELECT b06id FROM b06WorkflowStep WHERE b02ID=@pid OR b02ID_Target=@pid

OPEN cur1
FETCH NEXT FROM cur1 
INTO @b06id
WHILE @@FETCH_STATUS = 0
BEGIN
  
  EXEC b06_delete @j03id_sys,@b06id,@err_b06 OUTPUT

  if @err_b06 is not null
   begin
    set @err_ret=@err_b06
    return
   end
   
  FETCH NEXT FROM cur1 
  INTO @b06id
END
CLOSE cur1
DEALLOCATE cur1  
 
  
delete from b02WorkflowStatus where b02id=@pid




GO

----------P---------------b06_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('b06_delete') and type = 'P')
 drop procedure b06_delete
GO




CREATE   procedure [dbo].[b06_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--b06id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu workflow kroku z tabulky b06WorkflowStep


if isnull(@err_ret,'')<>''
 return 

if exists(select b08ID from b08WorkflowReceiverToStep where b06ID=@pid)
 delete from b08WorkflowReceiverToStep where b06ID=@pid  
 
if exists(select b10id from b10WorkflowCommandCatalog_Binding where b06ID=@pid)
 delete from b10WorkflowCommandCatalog_Binding where b06ID=@pid 
 
if exists(select b11ID FROM b11WorkflowMessageToStep where b06ID=@pid)
 delete from b11WorkflowMessageToStep where b06ID=@pid 
 


if exists(select b05id from b05Workflow_History WHERE b06id=@pid)
 DELETE FROM b05Workflow_History WHERE b06ID=@pid

  

delete from b06WorkflowStep where b06ID=@pid











GO

----------P---------------b07_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('b07_delete') and type = 'P')
 drop procedure b07_delete
GO




CREATE   procedure [dbo].[b07_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--b07id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu komentáře z tabulky b07Comment
if exists(select b07ID from b07Comment WHERE b07ID_Parent=@pid)
 set @err_ret='Existuje minimálně jeden podřízený komentář!'

if exists(select b05ID from b05Workflow_History WHERE b07ID=@pid)
 set @err_ret='Nelze odstranit, protože má vazbu na historii workflow stavového mechanismu!'



if isnull(@err_ret,'')<>''
 return 

if exists(select o27ID FROM o27Attachment WHERE b07ID=@pid)
 delete from o27Attachment WHERE b07ID=@pid

if exists(select b05id from b05Workflow_History WHERE b07ID=@pid)
 DELETE FROM b07Comment WHERE b07ID=@pid

  

delete from b07Comment where b07ID=@pid












GO

----------P---------------b65_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('b65_delete') and type = 'P')
 drop procedure b65_delete
GO







CREATE   procedure [dbo].[b65_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--b65id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu notifikační zprávy z tabulky b65WorkflowMessage
 
if exists(select b11ID from b11WorkflowMessageToStep where b65id=@pid)
 set @err_ret='Minimálně jeden workflow krok je svázán s touto notifikační zprávou.' 
 
if isnull(@err_ret,'')<>''
 return 


delete from b65WorkflowMessage where b65id=@pid









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
	set @name='Čtvrtletí '+convert(varchar(4),@year)+'-'+convert(char(1),@quarter)
  end

if @c11level=3
  begin
  	set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-000000-00'
	set @name='Měsíc '+convert(varchar(4),@year)+'-'+convert(varchar(2),@month)
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
      begin	---měsíce
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

---generování dnů---------
set @d1=convert(datetime,'01.01.'+convert(varchar(4),@year))

while year(@d1)=@year
  begin
    
    EXEC c11_insertrec 5, @d1, @d1,-1,-1,-1,-1
    set @d1=dateadd(day,1,@d1)
  end


---narovnání c11parentid dnů vůči týdnům---
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

---závěrečné čištění
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
---aktualizovat rozpis dnů ve fondu do regionálních instancí kalendářů
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
@j03id_sys int				--přihlášený uživatel
,@pid int					--c21id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu pracovního kalendáře z tabulky c21FondCalendar
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE c21ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna osoba má vazbu na tento pracovní kalendář ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


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
---aktualizovat rozpis dnů ve fondu, kam má vliv den svátku

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--c26id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu dnes svátku z tabulky c21FondCalendar


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

if exists(select j03ID FROM j03User WHERE j02ID=@j02id)
 UPDATE a set j03ValidFrom=b.j02ValidFrom,j03ValidUntil=b.j02ValidUntil FROM j03User a INNER JOIN j02Person b ON a.j02ID=b.j02ID WHERE a.j02ID=@j02id


exec dbo.j02_append_log @j02id



exec [j02_recovery]




GO

----------P---------------j02_append_log-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_append_log') and type = 'P')
 drop procedure j02_append_log
GO


CREATE    PROCEDURE [dbo].[j02_append_log]
@j02id int
AS

INSERT INTO [dbo].[j02Person_Log]
           ([j02ID]
           ,[j07ID]
           ,[c21ID]
           ,[j18ID]
           ,[j17ID]
           ,[o40ID]
           ,[p72ID_NonBillable]
           ,[j02FirstName]
           ,[j02LastName]
           ,[j02TitleBeforeName]
           ,[j02TitleAfterName]
           ,[j02Code]
           ,[j02JobTitle]
           ,[j02Email]
           ,[j02Mobile]
           ,[j02Phone]
           ,[j02Office]
           ,[j02IsIntraPerson]
           ,[j02Description]
           ,[j02AvatarImage]
           ,[j02EmailSignature]
           ,[j02RobotAddress]
           ,[j02DateInsert]
           ,[j02UserInsert]
           ,[j02DateUpdate]
           ,[j02UserUpdate]
           ,[j02ValidFrom]
           ,[j02ValidUntil]
           ,[j02ExternalPID]
           ,[j02TimesheetEntryDaysBackLimit]
           ,[j02TimesheetEntryDaysBackLimit_p34IDs]
           ,[j02Salutation]
           ,[j02WorksheetAccessFlag]
           ,[j02DomainAccount])
SELECT [j02ID]
           ,[j07ID]
           ,[c21ID]
           ,[j18ID]
           ,[j17ID]
           ,[o40ID]
           ,[p72ID_NonBillable]
           ,[j02FirstName]
           ,[j02LastName]
           ,[j02TitleBeforeName]
           ,[j02TitleAfterName]
           ,[j02Code]
           ,[j02JobTitle]
           ,[j02Email]
           ,[j02Mobile]
           ,[j02Phone]
           ,[j02Office]
           ,[j02IsIntraPerson]
           ,[j02Description]
           ,[j02AvatarImage]
           ,[j02EmailSignature]
           ,[j02RobotAddress]
           ,[j02DateInsert]
           ,[j02UserInsert]
           ,[j02DateUpdate]
           ,[j02UserUpdate]
           ,[j02ValidFrom]
           ,[j02ValidUntil]
           ,[j02ExternalPID]
           ,[j02TimesheetEntryDaysBackLimit]
           ,[j02TimesheetEntryDaysBackLimit_p34IDs]
           ,[j02Salutation]
           ,[j02WorksheetAccessFlag]
           ,[j02DomainAccount]
FROM j02Person WHERE j02ID=@j02id



GO

----------P---------------j02_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_delete') and type = 'P')
 drop procedure j02_delete
GO





CREATE   procedure [dbo].[j02_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p32id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu osoby z tabulky j02Person
declare @ref_pid int

if exists(select p31ID FROM p31Worksheet where j02ID=@pid)
 set @err_ret='Minimálně jeden worksheet záznam má vazbu na tuto osobu.'

if isnull(@err_ret,'')<>''
 return 

if exists(select p31ID FROM p31Worksheet where j02ID_ContactPerson=@pid)
 set @err_ret='Je kontaktní osobou minimálně v jednom worksheet záznamu.'

if isnull(@err_ret,'')<>''
 return 

set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálně v jednomu worksheet úkonu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálně v jednomu projektu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p41',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálně v jednomu záznamu kontaktu je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE j02ID_Owner=@pid
if @ref_pid is not null
 set @err_ret='Minimálně v jednomu záznamu faktury je vlastníkem záznamu tato osoba ('+dbo.GetObjectAlias('p91',@ref_pid)+')'



if exists(select p56ID FROM p56Task WHERE j02ID_Owner=@pid)
 set @err_ret='Osoba je vlastníkem (zakladatelem) minimálně jednoho úkolu.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	UPDATE j03User SET j02ID=NULL WHERE j02ID=@pid

	if exists(select b05ID FROM b05Workflow_History WHERE x29ID=102 AND b05RecordPID=@pid)
	 DELETE FROM b05Workflow_History WHERE x29ID=102 AND b05RecordPID=@pid

	DELETE FROM b05Workflow_History WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE j02ID_Owner=@pid)

	if exists(select b07ID FROM b07Comment WHERE x29ID=102 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=102 AND b07RecordPID=@pid

	if exists(select b07ID FROM b07Comment WHERE j02ID_Owner=@pid)
	 DELETE FROM b07Comment WHERE j02ID_Owner=@pid

	if exists(SELECT x69ID FROM x69EntityRole_Assign WHERE j02ID=@pid)
	 DELETE FROM x69EntityRole_Assign WHERE j02ID=@pid

	

	if exists(select j12ID FROM j12Team_Person where j02ID=@pid)
	 DELETE FROM j12Team_Person WHERE j02ID=@pid

	if exists(select j02ID FROM j02Person_FreeField WHERE j02ID=@pid)
	 DELETE FROM j02Person_FreeField WHERE j02ID=@pid

	if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=102))
	 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=102)

	if exists(select o52ID FROM o52TagBinding WHERE x29ID=102 AND o52RecordPID=@pid)
	 DELETE FROM o52TagBinding WHERE x29ID=102 AND o52RecordPID=@pid

	if exists(SELECT p30ID FROM p30Contact_Person WHERE j02ID=@pid)
	 DELETE FROM p30Contact_Person WHERE j02ID=@pid

	if exists(select p28ID FROM p28Contact WHERE j02ID_ContactPerson_DefaultInWorksheet=@pid OR j02ID_ContactPerson_DefaultInInvoice=@pid)
	begin
	 UPDATE p28Contact set j02ID_ContactPerson_DefaultInWorksheet=null WHERE j02ID_ContactPerson_DefaultInWorksheet=@pid

	 UPDATE p28Contact set j02ID_ContactPerson_DefaultInInvoice=null WHERE j02ID_ContactPerson_DefaultInInvoice=@pid
	end

	if exists(select p41ID FROM p41Project WHERE j02ID_ContactPerson_DefaultInWorksheet=@pid OR j02ID_ContactPerson_DefaultInInvoice=@pid)
	begin
	 UPDATE p41Project set j02ID_ContactPerson_DefaultInWorksheet=null WHERE j02ID_ContactPerson_DefaultInWorksheet=@pid

	 UPDATE p41Project set j02ID_ContactPerson_DefaultInInvoice=null WHERE j02ID_ContactPerson_DefaultInInvoice=@pid
	end

	if exists(select o20ID FROM o20Milestone_Receiver WHERE j02ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE j02ID=@pid

	if exists(SELECT j05ID FROM j05MasterSlave WHERE j02ID_Master=@pid OR j02ID_Slave=@pid)
	 DELETE FROM j05MasterSlave WHERE j02ID_Master=@pid OR j02ID_Slave=@pid


	if exists(select j70ID FROM j70QueryTemplate where j02ID_Owner=@pid)
	 begin
		DELETE FROM j71QueryTemplate_Item WHERE j70ID IN (select j70ID FROM j70QueryTemplate WHERE j02ID_Owner=@pid)

		DELETE FROM j70QueryTemplate WHERE j02ID_Owner=@pid
	 end
	 

	 if exists(select j77ID FROM j77WorksheetStatTemplate WHERE j02ID_Owner=@pid)
	 begin		
		DELETE FROM j77WorksheetStatTemplate WHERE j02ID_Owner=@pid
	 end

	 if exists(select p52ID FROM p52PriceList_Item WHERE j02ID=@pid)
	  DELETE FROM p52PriceList_Item WHERE j02ID=@pid
	

	DELETE FROM j02Person WHERE j02ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO

----------P---------------j02_changelog-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_changelog') and type = 'P')
 drop procedure j02_changelog
GO


CREATE procedure [dbo].[j02_changelog]
@j02id int
AS

SELECT a.RowID as pid
,row_number() over (order by a.RowID) as Verze
,a.j02DateInsert as Založeno
,a.j02UserInsert as Založil
,a.j02DateUpdate as Aktualizováno
,a.j02UserUpdate as Aktualizoval
,a.j02TitleBeforeName as [Titul]
,a.j02FirstName as [Jméno]
,a.j02LastName as [Příjmení]
,a.j02TitleAfterName as [Titul za jménem]
,a.j02Email as [E-mail]
,j07.j07Name as [Pozice]
,c21.c21Name as [Fond]
,j18.j18Name as [Středisko]
,a.j02Code as [Kód]
,a.j02Mobile as [Mobil]
,a.j02Office as [Kancelář]
,o40.o40Name as [SMTP účet]
,a.j02AvatarImage as [Avatar]
,a.j02IsIntraPerson as [Interní osoba]
,a.j02EmailSignature as [E-mail podpis]
,a.j02ExternalPID as [Externí klíč]
,a.j02ValidUntil as [Platnost záznamu]
FROM
j02Person_Log a
LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID
LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID
LEFT OUTER JOIN c21FondCalendar c21 ON a.c21ID=c21.c21ID
LEFT OUTER JOIN o40SmtpAccount o40 ON a.o40ID=o40.o40ID
WHERE a.j02ID=@j02id
ORDER BY a.RowID DESC

GO

----------P---------------j02_inhale_sumrow-------------------------

if exists (select 1 from sysobjects where  id = object_id('j02_inhale_sumrow') and type = 'P')
 drop procedure j02_inhale_sumrow
GO




CREATE procedure [dbo].[j02_inhale_sumrow]
@j03id_sys int
,@pid int						---j02id		
AS

declare @p56_actual_count int,@p56_closed_count int,@p91_count int,@o22_actual_count int
declare @p31_wip_time_count int,@p31_wip_expense_count int,@p31_wip_fee_count int,@p31_wip_kusovnik_count int,@b07_count int
declare @p31_approved_time_count int,@p31_approved_expense_count int,@p31_approved_fee_count int,@p31_approved_kusovnik_count int
declare @o23_count int,@last_access datetime,@last_worksheet varchar(100)

if exists(SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@pid OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@pid)))
 begin
	SELECT @p56_actual_count=sum(case when getdate() BETWEEN p56ValidFrom AND p56ValidUntil then 1 end)
	,@p56_closed_count=sum(case when getdate() NOT BETWEEN p56ValidFrom AND p56ValidUntil then 1 end)
	FROM p56Task
	WHERE p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@pid OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@pid)))

 end


SELECT @o22_actual_count=COUNT(o22ID)
FROM o22Milestone
WHERE o22DateUntil>=dateadd(day,-5,getdate()) AND getdate() BETWEEN o22ValidFrom AND o22ValidUntil AND o22ID IN (select o22ID FROM o20Milestone_Receiver WHERE j02ID=@pid OR j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@pid))

declare @is_p31id bit,@j03id int
select @j03id=j03ID FROM j03User WHERE j02ID=@pid
set @is_p31id=0

if exists(select p31ID FROM p31Worksheet WHERE j02ID=@pid)
 set @is_p31id=1

if @is_p31id=1
  SELECT @p91_count=COUNT(p91ID) from p91Invoice WHERE p91ID IN (SELECT p91ID FROM p31Worksheet where j02ID=@pid)


if @is_p31id=1
begin
select TOP 1 @last_worksheet=dbo.GetDDMMYYYYHHMM(a.p31DateInsert)+'/'+c.p34Name FROM p31Worksheet a INNER JOIN p32Activity d ON a.p32ID=d.p32ID INNER JOIN p34ActivityGroup c ON d.p34ID=c.p34ID WHERE a.j02ID_Owner=@pid ORDER BY a.p31ID DESC

if exists(select p31ID FROM p31Worksheet WHERE j02ID=@pid AND p71ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil)
 begin
  select @p31_wip_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_wip_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_wip_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_wip_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  WHERE a.j02ID=@pid AND a.p71ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil

 end


if exists(select p31ID FROM p31Worksheet WHERE j02ID=@pid AND p71ID=1 AND p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil)
 begin
  select @p31_approved_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_approved_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_approved_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_approved_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID  
  WHERE a.j02ID=@pid AND a.p71ID=1 AND a.p91ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil
  
 end
end

if exists(select b07ID from b07Comment WHERE x29ID=102 AND b07RecordPID=@pid)
 select @b07_count=count(b07ID) FROM b07Comment WHERE x29ID=102 AND b07RecordPID=@pid

if exists(select a.o23ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID INNER JOIN o23Doc c ON a.o23ID=c.o23ID WHERE a.x19RecordPID=@pid AND b.x29ID=102 and getdate() between c.o23ValidFrom and c.o23ValidUntil)
 select @o23_count=count(a.o23ID) FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID INNER JOIN o23Doc c ON a.o23ID=c.o23ID WHERE a.x19RecordPID=@pid AND b.x29ID=102 and getdate() between c.o23ValidFrom and c.o23ValidUntil
else
 set @o23_count=0

if @j03id is not null
 select TOP 1 @last_access=j90Date FROM j90LoginAccessLog WHERE j03ID=@j03id ORDER BY j90ID DESC

select isnull(@p56_actual_count,0) as p56_Actual_Count
,isnull(@o22_actual_count,0) as o22_Actual_Count
,isnull(@p56_closed_count,0) as p56_Closed_Count
,isnull(@p91_count,0) as p91_Count
,isnull(@p31_wip_time_count,0) as p31_Wip_Time_Count
,isnull(@p31_approved_time_count,0) as p31_Approved_Time_count
,isnull(@p31_wip_expense_count,0) as p31_Wip_Expense_Count
,isnull(@p31_approved_expense_count,0) as p31_Approved_Expense_Count
,isnull(@p31_wip_fee_count,0) as p31_Wip_Fee_Count
,isnull(@p31_approved_fee_count,0) as p31_Approved_Fee_Count
,isnull(@p31_wip_kusovnik_count,0) as p31_Wip_Kusovnik_Count
,isnull(@p31_approved_kusovnik_count,0) as p31_Approved_Kusovnik_Count
,isnull(@b07_count,0) as b07_Count
,isnull(@o23_count,0) as o23_Count
,@last_worksheet as Last_Worksheet
,@last_access as Last_Access


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
  INSERT INTO j11Team(j11IsAllPersons,j11Name,j11UserInsert,j11UserUpdate,j11DateInsert,j11DateUpdate) VALUES(1,'Všichni','recovery','recovery',getdate(),getdate())

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--j03id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu uživatele z tabulky j03User


BEGIN TRANSACTION

BEGIN TRY
	if exists(select j77ID FROM j77WorksheetStatTemplate WHERE j03ID=@pid)
	 begin		
		DELETE FROM j77WorksheetStatTemplate WHERE j03ID=@pid
	 end

	if exists(select j90ID FROM j90LoginAccessLog where j03ID=@pid)
      DELETE FROM j90LoginAccessLog where j03ID=@pid 
	  	

	if exists(select j70ID FROM j70QueryTemplate where j03ID=@pid)
	 BEGIN
		DELETE FROM j71QueryTemplate_Item WHERE j70ID IN (select j70ID FROM j70QueryTemplate where j03ID=@pid)

		DELETE FROM j70QueryTemplate WHERE j03ID=@pid
	 END

	DELETE FROM x36UserParam WHERE j03ID=@pid
	 

	if exists(select x47ID FROM x47EventLog where j03ID=@pid)
      DELETE FROM x47EventLog where j03ID=@pid 

	if exists(select j13ID FROM j13FavourteProject WHERE j03ID=@pid)
	 DELETE FROM j13FavourteProject WHERE j03ID=@pid


	delete from j03User where j03ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  














GO

----------P---------------j03_recovery_cache-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03_recovery_cache') and type = 'P')
 drop procedure j03_recovery_cache
GO




CREATE   procedure [dbo].[j03_recovery_cache]
@j03id int,@j02id int

AS

declare @p56_count int,@o22_count int,@o23_count int,@p39_count int,@d1 datetime,@d2 datetime,@is_approve bit,@x67id int,@login varchar(50),@j11ids varchar(200)
declare @homemenu varchar(50)

set @d1=dateadd(day,-1,getdate())
set @d2=dateadd(day,2,getdate())
set @is_approve=0
set @homemenu=null

select @x67id=b.x67ID,@login=a.j03Login,@j11ids=a.j03Cache_j11IDs
FROM j03user a INNER JOIN j04UserRole b on a.j04ID=b.j04ID
WHERE a.j03ID=@j03id


if exists(SELECT a.x68ID FROM x68EntityRole_Permission a INNER JOIN x53Permission b ON a.x53ID=b.x53ID WHERE a.x67ID=@x67id AND b.x53Value=23 AND b.x29ID=103)
 set @is_approve=1	--může paušálně schvalovat veškerý worksheet, oprávnění x53Value=GR_P31_Approver (23): Oprávnění schvalovat všechny worksheet úkony v databázi

if @is_approve=0
begin
 if exists(SELECT TOP 1 a.x67ID FROM x67EntityRole a INNER JOIN x69EntityRole_Assign x69 ON a.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON a.x67ID=o28.x67ID WHERE getdate() BETWEEN a.x67ValidFrom AND a.x67ValidUntil AND a.x29ID=141 AND o28.o28PermFlag IN (3,4) AND (x69.j02ID=@j02id OR x69.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id)))
  set @is_approve=1	---má oprávnění schvalovat worksheet v minimálně jednom projektu

end


---počet úkolů
select @p56_count=count(p56ID) FROM p56Task
WHERE ((p56PlanUntil BETWEEN @d1 AND @d2 and getdate() between p56ValidFrom and p56ValidUntil) OR p56ReminderDate between @d1 AND @d2)
AND (j02ID_Owner=@j02id OR p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))))
 

---počet událostí
select @o22_count=count(o22ID) FROM o22Milestone
WHERE (o22DateFrom BETWEEN @d1 AND @d2 OR o22DateUntil BETWEEN @d1 AND @d2 OR o22ReminderDate BETWEEN @d1 AND @d2)
AND (j02ID_Owner=@j02id OR j02ID=@j02id OR o22ID IN (SELECT o22ID FROM o20Milestone_Receiver WHERE j02ID=@j02id OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id)))

--počet dokumentů k připomenutí
select @o23_count=count(o23ID) FROM o23Doc
WHERE (o23ReminderDate BETWEEN @d1 AND @d2)
AND (j02ID_Owner=@j02id OR x23ID IN (select x23ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE a.x19RecordPID=@j02id AND b.x29ID=102))


----počet auto-generovaných odměn/paušálů/úkonů
select @p39_count=count(a.p39ID) FROM p39WorkSheet_Recurrence_Plan a INNER JOIN p40WorkSheet_Recurrence b ON a.p40ID=b.p40ID
WHERE (b.j02ID=@j02id or b.p40UserInsert=@login) AND a.p39DateCreate BETWEEN @d1 AND @d2


if @j11ids is null
 begin	---seznam týmů s účastí osoby
	select @j11ids=COALESCE(@j11ids + ',', '')+convert(varchar(10),j11ID)
	from
	j12Team_Person
	WHERE j02ID=@j02id

	UPDATE j03User set j03Cache_j11IDs=@j11ids WHERE j03ID=@j03id
 end

if exists(select j62ID FROM j62MenuHome WHERE j62ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=162 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))))
 begin
  select @homemenu=x35Value FROM x35GlobalParam WHERE x35Key like 'AppName'

  if isnull(@homemenu,'')='' or @homemenu='MARKTIME 5.0'
   set @homemenu='MT+'
 end
 
 

update j03User set j03Cache_TimeStamp=getdate()
,j03Cache_MessagesCount=isnull(@p56_count,0)+isnull(@o22_count,0)+isnull(@p39_count,0)+isnull(@o23_count,0)
,j03Cache_IsApprovingPerson=@is_approve,j03Cache_HomeMenu=@homemenu
WHERE j03ID=@j03id



















GO

----------P---------------j03user_load_sysuser-------------------------

if exists (select 1 from sysobjects where  id = object_id('j03user_load_sysuser') and type = 'P')
 drop procedure j03user_load_sysuser
GO




CREATE   procedure [dbo].[j03user_load_sysuser]
@login nvarchar(50)

AS

declare @j03id int,@j02id int,@j04id int,@personal_page varchar(200),@personal_page_mobile varchar(200)
declare @j03Cache_TimeStamp datetime,@j03Cache_MessagesCount int,@j03Cache_IsApprovingPerson bit,@j03Cache_j11IDs varchar(200),@j60id int

select @j03id=a.j03ID,@j02id=a.j02ID,@j04id=a.j04id,@personal_page=b.j04Aspx_PersonalPage,@personal_page_mobile=b.j04Aspx_PersonalPage_Mobile
,@j03Cache_TimeStamp=isnull(a.j03Cache_TimeStamp,convert(datetime,'01.01.2000',104)),@j03Cache_MessagesCount=isnull(a.j03Cache_MessagesCount,0),@j03Cache_IsApprovingPerson=a.j03Cache_IsApprovingPerson
,@j03Cache_j11IDs=a.j03Cache_j11IDs,@j60id=b.j60ID
FROM j03user a INNER JOIN j04UserRole b on a.j04ID=b.j04ID
WHERE a.j03Login=@login

if datediff(minute,@j03Cache_TimeStamp,getdate())>5
 begin
	exec dbo.j03_recovery_cache @j03id,@j02id

	select @j03Cache_MessagesCount=j03Cache_MessagesCount,@j03Cache_IsApprovingPerson=j03Cache_IsApprovingPerson,@j03Cache_j11IDs=j03Cache_j11IDs
	FROM j03User WHERE j03ID=@j03id

	---insert into j90LoginAccessLog(j03ID,j90Date) values(@j03id,getdate())
 end

if @j60id is null
 select @j60id=j60ID FROM j60MenuTemplate WHERE j60IsSystem=1

declare @is_master bit
set @is_master=0

if exists(select j05ID FROM j05MasterSlave WHERE j02ID_Master=@j02id)
 set @is_master=1

select a.*,a.j03id as _pid
,j04.j04name as _j04Name,j02.j02LastName as _j02LastName,j02.j02FirstName as _j02FirstName,j02.j02TitleBeforeName as _j02TitleBeforeName,j02.j02Email as _j02Email
,a.j03dateupdate as _dateupdate,a.j03dateinsert as _dateinsert,a.j03userupdate as _userupdate,a.j03userinsert as _userinsert
,a.j03validfrom as _validfrom,a.j03validuntil as _validuntil,a.j03IsLiveChatSupport,a.j03SiteMenuSkin,a.j03IsSiteMenuOnClick
,x67.x67RoleValue as _RoleValue
,j02.j02Email as _j02Email,@j03Cache_IsApprovingPerson as _IsApprovingPerson,@is_master as _IsMasterPerson,@j03Cache_MessagesCount as j03Cache_MessagesCount
,@j03Cache_MessagesCount as _MessagesCount,@j03Cache_j11IDs as _j11IDs
,case when a.j03Aspx_PersonalPage IS NULL THEN @personal_page ELSE a.j03Aspx_PersonalPage END as _PersonalPage
,j04.j04IsMenu_Worksheet,j04.j04IsMenu_Report,j04.j04IsMenu_Project,j04.j04IsMenu_People,j04.j04IsMenu_Contact,j04.j04IsMenu_Invoice,j04.j04IsMenu_Proforma,j04.j04IsMenu_Notepad,j04.j04IsMenu_MyProfile,j04.j04IsMenu_Task
,j04.j04Aspx_OneProjectPage as OneProjectPage,j04.j04Aspx_OneContactPage as OneContactPage,j04.j04Aspx_OneInvoicePage as OneInvoicePage,j04.j04Aspx_OnePersonPage as OnePersonPage
,j02.j07ID,@j60id as j60ID
,j02.j02WorksheetAccessFlag as _j02WorksheetAccessFlag
FROM j03user a INNER JOIN j04userrole j04 on a.j04id=j04.j04id LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id
INNER JOIN x67EntityRole x67 ON j04.x67ID=x67.x67ID
WHERE a.j03ID=@j03id























GO

----------P---------------j04_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j04_delete') and type = 'P')
 drop procedure j04_delete
GO







CREATE   procedure [dbo].[j04_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j04id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu role z tabulky j04UserRole
declare @ref_pid int,@x67id int

select @x67id=x67ID FROM j04UserRole WHERE j04ID=@pid

if @x67id is null
 begin
  set @err_ret='x67id missing'
  return
 end

SELECT TOP 1 @ref_pid=j03ID from j03User WHERE j04ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden uživatelský účet má vazbu na tuto aplikační roli ('+dbo.GetObjectAlias('j03',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM x68EntityRole_Permission WHERE x67ID=@x67id

	DELETE FROM x69EntityRole_Assign WHERE x67ID=@x67id

	UPDATE j04UserRole SET x67ID=(SELECT TOP 1 x67ID FROM x67EntityRole WHERE x67ID<>@x67id) WHERE j04ID=@pid

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--j05id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu MASTERSLAVE z tabulky j05MasterSlave



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
@j03id_sys int				--přihlášený uživatel
,@pid int					--j07id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu pozice z tabulky j07PersonPosition
declare @ref_pid int

SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE j07ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna osoba má vazbu na tuto pozici ('+dbo.GetObjectAlias('j02',@ref_pid)+')'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--j11id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu týmu osob z tabulky j11Team
declare @ref_pid int,@x29id int,@x69recordpid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=a.x69ID,@x29id=b.x29ID,@x69recordpid=a.x69recordpid
from x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID
WHERE a.j11ID=@pid

if @ref_pid is not null and @x29id=141
 set @err_ret='Tento tým je obsazen přes projektovou roli minimálně v jednom projektu ('+dbo.GetObjectAlias('p41',@x69recordpid)+')'

if @ref_pid is not null and @x29id=328
 set @err_ret='Tento tým je obsazen rolí minimálně v jednom záznamu kontaktu ('+dbo.GetObjectAlias('p28',@x69recordpid)+')'

if @ref_pid is not null and @x29id=391
 set @err_ret='Tento tým je obsazen rolí minimálně v jedné faktuře ('+dbo.GetObjectAlias('p91',@x69recordpid)+')'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--j17id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu státu z tabulky j17Country
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna vystavená faktura má vazbu na tento DPH region ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


SELECT TOP 1 @ref_pid=c26ID from c26Holiday WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden den svátku je svázaný s tímto regionem ('+dbo.GetObjectAlias('c26',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=j02ID from j02Person WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna osoba má vazbu na tento region ('+dbo.GetObjectAlias('j02',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE j17ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ faktury má vazbu na tento DPH region.'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--j18id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu regionu z tabulky j18Region
declare @ref_pid int

SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE j18ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden projekt je svázaný s tímto regionem ('+dbo.GetObjectAlias('p41',@ref_pid)+')'




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

----------P---------------j25_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j25_delete') and type = 'P')
 drop procedure j25_delete
GO





CREATE   procedure [dbo].[j25_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j25ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu kategorie z tabulky j25ReportCategory
declare @ref_pid int

SELECT TOP 1 @ref_pid=x31ID from x31Report WHERE x31ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna šablona sestavy nebo pluginu má vazbu na tuto kategorii ('+dbo.GetObjectAlias('x31',@ref_pid)+')'


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

----------P---------------j60_clone-------------------------

if exists (select 1 from sysobjects where  id = object_id('j60_clone') and type = 'P')
 drop procedure j60_clone
GO





CREATE    PROCEDURE [dbo].[j60_clone]
@j03id_sys int
,@j60id_orig int
,@j60id_dest int

AS

declare @j62id int,@login varchar(50),@j62id_new int,@parentid_orig int,@uid varchar(50),@parentid_new int

select @login=j03Login FROM j03User where j03ID=@j03id_sys

DECLARE curCR CURSOR FOR 
SELECT j62ID FROM j62MenuHome WHERE j60ID=@j60id_orig ORDER BY j62TreeIndex,j62Ordinary,j62Name

OPEN curCR
FETCH NEXT FROM curCR 
INTO @j62id
WHILE @@FETCH_STATUS = 0
BEGIN
  set @uid=convert(varchar(10),@j62id)

  select @parentid_orig=j62ParentID FROM j62MenuHome WHERE j62ID=@j62id
   
  insert into j62MenuHome(j62UserUpdate,j60ID,x29ID,j62Name,j62Name_ENG,j70ID,x31ID,j62URL,j62Target,j62Ordinary,j62GridGroupBy,j62ImageUrl,j62IsSeparator,j62TreeIndex,j62TreeLevel,j62TreePrev,j62TreeNext,j62DateInsert,j62UserInsert,j62DateUpdate,j62ValidFrom,j62ValidUntil,j62Tag)
  select @uid,@j60id_dest,x29ID,j62Name,j62Name_ENG,j70ID,x31ID,j62URL,j62Target,j62Ordinary,j62GridGroupBy,j62ImageUrl,j62IsSeparator,j62TreeIndex,j62TreeLevel,j62TreePrev,j62TreeNext,getdate(),@login,getdate(),j62ValidFrom,j62ValidUntil,j62Tag
  FROM
  j62MenuHome
  WHERE j62ID=@j62id

  SELECT @j62id_new=@@IDENTITY

  if @parentid_orig is not null
   begin
     select @parentid_new=j62ID FROM j62MenuHome WHERE j62UserUpdate=convert(varchar(10),@parentid_orig) AND j60ID=@j60id_dest

	 update j62MenuHome set j62ParentID=@parentid_new,j62UserUpdate=@login WHERE j62ID=@j62id_new

   end    

  FETCH NEXT FROM curCR 
  INTO @j62id
END
CLOSE curCR
DEALLOCATE curCR
	
	
	
	
	
	
	
	
 
 





GO

----------P---------------j60_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j60_delete') and type = 'P')
 drop procedure j60_delete
GO




CREATE   procedure [dbo].[j60_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j60ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění šablony hlavního aplikačního menu

if exists(select j04ID FROM j04UserRole WHERE j60ID=@pid)
 set @err_ret='K menu šabloně má vazbu minimálně jedna aplikační role.'

if exists(select j60ID FROM j60MenuTemplate WHERE j60ID=@pid AND j60IsSystem=1)
 set @err_ret='Výchozí (system) menu šablonu nelze odstranit ani upravovat.' 

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY

	delete from j62MenuHome WHERE j60ID=@pid
	
	delete from j60MenuTemplate where j60ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------j61_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j61_delete') and type = 'P')
 drop procedure j61_delete
GO




CREATE   procedure [dbo].[j61_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j61ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu textové šablony z tabulky j61TextTemplate
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE j61ID_Invoice=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden klient má vazbu na tuto textovou šablonu ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j61TextTemplate where j61ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------j62_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j62_delete') and type = 'P')
 drop procedure j62_delete
GO





CREATE   procedure [dbo].[j62_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j62ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu položky HOME menu z tabulky j62MenuHome

if exists(select j62ID FROM j62MenuHome WHERE j62ParentID=@pid)
 set @err_ret='Tato položka má pod sebou minimálně jednu podřízenou položku.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	delete from j62MenuHome where j62ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

declare @j02id int

select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys

exec dbo.j03_recovery_cache @j03id_sys,@j02id














GO

----------P---------------j70_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j70_delete') and type = 'P')
 drop procedure j70_delete
GO





CREATE   procedure [dbo].[j70_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j70id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky j70QueryTemplate


delete from j71QueryTemplate_Item WHERE j70ID=@pid

delete from j70QueryTemplate where j70ID=@pid
















GO

----------P---------------j77_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('j77_delete') and type = 'P')
 drop procedure j77_delete
GO





CREATE   procedure [dbo].[j77_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--j77id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky j77WorksheetStatTemplate



DELETE FROM x69EntityRole_Assign WHERE x69RecordPID=@pid AND x67ID IN (SELECT x67ID FROM x67EntityRole WHERE x29ID=177)

delete from j77WorksheetStatTemplate where j77ID=@pid

















GO

----------P---------------m62_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('m62_delete') and type = 'P')
 drop procedure m62_delete
GO





CREATE   procedure [dbo].[m62_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--m62id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu měnového kurzu z tabulky m62ExchangeRate


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

----------P---------------msoffice_find_binding-------------------------

if exists (select 1 from sysobjects where  id = object_id('msoffice_find_binding') and type = 'P')
 drop procedure msoffice_find_binding
GO





CREATE procedure [dbo].[msoffice_find_binding]
@j03id_sys int
,@entry_id varchar(200)
AS

declare @o23id int,@docname nvarchar(255),@p31id int,@ukon nvarchar(255),@p56id int,@task nvarchar(255),@o22id int,@appointment nvarchar(255)

if exists(select o23ID FROM o23Doc WHERE o23ExternalPID like @entry_id)
 select top 1 @o23id=a.o23ID,@docname=c.x18Name+': '+a.o23Name+' | '+isnull(o23UserInsert+convert(varchar(20),o23DateInsert),'') FROM o23Doc a INNER JOIN x23EntityField_Combo b ON a.x23ID=b.x23ID INNER JOIN x18EntityCategory c ON b.x23ID=c.x23ID WHERE a.o23ExternalPID like @entry_id

if exists(select p56ID FROM p56Task WHERE p56ExternalPID like @entry_id)
 select top 1 @p56id=a.p56ID,@task=b.p57Name+': '+a.p56Name+' | '+isnull(p56UserInsert+convert(varchar(20),p56DateInsert),'') FROM p56Task a INNER JOIN p57TaskType b ON a.p57ID=b.p57ID WHERE a.p56ExternalPID like @entry_id

if exists(select o22ID FROM o22Milestone WHERE o22ExternalPID like @entry_id)
 select top 1 @o22id=a.o22ID,@appointment=b.o21Name+': '+a.o22Name+' | '+isnull(o22UserInsert+convert(varchar(20),o22DateInsert),'') FROM o22Milestone a INNER JOIN o21MilestoneType b ON a.o21ID=b.o21ID WHERE a.o22ExternalPID like @entry_id


if @o23id is null and @p56id is null and @o22id is null
 begin
  if exists(select p31ID FROM p31Worksheet WHERE p31ExternalPID like @entry_id)
   select top 1 @p31id=a.p31ID,@ukon=convert(varchar(10),a.p31Date,104)+'/'+isnull(b.p41NameShort,b.p41Name) FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID WHERE a.p31ExternalPID like @entry_id
 end


select @entry_id as EntryID, @o23id as o23ID,@docname as DocName,@p31id as p31ID,@ukon as Ukon,@p56id as p56ID,@task as Task,@o22id as o22ID,@appointment as Appointment




GO

----------P---------------o21_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o21_delete') and type = 'P')
 drop procedure o21_delete
GO





CREATE   procedure [dbo].[o21_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o21id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu typu milníku z tabulky o21MilestoneType
declare @ref_pid int

SELECT TOP 1 @ref_pid=o22ID from o22Milestone WHERE o21ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden milník/termín/událost je svázaný s tímto typem ('+dbo.GetObjectAlias('o22',@ref_pid)+')'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--o22id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu milníku z tabulky o22Milestone


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select o22ID FROM o22Milestone_FreeField WHERE o22ID=@pid)
	 DELETE FROM o22Milestone_FreeField WHERE o22ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE o22ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE o22ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid)

    if exists(select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid

	if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=222))
	 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=222)

	delete from o22Milestone where o22ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------o23_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('o23_aftersave') and type = 'P')
 drop procedure o23_aftersave
GO





CREATE    PROCEDURE [dbo].[o23_aftersave]
@o23id int
,@j03id_sys int

AS

declare @o23code varchar(50),@x23id int,@x18id int,@x18EntryCodeFlag int,@x38id int,@count int,@o23ArabicCode varchar(50)

select @o23code=a.o23Code,@x23id=a.x23ID,@x18id=x18.x18ID,@x18EntryCodeFlag=x18.x18EntryCodeFlag,@x38id=x18.x38ID,@o23ArabicCode=a.o23ArabicCode
from o23Doc a INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID
INNER JOIN x18EntityCategory x18 ON x23.x23ID=x18.x23ID
where a.o23ID=@o23id

if @x18EntryCodeFlag=3 and @o23code is null
begin	---automaticky generovat kód v rámci všech položek štítku
 select @count=count(*) FROM o23Doc WHERE x23ID=@x23id AND o23ID<>@o23id AND o23Code is not null
 set @o23code=isnull(@count,0)+1

 update o23Doc set o23Code=@o23code WHERE o23ID=@o23id
end

 

if @x18EntryCodeFlag=4 and @o23code is null
begin	---automaticky generovat kód v rámci projektu daného štítku
 declare @p41id int,@x20id int
 select @p41id=a.x19RecordPID,@x20id=b.x20ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE a.o23ID=@o23id AND b.x29ID=141

 if @p41id is not null and @x20id is not null
 begin
  select @count=count(*) FROM o23Doc WHERE x23ID=@x23id AND o23ID<>@o23id AND o23Code IS NOT NULL AND o23ID IN (select o23ID FROM x19EntityCategory_Binding WHERE x20ID=@x20id AND x19RecordPID=@p41id)

  set @o23code=isnull(@count,0)+1

  update o23Doc set o23Code=@o23code WHERE o23ID=@o23id
 end
end



if (left(@o23code,4)='TEMP' OR @o23code is null) AND @x38id is not null
 begin
 

  exec dbo.x38_get_freecode_proc @x38id,223,@o23id,0,1,@o23code OUTPUT

  if @o23code<>''
   UPDATE o23Doc SET o23Code=@o23code WHERE o23ID=@o23id 
 end 

if ISNUMERIC(@o23code)=1 and @o23ArabicCode is null
 begin	---převod kódu na arabské číslo
  declare @num int
  set @num=convert(int,@o23code)

  update o23Doc set o23ArabicCode=case when @num<1000 then dbo.ConvertNumberToRoman(@num) else null end WHERE o23ID=@o23id
 end


 
 





GO

----------P---------------o23_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o23_delete') and type = 'P')
 drop procedure o23_delete
GO



CREATE   procedure [dbo].[o23_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o23id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu z tabulky o23Doc

if exists(select x19ID FROM x19EntityCategory_Binding WHERE o23ID=@pid)
begin
 declare @count int
 select @count=count(*) from x19EntityCategory_Binding WHERE o23ID=@pid
 if isnull(@count,0)>10
  set @err_ret='Nelze odstranit, protože dokument má vazbu na více záznamů. Dokument můžete přesunout do archivu.'

end

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	
	if exists(select b05ID FROM b05Workflow_History a INNER JOIN b07Comment b ON a.b07ID=b.b07ID WHERE b.x29ID=223 AND b.b07RecordPID=@pid)
	 DELETE FROM b05Workflow_History WHERE b07ID IN (select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)

	if exists(select o27ID FROM o27Attachment WHERE b07ID IS NOT NULL AND b07ID IN (select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IS NOT NULL AND b07ID IN (select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)

	if exists(select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid	

	if exists(select o52ID FROM o52TagBinding WHERE x29ID=223 AND o52RecordPID=@pid)
	 DELETE FROM o52TagBinding WHERE x29ID=223 AND o52RecordPID=@pid

    if exists(select o23ID FROM o23BigData WHERE o23ID=@pid)
	 DELETE FROM o23BigData WHERE o23ID=@pid

	delete from x19EntityCategory_Binding WHERE o23ID=@pid

	delete from o23Doc where o23ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  



















GO

----------P---------------o25_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o25_delete') and type = 'P')
 drop procedure o25_delete
GO











CREATE   procedure [dbo].[o25_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o25id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu z tabulky o25DmsBinding

BEGIN TRANSACTION

BEGIN TRY

	

	delete from o25DmsBinding where o25ID=@pid
	

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--o27id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu dokumentu z tabulky o27Attachment


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--o32id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu kontaktního média z tabulky o32Contact_Medium



delete from o32Contact_Medium where o32ID=@pid








GO

----------P---------------o38_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o38_delete') and type = 'P')
 drop procedure o38_delete
GO









CREATE   procedure [dbo].[o38_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--a38id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu adresy z tabulky o38Address
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE o38ID_Primary=@pid or o38ID_Delivery=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna klientská faktura má vazbu na tuto adresu ('+dbo.GetObjectAlias('p91',@ref_pid)+')'

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

----------P---------------o40_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o40_delete') and type = 'P')
 drop procedure o40_delete
GO






CREATE   procedure [dbo].[o40_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o40ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu role z tabulky o40SmtpAccount


if exists(select b01ID FROM b01WorkflowTemplate WHERE o40ID=@pid)
 set @err_ret='Minimálně jedna workflow šablona má vazbu s tímto SMTP účtem.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select j02ID FROM j02Person WHERE o40ID=@pid)
	 UPDATE j02Person SET o40ID=null WHERE o40ID=@pid

	UPDATE x40MailQueue SET o40ID=NULL WHERE o40ID=@pid

	DELETE FROM o40SmtpAccount WHERE o40ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------o41_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o41_delete') and type = 'P')
 drop procedure o41_delete
GO





CREATE   procedure [dbo].[o41_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o41ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu role z tabulky o41InboxAccount


if exists(select o42ID FROM o42ImapRule WHERE o41ID=@pid)
 set @err_ret='Minimálně jeden IMAP účet má definované IMAP pravidlo.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM o41InboxAccount WHERE o41ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------o42_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o42_delete') and type = 'P')
 drop procedure o42_delete
GO





CREATE   procedure [dbo].[o42_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o42ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu IMAP pravidla z tabulky o42ImapRule




BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM o42ImapRule WHERE o42ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------o51_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('o51_delete') and type = 'P')
 drop procedure o51_delete
GO




CREATE   procedure [dbo].[o51_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--o51ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu štítku z tabulky o51Tag

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM o52TagBinding WHERE o51ID=@pid

	
	delete from o51Tag where o51ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p11_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p11_delete') and type = 'P')
 drop procedure p11_delete
GO








CREATE   procedure [dbo].[p11_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p11ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p11Attendance



BEGIN TRANSACTION

BEGIN TRY
	
	delete from p11Attendance where p11ID=@pid

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

declare @p28code varchar(10),@p29id int,@x38id int,@x38id_draft int,@p28name nvarchar(255),@iscompany bit,@p51id_billing int
declare @p28companyname nvarchar(255),@p28companyshortname nvarchar(50),@isdraft bit

select @p28code=p28code,@p29id=a.p29id,@iscompany=p28IsCompany,@p51id_billing=a.p51ID_Billing,@isdraft=a.p28IsDraft
,@p28companyname=p28CompanyName,@p28companyshortname=p28CompanyShortName,@x38id=p29.x38ID,@x38id_draft=p29.x38ID_Draft
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
  if @x38id is null
   select @x38id=x38ID FROM x38CodeLogic WHERE x29ID=328 AND x38IsDraft=0


  if @isdraft=1
   set @x38id=@x38id_draft

  exec dbo.x38_get_freecode_proc @x38id,328,@p28id,@isdraft,1,@p28code OUTPUT

  if @p28code<>''
   UPDATE p28Contact SET p28Code=@p28code WHERE p28ID=@p28id 
 end 

if @p51id_billing is not null	---aktualizace názvu případného ceníku sazeb, který je nastaven na míru pro daný projekt
 begin
   
   if exists(select p51ID FROM p51PriceList WHERE p51IsCustomTailor=1 and p51ID=@p51id_billing)
    update p51PriceList set p51Name=@p28name WHERE p51ID=@p51id_billing

 end

if exists(select p28ID FROM p28Contact WHERE p28ID=@p28id AND (p28ParentID IS NOT NULL OR p28TreePrev<p28TreeNext))
 exec [p28_recalc_tree]	---aktualizovat stromovou strukturu klientů
else
 update p28Contact set p28TreePath=isnull(p28CompanyShortName,p28Name) WHERE p28ID=@p28id


exec dbo.p28_append_log @p28id

 
 




GO

----------P---------------p28_append_log-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_append_log') and type = 'P')
 drop procedure p28_append_log
GO


CREATE    PROCEDURE [dbo].[p28_append_log]
@p28id int
AS

INSERT INTO [dbo].[p28Contact_Log]
           ([p28ID]
           ,[p29ID]
           ,[p92ID]
           ,[p87ID]
           ,[p51ID_Billing]
           ,[p51ID_Internal]
           ,[j02ID_Owner]
           ,[b02ID]
           ,[p63ID]
           ,[j61ID_Invoice]
           ,[p28IsDraft]
           ,[p28IsCompany]
           ,[p28Name]
           ,[p28Code]
           ,[p28FirstName]
           ,[p28LastName]
           ,[p28TitleBeforeName]
           ,[p28TitleAfterName]
           ,[p28RegID]
           ,[p28VatID]
           ,[p28CompanyName]
           ,[p28CompanyShortName]
           ,[p28InvoiceDefaultText1]
           ,[p28InvoiceDefaultText2]
           ,[p28InvoiceMaturityDays]
           ,[p28LimitHours_Notification]
           ,[p28LimitFee_Notification]
           ,[p28AvatarImage]
           ,[p28SupplierFlag]
           ,[p28SupplierID]
           ,[p28RobotAddress]
           ,[p28DateInsert]
           ,[p28UserInsert]
           ,[p28DateUpdate]
           ,[p28UserUpdate]
           ,[p28ValidFrom]
           ,[p28ValidUntil]
           ,[p28Person_BirthRegID]
           ,[p28ExternalPID]
           ,[p28ParentID]
           ,[p28BillingMemo]
           ,[p28TreeLevel]
           ,[p28TreeIndex]
           ,[p28TreePrev]
           ,[p28TreeNext]
           ,[p28TreePath]
           ,[p28Pohoda_VatCode]
           ,[j02ID_ContactPerson_DefaultInWorksheet]
           ,[j02ID_ContactPerson_DefaultInInvoice])
SELECT [p28ID]
           ,[p29ID]
           ,[p92ID]
           ,[p87ID]
           ,[p51ID_Billing]
           ,[p51ID_Internal]
           ,[j02ID_Owner]
           ,[b02ID]
           ,[p63ID]
           ,[j61ID_Invoice]
           ,[p28IsDraft]
           ,[p28IsCompany]
           ,[p28Name]
           ,[p28Code]
           ,[p28FirstName]
           ,[p28LastName]
           ,[p28TitleBeforeName]
           ,[p28TitleAfterName]
           ,[p28RegID]
           ,[p28VatID]
           ,[p28CompanyName]
           ,[p28CompanyShortName]
           ,[p28InvoiceDefaultText1]
           ,[p28InvoiceDefaultText2]
           ,[p28InvoiceMaturityDays]
           ,[p28LimitHours_Notification]
           ,[p28LimitFee_Notification]
           ,[p28AvatarImage]
           ,[p28SupplierFlag]
           ,[p28SupplierID]
           ,[p28RobotAddress]
           ,[p28DateInsert]
           ,[p28UserInsert]
           ,[p28DateUpdate]
           ,[p28UserUpdate]
           ,[p28ValidFrom]
           ,[p28ValidUntil]
           ,[p28Person_BirthRegID]
           ,[p28ExternalPID]
           ,[p28ParentID]
           ,[p28BillingMemo]
           ,[p28TreeLevel]
           ,[p28TreeIndex]
           ,[p28TreePrev]
           ,[p28TreeNext]
           ,[p28TreePath]
           ,[p28Pohoda_VatCode]
           ,[j02ID_ContactPerson_DefaultInWorksheet]
           ,[j02ID_ContactPerson_DefaultInInvoice]
FROM p28Contact WHERE p28ID=@p28id




GO

----------P---------------p28_append_remove_isir-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_append_remove_isir') and type = 'P')
 drop procedure p28_append_remove_isir
GO




CREATE procedure [dbo].[p28_append_remove_isir]
@j03id_sys int				--přihlášený uživatel
,@pid int						---p28id		
,@append_remove_flag int		----1 - přidat, 2 - odstranit
AS

if @append_remove_flag=1
 begin
  if exists(select p28ID FROM o48IsirMonitoring WHERE p28ID=@pid)
   return
  
  declare @login varchar(50)

  select @login=j03Login FROM j03User WHERE j03ID=@j03id_sys

  insert into o48IsirMonitoring(p28ID,o48UserInsert,o48DateInsert,o48UserUpdate,o48DateUpdate) VALUES(@pid,@login,getdate(),@login,getdate())

  return
 end

if @append_remove_flag=2
 DELETE FROM o48IsirMonitoring WHERE p28ID=@pid

GO

----------P---------------p28_convertdraft-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_convertdraft') and type = 'P')
 drop procedure p28_convertdraft
GO




CREATE    PROCEDURE [dbo].[p28_convertdraft]
@p28id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---konverze klienta z DRAFT režimu do normálního klienta
set @err_ret=''

declare @code varchar(50),@x38id int,@isdraft bit

select @x38id=p29.x38ID,@isdraft=a.p28IsDraft
FROM
p28Contact a LEFT OUTER JOIN p29ContactType p29 ON a.p29ID=p29.p29ID
WHERE a.p28ID=@p28id

if @x38id is null
 select @x38id=x38id from x38CodeLogic WHERE x29ID=328 AND x38IsDraft=0 AND getdate() between x38ValidFrom and x38ValidUntil

if @isdraft=0
 begin
  set @err_ret='Záznam není v ŕežimu DRAFT.'
  return
 end
 
exec dbo.x38_get_freecode_proc @x38id,328,@p28id,0,1,@code OUTPUT

if @code=''
 begin
  set @err_ret='Systém nedokázal složit odpovídající kód podle nastavení číselné řady. Záznam zůstává v režimu DRAFT.'
  return
 end

UPDATE p28Contact SET p28Code=@code,p28IsDraft=0 WHERE p28ID=@p28id 

  






GO

----------P---------------p28_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_delete') and type = 'P')
 drop procedure p28_delete
GO





CREATE   procedure [dbo].[p28_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p28id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu kontaktu z tabulky p28Contact
declare @ref_pid int

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE p28ID_Client=@pid OR p28ID_Billing=@pid
if @ref_pid is not null
 set @err_ret='Tento klient je svázán s minimálně jedním projektem ('+dbo.GetObjectAlias('p41',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p28ID=@pid
if @ref_pid is not null
 set @err_ret='Tento klient je svázán s minimálně jednou fakturou ('+dbo.GetObjectAlias('p91',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p90ID from p90Proforma WHERE p28ID=@pid
if @ref_pid is not null
 set @err_ret='Tento klient je svázán s minimálně jednou zálohou fakturou ('+dbo.GetObjectAlias('p90',@ref_pid)+')'



if @err_ret is null and exists(select p49ID FROM p49FinancialPlan WHERE p28ID_Supplier=@pid)
 set @err_ret='Subjekt vystupuje jako dodavatel v minimálně jednom rozpočtu (finančním plánu).'

if @err_ret is null and exists(select p31ID FROM p31Worksheet WHERE p28ID_Supplier=@pid)
 set @err_ret='Subjekt vystupuje jako dodavatel v minimálně jednom peněžním worksheet úkonu.'

if exists(select p28ID FROM p28Contact WHERE p28ParentID=@pid)
 set @err_ret='Klient má pod sebou minimálně jednoho podřízeného klienta.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select b05ID FROM b05Workflow_History WHERE x29ID=228 AND b05RecordPID=@pid)
	 DELETE FROM b05Workflow_History WHERE x29ID=228 AND b05RecordPID=@pid

	if exists(select b07ID FROM b07Comment WHERE x29ID=228 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=228 AND b07RecordPID=@pid	


	if exists(SELECT o32ID FROM o32Contact_Medium WHERE p28ID=@pid)
	 DELETE FROM o32Contact_Medium WHERE p28ID=@pid

	if exists(select o48ID FROM o48IsirMonitoring WHERE p28ID=@pid)
	 DELETE FROM o48IsirMonitoring WHERE p28ID=@pid

	if exists(select o37ID FROM o37Contact_Address WHERE p28ID=@pid)
	 begin
	  DELETE FROM o37Contact_Address WHERE p28ID=@pid

	  DELETE FROM o38Address WHERE o38ID IN (select o38ID FROM o37Contact_Address WHERE p28ID=@pid)
	 end

	if exists(select p28ID FROM p28Contact_FreeField WHERE p28ID=@pid)
	 DELETE FROM p28Contact_FreeField WHERE p28ID=@pid

	if exists(select o52ID FROM o52TagBinding WHERE x29ID=328 AND o52RecordPID=@pid)
	 DELETE FROM o52TagBinding WHERE x29ID=328 AND o52RecordPID=@pid

	if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=328))
	 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=328)

	if exists(select p30ID FROM p30Contact_Person where p28ID=@pid)
	 DELETE FROM p30Contact_Person where p28ID=@pid

	if exists(select a.f01ID FROM f01Folder a INNER JOIN f02FolderType b ON a.f02ID=b.f02ID WHERE a.f01RecordPID=@pid AND b.x29ID=328)
	 DELETE FROM f01Folder WHERE f01RecordPID=@pid AND f02ID IN (select f02ID FROM f02FolderType WHERE x29ID=328)

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=328)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=328


	

	delete from p28Contact WHERE p28ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO

----------P---------------p28_changelog-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_changelog') and type = 'P')
 drop procedure p28_changelog
GO


CREATE procedure [dbo].[p28_changelog]
@p28id int
AS

SELECT a.RowID as pid
,row_number() over (order by a.RowID) as Verze
,a.p28DateInsert as Založeno
,a.p28UserInsert as Založil
,a.p28DateUpdate as Aktualizováno
,a.p28UserUpdate as Aktualizoval
,a.p28Name as [Název]
,a.p28CompanyShortName as [Zkrácený název]
,p29.p29Name as [Typ klienta]
,a.p28Code as [Kód]
,a.p28RegID as [IČ]
,a.p28VatID as [DIČ]
,p51Billing.p51Name as [Fakturační ceník]
,p87.p87Name as [Fakturační jazyk]
,b02.b02Name as [Stav]
,a.p28LimitHours_Notification as [Limit rozpracovaných hodin]
,a.p28LimitFee_Notification as [Limit rozpracovaného honoráře]
,a.p28BillingMemo as [Fakturační poznámka]
,a.p28ExternalPID as [Externí klíč]
,parent.p28Name as [Nadřízený klient]
,p92.p92Name as [Typ faktury]
,a.p28InvoiceMaturityDays as [Výchozí splatnost]
,a.p28InvoiceDefaultText1 as [Text faktury]
,a.p28IsDraft as [DRAFT záznam]
,a.p28ValidUntil as [Platnost záznamu]
FROM
p28Contact_Log a
LEFT OUTER JOIN p51PriceList p51Billing ON a.p51ID_Billing=p51Billing.p51ID
LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID
LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID
LEFT OUTER JOIN p29ContactType p29 ON a.p29ID=p29.p29ID
LEFT OUTER JOIN p28Contact parent ON a.p28ParentID=parent.p28ID
LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
WHERE a.p28ID=@p28id
ORDER BY a.RowID DESC

GO

----------P---------------p28_inhale_sumrow-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_inhale_sumrow') and type = 'P')
 drop procedure p28_inhale_sumrow
GO




CREATE procedure [dbo].[p28_inhale_sumrow]
@j03id_sys int
,@pid int						---p28id		
AS

declare @p56_actual_count int,@p56_closed_count int,@o22_actual_count int,@p91_count int,@p30_exist bit,@childs_count int
declare @p31_wip_time_count int,@p31_wip_expense_count int,@p31_wip_fee_count int,@p31_wip_kusovnik_count int,@b07_count int
declare @p31_approved_time_count int,@p31_approved_expense_count int,@p31_approved_fee_count int,@p31_approved_kusovnik_count int
declare @o23_count int,@p41_actual_count int,@p41_closed_count int, @o48_exist bit,@p90_count int
declare @last_invoice varchar(100),@last_wip_worksheet as varchar(100),@f01_exist bit,@o52_exist bit
declare @last_p91id int

SELECT @p56_actual_count=sum(case when getdate() BETWEEN p56ValidFrom AND p56ValidUntil then 1 end)
,@p56_closed_count=sum(case when getdate() NOT BETWEEN p56ValidFrom AND p56ValidUntil then 1 end)
FROM p56Task a INNER JOIN p41Project b ON a.p41ID=b.p41ID
WHERE b.p28ID_Client=@pid

if exists(select o22ID FROM o22Milestone WHERE o22DateUntil>=dateadd(day,-5,getdate()) AND p28ID=@pid OR p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@pid))
begin
SELECT @o22_actual_count=COUNT(o22ID)
FROM o22Milestone
WHERE p28ID=@pid OR p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@pid) AND o22DateUntil>=dateadd(day,-5,getdate()) AND getdate() BETWEEN o22ValidFrom AND o22ValidUntil
end

SELECT @p91_count=COUNT(p91ID)
from p91Invoice
WHERE p28ID=@pid

if exists(select p30ID FROM p30Contact_Person WHERE (p28ID=@pid OR p41ID IN (select p41ID FROM p41Project WHERE p28ID_Client=@pid)) AND getdate() BETWEEN p30ValidFrom AND p30ValidUntil)
 set @p30_exist=1
else
 set @p30_exist=0


if exists(select p28ID FROM p28Contact WHERE p28ParentID=@pid)
 select @childs_count=count(p28ID) FROM p28Contact WHERE p28ParentID=@pid


if exists(select a.p31ID FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID WHERE b.p28ID_Client=@pid AND a.p71ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil)
 begin
  select @p31_wip_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_wip_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_wip_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_wip_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  INNER JOIN p41Project d ON a.p41ID=d.p41ID
  WHERE d.p28ID_Client=@pid AND a.p71ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil

 end

if exists(select a.p31ID FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID WHERE b.p28ID_Client=@pid AND p71ID=1 AND a.p91ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil)
 begin
  select @p31_approved_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_approved_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_approved_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_approved_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  INNER JOIN p41Project d ON a.p41ID=d.p41ID
  WHERE d.p28ID_Client=@pid AND a.p71ID=1 AND a.p91ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil
  
 end

if exists(select b07ID from b07Comment WHERE x29ID=328 AND b07RecordPID=@pid)
 select @b07_count=count(b07ID) FROM b07Comment WHERE x29ID=328 AND b07RecordPID=@pid

if exists(select a.o23ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID INNER JOIN o23Doc c ON a.o23ID=c.o23ID WHERE a.x19RecordPID=@pid AND b.x29ID=328 and getdate() between c.o23ValidFrom and c.o23ValidUntil)
 select @o23_count=count(a.o23ID) FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID INNER JOIN o23Doc c ON a.o23ID=c.o23ID WHERE a.x19RecordPID=@pid AND b.x29ID=328 and getdate() between c.o23ValidFrom and c.o23ValidUntil
else
 set @o23_count=0

if exists(select p90ID FROM p90Proforma where p28ID=@pid)
 select @p90_count=count(p90ID) FROM p90Proforma WHERE p28ID=@pid
else
 set @p90_count=0

SELECT @p41_actual_count=sum(case when getdate() BETWEEN p41ValidFrom AND p41ValidUntil then 1 end)
,@p41_closed_count=sum(case when getdate() NOT BETWEEN p41ValidFrom AND p41ValidUntil then 1 end)
FROM p41Project
WHERE p28ID_Client=@pid
 
set @o48_exist=0
if exists(select o48ID FROM o48IsirMonitoring WHERE p28ID=@pid)
 set @o48_exist=1

if exists(select o52ID FROM o52TagBinding WHERE x29ID=328 AND o52RecordPID=@pid)
 set @o52_exist=1
else
 set @o52_exist=0

if exists(select a.f01ID FROM f01Folder a INNER JOIN f02FolderType b ON a.f02ID=b.f02ID WHERE b.x29ID=328 AND a.f01RecordPID=@pid)
 set @f01_exist=1
else
 set @f01_exist=0

if @p91_count>0
 select TOP 1 @last_invoice=p91Code+'/'+convert(varchar(10),p91DateSupply,104),@last_p91id=p91ID FROM p91Invoice WHERE p28ID=@pid ORDER BY p91ID DESC

if @p31_wip_time_count>0 or @p31_approved_time_count>0 or @p31_approved_expense_count>0
 select TOP 1 @last_wip_worksheet=dbo.GetDDMMYYYYHHMM(a.p31DateInsert)+'/'+c.j02FirstName+' '+c.j02LastName+'/'+d.p32Name FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID INNER JOIN j02Person c ON a.j02ID=c.j02ID INNER JOIN p32Activity d ON a.p32ID=d.p32ID WHERE b.p28ID_Client=@pid ORDER BY a.p31ID DESC


select isnull(@p56_actual_count,0) as p56_Actual_Count
,isnull(@p56_closed_count,0) as p56_Closed_Count
,isnull(@o22_actual_count,0) as o22_Actual_Count
,isnull(@p91_count,0) as p91_Count
,@p30_exist as p30_Exist
,isnull(@childs_count,0) as childs_Count
,isnull(@p31_wip_time_count,0) as p31_Wip_Time_Count
,isnull(@p31_approved_time_count,0) as p31_Approved_Time_count
,isnull(@p31_wip_expense_count,0) as p31_Wip_Expense_Count
,isnull(@p31_approved_expense_count,0) as p31_Approved_Expense_Count
,isnull(@p31_wip_fee_count,0) as p31_Wip_Fee_Count
,isnull(@p31_approved_fee_count,0) as p31_Approved_Fee_Count
,isnull(@p31_wip_kusovnik_count,0) as p31_Wip_Kusovnik_Count
,isnull(@p31_approved_kusovnik_count,0) as p31_Approved_Kusovnik_Count
,isnull(@b07_count,0) as b07_Count
,isnull(@o23_count,0) as o23_Count
,isnull(@p41_actual_count,0) as p41_actual_count
,isnull(@p41_closed_count,0) as p41_closed_count
,@o48_exist as o48_Exist
,@last_invoice as Last_Invoice
,@last_p91id as Last_p91ID
,@last_wip_worksheet as Last_Wip_Worksheet
,@p90_count as p90_Count
,@o52_exist as o52_Exist
,@f01_exist as f01_Exist

GO

----------P---------------p28_recalc_tree-------------------------

if exists (select 1 from sysobjects where  id = object_id('p28_recalc_tree') and type = 'P')
 drop procedure p28_recalc_tree
GO



CREATE    PROCEDURE [dbo].[p28_recalc_tree]

AS

update a set p28TreeIndex=b.TreeIndex,p28TreeLevel=b.TreeLevel,p28TreePath=b.TreePathAlias
FROM p28Contact a INNER JOIN dbo.view_p28_tree_recalc b ON a.p28ID=b.p28ID
WHERE isnull(a.p28TreeIndex,0)<>b.TreeIndex or isnull(a.p28TreeLevel,0)<>b.TreeLevel or a.p28TreePath<>b.TreePathAlias

update p28Contact set p28TreePrev=NULL,p28TreeNext=NULL

update a set p28TreePrev=a.p28TreeIndex,p28TreeNext=a.p28TreeIndex
FROM
p28Contact a INNER JOIN p28Contact b ON a.p28TreeIndex=b.p28TreeIndex-1
WHERE a.p28TreeLevel=b.p28TreeLevel OR a.p28TreeLevel>b.p28TreeLevel

update p28Contact set p28TreePrev=p28TreeIndex,p28TreeNext=p28TreeIndex
where p28TreeIndex In (select max(p28TreeIndex) from p28Contact)

declare @pid int,@level int,@index int,@index_max int

DECLARE curTR CURSOR FOR 
SELECT p28ID,p28TreeLevel,p28TreeIndex from p28Contact WHERE p28TreePrev IS NULL AND p28TreeNext IS NULL ORDER BY p28TreeIndex

OPEN curTR
FETCH NEXT FROM curTR 
INTO @pid,@level,@index
WHILE @@FETCH_STATUS = 0
BEGIN
  set @index_max=null
  
  select TOP 1 @index_max=p28TreeIndex FROM p28Contact WHERE p28TreeIndex>@index AND p28TreeLevel<=@level ORDER BY p28TreeIndex
  
  if @index_max is null
   select @index_max=max(p28TreeIndex) from p28Contact
  else
   set @index_max=@index_max-1
   
 
  update p28Contact set p28TreePrev=@index,p28TreeNext=@index_max WHERE p28ID=@pid

  FETCH NEXT FROM curTR 
  INTO @pid,@level,@index
END
CLOSE curTR
DEALLOCATE curTR


GO

----------P---------------p29_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p29_delete') and type = 'P')
 drop procedure p29_delete
GO







CREATE   procedure [dbo].[p29_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p29id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu instituce z tabulky p29contacttype
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE p29ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden kontakt má vazbu na tento typ ('+dbo.GetObjectAlias('p28',@ref_pid)+')'


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

----------P---------------p30_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p30_delete') and type = 'P')
 drop procedure p30_delete
GO




CREATE   procedure [dbo].[p30_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p30id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p30Contact_Person


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p30Contact_Person WHERE p30ID=@pid

	

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
,@p48id int					---ID operativního plánu
,@x45ids varchar(50) OUTPUT	---případné události, které se mají notifikovat (čárkou oddělené x45id)

AS

---automaticky se spouští po uložení worksheet záznamu
set @x45ids=''

declare @p31date datetime,@p32id int,@p41id int,@p34id int,@p71id int,@p70id int,@c11id int,@p33id int,@j02id_rec int
declare @j27id_billing_orig int,@j27id_internal int,@p31rate_billing_orig float,@p31rate_internal_orig float
declare @p31value_orig float,@p31amount_withoutvat_orig float,@p31vatrate_orig float,@p31amount_withvat_orig float
declare @p31amount_internal float,@p31amount_vat_orig float,@p91id int,@p32ManualFeeFlag int


select @c11id=a.c11ID,@p32id=a.p32ID,@p34id=p32.p34ID,@p71id=a.p71ID,@p70id=a.p70ID
,@p33id=p34.p33ID,@j02id_rec=a.j02ID,@p31date=a.p31date,@p41id=a.p41ID
,@p31value_orig=a.p31value_orig,@p91id=a.p91ID,@p32ManualFeeFlag=isnull(p32.p32ManualFeeFlag,0)
FROM
p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p31ID=@p31id

if isnull(@p71id,0)<>0 or ISNULL(@p70id,0)<>0 or ISNULL(@p91id,0)<>0
 return	---pokud úkon prošel schvalováním nebo fakturací, není možné měnit jeho atributy!!!!!

if @p48id is not null 
 UPDATE p48OperativePlan SET p31ID=@p31id WHERE p48ID=@p48id

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

if @p33id=1 or @p33id=3	---1 - čas, 3 - kusovník
 BEGIN
    if @p32ManualFeeFlag=0
     exec p31_getrate_tu @p31date,1, @p41id, @j02id_rec, @p32id, @j27id_billing_orig OUTPUT , @p31rate_billing_orig OUTPUT

	exec p31_getrate_tu @p31date,2, @p41id, @j02id_rec, @p32id, @j27id_internal OUTPUT , @p31rate_internal_orig OUTPUT  
	
	select @p31vatrate_orig=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
	
	if @p32ManualFeeFlag=1
	 begin
	  select @p31amount_withoutvat_orig=p31amount_withoutvat_orig FROM p31Worksheet where p31ID=@p31id	---částka pevného honoráře byla zadána ručně v úkonu

	  select @j27id_billing_orig=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic'
	  
	  set @p31rate_billing_orig=null
	 end
	
	if @p32ManualFeeFlag=0
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
 

 ----otestování limitů k notifikaci
 declare @limit_hours float,@limit_fee float,@waiting_hours float,@waiting_fee float,@p28id int

 select @p28id=p28ID_Client,@limit_hours=convert(float,p41LimitHours_Notification),@limit_fee=convert(float,p41LimitFee_Notification)
 FROM p41Project
 WHERE p41ID=@p41id

 if @limit_hours>0 OR @limit_fee>0	---notifikační limity nastavené u projektu
  begin
    select @waiting_hours=sum(p31Hours_Orig),@waiting_fee=sum(p31Hours_Orig*p31Rate_Billing_Orig)
	FROM p31Worksheet
	where p41ID=@p41id AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil AND p71ID IS NULL AND p91ID IS NULL

	if @waiting_hours>@limit_hours and @limit_hours>0
	 set @x45ids=@x45ids+',14110'

	if @waiting_fee>@limit_fee and @limit_fee>0
	 set @x45ids=@x45ids+',14111'
  end

set @limit_hours=0
set @limit_fee=0

if @p28id is not null
 select @limit_hours=convert(float,p28LimitHours_Notification),@limit_fee=convert(float,p28LimitFee_Notification) FROM p28Contact WHERE p28ID=@p28id

if @limit_hours>0 OR @limit_fee>0	---notifikační limit nastavené u klienta projektu
  begin
    select @waiting_hours=sum(p31Hours_Orig),@waiting_fee=sum(p31Hours_Orig*p31Rate_Billing_Orig)
	FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID
	where b.p28ID_Client=@p28id AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil AND a.p71ID IS NULL AND a.p91ID IS NULL

	if @waiting_hours>@limit_hours and @limit_hours>0
	 set @x45ids=@x45ids+',32810'

	if @waiting_fee>@limit_fee and @limit_fee>0
	 set @x45ids=@x45ids+',32811'
  end

if @x45ids<>''
 set @x45ids=right(@x45ids,len(@x45ids)-1)




if exists(select m62ID FROM m62ExchangeRate WHERE m62RateType=2)
begin
 declare @m62id_fixed_rate int
 select TOP 1 @m62id_fixed_rate=m62ID FROM m62ExchangeRate WHERE m62RateType=2 AND m62Date<=@p31date ORDER BY m62Date DESC

 if @m62id_fixed_rate is not null
  begin
   declare @p31exchangerate_fixed float,@j27id_domestic int

   select @j27id_domestic=j27ID_Master FROM m62ExchangeRate WHERE m62ID=@m62id_fixed_rate
   select @j27id_billing_orig=j27id_billing_orig FROM p31Worksheet WHERE p31ID=@p31id

   if @j27id_domestic<>@j27id_billing_orig
    select @p31exchangerate_fixed=dbo.get_exchange_rate(2,@p31date,@j27id_billing_orig,@j27id_domestic)
   else
    set @p31exchangerate_fixed=1

   update p31WorkSheet set p31ExchangeRate_Fixed=@p31exchangerate_fixed,p31Amount_WithoutVat_FixedCurrency=@p31exchangerate_fixed*p31Amount_WithoutVat_Orig
   WHERE p31ID=@p31id

 end
end

exec dbo.p31_append_log @p31id


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

---vložení schválených worksheet záznamů do uložené faktury @p91id
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

update p31worksheet set p31Minutes_Invoiced=case when p70ID=4 then p31Minutes_Approved_Billing else null end
,p31Hours_Invoiced=case when p70ID=4 then p31Hours_Approved_Billing else null end
,p31HHMM_Invoiced=case when p70id=4 then p31HHMM_Approved_Billing else null end
,p31Value_Invoiced=case when p70id=4 then p31Value_Approved_Billing else null end
,p31Amount_WithoutVat_Invoiced=case when p70id=4 then p31Amount_WithoutVat_Approved else null end
,p31Amount_WithVat_Invoiced=case when p70id=4 then p31Amount_WithVat_Approved else null end
,p31Amount_Vat_Invoiced=case when p70id=4 then p31Amount_Vat_Approved else null end
,p31VatRate_Invoiced=case when @x15id is not null then @p91fixedvatrate else p31VatRate_Approved end
,p31Rate_Billing_Invoiced=case when p70id=4 then p31Rate_Billing_Approved else 0 end
where p91ID=@p91id AND p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31')



exec p91_recalc_amount @p91id































GO

----------P---------------p31_append_log-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_append_log') and type = 'P')
 drop procedure p31_append_log
GO




CREATE    PROCEDURE [dbo].[p31_append_log]
@p31id int
AS

INSERT INTO [dbo].[p31Worksheet_Log]
           ([p31ID]
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
           ,[p28ID_Supplier]
           ,[p49ID]
           ,[j19ID]
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
           ,[p31ExchangeRate_Fixed]
           ,[p31DateTimeFrom_Orig]
           ,[p31DateTimeUntil_Orig]
           ,[p31Value_Orig_Entried]
           ,[p31Hours_Orig]
           ,[p31Hours_Trimmed]
           ,[p31Hours_Approved_Billing]
           ,[p31Hours_Approved_Internal]
           ,[p31Hours_Invoiced]
           ,[p31IsInvoiceManual]
           ,[p31ExchangeRate_InvoiceManual]
           ,[p31DateUpdate_InvoiceManual]
           ,[j02ID_InvoiceManual]
           ,[p31DateInsert]
           ,[p31UserInsert]
           ,[p31DateUpdate]
           ,[p31UserUpdate]
           ,[p31ValidFrom]
           ,[p31ValidUntil]
           ,[p31AKDS_FPR_BODY]
           ,[p31AKDS_FPR_PODIL]
           ,[p31AKDS_FPR_OBRAT]
           ,[p31AKDS_FPR_OBRAT_FixedCurrency]
           ,[p31Calc_Pieces]
           ,[p31Calc_PieceAmount]
           ,[p35ID]
           ,[p31ApprovingSet]
           ,[o23ID_First]
           ,[j02ID_ContactPerson]
           ,[p31ExternalPID]
           ,[p31ApprovingLevel]
           ,[p31Value_FixPrice])
SELECT [p31ID]
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
           ,[p28ID_Supplier]
           ,[p49ID]
           ,[j19ID]
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
           ,[p31ExchangeRate_Fixed]
           ,[p31DateTimeFrom_Orig]
           ,[p31DateTimeUntil_Orig]
           ,[p31Value_Orig_Entried]
           ,[p31Hours_Orig]
           ,[p31Hours_Trimmed]
           ,[p31Hours_Approved_Billing]
           ,[p31Hours_Approved_Internal]
           ,[p31Hours_Invoiced]
           ,[p31IsInvoiceManual]
           ,[p31ExchangeRate_InvoiceManual]
           ,[p31DateUpdate_InvoiceManual]
           ,[j02ID_InvoiceManual]
           ,[p31DateInsert]
           ,[p31UserInsert]
           ,[p31DateUpdate]
           ,[p31UserUpdate]
           ,[p31ValidFrom]
           ,[p31ValidUntil]
           ,[p31AKDS_FPR_BODY]
           ,[p31AKDS_FPR_PODIL]
           ,[p31AKDS_FPR_OBRAT]
           ,[p31AKDS_FPR_OBRAT_FixedCurrency]
           ,[p31Calc_Pieces]
           ,[p31Calc_PieceAmount]
           ,[p35ID]
           ,[p31ApprovingSet]
           ,[o23ID_First]
           ,[j02ID_ContactPerson]
           ,[p31ExternalPID]
           ,[p31ApprovingLevel]
           ,[p31Value_FixPrice]
FROM p31Worksheet WHERE p31ID=@p31id




GO

----------P---------------p31_append_log_p91-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_append_log_p91') and type = 'P')
 drop procedure p31_append_log_p91
GO





CREATE    PROCEDURE [dbo].[p31_append_log_p91]
@p91id int
AS

INSERT INTO [dbo].[p31Worksheet_Log]
           ([p31ID]
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
           ,[p28ID_Supplier]
           ,[p49ID]
           ,[j19ID]
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
           ,[p31ExchangeRate_Fixed]
           ,[p31DateTimeFrom_Orig]
           ,[p31DateTimeUntil_Orig]
           ,[p31Value_Orig_Entried]
           ,[p31Hours_Orig]
           ,[p31Hours_Trimmed]
           ,[p31Hours_Approved_Billing]
           ,[p31Hours_Approved_Internal]
           ,[p31Hours_Invoiced]
           ,[p31IsInvoiceManual]
           ,[p31ExchangeRate_InvoiceManual]
           ,[p31DateUpdate_InvoiceManual]
           ,[j02ID_InvoiceManual]
           ,[p31DateInsert]
           ,[p31UserInsert]
           ,[p31DateUpdate]
           ,[p31UserUpdate]
           ,[p31ValidFrom]
           ,[p31ValidUntil]
           ,[p31AKDS_FPR_BODY]
           ,[p31AKDS_FPR_PODIL]
           ,[p31AKDS_FPR_OBRAT]
           ,[p31AKDS_FPR_OBRAT_FixedCurrency]
           ,[p31Calc_Pieces]
           ,[p31Calc_PieceAmount]
           ,[p35ID]
           ,[p31ApprovingSet]
           ,[o23ID_First]
           ,[j02ID_ContactPerson]
           ,[p31ExternalPID]
           ,[p31ApprovingLevel]
           ,[p31Value_FixPrice])
SELECT [p31ID]
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
           ,[p28ID_Supplier]
           ,[p49ID]
           ,[j19ID]
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
           ,[p31ExchangeRate_Fixed]
           ,[p31DateTimeFrom_Orig]
           ,[p31DateTimeUntil_Orig]
           ,[p31Value_Orig_Entried]
           ,[p31Hours_Orig]
           ,[p31Hours_Trimmed]
           ,[p31Hours_Approved_Billing]
           ,[p31Hours_Approved_Internal]
           ,[p31Hours_Invoiced]
           ,[p31IsInvoiceManual]
           ,[p31ExchangeRate_InvoiceManual]
           ,[p31DateUpdate_InvoiceManual]
           ,[j02ID_InvoiceManual]
           ,[p31DateInsert]
           ,[p31UserInsert]
           ,[p31DateUpdate]
           ,[p31UserUpdate]
           ,[p31ValidFrom]
           ,[p31ValidUntil]
           ,[p31AKDS_FPR_BODY]
           ,[p31AKDS_FPR_PODIL]
           ,[p31AKDS_FPR_OBRAT]
           ,[p31AKDS_FPR_OBRAT_FixedCurrency]
           ,[p31Calc_Pieces]
           ,[p31Calc_PieceAmount]
           ,[p35ID]
           ,[p31ApprovingSet]
           ,[o23ID_First]
           ,[j02ID_ContactPerson]
           ,[p31ExternalPID]
           ,[p31ApprovingLevel]
           ,[p31Value_FixPrice]
FROM p31Worksheet WHERE p91ID=@p91id





GO

----------P---------------p31_create_from_workflow-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_create_from_workflow') and type = 'P')
 drop procedure p31_create_from_workflow
GO


CREATE    PROCEDURE [dbo].[p31_create_from_workflow]
@record_prefix varchar(50)
,@record_pid int
,@b10id int			---workflow krok, z kterého se úkon zakládá
,@j03id_sys int
,@err_ret varchar(500) OUTPUT	

AS


set @err_ret=''

declare @p41id_def int,@j02id_def int,@p31id_template int,@d1 datetime,@d2 datetime,@p72id int,@p33id int,@half_day bit
declare @p31date datetime,@p41id int,@j02id int,@p31id_new int,@value float,@p31text nvarchar(255)
declare @b10Worksheet_ProjectFlag int,@b10Worksheet_PersonFlag int,@b10Worksheet_DateFlag int,@b10Worksheet_Text nvarchar(255),@b10Worksheet_HoursFlag int

select @p31id_template=p31ID_Template,@p72id=b10Worksheet_p72ID
FROM
b10WorkflowCommandCatalog_Binding
WHERE b10ID=@b10id

if @p31id_template is null
begin
set @err_ret='Nelze dohledat vzorový úkon @p31id_template'
return
end

select @b10Worksheet_ProjectFlag=b10Worksheet_ProjectFlag,@b10Worksheet_PersonFlag=b10Worksheet_PersonFlag,@b10Worksheet_DateFlag=b10Worksheet_DateFlag
,@b10Worksheet_Text=b10Worksheet_Text,@b10Worksheet_HoursFlag=b10Worksheet_HoursFlag
FROM
b10WorkflowCommandCatalog_Binding
WHERE b10ID=@b10id

select @p41id_def=a.p41ID,@j02id_def=a.j02ID,@p31date=a.p31Date,@value=p31Value_Orig,@p33id=p34.p33ID
,@p31text=case when @b10Worksheet_Text is not null then @b10Worksheet_Text else a.p31Text end
FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p31ID=@p31id_template

set @d1=@p31date
set @d2=@p31date


if @record_prefix='o23'	---dokument
 begin

  if @b10Worksheet_ProjectFlag=2	---projekt se má načíst z workflow
   select @p41id=a.x19RecordPID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE b.x29ID=141 AND a.o23ID=@record_pid

  if @b10Worksheet_PersonFlag=2		---osoba se má načíst z workflow
   select @j02id=a.x19RecordPID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE b.x29ID=102 AND a.o23ID=@record_pid

  if @b10Worksheet_PersonFlag=3		---osoba se má načíst ze zakladatele
   select @j02id=j02ID_Owner FROM o23Doc WHERE o23ID=@record_pid
  
  if @b10Worksheet_DateFlag=2	---datum načíst z workflow
   select @d1=o23FreeDate01,@d2=o23FreeDate02,@half_day=o23FreeBoolean01 FROM o23Doc WHERE o23ID=@record_pid

 end

if @j02id is null
 set @j02id=@j02id_def


if @p41id is null
 set @p41id=@p41id_def

set @p31date=@d1

declare @go bit

WHILE @p31date between @d1 and @d2
BEGIN
  set @go=0

  if @b10Worksheet_HoursFlag=1	---hodnotu brát podle vzorového úkonu
   set @go=1

  if @b10Worksheet_HoursFlag=2 and @p33id=1	---hodiny počítat podle pracovního fondu osoby
   begin
    select @value=dbo.j02_c21_get_hours_for_day(@j02id,@p31date)

	if @half_day=1
	 set @value=@value/convert(float,2)
	 
	if @value>0
	 set @go=1	---je pracovní den
	else
     set @go=0	---není pracovní den

   end

  if @go=1
   begin
	INSERT INTO p31Worksheet(
	p41ID,j02ID,p31Date,p31Text,p32ID,p56ID,p31Code,j02ID_Owner
	,j27ID_Billing_Orig,p31HoursEntryFlag,p31Value_Orig,p31Amount_WithoutVat_Orig,p31Amount_WithVat_Orig,p31Amount_Vat_Orig,p31VatRate_Orig
	,j02ID_ContactPerson,p31ExternalPID
	,p31DateInsert,p31DateUpdate,p31UserInsert,p31UserUpdate
	)
	SELECT @p41id,@j02id,@p31date,@p31text,p32ID,p56ID,p31Code,@j02id
	,j27ID_Billing_Orig,p31HoursEntryFlag,@value,p31Amount_WithoutVat_Orig,p31Amount_WithVat_Orig,p31Amount_Vat_Orig,p31VatRate_Orig
	,j02ID_ContactPerson,p31ExternalPID
	,getdate(),getdate(),'workflow','workflow'
	FROM p31Worksheet
	WHERE p31ID=@p31id_template

	set @p31id_new=@@IDENTITY

	if @p33id=1
	 update p31Worksheet set p31Hours_Orig=@value,p31Minutes_Orig=@value*60 WHERE p31ID=@p31id_new

	exec dbo.p31_aftersave @p31id_new,@j03id_sys,null,null

	declare @rate float,@vat_rate float
	select @rate=p31Rate_Billing_Orig,@vat_rate=p31VatRate_Orig from p31Worksheet where p31ID=@p31id_new

	if isnull(@p72id,0)>0	--automaticky úkon schválit	
	 exec dbo.p31_save_approving @p31id_new,@j03id_sys,1,@p72id,null,@value,@value,@rate,@rate,@p31text,@vat_rate,@p31date,0,0,null,null	  

	if @record_prefix='o23'	---dokument
	 begin	---vložit odkaz na nový p31 záznam do x19EntityCategory_Binding
     declare @x20id int,@x18id int

	 select @x18id=c.x18ID FROM o23Doc a INNER JOIN x23EntityField_Combo b ON a.x23ID=b.x23ID INNER JOIN x18EntityCategory c ON b.x23ID=c.x23ID WHERE a.o23ID=@record_pid
	
	 select @x20id=x20ID FROM x20EntiyToCategory WHERE x29ID=331 AND x18ID=@x18id

	 insert into x19EntityCategory_Binding(o23ID,x20ID,x19RecordPID,x19UserInsert,x19UserUpdate) values(@record_pid,@x20id,@p31id_new,'workflow','workflow')
     end

   end
   set @p31date=DATEADD(day,1,@p31date)
END

GO

----------P---------------p31_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_delete') and type = 'P')
 drop procedure p31_delete
GO



CREATE   procedure [dbo].[p31_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p31id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu worksheet úkonu z tabulky p31Worksheet

declare @islocked bit,@p34id int,@isplan bit,@p31date datetime,@p33id int,@j02id_rec int,@p91id int,@p71id int,@p41id int,@ref_pid int


select @p34id=p32.p34id,@p33id=p34.p33id,@isplan=a.p31IsPlanRecord,@p31date=a.p31Date,@j02id_rec=a.j02ID,@p91id=a.p91ID,@p71id=a.p71ID,@p41id=a.p41ID
from p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
 inner join p34ActivityGroup p34 on p32.p34ID=p34.p34id
where a.p31ID=@pid

if @p71id is not null
 set @err_ret='Tento worksheet úkon již prošel schvalovacím procesem.'

if @p91id is not null
 set @err_ret='Tento worksheet úkon patří do faktury ('+dbo.GetObjectAlias('p91',@p91id)+').'


if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND (p41ValidFrom>getdate() OR p41ValidUntil<getdate()))
 set @err_ret='Projekt byl přesunut do koše, nelze v něm upravovat úkony.'



if isnull(@err_ret,'')<>''
 return 


if @isplan=0
 begin
  --test uzamčeného období-----------
  exec p31_test_lockedperiod @j03id_sys,@p31date,@j02id_rec, @p34id, @islocked OUTPUT 
      

  if @islocked=1
    set @err_ret='Datum ['+convert(varchar(30),@p31date,104)+'] patří do uzamčeného období, úkon nelze odstranit!'

 end

if isnull(@err_ret,'')<>''
 return 



BEGIN TRANSACTION

BEGIN TRY
	if exists(select p48ID FROM p48OperativePlan WHERE p31ID=@pid)
	 UPDATE p48OperativePlan SET p31ID=NULL WHERE p31ID=@pid

	

	if exists(select p31ID FROM p31worksheet_FreeField WHERE p31ID=@pid)
	 DELETE FROM p31WorkSheet_FreeField WHERE p31ID=@pid

	if exists(select o52ID FROM o52TagBinding WHERE x29ID=331 AND o52RecordPID=@pid)
	 DELETE FROM o52TagBinding WHERE x29ID=331 AND o52RecordPID=@pid

	if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=331))
	 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=331)

	
	
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
@date_rate datetime,@pricelisttype int,@p41id int,@j02id int,@p32id int
,@ret_j27id int OUTPUT,@ret_rate float OUTPUT

---@pricelisttype=1 - fakturační ceník
---@pricelisttype=2 - nákladový ceník
AS

  set @pricelisttype=isnull(@pricelisttype,1)
  set @date_rate=isnull(@date_rate,getdate())
   
  declare @p51id int,@p34id int,@j07id int,@isbillable bit,@p28id int,@p33id int
    
  set @ret_rate=0
  
  if @pricelisttype=1	--fakturační ceník
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

	 if @p51id is null	
	  begin	--zjistit, zda neexistuje výchozí nákladový ceník v globálních proměnných
	    select @p51id=p51ID FROM p50OfficePriceList WHERE p50RatesFlag=1 AND @date_rate BETWEEN p50ValidFrom AND p50ValidUntil
		
	  end
	  
	  
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

	 if @p51id is not null
	  select @ret_j27id=j27ID FROM p51PriceList WHERE p51ID=@p51id

	 if @ret_j27id is null
      select @ret_j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Invoice'

     return	--nefakturovatelná aktivita -> nulová sazba (pravidlo platí u fakt.ceníku)
   end

   
  declare @p52id int,@rate_default_amount float,@p51id_master int

  select @ret_j27id=j27id,@rate_default_amount=case when @p33id=1 THEN p51DefaultRateT else isnull(p51DefaultRateU,0) END,@p51id_master=isnull(p51id_master,0)
  FROM p51PriceList where p51id=@p51id


  --------sazba podle aktivita+uživatel napřímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j02ID=@j02id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return



  --------sazba podle aktivita+pozice osoby napřímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j07ID=@j07id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return

  --------sazba podle pouze aktivita--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  AND j02ID IS NULL AND j07ID IS NULL
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

if @p33id=1
 begin
-------ještě možnost, že sazba je definována pro všechny časové sešity přes volbu p52IsPlusAllTimeSheets=1
--------sazba podle osoby bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p52IsPlusAllTimeSheets=1 and p32id is null and j02ID=@j02id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return

--------sazba podle pozice bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p52IsPlusAllTimeSheets=1 and p32id is null and j07ID=@j07id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return

  --------sazba podle sheet bez aktivity i personálního zdroje--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p52IsPlusAllTimeSheets=1 and p32id is null and j02ID is null AND j07id is null
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return
 end

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

---změna vyfakturovaných úkonů ve faktuře @p91id
---vstupní úkony musí být již obsaženy ve faktuře a uloženy v TEMPu - p85TempBox
---p85Prefix='p31'
---p31id - p85DataPID
---p70id - p85OtherKey1
---p31Text - p85Message
---p31Value_Invoiced - p85FreeFloat01  (částka bez DPH u peněz nebo hodiny u času)
---p31Rate_Billing_Invoiced - p85FreeFloat02 (hodinová nebo úkonová sazba)
---p31VatRate_Invoiced - p85FreeFloat03  (explicitní sazba DPH)
---p85FreeText01 - do stringu převedeno FixPriceValue
---p85FreeBoolean01 - p31IsInvoiceManual
---p85FreeNumber02 - @manualfee

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


declare @j27id int,@x15id int,@p91fixedvatrate float,@is_creditnote bit,@p31value_fixprice float


select @j27id=j27ID,@x15id=x15ID,@p91fixedvatrate=p91FixedVatRate,@is_creditnote=convert(bit,case when p91ID_CreditNoteBind IS NOT NULL THEN 1 else 0 end)
from p91Invoice
where p91ID=@p91id  

declare @p31id int,@p70id_edit int,@vatrate_edit float,@value_edit float,@text_edit nvarchar(2000),@rate_edit float
declare @p33id int
declare @p31amount_withoutvat_invoiced float,@p31amount_vat_invoiced float,@p31amount_withvat_invoiced float,@p31IsInvoiceManual bit,@manualfee float,@p32ManualFeeFlag int

DECLARE curP31 CURSOR FOR 
select p85DataPID,p85OtherKey1,p85Message,p85FreeFloat01,p85FreeFloat02,p85FreeFloat03,convert(float,p85FreeNumber01)/10000000,isnull(p85FreeBoolean01,0),convert(float,p85FreeNumber02)/10000000 from p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31'
OPEN curP31
FETCH NEXT FROM curP31  INTO @p31id,@p70id_edit,@text_edit,@value_edit,@rate_edit,@vatrate_edit,@p31value_fixprice,@p31IsInvoiceManual,@manualfee
WHILE @@FETCH_STATUS = 0
BEGIN

 select @p33id=c.p33ID,@p32ManualFeeFlag=isnull(b.p32ManualFeeFlag,0) FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
 WHERE a.p31ID=@p31id


if @x15id is not null and @vatrate_edit is null
  set @vatrate_edit=@p91fixedvatrate	---DPH se přebírá z jednotné (fixní) dph faktury
 

 if @p70id_edit=2 or @p70id_edit=3 or @p70id_edit=6	---odpis nebo paušál
  begin
   set @value_edit=0
   set @rate_edit=0
  end
 
 if @p70id_edit<>6
  set @p31value_fixprice=null

 if @p33id=1	---čas
  begin
    UPDATE p31Worksheet set p31Value_Invoiced=@value_edit,p31Rate_Billing_Invoiced=case when @p32ManualFeeFlag=0 then @rate_edit end
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
   if @p32ManualFeeFlag=0
    set @p31amount_withoutvat_invoiced=@rate_edit*@value_edit

   if @p32ManualFeeFlag=1
    set @p31amount_withoutvat_invoiced=@manualfee

   --if @x15id is null
	 --select @vatrate_edit=p31VatRate_Approved FROM p31Worksheet WHERE p31ID=@p31id	---pokud DPH není ve faktuře fixní, pak to brát z úkonu
  end
  
  
  if @p33id=2 or @p33id=5
   set @p31amount_withoutvat_invoiced=@value_edit


  set @p31amount_vat_invoiced=@p31amount_withoutvat_invoiced*@vatrate_edit/100
  set @p31amount_withvat_invoiced=@p31amount_withoutvat_invoiced+@p31amount_vat_invoiced

  UPDATE p31Worksheet set p70ID=@p70id_edit,p31IsInvoiceManual=@p31IsInvoiceManual
  ,j02ID_InvoiceManual=case when @p31IsInvoiceManual=1 then @j02id_sys else null end
  ,p31DateUpdate_InvoiceManual=case when @p31IsInvoiceManual=1 then getdate() else null end
  ,p31Amount_WithoutVat_Invoiced=@p31amount_withoutvat_invoiced,p31Amount_WithVat_Invoiced=@p31amount_withvat_invoiced
  ,p31Amount_Vat_Invoiced=@p31amount_vat_invoiced,p31VatRate_Invoiced=@vatrate_edit,j27ID_Billing_Invoiced=@j27id
  ,p31Value_FixPrice=@p31value_fixprice,p31UserUpdate=@login,p31DateUpdate=getdate()
  WHERE p31ID=@p31id

  if @text_edit is not null
   UPDATE p31Worksheet set p31Text=@text_edit WHERE p31ID=@p31id

FETCH NEXT FROM curP31 INTO @p31id,@p70id_edit,@text_edit,@value_edit,@rate_edit,@vatrate_edit,@p31value_fixprice,@p31IsInvoiceManual,@manualfee
END

CLOSE curP31
DEALLOCATE curP31


if @is_creditnote=1
 begin
	declare @p32id_creditnote int

	if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'p32ID_CreditNote' AND ISNUMERIC(x35Value)=1)
	 select @p32id_creditnote=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key like 'p32ID_CreditNote'


	if @p32id_creditnote is not null
	 begin
	   UPDATE p31Worksheet set p31Amount_WithoutVat_Approved=p31Amount_WithoutVat_Invoiced,p31Amount_WithVat_Approved= p31Amount_WithVat_Invoiced,p31Amount_Vat_Approved=p31Amount_Vat_Invoiced,p31VatRate_Approved=p31VatRate_Invoiced
	   ,p31Amount_WithoutVat_Orig=p31Amount_WithoutVat_Invoiced,p31Amount_WithVat_Orig= p31Amount_WithVat_Invoiced,p31Amount_Vat_Orig=p31Amount_Vat_Invoiced,p31VatRate_Orig=p31VatRate_Invoiced
	   ,j27ID_Billing_Orig=j27ID_Billing_Invoiced
	   ,p31Value_Approved_Billing=p31Value_Invoiced,p31Value_Orig=p31Value_Invoiced
	   ,p31Value_Approved_Internal=p31Value_Invoiced
       WHERE p91ID=@p91id AND p32ID=@p32id_creditnote
     end

 end




exec p91_recalc_amount @p91id































GO

----------P---------------p31_changelog-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_changelog') and type = 'P')
 drop procedure p31_changelog
GO



CREATE procedure [dbo].[p31_changelog]
@p31id int
AS

SELECT a.RowID as pid
,row_number() over (order by a.RowID) as Verze
,a.p31DateInsert as Založeno
,a.p31UserInsert as Založil
,a.p31DateUpdate as Aktualizováno
,a.p31UserUpdate as Aktualizoval
,a.p31Date as [Datum]
,j02.j02LastName+' '+j02.j02FirstName as [Jméno]
,isnull(p41.p41NameShort,p41.p41Name) as [Projekt]
,p56.p56Name as [Úkol]
,p32.p32Name as [Aktivita]
,case when a.p31Hours_Orig<>0 then dbo.Hours2HHMM(a.p31Hours_Orig) end as [Vykázané hodiny]
,a.p31Rate_Billing_Orig as [Výchozí sazba]
,a.p31Amount_WithoutVat_Orig as [Vykázáno bez DPH]
,j27.j27Code as [Měna]
,p71.p71Name as [Schvalování]
,j02Approve.j02LastName+' '+j02Approve.j02FirstName as [Schvalovatel]
,a.p31Approved_When as [Schváleno kdy]
,p72Approve.p72Name as [Schválený status]
,a.p31Hours_Approved_Billing as [Schválené hodiny k fakturaci]
,a.p31Amount_WithoutVat_Approved as [Schváleno bez DPH]
,a.p31ApprovingSet as [Billing dávka]
,a.p31Text as [Text]
,p91.p91Code as [Faktura]
,p70.p70Name as [Fakturační status]
,case when a.p31Hours_Invoiced<>0 then dbo.Hours2HHMM(a.p31Hours_Invoiced) end as [Vyfakturované hodiny]
,a.p31Amount_WithoutVat_Invoiced as [Vyfakturováno bez DPH]
,j27Invoice.j27Code as [Měna faktury]
,a.p31VatRate_Orig as [Výchozí DPH sazba]
,a.p31Rate_Internal_Orig as [Nákladová sazba]
,a.p31Hours_Approved_Internal as [Interní schválené hodiny]
,j27Internal.j27Code as [Měna nákl.sazby]
,a.p31ValidUntil as [Platnost záznamu]
FROM
p31Worksheet_Log a
LEFT OUTER JOIN p41Project p41 ON a.p41ID=p41.p41ID
LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID
LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID
LEFT OUTER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
LEFT OUTER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
LEFT OUTER JOIN p71ApproveStatus p71 ON a.p71ID=p71.p71ID
LEFT OUTER JOIN j02Person j02Approve ON a.j02ID_ApprovedBy=j02Approve.j02ID
LEFT OUTER JOIN j27Currency j27Invoice ON a.j27ID_Billing_Invoiced=j27Invoice.j27ID
LEFT OUTER JOIN j27Currency j27 ON a.j27ID_Billing_Orig=j27.j27ID
LEFT OUTER JOIN j27Currency j27Internal ON a.j27ID_Internal=j27Internal.j27ID
LEFT OUTER JOIN p72PreBillingStatus p72Approve ON a.p72ID_AfterApprove=p72Approve.p72ID
LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID
WHERE a.p31ID=@p31id
ORDER BY a.RowID DESC



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
  set @msg_locked='Projekt byl přesunut do koše.'
 end

if @p41WorksheetOperFlag=1 and @record_state=1
 begin
  set @record_state=2		---locked, p41WorksheetOperFlagEnum=NoEntryData
  set @msg_locked='Projekt je uzavřený pro zapisování úkonů'
 end
if @isplan=0 and @record_state=1
 begin
  --test uzamčeného období-----------
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
 set @is_access_read=1	---osoba záznamu má vždy minimálně právo na čtení

if @j02id_owner=@j02id_sys
 begin
  set @is_access_edit=1
  set @is_access_read=1
 end

if @j02id_rec=@j02id_sys AND @j02id_rec<>@j02id_owner AND @is_access_edit=0
 begin
	---osoba záznamu není vlatníkem záznamu - natypoval ho někdo jiný, globální oprávnění GR_P31_EditAsNonOwner=25
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
    set @is_access_read=1	----globální právo číst veškerý worksheet, GR_P31_Reader=21
 end

if dbo.j03_test_permission_global(@j03id_sys,23)=1
 set @is_access_approve=1	----globální právo schvalovat veškerý worksheet, GR_P31_Approver=23

if @is_access_approve=0 or @is_access_read=0 or @is_access_edit=0
begin  ---ověřování oprávnění podle vztahu nadřízený x podřízený
 if exists(select j05ID FROM j05MasterSlave WHERE j02ID_Master=@j02id_sys AND (j02ID_Slave=@j02id_rec OR @j02id_rec IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_sys)))
  set @is_access_read=1

 if exists(select j05ID FROM j05MasterSlave WHERE j05Disposition_p31 IN (2,4) AND j02ID_Master=@j02id_sys AND (j02ID_Slave=@j02id_rec OR @j02id_rec IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_sys)))
  set @is_access_edit=1
 
 if exists(select j05ID FROM j05MasterSlave WHERE j05Disposition_p31 IN (3,4) AND j02ID_Master=@j02id_sys AND (j02ID_Slave=@j02id_rec OR @j02id_rec IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_sys)))
  set @is_access_approve=1
end


if @is_access_approve=0 or @is_access_read=0 or @is_access_edit=0
begin	---ověřování oprávnění podle projektové role
	---test manažerského oprávnění do projektového worksheet---------
	declare @o28permflag int	---0-pouze vlastní worksheet,1-Číst vše v rámci projektu, 2-Číst a upravovat vše v rámci projektu,3-Číst a schvalovat vše v rámci projektu,Číst, upravovat a schvalovat vše v rámci projektu

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
end	----konec ověřování práv podle projektové role

if @is_access_read=1
 set @record_disposition=1	---právo číst záznam


if @is_access_edit=1
 set @record_disposition=2	---právo číst + editovat záznam


if @is_access_approve=1
 set @record_disposition=3	---právo číst + schvalovat záznam


if @is_access_edit=1 and @is_access_approve=1
 set @record_disposition=4	---nejvyšší právo: číst + editovat + schvalovat záznam

GO

----------P---------------p31_inhale_p72id_nonbillable-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_inhale_p72id_nonbillable') and type = 'P')
 drop procedure p31_inhale_p72id_nonbillable
GO







CREATE procedure [dbo].[p31_inhale_p72id_nonbillable]
@p31id int
,@ret_p72id int OUTPUT


AS
  set @ret_p72id=3
  

  if isnull(@p31id,0)=0
   return	---skrytý odpis

  declare @p41id int,@j02id int

  select @p41id=p41ID,@j02id=j02ID from p31Worksheet where p31ID=@p31id

  select @ret_p72id=p72ID_NonBillable FROM p41Project WHERE p41ID=@p41id


  if @ret_p72id is not null
   return	---fakturační status definován v projektu
  

  select @ret_p72id=p72ID_NonBillable FROM j02Person WHERE j02ID=@j02id

  if @ret_p72id is null
   set @ret_p72id=3	---fakturační status definován v osobě

  















GO

----------P---------------p31_recalc_internal_rates-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_recalc_internal_rates') and type = 'P')
 drop procedure p31_recalc_internal_rates
GO




CREATE    PROCEDURE [dbo].[p31_recalc_internal_rates]
@d1 datetime
,@d2 datetime
,@p51id int

AS

declare @p31ID int,@p31Date datetime,@j02ID int,@p32ID int,@p41id int
declare @p31amount_internal float,@p31value_orig float,@p31rate_internal_orig float
declare @j27id_internal int

DECLARE curCR CURSOR FOR 
SELECT p31ID,p31Date,p41ID,j02ID,p32ID,p31Value_Orig
from p31Worksheet WHERE p31Date BETWEEN @d1 AND @d2 AND p32ID IN (SELECT p32ID FROM p32Activity a INNER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE b.p33ID IN (1,3))

OPEN curCR
FETCH NEXT FROM curCR 
INTO @p31ID,@p31Date,@p41id,@j02ID,@p32ID,@p31value_orig
WHILE @@FETCH_STATUS = 0
BEGIN
  set @j27id_internal=null
  set @p31rate_internal_orig=null

  exec p31_getrate_tu @p31date,2, @p41id, @j02ID, @p32ID, @j27id_internal OUTPUT , @p31rate_internal_orig OUTPUT  

  set @p31amount_internal=@p31value_orig*@p31rate_internal_orig	

  update p31WorkSheet set p31Amount_Internal=@p31amount_internal,p31Rate_Internal_Orig=@p31rate_internal_orig,j27ID_Internal=@j27id_internal
  ,p31Rate_Internal_Approved=case when p71ID=1 THEN @p31rate_internal_orig else NULL END
  ,p31Amount_Internal_Approved=case when p71ID=1 THEN p31Hours_Approved_Internal*@p31rate_internal_orig else NULL END
  WHERE p31ID=@p31ID

  FETCH NEXT FROM curCR 
  INTO @p31ID,@p31Date,@p41id,@j02ID,@p32ID,@p31value_orig
END
CLOSE curCR
DEALLOCATE curCR
	
	
	
	
	
	
	
	
 
 





GO

----------P---------------p31_remove_approve-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_remove_approve') and type = 'P')
 drop procedure p31_remove_approve
GO




CREATE procedure [dbo].[p31_remove_approve]
@guid varchar(50)
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---vyčištění worksheet záznamů ze schvalování
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

if @err_ret<>''
 return


update p31WorkSheet set p71ID=null,p72ID_AfterApprove=null,p31Approved_When=null,p31Value_Approved_Billing=null,p31Value_Approved_Internal=null
,p31Minutes_Approved_Billing=null,p31Hours_Approved_Billing=null,p31Hours_Approved_Internal=null,p31HHMM_Approved_Billing=null,p31HHMM_Approved_Internal=null
,p31Rate_Billing_Approved=null,p31Rate_Internal_Approved=null
,p31Amount_WithoutVat_Approved=null,p31Amount_WithVat_Approved=null,p31Amount_Vat_Approved=null,p31VatRate_Approved=null,p31Amount_Internal_Approved=null
,j02ID_ApprovedBy=null
where p31ID IN (select p85DataPID FROM p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31') AND p91ID IS NULL

































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

---vyjmutí worksheet záznamů z faktury @p91id
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
,@approvingset nvarchar(100)
,@value_approved_internal float
,@value_approved_billing float
,@rate_billing_approved float
,@rate_internal_approved float
,@p31text nvarchar(2000)
,@vatrate_approved float
,@dat_p31date datetime
,@approving_level int
,@value_fixprice float
,@manualfee_approved float
,@err_ret varchar(1000) OUTPUT

AS

set @err_ret=''
---exec p31_testbeforesave_approving @p31id,@j03id_sys,@p71id,@p72id,@hours_approved,@value_approved_billing,@amount_withoutvat_approved,@rate_billing_approved,@err_ret OUTPUT

if @err_ret<>''
 return


set @vatrate_approved=isnull(@vatrate_approved,0)
set @approving_level=isnull(@approving_level,0)

----validace sazby dph------------------
declare @vatisok bit,@p31date datetime,@p33code varchar(10),@p41id int,@j27id_orig int,@j02id_sys int,@p32id int,@p32ManualFeeFlag int

select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

select @p31date=p31date,@p33code=p33Code,@p41id=a.p41id,@j27id_orig=a.j27ID_Billing_Orig,@p32id=p32.p32ID,@p32ManualFeeFlag=isnull(p32.p32ManualFeeFlag,0)
from p31worksheet a inner join p41project b on a.p41id=b.p41id
left outer join p32activity p32 on a.p32id=p32.p32id
left outer join p34activitygroup p34 on p32.p34id=p34.p34id
left outer join p33ActivityInputType p33 on p34.p33id=p33.p33id
where a.p31id=@p31id

if @dat_p31date is not null
 set @p31date=@dat_p31date


if @p33code='M'
 begin
  select @vatrate_approved=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
 end

if @p33code='MV'	--DPH sazba se u schvalování testuje jenom u finančních worksheetů
 begin
  select @vatisok=dbo.p31_testvat(@vatrate_approved,@p41id,@p31date,@j27id_orig)
  if @vatisok=0
    set @err_ret='Sazba DPH ['+convert(varchar(10),@vatrate_approved)+'%] není platná pro dané období, projekt a měnu!'
 end
-----------------------------------------

if @err_ret<>''
  return


if @p72id=2 or @p72id=6 or @p72id=3
 begin
  set @rate_billing_approved=0		---odpis nebo paušál má fakturační sazbu celkovou cenu nulovou

   set @value_approved_billing=0
 end
 
if @p72id=6		--zahrnout do paušálu - schválená hodnota je původní hodnota
 begin
  select @value_approved_billing=p31value_approved_billing from p31WorkSheet where p31ID=@p31id
 end 
else
 set @value_fixprice=null
 
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
 
if ltrim(rtrim(@approvingset))=''
 set @approvingset=null

update p31worksheet set p71id=@p71id,p72ID_AfterApprove=@p72id,j02ID_ApprovedBy=@j02id_sys,p31Approved_When=getdate(),p31ApprovingSet=@approvingset,p31Date=@p31date,p31ApprovingLevel=@approving_level,p31Value_FixPrice=@value_fixprice
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
   if @p33code='T' and @p32ManualFeeFlag=0	----hodinová sazba
 	update p31worksheet set p31Minutes_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @minutes end
	,p31HHMM_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then null else dbo.Minutes2HHMM(@minutes) end
	,p31Rate_Billing_Approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved end
	,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved*@hours end
	,p31vatrate_approved=@vatrate_approved
	,p31amount_vat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved*@hours*@vatrate_approved/100 end
	,p31amount_withvat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved*@hours+@rate_billing_approved*@hours*@vatrate_approved/100 end
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @hours end
	,p31Hours_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @hours end
	,p31Hours_Approved_Internal=@hours_internal
	where p31id=@p31id

   if @p33code='T' and @p32ManualFeeFlag=1	---pevný honorář
 	update p31worksheet set p31Minutes_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @minutes end
	,p31HHMM_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then null else dbo.Minutes2HHMM(@minutes) end
	,p31Rate_Billing_Approved=null
	,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @manualfee_approved end
	,p31vatrate_approved=@vatrate_approved
	,p31amount_vat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @manualfee_approved*@vatrate_approved/100 end
	,p31amount_withvat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @manualfee_approved+@manualfee_approved*@vatrate_approved/100 end
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @hours end
	,p31Hours_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @hours end
	,p31Hours_Approved_Internal=@hours_internal
	where p31id=@p31id

   if @p33code='U'
 	update p31worksheet set p31Rate_Billing_Approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved end
	,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@value_approved_internal
	,p31amount_withoutvat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved*@value_approved_billing end
	,p31vatrate_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @vatrate_approved end
	,p31amount_vat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved*@value_approved_billing*@vatrate_approved/100 end
	,p31amount_withvat_approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @rate_billing_approved*@value_approved_billing+@rate_billing_approved*@value_approved_billing*@vatrate_approved/100 end
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else  @value_approved_billing end
	where p31id=@p31id

   if @p33code='M' or @p33code='MV'
   begin
 	update p31worksheet set p31Amount_WithoutVat_Approved=case when p72ID_AfterApprove IN (6,3,2) then 0 else @value_approved_billing end
	,p31vatrate_approved=@vatrate_approved
	,p31amount_withvat_approved=(case when p31vatrate_orig<>@vatrate_approved or @value_approved_billing<>p31amount_withoutvat_orig then @value_approved_billing+@value_approved_billing*@vatrate_approved/100 else p31amount_withvat_orig end)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when p72ID_AfterApprove IN (6,3,2) then 0 else @value_approved_billing end
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
 	,j02ID_ApprovedBy=null,p31approved_when=null,p31Value_FixPrice=null
	,p31Amount_Internal_Approved=null
 	where p31id=@p31id
 end


 exec dbo.p31_append_log @p31id




























































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
,@approvingset nvarchar(100)
,@value_approved_internal float
,@value_approved_billing float
,@rate_billing_approved float
,@rate_internal_approved float
,@p31text nvarchar(2000)
,@vatrate_approved float
,@dat_p31date datetime
,@approving_level int
,@value_fixprice float
,@manualfee_approved float
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
set @approving_level=isnull(@approving_level,0)

----validace sazby dph------------------
declare @vatisok bit,@p31date datetime,@p33code varchar(10),@p41id int,@j27id_orig int,@j02id_sys int,@p32id int,@p32ManualFeeFlag int

select @j02id_sys=j02ID FROM j03User WHERE j03ID=@j03id_sys

select @p31date=p31date,@p33code=p33Code,@p41id=a.p41id,@j27id_orig=a.j27ID_Billing_Orig,@p32id=p32.p32ID,@p32ManualFeeFlag=isnull(p32.p32ManualFeeFlag,0)
from p31worksheet a inner join p41project b on a.p41id=b.p41id
left outer join p32activity p32 on a.p32id=p32.p32id
left outer join p34activitygroup p34 on p32.p34id=p34.p34id
left outer join p33ActivityInputType p33 on p34.p33id=p33.p33id
where a.p31id=@p31id

if @dat_p31date is not null
 set @p31date=@dat_p31date

if @p33code='M'
 begin
  select @vatrate_approved=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
 end

if @p33code='MV'	--DPH sazba se u schvalování testuje jenom u finančních worksheetů
 begin
  select @vatisok=dbo.p31_testvat(@vatrate_approved,@p41id,@p31date,@j27id_orig)
  if @vatisok=0
    set @err_ret='Sazba DPH ['+convert(varchar(10),@vatrate_approved)+'%] není platná pro dané období, projekt a měnu!'
 end
-----------------------------------------

if @err_ret<>''
  return


if @p72id=2 or @p72id=6 or @p72id=3
 begin
  set @rate_billing_approved=0		---odpis nebo paušál má fakturační sazbu celkovou cenu nulovou

   set @value_approved_billing=0
 end

if @p72id<>6
 set @value_fixprice=null
 
------if @p72id=6		--zahrnout do paušálu - schválená hodnota je původní hodnota
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
 
if ltrim(rtrim(@approvingset))=''
 set @approvingset=null

update p31worksheet_temp set p71id=@p71id,p72ID_AfterApprove=@p72id,j02ID_ApprovedBy=@j02id_sys,p31Approved_When=getdate(),p31ApprovingSet=@approvingset,p31Date=@dat_p31date,p31ApprovingLevel=@approving_level,p31Value_FixPrice=@value_fixprice
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
   if @p33code='T' and @p32ManualFeeFlag=0
 	update p31worksheet_Temp set p31Minutes_Approved_Billing=@minutes,p31HHMM_Approved_Billing=dbo.Minutes2HHMM(@minutes),p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@hours,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@hours*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@hours+@rate_billing_approved*@hours*@vatrate_approved/100
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id IN (4,7) then @hours else 0 end
	,p31Hours_Approved_Billing=@hours,p31Hours_Approved_Internal=@hours_internal
	where p31GUID=@guid AND p31id=@p31id

   if @p33code='T' and @p32ManualFeeFlag=1
 	update p31worksheet_Temp set p31Minutes_Approved_Billing=@minutes,p31HHMM_Approved_Billing=dbo.Minutes2HHMM(@minutes),p31Rate_Billing_Approved=null,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@hours_internal
	,p31amount_withoutvat_approved=@manualfee_approved,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@manualfee_approved*@vatrate_approved/100
	,p31amount_withvat_approved=@manualfee_approved+@rate_billing_approved*@hours*@vatrate_approved/100
	,p31Minutes_Approved_Internal=@minutes_internal,p31HHMM_Approved_Internal=dbo.Minutes2HHMM(@minutes_internal)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id IN (4,7) then @hours else 0 end
	,p31Hours_Approved_Billing=@hours,p31Hours_Approved_Internal=@hours_internal
	where p31GUID=@guid AND p31id=@p31id

   if @p33code='U'
 	update p31Worksheet_Temp set p31Rate_Billing_Approved=@rate_billing_approved,p31rate_internal_approved=@rate_internal_approved
	,p31amount_internal_approved=@rate_internal_approved*@value_approved_internal
	,p31amount_withoutvat_approved=@rate_billing_approved*@value_approved_billing,p31vatrate_approved=@vatrate_approved,p31amount_vat_approved=@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31amount_withvat_approved=@rate_billing_approved*@value_approved_billing+@rate_billing_approved*@value_approved_billing*@vatrate_approved/100
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id IN (4,7) then @value_approved_billing else 0 end
	where p31GUID=@guid AND p31id=@p31id

   if @p33code='M' or @p33code='MV'
   begin
 	update p31worksheet_Temp set p31Amount_WithoutVat_Approved=@value_approved_billing,p31vatrate_approved=@vatrate_approved
	,p31amount_withvat_approved=(case when p31vatrate_orig<>@vatrate_approved or @value_approved_billing<>p31amount_withoutvat_orig then @value_approved_billing+@value_approved_billing*@vatrate_approved/100 else p31amount_withvat_orig end)
	,p31value_approved_internal=@value_approved_internal
	,p31Value_Approved_Billing=case when @p72id IN (4,7) then @value_approved_billing else 0 end
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

----------P---------------p31_save_freefields_after_approving-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_save_freefields_after_approving') and type = 'P')
 drop procedure p31_save_freefields_after_approving
GO





CREATE  procedure [dbo].[p31_save_freefields_after_approving]
@guid varchar(50)

AS



if not exists(select p31ID FROM p31WorkSheet_FreeField_Temp WHERE p31GUID=@guid)
 return

INSERT INTO p31WorkSheet_FreeField(p31ID)
SELECT p31ID
FROM
p31WorkSheet_FreeField_Temp
WHERE p31GUID=@guid AND p31ID NOT IN (SELECT p31ID FROM p31WorkSheet_FreeField)


UPDATE a
   SET [p31FreeText01] = b.p31FreeText01
      ,[p31FreeText02] = b.p31FreeText02
      ,[p31FreeText03] = b.p31FreeText03
      ,[p31FreeText04] = b.p31FreeText04
      ,[p31FreeText05] = b.p31FreeText05
      ,[p31FreeText06] = b.p31FreeText06
      ,[p31FreeText07] = b.p31FreeText07
      ,[p31FreeText08] = b.p31FreeText08
      ,[p31FreeText09] = b.p31FreeText09
      ,[p31FreeText10] = b.p31FreeText10
      ,[p31FreeBoolean01] = b.p31FreeBoolean01
      ,[p31FreeBoolean02] = b.p31FreeBoolean02
      ,[p31FreeBoolean03] = b.p31FreeBoolean03
      ,[p31FreeBoolean04] = b.p31FreeBoolean04
      ,[p31FreeBoolean05] = b.p31FreeBoolean05
      ,[p31FreeBoolean06] = b.p31FreeBoolean06
      ,[p31FreeBoolean07] = b.p31FreeBoolean07
      ,[p31FreeBoolean08] = b.p31FreeBoolean08
      ,[p31FreeBoolean09] = b.p31FreeBoolean09
      ,[p31FreeBoolean10] = b.p31FreeBoolean10
      ,[p31FreeDate01] = b.p31FreeDate01
      ,[p31FreeDate02] = b.p31FreeDate02
      ,[p31FreeDate03] = b.p31FreeDate03
      ,[p31FreeDate04] = b.p31FreeDate04
      ,[p31FreeDate05] = b.p31FreeDate05
      ,[p31FreeDate06] = b.p31FreeDate06
      ,[p31FreeDate07] = b.p31FreeDate07
      ,[p31FreeDate08] = b.p31FreeDate08
      ,[p31FreeDate09] = b.p31FreeDate09
      ,[p31FreeDate10] = b.p31FreeDate10
      ,[p31FreeNumber01] = b.p31FreeNumber01
      ,[p31FreeNumber02] = b.p31FreeNumber02
      ,[p31FreeNumber03] = b.p31FreeNumber03
      ,[p31FreeNumber04] = b.p31FreeNumber04
      ,[p31FreeNumber05] = b.p31FreeNumber05
      ,[p31FreeNumber06] = b.p31FreeNumber06
      ,[p31FreeNumber07] = b.p31FreeNumber07
      ,[p31FreeNumber08] = b.p31FreeNumber08
      ,[p31FreeNumber09] = b.p31FreeNumber09
      ,[p31FreeNumber10] = b.p31FreeNumber10
      ,[p31FreeCombo01] = b.p31FreeCombo01
      ,[p31FreeCombo02] = b.p31FreeCombo02
      ,[p31FreeCombo03] = b.p31FreeCombo03
      ,[p31FreeCombo04] = b.p31FreeCombo04
      ,[p31FreeCombo05] = b.p31FreeCombo05
      ,[p31FreeCombo06] = b.p31FreeCombo06
      ,[p31FreeCombo07] = b.p31FreeCombo07
      ,[p31FreeCombo08] = b.p31FreeCombo08
      ,[p31FreeCombo09] = b.p31FreeCombo09
      ,[p31FreeCombo10] = b.p31FreeCombo10
      ,[p31FreeCombo01Text] = b.p31FreeCombo01Text
      ,[p31FreeCombo02Text] = b.p31FreeCombo02Text
      ,[p31FreeCombo03Text] = b.p31FreeCombo03Text
      ,[p31FreeCombo04Text] = b.p31FreeCombo04Text
      ,[p31FreeCombo05Text] = b.p31FreeCombo05Text
      ,[p31FreeCombo06Text] = b.p31FreeCombo06Text
      ,[p31FreeCombo07Text] = b.p31FreeCombo07Text
      ,[p31FreeCombo08Text] = b.p31FreeCombo08Text
      ,[p31FreeCombo09Text] = b.p31FreeCombo09Text
      ,[p31FreeCombo10Text] = b.p31FreeCombo10Text
FROM p31WorkSheet_FreeField a INNER JOIN p31WorkSheet_FreeField_Temp b ON a.p31ID=b.p31ID
WHERE b.p31GUID=@guid
































































GO

----------P---------------p31_setup_temp-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_setup_temp') and type = 'P')
 drop procedure p31_setup_temp
GO






CREATE procedure [dbo].[p31_setup_temp]
@p31id int	---p31id
,@guid varchar(50)
AS

--if exists(select a.x19ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE a.x19RecordPID=@p31id AND b.x29ID=331)
-- begin	---k záznamu existuje vazba na štítky
--  INSERT INTO p85TempBox(p85GUID,p85Prefix,p85OtherKey1,p85OtherKey2,p85OtherKey3,p85OtherKey4) select @guid,'x19',b.x18ID,x25ID,x19RecordPID,a.x20ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE a.x19RecordPID=@p31id AND b.x29ID=331
-- end

if exists(select p31ID FROM p31WorkSheet_FreeField WHERE p31ID=@p31id)
 begin	---k záznamu existují uživatelská pole
   if not exists(select p31ID FROM p31WorkSheet_FreeField_Temp WHERE p31GUID=@guid AND p31ID=@p31id)
    begin
	  INSERT INTO p31WorkSheet_FreeField_Temp
	  (
	  [p31GUID]
           ,[p31ID]
           ,[p31FreeText01]
           ,[p31FreeText02]
           ,[p31FreeText03]
           ,[p31FreeText04]
           ,[p31FreeText05]
           ,[p31FreeText06]
           ,[p31FreeText07]
           ,[p31FreeText08]
           ,[p31FreeText09]
           ,[p31FreeText10]
           ,[p31FreeBoolean01]
           ,[p31FreeBoolean02]
           ,[p31FreeBoolean03]
           ,[p31FreeBoolean04]
           ,[p31FreeBoolean05]
           ,[p31FreeBoolean06]
           ,[p31FreeBoolean07]
           ,[p31FreeBoolean08]
           ,[p31FreeBoolean09]
           ,[p31FreeBoolean10]
           ,[p31FreeDate01]
           ,[p31FreeDate02]
           ,[p31FreeDate03]
           ,[p31FreeDate04]
           ,[p31FreeDate05]
           ,[p31FreeDate06]
           ,[p31FreeDate07]
           ,[p31FreeDate08]
           ,[p31FreeDate09]
           ,[p31FreeDate10]
           ,[p31FreeNumber01]
           ,[p31FreeNumber02]
           ,[p31FreeNumber03]
           ,[p31FreeNumber04]
           ,[p31FreeNumber05]
           ,[p31FreeNumber06]
           ,[p31FreeNumber07]
           ,[p31FreeNumber08]
           ,[p31FreeNumber09]
           ,[p31FreeNumber10]
           ,[p31FreeCombo01]
           ,[p31FreeCombo02]
           ,[p31FreeCombo03]
           ,[p31FreeCombo04]
           ,[p31FreeCombo05]
           ,[p31FreeCombo06]
           ,[p31FreeCombo07]
           ,[p31FreeCombo08]
           ,[p31FreeCombo09]
           ,[p31FreeCombo10]
           ,[p31FreeCombo01Text]
           ,[p31FreeCombo02Text]
           ,[p31FreeCombo03Text]
           ,[p31FreeCombo04Text]
           ,[p31FreeCombo05Text]
           ,[p31FreeCombo06Text]
           ,[p31FreeCombo07Text]
           ,[p31FreeCombo08Text]
           ,[p31FreeCombo09Text]
           ,[p31FreeCombo10Text])
		   SELECT @guid
           ,[p31ID]
           ,[p31FreeText01]
           ,[p31FreeText02]
           ,[p31FreeText03]
           ,[p31FreeText04]
           ,[p31FreeText05]
           ,[p31FreeText06]
           ,[p31FreeText07]
           ,[p31FreeText08]
           ,[p31FreeText09]
           ,[p31FreeText10]
           ,[p31FreeBoolean01]
           ,[p31FreeBoolean02]
           ,[p31FreeBoolean03]
           ,[p31FreeBoolean04]
           ,[p31FreeBoolean05]
           ,[p31FreeBoolean06]
           ,[p31FreeBoolean07]
           ,[p31FreeBoolean08]
           ,[p31FreeBoolean09]
           ,[p31FreeBoolean10]
           ,[p31FreeDate01]
           ,[p31FreeDate02]
           ,[p31FreeDate03]
           ,[p31FreeDate04]
           ,[p31FreeDate05]
           ,[p31FreeDate06]
           ,[p31FreeDate07]
           ,[p31FreeDate08]
           ,[p31FreeDate09]
           ,[p31FreeDate10]
           ,[p31FreeNumber01]
           ,[p31FreeNumber02]
           ,[p31FreeNumber03]
           ,[p31FreeNumber04]
           ,[p31FreeNumber05]
           ,[p31FreeNumber06]
           ,[p31FreeNumber07]
           ,[p31FreeNumber08]
           ,[p31FreeNumber09]
           ,[p31FreeNumber10]
           ,[p31FreeCombo01]
           ,[p31FreeCombo02]
           ,[p31FreeCombo03]
           ,[p31FreeCombo04]
           ,[p31FreeCombo05]
           ,[p31FreeCombo06]
           ,[p31FreeCombo07]
           ,[p31FreeCombo08]
           ,[p31FreeCombo09]
           ,[p31FreeCombo10]
           ,[p31FreeCombo01Text]
           ,[p31FreeCombo02Text]
           ,[p31FreeCombo03Text]
           ,[p31FreeCombo04Text]
           ,[p31FreeCombo05Text]
           ,[p31FreeCombo06Text]
           ,[p31FreeCombo07Text]
           ,[p31FreeCombo08Text]
           ,[p31FreeCombo09Text]
           ,[p31FreeCombo10Text]
		   FROM p31WorkSheet_FreeField WHERE p31ID=@p31id
	end

 end


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

----------P---------------p31_split-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_split') and type = 'P')
 drop procedure p31_split
GO




CREATE  procedure [dbo].[p31_split]
@p31id int
,@j03id_sys int
,@rec1_hours float
,@rec1_text nvarchar(2000)
,@rec2_hours float
,@rec2_text nvarchar(2000)
,@err_ret varchar(1000) OUTPUT
,@p31id_new int OUTPUT

AS
set @p31id_new=0

declare @login varchar(50)

select @login=j03Login FROM j03User where j03ID=@j03id_sys

INSERT INTO p31Worksheet(
	[p41ID]
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
      ,[p31Hours_Orig]
      ,[p31Hours_Trimmed]
      ,[p31Hours_Approved_Billing]
      ,[p31Hours_Approved_Internal]
      ,[p31Hours_Invoiced]
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
      ,[p31ExchangeRate_Fixed]
      ,[p31ExchangeRate_InvoiceManual]
      ,[p31DateTimeFrom_Orig]
      ,[p31DateTimeUntil_Orig]
      ,[p31Value_Orig_Entried]
      ,[p31IsInvoiceManual]
      ,[j02ID_InvoiceManual]
      ,[p31DateUpdate_InvoiceManual]
      ,[p31DateInsert]
      ,[p31UserInsert]
      ,[p31DateUpdate]
      ,[p31UserUpdate]
      ,[p31ValidFrom]
      ,[p31ValidUntil]
      ,[p31AKDS_FPR_BODY]
      ,[p31AKDS_FPR_PODIL]
      ,[p31AKDS_FPR_OBRAT]
      ,[p31AKDS_FPR_OBRAT_FixedCurrency]
      ,[p31Calc_Pieces]
      ,[p31Calc_PieceAmount]
      ,[p35ID]
      ,[p31ApprovingSet]
      ,[o23ID_First]
      ,[p28ID_Supplier]
      ,[p49ID]
      ,[j02ID_ContactPerson]
	  )
SELECT [p41ID]
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
      ,[p31Hours_Orig]
      ,[p31Hours_Trimmed]
      ,[p31Hours_Approved_Billing]
      ,[p31Hours_Approved_Internal]
      ,[p31Hours_Invoiced]
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
      ,[p31ExchangeRate_Fixed]
      ,[p31ExchangeRate_InvoiceManual]
      ,[p31DateTimeFrom_Orig]
      ,[p31DateTimeUntil_Orig]
      ,[p31Value_Orig_Entried]
      ,[p31IsInvoiceManual]
      ,[j02ID_InvoiceManual]
      ,[p31DateUpdate_InvoiceManual]
      ,[p31DateInsert]
      ,[p31UserInsert]
      ,[p31DateUpdate]
      ,[p31UserUpdate]
      ,[p31ValidFrom]
      ,[p31ValidUntil]
      ,[p31AKDS_FPR_BODY]
      ,[p31AKDS_FPR_PODIL]
      ,[p31AKDS_FPR_OBRAT]
      ,[p31AKDS_FPR_OBRAT_FixedCurrency]
      ,[p31Calc_Pieces]
      ,[p31Calc_PieceAmount]
      ,[p35ID]
      ,[p31ApprovingSet]
      ,[o23ID_First]
      ,[p28ID_Supplier]
      ,[p49ID]
      ,[j02ID_ContactPerson]
FROM p31Worksheet
WHERE p31ID=@p31id


SELECT @p31id_new=@@IDENTITY


UPDATE p31Worksheet SET p31Text=@rec1_text,p31Value_Orig=@rec1_hours,p31Hours_Orig=@rec1_hours,p31HHMM_Orig=dbo.Hours2HHMM(@rec1_hours)
WHERE p31ID=@p31id


UPDATE p31Worksheet SET p31Text=@rec2_text,p31Value_Orig=@rec2_hours,p31Hours_Orig=@rec2_hours,p31HHMM_Orig=dbo.Hours2HHMM(@rec2_hours)
WHERE p31ID=@p31id_new


EXEC [dbo].[p31_aftersave] @p31id,@j03id_sys,null,null

EXEC [dbo].[p31_aftersave] @p31id_new,@j03id_sys,null,null























































GO

----------P---------------p31_test_beforesave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_test_beforesave') and type = 'P')
 drop procedure p31_test_beforesave
GO




CREATE procedure [dbo].[p31_test_beforesave]
@p31id int
,@j03id_sys int
,@j02id_rec int
,@p41id int
,@p56id int
,@p31date datetime
,@p32id int
,@p48id int
,@p31vatrate_orig float
,@j27id_explicit int
,@p31text nvarchar(2000)
,@value_orig float
,@manualfee float
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

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND p41IsDraft=1)
 set @err='Projekt je v režimu DRAFT, nelze do něj vykazovat úkony.'

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND p41WorksheetOperFlag=1)
 set @err='V projektu platí zákaz zapisovat úkony.'

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND p41WorksheetOperFlag=3) AND isnull(@p56id,0)=0
 set @err='V projektu je povoleno vykazovat úkony pouze přes úkol.'

if isnull(@p56id,0)<>0 and isnull(@p31id,0)=0
 begin
  if exists(select p56ID FROM p56Task WHERE p56ID=@p56id AND getdate() not between p56ValidFrom AND p56ValidUntil)
   set @err='Úkol je již uzavřen, nelze do něj vykazovat nové úkony.'
 end

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND getdate() not between p41ValidFrom AND p41ValidUntil)
 set @err='Projekt byl přesunut do archivu, nelze do něj zapisovat úkony.'

if @p48id is not null
 begin	---test operativního plánu
  if exists(select p48ID FROM p48OperativePlan WHERE p48ID=@p48id AND p31ID IS NOT NULL)
   set @err='Předávaný záznam operativního plánu byl již dříve překlopen do reality!'
 end


if @err<>''
 return

select @round2minutes=convert(int,x35Value)
FROM x35GlobalParam WHERE x35Key LIKE 'Round2Minutes' AND ISNUMERIC(x35Value)=1

select @j27id_domestic=convert(int,x35Value)
FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic' AND ISNUMERIC(x35Value)=1


declare @islocked bit,@p34id int,@isplan bit,@person nvarchar(300),@j07id_rec int,@p32IsTextRequired bit,@p32name nvarchar(200),@p34IncomeStatementFlag int
declare @p32Value_Maximum float,@p32Value_Minimum float,@p32ManualFeeFlag int

select @person=j02LastName+' '+isnull(j02FirstName,''),@j07id_rec=isnull(j07id,-1) from j02Person where j02ID=@j02id_rec


select @p34id=a.p34id,@p33id=b.p33id,@isplan=b.p34iscapacityplan,@p32IsTextRequired=a.p32IsTextRequired,@p32name=a.p32Name,@p34IncomeStatementFlag=b.p34IncomeStatementFlag
,@p32Value_Maximum=isnull(a.p32Value_Maximum,0),@p32Value_Minimum=isnull(a.p32Value_Minimum,0),@p32ManualFeeFlag=isnull(a.p32ManualFeeFlag,0)
from p32Activity a inner join p34ActivityGroup b on a.p34ID=b.p34id
where a.p32ID=@p32id

if isnull(ltrim(rtrim(@p31text)),'')='' and @p32IsTextRequired=1
 begin
  set @err='Pro aktivitu ['+@p32name+'] je povinné zadávat podrobný popis úkonu.'
  return
 end

if @p32Value_Minimum<>0 and @value_orig<=@p32Value_Minimum
 begin
  set @err='Pro aktivitu ['+@p32name+'] musí být vykázaná hodnota větší než: '+convert(varchar(10),@p32Value_Minimum)+' (nyní předáváte hodnotu: '+convert(varchar(10),@value_orig)+')'
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

if @p48id is not null and @p33id<>1
 begin
  set @err='Operativní plán může být překlopen pouze do časového úkonu.'
  return
 end

if @p33id=1 and @p32ManualFeeFlag=1 and isnull(@manualfee,0)=0
 begin
  set @err='Aktivita ['+@p32name+'] vyžaduje zadat částku pevného honoráře.'
  return
 end


declare @j02TimesheetEntryDaysBackLimit int,@j02TimesheetEntryDaysBackLimit_p34IDs varchar(100),@j02id_sys int
select @j02id_sys=j02ID,@j02TimesheetEntryDaysBackLimit=j02TimesheetEntryDaysBackLimit,@j02TimesheetEntryDaysBackLimit_p34IDs=j02TimesheetEntryDaysBackLimit_p34IDs FROM j02Person WHERE j02ID IN (SELECT j02ID FROM j03User WHERE j03ID=@j03id_sys)


if @p33id=1 AND @j02TimesheetEntryDaysBackLimit=999 and @p31date<dbo.get_today()	---může zapisovat zpětně pouze v aktuální týden
 begin
  SET DATEFIRST 1
  declare @datPondeli datetime,@datNedele datetime
  set @datNedele= DATEADD(DAY , 7-DATEPART(WEEKDAY,GETDATE()),GETDATE())
  set @datPondeli=CAST(DATEADD(DAY,-6,@datNedele) as date)
  set @datNedele=CAST(@datNedele as date)
  set @datNedele=DATEADD(MINUTE,59,DATEADD(HOUR,23,@datNedele))  
  

  if @p31date NOT BETWEEN @datPondeli AND @datNedele
   begin
    set @err='Zpětný zápis hodin máte povolený pouze pro aktuální týden.'
	return
   end
 end

if @p33id=1 and isnull(@j02TimesheetEntryDaysBackLimit,0)>0 and @p31date<dbo.get_today()
 begin
  if DATEDIFF(day,@p31date,dbo.get_today())>@j02TimesheetEntryDaysBackLimit
   begin
    if @j02TimesheetEntryDaysBackLimit_p34IDs is null OR exists(select * FROM dbo.SplitString(@j02TimesheetEntryDaysBackLimit_p34IDs,',') WHERE s=convert(varchar(10),@p34id))
	 begin
      set @err='Máte povoleno zapisovat časové úkony maximálně ['+convert(varchar(10),@j02TimesheetEntryDaysBackLimit)+'] dní dozadu.'
      return
	 end
   end	 
 end
 

---test sazby DPH------------
if @p33id=5 ----testuje se pouze money sešit s plným rozpisem DPH
 begin
  declare @vatisok bit
  
  select @vatisok=dbo.p31_testvat(@p31vatrate_orig,@p41id,@p31date,@j27id_explicit)
  
  if @vatisok=0
   set @err='Sazba DPH ['+convert(varchar(30),@p31vatrate_orig)+'%] není povolena pro tento projekt, měnu a období!'
 

 end

if @err<>''
 return

if @p33id=1 or @p33id=3 or @p33id=2  ----pro čas a kusovník a částku bez rozpisu
 select @vatrate=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)



if @isplan=0
 begin
  --test uzamčeného období-----------
  exec p31_test_lockedperiod @j03id_sys,@p31date,@j02id_rec, @p34id, @islocked OUTPUT 


  if @islocked=1
    set @err='Datum ['+convert(varchar(30),@p31date,104)+'] patří do uzamčeného období!'
 end
 
if @err<>''
 return
   
---test oprávnění zapisovat do projektu worksheet---------
declare @o28id int,@o28entryflag int,@x69id int,@is_gr_p31_entry_all_projects bit

set @is_gr_p31_entry_all_projects=dbo.j03_test_permission_global(@j03id_sys,29)

if @is_gr_p31_entry_all_projects=1
 set @o28entryflag=1	--má paušální právo zapisovat do všech projektů

if @is_gr_p31_entry_all_projects=0
begin
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
 begin ----------oprávnění k projektu podle střediska
  select @o28id=a.o28id,@o28entryflag=a.o28entryflag
  from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
  inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  where a.p34ID=@p34id AND x69.x69RecordPID=@j18id AND x67.x29ID=118
  and (isnull(x69.j02ID,0)=@j02id_rec OR isnull(x69.j07ID,0)=@j07id_rec OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec))
  AND (a.o28entryflag>0)
  
 end

if @o28id is null
 begin  
  set @err='['+@person+'] nemá v tomto projektu nebo v příslušném středisku přiřazenou roli k zapisování worksheet úkonů do sešitu ['+dbo.GetObjectAlias('p34',@p34id)+']'

 end
end
   
if @err<>''
 return  

declare @test_todo bit
set @test_todo=1
 
if @o28entryflag=1
 set @test_todo=0	--OK - právo zapisovat do projektu i všech úkolů 

if @o28entryflag=1 and @j02id_sys<>@j02id_rec and @is_gr_p31_entry_all_projects=0	--liší se osoba uživatele a osoba úkonu a nemá právo zapisovat do všech projektů
 begin
  if not exists(select j05ID FROM j05MasterSlave WHERE j02ID_Master=@j02id_sys AND j02ID_Slave=@j02id_rec)
   set @err='Nemáte oprávnění vykazovat za osobu ['+@person+'], protože nejste jeho/její nadřízený.'
   return
 end

if @o28entryflag=4	--OK - právo zapisovat do projektu přes nadřízenou osobu
 begin
  if @j02id_sys=@j02id_rec
   begin
    set @err='Za ['+@person+'] může v projektu vykazovat úkony pouze jeho/její nadřízená osoba.'
    return
   end

  set @test_todo=0
 end

if isnull(@o28entryflag,0)=0
 begin
   set @err='Projektová role osoby ['+@person+'] nemá povoleno zapisovat worksheet do zvoleného sešitu'
   return
 end
 
if @o28entryflag=2 and @p56id is null
 set @test_todo=0	  --OK - právo zapisovat do projektu přímo nebo do úkolu s WR

if @test_todo=1
 begin
	if @o28entryflag=3 and @p56id is null
	 begin
		set @err='Projektová role osoby ['+@person+'] má povoleno zapisovat pouze do projektových úkolů.'
		return
	 end 
 
	if @p56id is null
	 begin
		set @err='Musíte vybrat úkol.'
		return
	 end 

	--situace, kdy se zapisuje úkon do úkolu a osoba nemá právo zapisovat do projektu 
	SELECT @x69id=a.x69ID
	from x69EntityRole_Assign a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
	WHERE a.x69RecordPID=@p56id and x67.x29ID=356
	AND (
	isnull(a.j02ID,0)=@j02id_rec
	OR isnull(a.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec)
	OR isnull(a.j07ID,0)=@j07id_rec
	)
  

	if @x69id is null 
	 set @err='Nejste řešitelem úkolu ['+dbo.GetObjectAlias('p56',@p56id)+'] a proto nemůžete zapisovat worksheet do zvoleného sešitu.'
 end

if @err<>''
 return 

if @p56id is not null
 begin	---test překročení plánu úkolu
  declare @p56IsPlan_Hours_Ceiling bit,@p56IsPlan_Expenses_Ceiling bit,@p56Plan_Hours float,@p56Plan_Expenses float,@real_hours float,@real_expenses float

  select @p56IsPlan_Hours_Ceiling=p56IsPlan_Hours_Ceiling,@p56IsPlan_Expenses_Ceiling=p56IsPlan_Expenses_Ceiling,@p56Plan_Hours=p56Plan_Hours,@p56Plan_Expenses=p56Plan_Expenses
  FROM p56Task WHERE p56ID=@p56id

  if @p56IsPlan_Hours_Ceiling=1 and @p33id=1
   begin
     select @real_hours=sum(p31Hours_Orig) FROM p31Worksheet WHERE p56ID=@p56id and p31ID<>@p31id
     
	 if isnull(@real_hours,0)+@value_orig>@p56Plan_Hours
	  set @err='Vykázané hodiny by překročily plán hodin úkolu ['+convert(varchar(10),@p56Plan_Hours)+'h.].'
   end
   if @p56IsPlan_Expenses_Ceiling=1 and @p33id IN (2,5) and @p34IncomeStatementFlag=1
   begin
     select @real_expenses=sum(p31Amount_WithoutVat_Orig) FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID WHERE a.p56ID=@p56id AND c.p34IncomeStatementFlag=1 AND c.p33ID IN (2,5) AND a.p31ID<>@p31id
     
	 if isnull(@real_expenses,0)+@value_orig>@p56Plan_Expenses
	  set @err='Vykázaný výdaj by překročil plán (limit) výdajů úkolu ['+convert(varchar(10),@p56Plan_Expenses)+',-].'
   end
   
 end

if @err<>''
 return 

if @p33id<>1
 return	---nejedná se o časový úkon, není třeba testovat limit rozpočtu projektu

---test limitů kapacitního plánu
declare @p45id int,@p46id int,@p46ExceedFlag int,@p46HoursTotal float,@real_total float,@real_billable float,@real_nonbillable float,@p32IsBillable bit
declare @p46HoursBillable float,@p46HoursNonBillable float
select @p45id=p45ID FROM p45Budget WHERE p41ID=@p41id and getdate() BETWEEN p45ValidFrom AND p45ValidUntil

if @p45id is null
 return

select @p46id=p46ID,@p46ExceedFlag=p46ExceedFlag,@p46HoursTotal=p46HoursTotal,@p46HoursBillable=p46HoursBillable,@p46HoursNonBillable=p46HoursNonBillable
FROM p46BudgetPerson WHERE p45ID=@p45id AND j02ID=@j02id_rec AND p46ExceedFlag<>5

if @p46id is null
 return

select @real_total=sum(a.p31Hours_Orig),@real_billable=sum(case when b.p32IsBillable=1 THEN a.p31Hours_Orig end),@real_nonbillable=sum(case when b.p32IsBillable=0 THEN a.p31Hours_Orig end)
from p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID
WHERE a.p41ID=@p41id AND a.j02ID=@j02id_rec AND a.p31ID<>@p31id

set @real_nonbillable=isnull(@real_nonbillable,0)
set @real_billable=isnull(@real_billable,0)
set @real_total=isnull(@real_total,0)

if @p46ExceedFlag=2 AND @real_total+@value_orig>@p46HoursTotal and @p46HoursTotal>0
 begin
  set @err='Vykázané hodiny by překročily plán hodin rozpočtu projektu ('+convert(varchar(10),@p46HoursTotal)+'h.).'
  return
 end

select @p32IsBillable=p32IsBillable FROM p32Activity WHERE p32ID=@p32id


if @p46ExceedFlag IN (1,3) AND @p32IsBillable=1 AND @real_billable+@value_orig>@p46HoursBillable and @p46HoursBillable>0
 begin
  set @err='Vykázané fakturovatelné hodiny by překročily plán fakturovatelných hodin rozpočtu projektu ('+convert(varchar(10),@p46HoursBillable)+'h.).'
  return
 end

if @p46ExceedFlag IN (1,4) AND @p32IsBillable=0 AND @real_nonbillable+@value_orig>@p46HoursNonBillable and @p46HoursNonBillable>0
 begin
  set @err='Vykázané ne-fakturovatelné hodiny by překročily plán ne-fakturovatelných hodin rozpočtu projektu ('+convert(varchar(10),@p46HoursNonBillable)+'h.).'
  return
 end

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

----------P---------------p31_update_temp_after_edit_orig-------------------------

if exists (select 1 from sysobjects where  id = object_id('p31_update_temp_after_edit_orig') and type = 'P')
 drop procedure p31_update_temp_after_edit_orig
GO



CREATE procedure [dbo].[p31_update_temp_after_edit_orig]
@j03id_sys int				--přihlášený uživatel
,@p31id int	---p31id
,@guid varchar(50)
---volá se po editaci původního worksheet záznam ze schvalovacího dialgu
AS
update a set p41ID=b.p41ID,j02ID=b.j02ID,p32ID=b.p32ID,p56ID=b.p56ID,j02ID_ContactPerson=b.j02ID_ContactPerson,p28ID_Supplier=b.p28ID_Supplier,p49ID=b.p49ID
,p31Date=b.p31Date,p31Text=b.p31Text,p31Value_Orig_Entried=b.p31Value_Orig_Entried
,p31Hours_Orig=b.p31Hours_Orig,p31Minutes_Orig=b.p31Minutes_Orig,p31Value_Orig=b.p31Value_Orig
,p31Amount_WithoutVat_Orig=b.p31Amount_WithoutVat_Orig,p31Amount_WithVat_Orig=b.p31Amount_WithVat_Orig,p31Amount_Vat_Orig=b.p31Amount_Vat_Orig,p31VatRate_Orig=b.p31VatRate_Orig
FROM
p31Worksheet_Temp a INNER JOIN p31Worksheet b ON a.p31ID=b.p31ID
WHERE a.p31ID=@p31id AND a.p31GUID=@guid AND b.p31ID=@p31id


UPDATE a
   SET [p31FreeText01] = b.p31FreeText01
      ,[p31FreeText02] = b.p31FreeText02
      ,[p31FreeText03] = b.p31FreeText03
      ,[p31FreeText04] = b.p31FreeText04
      ,[p31FreeText05] = b.p31FreeText05
      ,[p31FreeText06] = b.p31FreeText06
      ,[p31FreeText07] = b.p31FreeText07
      ,[p31FreeText08] = b.p31FreeText08
      ,[p31FreeText09] = b.p31FreeText09
      ,[p31FreeText10] = b.p31FreeText10
      ,[p31FreeBoolean01] = b.p31FreeBoolean01
      ,[p31FreeBoolean02] = b.p31FreeBoolean02
      ,[p31FreeBoolean03] = b.p31FreeBoolean03
      ,[p31FreeBoolean04] = b.p31FreeBoolean04
      ,[p31FreeBoolean05] = b.p31FreeBoolean05
      ,[p31FreeBoolean06] = b.p31FreeBoolean06
      ,[p31FreeBoolean07] = b.p31FreeBoolean07
      ,[p31FreeBoolean08] = b.p31FreeBoolean08
      ,[p31FreeBoolean09] = b.p31FreeBoolean09
      ,[p31FreeBoolean10] = b.p31FreeBoolean10
      ,[p31FreeDate01] = b.p31FreeDate01
      ,[p31FreeDate02] = b.p31FreeDate02
      ,[p31FreeDate03] = b.p31FreeDate03
      ,[p31FreeDate04] = b.p31FreeDate04
      ,[p31FreeDate05] = b.p31FreeDate05
      ,[p31FreeDate06] = b.p31FreeDate06
      ,[p31FreeDate07] = b.p31FreeDate07
      ,[p31FreeDate08] = b.p31FreeDate08
      ,[p31FreeDate09] = b.p31FreeDate09
      ,[p31FreeDate10] = b.p31FreeDate10
      ,[p31FreeNumber01] = b.p31FreeNumber01
      ,[p31FreeNumber02] = b.p31FreeNumber02
      ,[p31FreeNumber03] = b.p31FreeNumber03
      ,[p31FreeNumber04] = b.p31FreeNumber04
      ,[p31FreeNumber05] = b.p31FreeNumber05
      ,[p31FreeNumber06] = b.p31FreeNumber06
      ,[p31FreeNumber07] = b.p31FreeNumber07
      ,[p31FreeNumber08] = b.p31FreeNumber08
      ,[p31FreeNumber09] = b.p31FreeNumber09
      ,[p31FreeNumber10] = b.p31FreeNumber10
      ,[p31FreeCombo01] = b.p31FreeCombo01
      ,[p31FreeCombo02] = b.p31FreeCombo02
      ,[p31FreeCombo03] = b.p31FreeCombo03
      ,[p31FreeCombo04] = b.p31FreeCombo04
      ,[p31FreeCombo05] = b.p31FreeCombo05
      ,[p31FreeCombo06] = b.p31FreeCombo06
      ,[p31FreeCombo07] = b.p31FreeCombo07
      ,[p31FreeCombo08] = b.p31FreeCombo08
      ,[p31FreeCombo09] = b.p31FreeCombo09
      ,[p31FreeCombo10] = b.p31FreeCombo10
      ,[p31FreeCombo01Text] = b.p31FreeCombo01Text
      ,[p31FreeCombo02Text] = b.p31FreeCombo02Text
      ,[p31FreeCombo03Text] = b.p31FreeCombo03Text
      ,[p31FreeCombo04Text] = b.p31FreeCombo04Text
      ,[p31FreeCombo05Text] = b.p31FreeCombo05Text
      ,[p31FreeCombo06Text] = b.p31FreeCombo06Text
      ,[p31FreeCombo07Text] = b.p31FreeCombo07Text
      ,[p31FreeCombo08Text] = b.p31FreeCombo08Text
      ,[p31FreeCombo09Text] = b.p31FreeCombo09Text
      ,[p31FreeCombo10Text] = b.p31FreeCombo10Text
FROM p31WorkSheet_FreeField_Temp a INNER JOIN p31WorkSheet_FreeField b ON a.p31ID=b.p31ID
WHERE a.p31ID=@p31id AND a.p31GUID=@guid AND b.p31ID=@p31id


GO

----------P---------------p32_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p32_delete') and type = 'P')
 drop procedure p32_delete
GO





CREATE   procedure [dbo].[p32_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p32id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu instituce z tabulky p51PriceList
declare @ref_pid int

if exists(select p31ID FROM p31Worksheet where p32ID=@pid)
 set @err_ret='Minimálně jeden worksheet záznam má vazbu na tuto aktivitu.'

if exists(select p63ID FROM p63Overhead where p32ID=@pid)
 set @err_ret='Aktivita má vazbu na číselník režijní fakturační přirážky.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p51ID from p52PriceList_Item WHERE p32ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden ceník sazeb má vazbu na tuto aktivitu ('+dbo.GetObjectAlias('p51',@ref_pid)+')'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--p34id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p34activitygroup
declare @ref_pid int

SELECT TOP 1 @ref_pid=p32ID from p32Activity WHERE p34ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna aktivita má vazbu na tento sešit ('+dbo.GetObjectAlias('p32',@ref_pid)+')'


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

----------P---------------p35_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p35_delete') and type = 'P')
 drop procedure p35_delete
GO




CREATE   procedure [dbo].[p35_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p35id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p35Unit
if exists(select p32ID FROM p32Activity where p35ID=@pid)
 set @err_ret='Minimálně jedna aktivita má vazbu na tuto jednotku.'

if exists(select p31ID FROM p31Worksheet where p35ID=@pid)
 set @err_ret='Minimálně jeden worksheet úkon má vazbu na tuto jednotku.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p35Unit WHERE p35ID=@pid

	
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
@j03id_sys int				--přihlášený uživatel
,@pid int					--p36id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p36LockPeriod


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

----------P---------------p39_generate_recurrence-------------------------

if exists (select 1 from sysobjects where  id = object_id('p39_generate_recurrence') and type = 'P')
 drop procedure p39_generate_recurrence
GO




CREATE procedure [dbo].[p39_generate_recurrence]
@p40id int
AS

declare @tabGenerated table (
p39id int
,p31id_newinstance int
,p39date datetime
,p39datecreate datetime
,p39text nvarchar(1000)
)

insert into @tabGenerated(p39id,p31id_newinstance,p39date,p39datecreate,p39text)
select p39id,p31id_newinstance,p39date,p39datecreate,p39text
FROM p39WorkSheet_Recurrence_Plan where p40id=@p40id and p31id_newinstance is not null


declare @p31isrecurrence bit,@p40text nvarchar(1000),@recurrence_type int,@p39date datetime
declare @generate_dayafter int,@recurrence_end datetime,@recurrence_start datetime



select @recurrence_type=p40RecurrenceType,@p40text=p40text
,@generate_dayafter=p40GenerateDayAfterSupply
,@recurrence_start=p40FirstSupplyDate
,@recurrence_end=p40LastSupplyDate
FROM p40WorkSheet_Recurrence
where p40id=@p40id

set @p39date=@recurrence_start
set @recurrence_start=dateadd(day,@generate_dayafter,@recurrence_start)


delete from p39WorkSheet_Recurrence_Plan where p40id=@p40id

declare @dat datetime,@s nvarchar(50),@p39id int,@datnext datetime,@p39text nvarchar(1000)
declare @spid varchar(11)

set @dat=@recurrence_start

set @p40text=replace(@p40text,'<','[')
set @p40text=replace(@p40text,'>',']')

WHILE @dat<=@recurrence_end
BEGIN
  --if @dat>getdate()
    BEGIN
	  set @p39text=dbo.get_parsed_text_with_period(@p40text,@p39date,0)
	
	  --set @p39text=replace(@p39text,'[MM]',right('0'+convert(varchar(10),month(@p39date)),2))
	  --set @p39text=replace(@p39text,'[DD]',right('0'+convert(varchar(10),day(@p39date)),2))
	  --set @p39text=replace(@p39text,'[YYYY]',convert(varchar(10),year(@p39date)))
	  --set @p39text=replace(@p39text,'[WW]',right('0'+convert(varchar(10),datepart(week,@p39date)),2))
	  --set @p39text=replace(@p39text,'[QQ]',right('0'+convert(varchar(10),datepart(quarter,@p39date)),2))
	  --set @p39text=replace(@p39text,'[Q]',convert(varchar(10),datepart(quarter,@p39date)))
	  --set @p39text=replace(@p39text,'[M]',convert(varchar(10),month(@p39date)))

	  set @p39text=replace(@p39text,'[','')
	  set @p39text=replace(@p39text,']','')

	 
	  set @spid=CONVERT(varchar(10),@p40id)+right(convert(varchar(4),YEAR(@p39date)),2)
	  set @spid=@spid+right('0'+convert(varchar(2),month(@p39date)),2)
	  set @spid=@spid+right('0'+convert(varchar(2),day(@p39date)),2)
	  set @p39id=CONVERT(int,@spid)
	  
	  insert into p39WorkSheet_Recurrence_Plan(p39id,p40id,p39DateCreate,p39text,p39date) values(@p39id,@p40id,@dat,@p39text,@p39date)

	  --SELECT @p39id=@@IDENTITY
	
  	

    END


   if @recurrence_type=1
    begin
     set @p39date=dateadd(dd,1,@p39date)
     set @dat=dateadd(dd,1,@dat)
    end

   if @recurrence_type=2
    begin
     set @p39date=dateadd(wk,1,@p39date)
     set @dat=dateadd(wk,1,@dat)
    end

   if @recurrence_type=3
    begin
     set @p39date=dbo.GetNextMonth(@p39date)
     set @dat=dbo.GetNextMonth(@dat)
    end

   if @recurrence_type=4
    begin
     set @p39date=dbo.GetNextQuarter(@p39date)
     set @dat=dbo.GetNextQuarter(@dat)
    end

    if @recurrence_type=5
    begin
     set @p39date=dateadd(year,1,@p39date)
     set @dat=dateadd(year,1,@dat)
    end
   

END




update p39WorkSheet_Recurrence_Plan set p31id_newinstance=b.p31id_newinstance
FROM p39WorkSheet_Recurrence_Plan a inner join @tabGenerated b on a.p39id=b.p39id
where a.p40id=@p40id and a.p39date is not null and a.p31id_newinstance is null































GO

----------P---------------p40_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p40_aftersave') and type = 'P')
 drop procedure p40_aftersave
GO







CREATE PROCEDURE [dbo].[p40_aftersave]
@p40id int
,@j03id_sys int

AS

exec [p39_generate_recurrence] @p40id


declare @j02id int
select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys

exec j03_recovery_cache @j03id_sys,@j02id

GO

----------P---------------p40_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p40_delete') and type = 'P')
 drop procedure p40_delete
GO



CREATE   procedure [dbo].[p40_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p40id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu adresy z tabulky p40WorkSheet_Recurrence

BEGIN TRANSACTION

BEGIN TRY

	if exists(select p39ID FROM p39WorkSheet_Recurrence_Plan WHERE p40ID=@pid)
	 DELETE FROM p39WorkSheet_Recurrence_Plan WHERE p40ID=@pid


	delete from p40WorkSheet_Recurrence where p40ID=@pid
	

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
declare @p41code varchar(50),@x38id int,@x38id_draft int,@p51id_billing int,@name nvarchar(200),@p28id_client int,@isdraft bit,@p41RecurMotherID int

select @p41code=p41Code,@x38id=p42.x38ID,@x38id_draft=p42.x38ID_Draft,@p51id_billing=a.p51ID_Billing,@name=a.p41Name,@p28id_client=a.p28ID_Client,@isdraft=a.p41IsDraft,@p41RecurMotherID=a.p41RecurMotherID
FROM
p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID
WHERE a.p41ID=@p41id

if left(@p41code,4)='TEMP' OR @p41code is null
 begin
  if @isdraft=1
   set @x38id=@x38id_draft

  exec dbo.x38_get_freecode_proc @x38id,141,@p41id,@isdraft,1,@p41code OUTPUT

  --set @p41code=dbo.x38_get_freecode(@x38id,141,@p41id,@isdraft,1)
  if @p41code<>''
   UPDATE p41Project SET p41Code=@p41code WHERE p41ID=@p41id 
 end 

if @p51id_billing is not null	---aktualizace názvu případného ceníku sazeb, který je nastaven na míru pro daný projekt
 begin
   if @p28id_client is not null
    select @name=p28Name+' - '+@name FROM p28Contact WHERE p28ID=@p28id_client

   if exists(select p51ID FROM p51PriceList WHERE p51IsCustomTailor=1 and p51ID=@p51id_billing)
    update p51PriceList set p51Name=@name WHERE p51ID=@p51id_billing

 end

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND (p41ParentID IS NOT NULL OR p41TreePrev<p41TreeNext))
 exec [p41_recalc_tree]	---aktualizovat stromovou strukturu projektů
else
 update p41Project set p41TreePath=isnull(p41NameShort,p41name) WHERE p41ID=@p41id

if @p41RecurMotherID is not null AND CHARINDEX(']',@name)>0
 update p41Project set p41Name=dbo.get_parsed_text_with_period(p41Name,p41RecurBaseDate,0) WHERE p41ID=@p41id
 

 exec dbo.p41_append_log @p41id




GO

----------P---------------p41_append_log-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_append_log') and type = 'P')
 drop procedure p41_append_log
GO



CREATE    PROCEDURE [dbo].[p41_append_log]
@p41id int
AS

INSERT INTO [dbo].[p41Project_Log]
           ([p41ID]
           ,[p42ID]
           ,[j02ID_Owner]
           ,[p41Name]
           ,[p41NameShort]
           ,[p41Code]
           ,[p28ID_Client]
           ,[p28ID_Billing]
           ,[p87ID]
           ,[p51ID_Billing]
           ,[p51ID_Internal]
           ,[p92ID]
           ,[b02ID]
           ,[p61ID]
           ,[j18ID]
           ,[p65ID]
           ,[p72ID_NonBillable]
           ,[p72ID_BillableHours]
           ,[p41IsDraft]
           ,[p41InvoiceDefaultText1]
           ,[p41InvoiceDefaultText2]
           ,[p41InvoiceMaturityDays]
           ,[p41WorksheetOperFlag]
           ,[p41PlanFrom]
           ,[p41PlanUntil]
           ,[p41LimitHours_Notification]
           ,[p41LimitFee_Notification]
           ,[p41RobotAddress]
           ,[p41DateInsert]
           ,[p41UserInsert]
           ,[p41DateUpdate]
           ,[p41UserUpdate]
           ,[p41ValidFrom]
           ,[p41ValidUntil]
           ,[p41ExternalPID]
           ,[p41ParentID]
           ,[p41BillingMemo]
           ,[p41IsNoNotify]
           ,[p41TreeLevel]
           ,[p41TreeIndex]
           ,[p41TreePrev]
           ,[p41TreeNext]
           ,[p41TreePath]
           ,[j02ID_ContactPerson_DefaultInWorksheet]
           ,[j02ID_ContactPerson_DefaultInInvoice]
           ,[p41RecurNameMask]
           ,[p41RecurBaseDate]
           ,[p41RecurMotherID])
SELECT [p41ID]
           ,[p42ID]
           ,[j02ID_Owner]
           ,[p41Name]
           ,[p41NameShort]
           ,[p41Code]
           ,[p28ID_Client]
           ,[p28ID_Billing]
           ,[p87ID]
           ,[p51ID_Billing]
           ,[p51ID_Internal]
           ,[p92ID]
           ,[b02ID]
           ,[p61ID]
           ,[j18ID]
           ,[p65ID]
           ,[p72ID_NonBillable]
           ,[p72ID_BillableHours]
           ,[p41IsDraft]
           ,[p41InvoiceDefaultText1]
           ,[p41InvoiceDefaultText2]
           ,[p41InvoiceMaturityDays]
           ,[p41WorksheetOperFlag]
           ,[p41PlanFrom]
           ,[p41PlanUntil]
           ,[p41LimitHours_Notification]
           ,[p41LimitFee_Notification]
           ,[p41RobotAddress]
           ,[p41DateInsert]
           ,[p41UserInsert]
           ,[p41DateUpdate]
           ,[p41UserUpdate]
           ,[p41ValidFrom]
           ,[p41ValidUntil]
           ,[p41ExternalPID]
           ,[p41ParentID]
           ,[p41BillingMemo]
           ,[p41IsNoNotify]
           ,[p41TreeLevel]
           ,[p41TreeIndex]
           ,[p41TreePrev]
           ,[p41TreeNext]
           ,[p41TreePath]
           ,[j02ID_ContactPerson_DefaultInWorksheet]
           ,[j02ID_ContactPerson_DefaultInInvoice]
           ,[p41RecurNameMask]
           ,[p41RecurBaseDate]
           ,[p41RecurMotherID]
FROM
p41Project WHERE p41ID=@p41id

GO

----------P---------------p41_batch_update_childs-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_batch_update_childs') and type = 'P')
 drop procedure p41_batch_update_childs
GO



CREATE    PROCEDURE [dbo].[p41_batch_update_childs]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p41id nadřízeného projektu
,@is_roles bit
,@is_p28id bit
,@is_p87id bit
,@is_p51id bit
,@is_p92id bit
,@is_j18id bit
,@is_p61id bit
,@is_validity bit
,@err_ret varchar(500) OUTPUT		---případná návratová chyba
AS

declare @prev int,@next int,@p28id_client int,@p28id_billing int,@p87id int,@p51id int,@p92id int,@j18id int,@p61id int,@validuntil datetime

select @prev=p41TreePrev,@next=p41TreeNext
,@p28id_client=p28ID_Client,@p28id_billing=p28ID_Billing,@p87id=p87ID,@p51id=p51ID_Billing,@p92id=p92ID,@j18id=j18ID,@p61id=p61ID
,@validuntil=p41ValidUntil
FROM p41Project
WHERE p41ID=@pid



if @is_p28id=1
 update p41Project set p28ID_Client=@p28id_client,p28ID_Billing=@p28id_billing FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_p87id=1
 update p41Project set p87ID=@p87id FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_p51id=1
 update p41Project set p51ID_Billing=@p51id FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_p92id=1
 update p41Project set p92ID=@p92id FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_j18id=1
 update p41Project set j18ID=@j18id FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_p61id=1
 update p41Project set p61ID=@p61id FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_validity=1
 update p41Project set p41ValidUntil=@validuntil FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

if @is_roles=1
BEGIN
	declare @p41id int

	DELETE FROM x69EntityRole_Assign WHERE x69RecordPID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid) AND x67ID IN (SELECT x67ID FROM x67EntityRole WHERE x29ID=141)

	DECLARE curTR CURSOR FOR 
	SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN @prev AND @next AND p41ID<>@pid

	OPEN curTR
	FETCH NEXT FROM curTR 
	INTO @p41id
	WHILE @@FETCH_STATUS = 0
	BEGIN
	 INSERT INTO x69EntityRole_Assign(x69RecordPID,x67ID,j02ID,j11ID) SELECT @p41id,x67ID,j02ID,j11ID FROM x69EntityRole_Assign WHERE x69RecordPID=@pid AND x67ID IN (SELECT x67ID FROM x67EntityRole WHERE x29ID=141)

	FETCH NEXT FROM curTR 
	INTO @p41id
	END
	CLOSE curTR
	DEALLOCATE curTR
END
  

 



GO

----------P---------------p41_convertdraft-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_convertdraft') and type = 'P')
 drop procedure p41_convertdraft
GO




CREATE    PROCEDURE [dbo].[p41_convertdraft]
@p41id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT
AS

---konverze projektu z DRAFT režimu do normálního projektu
set @err_ret=''

declare @code varchar(50),@x38id int,@isdraft bit

select @x38id=p42.x38ID,@isdraft=a.p41IsDraft
FROM
p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID
WHERE a.p41ID=@p41id

if @isdraft=0
 begin
  set @err_ret='Záznam není v ŕežimu DRAFT.'
  return
 end

exec dbo.x38_get_freecode_proc @x38id,141,@p41id,0,1,@code OUTPUT

if @code=''
 begin
  set @err_ret='Systém nedokázal složit odpovídající kód podle nastavení číselné řady. Záznam zůstává v režimu DRAFT.'
  return
 end

UPDATE p41Project SET p41Code=@code,p41IsDraft=0 WHERE p41ID=@p41id 

  







GO

----------P---------------p41_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_delete') and type = 'P')
 drop procedure p41_delete
GO



CREATE   procedure [dbo].[p41_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p41id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu projektu z tabulky p41Project
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE p41ID=@pid
if @ref_pid is not null
 set @err_ret='Do projektu byl zapsán minimálně jeden worksheet úkon.'

if exists(select p41ID FROM p41Project WHERE p41ParentID=@pid)
 set @err_ret='Projek má pod sebou minimálně jeden pod-projekt.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p41ID=@pid
if @ref_pid is not null
 set @err_ret='K projektu je vytvořen minimálně jeden úkol ('+dbo.GetObjectAlias('p56',@ref_pid)+')'



if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select b05ID FROM b05Workflow_History WHERE x29ID=141 AND b05RecordPID=@pid)
	 DELETE FROM b05Workflow_History WHERE x29ID=141 AND b05RecordPID=@pid

	if exists(select b07ID FROM b07Comment WHERE x29ID=141 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=141 AND b07RecordPID=@pid

	

	if exists(SELECT o22ID FROM o22Milestone WHERE p41ID=@pid)
	 DELETE FROM o22Milestone WHERE p41ID=@pid

	if exists(select p41ID FROM p41Project_FreeField WHERE p41ID=@pid)
	 DELETE FROM p41Project_FreeField WHERE p41ID=@pid

	if exists(select o52ID FROM o52TagBinding WHERE x29ID=141 AND o52RecordPID=@pid)
	 DELETE FROM o52TagBinding WHERE x29ID=141 AND o52RecordPID=@pid

	if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=141))
	 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=141)

	if exists(select o39ID FROM o39Project_Address WHERE p41ID=@pid)
	 DELETE FROM o39Project_Address WHERE p41ID=@pid

	if exists(select p30ID FROM p30Contact_Person WHERE p41ID=@pid)
	 DELETE FROM p30Contact_Person WHERE p41ID=@pid

	if exists(select p45ID FROM p45Budget WHERE p41ID=@pid)
	 begin
		DELETE FROM p47CapacityPlan WHERE p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE p45ID IN (select p45ID FROM p45Budget WHERE p41ID=@pid))

		DELETE FROM p46BudgetPerson WHERE p45ID IN (SELECT p45ID FROM p45Budget WHERE p41ID=@pid)
		DELETE FROM p49FinancialPlan WHERE p45ID IN (SELECT p45ID FROM p45Budget WHERE p41ID=@pid)

		DELETE FROM p45Budget WHERE p41ID=@pid
	 end

	if exists(select p48ID FROM p48OperativePlan WHERE p41ID=@pid)
	 DELETE FROM p48OperativePlan WHERE p41ID=@pid

	if exists(select j13ID FROM j13FavourteProject WHERE p41ID=@pid)
	 DELETE FROM j13FavourteProject WHERE p41ID=@pid

	if exists(select a.f01ID FROM f01Folder a INNER JOIN f02FolderType b ON a.f02ID=b.f02ID WHERE a.f01RecordPID=@pid AND b.x29ID=141)
	 DELETE FROM f01Folder WHERE f01RecordPID=@pid AND f02ID IN (select f02ID FROM f02FolderType WHERE x29ID=141)

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=141)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=141

	
	
	delete from p41Project WHERE p41ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO

----------P---------------p41_get_efektivni_sazby_podle_faktur_vypis-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_efektivni_sazby_podle_faktur_vypis') and type = 'P')
 drop procedure p41_get_efektivni_sazby_podle_faktur_vypis
GO




CREATE   procedure [dbo].[p41_get_efektivni_sazby_podle_faktur_vypis]
@pid int,@p51id int,@datfrom datetime,@datuntil datetime

---@pid=p41ID,@p51id=ceník seniority podílu na paušální odměně

AS

select j02LastName+' '+j02FirstName as Person
,p91.p91Code
,p91.p91Code+' ('+convert(varchar(10),p91.p91DateSupply,104)+')' as Faktura
,p70Name
,case a.p70ID WHEN 4 THEN 'FAK' WHEN 6 THEN 'PAU' WHEN 2 THEN 'ODP' WHEN 3 THEN 'ODP' END as StatusKod
,p31Date
,p32Name
,a.p31Hours_Orig
,case when a.p70ID=4 THEN p31Hours_Invoiced END as hodiny_fakturovat
,case when a.p70ID=6 THEN p31Hours_Orig END as hodiny_pausal
,case when a.p70ID IN (2,3) THEN p31Hours_Orig END as hodiny_odpis
,dbo.p31_getrate_from_p51id(@p51id,a.p31ID) as vzorova_sazba
,case when a.p70ID=6 then pausal.Castka*(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)/honorar_cenik.Castka end as podil_na_pausalu
,case when a.p70ID=6 then pausal.Castka*(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)/honorar_cenik.Castka/a.p31Hours_Orig else a.p31Rate_Billing_Invoiced*isnull(a.p31ExchangeRate_Invoice,1) end as efektivni_sazba
,p31Text
,a.p91ID
,a.p70ID
from
p31Worksheet a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID
LEFT OUTER JOIN
(
select xa.p91ID,sum(case when xa.j27ID_Billing_Invoiced=2 THEN xa.p31Amount_WithoutVat_Invoiced else xa.p31Amount_WithoutVat_Invoiced_Domestic END) as Castka
from p31worksheet xa inner join p32activity b on xa.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id INNER JOIN p91Invoice p91 ON xa.p91ID=p91.p91ID
where xa.p70ID=4 AND c.p34IncomeStatementFlag=2 AND c.p33id IN (2,5)
and p91.p91DateSupply between @datfrom and @datuntil and xa.p91ID IN (select p91ID from p31Worksheet where p41ID=@pid AND p91ID IS NOT NULL)
GROUP BY xa.p91ID
) pausal ON a.p91ID=pausal.p91ID
LEFT OUTER JOIN
(
select xa.p91ID,sum(case when xa.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,xa.p31ID) end*p31Hours_Orig) as Castka
from p31worksheet xa inner join p32activity b on xa.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id INNER JOIN p91Invoice p91 ON xa.p91ID=p91.p91ID
where xa.p70ID=6 AND c.p33id=1
and p91.p91DateSupply between @datfrom and @datuntil and xa.p91ID IN (select p91ID from p31Worksheet where p41ID=@pid AND p91ID IS NOT NULL)
GROUP BY xa.p91ID
) honorar_cenik ON a.p91ID=honorar_cenik.p91ID
WHERE p34.p33ID=1 AND a.p41ID=@pid AND p91.p91DateSupply between @datfrom and @datuntil AND a.p70ID=6
ORDER BY p91.p91Code,a.p31Date


GO

----------P---------------p41_get_efektivni_sazby_podle_obdobi_vypis-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_get_efektivni_sazby_podle_obdobi_vypis') and type = 'P')
 drop procedure p41_get_efektivni_sazby_podle_obdobi_vypis
GO


CREATE   procedure [dbo].[p41_get_efektivni_sazby_podle_obdobi_vypis]
@pid int,@p51id int,@datfrom datetime,@datuntil datetime

---@pid=p41ID,@p51id=ceník seniority podílu na paušální odměně

AS

declare @pausaly float,@honorar_cenik float

select @pausaly=sum(case when a.j27ID_Billing_Invoiced=2 THEN p31Amount_WithoutVat_Invoiced else p31Amount_WithoutVat_Invoiced_Domestic END)
from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
where a.p70ID=4 AND c.p34IncomeStatementFlag=2 AND c.p33id IN (2,5)
and a.p41ID=@pid AND p91.p91DateSupply between @datfrom and @datuntil AND p91.p91ID_CreditNoteBind IS NULL

select @honorar_cenik=sum(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)
from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
where a.p70ID=6 AND c.p33id=1
and a.p41ID=@pid AND p91.p91DateSupply between @datfrom and @datuntil AND p91.p91ID_CreditNoteBind IS NULL

select j02LastName+' '+j02FirstName as Person
,p91.p91Code
,p91.p91Code+' ('+convert(varchar(10),p91.p91DateSupply,104)+')' as Faktura
,p70Name
,case a.p70ID WHEN 4 THEN 'FAK' WHEN 6 THEN 'PAU' WHEN 2 THEN 'ODP' WHEN 3 THEN 'ODP' END as StatusKod
,p31Date
,p32Name
,a.p31Hours_Orig
,case when a.p70ID=4 THEN p31Hours_Invoiced END as hodiny_fakturovat
,case when a.p70ID=6 THEN p31Hours_Orig END as hodiny_pausal
,case when a.p70ID IN (2,3) THEN p31Hours_Orig END as hodiny_odpis
,dbo.p31_getrate_from_p51id(@p51id,a.p31ID) as vzorova_sazba
,case when a.p70ID=6 then @pausaly*(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)/@honorar_cenik end as podil_na_pausalu
,case when a.p70ID=6 then @pausaly*(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)/@honorar_cenik/a.p31Hours_Orig else a.p31Rate_Billing_Invoiced*isnull(a.p31ExchangeRate_Invoice,1) end as efektivni_sazba
,p31Text
,a.p91ID
,a.p70ID
from
p31Worksheet a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID
WHERE p34.p33ID=1 AND a.p41ID=@pid AND p91.p91DateSupply between @datfrom and @datuntil AND a.p70ID=6
ORDER BY p91.p91Code,a.p31Date


GO

----------P---------------p41_changelog-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_changelog') and type = 'P')
 drop procedure p41_changelog
GO



CREATE procedure [dbo].[p41_changelog]
@p41id int
AS

SELECT a.RowID as pid
,row_number() over (order by a.RowID) as Verze
,a.p41DateInsert as Založeno
,a.p41UserInsert as Založil
,a.p41DateUpdate as Aktualizováno
,a.p41UserUpdate as Aktualizoval
,a.p41Name as [Název]
,a.p41NameShort as [Zkrácený název]
,p42.p42Name as [Typ projektu]
,a.p41Code as [Kód]
,p28Client.p28Name as [Klient projektu]
,p28Invoice.p28Name as [Odběratel faktury]
,p51Billing.p51Name as [Fakturační ceník]
,p87.p87Name as [Fakturační jazyk]
,j18.j18Name as [Středisko]
,b02.b02Name as [Stav]
,a.p41IsDraft as [Je draft]
,a.p41PlanFrom as [Plánované zahájení]
,a.p41PlanUntil as [Plánované dokončení]
,a.p41LimitHours_Notification as [Limit rozpracovaných hodin]
,a.p41LimitFee_Notification as [Limit rozpracovaného honoráře]
,a.p41BillingMemo as [Fakturační poznámka]
,a.p41ExternalPID as [Externí klíč]
,parent.p41Code+' - '+parent.p41Name as [Nadřízený projekt]
,p92.p92Name as [Typ faktury]
,a.p41InvoiceMaturityDays as [Výchozí splatnost]
,a.p41InvoiceDefaultText1 as [Text faktury]
,a.p41IsNoNotify as [Vypnout notifikaci]
,p65.p65Name as [Šablona opakování]
,p61.p61Name as [Výběr povolených aktivit]
,a.p41ValidUntil as [Platnost záznamu]
FROM
p41Project_Log a
LEFT OUTER JOIN p28Contact p28Client ON a.p28ID_Client=p28Client.p28ID
LEFT OUTER JOIN p28Contact p28Invoice ON a.p28ID_Billing=p28Invoice.p28ID
LEFT OUTER JOIN p51PriceList p51Billing ON a.p51ID_Billing=p51Billing.p51ID
LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID
LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID
LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID
LEFT OUTER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID
LEFT OUTER JOIN p41Project parent ON a.p41ParentID=parent.p41ID
LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
LEFT OUTER JOIN p65Recurrence p65 ON a.p65ID=p65.p65ID
LEFT OUTER JOIN p61ActivityCluster p61 ON a.p61ID=p61.p61ID
WHERE a.p41ID=@p41id
ORDER BY a.RowID DESC

GO

----------P---------------p41_inhale_sumrow-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_inhale_sumrow') and type = 'P')
 drop procedure p41_inhale_sumrow
GO




CREATE procedure [dbo].[p41_inhale_sumrow]
@j03id_sys int
,@pid int						---p41id		
AS

declare @p56_actual_count int,@p56_closed_count int,@o22_actual_count int,@p91_count int,@p30_exist bit,@childs_count int,@is_my_favourite bit,@p40_exist bit
declare @p31_wip_time_count int,@p31_wip_expense_count int,@p31_wip_fee_count int,@p31_wip_kusovnik_count int,@b07_count int
declare @p31_approved_time_count int,@p31_approved_expense_count int,@p31_approved_fee_count int,@p31_approved_kusovnik_count int
declare @o23_count int,@p45_count int,@last_p91id int
declare @last_invoice varchar(100),@last_wip_worksheet varchar(100),@o52_exist bit,@f01_exist bit
declare @p42IsModule_p31 bit,@p42IsModule_p56 bit,@p42IsModule_o23 bit,@p42IsModule_p45 bit,@p28id_client int

select @p42IsModule_p31=a.p42IsModule_p31,@p42IsModule_p56=a.p42IsModule_p56,@p42IsModule_o23=a.p42IsModule_o23,@p42IsModule_p45=p42IsModule_p45,@p28id_client=b.p28ID_Client
FROM p42ProjectType a INNER JOIN p41Project b ON a.p42ID=b.p42ID
WHERE b.p41ID=@pid

if @p42IsModule_p56=1
begin
SELECT @p56_actual_count=sum(case when getdate() BETWEEN p56ValidFrom AND p56ValidUntil then 1 end)
,@p56_closed_count=sum(case when getdate() NOT BETWEEN p56ValidFrom AND p56ValidUntil then 1 end)
FROM p56Task
WHERE p41ID=@pid
end


SELECT @o22_actual_count=COUNT(o22ID)
FROM o22Milestone
WHERE p41ID=@pid AND o22DateUntil>=dateadd(day,-5,getdate()) AND getdate() BETWEEN o22ValidFrom AND o22ValidUntil

if @p42IsModule_p31=1
begin
SELECT @p91_count=COUNT(p91ID)
from p91Invoice
WHERE p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p41ID=@pid)
end

if exists(select p30ID FROM p30Contact_Person WHERE (p41ID=@pid or p28ID=isnull(@p28id_client,-1)) AND getdate() BETWEEN p30ValidFrom AND p30ValidUntil)
 set @p30_exist=1
else
 set @p30_exist=0

if @p42IsModule_p31=1
begin
if exists(select p40ID FROM p40WorkSheet_Recurrence WHERE p41ID=@pid)
 set @p40_exist=1
else
 set @p40_exist=0
end

if exists(select p41ID FROM p41Project WHERE p41ParentID=@pid)
 select @childs_count=count(p41ID) FROM p41Project WHERE p41ParentID=@pid

if exists(select j13ID FROM j13FavourteProject WHERE p41ID=@pid AND j03ID=@j03id_sys)
 set @is_my_favourite=1
else
 set @is_my_favourite=0

if @p42IsModule_p31=1
begin
if exists(select p31ID FROM p31Worksheet WHERE p41ID=@pid AND p71ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil)
 begin
  select @p31_wip_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_wip_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_wip_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_wip_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  WHERE a.p41ID=@pid AND a.p71ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

 end

if exists(select p31ID FROM p31Worksheet WHERE p41ID=@pid AND p71ID=1 AND p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil)
 begin
  select @p31_approved_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_approved_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_approved_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_approved_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  WHERE a.p41ID=@pid AND a.p71ID=1 AND a.p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

 end
end


if exists(select b07ID from b07Comment WHERE x29ID=141 AND b07RecordPID=@pid)
 select @b07_count=count(b07ID) FROM b07Comment WHERE x29ID=141 AND b07RecordPID=@pid

if exists(select o52ID FROM o52TagBinding WHERE x29ID=141 AND o52RecordPID=@pid)
 set @o52_exist=1
else
 set @o52_exist=0

if exists(select a.f01ID FROM f01Folder a INNER JOIN f02FolderType b ON a.f02ID=b.f02ID WHERE b.x29ID=141 AND a.f01RecordPID=@pid)
 set @f01_exist=1
else
 set @f01_exist=0

if @p42IsModule_o23=1
begin

if exists(select a.o23ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID INNER JOIN o23Doc c ON a.o23ID=c.o23ID WHERE a.x19RecordPID=@pid AND b.x29ID=141 and getdate() between c.o23ValidFrom and c.o23ValidUntil)
 select @o23_count=count(a.o23ID) FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID INNER JOIN o23Doc c ON a.o23ID=c.o23ID WHERE a.x19RecordPID=@pid AND b.x29ID=141 and getdate() between c.o23ValidFrom and c.o23ValidUntil
else
 set @o23_count=0

end

if @p42IsModule_p45=1
 select @p45_count=COUNT(p45ID) FROM p45Budget WHERE p41ID=@pid

if @p91_count>0
 select TOP 1 @last_invoice=p91Code+'/'+convert(varchar(10),p91DateSupply,104),@last_p91id=a.p91ID FROM p91Invoice a INNER JOIN p31Worksheet b ON a.p91ID=b.p91ID WHERE b.p41ID=@pid ORDER BY a.p91ID DESC

if @p31_wip_time_count>0 or @p31_approved_time_count>0 or @p31_approved_expense_count>0
 select TOP 1 @last_wip_worksheet=dbo.GetDDMMYYYYHHMM(a.p31DateInsert)+'/'+c.j02FirstName+' '+c.j02LastName+'/'+d.p32Name FROM p31Worksheet a INNER JOIN j02Person c ON a.j02ID=c.j02ID INNER JOIN p32Activity d ON a.p32ID=d.p32ID WHERE a.p41ID=@pid ORDER BY a.p31ID DESC


select isnull(@p56_actual_count,0) as p56_Actual_Count
,isnull(@p56_closed_count,0) as p56_Closed_Count
,isnull(@o22_actual_count,0) as o22_Actual_Count
,isnull(@p91_count,0) as p91_Count
,@p30_exist as p30_Exist
,isnull(@childs_count,0) as childs_Count
,@is_my_favourite as is_My_Favourite
,@p40_exist as p40_Exist
,isnull(@p31_wip_time_count,0) as p31_Wip_Time_Count
,isnull(@p31_approved_time_count,0) as p31_Approved_Time_count
,isnull(@p31_wip_expense_count,0) as p31_Wip_Expense_Count
,isnull(@p31_approved_expense_count,0) as p31_Approved_Expense_Count
,isnull(@p31_wip_fee_count,0) as p31_Wip_Fee_Count
,isnull(@p31_approved_fee_count,0) as p31_Approved_Fee_Count
,isnull(@p31_wip_kusovnik_count,0) as p31_Wip_Kusovnik_Count
,isnull(@p31_approved_kusovnik_count,0) as p31_Approved_Kusovnik_Count
,isnull(@b07_count,0) as b07_Count
,isnull(@o23_count,0) as o23_Count
,isnull(@p45_count,0) as p45_Count
,@last_invoice as Last_Invoice
,@last_p91id as Last_p91ID
,@last_wip_worksheet as Last_Wip_Worksheet
,@o52_exist as o52_Exist
,@f01_exist as f01_Exist

GO

----------P---------------p41_recalc_tree-------------------------

if exists (select 1 from sysobjects where  id = object_id('p41_recalc_tree') and type = 'P')
 drop procedure p41_recalc_tree
GO


CREATE    PROCEDURE [dbo].[p41_recalc_tree]

AS

update a set p41TreeIndex=b.TreeIndex,p41TreeLevel=b.TreeLevel,p41TreePath=b.TreePathAlias
FROM p41Project a INNER JOIN dbo.view_p41_tree_recalc b ON a.p41ID=b.p41ID
WHERE isnull(a.p41TreeIndex,0)<>b.TreeIndex or isnull(a.p41TreeLevel,0)<>b.TreeLevel or a.p41TreePath<>b.TreePathAlias

update p41Project set p41TreePrev=NULL,p41TreeNext=NULL

update a set p41TreePrev=a.p41TreeIndex,p41TreeNext=a.p41TreeIndex
FROM
p41Project a INNER JOIN p41Project b ON a.p41TreeIndex=b.p41TreeIndex-1
WHERE a.p41TreeLevel=b.p41TreeLevel OR a.p41TreeLevel>b.p41TreeLevel

update p41Project set p41TreePrev=p41TreeIndex,p41TreeNext=p41TreeIndex
where p41TreeIndex In (select max(p41TreeIndex) from p41Project)

declare @pid int,@level int,@index int,@index_max int

DECLARE curTR CURSOR FOR 
SELECT p41ID,p41TreeLevel,p41TreeIndex from p41Project WHERE p41TreePrev IS NULL AND p41TreeNext IS NULL ORDER BY p41TreeIndex

OPEN curTR
FETCH NEXT FROM curTR 
INTO @pid,@level,@index
WHILE @@FETCH_STATUS = 0
BEGIN
  set @index_max=null
  
  select TOP 1 @index_max=p41TreeIndex FROM p41Project WHERE p41TreeIndex>@index AND p41TreeLevel<=@level ORDER BY p41TreeIndex
  
  if @index_max is null
   select @index_max=max(p41TreeIndex) from p41Project
  else
   set @index_max=@index_max-1
   
 
  update p41Project set p41TreePrev=@index,p41TreeNext=@index_max WHERE p41ID=@pid

  FETCH NEXT FROM curTR 
  INTO @pid,@level,@index
END
CLOSE curTR
DEALLOCATE curTR

GO

----------P---------------p42_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p42_delete') and type = 'P')
 drop procedure p42_delete
GO






CREATE   procedure [dbo].[p42_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p42id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu instituce z tabulky p42projecttype
declare @ref_pid int

SELECT TOP 1 @ref_pid=p41ID from  p41Project WHERE p42ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden projekt má vazbu na tento typ ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


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

----------P---------------p45_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p45_aftersave') and type = 'P')
 drop procedure p45_aftersave
GO



CREATE    PROCEDURE [dbo].[p45_aftersave]
@p45id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu rozpočtu

declare @j02id int,@p45planfrom datetime

select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys


declare @rate_billing float,@rate_cost float,@p41id int,@p32id int,@j27id_costrate int,@j27id_billingrate int,@p46id int,@dat datetime

set @dat=getdate()

select top 1 @p32id=p32ID FROM p32Activity WHERE p32IsBillable=1 and p34ID IN (SELECT p34ID FROM p34ActivityGroup WHERE p33ID=1)

DECLARE curW CURSOR FOR 
	select a.p46ID,a.j02ID,b.p41ID from p46BudgetPerson a INNER JOIN p45Budget b ON a.p45ID=b.p45ID WHERE a.p45ID=@p45id
	
	OPEN curW
	FETCH NEXT FROM curW 
	INTO @p46id,@j02id,@p41id
	WHILE @@FETCH_STATUS = 0
	BEGIN				

	    exec dbo.p31_getrate_tu @dat,1,@p41id,@j02id,@p32id,@j27id_billingrate OUTPUT,@rate_billing OUTPUT

		exec dbo.p31_getrate_tu @dat,2,@p41id,@j02id,@p32id,@j27id_costrate OUTPUT,@rate_cost OUTPUT

		update p46BudgetPerson set p46BillingRate=@rate_billing,p46CostRate=@rate_cost,j27ID_BillingRate=@j27id_billingrate,j27ID_CostRate=@j27id_costrate
		WHERE p46ID=@p46id

   	  FETCH NEXT FROM curW 
   	  INTO @p46id,@j02id,@p41id
	END
	CLOSE curW
	DEALLOCATE curW







GO

----------P---------------p45_clone-------------------------

if exists (select 1 from sysobjects where  id = object_id('p45_clone') and type = 'P')
 drop procedure p45_clone
GO




CREATE    PROCEDURE [dbo].[p45_clone]
@p45id_source int
,@p45id_dest int
,@j03id_sys int
,@is_p46 bit
,@is_p47 bit
,@is_p49 bit
AS

---kopíruje rozpočet do nového

declare @j02id int,@login varchar(50)

select @login=j03Login FROM j03User WHERE j03ID=@j03id_sys

select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys

if @is_p47=1
 set @is_p46=1

if @is_p47=1
 delete from p47CapacityPlan where p46ID IN (SELECT p46ID FROM p46BudgetPerson where p45ID=@p45ID_dest)

if @is_p46=1
 delete from p46BudgetPerson where p45ID=@p45ID_dest

if @is_p49=1
begin
  delete from p49FinancialPlan where p45ID=@p45id_dest

  INSERT INTO p49FinancialPlan(p45ID,p34ID,p32ID,p28ID_Supplier,j02ID,j27ID,p49Amount,p49DateFrom,p49DateUntil,p49Text
  ,p49DateInsert,p49UserInsert,p49DateUpdate,p49UserUpdate,p49ValidFrom,p49ValidUntil)
  select @p45id_dest,p34ID,p32ID,p28ID_Supplier,j02ID,j27ID,p49Amount,p49DateFrom,p49DateUntil,p49Text
  ,p49DateInsert,p49UserInsert,p49DateUpdate,p49UserUpdate,p49ValidFrom,p49ValidUntil
  FROM
  p49FinancialPlan
  WHERE p45ID=@p45id_source
end

declare @p46id_source int,@p46id_new int

if @is_p46=1
begin
   DECLARE curW CURSOR FOR 
	select p46ID from p46BudgetPerson WHERE p45ID=@p45id_source
	
	OPEN curW
	FETCH NEXT FROM curW 
	INTO @p46id_source
	WHILE @@FETCH_STATUS = 0
	BEGIN				

	    INSERT INTO p46BudgetPerson(p45ID,j02ID,p46ExceedFlag,p46HoursBillable,p46HoursNonBillable,p46HoursTotal,p46BillingRate,j27ID_BillingRate,p46CostRate
		,j27ID_CostRate,p46Description,p46DateInsert,p46UserInsert,p46DateUpdate,p46UserUpdate,p46ValidFrom,p46ValidUntil)
		SELECT @p45id_dest,j02ID,p46ExceedFlag,p46HoursBillable,p46HoursNonBillable,p46HoursTotal,p46BillingRate,j27ID_BillingRate,p46CostRate
		,j27ID_CostRate,p46Description,p46DateInsert,p46UserInsert,p46DateUpdate,p46UserUpdate,p46ValidFrom,p46ValidUntil
		FROM
		p46BudgetPerson
		WHERE p46ID=@p46id_source

		SELECT @p46id_new=@@IDENTITY

		if @is_p47=1
		 begin
		   INSERT INTO p47CapacityPlan(p46ID,p47DateFrom,p47DateUntil,p47HoursBillable,p47HoursNonBillable,p47HoursTotal,p47Description
		   ,p47DateInsert,p47UserInsert,p47DateUpdate,p47UserUpdate,p47ValidFrom,p47ValidUntil)
		   SELECT @p46id_new,p47DateFrom,p47DateUntil,p47HoursBillable,p47HoursNonBillable,p47HoursTotal,p47Description
		   ,p47DateInsert,p47UserInsert,p47DateUpdate,p47UserUpdate,p47ValidFrom,p47ValidUntil
		   FROM p47CapacityPlan WHERE p46ID=@p46id_source
		  end

   	  FETCH NEXT FROM curW 
   	  INTO @p46id_source
	END
	CLOSE curW
	DEALLOCATE curW
end






GO

----------P---------------p45_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p45_delete') and type = 'P')
 drop procedure p45_delete
GO







CREATE   procedure [dbo].[p45_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p45id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu rozpočtu z tabulky p45Budget
declare @ref_pid int,@p41id int,@count int

select @p41id=p41ID FROM p45Budget WHERE p45ID=@pid
select @count=count(*) FROM p45Budget WHERE p41ID=@p41id

if @count>1 AND exists(select p45ID FROM p45Budget WHERE p45ID=@pid AND getdate() BETWEEN p45ValidFrom AND p45ValidUntil)
 set @err_ret='Odstranit verzi rozpočtu lze pouze v případě, že je přesunutá do archivu.'

if exists(select p47ID FROM p47CapacityPlan WHERE p47HoursTotal>0 AND p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE p45ID=@pid))
 set @err_ret='Pro odstranění rozpočtu je třeba vyčistit jeho kapacitní plán.'


if exists(select p31ID FROM p31Worksheet WHERE p49ID IN (SELECT p49ID FROM p49FinancialPlan WHERE p45ID=@pid))
 set @err_ret='Minimálně jedna položka finančního rozpočtu má vazbu s reálně vykázanými worksheet úkony.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p47CapacityPlan WHERE p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE p45ID=@pid)

	DELETE FROM p49FinancialPlan WHERE p45ID=@pid

	DELETE FROM p46BudgetPerson WHERE p45ID=@pid

	DELETE from p45Budget where p45ID=@pid

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--p48id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu operativního plánu z tabulky p48OperativePlan


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

----------P---------------p49_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p49_delete') and type = 'P')
 drop procedure p49_delete
GO




CREATE   procedure [dbo].[p49_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p49id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu finančního plánu z tabulky p49FinancialPlan
if exists(select p31ID FROM p31Worksheet WHERE p49ID=@pid)
 set @err_ret='Záznam rozpočtu již má vazbu na reálný worksheet úkon.'

if isnull(@err_ret,'')<>''
 return 


BEGIN TRANSACTION

BEGIN TRY

	delete from p49FinancialPlan WHERE p49ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p50_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p50_delete') and type = 'P')
 drop procedure p50_delete
GO




CREATE   procedure [dbo].[p50_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p50id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu z tabulky p50OfficePriceList


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY

	
	delete from p50OfficePriceList where p50ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO

----------P---------------p51_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p51_aftersave') and type = 'P')
 drop procedure p51_aftersave
GO



CREATE    PROCEDURE [dbo].[p51_aftersave]
@p51id int
,@j03id_sys int

AS

if exists(select p51ID FROM p51PriceList WHERE p51ID=@p51id AND p51IsCustomTailor=1 AND p51Name IS NULL)
 begin
  declare @p41id int,@name nvarchar(200),@p28id int

  select top 1 @p28id=p28ID FROM p28Contact WHERE p51ID_Billing=@p51id

  select top 1 @p41id=p41ID,@name=p41Name FROM p41Project WHERE p51ID_Billing=@p51id


  if @p41id is not null
   UPDATE p51PriceList set p51Name=dbo.GetObjectAlias('p41',@p41id) WHERE p51ID=@p51id
  

  if @p41id is null and @p28id is not null
   UPDATE p51PriceList set p51Name=dbo.GetObjectAlias('p28',@p28id) WHERE p51ID=@p51id

   

   
 end


 
 





GO

----------P---------------p51_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p51_delete') and type = 'P')
 drop procedure p51_delete
GO





CREATE   procedure [dbo].[p51_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p51id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu instituce z tabulky p51PriceList
declare @ref_pid int

SELECT TOP 1 @ref_pid=p28ID from p28Contact WHERE p51ID_Billing=@pid OR p51ID_Internal=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden klient má vazbu na tento ceník ('+dbo.GetObjectAlias('p28',@ref_pid)+')'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID from p41Project WHERE p51ID_Billing=@pid OR p51ID_Internal=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden projekt má vazbu na tento ceník ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--p53id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu dph sazby z tabulky p53VatRate


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
declare @p56code varchar(50),@x38id int,@name nvarchar(200),@p56RecurMotherID int

select @p56code=p56Code,@x38id=p57.x38ID,@name=a.p56Name,@p56RecurMotherID=a.p56RecurMotherID
FROM
p56Task a INNER JOIN p57TaskType p57 ON a.p57ID=p57.p57ID
WHERE a.p56ID=@p56id

if left(@p56code,4)='TEMP' OR @p56code is null
 begin
  set @p56code=dbo.x38_get_freecode(@x38id,356,@p56id,0,1)
  if @p56code<>''
   UPDATE p56Task SET p56Code=@p56code WHERE p56ID=@p56id 
 end 

if @p56RecurMotherID is not null AND CHARINDEX(']',@name)>0
 update p56Task set p56Name=dbo.get_parsed_text_with_period(p56Name,p56RecurBaseDate,0) WHERE p56ID=@p56id
 

exec dbo.p56_append_log @p56id

declare @j02id int
select @j02id=j02ID FROM j03User WHERE j03ID=@j03id_sys

exec j03_recovery_cache @j03id_sys,@j02id




GO

----------P---------------p56_append_log-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_append_log') and type = 'P')
 drop procedure p56_append_log
GO





CREATE    PROCEDURE [dbo].[p56_append_log]
@p56id int
AS

INSERT INTO [dbo].[p56Task_Log]
           ([p56ID]
           ,[p41ID]
           ,[p57ID]
           ,[o22ID]
           ,[j02ID_Owner]
           ,[b02ID]
           ,[p65ID]
           ,[o43ID]
           ,[p59ID_Submitter]
           ,[p59ID_Receiver]
           ,[p56Name]
           ,[p56NameShort]
           ,[p56Code]
           ,[p56Description]
           ,[p56Ordinary]
           ,[p56PlanFrom]
           ,[p56PlanUntil]
           ,[p56ReminderDate]
           ,[p56Plan_Hours]
           ,[p56Plan_Expenses]
           ,[p56CompletePercent]
           ,[p56RatingValue]
           ,[p56DateInsert]
           ,[p56UserInsert]
           ,[p56DateUpdate]
           ,[p56UserUpdate]
           ,[p56ValidFrom]
           ,[p56ValidUntil]
           ,[p56ExternalPID]
           ,[p56IsPlan_Hours_Ceiling]
           ,[p56IsPlan_Expenses_Ceiling]
           ,[p56IsHtml]
           ,[p56DescriptionHtml]
           ,[p56IsNoNotify]
           ,[p56RecurNameMask]
           ,[p56RecurBaseDate]
           ,[p56RecurMotherID])
SELECT [p56ID]
           ,[p41ID]
           ,[p57ID]
           ,[o22ID]
           ,[j02ID_Owner]
           ,[b02ID]
           ,[p65ID]
           ,[o43ID]
           ,[p59ID_Submitter]
           ,[p59ID_Receiver]
           ,[p56Name]
           ,[p56NameShort]
           ,[p56Code]
           ,[p56Description]
           ,[p56Ordinary]
           ,[p56PlanFrom]
           ,[p56PlanUntil]
           ,[p56ReminderDate]
           ,[p56Plan_Hours]
           ,[p56Plan_Expenses]
           ,[p56CompletePercent]
           ,[p56RatingValue]
           ,[p56DateInsert]
           ,[p56UserInsert]
           ,[p56DateUpdate]
           ,[p56UserUpdate]
           ,[p56ValidFrom]
           ,[p56ValidUntil]
           ,[p56ExternalPID]
           ,[p56IsPlan_Hours_Ceiling]
           ,[p56IsPlan_Expenses_Ceiling]
           ,[p56IsHtml]
           ,[p56DescriptionHtml]
           ,[p56IsNoNotify]
           ,[p56RecurNameMask]
           ,[p56RecurBaseDate]
           ,[p56RecurMotherID]
FROM p56Task WHERE p56ID=@p56id



GO

----------P---------------p56_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_delete') and type = 'P')
 drop procedure p56_delete
GO




CREATE   procedure [dbo].[p56_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p56id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu úkolu z tabulky p56Task
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p31ID from p31Worksheet WHERE p56ID=@pid
if @ref_pid is not null
 set @err_ret='K úkolu má vazbu minimálně jeden worksheet úkon.'




if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	

	if exists(SELECT o22ID FROM o22Milestone WHERE p56ID=@pid)
	 DELETE FROM o22Milestone WHERE p56ID=@pid

	if exists(select p56ID FROM p56Task_FreeField WHERE p56ID=@pid)
	 DELETE FROM p56Task_FreeField WHERE p56ID=@pid

	if exists(select o52ID FROM o52TagBinding WHERE x29ID=356 AND o52RecordPID=@pid)
	 DELETE FROM o52TagBinding WHERE x29ID=356 AND o52RecordPID=@pid

	if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=356))
	 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=356)

	if exists(select o43ID FROM o43ImapRobotHistory WHERE p56ID=@pid)
	 DELETE FROM o43ImapRobotHistory WHERE p56ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IS NOT NULL AND b07ID IN (select b07ID FROM b07Comment WHERE x29ID=356 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IS NOT NULL AND b07ID IN (select b07ID FROM b07Comment WHERE x29ID=356 AND b07RecordPID=@pid)

	if exists(select b05iD FROM b05Workflow_History WHERE x29ID=356 AND b05RecordPID=@pid)
	 DELETE FROM b05Workflow_History WHERE x29ID=356 AND b05RecordPID=@pid

	if exists(select b07ID FROM b07Comment WHERE x29ID=356 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=356 AND b07RecordPID=@pid

	if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=356)
	 DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=356

	
	


	delete from p56Task WHERE p56ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p56_changelog-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_changelog') and type = 'P')
 drop procedure p56_changelog
GO




CREATE procedure [dbo].[p56_changelog]
@p56id int
AS

SELECT a.RowID as pid
,row_number() over (order by a.RowID) as Verze
,a.p56DateInsert as Založeno
,a.p56UserInsert as Založil
,a.p56DateUpdate as Aktualizováno
,a.p56UserUpdate as Aktualizoval
,a.p56Name as [Název]
,a.p56NameShort as [Zkrácený název]
,b02.b02Name as [Stav]
,p57.p57Name as [Typ úkolu]
,a.p56Code as [Kód]
,p41.p41Name as [Projekt]
,o22.o22Name as [Milník úkolu]
,p65.p65Name as [Šablona opakování]

,a.p56Description as [Podrobný popis]
,a.p56PlanFrom as [Plánované zahájení]
,a.p56PlanUntil as [Plánované dokončení]
,a.p56Plan_Hours as [Plán hodin]
,a.p56Plan_Expenses as [Plán výdajů]
,a.p56IsPlan_Hours_Ceiling as [Překročit hodiny]
,a.p56IsPlan_Expenses_Ceiling as [Překročit výdaje]
,a.p56ExternalPID as [Externí klíč]
,a.p56ValidUntil as [Platnost záznamu]
FROM
p56Task_Log a
LEFT OUTER JOIN p41Project p41 ON a.p41ID=p41.p41ID
LEFT OUTER JOIN p57TaskType p57 ON a.p57ID=p57.p57ID
LEFT OUTER JOIN o22Milestone o22 ON a.o22ID=o22.o22ID
LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID
LEFT OUTER JOIN p65Recurrence p65 ON a.p65ID=p65.p65ID
WHERE a.p56ID=@p56id
ORDER BY a.RowID DESC


GO

----------P---------------p56_inhale_sumrow-------------------------

if exists (select 1 from sysobjects where  id = object_id('p56_inhale_sumrow') and type = 'P')
 drop procedure p56_inhale_sumrow
GO





CREATE procedure [dbo].[p56_inhale_sumrow]
@j03id_sys int
,@pid int						---p56id		
AS

declare @p31_wip_time_count int,@p31_wip_expense_count int,@p31_wip_fee_count int,@p31_wip_kusovnik_count int
declare @p31_approved_time_count int,@p31_approved_expense_count int,@p31_approved_fee_count int,@p31_approved_kusovnik_count int
declare @o23_count int,@p91_count int
declare @last_invoice varchar(100),@last_wip_worksheet as varchar(100)
declare @Hours_Orig float,@Expenses_Orig float,@Incomes_Orig float

declare @is_p31id bit
set @is_p31id=0

if exists(select p31ID FROM p31Worksheet WHERE p56ID=@pid)
 set @is_p31id=1

if @is_p31id=1
 begin
	select @Hours_Orig=sum(case when xc.p33ID=1 then p31Hours_Orig end)
	,@Expenses_Orig=sum(case when xc.p33ID IN (2,5) AND xc.p34IncomeStatementFlag=1 then p31Value_Orig end)
	,@Incomes_Orig=sum(case when xc.p33ID IN (2,5) AND xc.p34IncomeStatementFlag=2 then p31Value_Orig end)
	FROM p31Worksheet xa INNER JOIN p32Activity xb ON xa.p32ID=xb.p32ID INNER JOIN p34ActivityGroup xc ON xb.p34ID=xc.p34ID
	WHERE xa.p56ID=@pid and getdate() BETWEEN xa.p31ValidFrom AND xa.p31ValidUntil
end

if @is_p31id=1
begin
SELECT @p91_count=COUNT(p91ID)
from p91Invoice
WHERE p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p56ID=@pid)
end

if @is_p31id=1
begin
if exists(select p31ID FROM p31Worksheet WHERE p56ID=@pid AND p71ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil)
 begin
  select @p31_wip_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_wip_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_wip_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_wip_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  WHERE a.p56ID=@pid AND a.p71ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

 end

if exists(select p31ID FROM p31Worksheet WHERE p56ID=@pid AND p71ID=1 AND p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil)
 begin
  select @p31_approved_time_count=sum(case when c.p33ID=1 then 1 end)
  ,@p31_approved_expense_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 then 1 end)
  ,@p31_approved_fee_count=sum(case when c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 then 1 end)
  ,@p31_approved_kusovnik_count=sum(case when c.p33ID=3 then 1 end)
  from
  p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID
  WHERE a.p56ID=@pid AND a.p71ID=1 AND a.p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

 end
end


if exists(select a.x19ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE a.x19RecordPID=@pid AND b.x29ID=356)
 select @o23_count=count(a.x19ID) FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE a.x19RecordPID=@pid AND b.x29ID=356
else
 set @o23_count=0



if @p91_count>0
 select TOP 1 @last_invoice=p91Code+'/'+convert(varchar(10),p91DateSupply,104) FROM p91Invoice a INNER JOIN p31Worksheet b ON a.p91ID=b.p91ID WHERE b.p56ID=@pid ORDER BY a.p91ID DESC

if @p31_wip_time_count>0 or @p31_approved_time_count>0 or @p31_approved_expense_count>0
 select TOP 1 @last_wip_worksheet=dbo.GetDDMMYYYYHHMM(a.p31DateInsert)+'/'+c.j02FirstName+' '+c.j02LastName+'/'+d.p32Name FROM p31Worksheet a INNER JOIN j02Person c ON a.j02ID=c.j02ID INNER JOIN p32Activity d ON a.p32ID=d.p32ID WHERE a.p56ID=@pid ORDER BY a.p31ID DESC


select isnull(@p91_count,0) as p91_Count
,isnull(@p31_wip_time_count,0) as p31_Wip_Time_Count
,isnull(@p31_approved_time_count,0) as p31_Approved_Time_count
,isnull(@p31_wip_expense_count,0) as p31_Wip_Expense_Count
,isnull(@p31_approved_expense_count,0) as p31_Approved_Expense_Count
,isnull(@p31_wip_fee_count,0) as p31_Wip_Fee_Count
,isnull(@p31_approved_fee_count,0) as p31_Approved_Fee_Count
,isnull(@p31_wip_kusovnik_count,0) as p31_Wip_Kusovnik_Count
,isnull(@p31_approved_kusovnik_count,0) as p31_Approved_Kusovnik_Count
,isnull(@o23_count,0) as o23_Count
,@last_invoice as Last_Invoice
,@last_wip_worksheet as Last_Wip_Worksheet
,@Hours_Orig as Hours_Orig,@Expenses_Orig as Expenses_Orig,@Incomes_Orig as Incomes_Orig


GO

----------P---------------p57_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p57_delete') and type = 'P')
 drop procedure p57_delete
GO






CREATE   procedure [dbo].[p57_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p57id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu typu úkolu z tabulky p57TaskType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p57ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden úkol má vazbu na tento typ ('+dbo.GetObjectAlias('p56',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	if exists(select o42ID FROM o42ImapRule WHERE p57ID=@pid)
	 DELETE FROM o42ImapRule WHERE p57ID=@pid


	DELETE from p57TaskType where p57ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO

----------P---------------p59_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p59_delete') and type = 'P')
 drop procedure p59_delete
GO




CREATE   procedure [dbo].[p59_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p59id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu priority z tabulky p59Priority
declare @ref_pid int

SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p59ID_Submitter=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden úkol má vazbu na tuto prioritu (zadavatele) ('+dbo.GetObjectAlias('p56',@ref_pid)+')'

SELECT TOP 1 @ref_pid=p56ID from p56Task WHERE p59ID_Receiver=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden úkol má vazbu na tuto prioritu (řešitele) ('+dbo.GetObjectAlias('p56',@ref_pid)+')'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	

	DELETE from p59Priority where p59ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p61_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p61_delete') and type = 'P')
 drop procedure p61_delete
GO




CREATE   procedure [dbo].[p61_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p61id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu týmu osob z tabulky p61ActivityCluster
declare @ref_pid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=p41ID FROM p41Project WHERE p61ID=@pid

if @ref_pid is not null
 set @err_ret='Tento cluster aktivit je přiřazen u projektu ('+dbo.GetObjectAlias('p41',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE FROM p62ActivityCluster_Item WHERE p61ID=@pid

	DELETE FROM p61ActivityCluster WHERE p61ID=@pid

	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO

----------P---------------p63_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p63_delete') and type = 'P')
 drop procedure p63_delete
GO






CREATE   procedure [dbo].[p63_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p63id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p63Overhead


if exists(select p28ID FROM p28Contact WHERE p63ID=@pid)
 set @err_ret='Má vazbu na minimálně jednoho klienta.'

if exists(select p91ID FROM p91Invoice WHERE p63ID=@pid)
 set @err_ret='Má vazbu na minimálně jednu vystavenou fakturu.'

if isnull(@err_ret,'')<>''
 return 

DELETE FROM p63Overhead WHERE p63ID=@pid



















GO

----------P---------------p65_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p65_delete') and type = 'P')
 drop procedure p65_delete
GO








CREATE   procedure [dbo].[p65_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p65ID
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p65Recurrence

if exists(select p41ID FROM p41Project WHERE p65ID=@pid)
 set @err_ret='Existuje minimálně jeden projekt svázaný s tímto pravidlem opakování.'

if exists(select p56ID FROM p56Task WHERE p65ID=@pid)
 set @err_ret='Existuje minimálně jeden úkol svázaný s tímto pravidlem opakování.'


if isnull(@err_ret,'')<>''
 return 

DELETE FROM p65Recurrence WHERE p65ID=@pid





















GO

----------P---------------p80_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p80_delete') and type = 'P')
 drop procedure p80_delete
GO







CREATE   procedure [dbo].[p80_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p80id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p80InvoiceAmountStructure


if exists(select p92ID FROM p92InvoiceType WHERE p80ID=@pid)
 set @err_ret='Má vazbu na minimálně jeden typ faktury.'

if exists(select p91ID FROM p91Invoice WHERE p80ID=@pid)
 set @err_ret='Má vazbu na minimálně jednu vystavenou fakturu.'

if isnull(@err_ret,'')<>''
 return 

DELETE FROM p80InvoiceAmountStructure WHERE p80ID=@pid




















GO

----------P---------------p86_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p86_delete') and type = 'P')
 drop procedure p86_delete
GO







CREATE   procedure [dbo].[p86_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p86id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu bankovního účtu z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p88ID from p88InvoiceHeader_BankAccount WHERE p86ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna hlavička vystavovatele faktur obsahuje vazbu na tento účet.'


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

----------P---------------p89_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p89_delete') and type = 'P')
 drop procedure p89_delete
GO




CREATE   procedure [dbo].[p89_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p89id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu typu zálohy z tabulky p89ProformaType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p90ID from p90Proforma WHERE p89ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna zálohová faktura má vazbu na tento typ ('+dbo.GetObjectAlias('p90',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	DELETE from p89ProformaType where p89ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO

----------P---------------p90_aftersave-------------------------

if exists (select 1 from sysobjects where  id = object_id('p90_aftersave') and type = 'P')
 drop procedure p90_aftersave
GO





CREATE    PROCEDURE [dbo].[p90_aftersave]
@p90id int
,@j03id_sys int

AS

if exists(select p90ID FROM p90Proforma WHERE p90ID=@p90id AND (p90Code LIKE 'TEMP%' OR p90Code IS NULL))
 exec p90_update_code @p90id,@j03id_sys

declare @p90Amount_Billed float,@p90DateBilled datetime,@login varchar(50),@p90Code varchar(50),@p89id int,@p82code varchar(50),@p82id int,@x38id int

select @p90Amount_Billed=sum(p82Amount),@p90DateBilled=max(p82Date) FROM p82Proforma_Payment where p90ID=@p90id

set @p90Amount_Billed=isnull(@p90Amount_Billed,0)

select @login=p90UserUpdate,@p90Code=p90Code,@p89id=p89ID FROM p90Proforma WHERE p90ID=@p90id

update p90Proforma set p90Amount_Billed=@p90Amount_Billed,p90DateBilled=@p90DateBilled WHERE p90ID=@p90id

update p90Proforma set p90Amount_Debt=p90Amount-p90Amount_Billed WHERE p90ID=@p90id

DECLARE curTR CURSOR FOR 
SELECT p82ID,p82Code from p82Proforma_Payment WHERE p90ID=@p90id AND (p82Code is null OR left(@p82Code,4)='TEMP')

OPEN curTR
FETCH NEXT FROM curTR 
INTO @p82id,@p82code
WHILE @@FETCH_STATUS = 0
BEGIN
  select @x38id=x38ID_Payment FROM p89ProformaType WHERE p89ID=@p89id

  if @x38id is null
   select @x38id=x38ID FROM x38CodeLogic WHERE x29ID=382

  if @x38id is not null
   set @p82Code=dbo.x38_get_freecode(@x38id,382,@p82id,0,1)

  if @p82Code=''
   set @p82Code='DPP-'+@p90Code

  UPDATE p82Proforma_Payment SET p82Code=@p82Code WHERE p82ID=@p82id

 FETCH NEXT FROM curTR 
 INTO @p82id,@p82code
END
CLOSE curTR
DEALLOCATE curTR


---automaticky se spouští po uložení záznamu faktury
declare @p91id int

DECLARE curTRX CURSOR FOR 
SELECT p91ID from p99Invoice_Proforma WHERE p90ID=@p90id

OPEN curTRX
FETCH NEXT FROM curTRX 
INTO @p91id
WHILE @@FETCH_STATUS = 0
BEGIN
 exec p91_recalc_amount @p91id

 FETCH NEXT FROM curTRX
 INTO @p91id
END
CLOSE curTRX
DEALLOCATE curTRX






GO

----------P---------------p90_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p90_delete') and type = 'P')
 drop procedure p90_delete
GO



CREATE   procedure [dbo].[p90_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p90id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu vystavené faktury z tabulky p90Proforma
declare @ref_pid int

set @ref_pid=null
SELECT TOP 1 @ref_pid=p91ID from p99Invoice_Proforma WHERE p90ID=@pid
if @ref_pid is not null
 set @err_ret='Zálohová faktura je svázaná s daňovou fakturou ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


if isnull(@err_ret,'')<>''
 return
 

declare @p91id_bind int

BEGIN TRANSACTION

BEGIN TRY

if exists(select p99ID FROM p99Invoice_Proforma WHERE p91ID=@pid)
 begin
  select @p91id_bind=p91ID FROM p99Invoice_Proforma WHERE p90ID=@pid

  delete from p99Invoice_Proforma where p90id=@pid
 end



if exists(select p82ID FROM p82Proforma_Payment WHERE p90ID=@pid)
 DELETE FROM p82Proforma_Payment WHERE p90ID=@pid

if exists(select p90ID FROM p90Proforma_FreeField WHERE p90ID=@pid)
  delete from p90Proforma_FreeField where p90id=@pid

if exists(select o52ID FROM o52TagBinding WHERE x29ID=390 AND o52RecordPID=@pid)
 DELETE FROM o52TagBinding WHERE x29ID=390 AND o52RecordPID=@pid

if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=390))
 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=390)

if exists(SELECT o22ID FROM o22Milestone WHERE p90ID=@pid)
  DELETE FROM o22Milestone WHERE p90ID=@pid

if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=390)
  DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=390





delete from p90Proforma where p90ID=@pid

	
COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

if @p91id_bind is not null
 exec p91_recalc_amount @p91id_bind,0
















GO

----------P---------------p90_update_code-------------------------

if exists (select 1 from sysobjects where  id = object_id('p90_update_code') and type = 'P')
 drop procedure p90_update_code
GO




CREATE    PROCEDURE [dbo].[p90_update_code]
@p90id int
,@j03id_sys int

AS

---automaticky se spouští po uložení záznamu zálohy
declare @p90code varchar(50),@x38id int,@isdraft bit,@x38id_draft int

select @p90code=p90Code,@x38id=p89.x38ID,@x38id_draft=p89.x38ID_Draft,@isdraft=a.p90IsDraft
FROM
p90Proforma a INNER JOIN p89ProformaType p89 ON a.p89ID=p89.p89ID
WHERE a.p90ID=@p90id


if @isdraft=1
 set @x38id=@x38id_draft


if left(@p90code,4)='TEMP' OR @p90code is null
 begin


  set @p90code=dbo.x38_get_freecode(@x38id,390,@p90id,@isdraft,1)
  if @p90code<>''
   UPDATE p90Proforma SET p90Code=@p90code WHERE p90ID=@p90id 

  
 end 







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
else
 exec p91_append_log @p91id








GO

----------P---------------p91_append_log-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_append_log') and type = 'P')
 drop procedure p91_append_log
GO





CREATE    PROCEDURE [dbo].[p91_append_log]
@p91id int
AS

if exists(select p91ID FROM p91Invoice WHERE p91ID=@p91id AND p91Code LIKE 'TEMP%')
 return

INSERT INTO [dbo].[p91Invoice_Log]
           ([p91ID]
           ,[p92ID]
           ,[p28ID]
           ,[j27ID]
           ,[j19ID]
           ,[j02ID_Owner]
           ,[p41ID_First]
           ,[p91ID_CreditNoteBind]
           ,[b02ID]
           ,[j17ID]
           ,[o38ID_Primary]
           ,[o38ID_Delivery]
           ,[x15ID]
           ,[p98ID]
           ,[j02ID_ContactPerson]
           ,[p63ID]
           ,[p80ID]
           ,[p91FixedVatRate]
           ,[p91Code]
           ,[p91IsDraft]
           ,[p91Date]
           ,[p91DateBilled]
           ,[p91DateMaturity]
           ,[p91DateSupply]
           ,[p91DateExchange]
           ,[p91ExchangeRate]
           ,[p91Datep31_From]
           ,[p91Datep31_Until]
           ,[p91Amount_WithoutVat]
           ,[p91Amount_Vat]
           ,[p91Amount_Billed]
           ,[p91Amount_WithVat]
           ,[p91Amount_Debt]
           ,[p91RoundFitAmount]
           ,[p91Text1]
           ,[p91Text2]
           ,[p91ProformaAmount]
           ,[p91ProformaBilledAmount]
           ,[p91Amount_WithoutVat_None]
           ,[p91VatRate_Low]
           ,[p91Amount_WithVat_Low]
           ,[p91Amount_WithoutVat_Low]
           ,[p91Amount_Vat_Low]
           ,[p91VatRate_Standard]
           ,[p91Amount_WithVat_Standard]
           ,[p91Amount_WithoutVat_Standard]
           ,[p91Amount_Vat_Standard]
           ,[p91VatRate_Special]
           ,[p91Amount_WithVat_Special]
           ,[p91Amount_WithoutVat_Special]
           ,[p91Amount_Vat_Special]
           ,[p91Amount_TotalDue]
           ,[p91ProformaAmount_VatRate]
           ,[p91ProformaAmount_Vat_Standard]
           ,[p91ProformaAmount_WithoutVat_None]
           ,[p91ProformaAmount_Vat_Low]
           ,[p91ProformaAmount_WithoutVat_Low]
           ,[p91ProformaAmount_WithoutVat_Standard]
           ,[p91DateInsert]
           ,[p91UserInsert]
           ,[p91DateUpdate]
           ,[p91UserUpdate]
           ,[p91ValidFrom]
           ,[p91ValidUntil]
           ,[p91Client]
           ,[p91ClientPerson]
           ,[p91ClientPerson_Salutation]
           ,[p91Client_RegID]
           ,[p91Client_VatID]
           ,[p91ClientAddress1_Street]
           ,[p91ClientAddress1_City]
           ,[p91ClientAddress1_ZIP]
           ,[p91ClientAddress1_Country]
           ,[p91ClientAddress2]
           ,[j27ID_Domestic])
SELECT [p91ID]
           ,[p92ID]
           ,[p28ID]
           ,[j27ID]
           ,[j19ID]
           ,[j02ID_Owner]
           ,[p41ID_First]
           ,[p91ID_CreditNoteBind]
           ,[b02ID]
           ,[j17ID]
           ,[o38ID_Primary]
           ,[o38ID_Delivery]
           ,[x15ID]
           ,[p98ID]
           ,[j02ID_ContactPerson]
           ,[p63ID]
           ,[p80ID]
           ,[p91FixedVatRate]
           ,[p91Code]
           ,[p91IsDraft]
           ,[p91Date]
           ,[p91DateBilled]
           ,[p91DateMaturity]
           ,[p91DateSupply]
           ,[p91DateExchange]
           ,[p91ExchangeRate]
           ,[p91Datep31_From]
           ,[p91Datep31_Until]
           ,[p91Amount_WithoutVat]
           ,[p91Amount_Vat]
           ,[p91Amount_Billed]
           ,[p91Amount_WithVat]
           ,[p91Amount_Debt]
           ,[p91RoundFitAmount]
           ,[p91Text1]
           ,[p91Text2]
           ,[p91ProformaAmount]
           ,[p91ProformaBilledAmount]
           ,[p91Amount_WithoutVat_None]
           ,[p91VatRate_Low]
           ,[p91Amount_WithVat_Low]
           ,[p91Amount_WithoutVat_Low]
           ,[p91Amount_Vat_Low]
           ,[p91VatRate_Standard]
           ,[p91Amount_WithVat_Standard]
           ,[p91Amount_WithoutVat_Standard]
           ,[p91Amount_Vat_Standard]
           ,[p91VatRate_Special]
           ,[p91Amount_WithVat_Special]
           ,[p91Amount_WithoutVat_Special]
           ,[p91Amount_Vat_Special]
           ,[p91Amount_TotalDue]
           ,[p91ProformaAmount_VatRate]
           ,[p91ProformaAmount_Vat_Standard]
           ,[p91ProformaAmount_WithoutVat_None]
           ,[p91ProformaAmount_Vat_Low]
           ,[p91ProformaAmount_WithoutVat_Low]
           ,[p91ProformaAmount_WithoutVat_Standard]
           ,[p91DateInsert]
           ,[p91UserInsert]
           ,[p91DateUpdate]
           ,[p91UserUpdate]
           ,[p91ValidFrom]
           ,[p91ValidUntil]
           ,[p91Client]
           ,[p91ClientPerson]
           ,[p91ClientPerson_Salutation]
           ,[p91Client_RegID]
           ,[p91Client_VatID]
           ,[p91ClientAddress1_Street]
           ,[p91ClientAddress1_City]
           ,[p91ClientAddress1_ZIP]
           ,[p91ClientAddress1_Country]
           ,[p91ClientAddress2]
           ,[j27ID_Domestic]
FROM
p91Invoice WHERE p91ID=@p91id

declare @p31id int,@rowid int,@p31id_changed int

if not exists(select RowID FROM p31Worksheet_Log WHERE p91ID=@p91id)
 begin
	exec dbo.p31_append_log_p91 @p91id

	return
 end

DECLARE curP31 CURSOR FOR 
SELECT p31ID,max(RowID) from p31Worksheet_Log WHERE p91ID=@p91id GROUP BY p31ID

OPEN curP31
FETCH NEXT FROM curP31 
INTO @p31id,@rowid
WHILE @@FETCH_STATUS = 0
BEGIN
  set @p31id_changed=null

  select @p31id_changed=a.p31ID
  FROM
  p31Worksheet a INNER JOIN (select * from p31Worksheet_Log WHERE RowID=@rowid) b ON a.p31ID=b.p31ID
  WHERE a.p31ID=@p31id
  AND (a.j27ID_Billing_Invoiced<>b.j27ID_Billing_Invoiced OR a.j27ID_Billing_Invoiced_Domestic<>b.j27ID_Billing_Invoiced_Domestic OR a.p31Text<>b.p31Text OR a.p70ID<>b.p70ID OR a.p31Hours_Invoiced<>b.p31Hours_Invoiced OR isnull(a.p31Amount_WithoutVat_Invoiced,0)<>isnull(b.p31Amount_WithoutVat_Invoiced,0) or a.p31Value_FixPrice<>b.p31Value_FixPrice or a.p31VatRate_Invoiced<>b.p31VatRate_Invoiced)

  if @p31id_changed is not null
   exec dbo.p31_append_log @p31id


  FETCH NEXT FROM curP31 
  INTO @p31id,@rowid
END
CLOSE curP31
DEALLOCATE curP31

GO

----------P---------------p91_calc_overhead-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_calc_overhead') and type = 'P')
 drop procedure p91_calc_overhead
GO




CREATE  PROCEDURE [dbo].[p91_calc_overhead]
@p91id int
,@p63id int
AS

  ---vypočítá částku režijní přirážky k faktuře a založí k tomu úkon
  ---je vnořené do procedury p91_recalc_amount ...nefunguje samostatně

if @p63id is null
 begin	---kontrola, zda nevisí již vygenerovaná přirážka ve faktuře ->v tom případě přirážkový úkon smazat
   if not exists(select p63ID FROM p63Overhead)
	 RETURN

    if exists(select p31ID FROM p31Worksheet WHERE p91ID=@p91id AND p31UserInsert='robot' AND p32ID IN (SELECT p32ID FROM p63Overhead))
	 DELETE FROM p31Worksheet WHERE p91ID=@p91id AND p31UserInsert='robot' AND p32ID IN (SELECT p32ID FROM p63Overhead)

	RETURN
 end

declare @j27id int,@datSupply datetime,@p41id_first int,@j02id int,@j17id int,@x15id int,@vatrate float,@p92invoicetype int

select @j27id=a.j27id,@datSupply=a.p91DateSupply,@p41id_first=a.p41ID_First,@j02id=a.j02ID_Owner,@x15id=a.x15ID,@j17id=a.j17ID,@p92invoicetype=b.p92InvoiceType
from p91invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
where a.p91id=@p91id

if @p92invoicetype=2
 RETURN	---dobropis je vždy bez přirážky

declare @overhead float,@sum float,@p32id int,@p63IsIncludeTime bit,@p63IsIncludeExpense bit,@p63IsIncludeFees bit,@p63PercentRate float,@p31text nvarchar(1000)

set @overhead=0

select @p32id=a.p32ID,@p63IsIncludeTime=a.p63IsIncludeTime,@p63IsIncludeExpense=a.p63IsIncludeExpense,@p63IsIncludeFees=a.p63IsIncludeFees,@p63PercentRate=a.p63PercentRate
FROM p63Overhead a INNER JOIN p32Activity b ON a.p32ID=b.p32ID WHERE a.p63ID=@p63id

select @p31text=dbo.p32_get_invoice_worksheet_text(@p91id,@p32id)

if @x15id is null
 select @x15id=x15ID FROM p32Activity where p32ID=@p32id

select @vatrate=dbo.p91_get_vatrate(@x15id,@j27id,@j17id,@datSupply)


if @p63IsIncludeTime=1
   select @sum=sum(p31Amount_WithoutVat_Invoiced) FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID WHERE a.p91ID=@p91id AND c.p33ID=1

set @overhead=isnull(@sum,0)*@p63PercentRate/100
set @sum=0

if @p63IsIncludeExpense=1
   select @sum=sum(p31Amount_WithoutVat_Invoiced) FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID WHERE a.p91ID=@p91id AND c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=1 AND a.p32ID<>@p32id
  
set @overhead=@overhead+(isnull(@sum,0)*@p63PercentRate/100)
  set @sum=0

if @p63IsIncludeFees=1
   select @sum=sum(p31Amount_WithoutVat_Invoiced) FROM p31Worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID INNER JOIN p34ActivityGroup c ON b.p34ID=c.p34ID WHERE a.p91ID=@p91id AND c.p33ID IN (2,5) AND c.p34IncomeStatementFlag=2 AND a.p32ID<>@p32id

set @overhead=@overhead+(isnull(@sum,0)*@p63PercentRate/100)





if not exists(select p31ID FROM p31Worksheet WHERE p91ID=@p91id AND p32ID=@p32id AND p31UserInsert='robot')
   begin
    declare @c11id int
	select top 1 @c11id=c11id from c11statperiod where c11level=5 and c11datefrom=@datSupply  

    insert into p31worksheet(p91ID,j02ID,p41ID,p32ID,j27ID_Billing_Orig,p31Date,j02ID_Owner,p31UserInsert,p31UserUpdate,p31DateInsert,p31DateUpdate,p31HoursEntryFlag,p71ID,p70ID,p72ID_AfterApprove,c11ID)
    values(@p91id,@j02id,@p41id_first,@p32id,@j27id,@datSupply,@j02id,'robot','robot',getdate(),getdate(),0,1,4,4,@c11id)
   end

declare @amount_vat float,@amount_withvat float

set @amount_vat=@overhead*@vatrate/100
set @amount_withvat=@overhead+@amount_vat

UPDATE p31Worksheet set p31VatRate_Orig=@vatrate,p31Amount_WithoutVat_Orig=@overhead,p31Amount_Vat_Orig=@amount_vat,p31Amount_WithVat_Orig=@amount_withvat,p31Value_Orig=@overhead
,p32ID=@p32id,p41ID=@p41id_first,j27ID_Billing_Orig=@j27id,p31Date=@datSupply,p31DateUpdate=getdate(),j27ID_Billing_Invoiced=@j27id
,p31Text=@p31text,j02ID_ApprovedBy=@j02id,p31Approved_When=getdate()
,p31VatRate_Approved=@vatrate,p31Amount_WithoutVat_Approved=@overhead,p31Amount_Vat_Approved=@amount_vat,p31Amount_WithVat_Approved=@amount_withvat,p31Value_Approved_Billing=@overhead
,p31VatRate_Invoiced=@vatrate,p31Amount_WithoutVat_Invoiced=@overhead,p31Amount_Vat_Invoiced=@amount_vat,p31Amount_WithVat_Invoiced=@amount_withvat,p31Value_Invoiced=@overhead
WHERE p91ID=@p91id AND p32ID=@p32id AND p31UserInsert='robot'

   




GO

----------P---------------p91_calc_p81-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_calc_p81') and type = 'P')
 drop procedure p91_calc_p81
GO




CREATE  PROCEDURE [dbo].[p91_calc_p81]
@p91id int
,@p80id int
AS

  ---Generuje souhrnný cenový rozpis faktury do tabulky p81InvoiceAmount
  ---je vnořené do procedury p91_recalc_amount ...nefunguje samostatně


if exists(select p81ID FROM p81InvoiceAmount WHERE p91ID=@p91id)
 DELETE FROM p81InvoiceAmount WHERE p91ID=@p91id

 ---explicitní rozpis podle p80InvoiceAmountStructure
 declare @time bit,@fee bit,@expense bit

 select @time=p80IsTimeSeparate,@expense=p80IsExpenseSeparate,@fee=p80IsFeeSeparate FROM p80InvoiceAmountStructure WHERE p80ID=@p80id

if @p80id is null OR (@time=0 and @expense=0 and @fee=0)
 begin	---bez explicitní struktury rozpisu ceny (podle fakturačních oddílů)
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,p31VatRate_Invoiced,isnull(round(sum(a.p31Amount_WithoutVat_Invoiced),2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
	WHERE a.p91ID=@p91id
	GROUP BY p32.p95ID,a.p31VatRate_Invoiced
 end

if @p80id is not null AND @expense=0
 begin
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,p31VatRate_Invoiced,isnull(round(sum(a.p31Amount_WithoutVat_Invoiced),2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
	WHERE a.p91ID=@p91id AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1
	GROUP BY p32.p95ID,a.p31VatRate_Invoiced
 end

if @p80id is not null AND @expense=1
 begin	---výdaje 1:1
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p31ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,a.p31ID,p31VatRate_Invoiced,isnull(round(a.p31Amount_WithoutVat_Invoiced,2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
	WHERE a.p91ID=@p91id AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1	
 end

if @p80id is not null AND @fee=0
 begin
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,p31VatRate_Invoiced,isnull(round(sum(a.p31Amount_WithoutVat_Invoiced),2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
	WHERE a.p91ID=@p91id AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2
	GROUP BY p32.p95ID,a.p31VatRate_Invoiced
 end

if @p80id is not null AND @fee=1
 begin	---pevné odměny 1:1
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p31ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,a.p31ID,p31VatRate_Invoiced,isnull(round(a.p31Amount_WithoutVat_Invoiced,2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
	WHERE a.p91ID=@p91id AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2	
 end

if @p80id is not null AND @time=0
 begin
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,p31VatRate_Invoiced,isnull(round(sum(a.p31Amount_WithoutVat_Invoiced),2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
	WHERE a.p91ID=@p91id AND p34.p33ID IN (1,3)
	GROUP BY p32.p95ID,a.p31VatRate_Invoiced
 end

if @p80id is not null AND @time=1
 begin	---časové úkony 1:1
	INSERT INTO p81InvoiceAmount(p91ID,p95ID,p31ID,p81VatRate,p81Amount_WithoutVat)
	SELECT @p91id,p32.p95ID,a.p31ID,p31VatRate_Invoiced,isnull(round(a.p31Amount_WithoutVat_Invoiced,2),0)
	FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
	WHERE a.p91ID=@p91id AND p34.p33ID IN (1,3)
 end


update p81InvoiceAmount set p81Amount_Vat=round(p81Amount_WithoutVat*p81VatRate/100,2) WHERE p91ID=@p91id

update p81InvoiceAmount set p81Amount_WithVat=p81Amount_WithoutVat+p81Amount_Vat WHERE p91ID=@p91id



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

declare @code varchar(50),@x38id int,@isdraft bit

select @x38id=p92.x38ID,@isdraft=a.p91IsDraft
FROM
p91Invoice a INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
WHERE a.p91ID=@p91id

if @isdraft=0
 begin
  set @err_ret='Faktura není v ŕežimu DRAFT.'
  return	--faktura není v draftu
 end
 
exec dbo.x38_get_freecode_proc @x38id,391,@p91id,0,1,@code OUTPUT

if @code=''
 begin
  set @err_ret='Systém nedokázal složit odpovídající kód podle nastavení číselné řady. Záznam zůstává v režimu DRAFT.'
  return
 end

if @code<>''
 UPDATE p91Invoice SET p91Code=@code,p91IsDraft=0 WHERE p91ID=@p91id 

exec dbo.p91_append_log @p91id

  





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
,@j02id_contactperson int
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
  set @err_ret='Chybí klient (odběratel) faktury!'

if @err_ret<>''
 return

declare @login nvarchar(50),@j02id_owner int
set @login=dbo.j03_getlogin(@j03id_sys)
select @j02id_owner=j02ID FROM j03User WHERE j03ID=@j03id_sys

if not exists(select a.p85ID from p85TempBox a INNER JOIN p31Worksheet b ON a.p85DataPID=b.p31ID where b.p91ID IS NULL AND a.p85guid=@guid and a.p85Prefix='p31' and a.p85IsDeleted=0)
 begin
  set @err_ret='Na vstupu chybí fronta worksheet položek faktury!'
  return

 end


declare @j27id int,@j19id int,@x15id int,@j17id int,@p98id int,@p91vatrate_standard float,@p91vatrate_low float,@p91vatrate_special float,@p80id int


select @j27id=j27id,@j19id=j19id,@x15id=x15id,@j17id=j17ID,@p98id=p98ID,@p80id=p80ID
from p92InvoiceType where p92id=@p92id  

if isnull(@j27id,0)=0
 begin
  if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Invoice')
   select @j27id=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Invoice'
  else
   set @j27id=2
 end

 if isnull(@x15id,0)<>0
  begin	---úvodní otestování existence nastavených DPH sazeb
   if not exists(select p53ID FROM p53VatRate WHERE x15ID=@x15id AND j27ID=@j27id AND isnull(j17ID,0)=isnull(@j17id,0) and GETDATE() between p53ValidFrom AND p53ValidUntil)
    begin
     select @err_ret='Pro fakturační měnu ['+(select j27Code FROM j27Currency WHERE j27ID=@j27id)+'] je třeba v systému nadefinovat DPH sazbu hladiny ['+ x15Name+'].' FROM x15VatRateType WHERE x15ID=@x15id
	 return
	end
  end
 


declare @code_temp varchar(50)
set @code_temp='TEMP'+@guid  

insert into p91invoice(p91code,p91dateinsert,p91userinsert,p91Date,p91DateSupply,p91DateMaturity,j02ID_Owner,p28ID,p92ID,j27ID,j02ID_ContactPerson) values(@code_temp,getdate(),@login,@p91date,@p91datesupply,@p91datematurity,@j02id_owner,@p28id,@p92id,@j27id,@j02id_contactperson)

SELECT @ret_p91id=@@IDENTITY

declare @o38id_primary int,@o38id_delivery int,@p41id_first int,@person nvarchar(100),@salutation nvarchar(150)

if @j02id_contactperson is not null
 select @person=isnull(j02TitleBeforeName+' ','')+j02FirstName+' '+j02LastName+isnull(' '+j02TitleAfterName,''),@salutation=j02Salutation FROM j02Person WHERE j02ID=@j02id_contactperson

select top 1 @p41id_first=p41ID FROM p31Worksheet WHERE p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31' and p85IsDeleted=0)

select top 1 @o38id_primary=o38ID from o37Contact_Address WHERE o36ID=1 AND p28ID=@p28id
   
select top 1 @o38id_delivery=o38ID from o37Contact_Address WHERE o36ID=2 AND p28ID=@p28id

if @o38id_delivery is null
 set @o38id_delivery=@o38id_primary

if CHARINDEX(']',@p91text1)>1
begin
 set @p91text1=dbo.get_parsed_text_with_period(@p91text1,@p91datesupply,0)
 set @p91text1=dbo.get_parsed_text_with_period(@p91text1,@p91datep31_from,1)
 set @p91text1=dbo.get_parsed_text_with_period(@p91text1,@p91datep31_until,2)
end

update p91Invoice set o38ID_Primary=@o38id_primary,@o38id_delivery=o38id_delivery,p41ID_First=@p41id_first,p80ID=@p80id,p98ID=@p98id
where p91ID=@ret_p91id

update a set p91IsDraft=@p91isdraft,j17ID=@j17id,p63ID=b.p63ID
,p91userupdate=@login,p91dateupdate=getdate()
,p91Text1=@p91text1
,p91Datep31_From=@p91datep31_from,p91Datep31_Until=@p91datep31_until,j19id=@j19id
,p91Client=isnull(b.p28CompanyName,b.p28Name),p91Client_RegID=b.p28RegID,p91Client_VatID=b.p28VatID
,p91ClientAddress1_City=left(o38prim.o38City,100),p91ClientAddress1_Street=left(o38prim.o38Street,150),p91ClientAddress1_ZIP=left(o38prim.o38ZIP,20),p91ClientAddress1_Country=o38prim.o38Country
,p91ClientAddress2=left(isnull(o38del.o38Street+char(13)+char(10),'')+isnull(o38del.o38City+char(13)+char(10),'')+isnull(o38del.o38ZIP+char(13)+char(10),'')+isnull(o38del.o38Country,''),255)
,p91ClientPerson=left(@person,100),p91ClientPerson_Salutation=left(@salutation,150)
FROM p91invoice a LEFT OUTER JOIN p28Contact b ON a.p28ID=b.p28ID
LEFT OUTER JOIN o38Address o38prim ON a.o38ID_Primary=o38prim.o38ID
LEFT OUTER JOIN o38Address o38del ON a.o38ID_Delivery=o38del.o38ID
where a.p91id=@ret_p91id
 

declare @p91text2 nvarchar(1000)

select @p91text2=p41InvoiceDefaultText2 from p41Project WHERE p41ID=@p41id_first

if @p91text2 is null
 select @p91text2=p28InvoiceDefaultText2 from p28Contact where p28ID=@p28id

if @p91text2 is null
 select @p91text2=p92InvoiceDefaultText2 from p92InvoiceType where p92ID=@p92id

if @p91text2 is not null
 update p91Invoice set p91Text2=@p91text2 where p91ID=@ret_p91id

update p31worksheet set p91ID=@ret_p91id,p70id=p72ID_AfterApprove
WHERE p71id=1 and isnull(p72ID_AfterApprove,0)<>0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31' and p85IsDeleted=0)

update p31worksheet set p91ID=@ret_p91id,p70id=(case when isnull(p31amount_withoutvat_approved,0)<>0 then 4 else 2 end)
WHERE p71id=1 and isnull(p72ID_AfterApprove,0)=0 and p31id in (select p85datapid from p85TempBox where p85guid=@guid and p85Prefix='p31' and p85IsDeleted=0)

update p31worksheet set p31Minutes_Invoiced=p31Minutes_Approved_Billing,p31Hours_Invoiced=p31Hours_Approved_Billing,p31HHMM_Invoiced=p31HHMM_Approved_Billing
,p31Value_Invoiced=p31Value_Approved_Billing
,p31Amount_WithoutVat_Invoiced=p31Amount_WithoutVat_Approved,p31Amount_WithVat_Invoiced=p31Amount_WithVat_Approved
,p31Amount_Vat_Invoiced=p31Amount_Vat_Approved,p31VatRate_Invoiced=p31VatRate_Approved
,p31Rate_Billing_Invoiced=p31Rate_Billing_Approved
where p91ID=@ret_p91id


select @p91vatrate_standard=dbo.p91_get_vatrate(3,@j27id,@j17id,@p91datesupply)	---DPH sazba, která bude aplikovaná pro všechny časové úkony
select @p91vatrate_low=dbo.p91_get_vatrate(2,@j27id,@j17id,@p91datesupply)
select @p91vatrate_special=dbo.p91_get_vatrate(4,@j27id,@j17id,@p91datesupply)

if @p91vatrate_standard is not null and isnull(@x15id,0)=0
 begin	---narovnat standardní fakturační sazbu DPH
  UPDATE p31Worksheet SET p31VatRate_Invoiced=@p91vatrate_standard
  ,p31Amount_Vat_Invoiced=p31Amount_WithoutVat_Invoiced*@p91vatrate_standard/100
  ,p31Amount_WithVat_Invoiced=p31Amount_WithoutVat_Invoiced*@p91vatrate_standard/100+p31Amount_WithoutVat_Invoiced
  WHERE p91ID=@ret_p91id AND p31VatRate_Invoiced<>@p91vatrate_standard
  AND p32ID IN (SELECT a.p32ID FROM p32Activity a INNER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE b.p33ID IN (1,2,3) AND ISNULL(a.x15ID,3)=3)

 end

if @p91vatrate_low is not null and isnull(@x15id,0)=0
 begin	---narovnat sníženou fakturační sazbu DPH
  UPDATE p31Worksheet SET p31VatRate_Invoiced=@p91vatrate_low
  ,p31Amount_Vat_Invoiced=p31Amount_WithoutVat_Invoiced*@p91vatrate_low/100
  ,p31Amount_WithVat_Invoiced=p31Amount_WithoutVat_Invoiced*@p91vatrate_low/100+p31Amount_WithoutVat_Invoiced
  WHERE p91ID=@ret_p91id AND p31VatRate_Invoiced<>@p91vatrate_low
  AND p32ID IN (SELECT a.p32ID FROM p32Activity a INNER JOIN p34ActivityGroup b ON a.p34ID=b.p34ID WHERE b.p33ID IN (2) AND a.x15ID=2)

 end

if isnull(@x15id,0)=0
 begin
  UPDATE p31Worksheet SET p31VatRate_Invoiced=@p91vatrate_standard
  ,p31Amount_Vat_Invoiced=p31Amount_WithoutVat_Invoiced*@p91vatrate_standard/100
  ,p31Amount_WithVat_Invoiced=p31Amount_WithoutVat_Invoiced*@p91vatrate_standard/100+p31Amount_WithoutVat_Invoiced
  WHERE p91ID=@ret_p91id AND p31VatRate_Invoiced<>@p91vatrate_standard and p31VatRate_Invoiced<>@p91vatrate_low and p31VatRate_Invoiced<>@p91vatrate_special and p31VatRate_Invoiced>0
 end


exec p91_recalc_amount @ret_p91id

if isnull(@x15id,0)<>0
 begin	---faktura má být kompletně převedena do jednotné DPH
  declare @explicit_vatrate float,@j18id int

  
  set @explicit_vatrate=dbo.p91_get_vatrate(@x15id,@j27id,@j17id,@p91datesupply)
  
  
  exec p91_change_vat @ret_p91id,@j03id_sys,@x15id,@explicit_vatrate,null
  
 end



 exec [p91_update_code] @ret_p91id,@j03id_sys

 exec [p91_aftersave] @ret_p91id,@j03id_sys,0





























GO

----------P---------------p91_create_creditnote-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_create_creditnote') and type = 'P')
 drop procedure p91_create_creditnote
GO



CREATE procedure [dbo].[p91_create_creditnote]
@j03id_sys int
,@p91id_bind int
,@p92id_creditnote int
,@err_ret varchar(1000) OUTPUT
,@ret_p91id int OUTPUT

AS

set @err_ret=''
set @j03id_sys=isnull(@j03id_sys,0)
set @p91id_bind=isnull(@p91id_bind,0)
set @p92id_creditnote=isnull(@p92id_creditnote,0)


if @j03id_sys=0
 begin
  set @err_ret='@j03id_sys is missing!'
  return
 end

declare @login nvarchar(50),@j02id_sys int,@guid varchar(50)
select @login=j03login,@j02id_sys=j02ID from j03user where j03id=@j03id_sys


if @p92id_creditnote=0
  set @err_ret='@p92id_creditnote is missing!'


if @err_ret<>''
  return

declare @x38id int,@p28id int,@j27id int,@p32id_creditnote int,@p98id int,@p80id int

if not exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'p32ID_CreditNote')
 begin
  set @err_ret='V globálním nastavení chybí hodnota parametru [p32ID_CreditNote].'
  return
 end

select @p32id_creditnote=convert(int,x35value) from x35GlobalParam WHERE x35Key like 'p32ID_CreditNote'

if not exists(select p32ID FROM p32Activity WHERE p32ID=@p32id_creditnote)
 set @err_ret='Pro parametr [p32ID_CreditNote] v systému neexistuje záznam aktivity!'

select @x38id=x38id,@p98id=p98ID,@p80id=p80ID from p92InvoiceType where p92id=@p92id_creditnote

select @p28id=p28id,@j27id=j27id from p91invoice where p91id=@p91id_bind

if @x38id is null
  set @err_ret='@x38id is not found!'


if @err_ret<>''
  return

insert into p91invoice(
p92id,p91dateinsert,p91userinsert,p91Date,p91DateSupply,p91DateMaturity,j02ID_Owner,p28id,j27id,p91dateupdate,p91userupdate
,o38ID_Primary,o38ID_Delivery,x15ID,p91fixedvatrate,j02ID_ContactPerson
,j19id,p41ID_First,j17ID
,p91ID_CreditNoteBind,p91Datep31_From,p91Datep31_Until,p98ID,p80ID
)
select @p92id_creditnote,getdate(),@login,getdate(),getdate(),getdate(),@j02id_sys,p28id,j27id,getdate(),@login
,o38ID_Primary,o38ID_Delivery,x15ID,p91fixedvatrate,j02ID_ContactPerson
,j19id,p41ID_First,j17ID
,@p91id_bind,p91Datep31_From,p91Datep31_Until,@p98id,@p80id
FROM p91invoice
where p91id=@p91id_bind

SELECT @ret_p91id=@@IDENTITY

update a set p91Client=isnull(b.p28CompanyName,b.p28Name),p91Client_RegID=b.p28RegID,p91Client_VatID=b.p28VatID
,p91ClientAddress1_City=o38prim.o38City,p91ClientAddress1_Street=o38prim.o38Street,p91ClientAddress1_ZIP=o38prim.o38ZIP,p91ClientAddress1_Country=o38prim.o38Country
,p91ClientAddress2=isnull(o38del.o38Street+char(13)+char(10),'')+isnull(o38del.o38City+char(13)+char(10),'')+isnull(o38del.o38ZIP+char(13)+char(10),'')+isnull(o38del.o38Country,'')
FROM p91invoice a LEFT OUTER JOIN p28Contact b ON a.p28ID=b.p28ID
LEFT OUTER JOIN o38Address o38prim ON a.o38ID_Primary=o38prim.o38ID
LEFT OUTER JOIN o38Address o38del ON a.o38ID_Delivery=o38del.o38ID
where a.p91id=@ret_p91id



exec p91_update_code @ret_p91id,@j03id_sys

---worksheet
declare @c11id int,@p31date datetime,@p31id int,@p31text nvarchar(300),@p31value_fixprice float
set @p31text='Dobropisovaná částka'
select top 1 @c11id=c11id,@p31date=c11datefrom from c11statperiod where c11level=5 and c11datefrom<=getdate() order by c11id desc

declare @amount_withoutvat decimal(18,2),@amount_withvat decimal(18,2),@vatrate decimal(18,2),@amount_vat decimal(18,2),@p41id int
DECLARE curP31CN CURSOR FOR 
select p41id,p31VatRate_Invoiced,-1*sum(p31Amount_WithoutVat_Invoiced),-1*sum(p31Amount_WithVat_Invoiced),-1*sum(p31Amount_Vat_Invoiced)
from p31worksheet where p91id=@p91id_bind
GROUP BY p41id,p31VatRate_Invoiced
	
OPEN curP31CN
FETCH NEXT FROM curP31CN 
INTO @p41id,@vatrate,@amount_withoutvat,@amount_withvat,@amount_vat
WHILE @@FETCH_STATUS = 0
BEGIN
  insert into p31worksheet(j02ID,p41ID,p32ID,j27ID_Billing_Orig,c11id,p31Date,p31Amount_WithoutVat_Orig,p31Amount_WithVat_Orig,p31VatRate_Orig,j02ID_Owner,p31UserInsert,p31UserUpdate,p31DateInsert,p31DateUpdate,p31value_orig,p31text,p31HoursEntryFlag)
  values(@j02id_sys,@p41id,@p32id_creditnote,@j27id,@c11id,@p31date,@amount_withoutvat,@amount_withvat,@vatrate,@j02id_sys,@login,@login,getdate(),getdate(),@amount_withoutvat,@p31text,0)

  SELECT @p31id=@@IDENTITY

  set @guid=convert(varchar(10),@ret_p91id)+'-'+convert(varchar(10),@p31id)+'-'+convert(varchar(50),getdate())
  insert into p85TempBox(p85GUID,p85DataPID,p85Prefix) values(@guid,@p31id,'p31')

  exec p31_save_approving @p31id,@j03id_sys,1,4,null,@amount_withoutvat,@amount_withoutvat,null,null,@p31text,@vatrate,@p31date,0,null,@amount_withoutvat,@err_ret OUTPUT

  print 'p31_save_approving, p31id: '+convert(varchar(10),@p31id)+', error: '+@err_ret
  

  exec p31_append_invoice @ret_p91id,@guid,@j03id_sys,@err_ret OUTPUT

  print 'p31_append_invoice, p31id: '+convert(varchar(10),@p31id)+', error: '+@err_ret



  FETCH NEXT FROM curP31CN 
  INTO @p41id,@vatrate,@amount_withoutvat,@amount_withvat,@amount_vat
END
CLOSE curP31CN
DEALLOCATE curP31CN


exec dbo.p91_recalc_amount @ret_p91id



GO

----------P---------------p91_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_delete') and type = 'P')
 drop procedure p91_delete
GO



CREATE   procedure [dbo].[p91_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p91id
,@guid varchar(50)			---seznam úkonů ve faktuře
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu vystavené faktury z tabulky p91Invoice
declare @ref_pid int



if @guid is null
 set @err_ret='Na vstupu chybí guid.'


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


---if @p92invoicetype=2 AND @p32id_overhead is not null
--- begin
---  if exists(select p31ID FROM p31Worksheet WHERE p91ID=@pid AND p32id=@p32id_overhead)
---    delete FROM p31worksheet WHERE p91id=@pid AND p32id=@p32id_overhead
--- end

update p31Worksheet set p91ID=null,p70ID=null
,j27ID_Billing_Invoiced=null,j27ID_Billing_Invoiced_Domestic=null,p31Value_Invoiced=null,p31Rate_Billing_Invoiced=null
,p31Amount_WithoutVat_Invoiced=null,p31Amount_WithVat_Invoiced=null,p31Amount_Vat_Invoiced=null,p31VatRate_Invoiced=null
,p31Amount_WithoutVat_Invoiced_Domestic=null,p31Amount_WithVat_Invoiced_Domestic=null,p31Amount_Vat_Invoiced_Domestic=null
,p31Minutes_Invoiced=null,p31HHMM_Invoiced=null,p31Hours_Invoiced=null
,p31ExchangeRate_Invoice=null,p31ExchangeRate_InvoiceManual=null
,p31IsInvoiceManual=0,j02ID_InvoiceManual=null
where p91ID=@pid

---přesunout úkony do rozpracovnaosti
update p31Worksheet set p71ID=null,p72ID_AfterApprove=null,p31Approved_When=null,p31Value_Approved_Billing=null,p31Value_Approved_Internal=null
,p31Minutes_Approved_Billing=null,p31Hours_Approved_Billing=null,p31Hours_Approved_Internal=null,p31HHMM_Approved_Billing=null,p31HHMM_Approved_Internal=null
,p31Rate_Billing_Approved=null,p31Rate_Internal_Approved=null
,p31Amount_WithoutVat_Approved=null,p31Amount_WithVat_Approved=null,p31Amount_Vat_Approved=null,p31VatRate_Approved=null,p31Amount_Internal_Approved=null
,j02ID_ApprovedBy=null
FROM p31Worksheet WHERE p31ID IN (SELECT p85DataPID FROM p85TempBox WHERE p85GUID=@guid and p85Prefix='p31' AND p85OtherKey1=2)

---přesunout úkony do archivu
update p31Worksheet set p31ValidUntil=getdate() WHERE p31ID IN (SELECT p85DataPID FROM p85TempBox WHERE p85GUID=@guid and p85Prefix='p31' AND p85OtherKey1=3)

---nenávratně odstranit úkony
if exists(select p85ID FROM p85TempBox WHERE p85GUID=@guid and p85Prefix='p31' AND p85OtherKey1=4)
begin
 DELETE FROM p31WorkSheet_FreeField WHERE p31ID IN (SELECT p85DataPID FROM p85TempBox WHERE p85GUID=@guid and p85Prefix='p31' AND p85OtherKey1=4)

 delete from p31Worksheet WHERE p31ID IN (SELECT p85DataPID FROM p85TempBox WHERE p85GUID=@guid and p85Prefix='p31' AND p85OtherKey1=4)
end
 

if exists(select p94ID FROM p94Invoice_Payment where p91ID=@pid)
  delete from p94Invoice_Payment where p91id=@pid

if exists(select p81ID FROM p81InvoiceAmount WHERE p91ID=@pid)
 delete from p81InvoiceAmount WHERE p91ID=@pid

if exists(select p96ID FROM p96Invoice_ExchangeRate where p91ID=@pid)
  delete from p96Invoice_ExchangeRate where p91id=@pid

if exists(select p91ID FROM p91Invoice_FreeField WHERE p91ID=@pid)
  delete from p91Invoice_FreeField where p91id=@pid

if exists(select o52ID FROM o52TagBinding WHERE x29ID=391 AND o52RecordPID=@pid)
 DELETE FROM o52TagBinding WHERE x29ID=391 AND o52RecordPID=@pid

if exists(select x19ID FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=391))
 DELETE FROM x19EntityCategory_Binding WHERE x19RecordPID=@pid AND x20ID IN (select x20ID FROM x20EntiyToCategory WHERE x29ID=391)

if exists(select p99ID FROM p99Invoice_Proforma WHERE p91ID=@pid)
  delete from p99Invoice_Proforma where p91id=@pid




if exists(SELECT o22ID FROM o22Milestone WHERE p91ID=@pid)
  DELETE FROM o22Milestone WHERE p91ID=@pid

if exists(select a.x69ID FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=391)
  DELETE a FROM x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE a.x69RecordPID=@pid AND b.x29ID=391





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

----------P---------------p91_fpr_recalc_all_invoices-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_fpr_recalc_all_invoices') and type = 'P')
 drop procedure p91_fpr_recalc_all_invoices
GO


CREATE procedure [dbo].[p91_fpr_recalc_all_invoices]
@p51id int
,@d1 datetime
,@d2 datetime
AS
declare @p91id int

if isnull(@p51id,0)=0
 begin
  select @p51id=p51ID FROM p50OfficePriceList WHERE p50RatesFlag=2 AND getdate() BETWEEN p50ValidFrom AND p50ValidUntil

  if @p51id is null
   return
 end


DECLARE curRPR CURSOR FOR 
SELECT p91id from p91invoice WHERE p91DateSupply BETWEEN @d1 AND @d2

OPEN curRPR
FETCH NEXT FROM curRPR 
INTO @p91id
WHILE @@FETCH_STATUS = 0
BEGIN
  exec [p91_fpr_recalc_invoice] @p51id,@p91id

  FETCH NEXT FROM curRPR 
  INTO @p91id
END
CLOSE curRPR
DEALLOCATE curRPR

GO

----------P---------------p91_fpr_recalc_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_fpr_recalc_invoice') and type = 'P')
 drop procedure p91_fpr_recalc_invoice
GO


CREATE procedure [dbo].[p91_fpr_recalc_invoice]
@p51id int	----srovnávací ceník pro výpočet efektivní sazby
,@p91id INT

AS

if isnull(@p51id,0)=0 or isnull(@p91id,0)=0
 return

declare @vynosy float
declare @hodiny float	---hodiny se statusem [Zahrnout do paušálu]
declare @body float		---honorář z paušálních hodin podle srovnávacího ceníku
declare @vynosy_fixedcurrency float	---pevná neboli paušální odměna
---p31AKDS_FPR_PODIL = procentuální podíl 

select @vynosy=sum(p31Amount_WithoutVat_Invoiced)
,@vynosy_fixedcurrency=sum(case when a.j27ID_Billing_Invoiced=2 THEN p31Amount_WithoutVat_Invoiced else p31Amount_WithoutVat_Invoiced_Domestic END)
from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id
where a.p91id=@p91id and a.p70ID=4 AND c.p34IncomeStatementFlag=2 AND c.p33id IN (2,5)

select @hodiny=isnull(sum(p31Hours_Orig),0)
FROM p31worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID
where p91id=@p91id AND p70ID=6 and p32IsBillable=1

if exists(select p52ID FROM p52PriceList_Item WHERE p51ID=@p51id AND j02ID IS NOT NULL)
 begin
  select @body=SUM(p52rate*p31Hours_Orig)
  FROM 
  p52PriceList_Item a 
  INNER JOIN j02Person j02 on a.j02ID=j02.j02ID
  INNER JOIN p31WorkSheet p31 ON j02.j02ID=p31.j02ID
  INNER JOIN p32Activity p32 ON p31.p32ID=p32.p32ID
  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
  WHERE a.p51id=@p51id AND p31.p91ID=@p91id and p31.p70ID=6 AND p34.p33ID=1
  
  update p31WorkSheet set p31AKDS_FPR_BODY=p31Hours_Orig*p52rate
  FROM
  p52PriceList_Item a
  INNER JOIN j02Person j02 on a.j02ID=j02.j02ID
  INNER JOIN p31WorkSheet p31 ON j02.j02ID=p31.j02ID
  INNER JOIN p32Activity p32 ON p31.p32ID=p32.p32ID
  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
  WHERE a.p51id=@p51id AND p31.p91ID=@p91id AND p31.p70ID=6 AND p34.p33ID=1
end

if exists(select p52ID FROM p52PriceList_Item WHERE p51ID=@p51id AND j07ID IS NOT NULL)
 begin
 select @body=SUM(p52rate*p31Hours_Orig)
 FROM 
 p52PriceList_Item a 
 INNER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID
 INNER JOIN j02Person j02 on j07.j07ID=j02.j07ID
 INNER JOIN p31WorkSheet p31 ON j02.j02ID=p31.j02ID
 INNER JOIN p32Activity p32 ON p31.p32ID=p32.p32ID
 INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
 WHERE a.p51id=@p51id AND p31.p91ID=@p91id and p31.p70ID=6 AND p34.p33ID=1
 
 update p31WorkSheet set p31AKDS_FPR_BODY=p31Hours_Orig*p52rate
  FROM
  p52PriceList_Item a
  INNER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID
 INNER JOIN j02Person j02 on j07.j07ID=j02.j07ID
  INNER JOIN p31WorkSheet p31 ON j02.j02ID=p31.j02ID
  INNER JOIN p32Activity p32 ON p31.p32ID=p32.p32ID
  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
  WHERE a.p51id=@p51id AND p31.p91ID=@p91id AND p31.p70ID=6 AND p34.p33ID=1
 end

update a set p31AKDS_FPR_PODIL=p31AKDS_FPR_BODY/@body
FROM p31WorkSheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p91ID=@p91id AND a.p70ID=6 AND p34.p33ID=1

update p31WorkSheet set p31AKDS_FPR_OBRAT=p31AKDS_FPR_PODIL*@vynosy
,p31AKDS_FPR_OBRAT_FixedCurrency=p31AKDS_FPR_PODIL*@vynosy_fixedcurrency
WHERE p91ID=@p91id AND p70ID=6

update p31Worksheet set p31AKDS_FPR_OBRAT_FixedCurrency=p31Amount_WithoutVat_Invoiced_Domestic,p31AKDS_FPR_OBRAT=p31Amount_WithVat_Invoiced
WHERE p91ID=@p91id AND p70ID=4 AND p32ID IN (SELECT p32ID FROM p32Activity a INNER JOIN p34ActivityGroup b on a.p34ID=b.p34ID WHERE b.p33ID=1)

update p31Worksheet set p31AKDS_FPR_PODIL=0,p31AKDS_FPR_OBRAT=0,p31AKDS_FPR_OBRAT_FixedCurrency=0
WHERE p91ID=@p91id AND p70ID IN (2,3)	---odpisy

GO

----------P---------------p91_get_cenovy_rozpis-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_get_cenovy_rozpis') and type = 'P')
 drop procedure p91_get_cenovy_rozpis
GO



CREATE   procedure [dbo].[p91_get_cenovy_rozpis]
@pid int,@include_rounding bit,@include_proforma bit,@langindex int

---@pid=p91ID

AS

declare @zaokrouhleni varchar(100),@zalohy varchar(100)
set @zaokrouhleni='Zaokrouhlení'
set @zalohy='Uhrazené zálohy'

set @langindex=isnull(@langindex,0)

if @langindex=1
 begin
  set @zaokrouhleni='Rounded'
  set @zalohy='Advance payment'
 end

if @langindex=2
 begin
  set @zaokrouhleni='Abrundung'
  set @zalohy='Vorauszahlung'
 end


select case when a.p31ID IS NULL THEN case when @langindex=1 then isnull(p95.p95Name_BillingLang1,p95.p95Name)
 when @langindex=2 then isnull(p95.p95Name_BillingLang2,p95.p95Name)
 when @langindex=10 then p95.p95Name+' / '+p95.p95Name_BillingLang1
 when @langindex=20 then p95.p95Name+' / '+p95.p95Name_BillingLang2
  else p95.p95Name end else (case when CHARINDEX('##', p31.p31Text)>0 then LEFT(p31.p31Text,CHARINDEX('##', p31.p31Text)-1) else p31.p31Text end) END as Oddil
,a.p81Amount_WithoutVat as BezDPH
,a.p81VatRate as DPHSazba
,a.p81Amount_Vat as DPH
,a.p81Amount_WithVat as VcDPH
,j27.j27Code as j27Code
,p95.p95Ordinary as Poradi
,a.p81ID as RowPID
from
p81InvoiceAmount a
INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID
LEFT OUTER JOIN p95InvoiceRow p95 ON a.p95ID=p95.p95ID
LEFT OUTER JOIN j27Currency j27 ON p91.j27ID=j27.j27ID
LEFT OUTER JOIN p31Worksheet p31 ON a.p31ID=p31.p31ID
where a.p91ID=@pid and a.p81Amount_WithVat<>0
UNION
SELECT @zaokrouhleni as Oddil
,p91RoundFitAmount as BEZDPH
,0 as DPHSazba
,0 as DPH
,p91RoundFitAmount as VcDPH
,j27.j27Code,1000 as Poradi
,0 as RowPID
FROM p91Invoice a INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID WHERE a.p91ID=@pid AND isnull(p91RoundFitAmount,0)<>0 AND @include_rounding=1
UNION
SELECT @zalohy as Oddil
,-1*(p91ProformaAmount_WithoutVat_None+p91ProformaAmount_WithoutVat_Low+p91ProformaAmount_WithoutVat_Standard) as BEZDPH
,p91ProformaAmount_VatRate as DPHSazba
,-1*(p91ProformaAmount_Vat_Low+p91ProformaAmount_Vat_Standard) as DPH
,-1*p91ProformaAmount as VcDPH
,j27.j27Code,1000 as Poradi
,0 as RowPID
FROM p91Invoice a INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID
WHERE a.p91ID=@pid AND isnull(p91ProformaAmount,0)<>0 AND @include_proforma=1
ORDER BY DPHSazba DESC,Poradi



GO

----------P---------------p91_get_efektivni_sazby_vypis-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_get_efektivni_sazby_vypis') and type = 'P')
 drop procedure p91_get_efektivni_sazby_vypis
GO





CREATE   procedure [dbo].[p91_get_efektivni_sazby_vypis]
@pid int,@p51id int

---@pid=p91ID,@p51id=ceník seniority podílu na paušální odměně

AS

declare @pausaly float,@honorar_cenik float

select @pausaly=sum(case when a.j27ID_Billing_Invoiced=2 THEN p31Amount_WithoutVat_Invoiced else p31Amount_WithoutVat_Invoiced_Domestic END)
from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id
where a.p70ID=4 AND c.p34IncomeStatementFlag=2 AND c.p33id IN (2,5)
and a.p91ID=@pid

select @honorar_cenik=sum(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)
from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id
where a.p70ID=6 AND c.p33id=1
and a.p91ID=@pid

select j02LastName+' '+j02FirstName as Person
,p41Name
,p70Name
,case a.p70ID WHEN 4 THEN 'FAK' WHEN 6 THEN 'PAU' WHEN 2 THEN 'ODP' WHEN 3 THEN 'ODP' END as StatusKod
,p31Date
,p32Name
,a.p31Hours_Orig
,case when a.p70ID=4 THEN p31Hours_Invoiced END as hodiny_fakturovat
,case when a.p70ID=6 THEN p31Hours_Orig END as hodiny_pausal
,case when a.p70ID IN (2,3) THEN p31Hours_Orig END as hodiny_odpis
,dbo.p31_getrate_from_p51id(@p51id,a.p31ID) as vzorova_sazba
,case when a.p70ID=6 then @pausaly*(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)/@honorar_cenik end as podil_na_pausalu
,case when a.p70ID=6 then @pausaly*(case when a.p70ID=6 THEN dbo.p31_getrate_from_p51id(@p51id,a.p31ID) end*p31Hours_Orig)/@honorar_cenik/a.p31Hours_Orig else a.p31Rate_Billing_Invoiced*isnull(a.p31ExchangeRate_Invoice,1) end as efektivni_sazba
,p31Text
,a.p41ID
,a.j02ID
from
p31Worksheet a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID
WHERE p34.p33ID=1 AND a.p91ID=@pid
ORDER BY p41.p41Name,a.p31Date

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


update p91Invoice set j27ID=@j27id,p91UserUpdate=@login,p91DateUpdate=getdate(),p91ExchangeRate=1,p91DateExchange=null WHERE p91ID=@p91id

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
  set @err_ret='Sazba DPH ['+convert(varchar(10),@newvatrate)+'%] není platná pro dané datum plnění, měnu a zemi!'
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

----------P---------------p91_changelog-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_changelog') and type = 'P')
 drop procedure p91_changelog
GO



CREATE procedure [dbo].[p91_changelog]
@p91id int
AS

SELECT a.RowID as pid
,row_number() over (order by a.RowID) as Verze
,a.p91DateInsert as Založeno
,a.p91UserInsert as Založil
,a.p91DateUpdate as Aktualizováno
,a.p91UserUpdate as Aktualizoval
,a.p91Code as [Kód]
,a.p91Client as [Klient]
,p92.p92Name as [Typ faktury]
,a.p91Code as [Kód]
,j27.j27Code as [Měna]
,a.p91ExchangeRate as [Měnový kurz]
,a.p91DateExchange as [Datum kurzu]
,a.p91Amount_WithoutVat as [Celkem bez DPH]
,a.p91Amount_Vat as [Celkem DPH]
,a.p91Amount_WithVat as [Celkem vč.DPH]
,a.p91Amount_Debt as [Dluh]
,a.p91RoundFitAmount as [Haléřové zaokrouhlení]
,a.p91Amount_TotalDue as [Celkem k úhradě]
,a.p91Amount_Billed as [Již uhrazeno]
,a.p91Text1 as [Text faktury]
,a.p91Text2 as [Technický text]
,a.p91ProformaBilledAmount as [Spárovaná úhrada zálohy]
,b02.b02Name as [Stav]
,a.p91IsDraft as [Je draft]
,j17.j17Name as [DPH region]
,a.p91Date as [Vystaveno]
,a.p91DateMaturity as [Splatnost]
,a.p91DateSupply as [DUZP]
,a.p91DateBilled as [Poslední úhrada]
,a.p91Datep31_From as [Worksheet od]
,a.p91Datep31_Until as [Worksheet do]
,a.p91Client_RegID as [IČ klienta]
,a.p91Client_VatID as [DIČ klienta]
,a.p91ClientAddress1_Street as [Ulice klienta]
,a.p91ClientAddress1_City as [Město klienta]
,a.p91ClientAddress1_ZIP as [PSČ klienta]
,a.p91ClientPerson as [Osoba klienta]
,p63.p63Name as [Režijní přirážka]
,p80.p80Name as [Struktura rozpisu]
,a.p91FixedVatRate as [Fixní DPH sazba]
,p98.p98Name as [Zaokrouhovací pravidlo]
,j27domestic.j27Code as [Domácí měna]
,a.p91Amount_WithoutVat_Standard as [Základ zvýšené DPH]
,a.p91VatRate_Standard as [Sazba zvýšení DPH]
,a.p91Amount_WithoutVat_None as [Základ nulová DPH]
,a.p91Amount_WithoutVat_Low as [Základ snížená DPH]
,a.p91VatRate_Low as [Sazba snížené DPH]
,a.p91Amount_WithoutVat_Standard as [Základ zvýšená DPH]
,a.p91ValidUntil as [Platnost záznamu]
FROM
p91Invoice_Log a
LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID
LEFT OUTER JOIN j27Currency j27domestic ON a.j27ID_Domestic=j27domestic.j27ID
LEFT OUTER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID
LEFT OUTER JOIN p98Invoice_Round_Setting_Template p98 ON a.p98ID=p98.p98ID
LEFT OUTER JOIN p63Overhead p63 ON a.p63ID=p63.p63ID
LEFT OUTER JOIN p80InvoiceAmountStructure p80 ON a.p80ID=p80.p80ID
LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID
LEFT OUTER JOIN p41Project p41 ON a.p41ID_First=p41.p41ID
LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID
WHERE a.p91ID=@p91id
ORDER BY a.RowID DESC


GO

----------P---------------p91_proforma_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_proforma_delete') and type = 'P')
 drop procedure p91_proforma_delete
GO






CREATE     procedure [dbo].[p91_proforma_delete]
@p91id int
,@p90id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT

AS

set @p91id=isnull(@p91id,0)
set @p90id=isnull(@p90id,0)


if @err_ret<>''
  return


delete from p99Invoice_Proforma WHERE p91ID=@p91id AND p90ID=@p90id

exec p91_recalc_amount @p91id


































GO

----------P---------------p91_proforma_save-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_proforma_save') and type = 'P')
 drop procedure p91_proforma_save
GO




CREATE     procedure [dbo].[p91_proforma_save]
@p91id int
,@p90id int
,@p82id int
,@j03id_sys int
,@err_ret varchar(1000) OUTPUT

AS

set @p91id=isnull(@p91id,0)

set @p90id=isnull(@p90id,0)

declare @login nvarchar(50)

set @login=dbo.j03_getlogin(@j03id_sys)

set @err_ret=''

if @p91id=0
 set @err_ret='@p91id is missing!'

if @p90id=0
 set @err_ret='@p90id is missing!'

if isnull(@p82id,0)=0
 set @err_ret='@p82id is missing!'

if @j03id_sys=0
 set @err_ret='@j03id_sys is missing!'

if @err_ret<>''
  return

declare @amount float,@amount_withoutvat float,@amount_vat float,@vatrate float

select @vatrate=p90VatRate FROM p90Proforma WHERE p90ID=@p90id

select @amount=p82Amount,@amount_withoutvat=p82Amount_WithoutVat,@amount_vat=p82Amount_Vat FROM p82Proforma_Payment WHERE p82ID=@p82id



if @amount<>(@amount_withoutvat+@amount_vat)
 begin
  set @amount_withoutvat=round(@amount/(1+@vatrate/100),2)
  set @amount_vat=@amount-@amount_withoutvat
 end

if not exists(select p99ID from p99Invoice_Proforma WHERE p91ID=@p91id AND p90ID=@p90id AND p82ID=@p82id)
 INSERT INTO p99Invoice_Proforma(p82ID,p91ID,p90ID,p99UserInsert,p99UserUpdate,p99DateUpdate,p99Amount) values(@p82id,@p91id,@p90id,@login,@login,getdate(),@amount)



update p99Invoice_Proforma set p99Amount=@amount,p99Amount_WithoutVat=@amount_withoutvat,p99Amount_Vat=@amount_vat
WHERE p91ID=@p91id AND p90ID=@p90id AND p82ID=@p82id


exec p91_recalc_amount @p91id

































GO

----------P---------------p91_recalc_amount-------------------------

if exists (select 1 from sysobjects where  id = object_id('p91_recalc_amount') and type = 'P')
 drop procedure p91_recalc_amount
GO



CREATE procedure [dbo].[p91_recalc_amount]
@p91id int

AS

declare @j27id_dest int,@datSupply datetime,@j27id_domestic int,@p92invoicetype int,@p92id int,@p41id_first int,@j17id int,@p98id int,@p63id int,@p80id int
declare @exchangedate datetime

select @j27id_dest=a.j27id,@datSupply=a.p91DateSupply,@p92id=a.p92id,@p92invoicetype=b.p92InvoiceType,@p41id_first=a.p41ID_First,@j17id=a.j17ID,@p98id=a.p98ID,@p63id=a.p63ID,@p80id=a.p80ID,@exchangedate=a.p91DateExchange
from p91invoice a INNER JOIN p92InvoiceType b ON a.p92ID=b.p92ID
where a.p91id=@p91id

declare @p41id_test int

select top 1 @p41id_test=p41id from p31worksheet where p91id=@p91id


--*****************měnové kurzy*************************---------------
if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Domestic')
 select @j27id_domestic=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Domestic'
else
 set @j27id_domestic=2

if @p98id is null
 select @p98id=p98ID FROM p98Invoice_Round_Setting_Template WHERE p98IsDefault=1	---výchozí zaokrouhlovací pravidlo v systému

if @exchangedate is null and @j27id_domestic<>@j27id_dest
 begin
  select TOP 1 @exchangedate=m62date FROM m62ExchangeRate where j27id_master=@j27id_domestic and m62date<=@datSupply and j27id_slave=@j27id_dest order by m62date desc

  update p91invoice set p91DateExchange=@exchangedate,p91ExchangeRate=dbo.get_exchange_rate(1,p91DateSupply,j27id,@j27id_domestic)
  where p91id=@p91id
 end

if @exchangedate is null and exists(select p31ID FROM p31Worksheet WHERE p91ID=@p91id AND j27ID_Billing_Orig IS NOT NULL AND j27ID_Billing_Orig<>@j27id_dest)
 begin
  declare @j27id_dest_worksheet int

  select TOP 1 @j27id_dest_worksheet=j27ID_Billing_Orig FROM p31Worksheet WHERE p91ID=@p91id AND j27ID_Billing_Orig IS NOT NULL AND j27ID_Billing_Orig<>@j27id_dest

  select TOP 1 @exchangedate=m62date FROM m62ExchangeRate where j27id_master=@j27id_domestic and m62date<=@datSupply and j27id_slave=@j27id_dest_worksheet order by m62date desc

  update p91invoice set p91DateExchange=@exchangedate,p91ExchangeRate=dbo.get_exchange_rate(1,p91DateSupply,@j27id_dest_worksheet,@j27id_domestic)
  where p91id=@p91id

 end


if @exchangedate is null
 begin
  update p91invoice set p91DateExchange=null,p91ExchangeRate=1 where p91id=@p91id

  set @exchangedate=@datSupply
 end

if @j27id_dest=@j27id_domestic
 begin
  update p91invoice set p91ExchangeRate=1 where p91id=@p91id
 end

----výchozí měnový kurz pocházející z j27id worksheet záznamu-----------
update p31worksheet set p31ExchangeRate_Invoice=dbo.get_exchange_rate(1,@exchangedate,j27ID_Billing_Orig,@j27id_dest)
WHERE p91id=@p91id

----měnový kurz pro manuálně upravované částky ve faktuře-----------
update p31worksheet set p31ExchangeRate_InvoiceManual=dbo.get_exchange_rate(1,@exchangedate,j27ID_Billing_Invoiced,@j27id_dest)
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
 begin  ---existují fakturované úkony, u kterých nesedí výpočet DPH
  update p31worksheet set p31amount_vat_invoiced=p31amount_withoutvat_invoiced*p31vatrate_invoiced/100
  ,p31amount_withvat_invoiced=p31amount_withoutvat_invoiced+(p31amount_withoutvat_invoiced*p31vatrate_invoiced/100)
  WHERE p91id=@p91id
    
 end

update p31WorkSheet set p31Value_Invoiced=p31Amount_WithoutVat_Invoiced
where p91ID=@p91id and p32ID in (select p32ID from p32Activity a inner join p34ActivityGroup b on a.p34ID=b.p34ID where b.p33ID in (2,5))

update p31worksheet set j27ID_Billing_Invoiced=@j27id_dest
where p91id=@p91id and isnull(j27ID_Billing_Invoiced,0)<>@j27id_dest


exec dbo.p91_calc_overhead @p91id,@p63id	---případná režijní přirážka k faktuře

----měnový kurz z fakturační měny do domácí měny----------
update p31worksheet set j27ID_Billing_Invoiced_Domestic=@j27id_domestic, p31ExchangeRate_Domestic=dbo.get_exchange_rate(1,@exchangedate,j27ID_Billing_Invoiced,@j27id_domestic)
WHERE p91id=@p91id

update p31worksheet set p31Amount_WithoutVat_Invoiced_Domestic=p31Amount_WithoutVat_Invoiced*p31ExchangeRate_Domestic
,p31Amount_WithVat_Invoiced_Domestic=p31Amount_WithVat_Invoiced*p31ExchangeRate_Domestic
,p31Amount_Vat_Invoiced_Domestic=p31Amount_Vat_Invoiced*p31ExchangeRate_Domestic
where p91id=@p91id



--***************částky DPH***********************************--
declare @p91amount_withoutvat_none float

declare @p91amount_withoutVat_low float,@p91amount_withoutVat_standard float,@p91amount_withoutVat_special float
declare @p91amount_withVat_low float,@p91amount_withVat_standard float,@p91amount_withVat_special float
declare @p91amount_Vat_low float,@p91amount_Vat_standard float,@p91amount_Vat_special float
declare @p91vatrate_low float,@p91vatrate_standard float,@p91vatrate_special float


select @p91vatrate_low=dbo.p91_get_vatrate(2,@j27id_dest,@j17id,@datSupply)
select @p91vatrate_standard=dbo.p91_get_vatrate(3,@j27id_dest,@j17id,@datSupply)
select @p91vatrate_special=dbo.p91_get_vatrate(4,@j27id_dest,@j17id,@datSupply)

exec dbo.p91_calc_p81 @p91id,@p80id

if @p92invoicetype=2	--dobropis
 begin  ---u dobropisu brát sazby DPH z původní faktury   
   select @p91vatrate_low=p91vatrate_low,@p91vatrate_standard=p91vatrate_standard,@p91vatrate_special=p91vatrate_special
   FROM
   p91Invoice WHERE p91ID in (select p91ID_CreditNoteBind FROM p91invoice where p91id=@p91id)
 end

select @p91amount_withoutvat_none=sum(p81Amount_WithoutVat)
FROM p81InvoiceAmount where p91id=@p91id and p81VatRate=0

if @p91vatrate_low>0
 begin
  select @p91amount_withoutVat_low=sum(p81Amount_WithoutVat),@p91amount_withVat_low=sum(p81Amount_WithVat),@p91amount_Vat_low=sum(p81Amount_Vat)
  FROM p81InvoiceAmount where p91id=@p91id and p81VatRate=@p91vatrate_low
 end

if @p91vatrate_standard>0
 begin
  select @p91amount_withoutVat_standard=sum(p81Amount_WithoutVat),@p91amount_withVat_standard=sum(p81Amount_WithVat),@p91amount_Vat_standard=sum(p81Amount_Vat)
  FROM p81InvoiceAmount where p91id=@p91id and p81VatRate=@p91vatrate_standard
 end


if @p91vatrate_special>0
 begin
  select @p91amount_withoutVat_special=sum(p81Amount_WithoutVat),@p91amount_withVat_special=sum(p81Amount_WithVat),@p91amount_Vat_special=sum(p81Amount_Vat)
  FROM p81InvoiceAmount where p91id=@p91id and p81VatRate=@p91vatrate_special
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

if @p91amount_withVat<>(@p91amount_withoutVat+@p91amount_Vat)
 begin	---celková částka vč. dph se matematicky nerovná
  set @p91amount_withVat=@p91amount_withoutVat+@p91amount_Vat
 end


---*************- spárovaná zálohová faktura************************************
declare @p91amount_billed float,@p91amount_debt float,@datLastBilled datetime,@p91proformaamount_withoutvat_low float,@p91proformaamount_withoutvat_standard float
declare @p91proformaamount float,@p91proformabilledamount float,@p91proformaamount_vat_low float,@p91proformaamount_vat_standard float,@p91proformaamount_withoutvat_none float
declare @p91proformaamount_vatrate float
select @p91proformaamount=sum(b.p99Amount),@p91proformabilledamount=sum(b.p99Amount)
,@p91proformaamount_vat_low=sum(case when a.p90VatRate=@p91vatrate_low then p99Amount_Vat end)
,@p91proformaamount_vat_standard=sum(case when a.p90VatRate=@p91vatrate_standard then p99Amount_Vat end)
,@p91proformaamount_withoutvat_low=sum(case when a.p90VatRate=@p91vatrate_low then p99Amount_WithoutVat end)
,@p91proformaamount_withoutvat_standard=sum(case when a.p90VatRate=@p91vatrate_standard then p99Amount_WithoutVat end)
,@p91proformaamount_withoutvat_none=sum(case when a.p90VatRate=0 then p99Amount_WithoutVat end)
FROM p90Proforma a inner join p99Invoice_Proforma b on a.p90id=b.p90id
where b.p91id=@p91id

select TOP 1 @p91proformaamount_vatrate=p90VatRate FROM p90Proforma WHERE p90ID IN (select p90ID FROM p99Invoice_Proforma WHERE p91ID=@p91id)

set @p91proformabilledamount=isnull(@p91proformabilledamount,0)
set @p91proformaamount=isnull(@p91proformaamount,0)
set @p91proformaamount_vat_standard=isnull(@p91proformaamount_vat_standard,0)
set @p91proformaamount_vat_low=isnull(@p91proformaamount_vat_low,0)
set @p91proformaamount_withoutvat_standard=isnull(@p91proformaamount_withoutvat_standard,0)
set @p91proformaamount_withoutvat_low=isnull(@p91proformaamount_withoutvat_low,0)
set @p91proformaamount_withoutvat_none=isnull(@p91proformaamount_withoutvat_none,0)

--set @p91amount_withoutvat_none=@p91amount_withoutvat_none-@p91proformaamount_withoutvat_none
--set @p91amount_withoutVat_low=@p91amount_withoutVat_low-@p91proformaamount_withoutvat_low
--set @p91amount_withoutVat_standard=@p91amount_withoutVat_standard-@p91proformaamount_withoutvat_standard
--set @p91amount_Vat_standard=@p91amount_Vat_standard-@p91proformaamount_vat_standard
--set @p91amount_Vat_low=@p91amount_Vat_low-@p91proformaamount_vat_low

--************************zaokrouhlování************************************----
declare @p97id int,@p97scale int,@p91roundfitamount float
declare @p97amountflag int	---jaká částka je předmětem zaokrouhlování: 1-částka DPH, 2-částka bez DPH (základ), 3-částka vč. DPH

set @p91roundfitamount=0

if @p98id is not null
 select @p97id=p97ID,@p97amountflag=p97AmountFlag,@p97scale=p97Scale FROM p97Invoice_Round_Setting WHERE p98ID=@p98id AND j27ID=@j27id_dest


if @p97id is not null
  begin  ----dojde k zaokrouhlování
    if @p97amountflag=3		---zaokrouhluje se celková částka faktury
	begin
	  set @p91roundfitamount=round(@p91amount_withVat,@p97scale)-@p91amount_withVat
	  set @p91amount_withVat=@p91amount_withVat+@p91roundfitamount
	  
	end
	if @p97amountflag=2		---zaokrouhluje se celkový základ daně
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
	   --set @p91roundfitamount=0	---zaokrouhlení je nula, protože už je zahrnuté v částce bez DPH
	   
	 end

    if @p97amountflag=1		---zaokrouhluje se výsledná částka DPH
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

	  set @p91roundfitamount=0	---zaokrouhlení je nula, protože už je zahrnuté v částce DPH
	  
	end
  end



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

----****závěrečný update**********************
update p91invoice set p91amount_withoutvat_none=@p91amount_withoutvat_none,p91amount_withoutVat_low=@p91amount_withoutVat_low,p91amount_withoutVat_standard=@p91amount_withoutVat_standard,p91amount_withoutVat_special=@p91amount_withoutVat_special
,p91amount_withVat_low=@p91amount_withVat_low,p91amount_withVat_standard=@p91amount_withVat_standard,p91amount_withVat_special=@p91amount_withVat_special
,p91amount_Vat_low=@p91amount_Vat_low,p91amount_Vat_standard=@p91amount_Vat_standard,p91amount_Vat_special=@p91amount_Vat_special
,p91vatrate_low=@p91vatrate_low,p91vatrate_standard=@p91vatrate_standard,p91vatrate_special=@p91vatrate_special
,p91amount_withoutVat=@p91amount_withoutVat,p91amount_withVat=@p91amount_withVat,p91amount_Vat=@p91amount_Vat
,p91amount_billed=@p91amount_billed,p91amount_debt=@p91amount_debt,p91DateBilled=@datLastBilled
,p91roundfitamount=@p91roundfitamount
,p91proformaamount=@p91proformaamount,p91proformabilledamount=@p91proformabilledamount
,p91Amount_TotalDue=@p91amount_withVat-@p91proformabilledamount
,p91proformaamount_vat_standard=@p91proformaamount_vat_standard,p91proformaamount_vat_low=@p91proformaamount_vat_low,p91proformaamount_withoutvat_standard=@p91proformaamount_withoutvat_standard
,p91proformaamount_withoutvat_low=@p91proformaamount_withoutvat_low,p91proformaamount_withoutvat_none=@p91proformaamount_withoutvat_none,p91proformaamount_vatrate=@p91proformaamount_vatrate
,p41id_first=@p41id_first
,p91DateUpdate=getdate()
,j27ID_Domestic=@j27id_domestic
WHERE p91id=@p91id


---rozúčtování faktury
--exec p91_invoice_statement @p91id

if @p92invoicetype=2
 begin
   --přepočítat ještě svázaný doklad k dobropisu
   declare @p91id_bound int
   select TOP 1 @p91id_bound=p91ID_CreditNoteBind from p91invoice where p91id=@p91id

   if @p91id_bound is not null
     exec p91_recalc_amount @p91id_bound
 end


 if exists(select x35ID FROM x35GlobalParam WHERE x35Key LIKE 'p51ID_FPR' AND ISNUMERIC(x35Value)=1)
  begin	---přepočet efektivních sazeb úkonů faktury
    declare @p51id int

	select @p51id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'p51ID_FPR'

	exec p91_fpr_recalc_invoice @p51id,@p91id
  end
  
exec dbo.p91_append_log @p91id
























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
declare @p91code varchar(50),@x38id int,@isdraft bit,@x38id_draft int

select @p91code=p91Code,@x38id=p92.x38ID,@x38id_draft=p92.x38ID_Draft,@isdraft=a.p91IsDraft
FROM
p91Invoice a INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID
WHERE a.p91ID=@p91id


if @isdraft=1
 set @x38id=@x38id_draft


if left(@p91code,4)='TEMP' OR @p91code is null
 begin

  set @p91code=dbo.x38_get_freecode(@x38id,391,@p91id,@isdraft,1)
  if @p91code<>''
   UPDATE p91Invoice SET p91Code=@p91code WHERE p91ID=@p91id 

  
 end 







GO

----------P---------------p92_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p92_delete') and type = 'P')
 drop procedure p92_delete
GO





CREATE   procedure [dbo].[p92_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p92id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu typu faktury z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p92ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna faktura má vazbu na tento typ ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--p93id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu hlavičky dodavatele z tabulky p92InvoiceType
declare @ref_pid int

SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE p93ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ faktury má vazbu na tento záznam.'


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

----------P---------------p95_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p95_delete') and type = 'P')
 drop procedure p95_delete
GO




CREATE   procedure [dbo].[p95_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p95id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu  z tabulky p95InvoiceRow
declare @ref_pid int

SELECT TOP 1 @ref_pid=p32ID from p32Activity WHERE p95ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna aktivita má vazbu na tento oddíl ('+dbo.GetObjectAlias('p32',@ref_pid)+')'

if exists(select p81ID FROM p81InvoiceAmount WHERE p95ID=@pid)
 set @err_ret='Minimálně u jedné faktury existuje cenový rozpis podle tohoto fakturačního oddílu.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	DELETE FROM p95InvoiceRow where p95ID=@pid

	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  















GO

----------P---------------p97_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p97_delete') and type = 'P')
 drop procedure p97_delete
GO





CREATE   procedure [dbo].[p97_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p97id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu zaokrouhlovacího pravidla z tabulky p97Invoice_Round_Setting


DELETE from p97Invoice_Round_Setting where p97ID=@pid

















GO

----------P---------------p98_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('p98_delete') and type = 'P')
 drop procedure p98_delete
GO



CREATE   procedure [dbo].[p98_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--p98id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu zaokrouhlovacího pravidla: p98Invoice_Round_Setting_Template
declare @ref_pid int

SELECT TOP 1 @ref_pid=p91ID from p91Invoice WHERE p98ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jedna vystavená faktura má vazbu na toto zaokrouhlovací pravidlo ('+dbo.GetObjectAlias('p91',@ref_pid)+')'


set @ref_pid=null
SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE p98ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ faktury má vazbu na toto zaokrouhlovací pravidlo.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	delete from p97Invoice_Round_Setting WHERE p98ID=@pid

	delete from p98Invoice_Round_Setting_Template where p98ID=@pid

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


if exists(select p85ID FROM p85TempBox)
 begin
  if exists(select p85ID FROM p85TempBox WHERE p85GUID LIKE 'timer%' AND  DATEDIFF(DAY,p85DateInsert,getdate())<2)
   DELETE FROM  p85TempBox WHERE p85GUID NOT LIKE 'timer%'   
  else
   truncate table p85TempBox
 end


if exists(select p31ID FROM p31Worksheet_Temp)
 truncate table p31Worksheet_Temp


if exists(select p31ID FROM p31WorkSheet_FreeField_Temp)
 truncate table p31WorkSheet_FreeField_Temp

 if exists(select x47ID FROM x47EventLog where x29ID=141 and x45ID=14101 and x47Description is null and x47RecordPID NOT IN (SELECT p41ID FROM p41Project))
   update x47EventLog set x47Description='deleted' where x29ID=141 and x45ID=14101 and x47RecordPID NOT IN (SELECT p41ID FROM p41Project)
	
	
if exists(select x47ID FROM x47EventLog where x29ID=328 and x45ID=32801 and x47Description is null and x47RecordPID NOT IN (SELECT p28ID FROM p28Contact))
   update x47EventLog set x47Description='deleted' where x29ID=328 and x45ID=32801 and x47RecordPID NOT IN (SELECT p28ID FROM p28Contact)
	
if exists(select x47ID FROM x47EventLog where x29ID=391 and x45ID=39101 and x47Description is null and x47RecordPID NOT IN (SELECT p91ID FROM p91Invoice))
   update x47EventLog set x47Description='deleted' where x29ID=391 and x45ID=39101 and x47RecordPID NOT IN (SELECT p91ID FROM p91Invoice)
	
if exists(select x47ID FROM x47EventLog where x29ID=223 and x45ID=22301 and x47Description is null and x47RecordPID NOT IN (SELECT o23ID FROM o23Doc))
   update x47EventLog set x47Description='deleted' where x29ID=223 and x45ID=22301 and x47RecordPID NOT IN (SELECT o23ID FROM o23Doc)

if exists(select x47ID FROM x47EventLog where x29ID=356 and x45ID=35601 and x47Description is null and x47RecordPID NOT IN (SELECT p56ID FROM p56Task))
   update x47EventLog set x47Description='deleted' where x29ID=356 and x45ID=35601 and x47RecordPID NOT IN (SELECT p56ID FROM p56Task)



update a set x47Name=b.p91Code,x47NameReference=b.p91Client
from x47EventLog a INNER JOIN p91Invoice b ON a.x47RecordPID=b.p91ID
WHERE a.x29ID=391


update a set x47Name=b.p41Name,x47NameReference=c.p28Name
from x47EventLog a INNER JOIN p41Project b ON a.x47RecordPID=b.p41ID
LEFT OUTER JOIN p28Contact c ON b.p28ID_Client=c.p28ID
WHERE a.x29ID=141

update a set x47Name=b.p28Name
from x47EventLog a INNER JOIN p28Contact b ON a.x47RecordPID=b.p28ID
WHERE a.x29ID=328

GO

----------P---------------x18_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x18_delete') and type = 'P')
 drop procedure x18_delete
GO




CREATE   procedure [dbo].[x18_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--x18id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu kategorie z tabulky  číselníku z tabulky x18EntityCategory
if exists(select x19ID FROM x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory b ON a.x20ID=b.x20ID WHERE b.x18ID=@pid)
 set @err_ret='Nelze odstranit, protože minimálně jedna položka již byla použita v oštítkování nějakého záznamu. Dokument můžete přesunout do archivu nebo vyčistit jeho vazby.'

if isnull(@err_ret,'')<>''
 return 

declare @x23id int

SELECT @x23id=x23ID FROM x18EntityCategory WHERE x18ID=@pid

if not exists(select x18ID FROM x18EntityCategory WHERE x18ID<>@pid AND x23ID=@x23id)
 begin
  if exists(select o23ID FROM o23Doc WHERE x23ID=@x23id)
   set @err_ret='Typ dokumentu obsahuje minimálně jeden dokument. Je třeba nejdříve odstranit svázané dokumenty.'

 end

if isnull(@err_ret,'')<>''
 return 



BEGIN TRANSACTION

BEGIN TRY	
	
	DELETE FROM x20EntiyToCategory WHERE x18ID=@pid

	DELETE FROM x16EntityCategory_FieldSetting WHERE x18ID=@pid		

	

	delete from x18EntityCategory where x18ID=@pid

	if not exists(select o23ID FROM o23Doc WHERE x23ID=@x23id)
	  DELETE FROM x23EntityField_Combo WHERE x23ID=@x23id

	

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  update x18EntityCategory set x23ID=@x23id WHERE x18ID=@pid

  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  




















GO

----------P---------------x23_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x23_delete') and type = 'P')
 drop procedure x23_delete
GO






CREATE   procedure [dbo].[x23_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--x23id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu combo číselníku z tabulky x27EntityFieldGroup
if exists(select o23ID FROM o23Doc WHERE x23ID=@pid)
 set @err_ret='Číselník obsahuje minimálně jednu položku - je třeba vyčistit položky číselníku.'

if exists(select x18ID FROM x18EntityCategory where x23ID=@pid)
 set @err_ret='Combo seznam má vazbu na minimálně jeden štítek.'

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x23EntityField_Combo where x23ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  


















GO

----------P---------------x27_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x27_delete') and type = 'P')
 drop procedure x27_delete
GO










CREATE   procedure [dbo].[x27_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--x27id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu skupiny uživatelských polí z tabulky x27EntityFieldGroup
if exists(select x28ID FROM x28EntityField WHERE x27ID=@pid)
 set @err_ret='Minimálně jedno uživatelské pole je svázané s touto skupinou.'

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--x28id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu uživatelského pole z tabulky x28EntityField

if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	if exists(select x26ID FROM x26EntityField_Binding WHERE x28ID=@pid)
	 DELETE FROM x26EntityField_Binding WHERE x28ID=@pid


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--x31id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu sestavy z tabulky x31Report





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
,@x36key varchar(50)		--název klíče
,@clear_after_read bit		--1 - ihned po přečtení vyčistit hodnotu
,@x36value nvarchar(500) OUTPUT	--hodnota klíče

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
,@x36key varchar(50)		--název klíče
,@x36value nvarchar(500)	--hodnota klíče

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

----------P---------------x36userparam_save_allusers-------------------------

if exists (select 1 from sysobjects where  id = object_id('x36userparam_save_allusers') and type = 'P')
 drop procedure x36userparam_save_allusers
GO




CREATE   procedure [dbo].[x36userparam_save_allusers]
@j03id_sys int				--id přihlášeného uživatele
,@x36key varchar(50)		--název klíče
,@x36value nvarchar(500)	--hodnota klíče

AS

--Uložení záznamu uživatelského parametru do tabulky x36UserParam všem uživatelům

if isnull(@x36key,'')=''
  return

declare @pid int,@login nvarchar(50),@x36validuntil datetime,@j03id int

set @login=dbo.j03_getlogin(@j03id_sys)
set @x36validuntil=convert(datetime,'01.01.3000',104)



DECLARE curCR CURSOR FOR 
SELECT j03ID FROM j03User WHERE getdate() between j03ValidFrom AND j03ValidUntil

OPEN curCR
FETCH NEXT FROM curCR 
INTO @j03id
WHILE @@FETCH_STATUS = 0
BEGIN
  set @pid=null

  select top 1 @pid=x36id from x36UserParam where j03id=@j03id and x36key like @x36key

  if @pid is not null
   update x36UserParam set x36value=@x36value,x36dateupdate=getdate(),x36validuntil=@x36validuntil,x36userupdate=@login where x36id=@pid
  else
   insert into x36UserParam(j03id,x36value,x36key,x36dateinsert,x36dateupdate,x36userinsert,x36userupdate,x36validfrom,x36validuntil) values(@j03id,@x36value,@x36key,getdate(),getdate(),@login,@login,getdate(),@x36validuntil)


  FETCH NEXT FROM curCR 
  INTO @j03id
END
CLOSE curCR
DEALLOCATE curCR



































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
@j03id_sys int				--přihlášený uživatel
,@pid int					--x38id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu číselné řady z tabulky x38CodeLogic
declare @ref_pid int



SELECT TOP 1 @ref_pid=p42ID from p42ProjectType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ projektu je svázaný s touto číselnou řadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p29ID from p29ContactType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ klienta je svázaný s touto číselnou řadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ faktury je svázaný s touto číselnou řadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p89ID from p89ProformaType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ zálohy je svázaný s touto číselnou řadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p57ID from p57TaskType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ úkolu je svázaný s touto číselnou řadou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=x18ID from x18EntityCategory WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minimálně jeden typ dokumentu je svázaný s touto číselnou řadou.'


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

----------P---------------x38_get_freecode_proc-------------------------

if exists (select 1 from sysobjects where  id = object_id('x38_get_freecode_proc') and type = 'P')
 drop procedure x38_get_freecode_proc
GO


CREATE PROCEDURE [dbo].[x38_get_freecode_proc]
@x38id int
,@x29id int
,@datapid int
,@isdraft bit
,@attempt_number int
,@ret_code varchar(50) OUTPUT
AS

declare @mask varchar(200),@x38IsUseDbPID bit

if @x38id is not null and isnull(@isdraft,0)=0
 begin
  select @mask=x38MaskSyntax,@x38IsUseDbPID=x38IsUseDbPID FROM x38CodeLogic WHERE x38id=@x38id
  
  if @mask is not null and @x38IsUseDbPID=0
   begin
	declare @s nvarchar(1000),@ret varchar(50),@pars nvarchar(1000)
	set @mask=replace(@mask,'@pid',convert(varchar(50),@datapid))

	set @s=N'select @ret='+@mask
	set @pars=N'@ret varchar(50) output'

	exec sp_executesql @s, @pars, @ret output;

	if @ret is not null
	 begin
	 set @ret_code=@ret
	 return
	 end
   end
 end



select @ret_code=dbo.x38_get_freecode(@x38id,@x29id,@datapid,@isdraft,@attempt_number)


GO

----------P---------------x40_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x40_delete') and type = 'P')
 drop procedure x40_delete
GO









CREATE   procedure [dbo].[x40_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--x40id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu odeslané e-mail zprávy


BEGIN TRANSACTION

BEGIN TRY

	delete from x43MailQueue_Recipient where x40ID=@pid

	delete from o27Attachment where x40ID=@pid

	delete from x40MailQueue where x40ID=@pid

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--x46id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu notifikačního nastavení z tabulky x46EventNotification


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

----------P---------------x47_appendlog-------------------------

if exists (select 1 from sysobjects where  id = object_id('x47_appendlog') and type = 'P')
 drop procedure x47_appendlog
GO






CREATE PROCEDURE [dbo].[x47_appendlog]
@j03ID int
,@x45ID int
,@x29ID int
,@x29ID_Reference int
,@x47RecordPID int
,@x47RecordPID_Reference int
,@x47Name varchar(200)
,@x47NameReference varchar(200)
,@x47Description varchar(200)
,@ret_x47id INT OUTPUT
AS

declare @login varchar(50)

select @login=j03Login FROM j03User WHERE j03ID=@j03ID

if exists(select x47ID FROM x47EventLog WHERE x29ID=@x29ID AND x47RecordPID=@x47RecordPID)
 UPDATE x47EventLog set x47Name=@x47Name,x47NameReference=@x47NameReference WHERE x29ID=@x29ID AND x47RecordPID=@x47RecordPID


INSERT INTO x47EventLog(x45ID,x29ID,x29ID_Reference,j03ID,x47RecordPID,x47RecordPID_Reference,x47Name,x47NameReference,x47Description,x47DateInsert,x47UserInsert,x47DateUpdate,x47UserUpdate)
VALUES(@x45ID,@x29ID,@x29ID_Reference,@j03ID,@x47RecordPID,@x47RecordPID_Reference,@x47Name,@x47NameReference,@x47Description,getdate(),@login,getdate(),@login)

set @ret_x47id=@@IDENTITY




GO

----------P---------------x48_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x48_delete') and type = 'P')
 drop procedure x48_delete
GO








CREATE   procedure [dbo].[x48_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--x48id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu SQL úlohy z tabulky x48SqlTask


BEGIN TRANSACTION

BEGIN TRY
	
	

	delete from x48SqlTask WHERE x48ID=@pid

	

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
@j03id_sys int				--přihlášený uživatel
,@pid int					--x50id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu pole z tabulky x50Help

if exists(select o27ID FROM o27Attachment where x50ID=@pid)
 DELETE FROM o27Attachment where x50ID=@pid

delete from x50Help where x50ID=@pid





























GO

----------P---------------x55_delete-------------------------

if exists (select 1 from sysobjects where  id = object_id('x55_delete') and type = 'P')
 drop procedure x55_delete
GO





CREATE   procedure [dbo].[x55_delete]
@j03id_sys int				--přihlášený uživatel
,@pid int					--x55id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu z tabulky x55HtmlSnippet

if exists(select x57ID FROM x57UserPageBinding WHERE x55ID=@pid)
 set @err_ret='Minimálně jedna osobní stránka využívá tento BOX.'


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
@j03id_sys int				--přihlášený uživatel
,@pid int					--x67id
,@err_ret varchar(500) OUTPUT		---případná návratová chyba

AS
--odstranění záznamu role z tabulky x67EntityRole
declare @ref_pid int,@x29id int,@x69recordpid int


set @ref_pid=null
SELECT TOP 1 @ref_pid=a.x69ID,@x29id=b.x29ID,@x69recordpid=a.x69recordpid
from x69EntityRole_Assign a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID
WHERE b.x67ID=@pid

if @ref_pid is not null and @x29id=141
 set @err_ret='Tato role je již obsazena v minimálně jednom projektu ('+dbo.GetObjectAlias('p41',@x69recordpid)+')'

if @ref_pid is not null and @x29id=328
 set @err_ret='Tato role je již obsazena v minimálně jednom záznamu klienta ('+dbo.GetObjectAlias('p28',@x69recordpid)+')'

if @ref_pid is not null and @x29id=356
 set @err_ret='Tato role je již obsazena v minimálně jednom úkolu ('+dbo.GetObjectAlias('p56',@x69recordpid)+')'

if @ref_pid is not null and @x29id=223
 set @err_ret='Tato role je již obsazena v minimálně jednom dokumentu ('+dbo.GetObjectAlias('p56',@x69recordpid)+')'

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

----------V---------------view_Emails-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_Emails') and type = 'V')
 drop view view_Emails
GO








CREATE VIEW [dbo].[view_Emails]
as

select j02Email as Adresa
from
j02Person
WHERE j02Email is not null
UNION SELECT o32Value as Adresa
FROM
o32Contact_Medium
WHERE o33ID=2
UNION SELECT x40Recipient as Adresa
FROM
x40MailQueue
WHERE x40Recipient IS NOT NULL




GO

----------V---------------view_fulltext_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_fulltext_invoice') and type = 'V')
 drop view view_fulltext_invoice
GO




CREATE VIEW [dbo].[view_fulltext_invoice]
as
select 'p91' as Prefix,'Kód' as Field,p91ID as RecPid,p91Code as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
UNION SELECT 'p91' as Prefix,'Text faktury' as Field,p91ID as RecPid,p91Text1 as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91Text1 IS NOT NULL
UNION SELECT 'p91' as Prefix,'Technický text' as Field,p91ID as RecPid,p91Text2 as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91Text2 is not null
UNION SELECT 'p91' as Prefix,'Klient faktury' as Field,p91ID as RecPid,p91Client as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91Client is not null
UNION SELECT 'p91' as Prefix,'Osoba klienta' as Field,p91ID as RecPid,p91ClientPerson as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91ClientPerson is not null
UNION SELECT 'p91' as Prefix,'IČ' as Field,p91ID as RecPid,p91Client_RegID as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91Client_RegID is not null
UNION SELECT 'p91' as Prefix,'DIČ' as Field,p91ID as RecPid,p91Client_VatID as RecValue,NULL as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91Client_VatID is not null
UNION SELECT 'p91' as Prefix,'Adresa faktury' as Field,p91ID as RecPid
,isnull(p91ClientAddress1_Street,'')+isnull(', '+p91ClientAddress1_City,'')+isnull(', '+p91ClientAddress1_ZIP,'') as RecValue
,p91ClientAddress1_Country as RecComment,p91Code+isnull('/'+p91Client,'') as RecName,p91DateInsert as RecDateInsert
from
p91Invoice
WHERE p91ClientAddress1_Street is not null
UNION select 'p90' as Prefix,'Kód' as Field,a.p90ID as RecPid,a.p90Code as RecValue,NULL as RecComment,a.p90Code+'/'+b.p28Name as RecName,a.p90DateInsert as RecDateInsert
from
p90Proforma a INNER JOIN p28Contact b ON a.p28ID=b.p28ID
UNION select 'p90' as Prefix,'Text zálohy' as Field,a.p90ID as RecPid,a.p90Text1 as RecValue,NULL as RecComment,a.p90Code+'/'+b.p28Name as RecName,a.p90DateInsert as RecDateInsert
from
p90Proforma a INNER JOIN p28Contact b ON a.p28ID=b.p28ID
WHERE p90Text1 IS NOT NULL
UNION select 'p90' as Prefix,'Klient zálohy' as Field,a.p90ID as RecPid
,b.p28Name+isnull(', '+b.p28RegID,'')+isnull(', '+b.p28VatID,'') as RecValue
,NULL as RecComment,a.p90Code+'/'+b.p28Name as RecName,a.p90DateInsert as RecDateInsert
from
p90Proforma a INNER JOIN p28Contact b ON a.p28ID=b.p28ID



GO

----------V---------------view_fulltext_main-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_fulltext_main') and type = 'V')
 drop view view_fulltext_main
GO





CREATE VIEW [dbo].[view_fulltext_main]
as
select 'p41' as Prefix,'Kód' as Field,p41ID as RecPid,p41Code as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
UNION SELECT 'p41' as Prefix,'Název' as Field,p41ID as RecPid,p41Name as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
UNION SELECT 'p41' as Prefix,'Zkrácený název' as Field,p41ID as RecPid,p41NameShort as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
WHERE p41NameShort is not null
UNION SELECT 'p41' as Prefix,'Fakturační poznámka' as Field,p41ID as RecPid,p41BillingMemo as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
WHERE p41BillingMemo is not null
UNION SELECT 'p41' as Prefix,'Výchozí text faktury' as Field,p41ID as RecPid,p41InvoiceDefaultText1 as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
WHERE p41InvoiceDefaultText1 is not null
UNION SELECT 'p41' as Prefix,'Výchozí technický text faktury' as Field,p41ID as RecPid,p41InvoiceDefaultText2 as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
WHERE p41InvoiceDefaultText2 is not null
UNION SELECT 'p41' as Prefix,'Externí kód' as Field,p41ID as RecPid,p41ExternalPID as RecValue,NULL as RecComment,p41Code+' - '+p41Name as RecName,p41DateInsert as RecDateInsert
from
p41Project
WHERE p41ExternalPID is not null
UNION SELECT 'p28' as Prefix,'Kód' as Field,p28ID as RecPid,p28Code as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
UNION SELECT 'p28' as Prefix,'Název' as Field,p28ID as RecPid,p28Name as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
UNION SELECT 'p28' as Prefix,'IČ' as Field,p28ID as RecPid,p28RegID as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
WHERE p28RegID IS NOT NULL
UNION SELECT 'p28' as Prefix,'DIČ' as Field,p28ID as RecPid,p28VatID as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
WHERE p28VatID IS NOT NULL
UNION SELECT 'p28' as Prefix,'Výchozí text faktury' as Field,p28ID as RecPid,p28InvoiceDefaultText1 as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
WHERE p28InvoiceDefaultText1 IS NOT NULL
UNION SELECT 'p28' as Prefix,'Výchozí technický text faktury' as Field,p28ID as RecPid,p28InvoiceDefaultText2 as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
WHERE p28InvoiceDefaultText2 IS NOT NULL
UNION SELECT 'p28' as Prefix,'Externí kód' as Field,p28ID as RecPid,p28ExternalPID as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
WHERE p28ExternalPID IS NOT NULL
UNION SELECT 'p28' as Prefix,'Fakturační poznámka' as Field,p28ID as RecPid,p28BillingMemo as RecValue,NULL as RecComment,p28Code+' - '+p28Code as RecName,p28DateInsert as RecDateInsert
from
p28Contact
WHERE p28BillingMemo IS NOT NULL
UNION SELECT 'p28' as Prefix,'Kontaktní médium klienta' as Field,a.p28ID as RecPid,b.o33Name+': '+a.o32Value as RecValue,NULL as RecComment,c.p28Code+' - '+c.p28Code as RecName,a.o32DateInsert as RecDateInsert
FROM
o32Contact_Medium a INNER JOIN o33MediumType b ON a.o33ID=b.o33ID INNER JOIN p28Contact c ON a.p28ID=c.p28ID
UNION SELECT 'p28' as Prefix,'Adresa klienta' as Field,b.p28ID as RecPid
,isnull(a.o38Name+': ','')+isnull(a.o38Street+', ','')+isnull(a.o38City,'')+isnull(' '+a.o38ZIP,'') as RecValue,a.o38Country as RecComment
,c.p28Code+' - '+c.p28Code as RecName,c.p28DateInsert as RecDateInsert
FROM o38Address a INNER JOIN o37Contact_Address b ON a.o38ID=b.o38ID INNER JOIN p28Contact c ON b.p28ID=c.p28ID
UNION SELECT 'j02' as Prefix,'Osoba' as Field,a.j02ID as RecPid
,a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')+isnull(' <'+a.j02Email+'>','')+isnull(', '+a.j02Mobile,'') as RecValue
,isnull(c.p28Name,'')+isnull(', '+a.j02JobTitle,'')+isnull(', '+a.j02Office,'') as RecComment
,a.j02LastName+' '+a.j02FirstName as RecName,a.j02DateInsert as RecDateInsert
FROM j02Person a LEFT OUTER JOIN p30Contact_Person b ON a.j02ID=b.j02ID LEFT OUTER JOIN p28Contact c ON b.p28ID=c.p28ID




GO

----------V---------------view_fulltext_task-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_fulltext_task') and type = 'V')
 drop view view_fulltext_task
GO




CREATE VIEW [dbo].[view_fulltext_task]
as
select 'p56' as Prefix,'Kód' as Field,a.p56ID as RecPid,a.p56Code as RecValue,NULL as RecComment,b.p41Name+'/'+a.p56Name as RecName,a.p56DateInsert as RecDateInsert
from
p56Task a INNER JOIN p41Project b ON a.p41ID=b.p41ID
UNION select 'p56' as Prefix,'Název úkolu' as Field,a.p56ID as RecPid,a.p56Name as RecValue,NULL as RecComment,b.p41Name+'/'+a.p56Name as RecName,a.p56DateInsert as RecDateInsert
from
p56Task a INNER JOIN p41Project b ON a.p41ID=b.p41ID
UNION select 'p56' as Prefix,'Podrobný popis' as Field,a.p56ID as RecPid,a.p56Description as RecValue,NULL as RecComment,b.p41Name+'/'+a.p56Name as RecName,a.p56DateInsert as RecDateInsert
from
p56Task a INNER JOIN p41Project b ON a.p41ID=b.p41ID
WHERE a.p56Description IS NOT NULL



GO

----------V---------------view_LastWorksheetDateOfPerson-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_LastWorksheetDateOfPerson') and type = 'V')
 drop view view_LastWorksheetDateOfPerson
GO




CREATE VIEW [dbo].[view_LastWorksheetDateOfPerson]
as
select a.p41ID,a.j02ID,MAX(a.p31Date) as LastDate
from
p31WorkSheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID
GROUP BY a.p41ID,a.j02ID


GO

----------V---------------view_p28_contactpersons-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p28_contactpersons') and type = 'V')
 drop view view_p28_contactpersons
GO






CREATE VIEW [dbo].[view_p28_contactpersons]
as
SELECT a.p28ID,min(j02.j02LastName+' '+j02.j02FirstName) as Person
FROM p30Contact_Person a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
GROUP BY a.p28ID


GO

----------V---------------view_p28_contactpersons_invoice-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p28_contactpersons_invoice') and type = 'V')
 drop view view_p28_contactpersons_invoice
GO




CREATE VIEW [dbo].[view_p28_contactpersons_invoice]
as
SELECT b.p28ID,a.j02LastName+' '+a.j02FirstName as Person
FROM j02Person a INNER JOIN p28Contact b ON a.j02ID=b.j02ID_ContactPerson_DefaultInInvoice
WHERE b.j02ID_ContactPerson_DefaultInInvoice IS NOT NULL




GO

----------V---------------view_p28_projects-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p28_projects') and type = 'V')
 drop view view_p28_projects
GO




CREATE VIEW [dbo].[view_p28_projects]
as
SELECT b.p28ID, COUNT(*) as PocetOtevrenychProjektu
FROM p41Project a INNER JOIN p28Contact b ON a.p28ID_Client=b.p28ID
WHERE a.p28ID_Client IS NOT NULL AND getdate() between a.p41ValidFrom AND a.p41ValidUntil
GROUP BY b.p28ID




GO

----------V---------------view_p28_tree_recalc-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p28_tree_recalc') and type = 'V')
 drop view view_p28_tree_recalc
GO




CREATE VIEW [dbo].[view_p28_tree_recalc]
as

WITH rst AS (
  SELECT p28ID
  ,p28ParentID
  ,p28Name
  ,0 AS TreeLevel
  ,CAST(p28name+convert(varchar(10),p28id) AS VARCHAR(255)) AS TreePath
  ,CAST(isnull(p28CompanyShortName,p28Name) AS VARCHAR(255)) AS TreePathAlias
  FROM p28Contact T1
  WHERE p28ParentID IS NULL

  UNION ALL

  SELECT T2.p28ID
  ,T2.p28ParentID
  ,T2.p28Name
  ,TreeLevel + 1
  ,CAST(TreePath + '.' + CAST(T2.p28Name+convert(varchar(10),isnull(T2.p28ParentID,T2.p28ID)) AS VARCHAR(255)) AS VARCHAR(255)) AS TreePath
  ,CAST(TreePathAlias + ' -> ' + CAST(isnull(T2.p28CompanyShortName,T2.p28Name) AS VARCHAR(255)) AS VARCHAR(255)) AS TreePathAlias
  FROM p28Contact T2
  INNER JOIN rst itms ON itms.p28ID = T2.p28ParentID
)
SELECT ROW_NUMBER() OVER (ORDER BY TreePath) as TreeIndex
,p28ID
,p28ParentID
, TreeLevel
,TreePathAlias
FROM  rst 

--, Replicate('.', TreeLevel * 4)+p28Name as Name







GO

----------V---------------view_p31_ocas-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p31_ocas') and type = 'V')
 drop view view_p31_ocas
GO





CREATE VIEW [dbo].[view_p31_ocas]
as
SELECT a.p31ID
,case when p32.p32IsBillable=1 then a.p31Amount_WithoutVat_Orig end as Vykazano_Vynos
,case when p34.p34IncomeStatementFlag=1 and p34.p33ID IN (2,5) then a.p31Amount_WithoutVat_Orig when p34.p33ID=1 THEN p31Rate_Internal_Orig*p31Hours_Orig end as Vykazano_Naklad
,isnull(case when p32.p32IsBillable=1 then a.p31Amount_WithoutVat_Orig end,0)-isnull(case when p34.p34IncomeStatementFlag=1 and p34.p33ID IN (2,5) then a.p31Amount_WithoutVat_Orig when p34.p33ID=1 THEN p31Rate_Internal_Orig*p31Hours_Orig end,0) as Vykazano_Zisk
,case when a.p91ID IS NOT NULL THEN a.p31Amount_WithoutVat_Invoiced END as Vyfakturovano_Vynos
,case when a.p91ID IS NOT NULL THEN a.p31Amount_WithoutVat_Invoiced_Domestic END as Vyfakturovano_Vynos_Domestic
,case when a.p91ID IS NOT NULL THEN isnull(a.p31Amount_WithoutVat_Invoiced_Domestic,0)-isnull(case when p34.p34IncomeStatementFlag=1 and p34.p33ID IN (2,5) then a.p31Amount_WithoutVat_Orig when p34.p33ID=1 THEN p31Rate_Internal_Orig*p31Hours_Orig end,0) END as Vyfakturovano_Zisk
FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID





GO

----------V---------------view_p41_tree_recalc-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p41_tree_recalc') and type = 'V')
 drop view view_p41_tree_recalc
GO











CREATE VIEW [dbo].[view_p41_tree_recalc]
as

WITH rst AS (
  SELECT p41ID
  ,p41ParentID
  ,p41Name
  ,0 AS TreeLevel
  ,CAST(p41name+convert(varchar(10),p41id) AS VARCHAR(255)) AS TreePath
  ,CAST(isnull(p41NameShort,p41name) AS VARCHAR(255)) AS TreePathAlias
  FROM p41Project T1
  WHERE p41ParentID IS NULL

  UNION ALL

  SELECT T2.p41ID
  ,T2.p41ParentID
  ,T2.p41Name
  ,TreeLevel + 1
  ,CAST(TreePath + '.' + CAST(T2.p41Name+convert(varchar(10),isnull(T2.p41ParentID,T2.p41ID)) AS VARCHAR(255)) AS VARCHAR(255)) AS TreePath
  ,CAST(TreePathAlias + ' -> ' + CAST(isnull(T2.p41NameShort,T2.p41Name) AS VARCHAR(255)) AS VARCHAR(255)) AS TreePathAlias
  FROM p41Project T2
  INNER JOIN rst itms ON itms.p41ID = T2.p41ParentID
)
SELECT ROW_NUMBER() OVER (ORDER BY TreePath) as TreeIndex
,p41ID
,p41ParentID
, TreeLevel
,TreePathAlias
FROM  rst 

--, Replicate('.', TreeLevel * 4)+p41Name as Name






GO

----------V---------------view_p41_wip-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p41_wip') and type = 'V')
 drop view view_p41_wip
GO







CREATE VIEW [dbo].[view_p41_wip]
as
select a.p41ID
,sum(a.p31Hours_Orig) as Hodiny
,sum(case when a.p31Amount_WithoutVat_Orig<>0 then p31Amount_WithoutVat_Orig end) as Castka_Celkem
,sum(case when p34.p33ID=1 then p31Amount_WithoutVat_Orig end) as Honorar
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=2 then p31Amount_WithoutVat_Orig end) as Honorar_CZK
,sum(case when p34.p33ID=1 and a.j27ID_Billing_Orig=3 then p31Amount_WithoutVat_Orig end) as Honorar_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 then p31Amount_WithoutVat_Orig end) as Vydaje
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=2 then p31Amount_WithoutVat_Orig end) as Vydaje_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 AND a.j27ID_Billing_Orig=3 then p31Amount_WithoutVat_Orig end) as Vydaje_EUR
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 then p31Amount_WithoutVat_Orig end) as Odmeny
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=2 then p31Amount_WithoutVat_Orig end) as Odmeny_CZK
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 and a.j27ID_Billing_Orig=3 then p31Amount_WithoutVat_Orig end) as Odmeny_EUR
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p71ID IS NULL AND getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.p41ID




GO

----------V---------------view_p41_worksheet-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p41_worksheet') and type = 'V')
 drop view view_p41_worksheet
GO







CREATE VIEW [dbo].[view_p41_worksheet]
as
select a.p41ID
,sum(a.p31Hours_Orig) as Vykazano_Hodiny
,sum(case when a.p70ID=4 then a.p31Hours_Invoiced end) as Vyfakturovano_Hodiny
,sum(a.p31Amount_WithoutVat_Invoiced_Domestic) as Vyfakturovano_Celkem_Domestic
,sum(case when p34.p33ID=1 THEN a.p31Amount_WithoutVat_Invoiced_Domestic END) as Vyfakturovano_Hodiny_Domestic
,sum(case when p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN a.p31Amount_WithoutVat_Invoiced_Domestic END) as Vyfakturovano_Odmeny_Domestic
from
p31WorkSheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE getdate() between a.p31ValidFrom and a.p31ValidUntil
GROUP BY a.p41ID




GO

----------V---------------view_p91_sendbyemail-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_p91_sendbyemail') and type = 'V')
 drop view view_p91_sendbyemail
GO





CREATE VIEW [dbo].[view_p91_sendbyemail]
as
select a.p91ID
,x40.x40WhenProceeded as Kdy_Odeslano
,x40.x40DateInsert as Kdy_Zahajeno
,case when x40.x40State=3 then 'Odesláno' when x40.x40State=1 then 'Odesílá se' when x40.x40State=2 then 'Chyba' when x40.x40State=4 then 'Zastaveno' end as AktualniStav
,x40.x40Recipient as Komu
from
p91Invoice a
INNER JOIN x40MailQueue x40 ON a.p91ID=x40.x40RecordPID
WHERE x40.x40ID IN (select max(x40ID) FROM x40MailQueue WHERE x29ID=391 GROUP BY x40RecordPID)






GO

----------V---------------view_PrimaryAddress-------------------------

if exists (select 1 from sysobjects where  id = object_id('view_PrimaryAddress') and type = 'V')
 drop view view_PrimaryAddress
GO






CREATE VIEW [dbo].[view_PrimaryAddress]
as
SELECT o37.p28ID,o38prim.*
FROM o38Address o38prim INNER JOIN o37Contact_Address o37 ON o38prim.o38ID=o37.o38ID
WHERE o37.o36ID=1



GO

