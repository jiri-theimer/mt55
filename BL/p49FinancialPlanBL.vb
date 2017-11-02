
Public Interface Ip49FinancialPlanBL
    Inherits IFMother
    Function Save(cRec As BO.p49FinancialPlan) As Boolean
    Function SaveBatch(lis As List(Of BO.p49FinancialPlan)) As Boolean
    Function Load(intPID As Integer) As BO.p49FinancialPlan
    Function LoadExtended(intPID As Integer) As BO.p49FinancialPlanExtended
    Function LoadMyLastCreated(intP45ID As Integer) As BO.p49FinancialPlan
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryP49) As IEnumerable(Of BO.p49FinancialPlan)
    Function GetList_Extended(mq As BO.myQueryP49, Optional intP41ID_OptimizeSQL As Integer = 0) As IEnumerable(Of BO.p49FinancialPlanExtended)
    Function TestBeforeSave(lis As List(Of BO.p49FinancialPlan)) As Boolean

End Interface
Class p49FinancialPlanBL
    Inherits BLMother
    Implements Ip49FinancialPlanBL
    Private WithEvents _cDL As DL.p49FinancialPlanDL
   
    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p49FinancialPlanDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p49FinancialPlan) As Boolean Implements Ip49FinancialPlanBL.Save
        Dim lis As New List(Of BO.p49FinancialPlan)
        lis.Add(cRec)

        If Not TestBeforeSave(lis) Then Return False

        Return _cDL.Save(cRec)
    End Function
    Public Function SaveBatch(lis As List(Of BO.p49FinancialPlan)) As Boolean Implements Ip49FinancialPlanBL.SaveBatch
        If Not TestBeforeSave(lis) Then Return False
        For Each c In lis
            If c.IsSetAsDeleted Then
                If c.PID > 0 Then _cDL.Delete(c.PID)
            Else
                _cDL.Save(c)
            End If
        Next
        Return True
    End Function
    Public Function TestBeforeSave(lis As List(Of BO.p49FinancialPlan)) As Boolean Implements Ip49FinancialPlanBL.TestBeforeSave
        If Not TestGlobalPerm() Then Return False
        Dim x As Integer = 1
        For Each c In lis
            With c
                If .p45ID = 0 Then _Error = "Chybí vazba na rozpočet" : Return False
                If .p49Amount = 0 Then _Error = "V záznamu rozpočtu nesmí být nulová částka." : Return False
                If .p34ID = 0 Then _Error = "Chybí vazba na sešit aktivit" : Return False

                If BO.BAS.IsNullDBDate(.p49DateFrom) Is Nothing Or BO.BAS.IsNullDBDate(.p49DateUntil) Is Nothing Then
                    _Error = "Chybí specifikace začátku nebo konce záznamu." : Return False
                Else
                    If .p49DateUntil < .p49DateFrom Then
                        _Error = "Začátek plánu musí být menší než datum konce." : Return False
                    End If
                End If
                If Not IsTestPerm(c) Then
                    If c.IsSetAsDeleted Then
                        _Error = "Nemáte oprávnění odstranit záznam [ID: " & c.PID.ToString & "]<hr> " & _Error
                    Else
                        _Error = "Záznam #" & x.ToString & "[ID: " & c.PID.ToString & "]<hr> " & _Error
                    End If

                    Return False
                End If
            End With
            x += 1
        Next
        Return True
    End Function
    Private Function TestGlobalPerm() As Boolean
        Return True
    End Function
    Private Function IsTestPerm(cRec As BO.p49FinancialPlan) As Boolean
        Return True
    End Function

    Public Function Load(intPID As Integer) As BO.p49FinancialPlan Implements Ip49FinancialPlanBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadExtended(intPID As Integer) As BO.p49FinancialPlanExtended Implements Ip49FinancialPlanBL.LoadExtended
        Return _cDL.LoadExtended(intPID)
    End Function
    Public Function LoadMyLastCreated(intP45ID As Integer) As BO.p49FinancialPlan Implements Ip49FinancialPlanBL.LoadMyLastCreated
        Return _cDL.LoadMyLastCreated(intP45ID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip49FinancialPlanBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQueryP49) As IEnumerable(Of BO.p49FinancialPlan) Implements Ip49FinancialPlanBL.GetList
        Dim lis As IEnumerable(Of BO.p49FinancialPlan) = _cDL.GetList(mq)

        ''If bolIncludeWorksheetSums Then
        ''    Dim mqP31 As New BO.myQueryP31
        ''    mqP31.p41IDs = mq.p41IDs
        ''    mqP31.p34IDs = lis.Select(Function(p) p.p34ID).Distinct.ToList
        ''    Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Me.Factory.p31WorksheetBL.GetList(mqP31)
        ''    Dim cLast As BO.p49FinancialPlan = Nothing

        ''    For Each c In lis
        ''        Dim bolGo As Boolean = True
        ''        If Not cLast Is Nothing Then
        ''            If cLast.p34ID = c.p34ID And cLast.p32ID = c.p32ID And cLast.j02ID = c.j02ID And cLast.p49DateFrom = c.p49DateFrom Then bolGo = False
        ''        End If
        ''        If bolGo Then
        ''            Dim lisQ As IEnumerable(Of BO.p31Worksheet) = lisP31.Where(Function(p) p.p41ID = c.p41ID And p.p34ID = c.p34ID And p.p31Date >= c.p49DateFrom And p.p31Date <= c.p49DateUntil)
        ''            If c.p32ID > 0 Then lisQ = lisQ.Where(Function(p) p.p32ID = c.p32ID)
        ''            If c.j02ID > 0 Then lisQ = lisQ.Where(Function(p) p.j02ID = c.j02ID)

        ''            c.Amount_Orig = lisQ.Sum(Function(p) p.p31Amount_WithoutVat_Orig)
        ''            c.Amount_Approved = lisQ.Sum(Function(p) p.p31Amount_WithoutVat_Approved)
        ''            c.Amount_Invoiced = lisQ.Sum(Function(p) p.p31Amount_WithoutVat_Invoiced)
        ''        End If
        ''        cLast = c
        ''    Next
        ''End If

        Return lis
    End Function
    Public Function GetList_Extended(mq As BO.myQueryP49, Optional intP41ID_OptimizeSQL As Integer = 0) As IEnumerable(Of BO.p49FinancialPlanExtended) Implements Ip49FinancialPlanBL.GetList_Extended
        Return _cDL.GetList_Extended(mq, intP41ID_OptimizeSQL)
    End Function
End Class
