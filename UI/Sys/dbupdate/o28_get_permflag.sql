if exists (select 1 from sysobjects where  id = object_id('o28_get_permflag') and type = 'FN')
 drop function o28_get_permflag
GO




CREATE  FUNCTION [dbo].[o28_get_permflag](@j02id int,@p41id int,@j18id int,@p34id int,@minpermflag int,@maxpermflag int)
RETURNS int
AS
BEGIN
   
declare @o28id int,@o28permflag int	---0-pouze vlastní worksheet,1-Èíst vše v rámci projektu, 2-Èíst a upravovat vše v rámci projektu,3-Èíst a schvalovat vše v rámci projektu 4 - Èíst, upravovat a schvalovat vše v rámci projektu

select TOP 1 @o28id=a.o28id,@o28permflag=a.o28PermFlag
from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
where a.p34ID=@p34id AND x69.x69RecordPID=@p41id AND x67.x29ID=141
and (isnull(x69.j02ID,0)=@j02id OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))
AND a.o28entryflag>=@minpermflag AND a.o28PermFlag<=@maxpermflag
ORDER BY a.o28PermFlag DESC
	
if @o28id is null and @j18id is not null
	begin ----------oprávnìní k projektu podle projektové skupiny (regionu)
		select TOP 1 @o28id=a.o28id,@o28permflag=a.o28PermFlag
		from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
		inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
		where a.p34ID=@p34id AND x69.x69RecordPID=@j18id AND x67.x29ID=118
		and (isnull(x69.j02ID,0)=@j02id OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))
		AND a.o28entryflag>=@minpermflag AND a.o28PermFlag<=@maxpermflag
		ORDER BY a.o28PermFlag DESC
  
	end


RETURN isnull(@o28permflag,0)

END




GO