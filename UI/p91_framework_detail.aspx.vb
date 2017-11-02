Imports Telerik.Web.UI

Public Class p91_framework_detail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm
    Private _allNodes

    Private Sub p91_framework_detail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        designer1.Factory = Master.Factory
        tags1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            Me.hidParentWidth.Value = BO.BAS.IsNullInt(Request.Item("parentWidth")).ToString
            hidSource.Value = Request.Item("source")
            With Master
                .SiteMenuValue = "p91"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .Factory.SysUser.OneInvoicePage <> "" Then
                    Server.Transfer(basUI.AddQuerystring2Page(.Factory.SysUser.OneInvoicePage, "pid=" & .DataPID.ToString))
                End If
                If Request.Item("source") = "2" Then
                    .IsHideAllRecZooms = True
                End If
                designer1.x36Key = "p91_framework_detail-j70id"
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p91_framework_detail-pid")
                    .Add("p91_framework_detail-group")
                    .Add(designer1.x36Key)
                    .Add("p91_framework_detail-pagesize")
                    .Add("p91_framework_detail-chkFFShowFilledOnly")
                    .Add("p91_menu-tabskin")
                    .Add("p91_menu-menuskin")
                    .Add("p91_framework_detail-searchbox")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                    basUI.SelectRadiolistValue(Me.opgGroupBy, .GetUserParam("p91_framework_detail-group", "flat"))
                    basUI.SelectDropdownlistValue(Me.cbxPaging, .GetUserParam("p91_framework_detail-pagesize", "20"))
                    Me.chkFFShowFilledOnly.Checked = BO.BAS.BG(.GetUserParam("p91_framework_detail-chkFFShowFilledOnly", "0"))
                    FNO("searchbox").Visible = BO.BAS.BG(.GetUserParam("p91_framework_detail-searchbox", "1"))

                End With

                If .DataPID = 0 Then
                    .DataPID = BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p91_framework_detail-pid", "O"))
                    If .DataPID = 0 Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p91")
                Else
                    If .DataPID <> BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p91_framework_detail-pid", "O")) Then
                        .Factory.j03UserBL.SetUserParam("p91_framework_detail-pid", .DataPID.ToString)
                    End If
                End If


            End With

            RefreshRecord()

            Dim strJ70ID As String = Request.Item("j70id")
            If strJ70ID = "" Then strJ70ID = Master.Factory.j03UserBL.GetUserParam(designer1.x36Key, "0")
            designer1.RefreshData(CInt(strJ70ID))

            SetupGrid()
            RecalcVirtualRowCount()

            With Master
                Select Case .Factory.j03UserBL.GetMyTag(True)
                    Case "draftisout"
                        .Notify("Číslo faktury je nyní [" & Me.p91Code.Text & "].", NotifyLevel.InfoMessage)
                End Select
            End With


            tabs1.Skin = Master.Factory.j03UserBL.GetUserParam("p91_menu-tabskin", "Default")
            menu1.Skin = Master.Factory.j03UserBL.GetUserParam("p91_menu-menuskin", "WebBlue")
        End If

        AdaptMenu()
    End Sub

    Private Sub AdaptMenu()
        If Not menu1.Visible Then Return
        Dim cbx As New RadComboBox()
        With cbx
            .DropDownWidth = Unit.Parse("400px")
            .RenderMode = RenderMode.Auto
            .EnableTextSelection = True
            .MarkFirstMatch = True
            .EnableLoadOnDemand = True
            .Width = Unit.Parse("200px")
            .Style.Item("margin-top") = "5px"
            .OnClientItemsRequesting = "cbxSearch_OnClientItemsRequesting"
            .OnClientSelectedIndexChanged = "cbxSearch_OnClientSelectedIndexChanged"
            .WebServiceSettings.Method = "LoadComboData"
            .WebServiceSettings.Path = "~/Services/invoice_service.asmx"
            .ToolTip = "Hledat fakturu"
            .OnClientFocus = "cbxSearch_OnClientFocus"
            .Text = "Hledat..."
        End With
        If hidSource.Value = "2" Then

            menu1.Skin = "Metro"
            imgIcon32.Visible = False


            FNO("reload").Visible = False
        Else

            FNO("reload").Visible = True
        End If
        If hidSource.Value = "3" Then
            FNO("searchbox").Controls.Add(cbx)
            imgIcon32.Style.Item("top") = "44px"
            cmdGo2Grid.Visible = True
        Else
            FNO("searchbox").Visible = False
            cmdGo2Grid.Visible = False
        End If
    End Sub

    Private Sub ShowHideMenu(bolPopupMenu As Boolean, bolContextMenu As Boolean)
        If bolPopupMenu Then
            menu1.Visible = True
        Else
            menu1.Nodes.Clear()
            menu1.Visible = False
        End If
        If bolContextMenu Then
            panPM1.Visible = True
        Else
            panPM1.Controls.Clear()
            panPM1.Visible = False
        End If
    End Sub
    Private Sub RefreshRecord()
        Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
        If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p91")
        Handle_Permissions(cRec)

        If hidSource.Value = "2" Then
            ShowHideMenu(False, False)
        Else
            If Master.Factory.SysUser.j03PageMenuFlag = 0 Then
                pm1.Attributes.Item("onclick") = "RCM('p91'," & cRec.PID.ToString & ",this,'pagemenu')"
                With linkPM
                    .Text = cRec.p92Name & ": " & cRec.p91Code & " <span class='lbl'>[" & cRec.p91Client & "]</span>"
                    ''.NavigateUrl = "p91_framework_detail.aspx?pid=" & cRec.PID.ToString & "&source=" & Me.hidSource.Value
                    .Attributes.Item("onclick") = "RCM('p91', " & cRec.PID.ToString & ", this, 'pagemenu')"
                End With
                If cRec.IsClosed Then panPM1.Style.Item("background-color") = "black" : linkPM.Style.Item("color") = "white"
                imgIcon32.Visible = False
                ShowHideMenu(False, True)
            Else
                ShowHideMenu(True, False)
                If cRec.IsClosed Then menu1.Skin = "Black"
                FNO("reload").NavigateUrl = "p91_framework_detail.aspx?pid=" & cRec.PID.ToString & "&source=" & Me.hidSource.Value

            End If
        End If
        
        

        With cRec
            'basUIMT.RenderHeaderMenu(.IsClosed, Me.panMenuContainer, menu1)

            

            Me.p91Code.Text = .p91Code
            Me.tabs1.Tabs(0).Text = .p92Name & ": " & .p91Code
            


            Me.p92Name.Text = .p92Name
            Me.clue_p92name.Attributes("rel") = "clue_p92_record.aspx?pid=" & cRec.p92ID.ToString
            With Me.Client
                .Text = cRec.p28Name
                If cRec.p28Name <> cRec.p91Client Then
                    .Text += " | " & cRec.p91Client
                End If
                .NavigateUrl = "p28_framework.aspx?pid=" & cRec.p28ID.ToString
            End With
            If .p28ID = 0 Then
                Me.clue_client.Visible = False
                Me.Client.NavigateUrl = ""
                Me.Client.Text = .p91Client
                'linkClientInvoices.Visible = False
                pm1Client.Visible = False
            Else
                pm1Client.Attributes.Item("onclick") = "RCM('p28'," & cRec.p28ID.ToString & ",this)"
                Me.clue_client.Attributes("rel") = "clue_p28_record.aspx?pid=" & .p28ID.ToString
                'linkClientInvoices.NavigateUrl = "p91_framework.aspx?masterprefix=p28&masterpid=" & .p28ID.ToString
            End If
            Me.p91ClientPerson.Text = .p91ClientPerson


            If .b01ID <> 0 Then
                Me.trWorkflow.Visible = True
                Me.b02Name.Text = .b02Name
            Else
                Me.trWorkflow.Visible = False
            End If


            Me.p91Amount_Debt.Text = BO.BAS.FN(.p91Amount_Debt)
            Me.p91Amount_WithoutVat.Text = BO.BAS.FN(.p91Amount_WithoutVat)
            Me.p91Amount_WithVat.Text = BO.BAS.FN(.p91Amount_WithVat)
            Me.p91Amount_Vat.Text = BO.BAS.FN(.p91Amount_Vat)

            Me.j27Code_debt.Text = .j27Code
            Me.j27Code_withoutvat.Text = .j27Code
            Me.j27Code_vat.Text = .j27Code
            Me.j27Code_withvat.Text = .j27Code

            If .p91ProformaAmount <> 0 Then
                Me.p91ProformaAmount.Text = BO.BAS.FN(.p91ProformaAmount)
                Me.j27Code_proforma.Text = .j27Code
            Else
                lblProforma.Visible = False
            End If


            Me.p91Date.Text = BO.BAS.FD(.p91Date, False, True)
            Me.p91DateMaturity.Text = BO.BAS.FD(.p91DateMaturity, False, True)
            If .p91Amount_Debt > 0 Then
                Me.p91DateMaturity.ForeColor = Drawing.Color.Red
            End If
            Me.p91DateSupply.Text = BO.BAS.FD(.p91DateSupply, False, True)
            Me.p91DateBilled.Text = BO.BAS.FD(.p91DateBilled, False, True)

            If .p91Text1 <> "" Then
                Me.p91Text1.Text = BO.BAS.CrLfText2Html(.p91Text1)
            Else
                panText1.Visible = False
            End If


            Me.p91Text2.Text = BO.BAS.CrLfText2Html(.p91Text2)
            Me.Owner.Text = .Owner
            Me.WorksheetRange.Text = BO.BAS.FD(.p91Datep31_From) & " - " & BO.BAS.FD(.p91Datep31_Until)
            ''Me.BillingAddress.Text = ParseAddress(.o38ID_Primary)
            ''Me.PostAddress.Text = ParseAddress(.o38ID_Delivery)
            Me.BillingAddress.Text = BO.BAS.CrLfText2Html(.PrimaryAddress)
            Me.PostAddress.Text = Replace(.p91ClientAddress2, vbCrLf, ",")
            HandleBankAccount(.p93ID, .j27ID)

            Me.p91RoundFitAmount.Text = BO.BAS.FN(.p91RoundFitAmount)

            HandleDirectReports(.p92ID)

            Me.lblExchangeRate.Visible = False : cmdRecalcExchangeRate.Visible = False
            If Not BO.BAS.IsNullDBDate(.p91DateExchange) Is Nothing Then
                lblExchangeRate.Visible = True
                If .p91ExchangeRate <> 1 Then
                    Me.p91ExchangeRate.Text = .p91ExchangeRate.ToString & " (" & BO.BAS.FD(.p91DateExchange) & ")"
                Else
                    Me.p91ExchangeRate.Text = String.Format("Kurz ze dne {0}", BO.BAS.FD(.p91DateExchange))
                End If
                cmdRecalcExchangeRate.Visible = True
            End If
            Dim cRecSource As BO.p91Invoice = Nothing
            If .p91ID_CreditNoteBind <> 0 Then
                'jedná se o dobropis
                cRecSource = Master.Factory.p91InvoiceBL.Load(.p91ID_CreditNoteBind)
            Else
                cRecSource = Master.Factory.p91InvoiceBL.LoadCreditNote(.PID)
                lblSourceCode.Text = "<img src='Images/correction_down.gif'></img>" & "Opravný doklad:"
            End If
            If Not cRecSource Is Nothing Then
                Me.trSourceCode.Visible = True
                Me.SourceLink.Text = cRecSource.p91Code
                Me.SourceLink.NavigateUrl = "p91_framework.aspx?pid=" & cRecSource.PID.ToString
            End If
            Me.p91Client.Text = .p91Client
            Me.ClientAddress.Text = .p91ClientAddress1_Street & ", " & .p91ClientAddress1_City & ", " & .p91ClientAddress1_ZIP & " " & .p91ClientAddress1_Country
            Me.ClientIDs.Text = .p91Client_VatID & " | " & .p91Client_RegID

            Me.linkTimestamp.Text = .Timestamp
        End With



        Me.comments1.RefreshData(Master.Factory, BO.x29IdEnum.p91Invoice, cRec.PID)
     

        imgDocType.Visible = False : cmdConvertDraft.Visible = False
        If cRec.p91IsDraft Then
            imgDocType.ImageUrl = "Images/draft_icon.gif" : imgDocType.Visible = True
            If cRec.j02ID_Owner = Master.Factory.SysUser.j02ID Or Master.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Owner) Then
                cmdConvertDraft.Visible = True
            End If
        End If
        labels1.RefreshData(Master.Factory, BO.x29IdEnum.p91Invoice, cRec.PID)
        boxX18.Visible = labels1.ContainsAnyData

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p91Invoice, cRec.PID)
        Me.roles1.RefreshData(lisX69, cRec.PID)
        If Me.roles1.RowsCount = 0 Then panRoles.Visible = False

        Dim lisFF As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p91Invoice, Master.DataPID, cRec.p92ID)
        If lisFF.Count > 0 Then
            ff1.FillData(lisFF, Not Me.chkFFShowFilledOnly.Checked)
        Else
            Me.chkFFShowFilledOnly.Visible = False : Me.ff1.Visible = False
        End If
        tags1.RefreshData(cRec.PID)
        RenderLastX40(cRec.PID)
    End Sub
    Private Sub RenderLastX40(intP91ID As Integer)
        Dim cX40 As BO.x40MailQueue = Master.Factory.x40MailQueueBL.LoadByEntity(intP91ID, BO.x29IdEnum.p91Invoice)
        If Not cX40 Is Nothing Then
            With linkLastX40
                .Visible = True
                .Text = "<img src='Images/email.png'/> " & cX40.StatusAlias & ": " & BO.BAS.FD(cX40.x40WhenProceeded, True, True) & " (" & cX40.x40Recipient & ")"
                .NavigateUrl = "javascript:x40_record(" & cX40.PID.ToString & ")"
                .Style.Item("color") = cX40.StatusColor
            End With
        End If
    End Sub

    Private Sub HandleDirectReports(intP92ID As Integer)
        Dim cRec As BO.p92InvoiceType = Master.Factory.p92InvoiceTypeBL.Load(intP92ID)
        With cRec
            Me.p92ReportConstantText.Text = BO.BAS.CrLfText2Html(.p92ReportConstantText)
            If .x31ID_Invoice > 0 Then
                Me.cmdReportInvoice.NavigateUrl = "javascript: report(" & .x31ID_Invoice.ToString & ")"
            Else
                Me.cmdReportInvoice.Visible = False
            End If
            If .x31ID_Attachment > 0 Then
                Me.cmdReportAttachment.NavigateUrl = "javascript: report(" & .x31ID_Attachment.ToString & ")"
            Else
                Me.cmdReportAttachment.Visible = False
            End If
            If .x31ID_Letter > 0 Then
                Me.cmdReportLetter.NavigateUrl = "javascript: report(" & .x31ID_Letter.ToString & ")"
            Else
                Me.cmdReportLetter.Visible = False
            End If
        End With
    End Sub
    ''Private Function ParseAddress(intO38ID As Integer) As String
    ''    If intO38ID = 0 Then Return ""
    ''    Dim cRec As BO.o38Address = Master.Factory.o38AddressBL.Load(intO38ID)
    ''    Return cRec.FullAddress
    ''End Function
    Private Sub HandleBankAccount(intP93ID As Integer, intJ27ID As Integer)
        Dim lis As IEnumerable(Of BO.p88InvoiceHeader_BankAccount) = Master.Factory.p93InvoiceHeaderBL.GetList_p88(intP93ID)
        If lis.Where(Function(p) p.j27ID = intJ27ID).Count > 0 Then
            Dim cRec As BO.p86BankAccount = Master.Factory.p86BankAccountBL.Load(lis.Where(Function(p) p.j27ID = intJ27ID)(0).p86ID)
            Me.BankAccount.Text = cRec.p86BankAccount & "/" & cRec.p86BankCode
            Me.BankName.Text = cRec.p86BankName
        End If

    End Sub


    Private Sub Handle_Permissions(cRec As BO.p91Invoice)
        If Not menu1.Visible Then Return
        FNO("cmdAboImport").Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Owner)
        FNO("cmdPohoda").Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Reader)

        FNO("cmdX40").NavigateUrl = "x40_framework.aspx?masterprefix=p91&masterpid=" & cRec.PID.ToString
        Dim cDisp As BO.p91RecordDisposition = Master.Factory.p91InvoiceBL.InhaleRecordDisposition(cRec)

        With Master.Factory
            FNO("cmdCreateInvoice").Visible = .TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator)
            FNO("cmdO23").Visible = .SysUser.j04IsMenu_Notepad
            FNO("cmdO22").Visible = .TestPermission(BO.x53PermValEnum.GR_O22_Creator)
            FNO("cmdPivot").Visible = .TestPermission(BO.x53PermValEnum.GR_P31_Pivot)
            FNO("cmdPivot").NavigateUrl = "p31_sumgrid.aspx?masterprefix=p91&masterpid=" & cRec.PID.ToString
            recmenu1.FindItemByValue("fullscreen").Visible = .SysUser.j04IsMenu_Worksheet
        End With
        With cDisp
            FNO("cmdPay").Visible = .OwnerAccess
            FNO("cmdEdit").Visible = .OwnerAccess
            FNO("cmdCreateInvoice").Visible = .OwnerAccess
            FNO("cmdPay").Visible = .OwnerAccess
            FNO("cmdAppendWorksheet").Visible = .OwnerAccess
            FNO("cmdChangeCurrency").Visible = .OwnerAccess
            FNO("cmdChangeVat").Visible = .OwnerAccess
            FNO("cmdProforma").Visible = .OwnerAccess
            FNO("cmdCreditNote").Visible = .OwnerAccess

            recmenu1.FindItemByValue("new").Visible = .OwnerAccess
            recmenu1.FindItemByValue("akce").Visible = .OwnerAccess
        End With


        If cRec.p92InvoiceType = BO.p92InvoiceTypeENUM.CreditNote Then
            FNO("cmdPay").Visible = False
            FNO("cmdProforma").Visible = False
            FNO("cmdCreditNote").Visible = False
            FNO("record").Text = "ZÁZNAM DOKLADU"
            lblp91DateBilled.Visible = False
            p91DateMaturity.Visible = False
            imgRecord.Visible = True : imgRecord.ImageUrl = "Images\correction_down.gif"
            lblExchangeRate.Visible = False : p91ExchangeRate.Visible = False
        End If
    End Sub



    Private Sub SetupGrid()
        With Master.Factory.j70QueryTemplateBL
            Dim cJ70 As BO.j70QueryTemplate = .Load(designer1.CurrentJ70ID)
            Dim cS As New SetupDataGrid(Master.Factory, grid1, cJ70)
            With cS
                .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                .AllowCustomPaging = True
                .AllowMultiSelect = True
                .AllowMultiSelectCheckboxSelector = True
                .MasterPrefix = "p91"
            End With
            Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
            hidCols.Value = cG.Cols
            Me.hidFrom.Value = cG.AdditionalFROM



            ''Dim strF As String = ""
            ''Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, CInt(Me.cbxPaging.SelectedValue), True, True, , , , , strF, , , "p91")
            ''Me.hidFrom.Value = strF
        End With


        Dim strGroupField As String = "", strHeaderText As String = ""
        Select Case Me.opgGroupBy.SelectedValue
            Case "p41"
                strGroupField = "p41Name" : strHeaderText = "Projekt"
            Case "p34"
                strGroupField = "p34Name" : strHeaderText = "Sešit"
            Case "p95"
                strGroupField = "p95Name" : strHeaderText = "Fakturační oddíl"
            Case "p32"
                strGroupField = "p32Name" : strHeaderText = "Aktivita"
            Case "j02"
                strGroupField = "Person" : strHeaderText = "Osoba"
            Case "p56"
                strGroupField = "p56Code" : strHeaderText = "Úkol"
            Case "p70"
                strGroupField = "p70Name" : strHeaderText = "Fakturační status úkonu"
            Case "p31ApprovingSet"
                strGroupField = "p31ApprovingSet" : strHeaderText = "Billing dávka"
            Case Else
                Return
        End Select

        ''grid1.radGridOrig.ClientSettings.Scrolling.ScrollHeight = Unit.Parse("300px")
        With grid1.radGridOrig.MasterTableView
            .ShowGroupFooter = True
            Dim GGE As New Telerik.Web.UI.GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = strGroupField
            fld.HeaderText = strHeaderText

            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)

        End With

    End Sub

    Public Sub RecalcVirtualRowCount()
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)

        Dim cSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, False, False)
        If Not cSum Is Nothing Then
            grid1.VirtualRowCount = cSum.RowsCount
            With tabs1.FindTabByValue("p31")
                .Text = BO.BAS.OM2(.Text, cSum.RowsCount.ToString)
                ''.Text = .Text & " (" & cSum.RowsCount.ToString & ")"
            End With
            ViewState("footersum") = grid1.GenerateFooterItemString(cSum)
        Else
            ViewState("footersum") = ""
            grid1.VirtualRowCount = 0
        End If


        grid1.radGridOrig.CurrentPageIndex = 0

    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)

        mq.p91ID = Master.DataPID
        mq.j70ID = designer1.CurrentJ70ID

    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, True, False, "p91_framework_detail")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim strGroupField As String = ""
        Select Case Me.opgGroupBy.SelectedValue
            Case "p41"
                strGroupField = "p41name"
            Case "p34"
                strGroupField = "p34Name"
            Case "p95"
                strGroupField = "p95Name"
            Case "p32"
                strGroupField = "p32Name"
            Case "j02"
                strGroupField = "Person"
            Case "j27"
                strGroupField = "j27Code_Billing_Orig"
            Case "p56"
                strGroupField = "p56Code"
            Case "p70"
                strGroupField = "p70Name"
            Case "p31ApprovingSet"
                strGroupField = "p31ApprovingSet"
            Case Else
        End Select
        ''If Me.opgGroupBy.SelectedValue <> "" Then
        ''    Me.hidCols.Value += "," & strSort
        ''End If


        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)
        With mq
            .MG_PageSize = CInt(Me.cbxPaging.SelectedValue)
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            .MG_GridGroupByField = strGroupField
            .MG_GridSqlColumns = hidCols.Value & ",a.p41ID as p41IDX,isnull(p41.p41NameShort,p41.p41Name) as p41NameX"
            .MG_AdditionalSqlFROM = hidFrom.Value
        End With

        Dim dt As DataTable = Master.Factory.p31WorksheetBL.GetGridDataSource(mq)
        If dt Is Nothing Then
            Master.Notify(Master.Factory.p31WorksheetBL.ErrorMessage, NotifyLevel.ErrorMessage)
            Return
        Else
            grid1.DataSourceDataTable = dt
        End If

        ''Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        ''grid1.DataSource = lis
        Dim p41ids = dt.AsEnumerable.Select(Function(p) p.Item("p41IDX").ToString & "|" & p.Item("p41NameX")).Distinct

        'Dim p41ids As List(Of Object) = dt.AsEnumerable.Select(Function(p) p.Item("p41ID").ToString & "|" & p.Item("p41Name")).Distinct

        rpProject.DataSource = p41ids
        rpProject.DataBind()
        If rpProject.Items.Count > 1 Then
            With lblProject
                .Text = BO.BAS.OM2(.Text, rpProject.Items.Count.ToString)
            End With
            If rpProject.Items.Count >= 3 Then panProjects.Style.Item("height") = "60px" : panProjects.Style.Item("overflow") = "auto"
        End If

    End Sub
    Private Sub grid1_NeedFooterSource(footerItem As Telerik.Web.UI.GridFooterItem, footerDatasource As Object) Handles grid1.NeedFooterSource
        footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"
        grid1.ParseFooterItemString(footerItem, ViewState("footersum"))
    End Sub

    Private Sub opgGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgGroupBy.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p91_framework_detail-group", Me.opgGroupBy.SelectedValue)

        SetupGrid()
        RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p91_framework_detail-pagesize", Me.cbxPaging.SelectedValue)
        SetupGrid()
        RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        If Me.hidHardRefreshFlag.Value = "" Then Return
        Select Case Me.hidHardRefreshFlag.Value
            Case "p31-save", "p31-remove", "p31-add"
                RefreshRecord()
                RecalcVirtualRowCount()
                grid1.Rebind(True)
                Me.tabs1.FindTabByValue("p31").Selected = True
            Case Else
                ReloadPage()
        End Select


        Me.hidHardRefreshFlag.Value = ""
    End Sub

    Private Sub cmdConvertDraft_Click(sender As Object, e As EventArgs) Handles cmdConvertDraft.Click
        With Master.Factory.p91InvoiceBL
            If .ConvertFromDraft(Master.DataPID) Then
                Master.Factory.j03UserBL.SetMyTag("draftisout")
                ReloadPage()
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With

    End Sub
    Private Sub ReloadPage()
        Response.Redirect(GetReloadedUrl())
    End Sub
    Private Function GetReloadedUrl() As String
        Return "p91_framework_detail.aspx?pid=" & Master.DataPID.ToString & "&source=" & Me.hidSource.Value
    End Function

    Private Sub chkFFShowFilledOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkFFShowFilledOnly.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p91_framework_detail-chkFFShowFilledOnly", BO.BAS.GB(Me.chkFFShowFilledOnly.Checked))
        ReloadPage()
    End Sub

    Private Sub rpProject_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpProject.ItemDataBound
        Dim s As String = CType(e.Item.DataItem, String)
        Dim a() As String = Split(s, "|")
        With CType(e.Item.FindControl("clue_project"), HyperLink)
            .Attributes("rel") = "clue_p41_record.aspx?pid=" & a(0)
        End With
        With CType(e.Item.FindControl("p41Name"), HyperLink)
            .Text = a(1)
            .NavigateUrl = "p41_framework.aspx?pid=" & a(0)
        End With
        With CType(e.Item.FindControl("pm1"), HyperLink)
            .Attributes.Item("onclick") = "RCM('p41'," & a(0) & ",this)"
        End With

    End Sub

    Private Sub p91_framework_detail_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Dim s As String = ""
        With plug1
            .AddDbParameter("pid", Master.DataPID)
            .GenerateTable(Master.Factory, "exec dbo.p91_get_cenovy_rozpis @pid,1,1,0")
        End With
        With FNO("fs")
            If hidSource.Value = "3" Then
                .ImageUrl = "Images/fullscreen.png"
                .Text = "Přepnout do datového přehledu"
                .Width = Nothing
            Else
                .ImageUrl = "Images/open_in_new_window.png"
                .Text = "Otevřít v nové záložce"
            End If

        End With
        designer1.ReloadUrl = GetReloadedUrl()
    End Sub

    Private Sub cmdRecalcExchangeRate_Click(sender As Object, e As EventArgs) Handles cmdRecalcExchangeRate.Click
        Master.Factory.p91InvoiceBL.ClearExchangeDateAndRecalc(Master.DataPID)
        ReloadPage()
    End Sub

    Private Function FNO(strValue As String) As Telerik.Web.UI.NavigationNode
        If Not menu1.Visible Then Return New NavigationNode()


        Return menu1.GetAllNodes.First(Function(p) p.ID = strValue)
        'If menu1.GetAllNodes.Where(Function(p) p.ID = strValue).Count > 0 Then

        'Else
        '    Master.Notify(strValue)
        '    Return Nothing
        'End If

    End Function
End Class