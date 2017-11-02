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