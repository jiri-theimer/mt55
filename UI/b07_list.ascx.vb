Public Class b07_list
    Inherits System.Web.UI.UserControl
    Private Property _intSelectedB07ID As Integer
    ''Private Property _lastB07ID As Integer
    ''Private Property _lastRI As RepeaterItem
    Private Property _lisO27 As IEnumerable(Of BO.o27Attachment) = Nothing
    Private Property _sysUser As BO.j03UserSYS

    Public Property ShowInsertButton As Boolean
        Get
            Return linkAdd.Visible
        End Get
        Set(value As Boolean)
            linkAdd.Visible = value
        End Set
    End Property
    Public Property AttachmentIsReadonly As Boolean
        Get
            If hidAttachmentsReadonly.Value = "1" Then Return True Else Return False
        End Get
        Set(value As Boolean)
            hidAttachmentsReadonly.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property ShowHeader As Boolean
        Get
            Return panHeader.Visible
        End Get
        Set(value As Boolean)
            panHeader.Visible = value
        End Set
    End Property
    Public Property IsClueTipInfo As Boolean
        Get
            Return BO.BAS.BG(hidIsClueTip.Value)
        End Get
        Set(value As Boolean)
            hidIsClueTip.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public ReadOnly Property RowsCount As Integer
        Get
            Return rp1.Items.Count
        End Get
    End Property
    Public Property JS_Create As String
        Get
            Return hidJS_Create.Value
        End Get
        Set(value As String)
            hidJS_Create.Value = value
        End Set
    End Property
    Public Property JS_Reaction As String
        Get
            Return hidJS_Reaction.Value
        End Get
        Set(value As String)
            hidJS_Reaction.Value = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        linkAdd.NavigateUrl = "javascript:" & Me.JS_Create
    End Sub

    Public Sub RefreshOneCommentRecord(factory As BL.Factory, intB07ID As Integer)
        _sysUser = factory.SysUser
        Dim mq As New BO.myQueryB07
        mq.AddItemToPIDs(intB07ID)

        Dim lisB07 As IEnumerable(Of BO.b07Comment) = factory.b07CommentBL.GetList(mq)

        If lisB07.Count > 0 Then
            Dim mqO27 As New BO.myQueryO27
            mqO27.Record_x29ID = lisB07(0).x29ID
            mqO27.Record_PID = lisB07(0).b07RecordPID
            _lisO27 = factory.o27AttachmentBL.GetList(mqO27)
        End If


        rp1.DataSource = lisB07
        rp1.DataBind()

        panHeader.Visible = False


    End Sub

    Public Sub RefreshData(factory As BL.Factory, x29id As BO.x29IdEnum, intRecordPID As Integer, Optional intSelectedB07ID As Integer = 0)
        _sysUser = factory.SysUser
        Dim mq As New BO.myQueryB07
        mq.RecordDataPID = intRecordPID
        mq.x29id = x29id
        If intSelectedB07ID > 0 Then mq.AddItemToPIDs(intSelectedB07ID)
        Me.hidPrefix.Value = BO.BAS.GetDataPrefix(x29id)
        Dim lisB07 As IEnumerable(Of BO.b07Comment) = factory.b07CommentBL.GetList(mq)

        Dim mqO27 As New BO.myQueryO27
        mqO27.Record_x29ID = x29id
        mqO27.Record_PID = intRecordPID
        _lisO27 = factory.o27AttachmentBL.GetList(mqO27)


        Me.hidRecordPID.Value = intRecordPID.ToString
        _intSelectedB07ID = intSelectedB07ID

        rp1.DataSource = lisB07
        rp1.DataBind()

        If linkAdd.Visible = False And rp1.Items.Count = 0 Then
            panHeader.Visible = False
        End If

        lblHeader.Text = BO.BAS.OM2(Me.lblHeader.Text, lisB07.Count.ToString)
    End Sub


    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.b07Comment = CType(e.Item.DataItem, BO.b07Comment), bolAtts As Boolean = False

        If Not _lisO27 Is Nothing Then
            With CType(e.Item.FindControl("rpAtt"), Repeater)
                .DataSource = _lisO27.Where(Function(p) p.b07ID = cRec.PID)
                .DataBind()
                If .Items.Count > 0 Then bolAtts = True
            End With
        End If
        
     

        With CType(e.Item.FindControl("panRecord"), Panel)
            If cRec.b07TreeLevel > 1 Then
                .Style.Item("padding-left") = ((cRec.b07TreeLevel - 1) * 20).ToString & "px"
            End If
            If _intSelectedB07ID > 0 And cRec.PID = _intSelectedB07ID Then
                .BackColor = Drawing.Color.Orange
            End If
            If cRec.o43ID > 0 Then
                .BackColor = Drawing.Color.LightSkyBlue
            End If
        End With
        With CType(e.Item.FindControl("b07Value"), Literal)
            If Len(cRec.b07Value) < 15000 Or hidIsClueTip.Value = "1" Then
                .Text = BO.BAS.CrLfText2Html(cRec.b07Value)
            Else
                .Text = "..."
            End If

        End With
        With CType(e.Item.FindControl("pan100"), Panel)
            If hidIsClueTip.Value = "1" Then
                .Style.Clear()
            
            End If
        End With
        With CType(e.Item.FindControl("Author"), Label)
            .Text = cRec.Author
        End With
        With CType(e.Item.FindControl("clue1"), HyperLink)
            If hidIsClueTip.Value = "1" Then
                .Visible = False
            Else
                .Attributes.Item("rel") = "clue_b07_record.aspx?pid=" & cRec.PID.ToString
            End If

        End With


        With CType(e.Item.FindControl("imgPhoto"), Image)
            If hidIsClueTip.Value = "1" Then
                .Visible = False
            Else
                If cRec.Avatar <> "" Then
                    .ImageUrl = "Plugins/Avatar/" & cRec.Avatar
                End If
            End If
        End With

        CType(e.Item.FindControl("b07WorkflowInfo"), Label).Text = cRec.b07WorkflowInfo

        With CType(e.Item.FindControl("aAnswer"), HyperLink)
            If hidIsClueTip.Value = "1" Then
                .Visible = False
            Else
                If cRec.o43ID = 0 Then
                    .NavigateUrl = "javascript:" & Me.hidJS_Reaction.Value & "(" & cRec.PID.ToString & ")"
                Else
                    .Visible = False
                End If
            End If


        End With
        With CType(e.Item.FindControl("aDelete"), HyperLink)
            If hidIsClueTip.Value = "1" Then
                .Visible = False
            Else
                If (cRec.j02ID_Owner = _sysUser.j02ID Or _sysUser.IsAdmin) And (cRec.b07Value <> "" Or bolAtts) Then
                    .Visible = True
                    .NavigateUrl = "javascript:trydeleteb07(" & cRec.PID.ToString & ")"
                Else
                    .Visible = False
                End If
            End If

        End With
        With CType(e.Item.FindControl("aMSG"), HyperLink)
            If cRec.o43ID = 0 Then
                .Visible = False
            Else
                .Visible = True
                .NavigateUrl = "binaryfile.aspx?format=msg&prefix=o43&pid=" & cRec.o43ID.ToString
            End If
        End With
        With CType(e.Item.FindControl("aEML"), HyperLink)
            If cRec.o43ID = 0 Then
                .Visible = False
            Else
                .Visible = True
                .NavigateUrl = "binaryfile.aspx?format=eml&prefix=o43&pid=" & cRec.o43ID.ToString
            End If
        End With
        With CType(e.Item.FindControl("aAtts"), Label)
            If cRec.o43Attachments = "" Then
                .Visible = False
            Else
                .Visible = True
                Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(cRec.o43Attachments, ",")
                For Each strAtt As String In lis
                    .Text += "<a href='binaryfile.aspx?prefix=o43&pid=" & cRec.o43ID.ToString & "&disposition=inline&att=" & strAtt & "' style='margin-left:6px;'><img src='Images/attachment.png'/>" & strAtt & "</a>"
                Next
            End If
        End With
        CType(e.Item.FindControl("Timestamp"), Label).Text = BO.BAS.FD(cRec.DateInsert, True, True)
     
    End Sub
End Class