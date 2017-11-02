Public Class PluginDbParameter
    Public Property Name As String
    Public Property Value As Object


    Public Sub New(parName As String, parValue As Object)
        Me.Name = parName
        Me.Value = parValue

    End Sub
End Class
Public Class PluginDataTable
    Public Property TableCssClass As String = "PluginDataTable"
    Public Property TableID As String
    Public Property SQL As String
    Public Property ColHeaders As String
    Public Property ColTypes As String
    Public Property PivotColHeaders As String
    Public Property IsExcelFormat As Boolean
    Public Property ColsCountWithoutPivot As Integer
    Public Property ColClasses As String
    Public Property CultureCode As String
    Public Property FormatNumber As String = "N"
    Public Property FormatDate As String = ""
    Public Property FormatDateTime As String = ""
    Public Property ColFlexSubtotals As String
    Public Property TOTAL_Caption As String = "Celkem"
    Public Property GRANDTOTAL_CAPTION As String = "Celkem"
    Public Property IsHideRepeatedGroupValues As Boolean = True
    Public Property ColHideRepeatedValues As String
    Public Property IsWithoutTABLE_ELEMENT As Boolean = False
    Public Property IsShowGrandTotals As Boolean
    Public Property TableCaption As String
    Public Property NoDataMessage As String = ""

    Public Property DbParameters As List(Of PluginDbParameter)

    Public Sub AddDbParameter(strName As String, Value As Object)
        If DbParameters Is Nothing Then DbParameters = New List(Of PluginDbParameter)
        Dim c As New PluginDbParameter(strName, Value)
        DbParameters.Add(c)
    End Sub
End Class
