if exists (select 1 from sysobjects where  id = object_id('p31_testvat') and type = 'FN')
 drop function p31_testvat
GO




CREATE   FUNCTION [dbo].[p31_testvat](@vatrate float,@p41id int,@dat datetime,@j27id_explicit int)
RETURNS bit
AS
BEGIN
  if @j27id_explicit=0
   set @j27id_explicit=null

  declare @ret bit,@p53id int,@j18id int,@p51id int,@j17id int

  select @j18id=j18id,@p51id=p51ID_Billing from p41project where p41id=@p41id

  if @j18id is not null
   select @j17id=j17ID FROM j18Region WHERE j18ID=@j18id

  if @p51id is not null and @j27id_explicit is null
   select @j27id_explicit=j27ID FROM p51PriceList WHERE p51ID=@p51id
  
  if @j27id_explicit is null
   select @j27id_explicit=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic' AND ISNUMERIC(x35Value)=1


  if @j17id is not null
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id_explicit AND p53validfrom<=@dat AND p53validuntil>=@dat AND (j17ID=@j17id or j17ID is null) and p53value=@vatrate
  else
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id_explicit AND p53validfrom<=@dat AND p53validuntil>=@dat AND p53value=@vatrate

  if @p53id is null
    set @ret=0
  else
    set @ret=1



  RETURN(@ret)
   
END



GO