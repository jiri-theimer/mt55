Public Class p86_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p86_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení bankovního účtu"


            End With

            RefreshRecord()

            If Master.DataPID = 0 Then

            End If

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If


        End If
    End Sub

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.p86BankAccount = Master.Factory.p86BankAccountBL.Load(Master.DataPID)
        With cRec
            Me.p86Name.Text = .p86Name
            Me.p86BankName.Text = .p86BankName
            Me.p86BankAccount.Text = .p86BankAccount
            Me.p86BankCode.Text = .p86BankCode
            Me.p86BankAddress.Text = .p86BankAddress
            Me.p86SWIFT.Text = .p86SWIFT
            Me.p86IBAN.Text = .p86IBAN


            ''Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp


        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p86BankAccountBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p86-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p86BankAccountBL
            Dim cRec As BO.p86BankAccount = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p86BankAccount)
            With cRec
                .p86Name = Me.p86Name.Text
                .p86BankName = Me.p86BankName.Text
                .p86BankAccount = Me.p86BankAccount.Text
                .p86BankCode = Me.p86BankCode.Text
                .p86SWIFT = Me.p86SWIFT.Text
                .p86IBAN = Me.p86IBAN.Text
                .p86BankAddress = Me.p86BankAddress.Text
                
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p86-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

End Class