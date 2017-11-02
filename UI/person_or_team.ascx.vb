Public Class person_or_team
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Public Property MobileMode As Boolean
        Get

        End Get
        Set(value As Boolean)

        End Set
    End Property
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Me.hidGUID.Value = "" Then Me.hidGUID.Value = BO.BAS.GetGUID
    End Sub

    Public ReadOnly Property RowsCount As Integer
        Get
            Return rp1.Items.Count
        End Get
    End Property

    Public Sub AddReceiver(intJ02ID As Integer, intJ11ID As Integer, bolMobile As Boolean)
        Me.hidIsMobile.Value = BO.BAS.GB(bolMobile)
        SaveTemp()
        Dim c As New BO.p85TempBox
        c.p85GUID = Me.hidGUID.Value
        If intJ02ID <> 0 Then
            c.p85OtherKey1 = intJ02ID
            c.p85FreeText01 = Me.Factory.j02PersonBL.Load(intJ02ID).FullNameDesc
        End If
        If intJ11ID <> 0 Then
            c.p85OtherKey2 = intJ11ID
            c.p85FreeText02 = Me.Factory.j11TeamBL.Load(intJ11ID).j11Name
        End If
        Me.Factory.p85TempBoxBL.Save(c)
        RefreshTempList()
    End Sub
    Private Sub RefreshTempList()
        rp1.DataSource = Me.Factory.p85TempBoxBL.GetList(Me.hidGUID.Value)
        rp1.DataBind()
    End Sub

    Public Function GetList() As List(Of BO.PersonOrTeam)
        Dim lis As New List(Of BO.PersonOrTeam)
        For Each c In Me.Factory.p85TempBoxBL.GetList(Me.hidGUID.Value)
            If c.p85OtherKey1 <> 0 Or c.p85OtherKey2 <> 0 Then
                lis.Add(New BO.PersonOrTeam(c.p85OtherKey1, c.p85OtherKey2))
            End If
        Next
        Return lis
    End Function
    Public Function GetInlineContent() As String
        Return hidInlineContent.Value
    End Function

    Public Sub SaveTemp()
        Me.hidInlineContent.Value = ""
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Me.Factory.p85TempBoxBL.GetList(Me.hidGUID.Value)
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)

            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("j02id"), UI.person).Value)
                If .p85OtherKey1 <> 0 Then .p85FreeText01 = Me.Factory.j02PersonBL.Load(.p85OtherKey1).FullNameDesc Else .p85FreeText01 = ""
                With CType(ri.FindControl("j11id"), DropDownList)
                    If Not .SelectedItem Is Nothing Then
                        cRec.p85OtherKey2 = BO.BAS.IsNullInt(.SelectedValue)
                        cRec.p85FreeText02 = .SelectedItem.Text
                    Else
                        cRec.p85OtherKey2 = 0 : cRec.p85FreeText02 = ""
                    End If
                End With
                If .p85FreeText01 <> "" Then Me.hidInlineContent.Value = BO.BAS.OM4(Me.hidInlineContent.Value, .p85FreeText01, "; ")
                If .p85FreeText02 <> "" Then Me.hidInlineContent.Value = BO.BAS.OM4(Me.hidInlineContent.Value, .p85FreeText02, "; ")

            End With
            Me.Factory.p85TempBoxBL.Save(cRec)

        Next
    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        SaveTemp()
        Dim cRec As BO.p85TempBox = Me.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(CType(e.Item.FindControl("p85id"), HiddenField).Value))
        If e.CommandName = "delete" Then
            If Me.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempList()
            End If
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandName = "delete"
        End With
        With CType(e.Item.FindControl("j11id"), DropDownList)
            .DataSource = Me.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
            .DataBind()
            .Items.Insert(0, "")
        End With
        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("j11id"), DropDownList), cRec.p85OtherKey2.ToString)
        With CType(e.Item.FindControl("j02id"), UI.person)
            .Value = cRec.p85OtherKey1.ToString
            .Text = cRec.p85FreeText01
            If Me.hidIsMobile.Value = "1" Then
                .RadCombo.RenderMode = Telerik.Web.UI.RenderMode.Mobile
                .RadCombo.DropDownWidth = Unit.Parse("350px")
            End If
        End With

    End Sub

End Class