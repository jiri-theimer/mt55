Imports System.IO


Public Class binaryfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim factory As BL.Factory = Nothing

            If HttpContext.Current.User.Identity.IsAuthenticated Then
                Dim strLogin As String = HttpContext.Current.User.Identity.Name
                factory = New BL.Factory(, strLogin)
            Else
                Response.Write("Nelze ověřit přihlášeného uživatele.")
                Return
            End If


            Dim strFullPath As String = "", strDestFileName As String = "", strContentType As String = "", strDisposition As String = "attachment", strPdfGuid As String = ""
            If Request.Item("disposition") <> "" Then strDisposition = Request.Item("disposition")

            If Request.Item("tempfile") <> "" Then
                strFullPath = factory.x35GlobalParam.TempFolder & "\" & Request.Item("tempfile")
                strDestFileName = Request.Item("tempfile")
                strContentType = (New BO.clsFile).GetContentType(strFullPath)
            End If
            If Request.Item("uploadedfile") <> "" Then
                strFullPath = factory.x35GlobalParam.UploadFolder & "\" & Request.Item("uploadedfile")
                strDestFileName = Request.Item("uploadedfile")
                strContentType = (New BO.clsFile).GetContentType(strFullPath)
            End If
            If Request.Item("prefix") <> "" And (Request.Item("pid") <> "" Or Request.Item("guid") <> "") Then
                Select Case Request.Item("prefix")
                    Case "o27"
                        Dim cRec As BO.o27Attachment = factory.o27AttachmentBL.Load(BO.BAS.IsNullInt(Request.Item("pid")))
                        If cRec Is Nothing Then
                            Response.Write("Nelze načíst záznam dokumentu.")
                            Return
                        End If
                        strPdfGuid = cRec.o27GUID
                        strContentType = cRec.o27ContentType
                        strFullPath = factory.x35GlobalParam.UploadFolder & "\" & cRec.o27ArchiveFolder & "\" & cRec.o27ArchiveFileName
                        strDestFileName = cRec.o27OriginalFileName
                    Case "p85"
                        Dim cRec As BO.p85TempBox = factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(Request.Item("pid")))
                        If cRec Is Nothing Then
                            Response.Write("Nelze načíst TEMP záznam dokumentu.")
                            Return
                        End If
                        strPdfGuid = cRec.p85GUID
                        If cRec.p85FreeText03 <> "" Then strContentType = cRec.p85FreeText03
                        strFullPath = factory.x35GlobalParam.TempFolder & "\" & cRec.p85FreeText02
                        strDestFileName = cRec.p85FreeText01
                    Case "o43"
                        Dim cRec As BO.o43ImapRobotHistory = Nothing
                        If Request.Item("guid") <> "" Then cRec = factory.o42ImapRuleBL.LoadHistoryByRecordGUID(Request.Item("guid"))
                        If Request.Item("pid") <> "" Then cRec = factory.o42ImapRuleBL.LoadHistoryByID(BO.BAS.IsNullInt(Request.Item("pid")))
                        If cRec Is Nothing Then
                            Response.Write("Nelze načíst IMAP záznam.")
                            Return
                        End If
                        strPdfGuid = cRec.o43RecordGUID
                        If Request.Item("att") <> "" Then
                            'dotaz na jednu z externích příloh z poštovní zprávy
                            strFullPath = factory.x35GlobalParam.UploadFolder & "\IMAP\" & cRec.o43ImapArchiveFolder
                            strFullPath += "\" & cRec.o43RecordGUID & "_" & Request.Item("att")
                            strDestFileName = Request.Item("att")
                        Else
                            strFullPath = factory.x35GlobalParam.UploadFolder & "\IMAP\" & cRec.o43ImapArchiveFolder
                            If Request.Item("format") = "msg" Then strFullPath += "\" & cRec.o43MsgFileName
                            If Request.Item("format") = "eml" Then strFullPath += "\" & cRec.o43EmlFileName

                            strDestFileName = ""
                            If cRec.p56ID <> 0 Then strDestFileName = factory.GetRecordCaption(BO.x29IdEnum.p56Task, cRec.p56ID)
                            If cRec.o23ID <> 0 Then strDestFileName = factory.GetRecordCaption(BO.x29IdEnum.o23Doc, cRec.o23ID)
                            strDestFileName = Left(BO.BAS.Prepare4FileName(strDestFileName), 80) & "." & Request.Item("format")
                        End If
                    Case "x40-eml", "x40-msg"
                        strContentType = "message/rfc822"
                        Dim cRec As BO.x40MailQueue = factory.x40MailQueueBL.Load(BO.BAS.IsNullInt(Request.Item("pid")))
                        strFullPath = factory.x35GlobalParam.UploadFolder & "\" & cRec.x40ArchiveFolder & "\" & cRec.x40MessageID & ".eml"
                        If Request.Item("prefix") = "x40-msg" Then
                            Dim strMSG As String = factory.x35GlobalParam.UploadFolder & "\" & cRec.x40ArchiveFolder & "\" & cRec.x40MessageID & ".msg"
                            If Not System.IO.File.Exists(strMSG) Then
                                Dim mail As New Rebex.Mail.MailMessage
                                mail.Load(strFullPath)
                                mail.Save(strMSG, Rebex.Mail.MailFormat.OutlookMsg)
                                strFullPath = strMSG
                                strContentType = "application/vnd.ms-outlook"
                            End If
                        End If
                        If Request.Item("att") <> "" Then
                            Dim mail As New Rebex.Mail.MailMessage
                            mail.Load(strFullPath)
                            mail.Attachments.First(Function(p) p.FileName = Request.Item("att")).Save(factory.x35GlobalParam.TempFolder & "\" & Request.Item("att"))
                            strFullPath = factory.x35GlobalParam.TempFolder & "\" & Request.Item("att")
                        End If

                        strDisposition = "inline"
                End Select

                If Request.Item("doc2pdf") = "1" Then
                    If strPdfGuid = "" Then strPdfGuid = BO.BAS.GetGUID
                    Dim strDest As String = factory.x35GlobalParam.TempFolder & "\" & strPdfGuid & ".pdf", bolPDF As Boolean = False
                    If System.IO.File.Exists(strDest) Then
                        bolPDF = True
                    Else
                        bolPDF = ConvertDoc2Pdf(strFullPath, strDest)
                    End If
                    If bolPDF Then
                        strFullPath = strDest
                        strDestFileName = strPdfGuid & ".pdf"
                        strContentType = "application/pdf"
                    End If
                End If
            End If

            strDestFileName = GetValidArchiveFileName(strDestFileName)

            FlushFile(strFullPath, strContentType, strDestFileName, strDisposition)
        End If
    End Sub

    Private Function ConvertDoc2Pdf(strSourcePath As String, strDestPath As String) As Boolean
        Dim strLicFile As String = BO.ASS.GetApplicationRootFolder() & "\bin\ZI.dll"
        Dim license As New Aspose.Words.License()
        license.SetLicense(strLicFile)

        Dim doc As New Aspose.Words.Document(strSourcePath)

        doc.Save(strDestPath, Aspose.Words.SaveFormat.Pdf)
        Return True
    End Function
    Private Function GetValidArchiveFileName(strFileName As String) As String
        Dim invalidChars As String = Regex.Escape(New String(System.IO.Path.GetInvalidFileNameChars()))
        Dim invalidReStr As String = String.Format("[{0}]+", invalidChars)
        Return Regex.Replace(strFileName, invalidReStr, "_").Replace(";", "").Replace(",", "")

    End Function



    Private Sub FlushFile(ByVal strPath As String, Optional ByVal strContentType As String = "", Optional ByVal strFileName As String = "", Optional ByVal strDisposition As String = "attachment")
        Dim reader As BinaryReader, sr As StreamReader
        If File.Exists(strPath) Then
            Dim fi As FileInfo = New FileInfo(strPath)

            sr = New StreamReader(strPath)
            reader = New BinaryReader(sr.BaseStream)

            With Me.Response
                If strContentType <> "" Then
                    .AddHeader("Content-Type", strContentType)
                Else
                    .AddHeader("Content-Type", "application/" & fi.Extension)
                End If
                .AddHeader("Content-Length", fi.Length)
                If strFileName = "" Or strFileName.IndexOf("\") >= 0 Then strFileName = fi.Name
                '.AddHeader("Content-Disposition", strDisposition & ";size=" & fi.Length & ";filename=" & strFileName)

                'Dim cdHeader As System.Net.Http.Headers.ContentDispositionHeaderValue = New System.Net.Http.Headers.ContentDispositionHeaderValue(strDisposition)
                'cdHeader.FileNameStar = strFileName
                'cdHeader.Size = fi.Length
                '.AddHeader("Content-Disposition", cdHeader.ToString())
                .AddHeader("X-Content-Type-Options", "nosniff")

                If strFileName = "" Or strFileName.IndexOf("\") >= 0 Then strFileName = fi.Name
                .AddHeader("Content-Disposition", strDisposition & ";size=" & fi.Length & ";filename=" & strFileName)
                .BinaryWrite(reader.ReadBytes(reader.BaseStream.Length()))




            End With
            reader.Close()
        Else

            Response.Write("Soubor '" & strFileName & "' neexistuje!")
            Response.Write("<hr><button onclick='history.back();' type='button'>Jít zpět</button>")
        End If
    End Sub
End Class