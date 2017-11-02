Public Class BAS
    Shared Function MQ() As BO.myQuery
        Return New BO.myQuery
    End Function

    Shared Function GetX29EntityAlias(x29id As BO.x29IdEnum, bolMnozneCislo As Boolean) As String
        Select Case x29id
            Case BO.x29IdEnum.p41Project And Not bolMnozneCislo : Return "Projekt"
            Case BO.x29IdEnum.p41Project And bolMnozneCislo : Return "Projekty"
            Case BO.x29IdEnum.p28Contact And Not bolMnozneCislo : Return "Klient"
            Case BO.x29IdEnum.p28Contact And bolMnozneCislo : Return "Klienti"
            Case x29IdEnum.o23Doc And Not bolMnozneCislo : Return "Dokument"
            Case x29IdEnum.o23Doc And bolMnozneCislo : Return "Dokumenty"
            Case BO.x29IdEnum.p31Worksheet : Return "Worksheet úkon"
            Case BO.x29IdEnum.p56Task And Not bolMnozneCislo : Return "Úkol"

            Case BO.x29IdEnum.x31Report And Not bolMnozneCislo : Return "Tisková sestava"

            Case BO.x29IdEnum.j02Person And Not bolMnozneCislo : Return "Osoba"
            Case BO.x29IdEnum.j02Person And bolMnozneCislo : Return "Lidé"
            Case BO.x29IdEnum.p91Invoice And Not bolMnozneCislo : Return "Faktura"
            Case BO.x29IdEnum.p91Invoice And bolMnozneCislo : Return "Faktury"
            Case BO.x29IdEnum.p90Proforma And Not bolMnozneCislo : Return "Záloha"
            Case BO.x29IdEnum.p90Proforma And bolMnozneCislo : Return "Zálohy"
            Case BO.x29IdEnum.p56Task And Not bolMnozneCislo : Return "Úkol"
            Case BO.x29IdEnum.p56Task And bolMnozneCislo : Return "Úkoly"

            Case BO.x29IdEnum.j07PersonPosition And Not bolMnozneCislo : Return "Pozice"
            Case BO.x29IdEnum.j07PersonPosition And bolMnozneCislo : Return "Pozice"
            Case BO.x29IdEnum.j18Region And Not bolMnozneCislo : Return "Středisko"
            Case BO.x29IdEnum.j18Region And bolMnozneCislo : Return "Střediska"
            Case BO.x29IdEnum.p34ActivityGroup And Not bolMnozneCislo : Return "Sešit"
            Case BO.x29IdEnum.p34ActivityGroup And bolMnozneCislo : Return "Sešity"
            Case BO.x29IdEnum.p32Activity And Not bolMnozneCislo : Return "Aktivita"
            Case BO.x29IdEnum.p32Activity And bolMnozneCislo : Return "Aktivity"

            Case Else
                Return ""
        End Select
    End Function
    Shared Function GetDataPrefix(x29id As x29IdEnum) As String
        Return Left(x29id.ToString, 3)
        
    End Function
    Shared Function GetX29FromPrefix(strPrefix As String) As BO.x29IdEnum
        Select Case strPrefix
            Case "p41" : Return x29IdEnum.p41Project
            Case "p91" : Return x29IdEnum.p91Invoice
            Case "p90" : Return x29IdEnum.p90Proforma
            Case "p82" : Return x29IdEnum.p82Proforma_Payment
            Case "p28" : Return x29IdEnum.p28Contact
            Case "p29" : Return x29IdEnum.p29ContactType
            Case "b02" : Return x29IdEnum.b02WorkflowStatus
            Case "b01" : Return x29IdEnum.b01WorkflowTemplate
            Case "p42" : Return x29IdEnum.p42ProjectType
            Case "p31" : Return x29IdEnum.p31Worksheet
            Case "j03" : Return x29IdEnum.j03User
            Case "j02" : Return x29IdEnum.j02Person
            Case "p56" : Return x29IdEnum.p56Task
            Case "p57" : Return x29IdEnum.p57TaskType
            Case "o23" : Return x29IdEnum.o23Doc
            Case "o22" : Return x29IdEnum.o22Milestone
            Case "j23" : Return x29IdEnum.j23NonPerson
            Case "j24" : Return x29IdEnum.j24NonPersonType
            Case "x31" : Return x29IdEnum.x31Report
            Case "b07" : Return x29IdEnum.b07Comment
            Case "app" : Return x29IdEnum.Approving
            Case "j18" : Return x29IdEnum.j18Region
            Case "p45" : Return x29IdEnum.p45Budget
            Case "p49" : Return x29IdEnum.p49FinancialPlan
            Case "p48" : Return x29IdEnum.p48OperativePlan
            Case "p47" : Return x29IdEnum.p47CapacityPlan
            Case "x18" : Return x29IdEnum.x18EntityCategory
            Case "p32" : Return x29IdEnum.p32Activity
            Case "p34" : Return x29IdEnum.p34ActivityGroup
            Case "p51" : Return x29IdEnum.p51PriceList
            Case Else
                Return x29IdEnum._NotSpecified
        End Select
    End Function
    Shared Function IsNullDBKey(ByVal val As Object) As Object
        If val Is Nothing Then Return Nothing
        If Not IsNumeric(val) Then Return Nothing
        If val = 0 Then Return Nothing
        Return CInt(val)
    End Function
    Shared Function IsNullInt(ByVal val As Object, Optional ByVal intIfNullSetValue As Integer = 0) As Integer
        If val Is Nothing Then Return intIfNullSetValue
        If val Is System.DBNull.Value Then
            Return intIfNullSetValue
        Else
            Try
                Return CInt(val)
            Catch ex As Exception
                Return 0
            End Try
        End If
    End Function

    Shared Function IsNullNum(ByVal val As Object, Optional ByVal dblIfNullSetValue As Double = 0) As Double
        If val Is Nothing Then Return dblIfNullSetValue
        If val Is System.DBNull.Value Then
            Return dblIfNullSetValue
        Else
            Try
                Return CDbl(val)
            Catch ex As Exception
                Return 0
            End Try

        End If
    End Function
    Shared Function IsNull(ByVal val As Object, Optional ByVal strIfNullSetValue As String = "") As String
        If val Is Nothing Then Return strIfNullSetValue
        If val Is System.DBNull.Value Then Return strIfNullSetValue Else Return val.ToString
    End Function
    Shared Function IsNullDBDate(ByVal val As Object) As Date?
        'If val Is Nothing Then Return DateSerial(1900, 1, 1)
        If val Is Nothing Then Return Nothing
        If val Is System.DBNull.Value Then
            'Return DateSerial(1900, 1, 1)
            Return Nothing
        Else
            'If Year(val) < 1900 Then Return DateSerial(1900, 1, 1)
            If Year(val) <= 1900 Then Return Nothing
            Return CDate(val)
        End If
    End Function
    Shared Function IsListEmpty(Of T)(lis As List(Of T)) As Boolean
        If lis Is Nothing Then Return True
        If lis.Count = 0 Then Return True

        Return False
    End Function

    Shared Function FD_TimePeriod(d1 As Date, d2 As Date) As String
        Dim s As String = ""
        If d1.Day = d2.Day And d1.Month = d2.Month And d1.Year = d2.Year Then
            s = Format(d1, "dd.MM.yyyy ddd HH:mm") & " - " & Format(d2, "HH:mm")
        Else
            s = FD(d1, True) & " - " & FD(d2, True)
        End If

        Return s & " (" & DatesIntervalDuration(d1, d2) & ")"

    End Function
    Shared Function DatesIntervalDuration(d1 As Date, d2 As Date) As String
        Dim ts As TimeSpan = d2 - d1
        Dim hours As Double = ts.TotalHours
        If hours > 24 Then
            Dim dur As New DatesDuration(d1, d2)
            If dur.Months > 1 Then
                Return dur.Months.ToString & "m " & dur.Days.ToString & "d"
            Else
                Return dur.Days.ToString & "d"
            End If
        Else
            Return Format(hours, "Standard") & "hod."
        End If


    End Function
    
    Shared Function FD(ByVal val As Object, Optional ByVal bolShowTimeIfIsFilled As Boolean = False, Optional ByVal bolShowDiff2Now As Boolean = False) As String
        If val Is Nothing Then Return ""
        If val Is System.DBNull.Value Then
            Return ""
        Else
            Dim dat As Date = CDate(val)
            If Year(dat) <= 1900 Then Return ""

            Dim s As String = ""
            If bolShowDiff2Now Then
                Dim days As Integer = DateDiff(DateInterval.Day, dat, Now, FirstDayOfWeek.Monday)
                If days <> 0 Then
                    If days > 365 Or days < -365 Then
                        Dim cDur As New DatesDuration(dat, Now)
                        s = cDur.Years.ToString & "r"
                        If cDur.Months > 0 Then
                            s += " " & cDur.Months.ToString & "m"
                        End If
                        If days > 0 Then s = "(-" & s Else s = "(" & s
                    Else
                        If days > 31 Or days < -31 Then
                            Dim cDur As New DatesDuration(dat, Now)
                            s = cDur.Months.ToString & "m"
                            If cDur.Days > 0 Then
                                s += " " & cDur.Days.ToString & "d"
                            End If
                            If days > 0 Then s = "(-" & s Else s = "(" & s
                        Else
                            s = " (" & DateDiff(DateInterval.Day, Now, dat, FirstDayOfWeek.Monday).ToString & "d"
                        End If
                    End If
                Else
                    days = DateDiff(DateInterval.Hour, dat, Now, FirstDayOfWeek.Monday)
                    If days <> 0 Then s = " (" & DateDiff(DateInterval.Hour, Now, dat, FirstDayOfWeek.Monday).ToString & "h"
                End If

                If days < 0 Then s += " zbývá)"
                If days > 0 Then s += ")"



                If dat.Day = Today.Day And dat.Month = Today.Month And dat.Year = Today.Year Then
                    If dat.Hour = 0 And dat.Minute = 0 Then Return "dnes" Else Return "dnes " & Format(dat, "HH:mm")
                End If
            End If
            If Not bolShowTimeIfIsFilled Then Return Format(dat, "dd.MM.yyyy ddd") & s

            'zde zobrazit i přesný čas
            If dat.Hour > 0 Or dat.Minute > 0 Then
                Return Format(dat, "dd.MM.yyyy HH:mm ddd") & s
            Else
                Return Format(dat, "dd.MM.yyyy ddd") & s
            End If
        End If
    End Function
    
    Shared Function FN(dbl As Double) As String
        Return Format(dbl, "standard")
    End Function
    Shared Function FN2(dbl As Double) As String
        If dbl = 0 Then Return ""
        Return Format(dbl, "standard")
    End Function
    Shared Function FN3(dbl As Double) As String
        If dbl = 0 Then Return ""
        If Math.Truncate(dbl) = dbl Then
            Return Format(dbl, "N0")
        Else
            Return Format(dbl, "standard")
        End If
    End Function
    Shared Function FNI(dbl As Double) As String
        Return Format(dbl, "N0")
    End Function
    Shared Function BG(ByVal strBol As String) As Boolean
        If strBol = "1" Or strBol = "-1" Then Return True Else Return False
    End Function

    Shared Function GB(ByVal bol As Boolean) As String
        If bol Then Return "1" Else Return "0"

    End Function
    Shared Function GB_Display(ByVal bol As Boolean) As String
        If bol Then Return "inline-block" Else Return "none"
    End Function

    Shared Function CrLfText2Html(strText As String) As String
        If strText = "" Then Return strText
        If strText.IndexOf(vbCrLf) > 0 Then
            Return strText.Replace(vbCrLf, "<br>")
        Else
            If strText.IndexOf(Chr(10)) > 0 Then
                Return strText.Replace(Chr(10), "<br>")
            End If
            Return strText
        End If
    End Function
    Shared Function Uvozovky2Apostrofy(strText As String) As String
        If strText = "" Then Return strText
        If strText.IndexOf(Chr(34)) > 0 Then
            Return strText.Replace(Chr(34), "'")
        Else
            Return strText
        End If
    End Function
    Public Overloads Shared Function OM1(strExpression As String, Optional bolOnlyIfFirstIsComma As Boolean = False) As String
        If strExpression <> "" Then
            If bolOnlyIfFirstIsComma Then
                'hledat čárku zleva
                If Left(strExpression, 1) <> "," Then Return strExpression
            End If
            Return Right(strExpression, Len(strExpression) - 1)
        Else
            Return ""
        End If
    End Function
    Public Overloads Shared Function OM1(strExpression As String, intLeftX As Integer) As String
        If strExpression <> "" Then
            Return Right(strExpression, Len(strExpression) - intLeftX)
        Else
            Return ""
        End If
    End Function
    Public Shared Function OM2(ByVal strExpression As String, ByVal strAppendExpression As String) As String
        If strExpression.IndexOf("(") > 0 Then
            strExpression = Trim(Left(strExpression, strExpression.IndexOf("(")))
        End If
        Return strExpression & " (" & strAppendExpression & ")"
    End Function
    Public Shared Function OM3(ByVal strExpression As String, intLimitLength As Integer) As String
        If strExpression = "" Then Return ""
        With strExpression
            If .Length > intLimitLength - 2 Then
                Return Left(strExpression, intLimitLength - 2) & "..."
            Else
                Return strExpression
            End If
        End With

    End Function
    Public Shared Function OM4(ByVal strExpression As String, ByVal strAppendExpression As String, Optional strDelimiter As String = ",") As String
        If strExpression = "" Then
            Return strAppendExpression
        Else
            Return strExpression & strDelimiter & strAppendExpression
        End If
    End Function

    Public Shared Function ConvertString2Date(ByVal strDat As String, Optional strFormat As String = "dd.MM.yyyy") As Date
        Try
            Return DateTime.ParseExact(strDat, strFormat, Nothing)
        Catch ex1 As Exception
            Try
                Return DateTime.Parse(strDat)
            Catch ex2 As Exception
                Return Nothing
            End Try
        End Try


    End Function

    Public Shared Function FormatFileSize(ByVal lngBytes As Long) As String
        Dim DoubleBytes As Double
        Try
            Select Case lngBytes
                Case Is >= 1099511627776
                    DoubleBytes = CDbl(lngBytes / 1099511627776) 'TB
                    Return FormatNumber(DoubleBytes, 2) & " TB"
                Case 1073741824 To 1099511627775
                    DoubleBytes = CDbl(lngBytes / 1073741824) 'GB
                    Return FormatNumber(DoubleBytes, 2) & " GB"
                Case 1048576 To 1073741823
                    DoubleBytes = CDbl(lngBytes / 1048576) 'MB
                    Return BO.BAS.FN3(DoubleBytes) & " MB"
                Case 1024 To 1048575
                    DoubleBytes = CDbl(lngBytes / 1024) 'KB
                    Return FormatNumber(DoubleBytes, 2) & " KB"
                Case 0 To 1023
                    DoubleBytes = lngBytes ' bytes
                    Return FormatNumber(DoubleBytes, 2) & " bytes"
                Case Else
                    Return ""
            End Select
        Catch
            Return ""
        End Try
    End Function
    Public Shared Function GS(ByVal str As String) As String
        str = Replace(str, "'", "''")
        Return "'" & str & "'"
    End Function
    Public Shared Function GN(ByVal str As String) As String
        Dim i As Long, s As String
        str = Trim(str)
        str = Replace(str, " ", "")
        str = Replace(str, ",", ".")

        For i = 1 To Len(str)
            s = Mid(str, i, 1)
            If Not IsNumeric(s) Then
                If s = "," Then
                    s = "."
                    str = Replace(str, ",", ".")
                End If
                If Not (s = "." Or (s = "-" And i = 1)) Then
                    str = Replace(str, s, "")
                End If
            End If
        Next
        If Not IsNumeric(Replace(str, ".", ",")) Then str = "0"
        Return str
    End Function
    Public Shared Function GetHashDate(ByVal varDatum As DateTime, Optional ByVal bolSaveTime As Boolean = False) As String
        If Not IsDate(varDatum) Then
            Return "null"
        End If

        Dim vyraz As String = Right("0" & Day(varDatum), 2) & "." & Right("0" & Month(varDatum), 2) & "." & Year(varDatum)
        If bolSaveTime Then
            vyraz = vyraz & " " & Right("0" & Hour(varDatum), 2) & ":" & Right("0" & Minute(varDatum), 2) & ":" & Right("0" & Second(varDatum), 2)
        End If
        vyraz = "convert(datetime,'" & vyraz & "',104)"
        Return vyraz
    End Function

    Public Shared Function GetPropertiesNames(ByVal obj As Object) As List(Of String)
        Dim objType As Type = obj.GetType(), lis As New List(Of String)
        Try
            For Each pInfo In objType.GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.IgnoreCase)
                lis.Add(pInfo.Name)
            Next
        Catch ex As Exception

        End Try

        Return lis

    End Function

    Public Shared Function GetPropertyValue(ByVal obj As Object, ByVal PropName As String) As Object
        Dim objType As Type = obj.GetType(), PropValue As Object = Nothing
        Try
            Dim pInfo As System.Reflection.PropertyInfo = objType.GetProperty(PropName, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.IgnoreCase)

            PropValue = pInfo.GetValue(obj, Reflection.BindingFlags.GetProperty, Nothing, Nothing, Nothing)
        Catch ex As Exception

        End Try
        
        Return PropValue

    End Function
    Public Shared Sub SetPropertyValue(obj As Object, PropName As String, objNewValue As Object)
        Dim objType As Type = obj.GetType()
        'Dim pinfo As System.Reflection.PropertyInfo = objType.GetProperty(PropName)
        Dim pInfo As System.Reflection.PropertyInfo = objType.GetProperty(PropName, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.IgnoreCase)


        pinfo.SetValue(obj, objNewValue, Nothing)
    End Sub

    Public Shared Function GetGUID() As String
        Return Guid.NewGuid().ToString("N")
    End Function

    Shared Function GetListFromDateRange(d1 As Date, d2 As Date) As List(Of Date)
        Dim lisDates As New List(Of Date)
        For i As Double = 0 To DateDiff(DateInterval.Day, d1, d2, Microsoft.VisualBasic.FirstDayOfWeek.Monday)
            lisDates.Add(d1.AddDays(i))
        Next
        Return lisDates
    End Function

    Overloads Shared Function TestPermission(cUser As BO.j03UserSYS, intNeededPermissionValue As BO.x53PermValEnum) As Boolean
        'If _cUser.IsAdmin Then Return True
        If cUser.RoleValue.Substring(intNeededPermissionValue - 1, 1) = "1" Then
            Return True
        Else
            Return False
        End If
    End Function
    Overloads Shared Function TestPermission(cUser As BO.j03UserSYS, intNeededPermissionValue As BO.x53PermValEnum, OrSecondPermission As BO.x53PermValEnum) As Boolean
        'If _cUser.IsAdmin Then Return True
        If cUser.RoleValue.Substring(intNeededPermissionValue - 1, 1) = "1" Then
            Return True
        Else
            If cUser.RoleValue.Substring(OrSecondPermission - 1, 1) = "1" Then Return True
            Return False
        End If
    End Function
    Shared Function TestEMailAddress(strEmail As String, ByRef strRetError As String) As Boolean
        strRetError = ""
        ''Try
        ''    Dim testMail As New System.Net.Mail.MailAddress(strEmail)
        ''    Return True
        ''Catch ex As Exception
        ''    strRetError = ex.Message
        ''    Return False
        ''End Try
        Dim isValid As Boolean = True, strEmailOrig As String = strEmail

        Dim emailExpression As New System.Text.RegularExpressions.Regex("^((\s*[a-zA-Z0-9\._%-]+@[a-zA-Z0-9\.-]+\.[a-zA-Z]{2,4}\s*[,;:]){1,100}?)?(\s*[a-zA-Z0-9\._%-]+@[a-zA-Z0-9\.-]+\.[a-zA-Z]{2,6})*$")
        Try
            strEmail = strEmail.Trim.Replace(",", ";")
            strEmail = strEmail.Trim.Replace(" ", "")
            isValid = emailExpression.IsMatch(strEmail.Trim)

            If Not isValid Then
                strRetError = "E-mail adresa [" & strEmailOrig & "] není akceptovatelná."
            End If
            Return isValid

        Catch ex As Exception
            strRetError = ex.Message
            Return False
        End Try
    End Function

    Shared Function RemoveDiacritism(ByVal Text As String) As String
        Dim stringFormD = Text.Normalize(System.Text.NormalizationForm.FormD)
        Dim retVal As New System.Text.StringBuilder()
        For index As Integer = 0 To stringFormD.Length - 1
            If (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stringFormD(index)) <> Globalization.UnicodeCategory.NonSpacingMark) Then
                retVal.Append(stringFormD(index))
            End If
        Next
        Return retVal.ToString().Normalize(System.Text.NormalizationForm.FormC)
    End Function
    Shared Function Prepare4FileName(s As String) As String
        'očistí výraz s, aby z toho byl validní název souboru
        s = Replace(s, " | ", "_").Replace(" ", "-").Replace(":", "_")
        s = BO.BAS.RemoveDiacritism(s)

        s = System.Text.RegularExpressions.Regex.Replace(s, "[^A-Za-z0-9 _ -]", "").Replace(" ", "")
        Return s
    End Function

    Shared Function GLX(strExpression As String, strPar1 As String, Optional strPar2 As String = "") As String
        strExpression = Replace(strExpression, "%1%", strPar1)
        If strPar2 <> "" Then
            strExpression = Replace(strExpression, "%2%", strPar2)
        End If
        Return strExpression
    End Function

    Shared Function ConvertTimeToHours(strTime As String) As Double
        If strTime.IndexOf(":") > 0 Then
            Dim cTime As New clsTime
            Return cTime.ConvertTimeToSeconds(strTime) / 60 / 60
        Else
            Return BO.BAS.IsNullNum(strTime)
        End If
    End Function

    Public Shared Function ConvertInt2List(int1 As Integer) As List(Of Integer)
        Dim a As New List(Of Integer)
        a.Add(int1)
        Return a
    End Function
    Public Shared Function ConvertPIDs2List(strPIDs As String, Optional strDelimiter As String = ",") As List(Of Integer)
        Dim lis As New List(Of Integer)
        If strPIDs = "" Then Return lis
        Dim a() As String = Split(strPIDs, strDelimiter)
        For i As Integer = 0 To UBound(a)
            lis.Add(a(i))
        Next
        Return lis
    End Function
    Public Shared Function ConvertDelimitedString2List(s As String, Optional strDelimiter As String = ",") As List(Of String)
        Dim lis As New List(Of String)
        If s = "" Then Return lis
        Dim a() As String = Split(s, strDelimiter)
        For i As Integer = 0 To UBound(a)
            lis.Add(a(i))
        Next
        Return lis
    End Function
    

    Public Shared Function GetFileExtensionIcon(strExtension As String) As String
        If strExtension = "" Then Return "other.png"
        If Left(strExtension, 1) = "." Then strExtension = Right(strExtension, Len(strExtension) - 1)
        Dim strIMG As String = BO.ASS.GetApplicationRootFolder & "\Images\Files\" & LCase(strExtension) & ".png"
        If System.IO.File.Exists(strIMG) Then
            Return LCase(strExtension) & ".png"
        Else
            Return "other.png"
        End If
    End Function
End Class
