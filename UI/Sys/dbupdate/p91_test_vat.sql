if exists (select 1 from sysobjects where  id = object_id('p91_test_vat') and type = 'FN')
 drop function p91_test_vat
GO




CREATE  FUNCTION [dbo].[p91_test_vat](@vatrate float,@j27id int,@j17id int,@dat datetime)
RETURNS bit
AS
BEGIN
  ---2: snížená, 3: standardní, 4: special

  declare @p53id int

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id AND j17ID=@j17id and p53Value=@vatrate and @dat BETWEEN p53validfrom AND p53validuntil
  

  if @p53id is null
    select top 1 @p53id=p53id from p53VatRate where j27ID=@j27id AND j17ID is null and p53Value=@vatrate and @dat BETWEEN p53validfrom AND p53validuntil

  
  if @p53id is null
    RETURN(0)
  

  RETURN(1)


   
   
END

GO