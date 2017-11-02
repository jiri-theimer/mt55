Public Class SubjectMonitoring
    Private Property _CountryCode As String
    Private Property _Ic As String
    Private Property _Dic As String
    Private Property _Rc As String
    Private Property _FirstName As String
    Private Property _LastName As String
    Private Property _datBirth As Date?
    Private Property _justiceRegistrNazev As String

    Public Sub New(cP28rec As BO.p28Contact)
        With cP28rec
            _Ic = .p28RegID
            _Dic = .p28VatID
            If .p28VatID <> "" Then
                _CountryCode = Left(.p28VatID, 2)
            Else
                _CountryCode = "CZ"
            End If
            If .p28IsCompany Then
                _FirstName = .p28FirstName
                _LastName = .p28LastName
            End If
        End With
    End Sub
    Public Sub New(strIC As String, strDIC As String, strCountryCode As String)
        _Ic = UCase(Trim(strIC))
        If strDIC <> "" Then _Dic = UCase(Replace(Trim(strDIC), " ", ""))
        If strCountryCode = "" And _Dic <> "" Then strCountryCode = Left(_Dic, 2)
        If strCountryCode = "" Then strCountryCode = "CZ"
        _CountryCode = UCase(strCountryCode)
    End Sub
    Public Sub New(strFirstName As String, strLastName As String, strRC As String, strIC As String, strDIC As String, datBirth As Date?)
        _CountryCode = "CZ"
        If strDIC <> "" Then _Dic = UCase(Replace(Trim(strDIC), " ", ""))
        If strRC <> "" Then
            _Rc = Replace(Trim(strRC), "/", "").Replace(" ", "")
        End If
        _FirstName = strFirstName
        _LastName = strLastName
        _datBirth = datBirth
    End Sub

    Public ReadOnly Property AresUrl As String
        Get
            If _Ic <> "" And _CountryCode = "CZ" Then
                Return "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_res.cgi?ico=" & _Ic & "&jazyk=cz&xml=1"
            End If
            Return ""
        End Get
    End Property
    Public ReadOnly Property IsirUrl As String
        Get
            Dim s As String = "https://isir.justice.cz/isir/ueu/vysledek_lustrace.do?ceuprob=x&mesto=&cislo_senatu=&bc_vec=&rocnik=&id_osoby_puvodce=&druh_stav_konkursu=&datum_stav_od=&datum_stav_do=&aktualnost=AKTUALNI_I_UKONCENA&druh_kod_udalost=&datum_akce_od=&datum_akce_do=&nazev_osoby_f=&nazev_osoby_spravce=&rowsAtOnce=50&spis_znacky_datum=&spis_znacky_obdobi=14DNI"
            If _Ic <> "" Then
                Return s & "&ic=" & _Ic
            End If
            If _Rc <> "" Then
                Return s & "&rc=" & _Rc
            End If
            If _FirstName <> "" And _LastName <> "" And Not _datBirth Is Nothing Then
                Return s & "&jmeno_osoby=" & _FirstName & "&nazev_osoby=" & _LastName & "&datum_narozeni=" & Format(_datBirth, "dd.MM.yyyy")
            End If
            Return ""
        End Get

    End Property
    Public ReadOnly Property JusticeUrl As String
        Get
            _justiceRegistrNazev = ""
            If Len(_Ic) = 8 And _CountryCode = "CZ" Then
                _justiceRegistrNazev = "JUSTICE.cz"
                Return "https://or.justice.cz/ias/ui/rejstrik-$firma?ico=" & _Ic
            End If
            If _Ic <> "" And _CountryCode = "SK" Then
                _justiceRegistrNazev = "OBCHODNÝ REGISTER | MINISTERSTVO SPRAVODLIVOSTI SLOVENSKEJ REPUBLIKY"
                Return "http://www.orsr.sk/hladaj_ico.asp?ICO=" & _Ic
            End If
            Return ""
        End Get
    End Property
    Public ReadOnly Property JusticeName As String
        Get
            Return _justiceRegistrNazev
        End Get
    End Property

End Class
