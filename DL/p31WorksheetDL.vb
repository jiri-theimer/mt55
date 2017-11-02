Public Class p31WorksheetDL
    Inherits DLMother
    Public Event NeedHandleAppEvents(strX45IDs As String, intP41ID As Integer)

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p31Worksheet
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2()
        s += " WHERE a.p31ID=@p31id"

        Return _cDB.GetRecord(Of BO.p31Worksheet)(s, New With {.p31id = intPID})
    End Function
    Public Function LoadTempRecord(intPID As Integer, strGUID_TempData As String) As BO.p31Worksheet
        Dim pars As New DbParameters
        pars.Add("p31id", intPID, DbType.Int32)
        pars.Add("guid", strGUID_TempData, DbType.String)

        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(strGUID_TempData)
        s += " WHERE a.p31ID=@p31id AND a.p31GUID=@guid"

        Return _cDB.GetRecord(Of BO.p31Worksheet)(s, pars)
    End Function
    Public Function LoadMyLastCreated(bolLoadTheSameTypeIfNoData As Boolean, Optional intP41ID As Integer = 0) As BO.p31Worksheet
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2()
        s += " WHERE a.j02ID_Owner=@j02id_owner"
        If intP41ID > 0 Then
            s += " AND a.p41ID=@p41id"
        End If
        s += " ORDER BY a.p31ID DESC"

        Dim pars As New DbParameters
        pars.Add("j02id_owner", _curUser.j02ID, DbType.Int32)
        pars.Add("p41id", intP41ID, DbType.Int32)

        Dim cRec As BO.p31Worksheet = _cDB.GetRecord(Of BO.p31Worksheet)(s, pars)
        If bolLoadTheSameTypeIfNoData And cRec Is Nothing And intP41ID <> 0 Then
            s = GetSQLPart1(1) & " " & GetSQLPart2() & " WHERE a.j02ID_Owner=@j02id_owner AND p41.p42ID IN (SELECT p42ID FROM p41Project WHERE p41ID=@p41id)"
            cRec = _cDB.GetRecord(Of BO.p31Worksheet)(s, pars)
        End If
        Return cRec
    End Function
    Public Function LoadMyLastCreated_TimeRecord() As BO.p31Worksheet
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2()
        s += " WHERE a.j02ID_Owner=@j02id_owner AND p34.p33ID=1 ORDER BY a.p31ID DESC"

        Return _cDB.GetRecord(Of BO.p31Worksheet)(s, New With {.j02id_owner = _curUser.j02ID})
    End Function
    Public Function ValidateBeforeSaveOrigRecord(cRecTU As BO.p31WorksheetEntryInput, lisFF As List(Of BO.FreeField)) As BO.p31ValidateBeforeSave
        Dim pars As New DbParameters
        With pars
            .Add("p31id", cRecTU.PID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("j02id_rec", cRecTU.j02ID, DbType.Int32)
            .Add("p41id", BO.BAS.IsNullDBKey(cRecTU.p41ID), DbType.Int32)
            .Add("p56id", BO.BAS.IsNullDBKey(cRecTU.p56ID), DbType.Int32)
            .Add("p31date", cRecTU.p31Date, DbType.DateTime)
            .Add("p32id", BO.BAS.IsNullDBKey(cRecTU.p32ID), DbType.Int32)
            .Add("p48id", BO.BAS.IsNullDBKey(cRecTU.p48ID), DbType.Int32)
            .Add("p31vatrate_orig", cRecTU.VatRate_Orig, DbType.Double)
            .Add("j27id_explicit", cRecTU.j27ID_Billing_Orig, DbType.Int32)
            .Add("p31text", cRecTU.p31Text, DbType.String)

            .Add("value_orig", cRecTU.p31Value_Orig, DbType.Double)
            .Add("manualfee", cRecTU.ManualFee, DbType.Double)

            .Add("err", , DbType.String, ParameterDirection.Output, 500)
            .Add("round2minutes", , DbType.Int32, ParameterDirection.Output)
            .Add("j27id_domestic", , DbType.Int32, ParameterDirection.Output)
            .Add("p33id", , DbType.Int32, ParameterDirection.Output)
            .Add("vatrate", , DbType.Double, ParameterDirection.Output)
        End With

        Dim c As New BO.p31ValidateBeforeSave

        If _cDB.RunSP("p31_test_beforesave", pars) Then
            c.ErrorMessage = pars.Get(Of String)("err")
            If c.ErrorMessage = "" Then
                c.Round2Minutes = pars.Get(Of Int32)("round2minutes")
                c.j27ID_Domestic = pars.Get(Of Int32)("j27id_domestic")
                c.p33ID = pars.Get(Of Int32)("p33id")
                c.VatRate = BO.BAS.IsNullNum(pars.Get(Of Double)("vatrate"))
            End If

        Else
            c.ErrorMessage = _cDB.ErrorMessage
        End If

        Return c
    End Function

    Public Function Update_p31Text(intPID As Integer, strNewText As String, Optional strGUID_TempData As String = "") As Boolean
        Dim pars As New DbParameters
        pars.Add("pid", intPID)
        pars.Add("p31Text", strNewText, DbType.String, , , True, "Popis úkonu")
        If strGUID_TempData = "" Then
            If _cDB.SaveRecord("p31Worksheet", pars, False, "p31ID=@pid", True, _curUser.j03Login) Then
                Return True
            Else
                Return False
            End If
        Else

            If _cDB.SaveRecord("p31Worksheet_Temp", pars, False, "p31ID=@pid AND p31GUID='" & strGUID_TempData & "'", True, _curUser.j03Login) Then
                Return True
            Else
                Return False
            End If
        End If
        
    End Function

    Public Function UpdateTempField(strField As String, dbValue As Object, strGUID As String, intP31ID As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("pid", intP31ID, DbType.Int32)
        pars.Add("guid", strGUID, DbType.String)
        pars.Add("val", dbValue)
        Return _cDB.RunSQL("UPDATE p31Worksheet_Temp SET " & strField & "=@val WHERE p31GUID=@guid AND p31ID=@pid", pars)
    End Function
    Public Function Save_Approving(cA As BO.p31WorksheetApproveInput) As Boolean
        'strGUID_TempData As String, intPID As Integer, p71id As BO.p71IdENUM, p72id As BO.p72IdENUM, dblValue_Approved_Billing As Double, dblRate_Billing_Approved As Double, dblValue_Approved_Internal As Double, dblRate_Internal_Approved As Double, strP31Text As String, dblVatRate_Approved As Double, strApprovingSet As String, datDate As Date?, intApprovingLevel As Integer, dblValue_FixPrice As Double
        Dim pars As New DbParameters
        With pars
            If cA.GUID_TempData <> "" Then
                .Add("guid", cA.GUID_TempData, DbType.String)   'TEMP - dočasná data
            End If
            .Add("p31id", cA.p31ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("p71id", BO.BAS.IsNullDBKey(cA.p71id), DbType.Int32)
            .Add("p72id", BO.BAS.IsNullDBKey(cA.p72id), DbType.Int32)
            .Add("approvingset", cA.p31ApprovingSet, DbType.String)
            .Add("value_approved_internal", cA.Value_Approved_Internal, DbType.Double)
            .Add("value_approved_billing", cA.Value_Approved_Billing, DbType.Double)
            .Add("rate_billing_approved", cA.Rate_Billing_Approved, DbType.Double)
            .Add("rate_internal_approved", cA.Rate_Internal_Approved, DbType.Double)
            .Add("p31Text", cA.p31Text, DbType.String)
            .Add("vatrate_approved", cA.VatRate_Approved, DbType.Double)
            .Add("dat_p31date", cA.p31Date, DbType.DateTime)
            .Add("approving_level", cA.p31ApprovingLevel, DbType.Int32)
            .Add("value_fixprice", cA.p31Value_FixPrice, DbType.Double)
            .Add("manualfee_approved", cA.ManualFee_Approved, DbType.Double)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        If cA.GUID_TempData <> "" Then
            'TEMP - dočasná data
            Return _cDB.RunSP("p31_save_approving_temp", pars)
        Else
            'uložení schvalování do ostrých dat
            Return _cDB.RunSP("p31_save_approving", pars)
        End If




    End Function
    Public Function SaveFreeFields(intP31ID As Integer, lisFF As List(Of BO.FreeField), bolIsTempRecord As Boolean, strP31Guid As String) As Boolean
        Dim strTab As String = "p31WorkSheet_FreeField"
        If bolIsTempRecord Then strTab = "p31WorkSheet_FreeField_Temp"
        Return bas.SaveFreeFields(_cDB, lisFF, strTab, intP31ID, _curUser, strP31Guid)

    End Function
    Public Function SaveFreeFields_Batch_AfterApproving(strP31Guid As String) As Boolean
        Dim pars As New DbParameters
        pars.Add("guid", strP31Guid, DbType.String)
        Return _cDB.RunSP("p31_save_freefields_after_approving", pars)
    End Function
    Public Function SaveOrigRecord(cRec As BO.p31WorksheetEntryInput, p33ID As BO.p33IdENUM, lisFF As List(Of BO.FreeField)) As Boolean
        Dim strX45IDs_Handle As String = "" 'aplikační údálosti, které se mají následně odchytávat

        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p31ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                If .PID = 0 Then
                    pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(_curUser.j02ID), DbType.Int32)
                Else
                    pars.Add("j02ID_Owner", .j02ID, DbType.Int32)
                End If
                pars.Add("p41ID", BO.BAS.IsNullDBKey(.p41ID), DbType.Int32)
                pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
                pars.Add("p56ID", BO.BAS.IsNullDBKey(.p56ID), DbType.Int32)
                pars.Add("p32ID", BO.BAS.IsNullDBKey(.p32ID), DbType.Int32)
                pars.Add("p72ID_AfterTrimming", BO.BAS.IsNullDBKey(.p72ID_AfterTrimming), DbType.Int32)
                pars.Add("p28ID_Supplier", BO.BAS.IsNullDBKey(.p28ID_Supplier), DbType.Int32)
                pars.Add("j02ID_ContactPerson", BO.BAS.IsNullDBKey(.j02ID_ContactPerson), DbType.Int32)

                pars.Add("p31Text", .p31Text, DbType.String, , , True, "Popis úkonu")
                pars.Add("p31HoursEntryflag", .p31HoursEntryflag, DbType.Int32)
                pars.Add("p31Date", .p31Date, DbType.DateTime)
                pars.Add("p31DateUntil", BO.BAS.IsNullDBDate(.p31DateUntil), DbType.DateTime)

                pars.Add("p31Value_Orig", .p31Value_Orig, DbType.Double)
                pars.Add("p31Value_Trimmed", .p31Value_Trimmed, DbType.Double)
                pars.Add("p31Minutes_Orig", .p31Minutes_Orig, DbType.Int32)
                pars.Add("p31Minutes_Trimmed", .p31Minutes_Trimmed, DbType.Int32)
                pars.Add("p31HHMM_Orig", .p31HHMM_Orig, DbType.String)
                pars.Add("p31Hours_Orig", .p31Hours_Orig, DbType.Double)
                pars.Add("p31Hours_Trimmed", .p31Hours_Trimmed, DbType.Double)
                pars.Add("p31DateTimeFrom_Orig", BO.BAS.IsNullDBDate(.p31DateTimeFrom_Orig), DbType.DateTime)
                pars.Add("p31DateTimeUntil_Orig", BO.BAS.IsNullDBDate(.p31DateTimeUntil_Orig), DbType.DateTime)
                pars.Add("p31Value_Orig_Entried", Left(.Value_Orig_Entried, 20), DbType.String)
                pars.Add("p31ExternalPID", .p31ExternalPID, DbType.String)

                pars.Add("p31Code", .p31Code, DbType.String)

                If p33ID = BO.p33IdENUM.PenizeBezDPH Or p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                    pars.Add("p31Amount_WithoutVat_Orig", .p31Amount_WithoutVat_Orig, DbType.Double)
                    pars.Add("p31VatRate_Orig", .VatRate_Orig, DbType.Double)
                    pars.Add("p31Amount_WithVat_Orig", .p31Amount_WithVat_Orig, DbType.Double)
                    pars.Add("p31Amount_Vat_Orig", .p31Amount_Vat_Orig, DbType.Double)
                    pars.Add("j27ID_Billing_Orig", BO.BAS.IsNullDBKey(.j27ID_Billing_Orig), DbType.Int32)
                    pars.Add("p31Calc_Pieces", .p31Calc_Pieces, DbType.Double)
                    pars.Add("p31Calc_PieceAmount", .p31Calc_PieceAmount, DbType.Double)
                    pars.Add("p35ID", BO.BAS.IsNullDBKey(.p35ID), DbType.Int32)
                    pars.Add("p49ID", BO.BAS.IsNullDBKey(.p49ID), DbType.Int32)
                    pars.Add("j19ID", BO.BAS.IsNullDBKey(.j19ID), DbType.Int32)
                End If
                If (p33ID = BO.p33IdENUM.Cas Or p33ID = BO.p33IdENUM.Kusovnik) And cRec.ManualFee <> 0 Then
                    'pevný honorář
                    pars.Add("p31Amount_WithoutVat_Orig", .ManualFee, DbType.Double)
                End If
            End With

            If _cDB.SaveRecord("p31Worksheet", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intSavedP31ID As Integer = _cDB.LastSavedRecordPID
                If Not lisFF Is Nothing Then    'volná pole
                    bas.SaveFreeFields(_cDB, lisFF, "p31Worksheet_FreeField", intSavedP31ID)
                End If

                pars = New DbParameters
                With pars
                    .Add("p31id", intSavedP31ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                    .Add("p48id", cRec.p48ID, DbType.Int32)
                    .Add("x45ids", , DbType.String, ParameterDirection.Output, 50)
                End With
                If _cDB.RunSP("p31_aftersave", pars) Then
                    sc.Complete()
                    strX45IDs_Handle = pars.Get(Of String)("x45ids")
                Else
                    Return False
                End If

            Else
                Return False
            End If
        End Using
        'události je nutné zpracovávat až mimo scope transakci
        If strX45IDs_Handle <> "" Then
            RaiseEvent NeedHandleAppEvents(strX45IDs_Handle, cRec.p41ID)
        End If
        Return True
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p31_delete", pars)
    End Function
    Public Function Get_p72ID_NonBillableWork(intP31ID As Integer) As BO.p72IdENUM
        Dim pars As New DbParameters()
        With pars
            .Add("p31id", intP31ID, DbType.Int32)
            .Add("ret_p72id", , DbType.Int32, ParameterDirection.Output)
        End With
        If _cDB.RunSP("p31_inhale_p72id_nonbillable", pars) Then
            Return pars.Get(Of Int32)("ret_p72id")
        Else
            Return BO.p72IdENUM.SkrytyOdpis
        End If
    End Function


    Private Function GetSQLWHERE(myQuery As BO.myQueryP31, ByRef pars As DL.DbParameters, Optional strGUID_TempData As String = "") As String
        Dim s As New System.Text.StringBuilder
        
        s.Append(bas.ParseWhereMultiPIDs("a.p31ID", myQuery))

        With myQuery
            If strGUID_TempData <> "" Then
                pars.Add("guid", strGUID_TempData, DbType.String) : s.Append(" AND a.p31GUID=@guid")
            End If
            If .p41ID <> 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                If Not .IncludeChildProjects Then
                    s.Append(" AND a.p41ID=@p41id")
                Else
                    s.Append(" AND a.p41ID IN (select p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID=@p41id) AND (select p41TreeNext FROM p41Project WHERE p41ID=@p41id))")
                End If
            End If
            If Not .p41IDs Is Nothing Then
                If .p41IDs.Count > 0 Then
                    s.Append(" AND a.p41ID IN (" & String.Join(",", .p41IDs) & ")")
                End If
            End If
            If .j02ID <> 0 Then
                pars.Add("j02id", .j02ID, DbType.Int32) : s.Append(" AND a.j02ID=@j02id")
            End If
            If Not .j02IDs Is Nothing Then
                If .j02IDs.Count > 0 Then
                    s.Append(" AND a.j02ID IN (" & String.Join(",", .j02IDs) & ")")
                End If
            End If
            If .p34ID <> 0 Then
                pars.Add("p34id", .p34ID, DbType.Int32) : s.Append(" AND p32.p34ID=@p34id")
            End If
            If Not .p34IDs Is Nothing Then
                s.Append(" AND p32.p34ID IN (" & String.Join(",", .p34IDs) & ")")
            End If
            If .p32ID <> 0 Then
                pars.Add("p32id", .p32ID, DbType.Int32) : s.Append(" AND a.p32ID=@p32id")
            End If
            If .j27ID_Billing_Orig > 0 Then
                pars.Add("j27id_billing_orig", .j27ID_Billing_Orig, DbType.Int32) : s.Append(" AND a.j27ID_Billing_Orig=@j27id_billing_orig")
            End If
            If Not .p33IDs Is Nothing Then
                s.Append(" AND p34.p33ID IN (" & String.Join(",", .p33IDs) & ")")
            End If
            
            If Not .p56IDs Is Nothing Then
                Select Case .p56IDs.Count
                    Case 1
                        pars.Add("p56id", .p56IDs(0), DbType.Int32) : s.Append(" AND a.p56ID=@p56id")
                    Case Is > 1
                        s.Append(" AND a.p56ID IN (" & String.Join(",", .p56IDs) & ")")
                End Select
            End If

            If .p28ID_Client <> 0 Then
                pars.Add("p28id", .p28ID_Client, DbType.Int32) : s.Append(" AND p41.p28ID_Client=@p28id")
            End If
            If Not .p28IDs_Client Is Nothing Then
                s.Append(" AND p41.p28ID_Client IN (" & String.Join(",", .p28IDs_Client) & ")")
            End If
            If Not .p28ID_Supplier Is Nothing Then
                If .p28ID_Supplier > 0 Then
                    pars.Add("p28id_supplier", .p28ID_Supplier, DbType.Int32)
                    s.Append(" AND a.p28ID_Supplier=@p28id_supplier")
                Else
                    s.Append(" AND a.p28ID_Supplier IS NULL")
                End If
            End If
            If Not .p31ApprovingLevel Is Nothing Then
                pars.Add("approvinglevel", .p31ApprovingLevel, DbType.Int32)
                s.Append(" AND a.p31ApprovingLevel=@approvinglevel")
            End If
            If .p91ID <> 0 Then
                pars.Add("p91id", .p91ID, DbType.Int32) : s.Append(" AND a.p91ID=@p91id")
            End If
            If Not .p91IDs Is Nothing Then
                s.Append(" AND a.p91ID IN (" & String.Join(",", .p91IDs) & ")")
            End If
            If .o23ID <> 0 Then
                pars.Add("o23id", .o23ID, DbType.Int32) : s.Append(" AND a.p31ID IN (select za.x19RecordPID FROM x19EntityCategory_Binding za INNER JOIN x20EntiyToCategory zb ON za.x20ID=zb.x20ID WHERE zb.x29ID=331 AND za.o23ID=@o23id)")
            End If
            If Not .p70ID Is Nothing Then
                If .p70ID > 0 Then
                    pars.Add("p70id", .p70ID, DbType.Int32) : s.Append(" AND a.p70ID=@p70id")
                Else
                    s.Append(" AND a.p70ID IS NULL")
                End If

            End If
            If Not .p71ID Is Nothing Then
                If .p71ID > 0 Then
                    pars.Add("p71id", .p71ID, DbType.Int32) : s.Append(" AND a.p71ID=@p71id")
                Else
                    s.Append(" AND a.p71ID IS NULL")
                End If
            End If
            If .DateFrom > DateSerial(1900, 1, 1) Or .DateUntil < DateSerial(3000, 1, 1) Then
                pars.Add("datefrom", .DateFrom, DbType.DateTime)
                pars.Add("dateuntil", .DateUntil, DbType.DateTime)
                Select Case .PeriodType
                    Case BO.myQueryP31_Period.p31Date
                        s.Append(" AND a.p31Date BETWEEN @datefrom AND @dateuntil")
                    Case BO.myQueryP31_Period.p31DateInsert
                        s.Append(" AND a.p31DateInsert >= @datefrom AND a.p31DateInsert < dateadd(day,1,@dateuntil)")
                    Case BO.myQueryP31_Period.p91Date
                        s.Append(" AND p91.p91Date BETWEEN @datefrom AND @dateuntil")
                    Case BO.myQueryP31_Period.p91DateSupply
                        s.Append(" AND p91.p91DateSupply BETWEEN @datefrom AND @dateuntil")
                End Select
            End If
            
            
       
            If .j70ID > 0 Then
                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    s.Append(" AND " & strQueryW)
                End If
            End If
            If .p49ID > 0 Then
                pars.Add("p49id", .p49ID, DbType.Int32) : s.Append(" AND a.p49ID=@p49id")
            End If
            If .QuickQuery > BO.myQueryP31_QuickQuery._NotSpecified Then
                s.Append(" AND " & bas.GetQuickQuerySQL_p31(.QuickQuery))
            End If
            If .SpecificQuery > BO.myQueryP31_SpecificQuery._NotSpecified Then
                If .j02ID_ExplicitQueryFor > 0 Then
                    pars.Add("j02id_query", .j02ID_ExplicitQueryFor, DbType.Int32)
                Else
                    pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
                End If
            End If
            Select Case .SpecificQuery
                Case BO.myQueryP31_SpecificQuery.AllowedForDoApprove, BO.myQueryP31_SpecificQuery.AllowedForReApprove
                    ''s.Append(" AND getdate() BETWEEN p41.p41ValidFrom AND p41.p41ValidUntil")   'vyloučit projekty z archivu
                    If .SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForDoApprove Then
                        If .QuickQuery <> BO.myQueryP31_QuickQuery.Editing Then
                            s.Append(" AND a.p71ID IS NULL AND a.p91ID IS NULL AND GETDATE() BETWEEN a.p31ValidFrom AND a.p31ValidUntil")
                        End If
                    Else
                        s.Append(" AND a.p71ID=1 AND a.p91ID IS NULL AND GETDATE() BETWEEN a.p31ValidFrom AND a.p31ValidUntil")    'přeschválit již dříve schválený worksheet
                    End If
                    If .QuickQuery = BO.myQueryP31_QuickQuery._NotSpecified Then
                        s.Append(" AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil")   'nabízet pouze platné záznamy, které nejsou v archivu
                    End If

                    If Not BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Approver) Then
                        'pokud nemá právo paušálně schvalovat všechny záznamy v db
                        ''Dim strJ11IDs As String = ""
                        ''If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"

                        s.Append(" AND (scope.p41ID IS NOT NULL")

                        ''s.Append(" a.p31ID IN (")
                        ''s.Append("SELECT p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag IN (3,4) AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID")
                        ''s.Append(")")

                        If _curUser.IsMasterPerson And _curUser.j02ID = .j02ID_ExplicitQueryFor Then
                            'oprávnění z titulu nadřazené osoby
                            s.Append(" OR a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j05Disposition_p31>=3 AND j02ID_Master=@j02id_query)")
                            s.Append(" OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j05Disposition_p31>=3 AND xj05.j02ID_Master=@j02id_query)")
                        End If

                        s.Append(")")
                    End If
                Case BO.myQueryP31_SpecificQuery.AllowedForRead
                    If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Owner)) Then
                        'poku dnemá právo paušálně vidět nebo vlastnit veškerý worksheet
                        s.Append(" AND (")

                        ''s.Append(" a.p31ID IN (")
                        '' ''s.Append(" EXISTS (")

                        ''Dim strJ11IDs As String = ""
                        ''If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"

                        ''s.Append("SELECT p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag>0 AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID")
                        s.Append("scope.p41ID IS NOT NULL OR a.j02ID_Owner=@j02id_query OR a.j02ID=@j02id_query")

                        If _curUser.IsMasterPerson And _curUser.j02ID = .j02ID_ExplicitQueryFor Then
                            'oprávnění z titulu nadřazené osoby
                            s.Append(" OR a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_query)")
                            s.Append(" OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_query)")
                        End If

                        s.Append(")")
                    End If
                Case BO.myQueryP31_SpecificQuery.AllowedForCreateInvoice
                    s.Append(" AND a.p71ID=1 AND a.p72ID_AfterApprove NOT IN (7) AND a.p91ID IS NULL AND GETDATE() BETWEEN a.p31ValidFrom AND a.p31ValidUntil")    'zahrnout do faktury schvalené, ale dosud nefakturované úkony
                    
            End Select
            If .ColumnFilteringExpression <> "" Then
                s.Append(" AND " & .ColumnFilteringExpression)
            End If
            If .SearchExpression <> "" Then
                s.Append(" AND (p28Client.p28Name LIKE '%'+@expr+'%' OR p41.p41Code LIKE @expr+'%' OR p41.p41Name LIKE '%'+@expr+'%' OR a.p31Text LIKE '%'+@expr+'%' OR p32.p32Name like '%'+@expr+'%' OR p28Client.p28CompanyName LIKE '%'+@expr+'%'")
                s.Append(" OR j02.j02LastName LIKE '%'+@expr+'%')")
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If Not String.IsNullOrEmpty(.MG_AdditionalSqlWHERE) Then
                s.Append(" AND " & .MG_AdditionalSqlWHERE)
            End If
            If Not String.IsNullOrEmpty(.TabAutoQuery) Then
                Select Case .TabAutoQuery
                    Case "time"
                        s.Append(" AND p34.p33ID=1")
                    Case "expense"
                        s.Append(" AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1")
                    Case "fee"
                        s.Append(" AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2")
                    Case "kusovnik"
                        s.Append(" AND p34.p33ID=3")
                End Select
            End If
            If _curUser.j02WorksheetAccessFlag = 1 Then s.Append(" AND a.p91ID IS NULL") 'absolutně bez práva vidět vyfakturované úkony
            If .x18Value <> "" Then
                s.Append(bas.CompleteX18QuerySql("p31", .x18Value)) 'filtr podle štítků
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then s.Append(" AND a.p31ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=331 AND o51ID IN (" & String.Join(",", .o51IDs) & "))")
            End If
        End With

        Return bas.TrimWHERE(s.ToString)
    End Function

    Public Function GetList_CalendarHours(intJ02ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.p31WorksheetCalendarHours)
        Dim pars As New DbParameters
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)
        pars.Add("j02id", intJ02ID, DbType.Int32)

        Dim s As String = "SELECT a.p31Date,sum(a.p31Hours_Orig) as Hours,count(case when p34.p33id in (2,5) then 1 end) as Moneys,count(case when p34.p33id=3 then 1 end) as Pieces FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID WHERE a.j02ID=@j02id AND a.p31Date BETWEEN @d1 AND @d2 GROUP BY a.p31Date"
        Return _cDB.GetList(Of BO.p31WorksheetCalendarHours)(s, pars)
    End Function


    Public Function GetList(myQuery As BO.myQueryP31, Optional strGUID_TempData As String = "") As IEnumerable(Of BO.p31Worksheet)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly)
        If myQuery.MG_SelectPidFieldOnly Then
            'SQL SELECT klauzule bude plnit pouze hodnotu primárního klíče
            s = "SELECT a.p31ID as _pid"
        End If
        s += " " & GetSQLPart2(strGUID_TempData, myQuery)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars, strGUID_TempData)
        With myQuery
            Dim strSort As String = .MG_SortString
            If strSort = "" Then
                strSort = "a.p31ID DESC"
            End If

            If .MG_PageSize > 0 Then
                'použít stránkování do gridu    
                s = GetSQL_OFFSET(strW, ParseSortExpression(strSort), .MG_PageSize, .MG_CurrentPageIndex, pars, myQuery)
            Else
                'normální select
                If strW <> "" Then s += " WHERE " & strW
                If strSort <> "" Then
                    s += " ORDER BY " & ParseSortExpression(strSort)
                End If
            End If
        End With
        Return _cDB.GetList(Of BO.p31Worksheet)(s, pars)
    End Function

    Public Function GetGridDataSource(myQuery As BO.myQueryP31, Optional strTempGUID As String = "") As DataTable
        Dim s As String = ""
        With myQuery
            If Not System.String.IsNullOrEmpty(.MG_GridGroupByField) Then
                If .MG_GridSqlColumns.ToLower.IndexOf(.MG_GridGroupByField.ToLower) < 0 Then
                    Select Case .MG_GridGroupByField
                        Case "SupplierName" : .MG_GridSqlColumns += ",supplier.p28Name as SupplierName"
                        Case "Owner" : .MG_GridSqlColumns += ",j02owner.j02LastName+char(32)+j02owner.j02FirstName as Owner"
                        Case "Person" : .MG_GridSqlColumns += ",j02.j02LastName+char(32)+j02.j02Firstname as Person"
                        Case "ClientName" : .MG_GridSqlColumns += ",p28client.p28Name as ClientName"
                        Case "j27Code_Billing_Orig" : .MG_GridSqlColumns += ",j27billing_orig.j27Code as j27Code_Billing_Orig"
                        Case "approve_p72Name" : .MG_GridSqlColumns += ",p72approve.p72Name as approve_p72Name"
                        Case Else
                            .MG_GridSqlColumns += "," & .MG_GridGroupByField
                    End Select
                End If
            End If
            .MG_GridSqlColumns += ",a.p31ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.p31ValidFrom AND a.p31ValidUntil THEN 0 else 1 END) as IsClosed,a.p72ID_AfterTrimming,a.p72ID_AfterApprove,a.p70ID,a.o23ID_First,a.p49ID,a.p71ID,p34.p33ID"
            .MG_GridSqlColumns += ",a.p31Date as p31Date_Grid,a.p31Hours_Trimmed as p31Hours_Trimmed_Grid,a.p31Hours_Orig as p31Hours_Orig_Grid,p34.p34IncomeStatementFlag"

        End With
        
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars, strTempGUID)
        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.p31ID as pid"
            Dim strORDERBY As String = .MG_SortString
            If .MG_GridGroupByField <> "" Then
                Dim strPrimarySortField As String = .MG_GridGroupByField
                If strPrimarySortField = "SupplierName" Then strPrimarySortField = "supplier.p28Name"
                If strPrimarySortField = "ClientName" Then strPrimarySortField = "p28client.p28Name"
                If strPrimarySortField = "Person" Then strPrimarySortField = "j02.j02LastName+char(32)+j02.j02Firstname"
                If strPrimarySortField = "approve_p72Name" Then strPrimarySortField = "p72approve.p72Name"
                If strORDERBY = "" Or LCase(strPrimarySortField) = Replace(Replace(LCase(.MG_SortString), " desc", ""), " asc", "") Then
                    strORDERBY = strPrimarySortField
                Else
                    strORDERBY = strPrimarySortField & "," & .MG_SortString
                End If
            End If
            If strORDERBY = "" Then strORDERBY = "a.p31ID DESC"

            If .MG_PageSize > 0 Then
                Dim intStart As Integer = (.MG_CurrentPageIndex) * .MG_PageSize

                s = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & .MG_GridSqlColumns & " " & GetSQLPart2(strTempGUID, myQuery)

                If strW <> "" Then s += " WHERE " & strW
                ''s += ") SELECT TOP " & .MG_PageSize.ToString & " * FROM rst"
                s += ") SELECT * FROM rst"
                pars.Add("start", intStart, DbType.Int32)
                pars.Add("end", (intStart + .MG_PageSize - 1), DbType.Int32)
                s += " WHERE RowIndex BETWEEN @start AND @end"
            Else
                'bez stránkování
                s = "SELECT " & .MG_GridSqlColumns & " " & GetSQLPart2(strTempGUID, myQuery)
                If strW <> "" Then s += " WHERE " & strW
                s += " ORDER BY " & strORDERBY
            End If

        End With

        Dim ds As DataSet = _cDB.GetDataSet(s, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function

    Private Function GetSQL_OFFSET(strWHERE As String, strORDERBY As String, intPageSize As Integer, intCurrentPageIndex As Integer, ByRef pars As DL.DbParameters, myQuery As BO.myQueryP31) As String
        Dim intStart As Integer = (intCurrentPageIndex) * intPageSize

        Dim s As String = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & GetSF() & " " & GetSQLPart2(, myQuery)

        If strWHERE <> "" Then s += " WHERE " & strWHERE
        s += ") SELECT TOP " & intPageSize.ToString & " * FROM rst"
        pars.Add("start", intStart, DbType.Int32)
        pars.Add("end", (intStart + intPageSize - 1), DbType.Int32)
        s += " WHERE RowIndex BETWEEN @start AND @end"
        's += " ORDER BY " & strORDERBY
        Return s
    End Function
    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "p31UserInsert").Replace("UserUpdate", "p31UserUpdate").Replace("DateInsert", "p31DateInsert").Replace("DateUpdate", "p31DateUpdate").Replace("ContactPerson", "cp.j02LastName")
        strSort = strSort.Replace("j27Code_Billing_Orig", "j27billing_orig.j27Code").Replace("Owner", "j02owner.j02LastName").Replace("Person", "j02.j02LastName").Replace("ClientName", "p28Client.p28Name").Replace("SupplierName", "supplier.p28Name")
        Return bas.NormalizeOrderByClause(strSort)
    End Function

    Private Function ParseFilterExpression(strFilter As String) As String
        strFilter = strFilter.Replace("ContactPerson", "cp.j02LastName+cp.j02FirstName").Replace("Person", "j02.j02LastName+j02.j02FirstName")

        Return ParseSortExpression(strFilter).Replace("[", "").Replace("]", "")
    End Function

    Public Function GetVirtualCount(myQuery As BO.myQueryP31, Optional strGUID_TempData As String = "") As Integer
        Dim s As String = "SELECT count(a.p31ID) as Value " & GetSQLPart2(strGUID_TempData, myQuery)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars, strGUID_TempData)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP31, strSumFields As String, Optional strGUID_TempData As String = "") As DataTable
        Dim s As String = "SELECT count(a.p31ID) as VirtualCount"
        Dim pars As New DL.DbParameters
        If strSumFields <> "" Then
            For Each strField As String In Split(strSumFields, "|")
                s += "," & strField
            Next
        End If
        s += " " & GetSQLPart2(strGUID_TempData, myQuery)
        Dim strW As String = GetSQLWHERE(myQuery, pars, strGUID_TempData)
        If strW <> "" Then s += " WHERE " & strW
        Dim ds As DataSet = _cDB.GetDataSet(s, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function
    Public Function LoadSumRow(myQuery As BO.myQueryP31, bolIncludeWaiting4Approval As Boolean, bolIncludeWaiting4Invoice As Boolean, Optional strGUID_TempData As String = "") As BO.p31WorksheetSum
        Dim s As String = "SELECT " & GetSF_SUM(bolIncludeWaiting4Approval, bolIncludeWaiting4Invoice) & " " & GetSQLPart2(strGUID_TempData, myQuery)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars, strGUID_TempData)
        If strW <> "" Then s += " WHERE " & strW


        Return _cDB.GetRecord(Of BO.p31WorksheetSum)(s, pars)
    End Function

    ''Public Function GetDrillDownDataTable(colDrill As BO.GridGroupByColumn, myQuery As BO.myQueryP31, strSumFieldsList As String) As DataTable
    ''    Dim s As String = "select " & colDrill.FieldSqlGroupBy & " as pid," & colDrill.AggregateSQL & " as " & colDrill.ColumnField
    ''    s += ",count(*) as RowsCount"
    ''    If strSumFieldsList <> "" Then
    ''        Dim a() As String = Split(strSumFieldsList, "|")
    ''        For Each strF In a
    ''            s += ",sum(" & strF & ") as " & strF
    ''        Next
    ''    End If

    ''    s += " " & GetSQLPart2(, myQuery)

    ''    Dim pars As New DbParameters
    ''    Dim strW As String = GetSQLWHERE(myQuery, pars)
    ''    Dim prs As List(Of BO.PluginDbParameter) = pars.Convert2PluginDbParameters()
    ''    If strW <> "" Then s += " WHERE " & strW
    ''    s += " GROUP BY " & colDrill.FieldSqlGroupBy
    ''    s += " ORDER BY " & colDrill.AggregateSQL

    ''    Return _cDB.GetDataSet(s, , prs).Tables(0)


    ''End Function
    

    Private Function GetSQLPart1(intTOP As Integer) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " " & GetSF()
        Return s
    End Function
    Private Function GetSF() As String
        Dim s As New System.Text.StringBuilder()
        s.Append("a.p41ID,a.j02ID,a.p32ID,a.p56ID,a.j02ID_Owner,a.j02ID_ApprovedBy,a.p31Code,a.p70ID,a.p71ID,a.p72ID_AfterApprove,a.p72ID_AfterTrimming,a.j27ID_Billing_Orig,a.j27ID_Billing_Invoiced,a.j27ID_Billing_Invoiced_Domestic,a.j27ID_Internal,a.p91ID,a.c11ID,a.p31Date,a.p31DateUntil,a.p31HoursEntryFlag,a.p31Approved_When,a.p31IsPlanRecord,a.p31Text,a.p31Value_Orig")
        s.Append(",a.p31Value_Trimmed,a.p31Value_Approved_Billing,a.p31Value_Approved_Internal,a.p31Value_Invoiced,a.p31Amount_WithoutVat_Orig,a.p31Amount_WithVat_Orig,a.p31Amount_Vat_Orig,a.p31VatRate_Orig,a.p31Amount_WithoutVat_FixedCurrency,a.p31Amount_WithoutVat_Invoiced,a.p31Amount_WithVat_Invoiced,a.p31Amount_Vat_Invoiced,a.p31VatRate_Invoiced,a.p31Amount_WithoutVat_Invoiced_Domestic,a.p31Amount_WithVat_Invoiced_Domestic,a.p31Amount_Vat_Invoiced_Domestic,a.p31Minutes_Orig,a.p31Minutes_Trimmed,a.p31Minutes_Approved_Billing,a.p31Minutes_Approved_Internal,a.p31Minutes_Invoiced")
        s.Append(",a.p31Hours_Orig,a.p31Hours_Trimmed,a.p31Hours_Approved_Billing,a.p31Hours_Approved_Internal,a.p31Hours_Invoiced,a.p31HHMM_Orig,a.p31HHMM_Trimmed,a.p31HHMM_Approved_Billing,a.p31HHMM_Approved_Internal,a.p31HHMM_Invoiced,a.p31Rate_Billing_Orig,a.p31Rate_Internal_Orig,a.p31Rate_Billing_Approved,a.p31Rate_Internal_Approved,a.p31Rate_Billing_Invoiced,a.p31Amount_WithoutVat_Approved,a.p31Amount_WithVat_Approved,a.p31Amount_Vat_Approved,a.p31VatRate_Approved,a.p31ExchangeRate_Domestic,a.p31ExchangeRate_Invoice,a.p31Amount_Internal")
        s.Append(",a.p31DateTimeFrom_Orig,a.p31DateTimeUntil_Orig,a.p31Value_Orig_Entried,a.p31Calc_Pieces,a.p31Calc_PieceAmount,a.p35ID,a.j19ID")
        s.Append(",j02.j02LastName+' '+j02.j02FirstName as Person,p32.p32Name,p32.p34ID,p32.p32IsBillable,p32.p32ManualFeeFlag,p34.p33ID,p34.p34Name,p34.p34IncomeStatementFlag,isnull(p41.p41NameShort,p41.p41Name) as p41Name,p41.p28ID_Client,p28Client.p28Name as ClientName,p28Client.p28CompanyShortName,p56.p56Name,p56.p56Code,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner")
        s.Append(",p91.p91Code,p70.p70Name,p71.p71Name,p72trim.p72Name as trim_p72Name,p72approve.p72Name as approve_p72Name,j27billing_orig.j27Code as j27Code_Billing_Orig,p32.p95ID,p95.p95Name,a.p31ApprovingSet,a.p31ApprovingLevel,a.p31Value_FixPrice,a.o23ID_First,a.p28ID_Supplier,supplier.p28Name as SupplierName,a.p49ID,a.j02ID_ContactPerson,cp.j02LastName+' '+cp.j02FirstName as ContactPerson,a.p31IsInvoiceManual," & bas.RecTail("p31", "a"))
        Return s.ToString
    End Function
    Private Function GetSF_SUM(bolIncludeWaiting4Approval As Boolean, bolIncludeWaiting4Invoice As Boolean) As String
        Dim s As New System.Text.StringBuilder()
        s.Append("COUNT(a.p31ID) as RowsCount,SUM(p31Value_Orig) as p31Value_Orig,SUM(p31Value_Trimmed) as p31Value_Trimmed,SUM(p31Value_Approved_Billing) as p31Value_Approved_Billing,SUM(p31Value_Approved_Internal) as p31Value_Approved_Internal")
        s.Append(",SUM(p31Value_Invoiced) as p31Value_Invoiced,SUM(p31Amount_WithoutVat_Orig) as p31Amount_WithoutVat_Orig")
        s.Append(",SUM(p31Hours_Orig) as p31Hours_Orig,SUM(p31Hours_Approved_Billing) as p31Hours_Approved_Billing,SUM(p31Hours_Approved_Internal) as p31Hours_Approved_Internal")
        s.Append(",SUM(p31Amount_WithoutVat_Invoiced) as p31Amount_WithoutVat_Invoiced,SUM(p31Amount_WithVat_Invoiced) as p31Amount_WithVat_Invoiced,SUM(p31Amount_Vat_Invoiced) as p31Amount_Vat_Invoiced,SUM(p31Hours_Invoiced) as p31Hours_Invoiced")
        s.Append(",SUM(p31Amount_WithoutVat_Approved) as p31Amount_WithoutVat_Approved,SUM(p31Amount_WithVat_Approved) as p31Amount_WithVat_Approved,SUM(p31Amount_Vat_Approved) as p31Amount_Vat_Approved")
        s.Append(",SUM(case when p32.p32IsBillable=1 THEN p31Hours_Orig END) as Hours_Orig_Billable")
        s.Append(",SUM(p31Amount_WithoutVat_Invoiced_Domestic) as p31Amount_WithoutVat_Invoiced_Domestic,SUM(p31Amount_WithVat_Invoiced_Domestic) as p31Amount_WithVat_Invoiced_Domestic")
        s.Append(",MAX(a.p91ID) as Last_p91ID")

        If bolIncludeWaiting4Approval Then
            Dim strInW As String = "a.p71ID IS NULL AND a.P91ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil"
            s.Append(",SUM(case when " & strInW & " THEN a.p31Hours_Orig end) as WaitingOnApproval_Hours_Sum,SUM(case when p34.p33ID=1 AND " & strInW & " THEN 1 end) as WaitingOnApproval_Hours_Count")
            s.Append(",SUM(case when p34.p33ID=1 AND " & strInW & " THEN a.p31Amount_WithoutVat_Orig end) as WaitingOnApproval_HoursFee")
            s.Append(",SUM(case when p34.p33ID>1 AND " & strInW & " THEN a.p31Value_Orig end) as WaitingOnApproval_Other_Sum,SUM(case when p34.p33ID>1 AND " & strInW & " THEN 1 end) as WaitingOnApproval_Other_Count")
        End If
        If bolIncludeWaiting4Invoice Then
            s.Append(",SUM(case when a.p71ID=1 AND a.P91ID IS NULL and p34.p33ID=1 THEN a.p31Hours_Approved_Billing end) as WaitingOnInvoice_Hours_Sum,SUM(case when p34.p33ID=1 AND a.p71ID=1 AND a.P91ID IS NULL THEN 1 end) as WaitingOnInvoice_Hours_Count")
            s.Append(",SUM(case when p34.p33ID>1 AND a.p71ID=1 AND a.P91ID IS NULL THEN a.p31Value_Approved_Billing end) as WaitingOnInvoice_Other_Sum,SUM(case when p34.p33ID>1 AND a.p71ID=1 AND a.P91ID IS NULL THEN 1 end) as WaitingOnInvoice_Other_Count")
        End If
        Return s.ToString
    End Function
    Private Function GetSQLPart2(Optional strGUID_TempData As String = "", Optional myQuery As BO.myQueryP31 = Nothing) As String
        Dim s As New System.Text.StringBuilder()

        If strGUID_TempData <> "" Then
            s.Append("FROM p31Worksheet_Temp a")
        Else
            s.Append("FROM p31Worksheet a")
        End If
        s.Append(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID")
        s.Append(" INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID")
        s.Append(" LEFT OUTER JOIN p28Contact p28Client ON p41.p28ID_Client=p28Client.p28ID")
        s.Append(" LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID LEFT OUTER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID")
        s.Append(" LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID LEFT OUTER JOIN p71ApproveStatus p71 ON a.p71ID=p71.p71ID LEFT OUTER JOIN p72PreBillingStatus p72trim ON a.p72ID_AfterTrimming=p72trim.p72ID LEFT OUTER JOIN p72PreBillingStatus p72approve ON a.p72ID_AfterApprove=p72approve.p72ID")
        s.Append(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID LEFT OUTER JOIN j02Person cp ON a.j02ID_ContactPerson=cp.j02ID LEFT OUTER JOIN p28Contact supplier ON a.p28ID_Supplier=supplier.p28ID")
        s.Append(" LEFT OUTER JOIN j27Currency j27billing_orig ON a.j27ID_Billing_Orig=j27billing_orig.j27ID")
        s.Append(" LEFT OUTER JOIN p95InvoiceRow p95 ON p32.p95ID=p95.p95ID")
        If strGUID_TempData = "" Then
            s.Append(" LEFT OUTER JOIN p31WorkSheet_FreeField p31free ON a.p31ID=p31free.p31ID")
        Else
            s.Append(" LEFT OUTER JOIN (select * from p31WorkSheet_FreeField_Temp WHERE p31GUID='" & strGUID_TempData & "') p31free ON a.p31ID=p31free.p31ID")
        End If

        If Not myQuery Is Nothing Then
            With myQuery
                If .MG_AdditionalSqlFROM <> "" Then s.Append(" " & .MG_AdditionalSqlFROM)
                Select Case .SpecificQuery
                    Case BO.myQueryP31_SpecificQuery.AllowedForDoApprove, BO.myQueryP31_SpecificQuery.AllowedForReApprove
                        If Not BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Approver) Then
                            'pokud nemá právo paušálně schvalovat všechny záznamy v db
                            Dim strJ11IDs As String = ""
                            If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"

                            ''s.Append(" LEFT OUTER JOIN (")
                            ''s.Append("SELECT DISTINCT p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag IN (3,4) AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID")
                            ''s.Append(") zbytek ON a.p31ID=zbytek.p31ID")
                            AppendSqlFrom_ProjectRoles(s, "zo28.o28PermFlag IN (3,4)")

                        End If
                    Case BO.myQueryP31_SpecificQuery.AllowedForRead
                        If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Owner)) Then
                            ''Dim strJ11IDs As String = ""
                            ''If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"
                            AppendSqlFrom_ProjectRoles(s, "zo28.o28PermFlag>0")
                            ''s.Append(" LEFT OUTER JOIN (")
                            ''s.Append("SELECT distinct p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag>0 AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID")
                            ''s.Append(") zbytek ON a.p31ID=zbytek.p31ID")
                        End If

                    Case Else
                End Select
            End With

        End If
        Return s.ToString
    End Function

    Private Sub AppendSqlFrom_ProjectRoles(ByRef s As Text.StringBuilder, strPermFlagSqlWhere As String)
        s.Append(" LEFT OUTER JOIN")
        s.Append(" (")
        s.Append(" select za.p41ID,zo28.p34ID")
        s.Append(" from p41Project za INNER JOIN x69EntityRole_Assign zx69 ON za.p41ID=zx69.x69RecordPID")
        s.Append(" INNER JOIN x67EntityRole zx67 ON zx69.x67ID=zx67.x67ID")
        s.Append(" INNER JOIN o28ProjectRole_Workload zo28 ON zx67.x67ID=zo28.x67ID")
        s.Append(" WHERE zx67.x29ID=141 AND " & strPermFlagSqlWhere & " AND (zx69.j02ID=@j02id_query")
        If _curUser.j11IDs <> "" Then s.Append(" OR zx69.j11ID IN (" & _curUser.j11IDs & ")")

        If 1 = 1 Then   'někdy se bude muset řešit kvůli výkonu
            s.Append(")")
            s.Append(" UNION select za.p41ID,zo28.p34ID")
            s.Append(" FROM p41Project za INNER JOIN j18Region zj18 ON za.j18ID=zj18.j18ID")
            s.Append(" INNER JOIN x69EntityRole_Assign zx69 ON zj18.j18ID=zx69.x69RecordPID")
            s.Append(" INNER JOIN x67EntityRole zx67 ON zx69.x67ID=zx67.x67ID INNER JOIN o28ProjectRole_Workload zo28 ON zx67.x67ID=zo28.x67ID")
            s.Append(" WHERE zx67.x29ID=118 AND " & strPermFlagSqlWhere & " AND (zx69.j02ID=@j02id_query")
            If _curUser.j11IDs <> "" Then s.Append(" OR zx69.j11ID IN (" & _curUser.j11IDs & ")")

        End If
        s.Append(")")
        s.Append(") scope ON a.p41ID=scope.p41ID AND p32.p34ID=scope.p34ID")
    End Sub



    Public Function InhaleRecordDisposition(intPID As Integer) As BO.p31WorksheetDisposition
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("record_disposition", , DbType.Int32, ParameterDirection.Output)
            .Add("record_state", , DbType.Int32, ParameterDirection.Output)
            .Add("msg_locked", , DbType.String, ParameterDirection.Output, 1000)
        End With
        Dim c As New BO.p31WorksheetDisposition(intPID)
        If _cDB.RunSP("p31_inhale_disposition", pars) Then
            c.RecordDisposition = pars.Get(Of Int32)("record_disposition")
            c.RecordState = pars.Get(Of Int32)("record_state")
            c.LockedReasonMessage = pars.Get(Of String)("msg_locked")
        End If
        Return c
    End Function

    Public Function GetList_Pivot(rows As List(Of BO.GridColumn), cols As List(Of BO.PivotRowColumnField), sums As List(Of BO.PivotSumField), mq As BO.myQueryP31) As IEnumerable(Of BO.PivotRecord)
        Dim s As New System.Text.StringBuilder, x As Integer = 0
        Dim lisSqlFROM As New List(Of String)
        s.Append("SELECT ")

        For Each c In rows
            If x > 0 Then s.Append(",")
            s.Append(c.Pivot_SelectSql & " AS Row" & (x + 1).ToString)
            If c.SqlSyntax_FROM <> "" Then lisSqlFROM.Add(c.SqlSyntax_FROM)
            x += 1
        Next
        Dim y As Integer = 0
        For Each c In cols
            If x > 0 Then s.Append(",")
            s.Append(c.SelectField & " AS Col" & (y + 1).ToString)
            x += 1 : y += 1
        Next
        x = 0
        For Each c In sums
            s.Append(",")
            s.Append(c.SelectField & " AS Sum" & (x + 1).ToString)
            x += 1
        Next

        If lisSqlFROM.Count > 0 Then mq.MG_AdditionalSqlFROM = String.Join(" ", lisSqlFROM.Distinct)


        s.Append(" " & GetSQLPart2("", mq))
        ''AppendSqlFROM_Pivot_Or_Drilldown(s)


        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(mq, pars)
        If strW <> "" Then
            s.Append(" WHERE " & strW)
        End If

        x = 0
        s.Append(" GROUP BY ")
        For Each c In rows
            If x > 0 Then s.Append(",")
            s.Append(c.Pivot_GroupBySql)
            x += 1
        Next
        For Each c In cols
            If x > 0 Then s.Append(",")
            s.Append(c.GroupByField)
            x += 1
        Next

        Return _cDB.GetList(Of BO.PivotRecord)(s.ToString, pars)
    End Function
    
    Public Function BinOperation(pids As List(Of Integer), bolMoveToBin As Boolean) As Boolean
        Dim s As String = "UPDATE p31Worksheet"
        If bolMoveToBin Then
            s += " SET p31ValidUntil=getdate()"
        Else
            s += " SET p31ValidUntil=convert(datetime,'01.01.3000',104)"
        End If
        s += " WHERE p31ID IN (" & String.Join(",", pids) & ")"
        Return _cDB.RunSQL(s)
    End Function
    Public Function Move2Project(intDestP41ID As Integer, pids As List(Of Integer)) As Boolean
        For Each intP31ID In pids
            Dim pars As New DbParameters
            pars.Add("p41id", intDestP41ID, DbType.Int32)
            pars.Add("pid", intP31ID, DbType.Int32)
            pars.Add("login", _curUser.j03Login, DbType.String)
            If _cDB.RunSQL("UPDATE p31Worksheet SET p41ID=@p41id,p31DateUpdate=getdate(),p31UserUpdate=@login WHERE p31ID=@pid", pars) Then
                pars = New DbParameters
                With pars
                    .Add("p31id", intP31ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                    .Add("p48id", Nothing, DbType.Int32)
                    .Add("x45ids", , DbType.String, ParameterDirection.Output, 50)
                End With
                _cDB.RunSP("p31_aftersave", pars)
            End If
        Next
        Return True
    End Function
    Public Function RecalcRates(pids As List(Of Integer)) As Boolean
        For Each intP31ID In pids
            Dim pars As New DbParameters
            With pars
                .Add("p31id", intP31ID, DbType.Int32)
                .Add("j03id_sys", _curUser.PID, DbType.Int32)
                .Add("p48id", Nothing, DbType.Int32)
                .Add("x45ids", , DbType.String, ParameterDirection.Output, 50)
            End With
            _cDB.RunSP("p31_aftersave", pars)
        Next
        Return True
    End Function
    Public Function Recalc_Internal_Rates(dat1 As Date, dat2 As Date, intP51ID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("d1", dat1, DbType.DateTime)
            .Add("d2", dat2, DbType.DateTime)
            .Add("p51id", intP51ID, DbType.Int32)
        End With
        Return _cDB.RunSP("p31_recalc_internal_rates", pars)

    End Function
    Public Function SplitRecord(intP31ID As Integer, dblRec1Hours As Double, strRec1Text As String, dblRec2Hours As Double, strRec2Text As String) As Integer
        Dim pars As New DbParameters
        With pars
            .Add("p31id", intP31ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("rec1_hours", dblRec1Hours, DbType.Double)
            .Add("rec1_text", strRec1Text, DbType.String)
            .Add("rec2_hours", dblRec2Hours, DbType.Double)
            .Add("rec2_text", strRec2Text, DbType.String)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
            .Add("p31id_new", , DbType.Int32, ParameterDirection.Output)
        End With
        If _cDB.RunSP("p31_split", pars) Then
            Return pars.Get(Of Integer)("p31id_new")
        Else
            Return 0
        End If

    End Function
    
    Public Function RemoveFromApproving(pids As List(Of Integer)) As Boolean
        Dim strGUID As String = BO.BAS.GetGUID
        _cDB.RunSQL("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID) SELECT '" & strGUID & "','p31',p31ID FROM p31Worksheet WHERE p31ID IN (" & String.Join(",", pids) & ")")
        Dim pars As New DbParameters
        With pars
            .Add("guid", strGUID, DbType.String)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p31_remove_approve", pars)
    End Function
    Public Function AppendToInvoice(intP91ID As Integer, pids As List(Of Integer)) As Boolean
        Dim strGUID As String = BO.BAS.GetGUID
        _cDB.RunSQL("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID) SELECT '" & strGUID & "','p31',p31ID FROM p31Worksheet WHERE p31ID IN (" & String.Join(",", pids) & ")")
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("guid", strGUID, DbType.String)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p31_append_invoice", pars)
    End Function
    Public Function RemoveFromInvoice(intP91ID As Integer, pids As List(Of Integer)) As Boolean
        Dim strGUID As String = BO.BAS.GetGUID
        _cDB.RunSQL("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID) SELECT '" & strGUID & "','p31',p31ID FROM p31Worksheet WHERE p31ID IN (" & String.Join(",", pids) & ")")
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("guid", strGUID, DbType.String)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p31_remove_invoice", pars)
    End Function
    Public Function UpdateInvoice(intP91ID As Integer, lis As List(Of BO.p31WorksheetInvoiceChange)) As Boolean
        Dim strGUID As String = BO.BAS.GetGUID
        For Each c In lis
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = strGUID
                .p85Prefix = "p31"
                .p85DataPID = c.p31ID
                .p85OtherKey1 = c.p70ID
                .p85Message = c.TextUpdate
                .p85FreeFloat01 = c.InvoiceValue
                .p85FreeFloat02 = c.InvoiceRate
                .p85FreeFloat03 = c.InvoiceVatRate
                .p85FreeNumber01 = c.FixPriceValue * 10000000
                .p85FreeBoolean01 = c.p31IsInvoiceManual
                .p85FreeNumber02 = c.ManualFee * 10000000
            End With
            Dim _cDLTemp As New DL.p85TempBoxDL(_curUser)
            _cDLTemp.Save(cTemp)
        Next
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("guid", strGUID, DbType.String)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p31_change_invoice", pars)
    End Function

    Public Function GetSumHoursPerMonth(intJ02ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.HoursInMonth)
        Dim s As String = "SELECT sum(p31Hours_Orig) as HodinyCelkem,sum(case when p32.p32IsBillable=1 THEN p31Hours_Orig END) as HodinyFa,sum(case when p32.p32IsBillable=0 THEN p31Hours_Orig END) as HodinyNefa,year(p31Date) as Rok,month(p31Date) as Mesic FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID WHERE a.j02ID=@j02id AND a.p31Date BETWEEN @d1 AND @d2 GROUP BY year(p31Date),month(p31Date) ORDER BY year(p31Date),month(p31Date)"
        Dim pars As New DbParameters
        pars.Add("j02id", intJ02ID, DbType.Int32)
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)

        Return _cDB.GetList(Of BO.HoursInMonth)(s, pars)
    End Function
    Public Function GetSumHoursPerEntityAndDate(strPrefix As String, pids As List(Of Integer), d1 As Date, d2 As Date) As IEnumerable(Of BO.p31HoursPerEntityAndDay)
        If pids Is Nothing Then pids = New List(Of Integer)
        Dim s As String = ""
        Select Case strPrefix
            Case "j02"
                s = "SELECT 'j02' as EntityPrefix, j02ID as EntityPID,p31Date,sum(p31Hours_Orig) as Hours_Orig FROM p31Worksheet WHERE p31Date BETWEEN @d1 AND @d2"
                If pids.Count > 0 Then
                    s += " AND j02ID IN (" & String.Join(",", pids) & ")"
                End If
                s += " GROUP BY j02ID,p31Date"
            Case "p41"
                s = "SELECT 'p41' as EntityPrefix, p41ID as EntityPID,p31Date,sum(p31Hours_Orig) as Hours_Orig FROM p31Worksheet WHERE p31Date BETWEEN @d1 AND @d2"
                If pids.Count > 0 Then
                    s += " AND p41ID IN (" & String.Join(",", pids) & ")"
                End If
                s += " GROUP BY p41ID,p31Date"
        End Select

        Dim pars As New DbParameters
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)

        Return _cDB.GetList(Of BO.p31HoursPerEntityAndDay)(s, pars)
    End Function
    Public Function GetDataSourceForTimeline(j02ids As List(Of Integer), d1 As Date, d2 As Date, p41ids As List(Of Integer)) As IEnumerable(Of BO.p31DataSourceForTimeline)
        Dim s As String = "SELECT a.j02ID,a.p41ID,min(p41.p28ID_Client) as p28ID,a.p31Date,sum(a.p31Hours_Orig) as Hours_Orig,a.p32ID,min(p32.p32Name) as p32Name,min(p34.p34Name) as p34Name"
        s += ",min(p34.p34Color) as p34Color,min(p32.p32Color) as p32Color,min(isnull(p41.p41NameShort,p41.p41Name)) as Project,min(p28.p28Name) as Client,min(a.p31ID) as p31ID_min,max(a.p31ID) as p31ID_max"
        s += " FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID"
        s += " LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID"
        s += " WHERE p34.p33ID=1 AND a.p31Date BETWEEN @d1 AND @d2"
        If j02ids.Count > 0 Then
            s += " AND a.j02ID IN (" & String.Join(",", j02ids) & ")"
        End If
        If p41ids.count > 0 Then
            s += " AND a.p41ID IN (" & String.Join(",", p41ids) & ")"
        End If
        s += " GROUP BY a.j02ID,a.p41ID,a.p32ID,a.p31Date"
        Dim pars As New DbParameters
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)

        Return _cDB.GetList(Of BO.p31DataSourceForTimeline)(s, pars)
    End Function

    Public Function GetList_BigSummary(myQuery As BO.myQueryP31) As IEnumerable(Of BO.p31WorksheetBigSummary)
        Dim pars As New DbParameters, s As New System.Text.StringBuilder
        s.Append("select min(j27.j27Code) as j27Code")
        s.Append(",SUM(case when a.p71ID IS NULL THEN a.p31hours_orig END) as rozpracovano_hodiny")
        s.Append(",MIN(case when a.p71ID IS NULL THEN p31Date END) as rozpracovano_prvni")
        s.Append(",MAX(case when a.p71ID IS NULL THEN p31Date END) as rozpracovano_posledni")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p33ID=1 THEN p31Amount_WithoutVat_Orig END) as rozpracovano_honorar")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p34IncomeStatementFlag=1 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as rozpracovano_vydaje")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p34IncomeStatementFlag=2 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as rozpracovano_odmeny")
        s.Append(",SUM(case when a.p71ID IS NULL THEN a.p31Amount_WithoutVat_Orig END) as rozpracovano_celkem")
        s.Append(",SUM(case when a.p71ID IS NULL THEN 1 END) as rozpracovano_pocet")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL AND a.p72ID_AfterApprove=4 THEN a.p31Hours_Approved_Billing END) as schvaleno_hodiny")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Approved END) as schvaleno_honorar")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL AND a.p72ID_AfterApprove=6 THEN (case when a.p31Value_FixPrice is null or a.p31Value_FixPrice=0 then a.p31hours_orig else a.p31Value_FixPrice end) END) as schvaleno_hodiny_pausal")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL AND a.p72ID_AfterApprove IN (2,3) THEN a.p31hours_orig END) as schvaleno_hodiny_odpis")
        s.Append(",SUM(case when a.p71ID=1 AND p34.p34IncomeStatementFlag=1 and a.p72ID_AfterApprove=4 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Approved END) as schvaleno_vydaje")
        s.Append(",SUM(case when a.p71ID=1 AND p34.p34IncomeStatementFlag=1 and a.p72ID_AfterApprove=6 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as schvaleno_vydaje_pausal")
        s.Append(",SUM(case when a.p71ID=1 AND p34.p34IncomeStatementFlag=1 and a.p72ID_AfterApprove IN (2,3) AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as schvaleno_vydaje_odpis")
        s.Append(",SUM(case when a.p71ID=1 AND p34.p34IncomeStatementFlag=2 AND a.p72ID_AfterApprove=4 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Approved END) as schvaleno_odmeny")
        s.Append(",SUM(case when a.p71ID=1 AND p34.p34IncomeStatementFlag=2 AND a.p72ID_AfterApprove=6 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as schvaleno_odmeny_pausal")
        s.Append(",SUM(case when a.p71ID=1 AND p34.p34IncomeStatementFlag=2 AND a.p72ID_AfterApprove IN (2,3) AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as schvaleno_odmeny_odpis")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL THEN a.p31Amount_WithoutVat_Approved END) as schvaleno_celkem")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL THEN 1 END) as schvaleno_pocet")
        s.Append(",MIN(case when a.p71ID=1 AND a.p91ID IS NULL THEN p31Date END) as schvaleno_prvni")
        s.Append(",MAX(case when a.p71ID=1 AND a.p91ID IS NULL THEN p31Date END) as schvaleno_posledni")
        s.Append(",SUM(case when a.p91ID IS NOT NULL THEN a.p31Hours_Invoiced END) as vyfakturovano_hodiny")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND a.p70ID=6 THEN a.p31hours_orig END) as vyfakturovano_hodiny_pausal")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND a.p70ID IN (2,3) THEN a.p31hours_orig END) as vyfakturovano_hodiny_odpis")
        s.Append(",SUM(case when a.p91ID IS NOT NULL THEN a.p31Amount_WithoutVat_Invoiced/p31ExchangeRate_Invoice END) as vyfakturovano_castka")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Invoiced/p31ExchangeRate_Invoice END) as vyfakturovano_honorar")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND p34.p34IncomeStatementFlag=1 and a.p70ID=4 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Invoiced/p31ExchangeRate_Invoice END) as vyfakturovano_vydaje")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND p34.p34IncomeStatementFlag=2 AND a.p70ID=4 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Invoiced/p31ExchangeRate_Invoice END) as vyfakturovano_odmeny")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND p34.p34IncomeStatementFlag=2 AND a.p70ID=6 AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as vyfakturovano_odmeny_pausal")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND p34.p34IncomeStatementFlag=2 AND a.p70ID IN (2,3) AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as vyfakturovano_odmeny_odpis")
        s.Append(",SUM(case when a.p91ID IS NOT NULL AND p34.p34IncomeStatementFlag=1 and a.p70ID IN (2,3) AND p34.p33ID IN (2,5) THEN p31Amount_WithoutVat_Orig END) as vyfakturovano_vydaje_pausal")
        s.Append(",SUM(case when a.p91ID IS NOT NULL THEN 1 END) as vyfakturovano_pocet")
        s.Append(" from p31WorkSheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID")
        s.Append(" LEFT OUTER JOIN j27Currency j27 ON a.j27ID_Billing_Orig=j27.j27ID")
        If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Owner)) Then
            Dim strJ11IDs As String = ""
            If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"
            ''s.Append(" LEFT OUTER JOIN (")
            ''s.Append("SELECT distinct p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag>0 AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID")
            ''s.Append(") zbytek ON a.p31ID=zbytek.p31ID")
            AppendSqlFrom_ProjectRoles(s, "zo28.o28PermFlag>0")
        End If
        
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s.Append(" WHERE " & strW)


        s.Append(" GROUP BY a.j27ID_Billing_Orig,case when a.j27ID_Billing_Invoiced is null then a.j27ID_Billing_Orig else a.j27ID_Billing_Invoiced end")

        Return _cDB.GetList(Of BO.p31WorksheetBigSummary)(s.ToString, pars)
    End Function

    Public Sub UpdateDeleteApprovingSet(strApprovingSet As String, p31ids As List(Of Integer), bolClear As Boolean, strTempGUID As String)
        If p31ids.Count = 0 Then Return
        Dim strTab As String = "p31Worksheet", strW As String = "p31ID IN (" & System.String.Join(",", p31ids) & ")"
        If strTempGUID <> "" Then
            strTab = "p31Worksheet_Temp"
            strW += " AND p31GUID=" & BO.BAS.GS(strTempGUID)
        End If
        If bolClear Then
            _cDB.RunSQL("UPDATE " & strTab & " set p31ApprovingSet=NULL WHERE " & strW)
        Else
            _cDB.RunSQL("UPDATE " & strTab & " set p31ApprovingSet=" & BO.BAS.GS(strApprovingSet) & " WHERE " & strW)
        End If
    End Sub
    Public Function GetList_ApprovingSet(strTempGUID As String, p41ids As List(Of Integer), p28ids As List(Of Integer)) As List(Of String)
        Dim s As String = "SELECT DISTINCT p31ApprovingSet as Value FROM ", pars As New DbParameters
        If strTempGUID <> "" Then
            s += " p31Worksheet_Temp WHERE p31GUID=@guid"
            pars.Add("guid", strTempGUID, DbType.String)
        Else
            s += " p31Worksheet WHERE p31ApprovingSet is not null AND p91ID IS NULL"
        End If

        If Not p41ids Is Nothing Then
            If p41ids.Count > 0 Then
                s += " AND p41ID IN (" & System.String.Join(",", p41ids) & ")"
            End If
        End If
        If Not p28ids Is Nothing Then
            If p28ids.Count > 0 Then
                s += " OR (p91ID IS NULL AND p31ApprovingSet is not null AND p41ID IN (select p41ID FROM p41Project WHERE p28ID_Client IN (" & System.String.Join(",", p28ids) & ")))"
            End If
        End If
        Return _cDB.GetList(Of BO.GetString)(s, pars).Select(Function(p) p.Value).ToList



    End Function

    Public Function GetList_ApprovingFramework(x29id As BO.x29IdEnum, myQuery As BO.myQueryP31, strX18Value As String, bolShowTags As Boolean) As IEnumerable(Of BO.ApprovingFramework)
        Dim s As New System.Text.StringBuilder
        Dim pars As New DbParameters, strPidField As String = ""
        Select Case x29id
            Case BO.x29IdEnum.p41Project
                s.Append("select min(a.p41ID) as PID,min(p41.p41Code) as Code,min(isnull(p41.p41NameShort,p41.p41Name)) as Project,min(p28.p28Name) as Client,'p41' as Prefix")
                strPidField = "a.p41ID"
            Case BO.x29IdEnum.p28Contact
                s.Append("select min(p28.p28ID) as PID,min(p28.p28Code) as Code,min(p28.p28Name) as Client,'p28' as Prefix")
                strPidField = "p28.p28ID"
            Case BO.x29IdEnum.j02Person
                s.Append("select min(j02.j02ID) as PID,min(j02.j02Code) as Code,min(j02.j02LastName+' '+j02.j02FirstName) as Person,'j02' as Prefix")
                strPidField = "j02.j02ID"
            Case BO.x29IdEnum.p56Task
                s.Append("select min(a.p56ID) as PID,min(p56.p56Name+' ('+p56.p56Code+')') as Task, min(p56.p56Code) as Code,min(isnull(p41.p41NameShort,p41.p41Name)) as Project,min(p28.p28Name) as Client,'p56' as Prefix")
                strPidField = "a.p56ID"
        End Select
        If bolShowTags Then
            s.Append(",dbo.tag_values_inline_html(" & CInt(x29id).ToString & ",min(" & strPidField & ")) as TagsInlineHtml")
        Else
            s.Append(",NULL as TagsInlineHtml")
        End If
        s.Append(",a.j27ID_Billing_Orig as j27ID,min(j27.j27Code) as j27Code,SUM(case when a.p71ID IS NULL and a.p31hours_orig<>0 THEN a.p31hours_orig END) as rozpracovano_hodiny")
        s.Append(",MIN(case when a.p71ID IS NULL THEN p31Date END) as rozpracovano_prvni")
        s.Append(",MAX(case when a.p71ID IS NULL THEN p31Date END) as rozpracovano_posledni")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p33ID=1 THEN a.p31Amount_WithoutVat_Orig END) as rozpracovano_honorar")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN p31Amount_WithoutVat_Orig END) as rozpracovano_vydaje")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN p31Amount_WithoutVat_Orig END) as rozpracovano_odmeny")
        s.Append(",SUM(case when a.p71ID IS NULL THEN a.p31Amount_WithoutVat_Orig END) as rozpracovano_celkem")
        s.Append(",SUM(case when a.p71ID IS NULL THEN 1 END) as rozpracovano_pocet")
        s.Append(",SUM(case when a.p71ID IS NULL AND p34.p33ID=3 THEN a.p31Amount_WithoutVat_Orig END) as rozpracovano_kusovnik_honorar")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=4 AND a.p91ID IS NULL THEN a.p31Hours_Approved_Billing END) as schvaleno_hodiny_fakturovat")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=4 AND p34.p33ID=1 THEN p31Amount_WithoutVat_Approved END) as schvaleno_honorar_fakturovat")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=4 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=1 THEN p31Amount_WithoutVat_Approved END) as schvaleno_vydaje_fakturovat")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=4 AND p34.p33ID IN (2,5) AND p34.p34IncomeStatementFlag=2 THEN p31Amount_WithoutVat_Approved END) as schvaleno_odmeny_fakturovat")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=4 AND a.p91ID IS NULL THEN a.p31Amount_WithoutVat_Approved END) as schvaleno_celkem_fakturovat")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=6 AND a.p91ID IS NULL THEN (case when a.p31Value_FixPrice is null or a.p31Value_FixPrice=0 then a.p31hours_orig else a.p31Value_FixPrice end) END) as schvaleno_hodiny_pausal")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove IN (2,3) AND a.p91ID IS NULL THEN a.p31Hours_Orig END) as schvaleno_hodiny_odpis")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=7 AND a.p91ID IS NULL THEN a.p31Hours_Approved_Billing END) as schvaleno_hodiny_pozdeji")
        s.Append(",SUM(case when a.p71ID=1 AND a.p91ID IS NULL THEN 1 END) as schvaleno_pocet")
        s.Append(",MIN(case when a.p71ID=1 AND a.p91ID IS NULL THEN p31Date END) as schvaleno_prvni")
        s.Append(",MAX(case when a.p71ID=1 AND a.p91ID IS NULL THEN p31Date END) as schvaleno_posledni")
        s.Append(",SUM(case when a.p71ID=1 AND a.p72ID_AfterApprove=4 AND p34.p33ID=3 THEN p31Amount_WithoutVat_Approved END) as schvaleno_kusovnik_honorar")
        s.Append(" from p31WorkSheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID")
        s.Append(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID")
        If x29id = BO.x29IdEnum.p56Task Then
            s.Append(" INNER JOIN p56Task p56 ON a.p56ID=p56.p56ID")
        End If
        s.Append(" LEFT OUTER JOIN j27Currency j27 ON a.j27ID_Billing_Orig=j27.j27ID")
        If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Owner)) Then
            ''Dim strJ11IDs As String = ""
            ''If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"

            ''s += " LEFT OUTER JOIN ("
            ''s += "SELECT distinct p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag>0 AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID"
            ''s += ") zbytek ON a.p31ID=zbytek.p31ID"
            'Dim ss As New Text.StringBuilder
            AppendSqlFrom_ProjectRoles(s, "zo28.o28PermFlag>0")
            's.Append(ss.ToString)
        End If


        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s.Append(" WHERE " & strW)

        ''If intJ70ID <> 0 Then
        ''    Dim strInW As String = bas.CompleteSqlJ70(_cDB, intJ70ID, _curUser)
        ''    Select Case x29id
        ''        Case BO.x29IdEnum.p41Project And strInW <> ""
        ''            s += " AND a.p41ID IN (SELECT a.p41ID FROM p41Project a LEFT OUTER JOIN p41Project_FreeField p41free ON a.p41ID=p41free.p41ID WHERE " & strInW & ")"
        ''        Case BO.x29IdEnum.p28Contact And strInW <> ""
        ''            s += " AND p41.p28ID_Client IN (SELECT a.p28ID FROM p28Contact a LEFT OUTER JOIN p28Contact_FreeField p28free ON a.p28ID=p28free.p28ID WHERE " & strInW & ")"
        ''        Case BO.x29IdEnum.j02Person And strInW <> ""
        ''            s += " AND a.j02ID IN (SELECT a.j02ID FROM j02Person a LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID WHERE " & strInW & ")"
        ''        Case BO.x29IdEnum.p56Task And strInW <> ""
        ''            s += " AND a.p56ID IN (SELECT a.p56ID FROM p56Task a LEFT OUTER JOIN p56Task_FreeField p56free ON a.p56ID=p56free.p56ID WHERE " & strInW & ")"
        ''        Case Else
        ''    End Select
        ''End If
        If strX18Value <> "" Then
            Dim strInW As String = bas.TrimWHERE(bas.CompleteX18QuerySql(BO.BAS.GetDataPrefix(x29id), strX18Value))
            Select Case x29id
                Case BO.x29IdEnum.p41Project And strInW <> ""
                    s.Append(" AND a.p41ID IN (SELECT a.p41ID FROM p41Project a LEFT OUTER JOIN p41Project_FreeField p41free ON a.p41ID=p41free.p41ID WHERE " & strInW & ")")
                Case BO.x29IdEnum.p28Contact And strInW <> ""
                    s.Append(" AND p41.p28ID_Client IN (SELECT a.p28ID FROM p28Contact a LEFT OUTER JOIN p28Contact_FreeField p28free ON a.p28ID=p28free.p28ID WHERE " & strInW & ")")
                Case BO.x29IdEnum.j02Person And strInW <> ""
                    s.Append(" AND a.j02ID IN (SELECT a.j02ID FROM j02Person a LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID WHERE " & strInW & ")")
                Case BO.x29IdEnum.p56Task And strInW <> ""
                    s.Append(" AND a.p56ID IN (SELECT a.p56ID FROM p56Task a LEFT OUTER JOIN p56Task_FreeField p56free ON a.p56ID=p56free.p56ID WHERE " & strInW & ")")
                Case Else
            End Select
        End If

        Select Case x29id
            Case BO.x29IdEnum.p41Project
                s.Append(" GROUP BY a.p41ID,a.j27ID_Billing_Orig")
                s.Append(" ORDER BY min(p28.p28Name),min(p41.p41Name),a.j27ID_Billing_Orig")
            Case BO.x29IdEnum.p28Contact
                s.Append(" GROUP BY p41.p28ID_Client,a.j27ID_Billing_Orig,a.j27ID_Billing_Orig")
                s.Append(" ORDER BY min(p28.p28Name)")
            Case BO.x29IdEnum.j02Person
                s.Append(" GROUP BY a.j02ID,a.j27ID_Billing_Orig")
                s.Append(" ORDER BY min(j02.j02LastName+' '+j02.j02FirstName),a.j27ID_Billing_Orig")
            Case BO.x29IdEnum.p56Task
                s.Append(" GROUP BY a.p56ID,a.j27ID_Billing_Orig")
                s.Append(" ORDER BY min(p28.p28Name),min(p41.p41Name),min(p56.p56Name),a.j27ID_Billing_Orig")
        End Select


        Return _cDB.GetList(Of BO.ApprovingFramework)(s.ToString, pars)
    End Function

    Public Function LoadRate(bolCostRate As Boolean, dat As Date, intJ02ID As Integer, intP41ID As Integer, intP32ID As Integer, ByRef intRetJ27ID As Integer) As Double
        Dim pars As New DbParameters, dblRate As Double = 0
        intRetJ27ID = 0
        With pars
            .Add("date_rate", dat, DbType.DateTime)
            If bolCostRate Then
                .Add("pricelisttype", 2, DbType.Int32)
            Else
                .Add("pricelisttype", 1, DbType.Int32)
            End If
            .Add("p41id", BO.BAS.IsNullDBKey(intP41ID), DbType.Int32)
            .Add("j02id", BO.BAS.IsNullDBKey(intJ02ID), DbType.Int32)
            .Add("p32id", BO.BAS.IsNullDBKey(intP32ID), DbType.Int32)
            .Add("ret_j27id", , DbType.Int32, ParameterDirection.Output)
            .Add("ret_rate", , DbType.Double, ParameterDirection.Output)
        End With
        If _cDB.RunSP("p31_getrate_tu", pars) Then
            intRetJ27ID = pars.Get(Of Integer)("ret_j27id")
            dblRate = pars.Get(Of Double)("ret_rate")

            Return dblRate
        Else
            Return 0
        End If
    End Function

    ''Public Function GetDrillDownDatasource(groupCol As BO.PivotRowColumnField, sumCols As List(Of BO.PivotSumField), strParentSqlWhere As String, mq As BO.myQueryP31) As DataTable
    ''    Dim s As New System.Text.StringBuilder, x As Integer = 0
    ''    s.Append("SELECT isnull(" & groupCol.GroupByField & ",0) AS pid")
    ''    s.Append("," & groupCol.SelectField & "as group" & groupCol.FieldTypeID.ToString)

    ''    For Each c In sumCols
    ''        s.Append("," & c.SelectField & " AS col" & c.FieldTypeID.ToString)
    ''    Next
    ''    Select Case groupCol.FieldType
    ''        Case BO.PivotRowColumnFieldType.Person
    ''            s.Append(",'j02' as prefix")
    ''        Case BO.PivotRowColumnFieldType.p41Name
    ''            s.Append(",'p41' as prefix")
    ''        Case BO.PivotRowColumnFieldType.p28Name
    ''            s.Append(",'p28' as prefix")
    ''        Case BO.PivotRowColumnFieldType.p56Name
    ''            s.Append(",'p56' as prefix")
    ''        Case Else
    ''            s.Append(",null as prefix")
    ''    End Select
    ''    AppendSqlFROM_Pivot_Or_Drilldown(s)


    ''    Dim pars As New DL.DbParameters
    ''    Dim strW As String = GetSQLWHERE(mq, pars)
    ''    If strParentSqlWhere <> "" Then
    ''        If strW = "" Then
    ''            strW = strParentSqlWhere
    ''        Else
    ''            strW = "(" & strW & ") AND " & strParentSqlWhere
    ''        End If
    ''    End If
    ''    If strW <> "" Then
    ''        s.Append(" WHERE " & strW)
    ''    End If


    ''    s.Append(" GROUP BY " & groupCol.GroupByField)

    ''    If mq.MG_SortString = "" Then
    ''        s.Append(" ORDER BY " & groupCol.SelectField)
    ''    Else
    ''        s.Append(" ORDER BY " & mq.MG_SortString)
    ''    End If

    ''    Dim ds As DataSet = _cDB.GetDataSet(s.ToString, , pars.Convert2PluginDbParameters())
    ''    If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    ''End Function

    Private Sub AppendSqlFROM_Pivot_Or_Drilldown(ByRef s As Text.StringBuilder)
        s.Append(" FROM p31Worksheet a")
        s.Append(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID")
        s.Append(" INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p42ProjectType p42 ON p41.p42ID=p42.p42ID")
        s.Append(" LEFT OUTER JOIN p28Contact p28Client ON p41.p28ID_Client=p28Client.p28ID")
        s.Append(" LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID")
        s.Append(" LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID LEFT OUTER JOIN p71ApproveStatus p71 ON a.p71ID=p71.p71ID LEFT OUTER JOIN p72PreBillingStatus p72approve ON a.p72ID_AfterApprove=p72approve.p72ID")
        s.Append(" LEFT OUTER JOIN j18Region j18 ON p41.j18ID=j18.j18ID")
        s.Append(" LEFT OUTER JOIN j18Region j18_j02 ON j02.j18ID=j18_j02.j18ID")
        s.Append(" LEFT OUTER JOIN j27Currency j27orig ON a.j27ID_Billing_Orig=j27orig.j27ID")
        s.Append(" LEFT OUTER JOIN j27Currency j27invoice ON a.j27ID_Billing_Invoiced=j27invoice.j27ID")
        s.Append(" LEFT OUTER JOIN p95InvoiceRow p95 ON p32.p95ID=p95.p95ID")
        s.Append(" LEFT OUTER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID")
        s.Append(" LEFT OUTER JOIN p28Contact p91Receiver ON p91.p28ID=p91Receiver.p28ID")
        s.Append(" LEFT OUTER JOIN j07PersonPosition j07 ON j02.j07ID=j07.j07ID")


        If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Owner)) Then
            ''Dim strJ11IDs As String = ""
            ''If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"
            ''s.Append(" LEFT OUTER JOIN (")
            ''s.Append("SELECT distinct p31x.p31ID FROM p31Worksheet p31x INNER JOIN p32Activity p32x ON p31x.p32ID=p32x.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag>0 AND x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")) scope ON p31x.p41ID=scope.x69RecordPID AND p32x.p34ID=scope.p34ID")
            ''s.Append(") zbytek ON a.p31ID=zbytek.p31ID")
            AppendSqlFrom_ProjectRoles(s, "zo28.o28PermFlag>0")
        End If

    End Sub
    Public Function UpdateTemp_After_EditOrig(intP31ID As Integer, strGUID As String) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("p31id", intP31ID, DbType.Int32)
            .Add("guid", strGUID, DbType.String)
        End With
        Return _cDB.RunSP("p31_update_temp_after_edit_orig", pars)

    End Function

    Public Function GetDrillDownGridSource(colDD1 As BO.GridColumn, colDD2 As BO.GridColumn, sumCols_Pivot As List(Of BO.PivotSumField), addCols As List(Of BO.GridColumn), strParentSqlWhere As String, mq As BO.myQueryP31) As DataTable
        Dim s As New System.Text.StringBuilder, x As Integer = 0, lisSqlFROM As New List(Of String)

        s.Append("SELECT " & colDD1.Pivot_SelectSql & " AS " & colDD1.ColumnName)
        If colDD1.SqlSyntax_FROM <> "" Then lisSqlFROM.Add(colDD1.SqlSyntax_FROM)
        If Not colDD2 Is Nothing Then
            s.Append("," & colDD2.Pivot_SelectSql & " AS " & colDD2.ColumnName)
            s.Append(",isnull(convert(varchar(50)," & colDD1.Pivot_GroupBySql & "),'')+'|'+isnull(convert(varchar(50)," & colDD2.Pivot_GroupBySql & "),'') as pid")

            If colDD2.SqlSyntax_FROM <> "" Then lisSqlFROM.Add(colDD2.SqlSyntax_FROM)
        Else
            s.Append(",isnull(convert(varchar(50)," & colDD1.Pivot_GroupBySql & "),'') as pid")
        End If
        s.Append(",COUNT(*) as RecsCount,MIN(a.p31Date) as RecFirst,MAX(a.p31Date) as RecLast")
        s.Append("," & colDD1.Pivot_GroupBySql & " as dd1")
        If Not colDD2 Is Nothing Then s.Append("," & colDD2.Pivot_GroupBySql & " as dd2")

        If Not sumCols_Pivot Is Nothing Then
            For Each c In sumCols_Pivot
                s.Append("," & c.SelectField & " AS sum" & c.FieldTypeID.ToString)
            Next
        End If
        If Not addCols Is Nothing Then
            For Each c In addCols
                Dim strField As String = c.ColumnDBName
                If strField = "" Then strField = c.ColumnName
                Select Case c.MyTag
                    Case "all"
                        s.Append(",max(" & strField & ") AS col" & c.ColumnName)
                    Case "min"
                        s.Append(",min(" & strField & ") AS col" & c.ColumnName)
                    Case "max"
                        s.Append(",max(" & strField & ") AS col" & c.ColumnName)
                End Select
                If c.SqlSyntax_FROM <> "" Then lisSqlFROM.Add(c.SqlSyntax_FROM)
                
            Next
        End If

        If lisSqlFROM.Count > 0 Then mq.MG_AdditionalSqlFROM = String.Join(" ", lisSqlFROM.Distinct)
        s.Append(" " & GetSQLPart2("", mq))


        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(mq, pars)
        If strParentSqlWhere <> "" Then
            If strW = "" Then
                strW = strParentSqlWhere
            Else
                strW = "(" & strW & ") AND " & strParentSqlWhere
            End If
        End If
        If strW <> "" Then
            s.Append(" WHERE " & strW)
        End If

        s.Append(" GROUP BY " & colDD1.Pivot_GroupBySql)
        If Not colDD2 Is Nothing Then
            s.Append("," & colDD2.Pivot_GroupBySql)
        End If
        If Not addCols Is Nothing Then
            For Each c In addCols.Where(Function(p) p.MyTag = "all")
                If c.Pivot_GroupBySql <> "" Then
                    s.Append("," & c.Pivot_GroupBySql)
                Else
                    s.Append("," & c.ColumnName)
                End If
            Next
        End If

        If mq.MG_SortString = "" Then
            s.Append(" ORDER BY " & colDD1.Pivot_SelectSql)
            If Not colDD2 Is Nothing Then
                s.Append("," & colDD2.Pivot_SelectSql)
            End If
        Else
            s.Append(" ORDER BY " & mq.MG_SortString)
        End If

        Dim ds As DataSet = _cDB.GetDataSet(s.ToString, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function
End Class
