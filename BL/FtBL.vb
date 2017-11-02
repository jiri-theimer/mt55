Public Interface IFtBL
    Inherits IFMother
    Function GetList_X53(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x53Permission)
    Function GetList_X29(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x29Entity)
    Function GetList_X15(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x15VatRateType)
    Function GetList_X21(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x21DatePeriod)
    Function GetList_P71(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p71ApproveStatus)
    Function GetList_P70(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p70BillingStatus)
    Function GetList_P72(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p72PreBillingStatus)
    Function GetList_P33(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p33ActivityInputType)
    Function GetList_P35(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p35Unit)
    Function GetList_X24(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x24DataType)
    Function GetList_P87(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p87BillingLanguage)
    Function GetList_j19(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j19PaymentType)

    Function SaveP87(lisP87 As List(Of BO.p87BillingLanguage)) As Boolean
    Function LoadP87(intP87ID As Integer) As BO.p87BillingLanguage
    Function LoadJ27(intJ27ID As Integer) As BO.j27Currency
    Function GetList_J27(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j27Currency)
    Function GetList_C11(datFrom As Date, datUntil As Date, levelFrom As BO.PeriodLevel, levelUntil As BO.PeriodLevel) As IEnumerable(Of BO.c11StatPeriod)
    Function GetList_X21_NonDB(bolIncludeFuture As Boolean, bolEnglish As Boolean) As List(Of BO.x21DatePeriod)
    Function LoadX45(intX45ID As Integer) As BO.x45Event
    Function GetList_X45(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x45Event)
    Function GetList_X61(x29id As BO.x29IdEnum) As IEnumerable(Of BO.x61PageTab)
    Function GetList_Emails(strFilterExpression As String, intTOP As Integer) As IEnumerable(Of BO.GetString)
    Function FulltextSearch(input As BO.FullTextQueryInput) As List(Of BO.FullTextRecord)
    Function AppendRobotLog(cRec As BO.j91RobotLog) As Boolean
    Function GetLastRobotRun(TaskFlag As BO.j91RobotTaskFlag) As BO.j91RobotLog
    Function GetChangeLog(strPrefix As String, intRecordPID As Integer) As DataTable
    Function get_ParsedText_With_Period(strExpression As String, dat As Date, intPeriodIndex As Integer) As String
End Interface
Class FtBL
    Inherits BLMother
    Implements IFtBL
    Private WithEvents _cDL As DL.FtDL
    Private _English As Boolean = False

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.FtDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function GetList_X24(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x24DataType) Implements IFtBL.GetList_X24
        Return _cDL.GetList_X24(mq)
    End Function
    Public Function GetList_P33(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p33ActivityInputType) Implements IFtBL.GetList_P33
        Return _cDL.GetList_P33(mq)
    End Function
    Public Function GetList_P35(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p35Unit) Implements IFtBL.GetList_P35
        Return _cDL.GetList_P35(mq)
    End Function
    Public Function GetList_P72(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p72PreBillingStatus) Implements IFtBL.GetList_P72
        Return _cDL.GetList_P72(mq)
    End Function
    Public Function GetList_P70(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p70BillingStatus) Implements IFtBL.GetList_P70
        Return _cDL.GetList_P70(mq)
    End Function
    Public Function GetList_P71(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p71ApproveStatus) Implements IFtBL.GetList_P71
        Return _cDL.GetList_P71(mq)
    End Function
    
    Public Function GetList_X15(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x15VatRateType) Implements IFtBL.GetList_X15
        Return _cDL.GetList_X15(mq)
    End Function
    Public Function GetList_X29(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x29Entity) Implements IFtBL.GetList_X29
        Return _cDL.GetList_X29(mq)
    End Function
    Public Function GetList_X53(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x53Permission) Implements IFtBL.GetList_X53
        Return _cDL.GetList_X53(mq)
    End Function
    Public Function GetList_P87(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p87BillingLanguage) Implements IFtBL.GetList_P87
        Return _cDL.GetList_P87(mq)
    End Function

    Public Function SaveP87(lisP87 As List(Of BO.p87BillingLanguage)) As Boolean Implements IFtBL.SaveP87
        For Each c In lisP87
            If Trim(c.p87Name) = "" Then
                _Error = BO.BAS.GLX("Jazyk č. %1% nemá uvedený název.", c.p87LangIndex.ToString) : Return False
            End If
            If Len(c.p87Name) > 20 Then
                _Error = BO.BAS.GLX("Výraz '%1%' je příliš dlouhý.", c.p87Name) : Return False
            End If
        Next
        _cDL.SaveP87(lisP87)
        Return True
    End Function
    Public Function LoadP87(intP87ID As Integer) As BO.p87BillingLanguage Implements IFtBL.LoadP87
        Return _cDL.LoadP87(intP87ID)
    End Function

    Public Function GetList_J27(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j27Currency) Implements IFtBL.GetList_J27
        Return _cDL.GetList_J27(mq)
    End Function

    Public Function GetList_C11(datFrom As Date, datUntil As Date, levelFrom As BO.PeriodLevel, levelUntil As BO.PeriodLevel) As IEnumerable(Of BO.c11StatPeriod) Implements IFtBL.GetList_C11
        Return _cDL.GetList_C11(datFrom, datUntil, levelFrom, levelUntil)
    End Function
    Public Function LoadJ27(intJ27ID As Integer) As BO.j27Currency Implements IFtBL.LoadJ27
        Return _cDL.LoadJ27(intJ27ID)
    End Function

    Public Function GetList_X21(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x21DatePeriod) Implements IFtBL.GetList_X21
        Return _cDL.GetList_X21(mq)
    End Function
    Public Function GetList_j19(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j19PaymentType) Implements IFtBL.GetList_j19
        Return New DL.j19PaymentTypeDL(_cUser).GetList(mq)
    End Function
    Public Function GetList_X21_NonDB(bolIncludeFuture As Boolean, bolEnglish As Boolean) As List(Of BO.x21DatePeriod) Implements IFtBL.GetList_X21_NonDB
        _English = bolEnglish
        Dim lis As New List(Of BO.x21DatePeriod)

        With lis
            .Add(AC(BO.x21IdEnum._NoQuery))
            .Add(AC(BO.x21IdEnum.Vcera))
            .Add(AC(BO.x21IdEnum.Dnes))
            .Add(AC(BO.x21IdEnum.TydenMinuly))
            .Add(AC(BO.x21IdEnum.TydenTento))
            .Add(AC(BO.x21IdEnum.MesicMinus5))
            .Add(AC(BO.x21IdEnum.MesicMinus4))
            .Add(AC(BO.x21IdEnum.MesicMinus3))
            .Add(AC(BO.x21IdEnum.MesicMinus2))

            .Add(AC(BO.x21IdEnum.MesicMinuly))

            .Add(AC(BO.x21IdEnum.MesicTento))
            .Add(AC(BO.x21IdEnum.KvartalMinuly))
            .Add(AC(BO.x21IdEnum.KvartalTento))

            .Add(AC(BO.x21IdEnum.RokTento))
            .Add(AC(BO.x21IdEnum.RokMinuly))
            .Add(AC(BO.x21IdEnum.DoMinulyRok))

            .Add(AC(BO.x21IdEnum.DoMinulyMesic3))
            .Add(AC(BO.x21IdEnum.DoMinulyMesic2))
            .Add(AC(BO.x21IdEnum.DoMinulyMesic))
            .Add(AC(BO.x21IdEnum.DoDnes))

            If bolIncludeFuture Then
                .Add(AC(BO.x21IdEnum.TydenPristi))
                .Add(AC(BO.x21IdEnum.MesicPristi))
                .Add(AC(BO.x21IdEnum.KvartalPristi))
                .Add(AC(BO.x21IdEnum.RokPristi))
            End If
        End With
        Return lis
    End Function
    Private Function AC(x21id As BO.x21IdEnum) As BO.x21DatePeriod
        Dim c As New BO.x21DatePeriod(x21id, _English)
        c.SetPeriod(Today)
        Return c
    End Function
   
    Public Function GetList_X45(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x45Event) Implements IFtBL.GetList_X45
        Return _cDL.GetList_X45(mq)
    End Function
    Public Function LoadX45(intX45ID As Integer) As BO.x45Event Implements IFtBL.LoadX45
        Return _cDL.LoadX45(intX45ID)
    End Function
    Public Function GetList_X61(x29id As BO.x29IdEnum) As IEnumerable(Of BO.x61PageTab) Implements IFtBL.GetList_X61
        Return _cDL.GetList_X61(x29id)
    End Function
    Public Function GetList_Emails(strFilterExpression As String, intTOP As Integer) As IEnumerable(Of BO.GetString) Implements IFtBL.GetList_Emails
        Return _cDL.GetList_Emails(strFilterExpression, intTOP)
    End Function
    Public Function FullTextSearch(input As BO.FullTextQueryInput) As List(Of BO.FullTextRecord) Implements IFtBL.FulltextSearch
        Return _cDL.FullTextSearch(input)
    End Function
    Public Function AppendRobotLog(cRec As BO.j91RobotLog) As Boolean Implements IFtBL.AppendRobotLog
        Return _cDL.AppendRobotLog(cRec)
    End Function
    Public Function GetLastRobotRun(TaskFlag As BO.j91RobotTaskFlag) As BO.j91RobotLog Implements IFtBL.GetLastRobotRun
        Return _cDL.GetLastRobotRun(CInt(TaskFlag))
    End Function
    Public Function GetChangeLog(strPrefix As String, intRecordPID As Integer) As DataTable Implements IFtBL.GetChangeLog
        Return _cDL.GetChangeLog(strPrefix, intRecordPID)
    End Function
    Public Function get_ParsedText_With_Period(strExpression As String, dat As Date, intPeriodIndex As Integer) As String Implements IFtBL.get_ParsedText_With_Period
        Return _cDL.get_ParsedText_With_Period(strExpression, dat, intPeriodIndex)
    End Function
End Class
