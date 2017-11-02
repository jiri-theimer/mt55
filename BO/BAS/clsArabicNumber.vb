Public Class clsArabicNumber
    Public Function NumberToRoman(ByVal nArabicValue As Integer) As String
        Dim nThousands As Integer, nFiveHundreds As Integer, nHundreds As Integer, nFifties As Integer, nTens As Integer, nFives As Integer, nOnes As Integer, tmp As String

        'vezmeme zaslanou hodnotu a rozdělíme ji na hodnoty,
        'reprezentující počty jedniček, desítek, stovek atd.
        nOnes = nArabicValue
        nThousands = nOnes \ 1000
        nOnes = nOnes - nThousands * 1000
        nFiveHundreds = nOnes \ 500
        nOnes = nOnes - nFiveHundreds * 500
        nHundreds = nOnes \ 100
        nOnes = nOnes - nHundreds * 100
        nFifties = nOnes \ 50
        nOnes = nOnes - nFifties * 50
        nTens = nOnes \ 10
        nOnes = nOnes - nTens * 10
        nFives = nOnes \ 5
        nOnes = nOnes - nFives * 5

        'Pomocí funkce String vytvoříme série řetězců, reprezentujících počet
        'jednotlivých hodnot
        tmp = StrDup(nThousands, "M")

        'Je třeba zachovat jisté zásady při práci s římskými číslicemi při vyjadřování
        'určitých hodnot - totiž některé "předsazené" číslice při reprezentaci určité hodnoty
        If nHundreds = 4 Then
            If nFiveHundreds = 1 Then
                tmp = tmp & "CM"
            Else
                tmp = tmp & "CD"
            End If
        Else
            tmp = tmp & StrDup(nFiveHundreds, "D") & StrDup(nHundreds, "C")

        End If

        If nTens = 4 Then
            If nFifties = 1 Then
                tmp = tmp & "XC"
            Else
                tmp = tmp & "XL"
            End If
        Else
            tmp = tmp & StrDup(nFifties, "L") & StrDup(nTens, "X")
        End If

        If nOnes = 4 Then
            If nFives = 1 Then
                tmp = tmp & "IX"
            Else
                tmp = tmp & "IV"
            End If
        Else
            tmp = tmp & StrDup(nFives, "V") & StrDup(nOnes, "I")
        End If

        Return tmp

    End Function


    Public Function RomanToNumber(ByVal strRoman As String) As Integer
        Dim cnt As Integer, strLen As Integer, nChar As Integer, nNextChar As Integer, nNextChar2 As Integer, tmpVal As Integer

        'převod na malá písmena a test na chybné znaky
        strRoman = LCase(strRoman)
        If InStr(strRoman, "iiii") Or _
           InStr(strRoman, "xxxx") Or _
           InStr(strRoman, "cccc") Or _
           InStr(strRoman, "vv") Or _
            InStr(strRoman, "ll") Or _
            InStr(strRoman, "dd") Then

            'Nalezena chyba, takže končíme
            Return -1
        End If

        'Pro každý znak v římském vyjádření čísla nalezneme jeho numerickou reprezentaci.
        'Například římské číslo 1995 (MCMXCV) je reprezentováno řetězcem "757352"
        strLen = Len(strRoman)
        For cnt = 1 To strLen
            Select Case Mid$(strRoman, cnt, 1)
                Case "i" : Mid$(strRoman, cnt, 1) = 1
                Case "v" : Mid$(strRoman, cnt, 1) = 2
                Case "x" : Mid$(strRoman, cnt, 1) = 3
                Case "l" : Mid$(strRoman, cnt, 1) = 4
                Case "c" : Mid$(strRoman, cnt, 1) = 5
                Case "d" : Mid$(strRoman, cnt, 1) = 6
                Case "m" : Mid$(strRoman, cnt, 1) = 7
            End Select
        Next

        For cnt = 1 To strLen

            nChar = CInt(Mid$(strRoman, cnt, 1))

            If cnt < strLen Then
                nNextChar = CInt(Mid$(strRoman, cnt + 1, 1))

                If cnt < strLen - 1 Then
                    nNextChar2 = CInt(Mid$(strRoman, cnt + 2, 1))
                Else
                    nNextChar2 = 0
                End If

                Select Case nChar

                    Case 7 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 1000)

                    Case 6 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 500)

                    Case 5 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 100)

                    Case 4 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 50)

                    Case 3 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 10)

                    Case 2 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 5)

                    Case 1 : tmpVal = GetTmpVal2(nChar, _
                                                nNextChar, _
                                                nNextChar2, _
                                                tmpVal, _
                                                cnt, 1)

                End Select
            Else
                tmpVal = tmpVal + ConvertValue(nChar)
            End If

            If tmpVal = -1 Then Exit For

        Next

        Return tmpVal

    End Function

    'Pomocné funkce
    Private Function GetTmpVal2(ByVal nChar As Integer, _
                                ByVal nNextChar As Integer, _
                                ByVal nNextChar1 As Integer, _
                                ByVal tmpVal As Integer, _
                                ByVal cnt As Integer, _
                                ByVal intValue As Integer) As Integer

        If nNextChar > nChar Then

            If ((nNextChar - nChar = 1 And _
                (nChar <> 2 And nChar <> 6)) _
            Or (nNextChar - nChar = 2 And _
               (nNextChar <> 4 And nNextChar <> 6))) _
            And nNextChar1 < nNextChar _
            And nNextChar1 <> nChar Then

                tmpVal = tmpVal + ConvertValue(nNextChar) - intValue
                cnt = cnt + 1

            Else
                tmpVal = -1
            End If

        Else
            tmpVal = tmpVal + intValue
        End If


        Return tmpVal

    End Function


    Private Function ConvertValue(ByVal nVal As Integer) As Integer

        Select Case nVal
            Case 7 : Return 1000
            Case 6 : Return 500
            Case 5 : Return 100
            Case 4 : Return 50
            Case 3 : Return 10
            Case 2 : Return 5
            Case 1 : Return 1
        End Select
        Return -1
    End Function
End Class
