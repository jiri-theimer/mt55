if exists (select 1 from sysobjects where  id = object_id('remove_alphacharacters') and type = 'FN')
 drop function remove_alphacharacters
GO


CREATE FUNCTION [dbo].[remove_alphacharacters](@InputString VARCHAR(1000),@leading_zeros_scale int)
RETURNS VARCHAR(1000)
AS
BEGIN
  WHILE PATINDEX('%[^0-9]%',@InputString)>0
        SET @InputString = STUFF(@InputString,PATINDEX('%[^0-9]%',@InputString),1,'')  		  
		
    
  
  IF @leading_zeros_scale>0
   begin
    
	 RETURN right('00000000000000000000000'+@InputString,@leading_zeros_scale)
   end

   RETURN @InputString
END


GO