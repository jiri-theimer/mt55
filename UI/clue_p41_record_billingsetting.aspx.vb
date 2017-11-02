Public Class clue_p41_record_billingsetting
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(intPID)
        Dim cClient As BO.p28Contact = Nothing

        With cRec
            ph1.Text = BO.BAS.OM2(ph1.Text, .FullName)
            If .p28ID_Client > 0 Then
                cClient = Master.Factory.p28ContactBL.Load(.p28ID_Client)

            End If
            If .p28ID_Billing > 0 Then
                Me.Invoice_Receiver.Text = Master.Factory.p28ContactBL.Load(.p28ID_Billing).p28Name
            Else
                If .p28ID_Client = 0 Then Me.Invoice_Receiver.Visible = False
            End If
          
            

            If .p41InvoiceDefaultText1 <> "" Then
                panInvoiceText1.Visible = True
                Me.p41InvoiceDefaultText1.Text = BO.BAS.CrLfText2Html(.p41InvoiceDefaultText1)

            End If

            If .p92ID > 0 Then
                Me.p92Name.Text = .p92Name
            Else
                If Not cClient Is Nothing Then
                    If cClient.p92ID > 0 Then
                        Me.p92Name.Text = cClient.p92Name
                        Me.p92Name_add.Text = "Dědí se z klienta projektu"
                    End If

                End If
            End If


        End With

        If Not cClient Is Nothing Then
            With cClient
             
                If Not panInvoiceText1.Visible And .p28InvoiceDefaultText1 <> "" Then
                    Me.p41InvoiceDefaultText1.Text = BO.BAS.CrLfText2Html(.p28InvoiceDefaultText1)
                    Me.p41InvoiceDefaultText1.ToolTip = "Dědí se z klienta projektu"
                    panInvoiceText1.Visible = True
                End If
            End With
        End If

        RefreshBillingLanguage(cRec, cClient)
        RefreshDefualtMaturity(cRec, cClient)
        

    End Sub

    Private Sub RefreshDefualtMaturity(cRec As BO.p41Project, cClient As BO.p28Contact)
        Dim x As Integer = 0, s As String = ""
        x = cRec.p41InvoiceMaturityDays
        If x = 0 And Not cClient Is Nothing Then
            x = cClient.p28InvoiceMaturityDays
            s = "Dědí se z klienta projektu"
        End If
        If x = 0 Then
            x = Master.Factory.x35GlobalParam.GetValueInteger("DefMaturityDays")
            s = "Výchozí hodnota systému"
        End If
        Me.DefaultMaturity.Text = x.ToString & " dnů"
        Me.DefaultMaturity_add.Text = s
    End Sub
    Private Sub RefreshBillingLanguage(cRec As BO.p41Project, cClient As BO.p28Contact)
        imgFlag.Visible = False
        With cRec
            If .p87ID > 0 Then
                Dim cP87 As BO.p87BillingLanguage = Master.Factory.ftBL.LoadP87(.p87ID)
                Me.p87Name.Text = cP87.p87Name
                Me.p87Name_add.Text = "K projektu je přímo přiřazený fakturační jazyk."
                If cP87.p87Icon <> "" Then
                    imgFlag.Visible = True
                    imgFlag.ImageUrl = "Images/flags/" & cP87.p87Icon
                End If
            Else
                If .p87ID_Client > 0 Then
                    If Not cClient Is Nothing Then
                        Dim cP87 As BO.p87BillingLanguage = Master.Factory.ftBL.LoadP87(.p87ID_Client)
                        Me.p87Name.Text = cP87.p87Name
                        Me.p87Name_add.Text = "Dědí se z klienta projektu."
                        If cP87.p87Icon <> "" Then
                            imgFlag.Visible = True
                            imgFlag.ImageUrl = "Images/flags/" & cP87.p87Icon
                        End If
                    End If
                End If
            End If
           
        End With
    End Sub
End Class