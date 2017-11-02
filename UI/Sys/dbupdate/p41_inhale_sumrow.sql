if exists (select 1 from sysobjects where  id = object_id('p41_inhale_sumrow') and type = 'P')
 drop procedure p41_inhale_sumrow
GO





CREATE procedure [dbo].[p41_inhale_sumrow]
@j03id_sys int
,@pid int						---p41id
,@p56_actual_count int OUTPUT	--_poèet otevøených úkolù v projektu
,@o22_actual_count int OUTPUT	--poèet otevøených termínù v projektu
,@p91_count int OUTPUT			--poèet vystavených faktur
AS

set @p56_actual_count=0
set @o22_actual_count=0
set @p91_count=0


SELECT @p56_actual_count=COUNT(*)
FROM p56Task
WHERE p41ID=@pid AND getdate() BETWEEN p56ValidFrom AND p56ValidUntil

SELECT @o22_actual_count=COUNT(*)
FROM o22Milestone
WHERE p41ID=@pid AND o22DateUntil>=dateadd(day,-2,getdate()) AND getdate() BETWEEN o22ValidFrom AND o22ValidUntil

SELECT @p91_count=COUNT(*)
from p91Invoice
WHERE p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p41ID=@pid)

GO