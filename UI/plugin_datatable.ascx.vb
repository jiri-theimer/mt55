Public Class plugin_datatable
    Inherits System.Web.UI.UserControl
    Private _tabDefinition As BO.PluginDataTable
    Public Sub New()
        _tabDefinition = New BO.PluginDataTable
    End Sub
    Public Property TableID As String
        Get
            Return hidTableID.Value
        End Get
        Set(value As String)
            hidTableID.Value = value
        End Set
    End Property
    Public Property TableCaption As String
        Get
            Return hidTableCaption.Value
        End Get
        Set(value As String)
            hidTableCaption.Value = value
        End Set
    End Property
    Public Property TableCssClass As String
        Get
            Return hidTableCssClass.Value
        End Get
        Set(value As String)
            hidTableCssClass.Value = value
        End Set
    End Property
    Public Property ColHeaders As String
        Get
            Return hidColHeaders.Value
        End Get
        Set(value As String)
            hidColHeaders.Value = value
        End Set
    End Property
    Public Property ColTypes As String
        Get
            Return hidColTypes.Value
        End Get
        Set(value As String)
            hidColTypes.Value = value
        End Set
    End Property
    Public Property ColFlexSubtotals As String
        Get
            Return hidColFlexSubtotals.Value
        End Get
        Set(value As String)
            hidColFlexSubtotals.Value = value
        End Set
    End Property
    Public Property NoDataMessage As String
        Get
            Return Me.hidNoDataMessage.Value
        End Get
        Set(value As String)
            hidNoDataMessage.Value = value
        End Set
    End Property
    Public Property IsShowGrandTotals As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsShowGrandTotals.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsShowGrandTotals.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property IsHideRepeatedGroupValue As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsHideRepeatedGroupValues.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsHideRepeatedGroupValues.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property ColHideRepeatedValues As String
        Get
            Return hidColHideRepeatedValues.Value
        End Get
        Set(value As String)
            hidColHideRepeatedValues.Value = value
        End Set
    End Property
    Public Property FormatDate As String
        Get
            Return hidFormatDate.Value
        End Get
        Set(value As String)
            hidFormatDate.Value = value
        End Set
    End Property
    Public Property FormatDateTime As String
        Get
            Return hidFormatDateTime.Value
        End Get
        Set(value As String)
            hidFormatDateTime.Value = value
        End Set
    End Property
    Public Property FormatNumber As String
        Get
            Return hidFormatNumber.Value
        End Get
        Set(value As String)
            hidFormatNumber.Value = value
        End Set
    End Property
    Public ReadOnly Property ErrorMessage As String
        Get
            Return hidErrorMessage.Value
        End Get
    End Property
    Public ReadOnly Property GeneratedRowsCount As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidRowsCount.Value)
        End Get
    End Property


    Public Sub AddDbParameter(parName As String, parValue As Object)
        _tabDefinition.AddDbParameter(parName, parValue)
    End Sub
    Public Sub GenerateTable(factory As BL.Factory, strSQL As String)
        With _tabDefinition
            .SQL = strSQL
            .ColHeaders = Me.ColHeaders
            .ColTypes = Me.ColTypes
            .ColFlexSubtotals = Me.ColFlexSubtotals
            .IsShowGrandTotals = Me.IsShowGrandTotals
            .IsHideRepeatedGroupValues = Me.IsHideRepeatedGroupValue
            .ColHideRepeatedValues = Me.ColHideRepeatedValues
            .TableCaption = Me.TableCaption
            .TableID = Me.TableID
            .FormatDate = Me.FormatDate
            .FormatDateTime = Me.FormatDateTime
            .FormatNumber = Me.FormatNumber
            .NoDataMessage = Me.NoDataMessage
        End With

        place1.Controls.Add(New LiteralControl(factory.pluginBL.CreateDataTableIntoString(_tabDefinition)))
        Me.hidRowsCount.Value = factory.pluginBL.RowsCount.ToString
    End Sub


    
    
End Class