Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Public Class clsAresImport
    Private _strXML As String
    Private _Error As String = ""

    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Function LoadAresRecord(strIC As String) As BO.AresRecord
        Dim strURL As String = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_bas.cgi?ico=" & strIC

        Dim client As System.Net.WebClient = New System.Net.WebClient()
        Dim data As System.IO.Stream
        
        Try
            data = client.OpenRead(strURL)
        Catch ex As Exception
            _Error = ex.Message
            Return Nothing
        End Try

        Dim c As New BO.AresRecord
        Dim reader As System.IO.StreamReader = New System.IO.StreamReader(data, True)

        Dim ds As New DataSet
        ds.ReadXml(reader)

        reader.Close()
        data.Close()

        Try
            c.Company = ds.Tables("OF").Rows(0).Item(1)
        Catch ex As Exception
        End Try
        Try
            c.DIC = ds.Tables("DIC").Rows(0).Item(1)
        Catch ex As Exception
        End Try

        Dim dbRow As DataRow = Nothing
        Try
            dbRow = ds.Tables("AA").Rows(0)
        Catch ex As Exception
            _Error = ex.Message
            Return Nothing
        End Try

        c.ID_adresy = RF(dbRow, "IDA")
        c.Street = Trim(RF(dbRow, "NU") & " " & RF(dbRow, "CD"))
        c.PostCode = RF(dbRow, "PSC")
        c.City = RF(dbRow, "NMC")
        c.Country = RF(dbRow, "NS")

        If c.ID_adresy <> "" And c.Street = "" And c.City = "" Then
            c.Street = RF(dbRow, "AT")  'adresa není rozdělená do samostatných polí
        End If

        ds.Clear()
        Return c
    End Function
    Private Function RF(dbRow As DataRow, strProperty As String) As String
        Try
            Return dbRow.Item(strProperty)
        Catch ex As Exception
            Return ""
        End Try
    End Function
    'Public Function LoadAresRecord(ByVal strIC As String) As BO.AresRecord
    '    Dim strURL As String = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_std.cgi?ico=" & strIC

    '    Dim client As System.Net.WebClient = New System.Net.WebClient()
    '    Dim data As System.IO.Stream
    '    Try
    '        data = client.OpenRead(strURL)
    '    Catch ex As Exception
    '        _Error = ex.Message
    '        Return Nothing
    '    End Try
    '    Dim c As New BO.AresRecord
    '    'Dim reader As System.IO.StreamReader = New System.IO.StreamReader(data, System.Text.Encoding.GetEncoding(1250))
    '    Dim reader As System.IO.StreamReader = New System.IO.StreamReader(data, True)

    '    Dim ds As New DataSet
    '    ds.ReadXml(reader)

    '    reader.Close()
    '    data.Close()

    '    Dim strRows As String = ds.Tables("Odpoved").Rows(0).Item("Pocet_zaznamu")
    '    If strRows = "0" Then
    '        Return Nothing
    '    End If

    '    Dim dbRow As DataRow = ds.Tables("Adresa_ARES").Rows(0)
    '    Try
    '        c.City = dbRow.Item("Nazev_obce") & ""
    '    Catch ex As Exception

    '    End Try

    '    Try
    '        c.ID_adresy = dbRow.Item("ID_adresy")
    '        c.City += Trim(" " & dbRow.Item("Nazev_casti_obce"))
    '    Catch ex As Exception

    '    End Try
    '    Try
    '        If dbRow.Item("Nazev_mestske_casti") & "" <> "" Then c.City = dbRow.Item("Nazev_mestske_casti")
    '    Catch ex As Exception

    '    End Try

    '    Try
    '        c.Street = dbRow.Item("Nazev_ulice") & ""
    '    Catch ex As Exception

    '    End Try
    '    Try
    '        c.Street += " " & dbRow.Item("Cislo_domovni")
    '    Catch ex As Exception

    '    End Try
    '    Try
    '        If dbRow.Item("Cislo_orientacni") & "" <> "" Then c.Street += "/" & dbRow.Item("Cislo_orientacni")
    '    Catch ex As Exception

    '    End Try
    '    Try
    '        c.PostCode = dbRow.Item("PSC") & ""
    '    Catch ex As Exception

    '    End Try


    '    c.Company = ds.Tables("Zaznam").Rows(0).Item("Obchodni_firma") & ""
    '    c.IC = ds.Tables("Zaznam").Rows(0).Item("ICO") & ""


    '    ds.Clear()

    '    Return c

    'End Function
End Class
