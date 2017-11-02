if exists (select 1 from sysobjects where  id = object_id('x37_save') and type = 'P')
 drop procedure x37_save
GO








CREATE   procedure [dbo].[x37_save]
@j03id int					--id u�ivatele
,@page varchar(50)		--n�zev aspx str�nky
,@dockstate nvarchar(MAX)	--ulo�en� dockstate

AS

--Ulo�en� z�znamu u�ivatelem nastaven�ho stavu RadDock na str�nce @page

if @j03id is null
  return

if @page is null
  return

if not exists(SELECT x37ID FROM x37SavedDockState WHERE j03ID=@j03id AND x37Page=@page)
 INSERT INTO x37SavedDockState(j03ID,x37Page,x37DockState,x37UserInsert) VALUES(@j03id,@page,@dockstate,dbo.j03_getlogin(@j03id))
else
 UPDATE x37SavedDockState set x37DockState=@dockstate,x37DateUpdate=getdate(),x37UserUpdate=dbo.j03_getlogin(@j03id) WHERE j03ID=@j03id AND x37Page=@page































GO