Public Class record_code
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub record_code_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Private ReadOnly Property CurrentX29ID As BO.x29IdEnum
        Get
            Return BO.BAS.GetX29FromPrefix(ViewState("prefix"))
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                ViewState("prefix") = Request.Item("prefix")
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If ViewState("prefix") = "" Or .DataPID = 0 Then
                    .StopPage("prefix or pid missing.")
                End If
                .HeaderText = "Kód záznamu | " & .Factory.GetRecordCaption(Me.CurrentX29ID, .DataPID)
                .HeaderIcon = "Images/record_32.png"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
                
            End With

            RefreshRecord()

        End If
    End Sub

    Private Sub RefreshRecord()
        Dim intX38ID As Integer = 0, bolCanEdit As Boolean = False, bolIamOwner As Boolean = False
        With Master.Factory
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p28Contact
                    Dim cRec As BO.p28Contact = .p28ContactBL.Load(Master.DataPID)
                    Dim cDisp As BO.p28RecordDisposition = .p28ContactBL.InhaleRecordDisposition(cRec)
                    bolIamOwner = cDisp.OwnerAccess
                    Me.txtCode.Text = cRec.p28Code : Me.Owner.Text = cRec.Owner
                    If cRec.p29ID <> 0 Then
                        Dim cTR As BO.p29ContactType = .p29ContactTypeBL.Load(cRec.p29ID)
                        intX38ID = cTR.x38ID
                    Else
                        'pokud záznam kontaktu nemá vazbu na p29, pak jinak nejsme schopni ověřit
                        bolCanEdit = cDisp.OwnerAccess
                    End If
                    ShowLast10("select top 10 p28Code,p29Name FROM p28Contact a LEFT OUTER JOIN p29ContactType b ON a.p29ID=b.p29ID WHERE a.p28IsDraft=0 ORDER BY p28ID DESC")
                Case BO.x29IdEnum.p41Project
                    Dim cRec As BO.p41Project = .p41ProjectBL.Load(Master.DataPID)
                    Dim cDisp As BO.p41RecordDisposition = .p41ProjectBL.InhaleRecordDisposition(cRec)
                    Me.txtCode.Text = cRec.p41Code : Me.Owner.Text = cRec.Owner
                    Dim cTR As BO.p42ProjectType = .p42ProjectTypeBL.Load(cRec.p42ID)
                    intX38ID = cTR.x38ID
                    bolIamOwner = cDisp.OwnerAccess
                    ShowLast10("select top 10 p41Code,p42Name FROM p41Project a LEFT OUTER JOIN p42ProjectType b ON a.p42ID=b.p42ID WHERE a.p41IsDraft=0 ORDER BY p41ID DESC")
                Case BO.x29IdEnum.p91Invoice
                    Dim cRec As BO.p91Invoice = .p91InvoiceBL.Load(Master.DataPID)
                    Dim cDisp As BO.p91RecordDisposition = .p91InvoiceBL.InhaleRecordDisposition(cRec)
                    Me.txtCode.Text = cRec.p91Code : Me.Owner.Text = cRec.Owner
                    Dim cTR As BO.p92InvoiceType = .p92InvoiceTypeBL.Load(cRec.p92ID)
                    intX38ID = cTR.x38ID
                    bolIamOwner = cDisp.OwnerAccess
                    ShowLast10("select top 10 p91Code,p92Name FROM p91Invoice a LEFT OUTER JOIN p92InvoiceType b ON a.p92ID=b.p92ID WHERE a.p91IsDraft=0 ORDER BY p91ID DESC")
                Case BO.x29IdEnum.p90Proforma
                    Dim cRec As BO.p90Proforma = .p90ProformaBL.Load(Master.DataPID)
                    Me.txtCode.Text = cRec.p90Code : Me.Owner.Text = cRec.Owner
                    Dim cTR As BO.p89ProformaType = .p89ProformaTypeBL.Load(cRec.p89ID)
                    intX38ID = cTR.x38ID
                    If cRec.j02ID_Owner = .SysUser.j02ID Or .TestPermission(BO.x53PermValEnum.GR_P90_Owner) Then
                        bolIamOwner = True
                    End If
                    ShowLast10("select top 10 p90Code,p89Name FROM p90Proforma a LEFT OUTER JOIN p89ProformaType b ON a.p89ID=b.p89ID WHERE a.p90IsDraft=0 ORDER BY p90ID DESC")
                Case BO.x29IdEnum.p82Proforma_Payment
                    Dim cRec As BO.p82Proforma_Payment = .p90ProformaBL.LoadP82(Master.DataPID)
                    Dim cP90 As BO.p90Proforma = .p90ProformaBL.Load(cRec.p90ID)
                    Me.txtCode.Text = cRec.p82Code : Me.Owner.Text = cP90.Owner
                    Dim cTR As BO.p89ProformaType = .p89ProformaTypeBL.Load(cP90.p89ID)
                    intX38ID = cTR.x38ID_Payment
                    If cP90.j02ID_Owner = .SysUser.j02ID Or .TestPermission(BO.x53PermValEnum.GR_P90_Owner) Then
                        bolIamOwner = True
                    End If
                    ShowLast10("select top 10 p82Code,NULL as p89Name FROM p82Proforma_Payment ORDER BY p82ID DESC")
                Case BO.x29IdEnum.p56Task
                    Dim cRec As BO.p56Task = .p56TaskBL.Load(Master.DataPID)
                    Dim cDisp As BO.p56RecordDisposition = .p56TaskBL.InhaleRecordDisposition(cRec)
                    Me.txtCode.Text = cRec.p56Code : Me.Owner.Text = cRec.Owner
                    Dim cTR As BO.p57TaskType = .p57TaskTypeBL.Load(cRec.p57ID)
                    intX38ID = cTR.x38ID
                    bolIamOwner = cDisp.OwnerAccess
            End Select

            If intX38ID > 0 Then
                Dim cRec As BO.x38CodeLogic = .x38CodeLogicBL.Load(intX38ID)
                Select Case cRec.x38EditModeFlag
                    Case BO.x38EditModeFlagENUM.AdminOnly
                        bolCanEdit = .TestPermission(BO.x53PermValEnum.GR_Admin)
                    Case BO.x38EditModeFlagENUM.RecordOwnerOnly
                        bolCanEdit = bolIamOwner
                    Case BO.x38EditModeFlagENUM.NotEditable
                        Master.Notify(String.Format("Nastavení číselné řady [{0}] neumožňuje upravovat kód záznamu.", cRec.x38Name), NotifyLevel.WarningMessage)
                End Select
            End If
        End With
        
        If Not bolCanEdit Then
            Master.HideShowToolbarButton("ok", False)
            Master.Notify("Nemáte oprávnění měnit kód tohoto záznamu.")
        End If
        Me.txtCode.Enabled = bolCanEdit

    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim strCode As String = Trim(Me.txtCode.Text), strErr As String = "", bolOK As Boolean = False
            If strCode = "" Then
                Master.Notify("Kód musí být vyplněn.", NotifyLevel.ErrorMessage)
                Return
            End If
            If LCase(Left(strCode, 4)) = "temp" Then
                Master.Notify("Kód nesmí začínat výrazem 'TEMP'.", NotifyLevel.ErrorMessage)
                Return
            End If
            With Master.Factory
                Select Case Me.CurrentX29ID
                    Case BO.x29IdEnum.p41Project
                        Dim cRec As BO.p41Project = .p41ProjectBL.Load(Master.DataPID)
                        cRec.p41Code = strCode
                        If .p41ProjectBL.Save(cRec, Nothing, Nothing, Nothing, Nothing) Then
                            bolOK = True
                        Else
                            strErr = .p41ProjectBL.ErrorMessage
                        End If
                    Case BO.x29IdEnum.p28Contact
                        Dim cRec As BO.p28Contact = .p28ContactBL.Load(Master.DataPID)
                        cRec.p28Code = strCode
                        If .p28ContactBL.Save(cRec, Nothing, Nothing, Nothing, Nothing, Nothing) Then
                            bolOK = True
                        Else
                            strErr = .p28ContactBL.ErrorMessage
                        End If
                    Case BO.x29IdEnum.p91Invoice
                        Dim cRec As BO.p91Invoice = .p91InvoiceBL.Load(Master.DataPID)
                        cRec.p91Code = strCode
                        If .p91InvoiceBL.Update(cRec, Nothing, Nothing) Then
                            bolOK = True
                        Else
                            strErr = .p91InvoiceBL.ErrorMessage
                        End If
                    Case BO.x29IdEnum.p90Proforma
                        Dim cRec As BO.p90Proforma = .p90ProformaBL.Load(Master.DataPID)
                        cRec.p90Code = strCode
                        If .p90ProformaBL.Save(cRec, Nothing, Nothing) Then
                            bolOK = True
                        Else
                            strErr = .p90ProformaBL.ErrorMessage
                        End If
                    Case BO.x29IdEnum.p82Proforma_Payment
                        Dim cRec As BO.p82Proforma_Payment = .p90ProformaBL.LoadP82(Master.DataPID)
                        If .p90ProformaBL.UpdateP82Code(cRec.PID, strCode) Then
                            bolOK = True
                        Else
                            strErr = .p90ProformaBL.ErrorMessage
                        End If
                    Case BO.x29IdEnum.p56Task
                        Dim cRec As BO.p56Task = .p56TaskBL.Load(Master.DataPID)
                        cRec.p56Code = strCode
                        If .p56TaskBL.Save(cRec, Nothing, Nothing, "") Then
                            bolOK = True
                        Else
                            strErr = .p56TaskBL.ErrorMessage
                        End If
                End Select
            End With
            If bolOK Then
                Master.CloseAndRefreshParent("record-code", strCode)
            Else
                Master.Notify("Změna kód záznamu se nepodařila, viz chyba:<br>" & strErr, NotifyLevel.ErrorMessage)
            End If
        End If
    End Sub

    Private Sub ShowLast10(strSQL As String)
        Dim dt As DataTable = Master.Factory.pluginBL.GetDataTable(strSQL, Nothing)
        For Each row As DataRow In dt.Rows
            Me.last10.Text += row.Item(0) & " (" & row.Item(1) & ")<br>"
        Next
    End Sub
End Class