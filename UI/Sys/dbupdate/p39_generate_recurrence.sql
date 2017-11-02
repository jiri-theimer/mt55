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
	  set @p39text=@p40text
	
	  set @p39text=replace(@p39text,'[MM]',right('0'+convert(varchar(10),month(@p39date)),2))
	  set @p39text=replace(@p39text,'[DD]',right('0'+convert(varchar(10),day(@p39date)),2))
	  set @p39text=replace(@p39text,'[YYYY]',convert(varchar(10),year(@p39date)))
	  set @p39text=replace(@p39text,'[WW]',right('0'+convert(varchar(10),datepart(week,@p39date)),2))
	  set @p39text=replace(@p39text,'[QQ]',right('0'+convert(varchar(10),datepart(quarter,@p39date)),2))
	  set @p39text=replace(@p39text,'[Q]',convert(varchar(10),datepart(quarter,@p39date)))
	  set @p39text=replace(@p39text,'[M]',convert(varchar(10),month(@p39date)))

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