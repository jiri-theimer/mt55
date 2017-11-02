Public Class changelog
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub changelog_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                hidPrefix.Value = Request.Item("prefix")
                If hidPrefix.Value = "" Or .DataPID = 0 Then .StopPage("pid or prefix missing")
                .HeaderText = "CHANGE-LOG | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(hidPrefix.Value), .DataPID)
                
                With .Factory.j03UserBL
                    .InhaleUserParams("changelog-view")
                    basUI.SelectRadiolistValue(Me.opgView, .GetUserParam("changelog-view", "2"))
                End With

                Select Case hidPrefix.Value
                    Case "p41"
                        Dim c As BO.p41Project = .Factory.p41ProjectBL.Load(.DataPID)
                        Me.Timestamp.Text = c.Timestamp
                        Dim cDISP As BO.p41RecordDisposition = .Factory.p41ProjectBL.InhaleRecordDisposition(c)
                        If Not cDISP.OwnerAccess Then .StopPage("K projektu nemáte dostatečné oprávnění.")

                    Case "p28" : Me.Timestamp.Text = .Factory.p28ContactBL.Load(.DataPID).Timestamp
                    Case "j02" : Me.Timestamp.Text = .Factory.j02PersonBL.Load(.DataPID).Timestamp
                    Case "p91" : Me.Timestamp.Text = .Factory.p91InvoiceBL.Load(.DataPID).Timestamp
                    Case "p56" : Me.Timestamp.Text = .Factory.p56TaskBL.Load(.DataPID).Timestamp
                    Case "p31"
                        If Not .Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                            .StopPage("K této funkci nemáte dostatečné oprávnění.")
                        End If
                        Dim cDISP As BO.p31WorksheetDisposition = .Factory.p31WorksheetBL.InhaleRecordDisposition(.DataPID)
                        If cDISP.RecordDisposition = BO.p31RecordDisposition._NoAccess Then
                            .StopPage("K záznamu nemáte oprávnění.")
                        End If
                End Select
            End With

            With g1
                .radGridOrig.ShowFooter = False

            End With
            
            SetupGrid()
        End If
    End Sub

    Private Sub SetupGrid()
        g1.ClearColumns()
        Dim dt As DataTable = Master.Factory.ftBL.GetChangeLog(hidPrefix.Value, Master.DataPID)
        If dt.Rows.Count = 0 Then
            lblMessage.Text = "CHANGE-LOG historie tohoto záznamu je zatím prázdná.<hr>Je to tím, že byl pořízen v době, kdy ještě MARKTIME nepodporoval CHANGE-LOG funkcionalitu.<hr>K prvnímu zápisu do historie dojde po pře-uložení záznamu."
            g1.Visible = False
            opgView.Visible = False

            Return
        End If
       
        For Each col As DataColumn In dt.Columns
            Dim bol As Boolean = False
            If col.ColumnName <> "pid" Then
                Select Case opgView.SelectedValue
                    Case "1"
                        bol = True
                    Case "2"
                        bol = HasValue(dt, col.ColumnName)
                    Case "3"
                        bol = HasChangedValue(dt, col.ColumnName)
                End Select
            End If

            If bol Then
                Select Case col.DataType.Name
                    Case "Boolean"
                        g1.AddColumn(col.ColumnName, col.ColumnName, BO.cfENUM.Checkbox)
                    Case "DateTime"
                        g1.AddColumn(col.ColumnName, col.ColumnName, BO.cfENUM.DateTime)
                    Case "Double", "Decimal"
                        g1.AddColumn(col.ColumnName, col.ColumnName, BO.cfENUM.Numeric)
                    Case "Int32"
                        g1.AddColumn(col.ColumnName, col.ColumnName, BO.cfENUM.Numeric0)
                    Case Else
                        g1.AddColumn(col.ColumnName, col.ColumnName)
                End Select
            End If


            ''lbl1.Text = lbl1.Text & "<hr>" & col.ColumnName & ": " & col.DataType.Name
        Next


    End Sub

    Private Function HasValue(dt As DataTable, strColName As String) As Boolean
        Dim lis As IEnumerable(Of Object) = dt.AsEnumerable.Select(Function(p) p.Item(strColName)).Distinct
        If lis.Count = 0 Then Return False

        If lis.Count = 1 Then
            If lis(0) Is System.DBNull.Value Then Return False
            Try
                If lis(0) = False Then Return False
            Catch ex As Exception

            End Try
            Try
                If lis(0) = 0 Then Return False
            Catch ex As Exception

            End Try

            Return True
        Else
            Return True
        End If

       
        
    End Function
    Private Function HasChangedValue(dt As DataTable, strColName As String) As Boolean
        Dim lis As IEnumerable(Of Object) = dt.AsEnumerable.Select(Function(p) p.Item(strColName)).Distinct
        If lis.Count > 1 Then
            Return True
        End If

        Return False


    End Function

    

    Private Sub g1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles g1.NeedDataSource
        Dim dt As DataTable = Master.Factory.ftBL.GetChangeLog(hidPrefix.Value, Master.DataPID)
        g1.DataSourceDataTable = dt
    End Sub

    Private Sub opgView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgView.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("changelog-view", Me.opgView.SelectedValue)
        Response.Redirect("changelog.aspx?prefix=" & hidPrefix.Value & "&pid=" & Master.DataPID.ToString, True)
    End Sub

  
    Private Sub cmdDOC_Click(sender As Object, e As EventArgs) Handles cmdDOC.Click
        basUIMT.Handle_GridTelerikExport(g1, "doc")
    End Sub

    Private Sub cmdPDF_Click(sender As Object, e As EventArgs) Handles cmdPDF.Click
        basUIMT.Handle_GridTelerikExport(g1, "pdf")
    End Sub

    Private Sub cmdXLS_Click(sender As Object, e As EventArgs) Handles cmdXLS.Click
        basUIMT.Handle_GridTelerikExport(g1, "xls")
    End Sub

    Private Sub changelog_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If g1.RowsCount = 0 Then
            panExport.Visible = False
        Else
            panExport.Visible = True
        End If
    End Sub
End Class