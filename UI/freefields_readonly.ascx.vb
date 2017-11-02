Public Class freefields_readonly
    Inherits System.Web.UI.UserControl
    Private _Error As String
    Private _dataprefix As String
    Private _lastX27ID As Integer
    Private _bolIncludeEmptyValues As Boolean

    Public Factory As BL.Factory

    Public Function IsEmpty() As Boolean
        If rpFF.Items.Count > 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Public ReadOnly Property FieldsCount As Integer
        Get
            Return rpFF.Items.Count
        End Get
    End Property
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Property DataTable() As String
        Get
            Return hidDataTable.Value
        End Get
        Set(ByVal value As String)
            hidDataTable.Value = value
            _dataprefix = Left(value, 3)
        End Set
    End Property

    Public Sub FillData(ByVal listFF As IEnumerable(Of BO.FreeField), bolIncludeEmptyValues As Boolean)
        _bolIncludeEmptyValues = bolIncludeEmptyValues
        If Not _bolIncludeEmptyValues Then
            listFF = listFF.AsEnumerable.Where(Function(p) Not (p.DBValue Is System.DBNull.Value Or p.DBValue Is Nothing))
        End If
        rpFF.DataSource = listFF
        
        rpFF.DataBind()
        If Me.rpFF.Items.Count = 0 Then
            Me.panContainer.Visible = False
        Else
            Me.panContainer.Visible = True
        End If
    End Sub

    Private Sub rpFF_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpFF.ItemDataBound
        Dim cRec As BO.FreeField = CType(e.Item.DataItem, BO.FreeField)
        If Not (cRec.DBValue Is System.DBNull.Value Or cRec.DBValue Is Nothing) Or _bolIncludeEmptyValues Then
            If BO.BAS.IsNullInt(cRec.x27ID) <> _lastX27ID Then
                e.Item.FindControl("trHeader").Visible = True
                CType(e.Item.FindControl("headerFF"), Label).Text = cRec.x27Name
            End If
            _lastX27ID = BO.BAS.IsNullInt(cRec.x27ID)
            With CType(e.Item.FindControl("lblFF"), Label)
                .Text = cRec.x28Name & ":"
            End With

            CType(e.Item.FindControl("hidField"), HiddenField).Value = cRec.x28Field
            CType(e.Item.FindControl("hidType"), HiddenField).Value = cRec.TypeName
            CType(e.Item.FindControl("hidX28ID"), HiddenField).Value = cRec.PID.ToString

            If Not (cRec.DBValue Is Nothing Or cRec.DBValue Is System.DBNull.Value) Then
                With CType(e.Item.FindControl("valFF"), Label)
                    Select Case cRec.TypeName
                        Case "string"
                            If cRec.x28TextboxHeight > 0 Then
                                .Text = BO.BAS.CrLfText2Html(cRec.DBValue)
                            Else
                                .Text = cRec.DBValue
                            End If
                            If .Text = "" Then DoHide(e)
                        Case "boolean"
                            If cRec.DBValue = True Then
                                .Text = "ANO"
                            Else
                                .Text = "NE"
                            End If
                        Case "decimal"
                            .Text = BO.BAS.FN(cRec.DBValue)
                            If cRec.DBValue = 0 Then DoHide(e)
                        Case "integer"
                            If cRec.x23ID = 0 Then
                                .Text = BO.BAS.FNI(cRec.DBValue)
                            Else
                                If cRec.DBValue > 0 Then
                                    .Text = cRec.ComboText
                                End If
                            End If
                            If cRec.DBValue = 0 And Not _bolIncludeEmptyValues Then DoHide(e)
                        Case "datetime"
                            .Text = Format(cRec.DBValue, "dd.MM.yyyy HH:mm")
                        Case "date"

                            .Text = Format(cRec.DBValue, "dd.MM.yyyy")
                        Case "time"
                            .Text = Format(cRec.DBValue, "HH:mm")
                        Case Else
                    End Select

                End With
            End If
        End If
    End Sub

    Private Sub DoHide(e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        e.Item.FindControl("valFF").Visible = False
        e.Item.FindControl("lblFF").Visible = False
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class