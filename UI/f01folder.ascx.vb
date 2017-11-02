Public Class f01folder
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory

    Public Property Prefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(intRecordPID As Integer)
        Dim lis As IEnumerable(Of BO.f01Folder) = Me.Factory.f01FolderBL.GetList(New BO.myQuery, intRecordPID, BO.BAS.GetX29FromPrefix(hidPrefix.Value))
        rpFolders.DataSource = lis
        rpFolders.DataBind()
    End Sub

    Private Sub rpFolders_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpFolders.ItemDataBound
        Dim cRec As BO.f01Folder = CType(e.Item.DataItem, BO.f01Folder)
        With CType(e.Item.FindControl("link1"), HyperLink)
            .NavigateUrl = "file:///" & cRec.f02RootPath & "\" & cRec.f01Name
            .Text = cRec.f01Name
            .ToolTip = cRec.f02Name
        End With
    End Sub
End Class