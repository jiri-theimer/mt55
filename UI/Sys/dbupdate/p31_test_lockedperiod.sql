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