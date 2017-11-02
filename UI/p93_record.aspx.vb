Public Class p93_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p93_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID()
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení hlavičky vystavovatele faktury"


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

        Dim cRec As BO.p93InvoiceHeader = Master.Factory.p93InvoiceHeaderBL.Load(Master.DataPID)
        With cRec
            Me.p93Name.Text = .p93Name
            Me.p93Company.Text = .p93Company
            Me.p93City.Text = .p93City
            Me.p93Street.Text = .p93Street
            Me.p93Zip.Text = .p93Zip
            Me.p93RegID.Text = .p93RegID
            Me.p93VatID.Text = .p93VatID
            Me.p93Contact.Text = .p93Contact
            Me.p93Referent.Text = .p93Referent
            Me.p93Registration.Text = .p93Registration
            Me.p93Signature.Text = .p93Signature
            Me.p93FreeText01.Text = .p93FreeText01
            Me.p93FreeText02.Text = .p93FreeText02
            Me.p93FreeText03.Text = .p93FreeText03
            Me.p93FreeText04.Text = .p93FreeText04


            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp


        End With

        Dim lisP88 As IEnumerable(Of BO.p88InvoiceHeader_BankAccount) = Master.Factory.p93InvoiceHeaderBL.GetList_p88(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid"))
        For Each c In lisP88
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid")
                .p85DataPID = c.PID
                .p85OtherKey1 = c.j27ID
                .p85OtherKey2 = c.p86ID
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempP88()
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p93InvoiceHeaderBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p93-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        SaveTempP88()

        With Master.Factory.p93InvoiceHeaderBL
            Dim cRec As BO.p93InvoiceHeader = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p93InvoiceHeader)
            With cRec
                .p93Name = Me.p93Name.Text
                .p93Company = Me.p93Company.Text
                .p93City = Me.p93City.Text
                .p93Street = Me.p93Street.Text
                .p93Zip = Me.p93Zip.Text
                .p93RegID = Me.p93RegID.Text
                .p93VatID = Me.p93VatID.Text
                .p93Contact = Me.p93Contact.Text
                .p93Referent = Me.p93Referent.Text
                .p93Registration = Me.p93Registration.Text
                .p93Signature = Me.p93Signature.Text
                .p93FreeText01 = Me.p93FreeText01.Text
                .p93FreeText02 = Me.p93FreeText02.Text
                .p93FreeText03 = Me.p93FreeText03.Text
                .p93FreeText04 = Me.p93FreeText04.Text

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With

            Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            Dim lisP88 As New List(Of BO.p88InvoiceHeader_BankAccount)
            For Each cTMP In lisTEMP
                Dim c As New BO.p88InvoiceHeader_BankAccount
                With cTMP
                    c.j27ID = .p85OtherKey1
                    c.p86ID = .p85OtherKey2
                End With
                lisP88.Add(c)
            Next


            If .Save(cRec, lisP88) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p93-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdAddP88_Click(sender As Object, e As EventArgs) Handles cmdAddP88.Click
        SaveTempP88()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = ViewState("guid")

        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTempP88()
    End Sub
    Private Sub RefreshTempP88()
        rpP88.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rpP88.DataBind()
    End Sub
    Private Sub SaveTempP88()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each ri As RepeaterItem In rpP88.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("j27id"), DropDownList).SelectedValue)
                .p85OtherKey2 = BO.BAS.IsNullInt(CType(ri.FindControl("p86id"), DropDownList).SelectedValue)

            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub

    Private Sub rpP88_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP88.ItemCommand
        SaveTempP88()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempP88()
            End If
        End If
    End Sub

    Private Sub rpP88_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP88.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With

        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            With CType(e.Item.FindControl("j27id"), DropDownList)
                If .Items.Count = 0 Then
                    .DataSource = Master.Factory.ftBL.GetList_J27()
                    .DataBind()
                    .Items.Insert(0, "")
                End If
            End With
            With CType(e.Item.FindControl("p86id"), DropDownList)
                If .Items.Count = 0 Then
                    .DataSource = Master.Factory.p86BankAccountBL.GetList(New BO.myQuery)
                    .DataBind()
                    .Items.Insert(0, "")
                End If
            End With

            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("j27id"), DropDownList), .p85OtherKey1.ToString)
            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p86id"), DropDownList), .p85OtherKey2.ToString)

        End With
    End Sub
End Class