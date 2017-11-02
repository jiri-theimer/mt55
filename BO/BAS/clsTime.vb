Public Class clsTime
    Public Function ConvertTimeToSeconds(ByVal strTime As String) As Integer
        Dim dblTime As Double = 0
        Try
            dblTime = CDbl(strTime)
            Return CInt(dblTime * 60 * 60)
        Catch ex As Exception
            'dále už jenom testovat, zda není čas zadaný jako HH:MM
        End Try

        Dim arr() As String = Split(strTime, ":"), lngMinutes As Integer, lngHours As Integer
        If UBound(arr) <> 1 Then Return 0
        If IsNumeric(arr(0)) Then
            lngHours = CInt(arr(0))
        Else
            Return 0
        End If
        If IsNumeric(arr(1)) Then
            lngMinutes = CInt(arr(1))
        Else
            Return 0
        End If
        Return lngHours * 60 * 60 + lngMinutes * 60
    End Function

    Public Function ShowAsDec(ByVal strTime As String, Optional ByVal dblRetWithPrec As Double = 0, Optional ByVal lngMinTimeUnit As Integer = 0) As Double
        'strTime je ve formátu hh:mm
        'fce vrací čas z výrazu hh:mm na decadické číslo
        Dim lngSec As Integer = ConvertTimeToSeconds(strTime)
        If lngMinTimeUnit > 0 Then
            lngSec = RoundSeconds(lngSec, lngMinTimeUnit)
        End If
        Return CDbl(lngSec) / 60 / 60
    End Function

    Public Function RoundSeconds(ByVal lngSeconds As Integer, ByVal lngMinTimeSecUnit As Integer) As Integer
        'zaokrouhlí sekundy na jednotky lngMinTimeSecUnit - vše NAHORU!!
        If lngMinTimeSecUnit = 0 Then Return lngSeconds 'nezaokrouhlovat
        'If lngSeconds < 0 Then lngSeconds = 0
        Dim dbl As Double, lng As Integer
        dbl = CDbl(lngSeconds) / CDbl(lngMinTimeSecUnit)

        If CInt(dbl) <> dbl Or dbl = 0 Then
            'je třeba zaokrouhlovat

            lng = Math.Round(dbl + 0.5, 0)
            RoundSeconds = lng * lngMinTimeSecUnit


            If RoundSeconds = 0 Then RoundSeconds = lngMinTimeSecUnit
        Else
            RoundSeconds = lngSeconds
        End If
    End Function

    Function ShowAsHHMM(ByVal strTime As String, Optional ByVal lngMinTimeUnit As Integer = 0) As String
        'strTime jsou dekadické hodiny
        'fce vrací čas z dekadického hodinového výrazu na hh:mm
        Dim lngSec As Integer, strHHMM As String
        lngSec = ConvertTimeToSeconds(strTime)
        If lngMinTimeUnit > 0 Then
            lngSec = RoundSeconds(lngSec, lngMinTimeUnit)
        End If
        strHHMM = GetTimeFromSeconds(CDbl(lngSec))
        Return strHHMM
        'ShowAsHHMM = Left(strHHMM, 5)
    End Function

    Function GetTimeFromSeconds(ByVal tim As Double, Optional bolIncludeSeconds As Boolean = False) As String
        'tim... časový úsek vyjádřený v sekundách
        Dim hod As Double, cmin As String, chod As String, znam As String, Min As Integer

        If tim = 0 Then
            If Not bolIncludeSeconds Then Return "00:00" Else Return "00:00:00"
        End If
        tim = Int(tim)

        ''tim = CInt(tim * 25)      'převod na framy

        If tim < 0 Then
            znam = "-"
            tim = Math.Abs(tim)
        Else
            znam = ""
        End If

        ''hod = Int(tim / 90000)
        hod = Int(tim / 3600)

        ''If hod > 0 Then tim = tim - (hod * 90000)
        If hod > 0 Then tim = tim - (hod * 3600)

        Min = Int(tim / 60)
        If Min > 0 Then tim = tim - (Min * 60)


        ''sec = Int(tim / 25)
        ''If sec > 0 Then tim = tim - (sec * 25)

        ''If tim = 0 Then
        ''    fra = 0
        ''Else
        ''    fra = tim
        ''End If


        If hod < 10 Then
            chod = "0" + Trim(hod & "")
        Else
            chod = Trim(hod & "")
        End If

        If Min < 10 Then
            cmin = "0" + Trim(Min & "")
        Else
            cmin = Trim(Min & "")
        End If

        

        If bolIncludeSeconds Then
            Dim csec As String = ""
            If tim < 10 Then
                csec = "0" + Trim(tim & "")
            Else
                csec = Trim(tim & "")
            End If

            Return znam + chod + ":" + cmin & ":" & csec
        Else
            Return znam + chod + ":" + cmin
        End If


    End Function

    Function GetDecTimeFromSeconds(ByVal tim As Double, Optional ByVal dblRetPrec As Single = 0, Optional ByVal lngRoundToDecimals As Integer = 0) As Double
        If lngRoundToDecimals = 0 Then lngRoundToDecimals = 3
        'tim... časový úsek vyjádřený v sekundách
        If tim = 0 Then
            GetDecTimeFromSeconds = 0
            Exit Function
        End If
        Dim lng As Integer, dbl As Double

        lng = CInt(tim)
        dbl = Math.Round(tim / 60 / 60, lngRoundToDecimals)
        dblRetPrec = Math.Round(tim / 60 / 60, 6)
        GetDecTimeFromSeconds = dbl
    End Function

    Public Function KolikZbyvaCasu(ByVal datFrom As Date, datTo As Date, ByVal intMaxPocetUrovni As Integer, ByVal bolShowZeroUnits As Boolean) As String
        Dim s As String = "", x As Integer = 0
        Dim dny As Long = DateDiff(DateInterval.Day, datFrom, datTo, , 0)
        If (dny = 0 And bolShowZeroUnits) Or dny <> 0 Then
            If x < intMaxPocetUrovni Then s = dny.ToString & "d"
            x += 1
        End If

        Dim hod As Long = DateDiff(DateInterval.Hour, datFrom, datTo, 0)
        If dny > 0 Then hod = hod - dny * 24
        If (hod = 0 And bolShowZeroUnits) Or hod <> 0 Then
            If x < intMaxPocetUrovni Then s += " " & hod.ToString & "h"
            x += 1
        End If

        If x < intMaxPocetUrovni Then
            Dim min As Long = DateDiff(DateInterval.Minute, datFrom, datTo, 0)
            If dny > 0 Then min = min - dny * 24 * 60
            If hod > 0 Then min = min - hod * 60
            s += " " & min.ToString & "m"
        End If

        Return LTrim(s)
    End Function

    Public Function GetDateFromDecimal(ByVal decHours As Decimal) As Date

        Dim intMinutes As Decimal = decHours * 60
        Return Today.AddMinutes(intMinutes)
    End Function

    Public Function GetDecimalFromDate(ByVal dat As Date) As Decimal
        Dim intHours As Decimal = dat.Hour
        Dim intMinutes As Decimal = dat.Minute
        Return intHours + intMinutes / 60
    End Function




End Class


