Imports Telerik.Web.UI

Public Class fileupload
    Inherits System.Web.UI.UserControl
    Public Event AfterUploadAll()
    Public Event AfterUploadOneFile(ByVal strFulllPath As String, ByVal strErrorMessage As String)
    Public Event ErrorUpload(ByVal strFileName As String, strError As String)
    Public Property Factory As BL.Factory

    Public Property GUID() As String
        Get
            Return hidGUID.Value
        End Get
        Set(ByVal value As String)
            hidGUID.Value = value
        End Set
    End Property
    Public Property EntityX29ID As BO.x29IdEnum
        Get
            If Me.hidX29ID.Value <> "" Then
                Return CInt(Me.hidX29ID.Value)
            Else
                Return BO.x29IdEnum._NotSpecified
            End If
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
        End Set
    End Property



    Public WriteOnly Property AllowedFileExtensions As String
        Set(value As String)
            If value <> "" Then
                upload1.AllowedFileExtensions = Split(value, ",")
                ShowLimits()
            End If

        End Set
    End Property
    Public WriteOnly Property MaxFileSize As Integer
        Set(value As Integer)
            upload1.MaxFileSize = value
            ShowLimits()
        End Set
    End Property

    Private Sub ShowLimits()
        Me.lblLimitsInfo.Text = ""
        Dim s As String = String.Join(",", Me.upload1.AllowedFileExtensions)
        If s <> "" Then Me.lblLimitsInfo.Text = String.Format("Povolené formáty: {0}", "<b>" & Replace(s, ",", ", ") & "</b>")

        If Me.upload1.MaxFileSize > 0 Then
            Me.lblLimitsInfo.Text += String.Format(" | Max. povolená velikost: {0} ", "<b>" & BO.BAS.FormatFileSize(Me.upload1.MaxFileSize)) & "</b>"
        End If
    End Sub

    Public Property ButtonText_Add() As String
        Get
            Return BO.BAS.IsNull(ViewState("buttontext_add"))
        End Get
        Set(ByVal value As String)
            ViewState("buttontext_add") = value
        End Set
    End Property
    Public Property InitialFileInputsCount() As Integer
        Get
            Return upload1.InitialFileInputsCount
        End Get
        Set(ByVal value As Integer)
            upload1.InitialFileInputsCount = value
        End Set
    End Property

    Public Property MaxFileInputsCount() As Integer
        Get
            Return upload1.MaxFileInputsCount
        End Get
        Set(value As Integer)
            upload1.MaxFileInputsCount = value
        End Set
    End Property

    Public Property MaxFileUploadedCount As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMaxFileUploadedCount.Value)
        End Get
        Set(value As Integer)
            Me.hidMaxFileUploadedCount.Value = value.ToString
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        upload1.ControlObjectsVisibility = ControlObjectsVisibility.AddButton Or ControlObjectsVisibility.RemoveButtons
        With upload1.Localization
            If Me.ButtonText_Add = "" Then
                .Add = "Přidat přílohu"
            Else
                .Add = Me.ButtonText_Add
            End If

            .Clear = "Vyčistit"
            .Remove = "Odstranit"
            .Select = "Procházet"
            .Delete = "Odstranit"
        End With

        If Me.o13ID.Items.Count = 0 Then
            Me.o13ID.DataSource = Me.Factory.o13AttachmentTypeBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.EntityX29ID)
            Me.o13ID.DataBind()
        End If
    End Sub


    Public Sub TryUploadhWaitingFilesOnClientSide()
        TryUploadFiles2Temp()
    End Sub



    Private Function GetValidArchiveFileName(strFileName As String) As String
        Dim invalidChars As String = Regex.Escape(New String(System.IO.Path.GetInvalidFileNameChars()))
        Dim invalidReStr As String = String.Format("[{0}]+", invalidChars)
        Return Regex.Replace(strFileName, invalidReStr, "_").Replace(";", "").Replace(",", "")
    End Function

    Private Sub TryUploadFiles2Temp()
        lblError.Text = ""

        Dim strDir As String = Me.Factory.x35GlobalParam.TempFolder, bolOK As Boolean = False
        Dim cO13 As BO.o13AttachmentType = Factory.o13AttachmentTypeBL.Load(BO.BAS.IsNullInt(o13ID.SelectedValue))
        If cO13 Is Nothing Then
            lblError.Text = "Na vstupu chybí typ dokumentu!"
            Return
        End If

        For Each validFile As UploadedFile In upload1.UploadedFiles
            Dim strValidFileName As String = GetValidArchiveFileName(validFile.FileName)
            'strValidFileName = BO.BAS.RemoveDiacritism(strValidFileName)


            Dim strFileName As String = Me.GUID & "_" & strValidFileName
            If Not cO13.o13IsUniqueArchiveFileName Then
                strFileName = strValidFileName  'název archivního souboru nemá být jedinečný
            End If
            Try
                validFile.SaveAs(strDir & "\" & strFileName, True)
            Catch ex As Exception
                log4net.LogManager.GetLogger("debuglog").Debug("TEMP upload cesta: " & strDir & "\" & strFileName & vbCrLf & "Chyba: " & ex.Message)
                Response.Write(ex.Message)
                Return
            End Try

            If Me.MaxFileUploadedCount > 0 Then
                'test maximálního počtu nahraných souborů
                Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Factory.p85TempBoxBL.GetList(Me.GUID)
                If lisTemp.Count >= Me.MaxFileUploadedCount Then
                    RaiseEvent ErrorUpload(strFileName, "Maximální počet nahratelných souborů je v tomto případě: " & Me.MaxFileUploadedCount.ToString)
                    Return
                End If
            End If

            Dim cRec As New BO.p85TempBox()
            cRec.p85GUID = Me.GUID
            cRec.p85Prefix = "o27"
            cRec.p85OtherKey1 = cO13.PID
            cRec.p85FreeText06 = cO13.o13Name

            cRec.p85FreeText01 = GetValidArchiveFileName(validFile.FileName)
            cRec.p85FreeText02 = strFileName
            cRec.p85FreeText03 = validFile.ContentType
            cRec.p85FreeText04 = validFile.GetFieldValue("Title")
            cRec.p85FreeNumber01 = validFile.ContentLength

            Factory.p85TempBoxBL.Save(cRec)

            RaiseEvent AfterUploadOneFile(strDir & "\" & cRec.p85FreeText01, "")
            bolOK = True
        Next
        For Each invalidFile As UploadedFile In upload1.InvalidFiles
            lblError.Text = "Soubor [" & invalidFile.FileName & "] není povolen nahrát na server."
            RaiseEvent ErrorUpload(invalidFile.FileName, lblError.Text)
        Next
        upload1.UploadedFiles.Clear()

        If bolOK Then RaiseEvent AfterUploadAll()
    End Sub


    Private Sub cmdUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpload.Click
        TryUploadFiles2Temp()
    End Sub

   

End Class