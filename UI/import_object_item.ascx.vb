Public Class import_object_item
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Private Property _lis As IEnumerable(Of BO.p85TempBox) = Nothing
    Public ReadOnly Property GUID As String
        Get
            Return hidGUID.Value
        End Get
    End Property
    Public ReadOnly Property FilesCount As Integer
        Get
            Return rp1.Items.Count
        End Get
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub InhaleObjectRecord(strGUID As String, strPrefix As String, bolShowFileCheckboxlist As Boolean)
        hidGUID.Value = strGUID
        hidPrefix.Value = strPrefix

        LoadList()
        If bolShowFileCheckboxlist Then
            rp1.DataSource = _lis.Where(Function(p) p.p85Prefix = "o27")
            rp1.DataBind()
        Else
            rp1.Visible = False
        End If
        

    End Sub
    Private Sub LoadList()
        _lis = Me.Factory.p85TempBoxBL.GetList(hidGUID.Value, True)
    End Sub
    Private Function GetRec(strKey As String) As BO.p85TempBox
        If _lis Is Nothing Then
            LoadList()
        End If
        If _lis.Where(Function(p) LCase(p.p85FreeText02) = LCase(strKey)).Count > 0 Then
            Return _lis.Where(Function(p) LCase(p.p85FreeText02) = LCase(strKey))(0)
        Else
            Return New BO.p85TempBox
        End If
    End Function
    Public Function IsMailMessage() As Boolean
        If _lis.Where(Function(p) p.p85Prefix = "o27" And LCase(p.p85FreeText01).IndexOf(".msg") > 0).Count > 0 Then
            Return True
        End If
        If _lis.Where(Function(p) p.p85Prefix = "o27" And LCase(p.p85FreeText01).IndexOf(".eml") > 0).Count > 0 Then
            Return True
        End If
    End Function

    
    Public Function FindString(strKey As String) As String
        Return GetRec(strKey).p85Message
    End Function
    Public Function FindDate(strKey As String) As Date
        Return GetRec(strKey).p85FreeDate01

    End Function
    Public Function FindDouble(strKey As String) As Double
        Return GetRec(strKey).p85FreeFloat01
    End Function
    Public Function FindInteget(strKey As String) As Integer
        Return GetRec(strKey).p85OtherKey1
    End Function
    Public Function FindBoolean(strKey As String) As Boolean
        Return GetRec(strKey).p85FreeBoolean01
    End Function

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        
        With CType(e.Item.FindControl("link1"), HyperLink)
            .Text = cRec.p85FreeText01
            If LCase(.Text).IndexOf(".eml") > 0 Then
                .Text = "EML soubor poštovní zprávy"
            End If
            If LCase(.Text).IndexOf(".msg") > 0 Then
                .Text = "Outlook MSG soubor poštovní zprávy"
            End If
            .Text = "<img src='Images/Files/" & BO.BAS.GetFileExtensionIcon(Right(cRec.p85FreeText02, 4)) & "'/>" & .Text
            .NavigateUrl = "binaryfile.aspx?tempfile=" & cRec.p85FreeText02
        End With
        With CType(e.Item.FindControl("FileSize"), Label)
            .Text = BO.BAS.FormatFileSize(CInt(cRec.p85FreeNumber01))
        End With
    End Sub


    Public Sub PrepareTempFileUpload(strFileUploadGUID As String)
        LoadList()
        For Each c In _lis.Where(Function(p) p.p85Prefix = "o27")
            Me.Factory.p85TempBoxBL.CloneOneRecord(c.PID, strFileUploadGUID)
        Next
    End Sub
End Class