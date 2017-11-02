Public Class j04_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j04_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "j04_record"
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Aplikační role"
                Me.j60ID.DataSource = .Factory.j62MenuHomeBL.GetList_J60()
                Me.j60ID.DataBind()
            End With

            SetupPersonalPageCombo()
            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
        End If
    End Sub
    Private Sub SetupPersonalPageCombo()
        Me.j04Aspx_PersonalPage.DataSource = Master.Factory.x31ReportBL.GetList_PersonalPageSource()
        Me.j04Aspx_PersonalPage.DataBind()
        For Each n As Telerik.Web.UI.RadComboBoxItem In Me.j04Aspx_PersonalPage.RadCombo.Items
            If BO.BAS.IsNullInt(n.Value) > 0 Then n.ImageUrl = "Images/plugin.png"
        Next
        Me.j04Aspx_PersonalPage_Mobile.DataSource = Me.j04Aspx_PersonalPage.DataSource
        Me.j04Aspx_PersonalPage_Mobile.DataBind()
        For Each n As Telerik.Web.UI.RadComboBoxItem In Me.j04Aspx_PersonalPage_Mobile.RadCombo.Items
            If BO.BAS.IsNullInt(n.Value) > 0 Then n.ImageUrl = "Images/plugin.png"
        Next
        'Me.j04Aspx_PersonalPage.ChangeItemText("", "Úvodní stránka s pozdravem")
        'Me.j04Aspx_PersonalPage_Mobile.ChangeItemText("", "Úvodní stránka s pozdravem")
    End Sub

    Private Sub RefreshRecord()
        Dim lis As IEnumerable(Of BO.x53Permission) = Master.Factory.ftBL.GetList_X53(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.j03User)

        Me.x53ids.DataSource = lis
        Me.x53ids.DataBind()

        If Master.DataPID = 0 Then
            'u nového záznamu role zaškrtnout několik voleb
            Try
                With x53ids.Items
                    .FindByValue(lis.Where(Function(p) p.x53Value = BO.x53PermValEnum.GR_P31_EditAsNonOwner)(0).PID.ToString).Selected = True
                    .FindByValue(lis.Where(Function(p) p.x53Value = BO.x53PermValEnum.GR_P31_AllowRates)(0).PID.ToString).Selected = True
                    .FindByValue(lis.Where(Function(p) p.x53Value = BO.x53PermValEnum.GR_P31_Approve_P72)(0).PID.ToString).Selected = True
                End With


            Catch ex As Exception
            End Try
            
            Return
        End If

        Dim cRec As BO.j04UserRole = Master.Factory.j04UserRoleBL.Load(Master.DataPID)
        With cRec
            basUI.SelectDropdownlistValue(Me.j60ID, .j60ID.ToString)
            Me.j04Name.Text = .j04Name
            Me.j04Aspx_PersonalPage.SelectedValue = .j04Aspx_PersonalPage
            Me.j04Aspx_PersonalPage_Mobile.SelectedValue = .j04Aspx_PersonalPage_Mobile
            Me.j04IsMenu_Worksheet.Checked = .j04IsMenu_Worksheet
            Me.j04IsMenu_Project.Checked = .j04IsMenu_Project
            Me.j04IsMenu_Contact.Checked = .j04IsMenu_Contact
            Me.j04IsMenu_People.Checked = .j04IsMenu_People
            Me.j04IsMenu_Report.Checked = .j04IsMenu_Report
            Me.j04IsMenu_Invoice.Checked = .j04IsMenu_Invoice
            Me.j04IsMenu_Proforma.Checked = .j04IsMenu_Proforma
            Me.j04IsMenu_Notepad.Checked = .j04IsMenu_Notepad
            Me.j04IsMenu_MyProfile.Checked = .j04IsMenu_MyProfile
            Me.j04IsMenu_Task.Checked = .j04IsMenu_Task

            Me.j04Aspx_OnePersonPage.Text = .j04Aspx_OnePersonPage
            Me.j04Aspx_OneProjectPage.Text = .j04Aspx_OneProjectPage
            Me.j04Aspx_OneContactPage.Text = .j04Aspx_OneContactPage
            Me.j04Aspx_OneInvoicePage.Text = .j04Aspx_OneInvoicePage

            ''Me.j04IsMenu_Proforma.Checked = .j04IsMenu_Proforma

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp

            lis = Master.Factory.x67EntityRoleBL.GetList_BoundX53(.x67ID)
            basUI.CheckItems(Me.x53ids, lis.Select(Function(p) p.PID).ToList)
            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j04UserRoleBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j04-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.j04UserRoleBL
            Dim cRec As BO.j04UserRole = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j04UserRole)
            With cRec
                .j60ID = BO.BAS.IsNullInt(Me.j60ID.SelectedValue)
                .j04Name = Me.j04Name.Text
                .j04Aspx_PersonalPage = Me.j04Aspx_PersonalPage.SelectedValue
                .j04Aspx_PersonalPage_Mobile = Me.j04Aspx_PersonalPage_Mobile.SelectedValue
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .j04IsMenu_Worksheet = Me.j04IsMenu_Worksheet.Checked
                .j04IsMenu_Project = Me.j04IsMenu_Project.Checked
                .j04IsMenu_Contact = Me.j04IsMenu_Contact.Checked
                .j04IsMenu_People = Me.j04IsMenu_People.Checked
                .j04IsMenu_Report = Me.j04IsMenu_Report.Checked
                .j04IsMenu_Invoice = Me.j04IsMenu_Invoice.Checked
                .j04IsMenu_Proforma = Me.j04IsMenu_Proforma.Checked
                .j04IsMenu_Notepad = Me.j04IsMenu_Notepad.Checked
                .j04IsMenu_MyProfile = Me.j04IsMenu_MyProfile.Checked
                .j04IsMenu_Task = Me.j04IsMenu_Task.Checked

                .j04Aspx_OnePersonPage = Me.j04Aspx_OnePersonPage.Text
                .j04Aspx_OneProjectPage = Me.j04Aspx_OneProjectPage.Text
                .j04Aspx_OneContactPage = Me.j04Aspx_OneContactPage.Text
                .j04Aspx_OneInvoicePage = Me.j04Aspx_OneInvoicePage.Text
            End With
            

            Dim mq As New BO.myQuery
            mq.AddItemToPIDs(-1)
            For Each intX53ID As Integer In basUI.GetCheckedItems(Me.x53ids)
                mq.AddItemToPIDs(intX53ID)
            Next
            Dim lisX53 As List(Of BO.x53Permission) = Master.Factory.ftBL.GetList_X53(mq).ToList
            If .Save(cRec, lisX53) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("j04-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    
    
   
  
    
    Private Sub cmdCheckAll_Click(sender As Object, e As EventArgs) Handles cmdCheckAll.Click
        For Each li As ListItem In Me.x53ids.Items
            li.Selected = True
        Next
    End Sub

    Private Sub cmdUnCheckAll_Click(sender As Object, e As EventArgs) Handles cmdUnCheckAll.Click
        For Each li As ListItem In Me.x53ids.Items
            li.Selected = False
        Next
    End Sub
End Class