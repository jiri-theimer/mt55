Imports Dapper

Public Class o27AttachmentDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o27Attachment
        Dim s As String = GetSQLPart1()
        s += " WHERE a.o27ID=@pid"
        Return _cDB.GetRecord(Of BO.o27Attachment)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByGUID(strGUID As String) As BO.o27Attachment
        Dim s As String = GetSQLPart1()
        s += " WHERE a.o27GUID=@guid"
        Return _cDB.GetRecord(Of BO.o27Attachment)(s, New With {.guid = strGUID})
    End Function
    Private Function GetSQLPart1(Optional intTOP As Integer = 0) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*," & bas.RecTail("o27", "a") & ",o13.o13Name as _o13Name,o13.o13ArchiveFolder as _o13ArchiveFolder"
        s += ",a.o27ArchiveFileName as _o27ArchiveFileName,a.o27FileExtension as _o27FileExtension"
        s += ",a.o27ArchiveFolder as _o27ArchiveFolder,a.o27FileSize as _o27FileSize"
        s += " FROM o27Attachment a INNER JOIN o13AttachmentType o13 ON a.o13ID=o13.o13ID"
        Return s
    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("o27_delete", pars)
    End Function
    Public Function Save(cRec As BO.o27Attachment, strOrigFileName As String, strFileExtension As String, intFileSize As Integer, strArchiveFileName As String, strArchiveFolder As String) As Boolean

        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        strOrigFileName = Trim(strOrigFileName)

        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o27id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("o27ArchiveFileName", strArchiveFileName, DbType.String, , , True)
            pars.Add("o27ArchiveFolder", strArchiveFolder, DbType.String, , , True)

            pars.Add("o27Name", .o27Name, DbType.String, , , True, "Název")
            pars.Add("o27GUID", .o27GUID, DbType.String)
            pars.Add("o13ID", BO.BAS.IsNullDBKey(.o13ID), DbType.Int32)

           
            pars.Add("b07ID", BO.BAS.IsNullDBKey(.b07ID), DbType.Int32)
            pars.Add("x31ID", BO.BAS.IsNullDBKey(.x31ID), DbType.Int32)
            pars.Add("x40ID", BO.BAS.IsNullDBKey(.x40ID), DbType.Int32)
            pars.Add("x50ID", BO.BAS.IsNullDBKey(.x50id), DbType.Int32)

            pars.Add("o27FileSize", intFileSize, DbType.Int32)
            pars.Add("o27OriginalFileName", strOrigFileName, DbType.String, , , True)
            pars.Add("strFileExtension", strFileExtension, DbType.String, , , True)
            pars.Add("o27ContentType", .o27ContentType, DbType.String, , , True)

            pars.Add("o27validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("o27validuntil", cRec.ValidUntil, DbType.DateTime)
        End With


        If _cDB.SaveRecord("o27Attachment", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function
    Public Function UpdateOnly(cRec As BO.o27Attachment) As Boolean
        If cRec.PID = 0 Then Return False

        Dim pars As New DbParameters()
        With cRec
            pars.Add("pid", .PID)
            pars.Add("o27Name", .o27Name, DbType.String, , , True, "Název")
            pars.Add("o13ID", BO.BAS.IsNullDBKey(.o13ID), DbType.Int32)

            pars.Add("o27GUID", .o27GUID, DbType.String)
           
            pars.Add("o27ContentType", .o27ContentType, DbType.String, , , True)
            pars.Add("o27validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("o27validuntil", cRec.ValidUntil, DbType.DateTime)
        End With


        If _cDB.SaveRecord("o27Attachment", pars, False, "o27id=@pid", True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetList(myQuery As BO.myQueryO27) As IEnumerable(Of BO.o27Attachment)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.o27ID", myQuery)
        ''strW += bas.ParseWhereValidity("o27", "a", myQuery)

        Dim pars As New DbParameters
        With myQuery
            If .o13ID <> 0 Then
                strW += " AND a.o13ID=@o13id"
                pars.Add("o13id", .o13ID, DbType.Int32)
            End If
            If .Record_x29ID > BO.x29IdEnum._NotSpecified And .Record_PID <> 0 Then
                pars.Add("record_pid", .Record_PID, DbType.Int32)
                pars.Add("x29id", .Record_x29ID)
                strW += " AND a.b07ID IN (select b07ID FROM b07Comment WHERE x29ID=@x29id AND b07RecordPID=@record_pid)"
            End If
            
            If .x31ID <> 0 Then
                strW += " AND a.x31ID=@x31id"
                pars.Add("x31id", .x31ID, DbType.Int32)
            End If
            If .x50ID <> 0 Then
                strW += " AND a.x50ID=@x50id"
                pars.Add("x50id", .x50ID, DbType.Int32)
            End If
            If .x40ID <> 0 Then
                strW += " AND a.x40ID=@x40id"
                pars.Add("x40id", .x40ID, DbType.Int32)
            End If

            If .GUID <> "" Then
                strW += " AND a.o27GUID=@guid"
                pars.Add("guid", .GUID, DbType.String)
            End If

        End With
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.o27ID DESC"

        Return _cDB.GetList(Of BO.o27Attachment)(s, pars)

    End Function

    Public Sub CopyRecordsToTemp(strGUID As String, x29id As BO.x29IdEnum, intDataPID As Integer)
        Dim s As String = "IF EXISTS(select p85ID FROM p85TempBox WHERE p85GUID='" & strGUID & "') DELETE FROM p85TempBox WHERE p85GUID='" & strGUID & "';"
        s += " INSERT INTO p85TempBox(p85guid,p85datapid,p85prefix,p85FreeText04,p85FreeText03,p85FreeText01,p85FreeText05,p85FreeNumber01,p85freedate01,p85OtherKey1,p85FreeText02,p85FreeText06)"
        s += " SELECT '" & strGUID & "',o27ID,'o27',o27Name,o27ContentType,o27OriginalFileName,o27FileExtension,o27FileSize,o27DateInsert,a.o13ID,o27ArchiveFileName,o13Name"
        s += " FROM o27Attachment a INNER JOIN o13AttachmentType o13 ON a.o13ID=o13.o13ID"
        s += " WHERE a." & BO.BAS.GetDataPrefix(x29id) & "ID=@datapid"

        Dim pars As New DbParameters
        pars.Add("datapid", intDataPID, DbType.Int32)

        _cDB.RunSQL(s, pars)
    End Sub

    Public Function UploadAndSave(cRec As BO.o27Attachment, strOrigFileName As String, strOrigExtension As String, intFileSize As Integer, strArchiveFileName As String, strArchiveFolder As String) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        strOrigFileName = Trim(strOrigFileName)

        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o27id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            
            pars.Add("x31ID", BO.BAS.IsNullDBKey(.x31ID), DbType.Int32)
            pars.Add("x40ID", BO.BAS.IsNullDBKey(.x40ID), DbType.Int32)
            pars.Add("x50ID", BO.BAS.IsNullDBKey(.x50ID), DbType.Int32)
            pars.Add("b07ID", BO.BAS.IsNullDBKey(.b07ID), DbType.Int32)

            pars.Add("o27ArchiveFileName", strArchiveFileName, DbType.String, , , True)
            pars.Add("o27ArchiveFolder", strArchiveFolder, DbType.String, , , True)

            pars.Add("o27Name", .o27Name, DbType.String, , , True, "Název")
            pars.Add("o27GUID", .o27GUID, DbType.String)
           
            pars.Add("o13ID", BO.BAS.IsNullDBKey(.o13ID), DbType.Int32)
            pars.Add("o27FileSize", intFileSize, DbType.Int32)
            pars.Add("o27OriginalFileName", strOrigFileName, DbType.String, , , True)
            pars.Add("o27FileExtension", strOrigExtension, DbType.String, , , True)
            pars.Add("o27ContentType", .o27ContentType, DbType.String, , , True)

            pars.Add("o27validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("o27validuntil", cRec.ValidUntil, DbType.DateTime)
        End With


        If _cDB.SaveRecord("o27Attachment", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function
End Class
