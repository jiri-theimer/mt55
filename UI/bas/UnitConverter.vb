Imports System.Web.Script.Serialization
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Web.UI.WebControls

Public Class UnitConverter
    Inherits JavaScriptConverter
    Public Overloads Overrides Function Deserialize(ByVal dictionary As System.Collections.Generic.IDictionary(Of String, Object), ByVal type As Type, ByVal serializer As JavaScriptSerializer) As Object
        If dictionary Is Nothing Then
            Throw New ArgumentNullException("dictionary")
        End If

        If type Is GetType(Unit) AndAlso dictionary.Count > 0 Then
            If DirectCast(dictionary("IsEmpty"), Boolean) Then
                Return System.Web.UI.WebControls.Unit.Empty
            End If
            Dim value As Double = Convert.ToDouble(dictionary("Value"))
            Dim unitType As UnitType = DirectCast([Enum].Parse(GetType(UnitType), dictionary("Type").ToString()), UnitType)
            Dim unit As New Unit(value, unitType)

            Return unit
        End If

        Return Nothing
    End Function

    Public Overloads Overrides Function Serialize(ByVal obj As Object, ByVal serializer As JavaScriptSerializer) As System.Collections.Generic.IDictionary(Of String, Object)
        Dim unit As Unit = DirectCast(obj, Unit)

        If unit <> Nothing Then
            Dim dict As New Dictionary(Of String, Object)()
            dict.Add("Type", unit.Type)
            dict.Add("Value", unit.Value)
            dict.Add("IsEmpty", unit.IsEmpty)
            Return dict
        End If

        Return New Dictionary(Of String, Object)()
    End Function

    Public Overloads Overrides ReadOnly Property SupportedTypes() As System.Collections.Generic.IEnumerable(Of Type)
        Get
            Return New ReadOnlyCollection(Of Type)(New List(Of Type)(New Type() {GetType(Unit)}))
        End Get
    End Property
End Class
