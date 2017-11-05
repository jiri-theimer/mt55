Public Class schedulers
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
                    
                    SetupO25(.GetUserParam("schedulers-o25ids"))
                End With
            End With


        End If
    End Sub

    Private Sub SetupO25(strO25IDs As String)
        Dim lis As IEnumerable(Of BO.o25App) = Master.Factory.o25AppBL.GetList(New BO.myQuery).Where(Function(p) p.o25AppFlag = BO.o25AppFlagENUM.GoogleCalendar And p.o25IsMainMenu = True)
        rp1.DataSource = lis
        rp1.DataBind()

        If strO25IDs <> "" Then
            Dim o25ids As List(Of String) = BO.BAS.ConvertDelimitedString2List(strO25IDs)
            For Each strO25ID In o25ids
                For Each ri As RepeaterItem In rp1.Items
                    With CType(ri.FindControl("chk1"), CheckBox)
                        If CType(ri.FindControl("o25id"), HiddenField).Value = strO25ID Then
                            .Checked = True
                        End If
                    End With
                Next
            Next
        End If
        
        
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.o25App = CType(e.Item.DataItem, BO.o25App)
        With CType(e.Item.FindControl("chk1"), CheckBox)
            .Text = cRec.o25Name
        End With
        CType(e.Item.FindControl("o25Code"), HiddenField).Value = cRec.o25Code
        CType(e.Item.FindControl("o25id"), HiddenField).Value = cRec.PID.ToString

    End Sub

    Private Sub schedulers_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

        Dim src As New List(Of String), strURL As String = "https://calendar.google.com/calendar/embed?wkst=1", o25IDs As New List(Of String)

        For Each ri As RepeaterItem In rp1.Items
            With CType(ri.FindControl("chk1"), CheckBox)
                If .Checked Then
                    strURL += "&amp;src=" & CType(ri.FindControl("o25Code"), HiddenField).Value
                    o25IDs.Add(CType(ri.FindControl("o25id"), HiddenField).Value)
                End If
            End With
        Next
        If o25IDs.Count = 0 Then
            strURL = "blank.aspx"
        End If
        Master.Factory.j03UserBL.SetUserParam("schedulers-o25ids", String.Join(",", o25IDs))

        fra1.Src = strURL
       

    End Sub

    
End Class