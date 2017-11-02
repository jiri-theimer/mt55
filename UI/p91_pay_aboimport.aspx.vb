Public Class p91_pay_aboimport
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Class AboRecord
        Public Property TypZaznamu As String
        Public Property CisloUctuKlienta As String
        Public Property CisloProtiUctu As String
        Public Property CisloDokladu As String
        Public Property Castka As String
        Public Property KodUctovani As String
        Public Property VSymbol As String
        Public Property KSymbol As String
        Public Property SSymbol As String
        Public Property Valuta As String
        Public Property DoplnujiciUdaj As String
        Public Property KodBanky As String
      
        Public Property Vysledek As String
        Public Property IsStrike As Boolean
    End Class



    Private Sub p91_pay_aboimport_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

           
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Import úhrad z ABO souboru"

                
                .AddToolbarButton("Spustit import", "ok", , "Images/ok.png")
            End With




        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            For Each invalidFile As Telerik.Web.UI.UploadedFile In upload1.InvalidFiles
                Master.Notify(String.Format("Soubor [{0}] nelze použít jako vstupní soubor.", invalidFile.FileName), 2)
            Next
            If upload1.UploadedFiles.Count = 0 Then
                Return
            End If

            For Each validFile As Telerik.Web.UI.UploadedFile In upload1.UploadedFiles
                Dim cF As New BO.clsFile
              
                Dim strTemp As String = Master.Factory.x35GlobalParam.TempFolder & "\" & cF.GetNameFromFullpath(validFile.FileName)

                Try
                    validFile.SaveAs(strTemp, True)
                    Dim s As String = cF.GetFileContents(strTemp), intCountNotHandled As Integer = 0
                    Dim lis As List(Of AboRecord) = ParseFileContent(s)
                    Master.Notify("Zpracování ABO souboru dokončeno.", NotifyLevel.InfoMessage)
                    rp1.DataSource = lis
                    rp1.DataBind()
                Catch ex As Exception
                    Master.Notify(ex.Message, NotifyLevel.ErrorMessage)
                    Return
                End Try
            Next
        End If
    End Sub

    Private Function ParseFileContent(s As String) As List(Of AboRecord)
        Dim a() As String = Split(s, vbCrLf), intHandled As Integer = 0, lisRP As New List(Of AboRecord)

        For i As Integer = 1 To UBound(a) - 1
            Dim c As New AboRecord, intStart As Integer = 3
            c.TypZaznamu = Left(a(i), 3)
            c.CisloUctuKlienta = Mid(a(i), intStart + 1, 16)
            intStart += 16
            c.CisloProtiUctu = RemoveLeadingZeros(Mid(a(i), intStart + 1, 16))
            intStart += 16
            c.CisloDokladu = Mid(a(i), intStart + 1, 13)
            intStart += 13
            c.Castka = RemoveLeadingZeros(Mid(a(i), intStart + 1, 12))
            intStart += 12
            c.KodUctovani = Mid(a(i), intStart + 1, 1)
            intStart += 1
            c.VSymbol = RemoveLeadingZeros(Mid(a(i), intStart + 1, 10))
            intStart += 10
            c.KSymbol = Mid(a(i), intStart + 1, 10)
            If Len(c.KSymbol) > 8 Then
                c.KodBanky = Left(Right(c.KSymbol, 8), 4)
            End If

            intStart += 10
            c.SSymbol = Mid(a(i), intStart + 1, 10)
            intStart += 10
            c.Valuta = Mid(a(i), intStart + 1, 6)
            intStart += 6
            c.DoplnujiciUdaj = Mid(a(i), intStart + 1, 20)


            If c.CisloProtiUctu <> "" And c.VSymbol <> "" Then
                Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.LoadByCode(c.VSymbol)
                If Not cRec Is Nothing Then
                    If cRec.p91Amount_Debt > 1 Then
                        If Master.Factory.p91InvoiceBL.LoadP94ByCode(c.VSymbol & "-" & c.CisloDokladu) Is Nothing Then
                            Dim cP94 As New BO.p94Invoice_Payment
                            With cP94
                                .p94Code = c.VSymbol & "-" & c.CisloDokladu
                                .p91ID = cRec.PID
                                .p94Amount = CDbl(c.Castka) / 100
                                .p94Date = Today
                                .p94Description = c.DoplnujiciUdaj
                            End With
                            If Master.Factory.p91InvoiceBL.SaveP94(cP94) Then
                                intHandled += 1
                                c.Vysledek = String.Format("Nově spárovaná úhrada k faktuře {0}.", c.VSymbol)
                                c.IsStrike = True
                            End If
                        Else
                            c.Vysledek = String.Format("Úhrada byla již dříve spárovaná.")
                        End If
                    Else
                        c.Vysledek = String.Format("Faktura s ID {0} byla již dříve uhrazena.", c.VSymbol)
                    End If
                Else
                    c.Vysledek = String.Format("Pro variabilní symbol {0} nebyla nalezena vystavená faktura.", c.VSymbol)
                End If
            Else
                c.Vysledek = String.Format("Bez pokusu o spárování.")
            End If

            lisRP.Add(c)


        Next
        Return lisRP
    End Function

    Private Function RemoveLeadingZeros(str As String) As String
        If str = "" Then Return ""
        Dim s As String = str
        For i As Integer = 1 To Len(str)
            If Mid(str, i, 1) = "0" Then
                s = Right(str, Len(str) - i)
            Else
                Return s
            End If
            If s = "0" Then Return ""
        Next
        Return s
    End Function

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim c As AboRecord = CType(e.Item.DataItem, AboRecord)
        CType(e.Item.FindControl("Castka"), Label).Text = BO.BAS.FN(CDbl(c.Castka) / 100)
        If c.CisloProtiUctu <> "" Then
            CType(e.Item.FindControl("CisloProtiUctu"), Label).Text = c.CisloProtiUctu & "/" & c.KodBanky
        End If

        CType(e.Item.FindControl("VSymbol"), Label).Text = c.VSymbol
        CType(e.Item.FindControl("DoplnujiciUdaj"), Label).Text = c.DoplnujiciUdaj
        CType(e.Item.FindControl("Vysledek"), Label).Text = c.Vysledek
        If c.IsStrike Then
            CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/ok.png"
        End If

    End Sub
End Class