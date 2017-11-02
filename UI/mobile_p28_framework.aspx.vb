Public Class mobile_p28_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Sub mobile_p28_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .MenuPrefix = "p28"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                Me.worksheet1.AllowShowRates = .Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates)

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p28_framework_detail-pid")
                    
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                End With
                If .DataPID = 0 Then
                    .DataPID = BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p28_framework_detail-pid", "O"))
                    If .DataPID = 0 Then Response.Redirect("mobile_entity_framework_missing.aspx?prefix=p28")
                Else
                    If .DataPID <> BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p28_framework_detail-pid", "O")) Then
                        .Factory.j03UserBL.SetUserParam("p28_framework_detail-pid", .DataPID.ToString)
                    End If
                End If

            End With

            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
        With cRec
            Me.RecordHeader.Text = BO.BAS.OM3(.p28Name, 30)
            Me.RecordHeader.NavigateUrl = "mobile_p28_framework.aspx?pid=" & .PID.ToString
            Me.RecordName.Text = "[" & .p28Code & "] " & .p28Name
            Me.cmdP31Grid.NavigateUrl = "mobile_grid.aspx?prefix=p31&masterprefix=p28&masterpid=" & .PID.ToString

            If .p29ID > 0 Or .p28IsDraft Then
                trType.Visible = True
                Me.p29Name.Text = .p29Name
            End If
            Me.imgDraft.Visible = .p28IsDraft
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                If .p51ID_Billing > 0 Then
                    If .p51Name_Billing.IndexOf(cRec.p28Name) >= 0 Then
                        'sazby na míru
                        Me.PriceList_Billing.Text = "Sazby na míru"
                    Else
                        Me.PriceList_Billing.Text = .p51Name_Billing
                    End If
                End If
            Else
                trP51.Visible = False
            End If

            If .b02ID > 0 Then
                Me.b02Name.Text = .b02Name
                trB02.Visible = True
            End If
            If .p28ParentID <> 0 Then
                Me.trParent.Visible = True
                Me.ParentClient.NavigateUrl = "mobile_p28_framework.aspx?pid=" & .p28ParentID.ToString
                Me.ParentClient.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, .p28ParentID)
            End If
        End With

        Dim lisO37 As IEnumerable(Of BO.o37Contact_Address) = Master.Factory.p28ContactBL.GetList_o37(cRec.PID)
        If lisO37.Count > 0 Then
            Me.address1.FillData(lisO37)
            Me.address1.Visible = True
        End If
        Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(cRec.PID)
        If lisO32.Count > 0 Then
            Me.medium1.FillData(lisO32)
            Me.medium1.Visible = True
        End If

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, cRec.PID)

        Me.roles_project.RefreshData(lisX69, cRec.PID)

        Dim mqO23 As New BO.myQueryO23(0)
        mqO23.p28IDs = BO.BAS.ConvertInt2List(Master.DataPID)
        mqO23.SpecificQuery = BO.myQueryO23_SpecificQuery.AllowedForRead
        Dim lisO23 As IEnumerable(Of BO.o23Doc) = Master.Factory.o23DocBL.GetList(mqO23)
        If lisO23.Count > 0 Then
            Me.boxO23.Visible = True
            notepad1.RefreshData(lisO23, cRec.PID)
            CountO23.Text = lisO23.Count.ToString
        Else
            Me.boxO23.Visible = False
        End If

        Dim lisP30 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(Master.DataPID, 0, True)
        If lisP30.Count > 0 Then
            Me.boxP30.Visible = True
            Me.persons1.FillData(lisP30, False)
            Me.CountP30.Text = lisP30.Count.ToString

        Else
            Me.boxP30.Visible = False
        End If
        If Master.Factory.SysUser.j04IsMenu_Invoice Then
            Dim mqP91 As New BO.myQueryP91
            mqP91.p28ID = cRec.PID
            mqP91.MG_SelectPidFieldOnly = True
            Dim intCount As Integer = Master.Factory.p91InvoiceBL.GetVirtualCount(mqP91)
            If intCount > 0 Then
                Me.lisP91.Visible = True : Me.CountP91.Text = intCount.ToString
            End If
        End If
        

        

       labels1.RefreshData(Master.Factory, BO.x29IdEnum.p28Contact, cRec.PID)
        boxX18.Visible = labels1.ContainsAnyData
        RefreshP31Summary()

        Dim mqP41 As New BO.myQueryP41
        mqP41.p28ID = cRec.PID
        mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
        mqP41.Closed = BO.BooleanQueryMode.NoQuery
        mqP41.MG_SelectPidFieldOnly = True
        Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mqP41)
        If lisP41.Count > 0 Then
            liP41.Visible = True
            If lisP41.Where(Function(p) p.IsClosed = True).Count > 0 Then
                Me.CountP41.Text = lisP41.Where(Function(p) p.IsClosed = True).ToString & " + " & lisP41.Where(Function(p) p.IsClosed = True).ToString
            Else
                Me.CountP41.Text = lisP41.Count.ToString
            End If
        End If
        Dim mqP56 As New BO.myQueryP56
        mqP56.MG_SelectPidFieldOnly = True
        mqP56.p28ID = cRec.PID
        mqP56.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
        mqP56.Closed = BO.BooleanQueryMode.NoQuery
        Dim lisP56 As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mqP56)
        If lisP56.Count > 0 Then
            liP56_Actual.Visible = True : liP56_Closed.Visible = True
            Me.CountP56_Actual.Text = lisP56.Where(Function(p) p.IsClosed = True).Count.ToString
            Me.CountP56_Closed.Text = lisP56.Where(Function(p) p.IsClosed = False).Count.ToString

        End If

        Handle_Permissions(cRec)
    End Sub

    Private Sub RefreshP31Summary()
        Dim mq As New BO.myQueryP31
        mq.p28ID_Client = Master.DataPID
        'mq.DateFrom = period1.DateFrom
        'mq.DateUntil = period1.DateUntil
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
        mq.QuickQuery = BO.myQueryP31_QuickQuery.EditingOrApproved

        Dim lis As IEnumerable(Of BO.p31WorksheetBigSummary) = Master.Factory.p31WorksheetBL.GetList_BigSummary(mq)

        If lis.Count = 0 Then
            worksheet1.Visible = False
        Else
            worksheet1.Visible = True
            worksheet1.RefreshData(lis, 1)

        End If

    End Sub

    

    Private Sub Handle_Permissions(cRec As BO.p28Contact)
        Dim cDisp As BO.p28RecordDisposition = Master.Factory.p28ContactBL.InhaleRecordDisposition(cRec)
        If Not cDisp.ReadAccess Then
            Master.StopPage("Nedisponujete přístupovým oprávněním ke klientovi.")
        End If
        With Master.Factory.SysUser
            If Not .j04IsMenu_Invoice Then lisP91.Visible = False
        End With

    End Sub
End Class