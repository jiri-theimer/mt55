Public Interface Im62ExchangeRateBL
    Inherits IFMother
    Function Save(cRec As BO.m62ExchangeRate) As Boolean
    Function Load(intPID As Integer) As BO.m62ExchangeRate
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.m62ExchangeRate)
    Sub ImportRateList_CNB(datImport As Date, Optional strExplicitJ27Codes As String = "")
End Interface

Class m62ExchangeRateBL
    Inherits BLMother
    Implements Im62ExchangeRateBL
    Private WithEvents _cDL As DL.m62ExchangeRateDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.m62ExchangeRateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.m62ExchangeRate) As Boolean Implements Im62ExchangeRateBL.Save
        With cRec
            If .j27ID_Master = 0 Then _Error = "Chybí zdrojová měna." : Return False
            If .j27ID_Slave = 0 Then _Error = "Chybí cílová měna." : Return False
            If .m62Rate <= 0 Then _Error = "Hodnota kurzu musí být větší než nula." : Return False
            If .m62Units <= 0 Then _Error = "Množství musí být větší než nula." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.m62ExchangeRate Implements Im62ExchangeRateBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Im62ExchangeRateBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.m62ExchangeRate) Implements Im62ExchangeRateBL.GetList
        Return _cDL.GetList(mq)
    End Function

    Public Sub ImportRateList_CNB(datImport As Date, Optional strExplicitJ27Codes As String = "") Implements Im62ExchangeRateBL.ImportRateList_CNB
        datImport = DateSerial(Year(datImport), Month(datImport), Day(datImport))
        Dim lisM62 As IEnumerable(Of BO.m62ExchangeRate) = GetList().Where(Function(p) p.m62RateType = BO.m62RateTypeENUM.InvoiceRate And p.m62Date = datImport)
        Dim lisJ27 As IEnumerable(Of BO.j27Currency) = Factory.ftBL.GetList_J27()

        Dim strMeny As String = strExplicitJ27Codes
        If strMeny = "" Then
            'načíst z globálního nastavení
            strMeny = Factory.x35GlobalParam.GetValueString("j27Codes_Import_CNB")
        End If
        If strMeny = "" Then Return
        Dim meny() As String = Split(strMeny, ",")

        Dim url As String = String.Format("http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt?date={0:dd.MM.yyyy}", datImport)
        Dim rq As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
        Dim rs As System.Net.HttpWebResponse = rq.GetResponse()
        Dim sr As New IO.StreamReader(rs.GetResponseStream()), x As Integer = 0

        While Not sr.EndOfStream
            Dim s As String = sr.ReadLine()
            If x = 0 Then
                'začátek souboru
                Dim radek() As String = Split(s, "#")
                Dim datSoubor As Date = BO.BAS.ConvertString2Date(Trim(radek(0)))
                If datSoubor <> datImport Then
                    'pro den datImport neexistuje kurzovní lístek
                    datImport = datSoubor
                    lisM62 = GetList().Where(Function(p) p.m62RateType = BO.m62RateTypeENUM.InvoiceRate And p.m62Date = datImport)
                End If
            End If
            For i As Integer = 0 To UBound(meny)
                Dim strMena As String = meny(i)
                If s.Contains("|" & strMena & "|") Then
                    Dim radek() As String = Split(s, "|")
                    Dim cRec As New BO.m62ExchangeRate
                    cRec.m62Date = datImport
                    cRec.j27ID_Master = 2
                    cRec.j27ID_Slave = lisJ27.Where(Function(p) p.j27Code = radek(3)).First.PID
                    cRec.m62Units = CInt(radek(2))
                    cRec.m62RateType = BO.m62RateTypeENUM.InvoiceRate
                    cRec.m62Rate = CDbl(radek(4))
                    cRec.SetUserInsert("robot")

                    Dim lisFound As IEnumerable(Of BO.m62ExchangeRate) = lisM62.Where(Function(p) p.j27ID_Slave = cRec.j27ID_Slave And p.m62Date = datImport)
                    If lisFound.Count > 0 Then
                        cRec.SetPID(lisFound(0).PID)
                    End If
                    Factory.m62ExchangeRateBL.Save(cRec)
                End If
            Next
            x += 1
        End While

        sr.Close()
        rs.Close()
    End Sub
End Class
