Public Class j61_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j61_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Textová šablona"


            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc
            If Request.Item("prefix") <> "" Then
                basUI.SelectDropdownlistValue(Me.x29ID, CInt(BO.BAS.GetX29FromPrefix(Request.Item("prefix"))))
            End If
            Return
        End If

        Dim cRec As BO.j61TextTemplate = Master.Factory.j61TextTemplateBL.Load(Master.DataPID)
        Dim cDisp As BO.RecordDisposition = Master.Factory.j61TextTemplateBL.InhaleRecordDisposition(cRec)
        If Not cDisp.OwnerAccess Then
            Master.StopPage("Pro tuto šablonu nedisponujete vlastnickým oprávněním!")
        End If
        With cRec
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)
            Me.j61Name.Text = .j61Name
            Me.j61Ordinary.Value = .j61Ordinary
            Master.Timestamp = .Timestamp
            Me.j61PlainTextBody.Text = .j61PlainTextBody
            Me.j61MailSubject.Text = .j61MailSubject
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.j61MailCC.Text = .j61MailCC
            Me.j61MailBCC.Text = .j61MailBCC
            Me.j61MailTO.Text = .j61MailTO
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With


        roles1.InhaleInitialData(cRec.PID)
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j61TextTemplateBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j61-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        roles1.SaveCurrentTempData()
        With Master.Factory.j61TextTemplateBL
            Dim cRec As BO.j61TextTemplate = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j61TextTemplate)
            With cRec
                .x29ID = BO.BAS.IsNullInt(Me.x29ID.SelectedValue)
                .j61Name = Me.j61Name.Text
                .j61Ordinary = BO.BAS.IsNullInt(Me.j61Ordinary.Value)
                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .j61PlainTextBody = Trim(Me.j61PlainTextBody.Text)
                .j61MailSubject = Me.j61MailSubject.Text
                .j61MailCC = Me.j61MailCC.Text
                .j61MailBCC = Me.j61MailBCC.Text
                .j61MailTO = Me.j61MailTO.Text
            End With

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return
            End If

            If .Save(cRec, lisX69) Then
                Master.DataPID = .LastSavedPID


                Master.CloseAndRefreshParent("j61-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub
End Class