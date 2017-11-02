Public Class p32ActivityDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p32Activity
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p32ID=@p32ID"

        Return _cDB.GetRecord(Of BO.p32Activity)(s, New With {.p32ID = intPID})
    End Function

    Public Function Save(cRec As BO.p32Activity) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p32ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p34ID", BO.BAS.IsNullDBKey(.p34ID), DbType.Int32)
            pars.Add("p95ID", BO.BAS.IsNullDBKey(.p95ID), DbType.Int32)
            pars.Add("p35ID", BO.BAS.IsNullDBKey(.p35ID), DbType.Int32)
            pars.Add("x15ID", BO.BAS.IsNullDBKey(.x15ID), DbType.Int32)
            
            pars.Add("p32Name", .p32Name, DbType.String, , , True, "Název")
            pars.Add("p32Code", .p32Code, DbType.String, , , True, "Kód")
            pars.Add("p32DefaultWorksheetText", .p32DefaultWorksheetText, DbType.String, , , True, "Výchozí popis úkonu")

            pars.Add("p32Ordinary", .p32Ordinary, DbType.Int32)

            pars.Add("p32IsBillable", .p32IsBillable, DbType.Boolean)
            pars.Add("p32IsTextRequired", .p32IsTextRequired, DbType.Boolean)
            pars.Add("p32HelpText", .p32HelpText, DbType.String, , , True, "Nápověda pro zapisování úkonu")
            pars.Add("p32Color", .p32Color, DbType.String)

            pars.Add("p32Name_EntryLang1", .p32Name_EntryLang1, DbType.String)
            pars.Add("p32Name_EntryLang2", .p32Name_EntryLang2, DbType.String)
            pars.Add("p32Name_EntryLang3", .p32Name_EntryLang3, DbType.String)
            pars.Add("p32Name_EntryLang4", .p32Name_EntryLang4, DbType.String)

            pars.Add("p32Name_BillingLang1", .p32Name_BillingLang1, DbType.String)
            pars.Add("p32Name_BillingLang2", .p32Name_BillingLang2, DbType.String)
            pars.Add("p32Name_BillingLang3", .p32Name_BillingLang3, DbType.String)
            pars.Add("p32Name_BillingLang4", .p32Name_BillingLang4, DbType.String)

            pars.Add("p32FreeText01", .p32FreeText01, DbType.String)
            pars.Add("p32FreeText02", .p32FreeText02, DbType.String)
            pars.Add("p32FreeText03", .p32FreeText03, DbType.String)

            pars.Add("p32DefaultWorksheetText_Lang1", .p32DefaultWorksheetText_Lang1, DbType.String, , , True, "Výchozí popis úkonu - fakturační jazyk 1")
            pars.Add("p32DefaultWorksheetText_Lang2", .p32DefaultWorksheetText_Lang2, DbType.String, , , True, "Výchozí popis úkonu - fakturační jazyk 2")
            pars.Add("p32DefaultWorksheetText_Lang3", .p32DefaultWorksheetText_Lang3, DbType.String, , , True, "Výchozí popis úkonu - fakturační jazyk 3")
            pars.Add("p32DefaultWorksheetText_Lang4", .p32DefaultWorksheetText_Lang4, DbType.String, , , True, "Výchozí popis úkonu - fakturační jazyk 4")

            pars.Add("p32Value_Default", .p32Value_Default, DbType.Double)
            pars.Add("p32Value_Minimum", .p32Value_Minimum, DbType.Double)
            pars.Add("p32Value_Maximum", .p32Value_Maximum, DbType.Double)

            pars.Add("p32validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p32validuntil", .ValidUntil, DbType.DateTime)
            pars.Add("p32ExternalPID", .p32ExternalPID, DbType.String)
            pars.Add("p32AttendanceFlag", CInt(.p32AttendanceFlag), DbType.Int32)
            pars.Add("p32ManualFeeFlag", .p32ManualFeeFlag, DbType.Int32)
            pars.Add("p32ManualFeeDefAmount", .p32ManualFeeDefAmount, DbType.Double)
        End With

        If _cDB.SaveRecord("p32Activity", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p32_delete", pars)
    End Function


    Public Function GetList(myQuery As BO.myQueryP32) As IEnumerable(Of BO.p32Activity)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p32ID", myQuery)
        strW += bas.ParseWhereValidity("p32", "a", myQuery)
        With myQuery
            If Not .p33ID Is Nothing Then
                pars.Add("p33id", CInt(.p33ID), DbType.Int32)
                strW += " AND p34.p33ID=@p33id"
            End If
            If .IsMoneyInput = BO.BooleanQueryMode.TrueQuery Then
                strW += " AND p34.p33ID IN (2,5)"
            End If
            If .IsMoneyInput = BO.BooleanQueryMode.FalseQuery Then
                strW += " AND p34.p33ID NOT IN (2,5)"
            End If
            If .p34ID <> 0 Then
                pars.Add("p34id", .p34ID, DbType.Int32)
                strW += " AND a.p34ID=@p34id"
            End If
            If .p61ID > 0 Then
                pars.Add("p61id", .p61ID, DbType.Int32)
                strW += " AND a.p32ID IN (SELECT p32ID FROM p62ActivityCluster_Item WHERE p61ID=@p61id)"
            End If
            If Not .x15ID Is Nothing Then
                pars.Add("x15id", .x15ID, DbType.Int32)
                strW += " AND a.x15ID=@x15id"
            End If
            If .Billable <> BO.BooleanQueryMode.NoQuery Then
                pars.Add("p32isbillable", .Billable, DbType.Int32)
                strW += " AND a.p32IsBillable=@p32isbillable"
            End If
            If Trim(.SearchExpression) <> "" Then
                strW += " AND (p32Name like '%'+@expr+'%' OR p34.p34Name LIKE '%'+@expr+'%' OR p32Code like @expr OR p34.p34Code like @expr)"
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
        End With

        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p32Ordinary,p32Name"

        Return _cDB.GetList(Of BO.p32Activity)(s, pars)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p32", "a")
        s += ",p34.p34Name as _p34Name,p95.p95Name as _p95Name,x15.x15Name as _x15Name,p34.p33ID as _p33ID,p34.p34IncomeStatementFlag as _p34IncomeStatementFlag"
        s += " FROM p32Activity a INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " LEFT OUTER JOIN p95InvoiceRow p95 ON a.p95ID=p95.p95ID"
        s += " LEFT OUTER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID"
        Return s
    End Function
End Class
