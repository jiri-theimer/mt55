Public Class entity_framework_rec_p31
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm
    Public Property CurrentMasterPrefix As String
        Get
            Return hidMasterPrefix.Value
        End Get
        Set(value As String)
            hidMasterPrefix.Value = value
        End Set
    End Property

    Private Sub entity_framework_rec_p31_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        menu1.Factory = Master.Factory
        gridP31.Factory = Master.Factory
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
                Me.CurrentMasterPrefix = Request.Item("masterprefix")
                If .DataPID = 0 Or Me.CurrentMasterPrefix = "" Then .StopPage("masterpid or masterprefix is missing")

                .SiteMenuValue = Me.CurrentMasterPrefix
                menu1.DataPrefix = Me.CurrentMasterPrefix
                If Me.menu1.PageSource = "2" Then
                    .IsHideAllRecZooms = True
                End If

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(Me.CurrentMasterPrefix & "_menu-tabskin")
                    .Add(Me.CurrentMasterPrefix & "_menu-x31id-plugin")
                    ''.Add(Me.CurrentMasterPrefix & "_menu-menuskin")
                    .Add(Me.CurrentMasterPrefix & "_menu-remember-tab")
                    .Add(Me.CurrentMasterPrefix & "_framework_detail-tab")
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    menu1.TabSkin = .GetUserParam(Me.CurrentMasterPrefix & "_menu-tabskin")
                    ''menu1.MenuSkin = .GetUserParam(Me.CurrentMasterPrefix & "_menu-menuskin")
                    menu1.x31ID_Plugin = .GetUserParam(Me.CurrentMasterPrefix & "_menu-x31id-plugin")
                    If .GetUserParam(Me.CurrentMasterPrefix & "_menu-remember-tab", "0") = "1" Then
                        menu1.LockedTab = .GetUserParam(Me.CurrentMasterPrefix & "_framework_detail-tab")
                    End If
                End With
            End With
            gridP31.MasterTabAutoQueryFlag = Request.Item("p31tabautoquery")


            If Request.Item("pid") <> "" Then
                gridP31.DefaultSelectedPID = BO.BAS.IsNullInt(Request.Item("pid"))
            End If
        End If
        RefreshRecord()

        menu1.DataPID = Master.DataPID
        gridP31.EntityX29ID = BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix)
        gridP31.MasterDataPID = Master.DataPID
        If Me.CurrentMasterPrefix = "p41" Then
            gridP31.AllowApproving = menu1.IsExactApprovingPerson
        Else
            gridP31.AllowApproving = Master.Factory.SysUser.IsApprovingPerson
        End If

    End Sub

    Private Sub RefreshRecord()
        Dim strTab As String = gridP31.MasterTabAutoQueryFlag
        If strTab = "" Then strTab = "p31"

        Select Case Me.CurrentMasterPrefix
            Case "p41"
                Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p41")
                Dim cP42 As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(cRec.p42ID)
                Dim cRecSum As BO.p41ProjectSum = Master.Factory.p41ProjectBL.LoadSumRow(cRec.PID)
                menu1.p41_RefreshRecord(cRec, cRecSum, strTab)
                If cRec.p41TreePrev < cRec.p41TreeNext Then
                    gridP31.EnableEntityChilds = True
                End If
            Case "p28"
                Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p28")
                Dim cRecSum As BO.p28ContactSum = Master.Factory.p28ContactBL.LoadSumRow(cRec.PID)
                menu1.p28_RefreshRecord(cRec, cRecSum, strTab)
            Case "j02"
                Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=j02")
                Dim cRecSum As BO.j02PersonSum = Master.Factory.j02PersonBL.LoadSumRow(cRec.PID)
                menu1.j02_RefreshRecord(cRec, cRecSum, strTab)
            Case "p56"
                Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p56")
                Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(cRec.p41ID)
                Dim cRecSum As BO.p56TaskSum = Master.Factory.p56TaskBL.LoadSumRow(Master.DataPID)
                menu1.p56_RefreshRecord(cRec, cRecSum, cP41, strTab)
        End Select

    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        Select Case Me.hidHardRefreshFlag.Value
            Case "p31-save"
                gridP31.DefaultSelectedPID = BO.BAS.IsNullInt(Me.hidHardRefreshPID.Value)
                gridP31.Rebind(True, gridP31.DefaultSelectedPID)
        End Select
        Me.hidHardRefreshFlag.Value = "" : Me.hidHardRefreshPID.Value = ""
    End Sub

    
End Class