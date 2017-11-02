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