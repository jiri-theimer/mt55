if exists (select 1 from sysobjects where  id = object_id('j03_recovery_cache') and type = 'P')
 drop procedure j03_recovery_cache
GO




CREATE   procedure [dbo].[j03_recovery_cache]
@j03id int,@j02id int

AS

declare @p56_count int,@o22_count int,@o23_count int,@p39_count int,@d1 datetime,@d2 datetime,@is_approve bit,@x67id int,@login varchar(50)

set @d1=dateadd(day,-1,getdate())
set @d2=dateadd(day,1,getdate())
set @is_approve=0

select @x67id=b.x67ID,@login=a.j03Login
FROM j03user a INNER JOIN j04UserRole b on a.j04ID=b.j04ID
WHERE a.j03ID=@j03id


if exists(SELECT x68ID FROM x68EntityRole_Permission WHERE x67ID=@x67id AND x53ID=58)
 set @is_approve=1	--mùže paušálnì schvalovat veškerý worksheet, oprávnìní x53ID=58: Oprávnìní schvalovat všechny worksheet úkony v databázi

if @is_approve=0
begin
 if exists(SELECT TOP 1 a.x67ID FROM x67EntityRole a INNER JOIN x69EntityRole_Assign x69 ON a.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON a.x67ID=o28.x67ID WHERE getdate() BETWEEN a.x67ValidFrom AND a.x67ValidUntil AND a.x29ID=141 AND o28.o28PermFlag IN (3,4) AND (x69.j02ID=@j02id OR x69.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id)))
  set @is_approve=1	---má oprávnìní schvalovat worksheet v minimálnì jednom projektu

end


---poèet úkolù
select @p56_count=count(p56ID) FROM p56Task
WHERE (p56PlanUntil BETWEEN @d1 AND @d2 OR p56ReminderDate between @d1 AND @d2)
AND (j02ID_Owner=@j02id OR p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))))
 

---poèet událostí
select @o22_count=count(o22ID) FROM o22Milestone
WHERE (o22DateFrom BETWEEN @d1 AND @d2 OR o22ReminderDate BETWEEN @d1 AND @d2)
AND (j02ID_Owner=@j02id OR j02ID=@j02id OR o22ID IN (SELECT o22ID FROM o20Milestone_Receiver WHERE j02ID=@j02id OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id)))

--poèet poznámek
select @o23_count=count(o23ID) FROM o23Notepad
WHERE (o23ReminderDate BETWEEN @d1 AND @d2)
AND (j02ID_Owner=@j02id OR j02ID=@j02id)


----poèet auto-generovaných odmìn/paušálù/úkonù
select @p39_count=count(a.p39ID) FROM p39WorkSheet_Recurrence_Plan a INNER JOIN p40WorkSheet_Recurrence b ON a.p40ID=b.p40ID
WHERE (b.j02ID=@j02id or b.p40UserInsert=@login) AND a.p39DateCreate BETWEEN @d1 AND @d2

update j03User set j03Cache_TimeStamp=getdate()
,j03Cache_MessagesCount=isnull(@p56_count,0)+isnull(@o22_count,0)+isnull(@p39_count,0)+isnull(@o23_count,0)
,j03Cache_IsApprovingPerson=@is_approve
WHERE j03ID=@j03id



















GO