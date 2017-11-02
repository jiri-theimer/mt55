if exists (select 1 from sysobjects where  id = object_id('j03user_load_sysuser') and type = 'P')
 drop procedure j03user_load_sysuser
GO




CREATE   procedure [dbo].[j03user_load_sysuser]
@login nvarchar(50)

AS

declare @j03id int,@j02id int,@j04id int,@personal_page varchar(200),@personal_page_mobile varchar(200)
declare @j03Cache_TimeStamp datetime,@j03Cache_MessagesCount int,@j03Cache_IsApprovingPerson bit

select @j03id=a.j03ID,@j02id=a.j02ID,@j04id=a.j04id,@personal_page=b.j04Aspx_PersonalPage,@personal_page_mobile=b.j04Aspx_PersonalPage_Mobile
,@j03Cache_TimeStamp=isnull(a.j03Cache_TimeStamp,convert(datetime,'01.01.2000',104)),@j03Cache_MessagesCount=isnull(a.j03Cache_MessagesCount,0),@j03Cache_IsApprovingPerson=a.j03Cache_IsApprovingPerson
FROM j03user a INNER JOIN j04UserRole b on a.j04ID=b.j04ID
WHERE a.j03Login=@login

if datediff(minute,@j03Cache_TimeStamp,getdate())>5
 begin
	exec dbo.j03_recovery_cache @j03id,@j02id

	select @j03Cache_MessagesCount=j03Cache_MessagesCount,@j03Cache_IsApprovingPerson=j03Cache_IsApprovingPerson FROM j03User WHERE j03ID=@j03id
 end

declare @appscope varchar(255),@is_dropbox bit

select @appscope=convert(varchar(255),x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'AppScope'

set @is_dropbox=0
if exists(select x35ID FROM x35GlobalParam WHERE x35Key='Dropbox_IsUse' AND x35Value='1')
 set @is_dropbox=1

declare @is_master bit
set @is_master=0

if exists(select j05ID FROM j05MasterSlave WHERE j02ID_Master=@j02id)
 set @is_master=1

select a.*,a.j03id as _pid
,j04.j04name as _j04Name,j02.j02LastName as _j02LastName,j02.j02FirstName as _j02FirstName,j02.j02TitleBeforeName as _j02TitleBeforeName,j02.j02Email as _j02Email
,a.j03dateupdate as _dateupdate,a.j03dateinsert as _dateinsert,a.j03userupdate as _userupdate,a.j03userinsert as _userinsert
,a.j03validfrom as _validfrom,a.j03validuntil as _validuntil,a.j03IsLiveChatSupport,a.j03SiteMenuSkin,a.j03IsSiteMenuOnClick
,@appscope as _AppScopeCrypted,x67.x67RoleValue as _RoleValue
,j02.j02Email as _j02Email,@j03Cache_IsApprovingPerson as _IsApprovingPerson,@is_master as _IsMasterPerson,@j03Cache_MessagesCount as j03Cache_MessagesCount
,@is_dropbox as _IsDropbox,@j03Cache_MessagesCount as _MessagesCount
,case when a.j03Aspx_PersonalPage IS NULL THEN @personal_page ELSE a.j03Aspx_PersonalPage END as _PersonalPage
,j04.j04IsMenu_Worksheet,j04.j04IsMenu_Report,j04.j04IsMenu_Project,j04.j04IsMenu_People,j04.j04IsMenu_Contact,j04.j04IsMenu_Invoice,j04.j04IsMenu_Proforma
,j04.j04Aspx_OneProjectPage as OneProjectPage,j04.j04Aspx_OneContactPage as OneContactPage,j04.j04Aspx_OneInvoicePage as OneInvoicePage,j04.j04Aspx_OnePersonPage as OnePersonPage
FROM j03user a INNER JOIN j04userrole j04 on a.j04id=j04.j04id LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id
INNER JOIN x67EntityRole x67 ON j04.x67ID=x67.x67ID
WHERE a.j03ID=@j03id























GO