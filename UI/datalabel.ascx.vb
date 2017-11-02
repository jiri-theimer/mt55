Public Class datalabel
    Inherits System.Web.UI.UserControl

    Public Overridable Property Text() As String
        Get
            Return lbl1.Text
        End Get
        Set(ByVal value As String)
            lbl1.Text = value
        End Set

    End Property
    Public Property ToolTip() As String
        Get
            Return lbl1.ToolTip
        End Get
        Set(ByVal value As String)
            lbl1.ToolTip = value
        End Set

    End Property
    Public Property IsRequired As Boolean
        Get
            If lbl1.CssClass = "lblReq" Then Return True Else Return False
        End Get
        Set(value As Boolean)
            If value Then lbl1.CssClass = "lblReq" Else lbl1.CssClass = "lbl"
        End Set
    End Property
    Public Overridable Property CssClass() As String
        Get
            Return lbl1.CssClass
        End Get
        Set(ByVal value As String)
            lbl1.CssClass = value

        End Set
    End Property

    Public Overridable Property Color() As String
        Get
            Return lbl1.Style.Item("color")
        End Get
        Set(ByVal value As String)
            lbl1.Style.Item("color") = value

        End Set
    End Property

    Public Overridable Property AssociatedControlID() As String
        Get
            Return lbl1.AssociatedControlID
        End Get
        Set(ByVal value As String)
            lbl1.AssociatedControlID = value
        End Set
    End Property

    Public Property GLX() As String
        Get
            Return lbl1.Attributes.Item("glx")
        End Get
        Set(ByVal value As String)
            lbl1.Attributes.Item("glx") = value
        End Set
    End Property


    Public Property JTI1() As String
        Get
            Return lbl1.Attributes.Item("jti1")
        End Get
        Set(ByVal value As String)
            Return 'dočasně vypnuto - 21.11.2012
            lbl1.Attributes.Item("jti1") = value
            If value = "" Then
                place1.Controls.Clear()
                place2.Controls.Clear()
            Else
                place1.Controls.Add(New LiteralControl("<span class='formInfo'>"))
                place2.Controls.Add(New LiteralControl("<a rel='jtip.aspx?x=" & value & "' id='jtip_" & Me.ClientID & "' title='HELP | INFO'  class='jti1'>?</a></span>"))
            End If
        End Set
    End Property
    Public Property JTI2() As String
        Get
            Return lbl1.Attributes.Item("jti2")
        End Get
        Set(ByVal value As String)
            Return 'dočasně vypnuto - 21.11.2012
            lbl1.Attributes.Item("jti2") = value
            If value = "" Then
                place1.Controls.Clear()
                place2.Controls.Clear()
            Else
                place1.Controls.Add(New LiteralControl("<span class='formInfo'>"))
                place2.Controls.Add(New LiteralControl("<a rel='jtip.aspx?x=" & value & "' id='jtip_" & Me.ClientID & "' class='jti2'>?</a></span>"))
            End If
        End Set
    End Property
    Public Property DataType() As String
        Get
            Return lbl1.Attributes.Item("datatype")
        End Get
        Set(ByVal value As String)
            lbl1.Attributes.Item("datatype") = LCase(value)
        End Set
    End Property
    Public Property DataFormat() As String
        Get
            Return lbl1.Attributes.Item("dataformat")
        End Get
        Set(ByVal value As String)
            lbl1.Attributes.Item("dataformat") = value
        End Set
    End Property
    Public Property DataValue() As Object
        Get
            Return ViewState("value")
        End Get
        Set(ByVal value As Object)
            ViewState("value") = value
            If value Is Nothing Then
                lbl1.Text = "" : Return
            End If
            lbl1.CssClass = "valbold"
            If value Is System.DBNull.Value Then
                lbl1.Text = "" : Return
            End If
            Select Case Me.DataType
                Case "", "text"
                    If Me.DataFormat = "" Then lbl1.Text = value.ToString Else lbl1.Text = Format(value, Me.DataFormat)
                Case "date"
                    If Me.DataFormat = "" Then lbl1.Text = BO.BAS.FD(value, False) Else lbl1.Text = Format(value, Me.DataFormat)
                Case "datetime"
                    If Me.DataFormat = "" Then lbl1.Text = BO.BAS.FD(value, True) Else lbl1.Text = Format(value, Me.DataFormat)
                Case "boolean"
                    If Me.DataFormat = "" Then
                        If value = True Then lbl1.Text = "ANO" Else lbl1.Text = "NE"
                    Else
                        lbl1.Text = Format(value, Me.DataFormat)
                    End If
                Case "number"
                    If Me.DataFormat = "" Then Me.DataFormat = "standard"
                    lbl1.Text = Format(value, Me.DataFormat)
                Case "h"
                    If Me.DataFormat = "hhmm" Then
                        Dim c As New COM.clsTime
                        lbl1.Text = c.ShowAsHHMM(value)
                    Else
                        lbl1.Text = Format(value, "Standard")
                    End If
            End Select
        End Set
    End Property





    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class