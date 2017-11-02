Public Class o48_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("record not found.")
                .HeaderText = "Monitoring subjektu v insolvenčním rejstříku"
                If BO.ASS.GetConfigVal("isir_service") = "" Then
                    .StopPage("Napojení na ISIR monitoring není zapnuté.")
                End If
                ViewState("isir_login") = BO.ASS.GetConfigVal("isir_login")
                ViewState("isir_pwd") = BO.ASS.GetConfigVal("isir_pwd")
            End With
            
            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
        Dim b As Boolean = False
        With cP28
            If .p28RegID <> "" Then
                ViewState("subject") = "<subjects><subject><name>" & .p28CompanyName & "</name><ic>" & .p28RegID & "</ic></subject></subjects>"
                b = True
            End If
            If .p28Person_BirthRegID <> "" Then
                ViewState("subject") = "<subjects><subject><firstname>" & .p28FirstName & "</firstname><name>" & .p28LastName & "</name><rc>" & .p28Person_BirthRegID & "</rc></subject></subjects>"
                b = True
            End If
            If Not b Then
                Master.StopPage("Monitorovaný subjekt musí mít vyplněné IČ nebo RČ.")
            End If
            Me.p28Name.Text = .p28Name
            Me.p28RecID.Text = .p28RegID
            Me.p28Person_BirthRegID.Text = .p28Person_BirthRegID
        End With
        Dim cRec As BO.o48IsirMonitoring = Master.Factory.p28ContactBL.LoadO48Record(Master.DataPID)
        If Not cRec Is Nothing Then
            Me.Timestamp.Text = cRec.Timestamp
            cmdRemove.Visible = True
        Else
            cmdAppend.Visible = True
        End If

    End Sub

    Private Sub cmdAppend_Click(sender As Object, e As EventArgs) Handles cmdAppend.Click
        rw(False)
    End Sub

    Private Sub cmdRemove_Click(sender As Object, e As EventArgs) Handles cmdRemove.Click
        rw(True)
    End Sub
    Private Sub rw(bolRemove As Boolean)
        With Master.Factory.p28ContactBL
            If .AppendOrRemove_IsirMoniting(Master.DataPID, bolRemove) Then
                
                Master.CloseAndRefreshParent("o48")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub
End Class