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