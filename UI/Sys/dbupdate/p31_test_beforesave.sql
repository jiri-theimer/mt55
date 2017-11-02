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
 set @err='Na vstupu chyb� aktivita.'

if @p41id is null
 set @err='Na vstupu chyb� projekt.'

if @j02id_rec is null
 set @err='Na vstupu chyb� osoba.'

if @err<>''
 return

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND p41WorksheetOperFlag=1)
 set @err='Projekt je uzav�en� pro zapisov�n� worksheet �kon�.'

if exists(select p41ID FROM p41Project WHERE p41ID=@p41id AND (p41ValidFrom>getdate() OR p41ValidUntil<getdate()))
 set @err='Projekt byl p�esunut do ko�e, nelze do n�j zapisovat worksheet �kony.'

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
  set @err='Pro aktivitu ['+@p32name+'] je povinn� zad�vat podrobn� popis �konu.'
  return
 end

if @p32Value_Minimum<>0 and @value_orig<=@p32Value_Minimum
 begin
  set @err='Pro aktivitu ['+@p32name+'] mus� b�t vyk�zan� hodnota v�t�� ne�: '+convert(varchar(10),@p32Value_Minimum)
  if @p33id=1
   set @err=@err+'h.'

  return
 end

if @p32Value_Maximum<>0 and @value_orig>=@p32Value_Maximum
 begin
  set @err='Pro aktivitu ['+@p32name+'] mus� b�t vyk�zan� hodnota men�� ne�: '+convert(varchar(10),@p32Value_Maximum)
  if @p33id=1
   set @err=@err+'h.'

  return
 end

---test sazby DPH------------
if @p33id=5 ----testuje se pouze money se�it s pln�m rozpisem DPH
 begin
  declare @vatisok bit
  
  select @vatisok=dbo.p31_testvat(@p31vatrate_orig,@p41id,@p31date,@j27id_explicit)
  
  if @vatisok=0
   set @err='Sazba DPH ['+convert(varchar(30),@p31vatrate_orig)+'%] nen� povolena pro tento projekt, m�nu a obdob�!'
 

 end

if @err<>''
 return

if @p33id=1 or @p33id=3 or @p33id=2  ----pro �as a kusovn�k a ��stku bez rozpisu
 select @vatrate=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)



if @isplan=0
 begin
  --test uzam�en�ho obdob�-----------
  exec p31_test_lockedperiod @j03id_sys,@p31date,@j02id_rec, @p34id, @islocked OUTPUT 


  if @islocked=1
    set @err='Datum ['+convert(varchar(30),@p31date,104)+'] pat�� do uzam�en�ho obdob�!'
 end
 
if @err<>''
 return
   
---test opr�vn�n� zapisovat do projektu worksheet---------
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
 begin ----------opr�vn�n� k projektu podle projektov� skupiny (regionu)
  select @o28id=a.o28id,@o28entryflag=a.o28entryflag
  from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
  inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  where a.p34ID=@p34id AND x69.x69RecordPID=@j18id AND x67.x29ID=118
  and (isnull(x69.j02ID,0)=@j02id_rec OR isnull(x69.j07ID,0)=@j07id_rec OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec))
  AND (a.o28entryflag>0)
  
 end

if @o28id is null
 begin  
  set @err='Osoba ['+@person+'] nem� v tomto projektu nebo v p��slu�n� projektov� skupin� p�i�azenou roli k zapisov�n� worksheet �kon� do se�itu ['+dbo.GetObjectAlias('p34',@p34id)+']'

 end
   
if @err<>''
 return  
 
if @o28entryflag=1
 return	--OK - pr�vo zapisovat do projektu i v�ech �loh 
 
if isnull(@o28entryflag,0)=0
 begin
   set @err='Projektov� role osoby ['+@person+'] nem� povoleno zapisovat worksheet do zvolen�ho se�itu'
   return
 end
 
if @o28entryflag=2 and @p56id is null
 return	  --OK - pr�vo zapisovat do projektu p��mo nebo do �loh s WR
 
if @o28entryflag=3 and @p56id is null
 begin
   set @err='Projektov� role osoby ['+@person+'] m� povoleno zapisovat pouze do projektov�ch �loh.'
   return
 end 
 
if @p56id is null
 begin
   set @err='Mus�te vybrat �lohu.'
   return
 end 

--situace, kdy se zapisuje �kon do �lohy a osoba nem� pr�vo zapisovat do projektu 
SELECT @x69id=a.x69ID
from x69EntityRole_Assign a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
WHERE a.x69RecordPID=@p56id and x67.x29ID=356
AND (
 isnull(a.j02ID,0)=@j02id_rec
 OR isnull(a.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_rec)
 OR isnull(a.j07ID,0)=@j07id_rec
)
  

if @x69id is null 
  set @err='Nejste �e�itelem �lohy ['+dbo.GetObjectAlias('p56',@p56id)+'] a proto nem��ete zapisovat worksheet do zvolen�ho se�itu.'
  
 


GO