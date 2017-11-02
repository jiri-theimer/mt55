Public Class entity_framework_detail_missing
    Inherits System.Web.UI.Page
  

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("masterpid") = Request.Item("masterpid")
            ViewState("masterprefix") = Request.Item("masterprefix")
            If ViewState("masterprefix") <> "" Then
                panStat.Visible = False
                Me.MasterRecord.Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(ViewState("masterprefix")), BO.BAS.IsNullInt(ViewState("masterpid")))
                Me.MasterRecord.NavigateUrl = ViewState("masterprefix") & "_framework.aspx?pid=" & ViewState("masterpid")
            End If
            ViewState("prefix") = Request.Item("prefix")
            Master.SiteMenuValue = Request.Item("prefix")
            Select Case BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
                Case BO.x29IdEnum.p28Contact

                    img1.ImageUrl = "Images/contact_32.png"
                    Dim mq As New BO.myQueryP28
                    mq.SpecificQuery = BO.myQueryP28_SpecificQuery.AllowedForRead
                    Dim lis As IEnumerable(Of BO.p28Contact) = Master.Factory.p28ContactBL.GetList(mq)
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Klienti"
                    ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P28_Creator) Then
                    ''    cmdNew.Visible = True : cmdNew.Text = "Založit nového klienta"
                    ''    cmdNew.NavigateUrl = "javascript:p28_create()"
                    ''End If

                Case BO.x29IdEnum.p41Project

                    img1.ImageUrl = "Images/project_32.png"
                    Dim mq As New BO.myQueryP41
                    mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
                    Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Projekty"
                    ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P41_Creator, BO.x53PermValEnum.GR_P41_Draft_Creator) Then
                    ''    cmdNew.Visible = True : cmdNew.Text = "Založit nový projekt"
                    ''    cmdNew.NavigateUrl = "javascript:p41_create()"
                    ''End If
                Case BO.x29IdEnum.p56Task

                    img1.ImageUrl = "Images/task_32.png"
                    Dim mq As New BO.myQueryP56
                    mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
                    Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq)
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Úkoly"
                    ''cmdNew.Visible = True : cmdNew.Text = "Založit nový úkol"
                    ''cmdNew.NavigateUrl = "javascript:p56_create()"
                Case BO.x29IdEnum.p91Invoice
                    img1.ImageUrl = "Images/invoice_32.png"

                    Dim mq As New BO.myQueryP91
                    mq.SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead
                    Dim lis As IEnumerable(Of BO.p91Invoice) = Master.Factory.p91InvoiceBL.GetList(mq)
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Klientské faktury"
                    ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator) Then
                    ''    cmdNew.Visible = True : cmdNew.Text = "Vytvořit novou fakturu"
                    ''    cmdNew.NavigateUrl = "javascript:p91_create()"
                    ''End If
                Case BO.x29IdEnum.j02Person

                    img1.ImageUrl = "Images/person_32.png"
                    Dim mq As New BO.myQueryJ02
                    Dim lis As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq)
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Lidé | Osoby"
                    ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
                    ''    cmdNew.Visible = True : cmdNew.Text = "Založit novou osobu"
                    ''    cmdNew.NavigateUrl = "javascript:j02_create()"
                    ''End If
                Case BO.x29IdEnum.x31Report

                    img1.ImageUrl = "Images/reporting_32.png"

                    Dim lis As IEnumerable(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList()
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Tiskové sestavy | Pluginy"
                    ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
                    ''    cmdNew.Visible = True : cmdNew.Text = "Nahrát do systému novou šablonu sestavy/pluginu"
                    ''    cmdNew.NavigateUrl = "javascript:x31_create()"
                    ''End If
                Case BO.x29IdEnum.o23Doc

                    img1.ImageUrl = "Images/notepad_32.png"
                    Dim mq As New BO.myQueryO23(0)
                    mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
                    Dim lis As IEnumerable(Of BO.o23Doc) = Master.Factory.o23DocBL.GetList(mq)
                    Me.Count4Read.Text = BO.BAS.FNI(lis.Count)
                    Me.CountBin.Text = BO.BAS.FNI(lis.Where(Function(p) p.IsClosed = True).Count)
                    lblHeader.Text = "Dokumenty"
                 
                Case Else
                    img1.Visible = False

            End Select


        End If
    End Sub

    Private Sub entity_framework_detail_missing_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
       
        
    End Sub
End Class