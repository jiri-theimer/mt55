﻿Public Class schedulers
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "o22"
                
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("schedulers-o25ids")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    

                End With
            End With

            SetupO25()
        End If
    End Sub

    Private Sub SetupO25()
        Dim lis As IEnumerable(Of BO.o25App) = Master.Factory.o25AppBL.GetList(New BO.myQuery).Where(Function(p) p.o25AppFlag = BO.o25AppFlagENUM.GoogleCalendar And p.o25IsMainMenu = True)
        rp1.DataSource = lis
        rp1.DataBind()

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.o25App = CType(e.Item.DataItem, BO.o25App)
        With CType(e.Item.FindControl("ch1"), CheckBox)
            .Text = cRec.o25Name
        End With
        CType(e.Item.FindControl("o25Code"), HiddenField).Value = cRec.o25Code

    End Sub

    Private Sub schedulers_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

        Dim src As New List(Of String), strURL As String = ""

        For Each ri As RepeaterItem In rp1.Items
            With CType(ri.FindControl("chk1"), CheckBox)
                If .Checked Then
                    src.Add("src=" & CType(ri.FindControl("o25Code"), HiddenField).Value)

                End If
            End With
        Next
        If src.Count = 0 Then

        Else
            strURL = "https://calendar.google.com/calendar/embed?" & String.Join(";", src)
        End If


    End Sub
End Class