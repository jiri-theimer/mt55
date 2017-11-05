Public Class EventColor
    Public Property BackColor As String
    Public Property ForeColor As String

    Public Sub New(strColorID As String)
        Select Case strColorID
            Case "1" : Me.BackColor = "#9fc6e7" : Me.ForeColor = "#1d1d1d"
            Case "9" : Me.BackColor = "#4986e7" : Me.ForeColor = "#1d1d1d"
            Case "7" : Me.BackColor = "#30d5c8" : Me.ForeColor = "#1d1d1d"
            Case "2" : Me.BackColor = "#7ae7bf" : Me.ForeColor = "#1d1d1d"
            Case "10" : Me.BackColor = "#b3dc6c" : Me.ForeColor = "#1d1d1d"
            Case "5" : Me.BackColor = "#fbd75b" : Me.ForeColor = "#1d1d1d"
            Case "6" : Me.BackColor = "#ffb878" : Me.ForeColor = "#1d1d1d"
            Case "4" : Me.BackColor = "#ff887c" : Me.ForeColor = "#1d1d1d"
            Case "11" : Me.BackColor = "#dc2127" : Me.ForeColor = "#1d1d1d"
            Case "3" : Me.BackColor = "#dbadff" : Me.ForeColor = "#1d1d1d"
            Case "8" : Me.BackColor = "#e1e1e1" : Me.ForeColor = "#1d1d1d"

                ''Case "12" : Me.BackColor = "#fad165" : Me.ForeColor = "#1d1d1d"
                ''Case "13" : Me.BackColor = "#92e1c0" : Me.ForeColor = "#1d1d1d"
                ''Case "14" : Me.BackColor = "#9fe1e7" : Me.ForeColor = "#1d1d1d"
                ''Case "15" : Me.BackColor = "#9fc6e7" : Me.ForeColor = "#1d1d1d"
                ''Case "16" : Me.BackColor = "#4986e7" : Me.ForeColor = "#1d1d1d"
                ''Case "17" : Me.BackColor = "#9a9cff" : Me.ForeColor = "#1d1d1d"
                ''Case "18" : Me.BackColor = "#b99aff" : Me.ForeColor = "#1d1d1d"
                ''Case "19" : Me.BackColor = "#c2c2c2" : Me.ForeColor = "#1d1d1d"
                ''Case "20" : Me.BackColor = "#cabdbf" : Me.ForeColor = "#1d1d1d"
                ''Case "21" : Me.BackColor = "#cabdbf" : Me.ForeColor = "#1d1d1d"
                ''Case "22" : Me.BackColor = "#f691b2" : Me.ForeColor = "#1d1d1d"
                ''Case "23" : Me.BackColor = "#cd74e6" : Me.ForeColor = "#1d1d1d"
                ''Case "24" : Me.BackColor = "#a47ae2" : Me.ForeColor = "#1d1d1d"
            Case Else
                Me.BackColor = ""
                Me.ForeColor = ""
        End Select
    End Sub

    ''''key=1 ,bg=#ac725e, fc=#1d1d1d
    ''''key=2 ,bg=#d06b64, fc=#1d1d1d
    ''''key=3 ,bg=#f83a22, fc=#1d1d1d
    ''''key=4 ,bg=#fa573c, fc=#1d1d1d
    ''key=5 ,bg=#ff7537, fc=#1d1d1d
    ''key=6 ,bg=#ffad46, fc=#1d1d1d
    ''key=7 ,bg=#42d692, fc=#1d1d1d
    ''key=8 ,bg=#16a765, fc=#1d1d1d
    ''key=9 ,bg=#7bd148, fc=#1d1d1d
    ''key=10 ,bg=#b3dc6c, fc=#1d1d1d
    ''key=11 ,bg=#fbe983, fc=#1d1d1d
    ''key=12 ,bg=#fad165, fc=#1d1d1d
    ''key=13 ,bg=#92e1c0, fc=#1d1d1d
    ''key=14 ,bg=#9fe1e7, fc=#1d1d1d
    ''key=15 ,bg=#9fc6e7, fc=#1d1d1d
    ''key=16 ,bg=#4986e7, fc=#1d1d1d
    ''key=17 ,bg=#9a9cff, fc=#1d1d1d
    ''key=18 ,bg=#b99aff, fc=#1d1d1d
    ''key=19 ,bg=#c2c2c2, fc=#1d1d1d
    ''key=20 ,bg=#cabdbf, fc=#1d1d1d
    ''key=21 ,bg=#cca6ac, fc=#1d1d1d
    ''key=22 ,bg=#f691b2, fc=#1d1d1d
    ''key=23 ,bg=#cd74e6, fc=#1d1d1d
    ''key=24 ,bg=#a47ae2, fc=#1d1d1d

End Class
