if exists (select 1 from sysobjects where  id = object_id('o28_get_permflag') and type = 'FN')
 drop function o28_get_permflag
GO




CREATE  FUNCTION [dbo].[o28_get_permflag](@j02id int,@p41id int,@j18id int,@p34id int,@minpermflag int,@maxpermflag int)
RETURNS int
AS
BEGIN
   
declare @o28id int,@o28permflag int	---0-pouze vlastn� worksheet,1-��st v�e v r�mci projektu, 2-��st a upravovat v�e v r�mci projektu,3-��st a schvalovat v�e v r�mci projektu 4 - ��st, upravovat a schvalovat v�e v r�mci projektu

select TOP 1 @o28id=a.o28id,@o28permflag=a.o28PermFlag
from o28ProjectRole_Workload a inner join x67EntityRole x67 on a.x67ID=x67.x67ID
inner join x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
where a.p34ID=@p34id AND x69.x69RecordPID=@p41id AND x67.x29ID=141
and (isnull(x69.j02ID,0)=@j02id OR isnull(x69.j11ID,0) IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))
AND a.o28entryflag>=@minpermflag AND a.o28PermFlag<=@maxpermflag
ORDER BY a.o28PermFlag DESC
	
if @o28id is null and @j18id is not null
	begin ----------opr�vn�n� k projektu podle projektov� skupiny (regionu)
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