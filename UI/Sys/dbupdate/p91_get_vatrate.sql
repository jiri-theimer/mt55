if exists (select 1 from sysobjects where  id = object_id('p91_get_vatrate') and type = 'FN')
 drop function p91_get_vatrate
GO




CREATE  FUNCTION [dbo].[p91_get_vatrate](@x15id int,@j27id int,@j17id int,@dat datetime)
RETURNS float
AS
BEGIN
  ---2: snížená, 3: standardní, 4: special

  declare @ret float,@p53id int

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID=@j17id and x15id=@x15id and @dat BETWEEN p53validfrom AND p53validuntil ORDER BY p53ValidFrom DESC
  

  if @p53id is null
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID is null and x15id=@x15id and @dat BETWEEN p53validfrom AND p53validuntil ORDER BY p53ValidFrom DESC

  
  if @p53id is not null
    RETURN(@ret)
    
  
  RETURN(0)
   
END

GO