if exists (select 1 from sysobjects where  id = object_id('j02_recovery') and type = 'P')
 drop procedure j02_recovery
GO








CREATE PROCEDURE [dbo].[j02_recovery]
AS

declare @j11id_all int

select @j11id_all=j11ID FROM j11Team WHERE j11IsAllPersons=1

if @j11id_all is null
 BEGIN
  INSERT INTO j11Team(j11IsAllPersons,j11Name,j11UserInsert,j11UserUpdate,j11DateInsert,j11DateUpdate) VALUES(1,'Všechny osoby','recovery','recovery',getdate(),getdate())

  set @j11id_all=@@IDENTITY
 END


 INSERT INTO j12Team_Person(j11ID,j02ID)
 SELECT @j11id_all,j02ID
 FROM j02Person
 WHERE j02IsIntraPerson=1 AND j02ID NOT IN (SELECT j02ID FROM j12Team_Person where j11ID=@j11id_all)



	
	




GO