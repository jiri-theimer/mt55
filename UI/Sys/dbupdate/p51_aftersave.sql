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