Public Class clue_j02_record
    Inherits System.Web.UI.Page
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            linkGoTo.Visible = Master.Factory.SysUser.j04IsMenu_People
            RefreshRecord()

        End If
    End Sub
    Private Sub RefreshRecord()
        Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .FullNameAsc
            If .j02IsInvoiceEmail Then ph1.ForeColor = Drawing.Color.Green
            If .IsClosed Then ph1.Font.Strikeout = True
            Me.j07Name.Text = .j07Name
            Me.j02Code.Text = .j02Code
            Me.c21Name.Text = .c21Name
            Me.j02Email.Text = .j02Email
            Me.j02Email.NavigateUrl = "mailto:" & .j02Email
            Me.j18Name.Text = .j18Name
            Me.j02Office.Text = .j02Office
            Me.j02Mobile.Text = .j02Mobile
            Me.j02Phone.Text = .j02Phone
            Me.j02JobTitle.Text = .j02JobTitle
            Me.j11Names.Text = Master.Factory.j02PersonBL.GetTeamsInLine(.PID)
            panIntraOnly.Visible = .j02IsIntraPerson
            If .j02Phone <> "" Or .j02JobTitle <> "" Or .j02Office <> "" Or .j02Mobile <> "" Then

            Else
                panContacts.Visible = False
            End If
        End With

        If cRec.j02IsIntraPerson Then
            Dim mq As New BO.myQueryJ03
            mq.j02ID = cRec.PID
            Dim lis As IEnumerable(Of BO.j03User) = Master.Factory.j03UserBL.GetList(mq)
            If lis.Count > 0 Then
                Me.j03Login.Text = lis(0).j03Login
                Me.j04Name.Text = lis(0).j04Name
            End If
        End If
        panIntraOnly.Visible = cRec.j02IsIntraPerson

        Me.rpP30.DataSource = Master.Factory.p30Contact_PersonBL.GetList(0, 0, cRec.PID)
        Me.rpP30.DataBind()
        If rpP30.Items.Count > 0 Then panContacts.Visible = True
        tags1.RefreshData(cRec.PID)
    End Sub

    Private Sub rpP30_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP30.ItemDataBound
        Dim cRec As BO.p30Contact_Person = CType(e.Item.DataItem, BO.p30Contact_Person)
        With CType(e.Item.FindControl("BindLink"), HyperLink)
            If cRec.p28ID <> 0 Then
                If cRec.p28CompanyName <> "" Then
                    .Text = cRec.p28CompanyName
                Else
                    .Text = cRec.p28Name
                End If
                If Master.Factory.SysUser.j04IsMenu_Contact Then
                    .NavigateUrl = "p28_framework.aspx?pid=" & cRec.p28ID.ToString
                End If
                CType(e.Item.FindControl("imgBind"), Image).ImageUrl = "Images/contact.png"
            End If
            If cRec.p41ID <> 0 Then
                .Text = cRec.Project
                If Master.Factory.SysUser.j04IsMenu_Project Then
                    .NavigateUrl = "p41_framework.aspx?pid=" & cRec.p41ID.ToString
                End If
                CType(e.Item.FindControl("imgBind"), Image).ImageUrl = "Images/project.png"
            End If
        End With
        CType(e.Item.FindControl("p27Name"), Label).Text = cRec.p27Name
    End Sub
End Class