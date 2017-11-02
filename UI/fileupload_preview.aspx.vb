Public Class fileupload_preview
    Inherits System.Web.UI.Page

    Protected WithEvents _MasterPage As ModalForm

    Private Sub fileupload_preview_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.list1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            With Master
                If (Request.Item("prefix") = "" Or Request.Item("pid") = "") And Request.Item("tempfile") = "" Then
                    .StopPage("prefix or pid missing...")
                End If
                .HeaderText = "Náhled na soubor/přílohu"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Request.Item("tempfile") <> "" Then
                    .AddToolbarButton("Stáhnout", "download", , "Images/download.png", False, "javascript:download2('tempfile=" & Request.Item("tempfile") & "&disposition=attachment')")
                Else
                    .AddToolbarButton("Stáhnout", "download", , "Images/download.png", False, "javascript:download2('prefix=" & Request.Item("prefix") & "&pid=" & Request.Item("pid") & "&disposition=attachment')")
                End If
                
            End With
            ViewState("if1_height") = "90%"
            Dim mq As New BO.myQueryO27
            Select Case Request.Item("prefix")
                Case "o23"
                    Dim cDoc As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
                    Dim cDisp As BO.o23RecordDisposition = Master.Factory.o23DocBL.InhaleDisposition(cDoc)
                    If Not cDisp.ReadAccess Then Master.StopPage("Nemáte přístup k tomuto dokumentu.") : Return

                    Me.panList.Visible = True
                    mq.Record_x29ID = BO.x29IdEnum.o23Doc
                    mq.Record_PID = Master.DataPID

                    Me.list1.RefreshData(mq)
                    If Me.list1.ItemsCount > 0 Then
                        Dim cRec As BO.o27Attachment = Me.list1.GetListO27(0)
                        ViewState("url") = "binaryfile.aspx?prefix=o27&disposition=inline&pid=" & cRec.PID.ToString
                    End If
                    If list1.ItemsCount > 3 Then
                        list1.IsRepeatDirectionVerticaly = True : list1.RepeatColumns = 2
                    End If
                    Master.HeaderText = Master.Factory.GetRecordCaption(BO.x29IdEnum.o23Doc, Master.DataPID)
                    ViewState("if1_height") = "80%"
                Case "o27"
                    Dim cRec As BO.o27Attachment = Master.Factory.o27AttachmentBL.Load(Master.DataPID)
                    ViewState("url") = "binaryfile.aspx?prefix=o27&disposition=inline&pid=" & cRec.PID.ToString
                Case "p85"
                    ViewState("url") = "binaryfile.aspx?prefix=p85&disposition=inline&pid=" & Master.DataPID.ToString
                Case ""
                    If Request.Item("tempfile") <> "" Then
                        ViewState("url") = "binaryfile.aspx?disposition=inline&tempfile=" & Request.Item("tempfile")
                    End If
            End Select


        End If
    End Sub

    

End Class