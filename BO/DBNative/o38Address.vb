Public Class o38Address
    Inherits BOMother
    Public Property o38Name As String
    Public Property o38Street As String
    Public Property o38City As String
    Public Property o38ZIP As String
    Public Property o38Country As String
    Public Property o38Description As String
    Public Property o38AresID As String

    Public ReadOnly Property FullAddress As String
        Get
            Dim s As String = ""
            If Me.o38Street <> "" Then
                s = Me.o38Street & ", " & Me.o38City
            Else
                s = Me.o38City
            End If
            If Me.o38ZIP <> "" Then
                s += ", " & Me.o38ZIP
            End If
            Return s
        End Get
    End Property
    Public ReadOnly Property FullAddressWithBreaks As String
        Get
            Dim s As String = ""
            If Me.o38Street <> "" Then
                s = Me.o38Street & vbCrLf & Me.o38City
            Else
                s = Me.o38City
            End If
            If Me.o38ZIP <> "" Then
                s += vbCrLf & Me.o38ZIP
            End If
            If Me.o38Country <> "" Then
                s += vbCrLf & Me.o38Country
            End If
            Return s
        End Get
    End Property
End Class
