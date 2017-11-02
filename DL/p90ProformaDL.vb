Public Class p90ProformaDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p90Proforma
        Dim s As String = GetSQLPart1(0, False)
        s += " WHERE a.p90ID=@p90id"

        Return _cDB.GetRecord(Of BO.p90Proforma)(s, New With {.p90id = intPID})
    End Function
    Public Function LoadP82(intP82ID As Integer) As BO.p82Proforma_Payment
        Dim s As String = "SELECT a.*," & bas.RecTail("p82", "a", False)
        s += " FROM p82Proforma_Payment a WHERE a.p82ID=@p82id"

        Return _cDB.GetRecord(Of BO.p82Proforma_Payment)(s, New With {.p82id = intP82ID})
    End Function
    Public Function LoadMyLastCreated() As BO.p90Proforma
        Dim s As String = GetSQLPart1(1, False)
        s += " WHERE a.j02ID_Owner=@j02id_owner ORDER BY a.p90ID DESC"

        Return _cDB.GetRecord(Of BO.p90Proforma)(s, New With {.j02id_owner = _curUser.j02ID})
    End Function

    Public Function Save(cRec As BO.p90Proforma, lisFF As List(Of BO.FreeField), lisP82 As List(Of BO.p82Proforma_Payment)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p90ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            If .PID = 0 Then
                pars.Add("p90IsDraft", .p90IsDraft, DbType.Boolean) 'info o draftu raději ukládat pouze při založení a poté už jenom pomocí workflow
            End If
            pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
            pars.Add("p89ID", BO.BAS.IsNullDBKey(.p89ID), DbType.Int32)
            pars.Add("p28ID", BO.BAS.IsNullDBKey(.p28ID), DbType.Int32)
            pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)

            pars.Add("p90Amount_WithoutVat", .p90Amount_WithoutVat, DbType.Double)
            pars.Add("p90Amount_Vat", .p90Amount_Vat, DbType.Double)
            pars.Add("p90VatRate", .p90VatRate, DbType.Double)
            pars.Add("p90Amount", .p90Amount, DbType.Double)

            pars.Add("p90Code", .p90Code, DbType.String, , , True, "Číslo zálohy")

            pars.Add("p90Text1", .p90Text1, DbType.String, , , True, "Text")
            pars.Add("p90Text2", .p90Text2, DbType.String, , , True, "p90Text2")
            pars.Add("p90TextDPP", .p90TextDPP, DbType.String, , , True, "p90TextDPP")


            pars.Add("p90Date", BO.BAS.IsNullDBDate(.p90Date), DbType.DateTime)
            pars.Add("p90DateMaturity", BO.BAS.IsNullDBDate(.p90DateMaturity), DbType.DateTime)

            pars.Add("p90IsDraft", .p90IsDraft, DbType.Boolean)

            pars.Add("p90validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p90validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p90Proforma", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedPID As Integer = cRec.PID
            If bolINSERT Then intLastSavedPID = _cDB.LastIdentityValue

            If Not lisFF Is Nothing Then    'volná pole
                bas.SaveFreeFields(_cDB, lisFF, "p90Proforma_FreeField", intLastSavedPID)
            End If
            If Not lisP82 Is Nothing Then   'úhrady
                For Each c In lisP82
                    pars = New DbParameters
                    pars.Add("p90ID", intLastSavedPID, DbType.Int32)
                    pars.Add("p82Date", c.p82Date, DbType.DateTime)
                    pars.Add("p82Amount", c.p82Amount, DbType.Double)
                    If c.p82Amount <> (c.p82Amount_WithoutVat + c.p82Amount_Vat) Then
                        If cRec.p90VatRate > 0 Then
                            c.p82Amount_WithoutVat = Math.Round(c.p82Amount / (1 + cRec.p90VatRate / 100), 1)
                            c.p82Amount_Vat = c.p82Amount - c.p82Amount_WithoutVat
                        Else
                            c.p82Amount_WithoutVat = c.p82Amount
                            c.p82Amount_Vat = 0
                        End If
                        
                    End If
                    pars.Add("p82Amount_WithoutVat", c.p82Amount_WithoutVat, DbType.Double)
                    pars.Add("p82Amount_Vat", c.p82Amount_Vat, DbType.Double)
                    pars.Add("p82Text", c.p82Text, DbType.String)
                    If c.PID = 0 And c.IsSetAsDeleted = False Then
                        _cDB.SaveRecord("p82Proforma_Payment", pars, True, , True, _curUser.j03Login, False)
                    Else
                        If c.IsSetAsDeleted = False Then
                            _cDB.SaveRecord("p82Proforma_Payment", pars, False, "p82ID=" & c.PID.ToString, True, _curUser.j03Login, False)
                        Else
                            _cDB.RunSQL("DELETE FROM p82Proforma_Payment WHERE p82ID=" & c.PID.ToString)
                        End If
                    End If
                Next
            End If

            pars = New DbParameters
            With pars
                .Add("p90id", intLastSavedPID, DbType.Int32)
                .Add("j03id_sys", _curUser.PID, DbType.Int32)
            End With
            If _cDB.RunSP("p90_aftersave", pars) Then
                Return True
            Else
                Return False
            End If
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
        Return _cDB.RunSP("p90_delete", pars)
    End Function

    Public Function GetList_p99(intP91ID As Integer, intP90ID As Integer, intP82ID As Integer) As IEnumerable(Of BO.p99Invoice_Proforma)
        Dim pars As New DbParameters
        Dim s As String = "SELECT a.*,p90.p90Code as _p90Code,p91.p91Code as _p91Code"
        s += " FROM p99Invoice_Proforma a INNER JOIN p90Proforma p90 ON a.p90ID=p90.p90ID INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID"
        If intP91ID <> 0 Then
            pars.Add("p91id", intP91ID, DbType.Int32)
            s += " WHERE a.p91ID=@p91id"
        End If
        If intP90ID <> 0 Then
            pars.Add("p90id", intP90ID, DbType.Int32)
            s += " WHERE a.p90ID=@p90id"
        End If
        If intP82ID <> 0 Then
            pars.Add("p82id", intP82ID, DbType.Int32)
            s += " WHERE a.p82ID=@p82id"
        End If
        Return _cDB.GetList(Of BO.p99Invoice_Proforma)(s, pars)
    End Function
    Public Function GetList_p82(intP90ID As Integer) As IEnumerable(Of BO.p82Proforma_Payment)
        Dim pars As New DbParameters

        Dim s As String = "SELECT a.*," & bas.RecTail("p82", "a", False)
        s += " FROM p82Proforma_Payment a"
        pars.Add("p90id", intP90ID, DbType.Int32)
        s += " WHERE a.p90ID=@p90id"

        Return _cDB.GetList(Of BO.p82Proforma_Payment)(s, pars)
    End Function
    Public Function GetList(myQuery As BO.myQueryP90) As IEnumerable(Of BO.p90Proforma)
        Dim s As String = GetSQLPart1(0, myQuery.IsShowTagsInColumn), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p90ID", myQuery)
        With myQuery
            If .p89ID <> 0 Then
                pars.Add("p89id", .p89ID, DbType.Int32)
                strW += " AND a.p89ID=@p89id"
            End If
            If .p28ID <> 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND a.p28ID=@p28id"
            End If
            If .p91id <> 0 Then
                pars.Add("p91id", .p91ID, DbType.Int32)
                strW += " AND (a.p90ID IN (SELECT p90ID FROM p99Invoice_Proforma WHERE p91ID=@p91id))"
            End If
            If .IsP99Bounded = BO.BooleanQueryMode.TrueQuery Then
                strW += " AND a.p90ID IN (SELECT p90ID FROM p99Invoice_Proforma)"
            End If
            If .IsP99Bounded = BO.BooleanQueryMode.FalseQuery Then
                strW += " AND a.p90ID NOT IN (SELECT p90ID FROM p99Invoice_Proforma)"   'zálohy ještě nesvázané s fakturami
            End If
            If .SearchExpression <> "" Then
                strW += " AND (a.p90Code like '%'+@expr+'%' OR a.p90Text1 LIKE '%'+@expr+'%' OR p28.p28Name like '%'+@expr+'%')"
                pars.Add("expr", myQuery.SearchExpression, DbType.String)
            End If
            If .ColumnFilteringExpression <> "" Then
                strW += " AND " & .ColumnFilteringExpression.Replace("[", "").Replace("]", "")
            End If
          
            If .j27ID <> 0 Then
                pars.Add("j27id", .j27ID, DbType.Int32)
                strW += " AND a.j27ID=@j27id"
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then strW += " AND a.p90ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=390 AND o51ID IN (" & String.Join(",", .o51IDs) & "))"
            End If
            If Year(.DateFrom) > 2000 Or Year(.DateUntil) < 2100 Then
                pars.Add("d1", .DateFrom, DbType.DateTime)
                pars.Add("d2", .DateUntil, DbType.DateTime)
                strW += " AND a.p90Date BETWEEN @d1 AND @d2"
            End If
        End With
        strW += bas.ParseWhereValidity("p90", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)


        's += " ORDER BY p90DateUntil"

        Return _cDB.GetList(Of BO.p90Proforma)(s, pars)

    End Function


    Private Function GetSQLPart1(intTOP As Integer, bolIncludeTags As Boolean) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*,j27.j27Code as _j27Code,p89.p89Name as _p89Name," & bas.RecTail("p90", "a")
        s += ",p28.p28Name as _p28Name,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner,p82.p82Code as _p82Code,p82.p82ID as _p82ID"
        If bolIncludeTags Then s += ",dbo.tag_values_inline_html(390,a.p90ID) as TagsInlineHtml" Else s += ",NULL as TagsInlineHtml"
        s += " FROM p90Proforma a INNER JOIN p89ProformaType p89 ON a.p89ID=p89.p89ID"
        s += " LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID"
        s += " LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID"
        s += " LEFT OUTER JOIN p82Proforma_Payment p82 ON a.p90ID=p82.p90ID"

        Return s
    End Function

    Public Function UpdateP82Code(intP82ID As Integer, strP82Code As String) As Boolean
        Dim pars As New DbParameters()
        pars.Add("p82code", strP82Code, DbType.String)
        pars.Add("p82id", intP82ID, DbType.Int32)
        If _cDB.RunSQL("UPDATE p82Proforma_Payment SET p82Code=@p82code WHERE p82ID=@p82id", pars) Then
            Return True
        Else
            Return False
        End If
    End Function
    
End Class
