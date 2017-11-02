Public Class p95_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p95_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Fakturační oddíl"
            End With

            RefreshRecord()

            Dim lis As IEnumerable(Of BO.p87BillingLanguage) = Master.Factory.ftBL.GetList_P87(New BO.myQuery)
            For Each c In lis
                CType(panLang.FindControl("lblLang" & c.p87LangIndex.ToString), datalabel).Text = c.p87Name
            Next
            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()
        
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p95InvoiceRow = Master.Factory.p95InvoiceRowBL.Load(Master.DataPID)
        With cRec
            Me.p95name.Text = .p95Name
            Me.p95Code.Text = .p95Code
           
            Master.Timestamp = .Timestamp
            Me.p95Ordinary.Value = .p95Ordinary
            Me.p95Name_BillingLang1.Text = .p95Name_BillingLang1
            Me.p95Name_BillingLang2.Text = .p95Name_BillingLang2
            Me.p95Name_BillingLang3.Text = .p95Name_BillingLang3
            Me.p95Name_BillingLang4.Text = .p95Name_BillingLang4

            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p95InvoiceRowBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p95-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p95InvoiceRowBL
            Dim cRec As BO.p95InvoiceRow = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p95InvoiceRow)
            cRec.p95Name = Me.p95name.Text
            cRec.p95Code = Me.p95Code.Text
            cRec.p95Name_BillingLang1 = Me.p95Name_BillingLang1.Text
            cRec.p95Name_BillingLang2 = Me.p95Name_BillingLang2.Text
            cRec.p95Name_BillingLang3 = Me.p95Name_BillingLang3.Text
            cRec.p95Name_BillingLang4 = Me.p95Name_BillingLang4.Text
            cRec.p95Ordinary = BO.BAS.IsNullInt(Me.p95Ordinary.Value)

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p95-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class