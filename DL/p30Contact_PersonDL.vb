Public Class p30Contact_PersonDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,j02.*,p27.p27Name as _p27Name,p28.p28CompanyName as _p28CompanyName,p28.p28Name as _p28Name,isnull(p41.p41NameShort,p41.p41Name) as _p41Name,p41.p41Code as _p41Code," & bas.RecTail("p30", "a", False, True)
        s += ",j02.j02ValidFrom as _validfrom,j02.j02ValidUntil as _validuntil"
        s += " FROM p30Contact_Person a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p27ContactPersonRole p27 ON a.p27ID=p27.p27ID LEFT OUTER JOIN p41Project p41 ON a.p41ID=p41.p41ID"
        Return s
    End Function
    Public Overloads Function Load(intPID As Integer) As BO.p30Contact_Person
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p30ID=@pid"

        Return _cDB.GetRecord(Of BO.p30Contact_Person)(s, New With {.pid = intPID})
    End Function

    Public Function GetList(intP28ID As Integer, intP41ID As Integer, intJ02ID As Integer) As IEnumerable(Of BO.p30Contact_Person)
        Dim pars As New DbParameters
        Dim s As String = GetSQLPart1()
        If intP28ID <> 0 And intP41ID <> 0 Then
            s += " WHERE (a.p28ID=@p28id OR a.p41ID=@p41id)"
            pars.Add("p28id", intP28ID, DbType.Int32)
            pars.Add("p41id", intP41ID, DbType.Int32)
        Else
            If intP28ID <> 0 Then
                s += " WHERE (a.p28ID=@p28id OR a.p41ID IN (select p41ID FROM p41Project WHERE p28ID_Client=@p28id))"
                pars.Add("p28id", intP28ID, DbType.Int32)
            End If
            If intP41ID <> 0 Then
                s += " WHERE a.p41ID=@p41id"
                pars.Add("p41id", intP41ID, DbType.Int32)
            End If
        End If
        
        If intJ02ID <> 0 Then
            s += " WHERE a.j02ID=@j02id"
            pars.Add("j02id", intJ02ID, DbType.Int32)
        End If

        Return _cDB.GetList(Of BO.p30Contact_Person)(s, pars)
    End Function

    Public Function GetList_J02(intP28ID As Integer, intP41ID As Integer, bolIncludeClientProjects As Boolean) As IEnumerable(Of BO.j02Person)
        Dim s As String = "SELECT"
        s += " a.j07ID,a.j17ID,a.j18ID,a.c21ID,a.j02IsIntraPerson,a.j02FirstName,a.j02LastName,a.j02TitleBeforeName,a.j02TitleAfterName,a.j02Code,a.j02JobTitle,a.j02Email,a.j02Mobile,a.j02Phone,a.j02Office,a.j02EmailSignature,a.j02Description,a.j02AvatarImage"
        s += ",j02free.*,j07.j07Name as _j07Name,c21.c21Name as _c21Name,j18.j18Name as _j18Name,a.j02IsInvoiceEmail," & bas.RecTail("j02", "a")
        s += " FROM j02Person a LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID LEFT OUTER JOIN c21FondCalendar c21 ON a.c21ID=c21.c21ID LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID"
        Dim strW As String = "", pars As New DbParameters
        If intP28ID <> 0 Then
            strW += " OR p28ID=@p28id"
            pars.Add("p28id", intP28ID, DbType.Int32)
        End If
        If intP28ID <> 0 And bolIncludeClientProjects Then
            strW += " OR p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@p28id)"
        End If
        If intP41ID <> 0 Then
            strW += " OR p41ID=@p41id"
            pars.Add("p41id", intP41ID, DbType.Int32)
        End If
        s += " WHERE a.j02ID IN (SELECT j02ID FROM p30Contact_Person WHERE " & bas.TrimWHERE(strW) & ")"

        s += " ORDER BY a.j02LastName,a.j02FirstName"
        Return _cDB.GetList(Of BO.j02Person)(s, pars)
    End Function

    Public Function Save(cRec As BO.p30Contact_Person) As Boolean

        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p30ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j02ID", .j02ID, DbType.Int32)
            pars.Add("p28ID", BO.BAS.IsNullDBKey(.p28ID), DbType.Int32)
            pars.Add("p41ID", BO.BAS.IsNullDBKey(.p41ID), DbType.Int32)
            pars.Add("p27ID", BO.BAS.IsNullDBKey(.p27ID), DbType.Int32)
            pars.Add("p30ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("p30ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("p30Contact_Person", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            
            Return True
        Else
            Return False
        End If

    End Function

    ''Public Function SaveAsDefaultPerson(cRec As BO.p30Contact_Person, bolp30IsDefaultInWorksheet As Boolean) As Boolean
    ''    With cRec
    ''        _cDB.RunSQL("UPDATE p30Contact_Person set p30IsDefaultInWorksheet=" & BO.BAS.GB(bolp30IsDefaultInWorksheet) & " WHERE p30ID=" & .PID.ToString)
    ''        If bolp30IsDefaultInWorksheet Then
    ''            If .p28ID <> 0 Then
    ''                _cDB.RunSQL("UPDATE p30Contact_Person set p30IsDefaultInWorksheet=0 WHERE p28ID=" & .p28ID.ToString & " AND p30ID<>" & .PID.ToString)
    ''            End If
    ''            If .p41ID <> 0 Then
    ''                _cDB.RunSQL("UPDATE p30Contact_Person set p30IsDefaultInWorksheet=0 WHERE p41ID=" & .p41ID.ToString & " AND p30ID<>" & .PID.ToString)
    ''            End If
    ''        End If
    ''    End With
    ''    Return True
    ''End Function
    ''Public Function SaveAsDefaultInInvoice(cRec As BO.p30Contact_Person, bolp30IsDefaultInInvoice As Boolean) As Boolean
    ''    With cRec
    ''        _cDB.RunSQL("UPDATE p30Contact_Person set p30IsDefaultInInvoice=" & BO.BAS.GB(bolp30IsDefaultInInvoice) & " WHERE p30ID=" & .PID.ToString)
    ''        If bolp30IsDefaultInInvoice Then
    ''            If .p28ID <> 0 Then
    ''                _cDB.RunSQL("UPDATE p30Contact_Person set p30IsDefaultInInvoice=0 WHERE p28ID=" & .p28ID.ToString & " AND p30ID<>" & .PID.ToString)
    ''            End If
    ''            If .p41ID <> 0 Then
    ''                _cDB.RunSQL("UPDATE p30Contact_Person set p30IsDefaultInInvoice=0 WHERE p41ID=" & .p41ID.ToString & " AND p30ID<>" & .PID.ToString)
    ''            End If
    ''        End If
    ''    End With
    ''    Return True
    ''End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p30_delete", pars)
    End Function

    Public Function GetList_p27() As IEnumerable(Of BO.p27ContactPersonRole)
        Return _cDB.GetList(Of BO.p27ContactPersonRole)("SELECT p27ID as pid,p27Name," & bas.RecTail("p27", "") & " FROM p27ContactPersonRole WHERE getdate() between p27ValidFrom AND p27ValidUntil")
    End Function
End Class
