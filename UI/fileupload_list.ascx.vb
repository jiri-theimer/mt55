Public Class fileupload_list
    Inherits System.Web.UI.UserControl

    Public Event BeforeSetAsDeleted(ByVal cFile As BO.p85TempBox, ByRef Cancel As Boolean)
    Public Event AfterSetAsDeleted(ByVal cFile As BO.p85TempBox)
    Public Property Factory As BL.Factory



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        TestUpdateTitle()
    End Sub

    Public Property GUID() As String
        Get
            Return hidGUID.Value
        End Get
        Set(ByVal value As String)
            hidGUID.Value = value
        End Set
    End Property
    Public Property Target4Preview As String
        Get
            Return Me.hidTarget4DispositionInline.Value
        End Get
        Set(value As String)
            hidTarget4DispositionInline.Value = value
        End Set
    End Property
    Public Property OnClientClickPreview As String
        Get
            Return hidOnClientClickPreview.Value
        End Get
        Set(value As String)
            hidOnClientClickPreview.Value = value
        End Set
    End Property
    Public Property LockFlag As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidLockFlag.Value)
        End Get
        Set(value As Integer)
            Me.hidLockFlag.Value = value.ToString
        End Set
    End Property
  
    Public Function GetListO27() As List(Of BO.o27Attachment)
        Return CType(rp1.DataSource, IEnumerable(Of BO.o27Attachment)).ToList

    End Function
    Public Property IsRepeatDirectionVerticaly As Boolean
        Get
            If rp1.RepeatDirection = RepeatDirection.Vertical Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(value As Boolean)
            If value Then
                Me.rp1.RepeatDirection = RepeatDirection.Vertical
            Else
                Me.rp1.RepeatDirection = RepeatDirection.Horizontal
            End If
        End Set
    End Property
    Public Property RepeatColumns As Integer
        Get
            Return rp1.RepeatColumns
        End Get
        Set(value As Integer)
            rp1.RepeatColumns = value
        End Set
    End Property

    Public ReadOnly Property ItemsCount As Integer
        Get
            Return rp1.Items.Count
        End Get
    End Property

    Private Sub TestUpdateTitle()
        If hidUpdateTitle.Value <> "" Then
            Dim a() As String = Split(hidUpdateTitle.Value, "||")
            Dim intP85ID As Integer
            If IsNumeric(a(0)) Then intP85ID = CInt(a(0)) Else Return
            Dim cRec As BO.p85TempBox = Me.Factory.p85TempBoxBL.Load(intP85ID)
            cRec.p85FreeText04 = Trim(a(2))
            Factory.p85TempBoxBL.Save(cRec)
            hidUpdateTitle.Value = ""
            RefreshData_TEMP()
        End If
    End Sub

    Public ReadOnly Property AllowEdit() As Boolean
        Get
            Return BO.BAS.BG(hidAllowEdit.Value)
        End Get
    End Property
    Public ReadOnly Property ShowDateInsert As Boolean
        Get
            Return BO.BAS.BG(hidShowDateInsert.Value)
        End Get
    End Property
    
    Public Sub HandleVersions(ByRef lis As IEnumerable(Of BO.o27Attachment))
        lis = lis.OrderBy(Function(p) p.o27OriginalFileName).ThenBy(Function(p) p.PID)
        Dim s As String = "", sLast As String = "", intV As Integer = 0, bolVersions As Boolean = False, x As Integer = 0
        For Each c In lis
            If c.o27OriginalFileName = sLast Then
                intV += 1 : bolVersions = True
                c.ShowVersion = True
                lis(x - 1).ShowVersion = True
            Else
                intV = 1
            End If
            c.Version = intV
            sLast = c.o27OriginalFileName
            x += 1
        Next
        If bolVersions Then lis = lis.OrderBy(Function(p) p.o27OriginalFileName).ThenByDescending(Function(p) p.Version)
    End Sub
    Public Sub HandleVersionsTempBox(ByRef lis As IEnumerable(Of BO.p85TempBox))
        lis = lis.OrderBy(Function(p) p.p85FreeText01).ThenBy(Function(p) p.p85DataPID)
        Dim s As String = "", sLast As String = "", intV As Integer = 0, bolVersions As Boolean = False, x As Integer = 0
        For Each c In lis
            If c.p85FreeText01 = sLast Then
                intV += 1 : bolVersions = True
                c.p85FreeBoolean04 = True
                lis(x - 1).p85FreeBoolean04 = True
            Else
                intV = 1
            End If
            c.p85OtherKey8 = intV
            sLast = c.p85FreeText01
            x += 1
        Next
        If bolVersions Then lis = lis.OrderBy(Function(p) p.p85FreeText01).ThenByDescending(Function(p) p.p85OtherKey8)
    End Sub

    Public Sub RefreshData(mq As BO.myQueryO27)
        Dim lis As IEnumerable(Of BO.o27Attachment) = Factory.o27AttachmentBL.GetList(mq)
        HandleVersions(lis)
        rp1.DataSource = lis
        rp1.DataBind()

        After_RefreshData()
    End Sub
    Public Sub RefreshData_TEMP()
        Dim lis As IEnumerable(Of BO.p85TempBox) = Factory.p85TempBoxBL.GetList(Me.GUID)
        HandleVersionsTempBox(lis)
        rp1.DataSource = lis
        rp1.DataBind()

        After_RefreshData()
    End Sub

    Private Sub After_RefreshData()
        'zde se nic neděje
    End Sub




    Private Sub Handle_o27_ItemDataBound(sender As Object, e As DataListItemEventArgs)
        Dim cRec As BO.o27Attachment = CType(e.Item.DataItem, BO.o27Attachment)
        CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/Files/" & BO.BAS.GetFileExtensionIcon(cRec.o27FileExtension)
        With CType(e.Item.FindControl("aPreview"), HyperLink)
            If Len(cRec.o27OriginalFileName) > 30 Then
                .Text = Left(cRec.o27OriginalFileName, 20) & "...." & Right(cRec.o27OriginalFileName, 3)
                .ToolTip = cRec.o27OriginalFileName
            Else
                .Text = cRec.o27OriginalFileName
            End If

            .NavigateUrl = "binaryfile.aspx"
            
            .NavigateUrl += "?prefix=o27&disposition=inline&pid=" & cRec.PID.ToString
            .Target = Me.Target4Preview
            If Me.OnClientClickPreview <> "" Then
                .NavigateUrl = "javascript: " & Me.OnClientClickPreview & "('o27'," & cRec.PID.ToString & ")"
            End If
            Select Case Me.hidLockFlag.Value
                Case "1"    'zcela uzamčený přístup ke stahování souborů
                    .NavigateUrl = ""
            End Select
        End With

        With CType(e.Item.FindControl("cmdDownload"), ImageButton)
            .OnClientClick = "return download('prefix=o27&pid=" & cRec.PID.ToString & "&disposition=attachment')"
            Select Case Me.hidLockFlag.Value
                Case "1"    'zcela uzamčený přístup ke stahování souborů
                    .OnClientClick = ""
                    .Enabled = False
                    .ImageUrl = "Images/lock.png"
                    .CssClass = ""

            End Select
        End With
        With CType(e.Item.FindControl("version"), Label)
            If cRec.ShowVersion Then
                .Text = cRec.Version.ToString
                .ToolTip = String.Format("Verze #{0}, čas: {2}, nahrál {1}", cRec.Version.ToString, cRec.UserInsert, BO.BAS.FD(cRec.DateInsert, True, True))
            Else
                .Visible = False
            End If

        End With
        With CType(e.Item.FindControl("DateInsert"), Label)
            .Visible = Me.ShowDateInsert
            If .Visible Then
                Dim d As Date = cRec.DateUpdate
                .Text = BO.BAS.FD(d, True, True)
                If DateDiff(DateInterval.Day, d, Now) > 1 Then
                    .ForeColor = System.Drawing.Color.Gray
                Else
                    .ForeColor = System.Drawing.Color.Red
                End If
            End If

        End With
        With CType(e.Item.FindControl("UserInsert"), Label)
            .Text = cRec.UserInsert
        End With

        CType(e.Item.FindControl("FileSize"), Label).Text = " [" & BO.BAS.FormatFileSize(cRec.o27FileSize) & "]"

        With CType(e.Item.FindControl("FileTitle"), HyperLink)
            .Text = cRec.o27Name
        End With


        e.Item.FindControl("cmdDelete").Visible = False
        With CType(e.Item.FindControl("cmdPdfExport"), HyperLink)
            Select Case LCase(cRec.o27FileExtension)
                Case ".docx", ".doc", ".rtf", ".odt", ".dotx"
                    .NavigateUrl = "binaryfile.aspx?doc2pdf=1&disposition=inline&prefix=o27&pid=" & cRec.PID.ToString
                Case Else
                    .Visible = False
                    'převod do PDF
            End Select
        End With
        
    End Sub

    Private Sub Handle_p85_ItemDataBound(sender As Object, e As DataListItemEventArgs)
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/Files/" & BO.BAS.GetFileExtensionIcon(Right(cRec.p85FreeText01, 4))
        With CType(e.Item.FindControl("aPreview"), HyperLink)
            If Len(cRec.p85FreeText01) > 30 Then
                .Text = Left(cRec.p85FreeText01, 20) & "...." & Right(cRec.p85FreeText01, 3)
                .ToolTip = cRec.p85FreeText01
            Else
                .Text = cRec.p85FreeText01
            End If

            .NavigateUrl = "binaryfile.aspx"
            
            If cRec.p85DataPID = 0 Then
                .NavigateUrl += "?prefix=p85&pid=" & cRec.PID.ToString
            Else
                .NavigateUrl += "?prefix=o27&pid=" & cRec.p85DataPID.ToString
            End If
            .NavigateUrl += "&disposition=inline"
            .Target = Me.Target4Preview
            If Me.OnClientClickPreview <> "" Then
                .NavigateUrl = "javascript: " & Me.OnClientClickPreview & "('p85'," & cRec.PID.ToString & ")"
            End If
        End With
        With CType(e.Item.FindControl("cmdDownload"), ImageButton)
            .OnClientClick = "return download('prefix=p85&pid=" & cRec.PID.ToString & "&disposition=attachment')"
        End With
        With CType(e.Item.FindControl("DateInsert"), Label)
            .Visible = Me.ShowDateInsert
            If .Visible Then
                Dim d As Date = cRec.DateInsert
                If cRec.p85DataPID <> 0 Then
                    d = cRec.p85FreeDate01
                End If
                .Text = BO.BAS.FD(d, True, True)
                If DateDiff(DateInterval.Day, d, Now) > 1 Then
                    .ForeColor = System.Drawing.Color.Gray
                Else
                    .ForeColor = System.Drawing.Color.Red
                End If
            End If

        End With
        With CType(e.Item.FindControl("version"), Label)
            If cRec.p85FreeBoolean04 Then
                .Text = cRec.p85OtherKey8.ToString
                .ToolTip = String.Format("Verze #{0}, soubor {1}", cRec.p85OtherKey8.ToString, cRec.p85FreeText01)
            Else
                .Visible = False
            End If

        End With

        CType(e.Item.FindControl("FileSize"), Label).Text = " [" & BO.BAS.FormatFileSize(cRec.p85FreeNumber01) & "]"

        With CType(e.Item.FindControl("FileTitle"), HyperLink)
            If cRec.p85FreeText04 <> "" Then
                .Text = cRec.p85FreeText04
                .ToolTip = "Upravit popis přílohy"
            Else
                .Text = "Bez popisu"
                .ToolTip = "Zadat název/popis přílohy"
            End If
            .NavigateUrl = "javascript: edittitle(" & cRec.PID.ToString & ",null,'" & cRec.p85FreeText04 & "')"
        End With
        'CType(e.Item.FindControl("panTitle"), Panel).Style.Item("margin-bottom") = "10px"

        If Not Me.AllowEdit Then
            e.Item.FindControl("cmdDelete").Visible = False
            CType(e.Item.FindControl("FileTitle"), HyperLink).NavigateUrl = ""
        Else
            CType(e.Item.FindControl("cmdDelete"), ImageButton).CommandArgument = cRec.PID.ToString
        End If
        e.Item.FindControl("cmdPdfExport").Visible = False
    End Sub

    





    Private Sub rp1_ItemCommand(source As Object, e As DataListCommandEventArgs) Handles rp1.ItemCommand
        If e.CommandName = "delete" Then
            Dim bolCancel As Boolean = False

            Dim cRec As BO.p85TempBox = Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
            RaiseEvent BeforeSetAsDeleted(cRec, bolCancel)
            If bolCancel Then Return

            If Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshData_TEMP()
                RaiseEvent AfterSetAsDeleted(cRec)
            End If
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles rp1.ItemDataBound
        If Me.GUID <> "" Then
            Handle_p85_ItemDataBound(sender, e) 'temp data z p85
        Else
            Handle_o27_ItemDataBound(sender, e)  'ostré data z o27
        End If
    End Sub
End Class