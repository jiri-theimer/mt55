Public Enum NotifyLevel
    InfoMessage = 0
    WarningMessage = 1
    ErrorMessage = 2
End Enum
Public Class basUI
    Public Shared Function ColorQueryRGB() As System.Drawing.Color
        Return System.Drawing.Color.FromArgb(255, 128, 128)
    End Function
    Public Shared Function ColorQueryCSS() As String
        Return "#ff8080"
    End Function

    Public Shared Function ParseRefUri(pageuri As System.Uri) As String
        If pageuri Is Nothing Then Return ""
        Dim s As String = VirtualPathUtility.ToAbsolute("~/")
        Dim strReferrerURI As String = pageuri.PathAndQuery.Remove(0, s.Length - 1)

        If strReferrerURI = "" Then Return ""
        If Left(strReferrerURI, 1) = "/" Then strReferrerURI = Right(strReferrerURI, Len(strReferrerURI) - 1)
        Return strReferrerURI
    End Function
    Public Shared Function AddQuerystring2Page(strPage As String, strAddQuerystring As String) As String
        'do aspx stránky strPage přidat parametr strAddQuerystring
        If strPage.IndexOf("?") >= 0 Then
            Return strPage & "&" & strAddQuerystring
        Else
            Return strPage & "?" & strAddQuerystring
        End If
    End Function
    Public Shared Function GetCompleteQuerystring(p As HttpRequest, Optional bolOtaznikNapred As Boolean = False) As String
        Dim s As String = ""
        With p.QueryString
            For i As Integer = 0 To .Count - 1
                If Trim(.Item(i)) <> "" Then
                    If s = "" Then
                        s = .GetKey(i) & "=" & .Item(i)
                    Else
                        s += "&" & .GetKey(i) & "=" & .Item(i)
                    End If
                End If
            Next
        End With
        If s = "" Then
            Return ""
        Else
            If bolOtaznikNapred Then
                Return "?" & s
            Else
                Return s
            End If
        End If
    End Function

    Public Shared Function GetCheckedItems(chkl As CheckBoxList) As List(Of Integer)
        Dim lis As New List(Of Integer)
        For Each it As ListItem In chkl.Items
            If it.Selected And IsNumeric(it.Value) Then lis.Add(CInt(it.Value))
        Next
        Return lis
    End Function
    Public Shared Function GetCheckedItemsString(chkl As CheckBoxList) As List(Of String)
        Dim lis As New List(Of String)
        For Each it As ListItem In chkl.Items
            If it.Selected Then lis.Add(it.Value)
        Next

        Return lis
    End Function
    Public Shared Sub SelectDropdownlistValue(cbx As DropDownList, strValue As String)
        Try
            cbx.SelectedValue = strValue
        Catch ex As Exception

        End Try
    End Sub
    Public Shared Sub SelectRadiolistValue(opg As RadioButtonList, strValue As String)
        Try
            opg.SelectedValue = strValue
        Catch ex As Exception

        End Try
    End Sub

    Public Overloads Shared Sub CheckItems(chkl As CheckBoxList, ids As List(Of Integer))
        For Each i As Integer In ids
            If Not chkl.Items.FindByValue(i.ToString) Is Nothing Then
                chkl.Items.FindByValue(i.ToString).Selected = True
            End If
        Next
    End Sub
    Public Overloads Shared Sub CheckItems(chkl As CheckBoxList, ids As List(Of String))
        For Each s As String In ids
            If Not chkl.Items.FindByValue(s) Is Nothing Then
                chkl.Items.FindByValue(s).Selected = True
            End If
        Next
    End Sub
    Public Shared Sub CheckUnheckAllItems(chkl As CheckBoxList, bolCheck As Boolean)
        For Each it As ListItem In chkl.Items
            it.Selected = bolCheck
        Next
    End Sub

    Public Shared Sub NotifyMessage(notifyControl As Telerik.Web.UI.RadNotification, ByVal strText As String, Optional ByVal msgLevel As NotifyLevel = NotifyLevel.InfoMessage, Optional ByVal strTitle As String = "")
        With notifyControl
            .Text = strText
            Select Case msgLevel
                Case NotifyLevel.InfoMessage : .ContentIcon = "info" : .Title = "Info"
                Case NotifyLevel.WarningMessage : .ContentIcon = "warning" : .Title = "Varování"
                Case NotifyLevel.ErrorMessage : .ContentIcon = "delete" : .Title = "Chyba"
            End Select
            .TitleIcon = ""
            If strTitle <> "" Then .Title = strTitle
            .Show()
        End With
    End Sub
    Public Overloads Shared Sub AddToolbarButton(toolbarControl As Telerik.Web.UI.RadToolBar, c As clsToolBarButton)
        Dim b As New Telerik.Web.UI.RadToolBarButton
        With c
            b.Text = .Text
            b.Target = .Target
            b.Value = .Value
            If .ImageURL > "" Then b.ImageUrl = .ImageURL
            b.PostBack = .AutoPostback
            If .AutoPostback Then
                b.Attributes.Add("postback", "1")
            Else
                b.Attributes.Add("postback", "0")
            End If
            If .ShowLoading Then
                b.Attributes.Add("showloading", "1")
            Else
                b.Attributes.Add("showloading", "0")
            End If
            b.NavigateUrl = .NavigateURL
            If .Index > toolbarControl.Items.Count - 1 Then .Index = toolbarControl.Items.Count - 1

            If .GroupText <> "" Then
                Dim sp As Telerik.Web.UI.RadToolBarSplitButton = toolbarControl.FindItemByText(.GroupText)
                If sp Is Nothing Then
                    sp = New Telerik.Web.UI.RadToolBarSplitButton(.GroupText)
                    toolbarControl.Items.Insert(.Index, sp)
                End If
                sp.Buttons.Add(b)

            Else
                toolbarControl.Items.Insert(.Index, b)
            End If

        End With

    End Sub
    Public Overloads Shared Sub AddToolbarButton(toolbarControl As Telerik.Web.UI.RadToolBar, ByVal strText As String, ByVal strValue As String, Optional ByVal Index As Integer = 0, Optional ByVal strImageURL As String = "", Optional ByVal bolPostBack As Boolean = True, Optional ByVal strNavigateURL As String = "", Optional strTarget As String = "", Optional bolShowLoading As Boolean = False)
        Dim c As New clsToolBarButton(strText, strValue)
        With c
            .Index = Index
            .ImageURL = strImageURL
            .AutoPostback = bolPostBack
            .NavigateURL = strNavigateURL
            .Target = strTarget
            .ShowLoading = bolShowLoading
        End With
        AddToolbarButton(toolbarControl, c)
    End Sub
    Public Shared Sub HideShowToolbarButton(toolbarControl As Telerik.Web.UI.RadToolBar, ByVal strButtonValue As String, ByVal bolVisible As Boolean)
        Try
            toolbarControl.FindItemByValue(strButtonValue).Style.Item("display") = BO.BAS.GB_Display(bolVisible)
        Catch ex As Exception

        End Try
        If Not toolbarControl.FindItemByValue("sep_" & strButtonValue) Is Nothing Then
            toolbarControl.FindItemByValue("sep_" & strButtonValue).Style.Item("display") = BO.BAS.GB_Display(bolVisible)
        End If
    End Sub
    Public Shared Sub RenameToolbarButton(toolbarControl As Telerik.Web.UI.RadToolBar, ByVal strButtonValue As String, ByVal strNewName As String)
        Try
            toolbarControl.FindItemByValue(strButtonValue).Text = strNewName
        Catch ex As Exception
        End Try
    End Sub
    Public Shared Sub ChangeToolbarButtonAttribute(toolbarControl As Telerik.Web.UI.RadToolBar, ByVal strButtonValue As String, ByVal strAttribute As String, strAttributeValue As String)
        Try
            toolbarControl.FindItemByValue(strButtonValue).Attributes.Item(strAttribute) = strAttributeValue
        Catch ex As Exception
        End Try
    End Sub


    Public Shared Sub SetupP87Combo(_factory As BL.Factory, cbx As UI.datacombo)
        Dim lisP87 As IEnumerable(Of BO.p87BillingLanguage) = _factory.ftBL.GetList_P87()
        cbx.DataSource = lisP87
        cbx.DataBind()
        For Each c In lisP87
            If c.p87Icon <> "" Then
                cbx.RadCombo.FindItemByValue(c.PID.ToString).ImageUrl = "Images/flags/" & c.p87Icon
            End If
        Next
    End Sub

    Public Shared Function GetColorFromPicker(picker As Telerik.Web.UI.RadColorPicker) As String
        With picker
            If Not .SelectedColor.IsEmpty Then
                If .SelectedColor.Name.IndexOf("#") = -1 Then
                    Return "#" & Right(.SelectedColor.Name, .SelectedColor.Name.Length - 2)
                Else
                    Return .SelectedColor.Name
                End If
            Else
                Return ""
            End If
        End With

    End Function
    Public Shared Sub SetColorToPicker(picker As Telerik.Web.UI.RadColorPicker, strColor As String)
        If strColor <> "" And Left(strColor, 2) <> "ff" Then
            picker.SelectedColor = Drawing.Color.FromName(strColor)
        Else
            picker.SelectedColor = Nothing
        End If
    End Sub

    Public Shared Function DetectIfMobileDefice(r As HttpRequest) As Boolean
        Dim u As String = r.ServerVariables("HTTP_USER_AGENT")
        Dim b As New Regex("(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase)
        Dim v As New Regex("1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase)
        If b.IsMatch(u) Or v.IsMatch(Left(u, 4)) Then
            'detekováno mobilní zařízení
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function GetPageUrlPerSAW(r As HttpRequest, strDataPrefix As String) As String
        If GetCookieValue(r, "MT50-SAW") = "1" Then
            Dim s As String = GetCompleteQuerystring(r, True)
            If s.IndexOf("pid=") > 0 And s.IndexOf("masterprefix") = -1 Then
                Return strDataPrefix & "_framework_detail.aspx" & s & "&source=3"
            Else
                Return "entity_framework.aspx?prefix=" & strDataPrefix & "&" & GetCompleteQuerystring(r)
            End If
        Else
            Return "entity_framework.aspx?prefix=" & strDataPrefix & "&" & GetCompleteQuerystring(r)
        End If
    End Function
    Public Shared Function GetCookieValue(r As HttpRequest, strCookie As String) As String
        Dim cook As HttpCookie = r.Cookies(strCookie)
        If Not cook Is Nothing Then
            Return cook.Value
        Else
            Return ""
        End If
    End Function


    Public Shared Sub Write2AccessLog(factory As BL.Factory, bolAllowLogIn As Boolean, r As System.Web.HttpRequest, Optional screenWidth As String = "", Optional screenHeight As String = "", Optional strAppClient As String = "")
        Dim cLog As New BO.j90LoginAccessLog
        With cLog
            .j90ClientBrowser = DetectBrowser(r)
            .j90Platform = r.Browser.Platform
            .j90IsMobileDevice = DetectIfMobileDefice(r)
            If .j90IsMobileDevice Then
                .j90MobileDevice = r.Browser.MobileDeviceManufacturer & "/" & r.Browser.MobileDeviceModel
            End If
            If screenWidth <> "" Then
                .j90ScreenPixelsHeight = BO.BAS.IsNullInt(screenHeight)
                .j90ScreenPixelsWidth = BO.BAS.IsNullInt(screenWidth)
            End If
            .j90UserHostAddress = r.UserHostAddress
            .j90UserHostName = r.UserHostName
            .j90RequestURL = r.Url.ToString
            .j90AppClient = strAppClient

        End With
        factory.j03UserBL.AppendAccessLog(factory.SysUser.PID, cLog)
    End Sub
    Public Shared Sub PingAccessLog(factory As BL.Factory, r As System.Web.HttpRequest)
        With factory.SysUser
            If .j03Ping_TimeStamp Is Nothing Then .j03Ping_TimeStamp = Today
            If DateDiff(DateInterval.Second, CDate(.j03Ping_TimeStamp), Now) > 100 Then Write2AccessLog(factory, True, r)
        End With
    End Sub

    Private Shared Function DetectBrowser(r As HttpRequest) As String
        If r.UserAgent.IndexOf("Edge/") > 0 Then
            Return "Edge | " & r.UserAgent
        End If
        With r.Browser
            'Return .Browser & " | " & .Type & ", version " & .Version & " | " & r.Browser.Browser
            Return .Browser & "/" & .Type & " | " & r.UserAgent
        End With
        ''Dim s As String = ""
        ''With r.Browser
        ''    's &= "Browser Capabilities" & vbCrLf
        ''    s &= .Type & vbCrLf
        ''    s &= "Name = " & .Browser & vbCrLf
        ''    s &= "Version = " & .Version & vbCrLf
        ''    s &= "Major Version = " & .MajorVersion & vbCrLf
        ''    s &= "Minor Version = " & .MinorVersion & vbCrLf
        ''    s &= "Platform = " & .Platform & vbCrLf
        ''    s &= "Is Beta = " & .Beta & vbCrLf
        ''    's &= "Is Crawler = " & .Crawler & vbCrLf
        ''    's &= "Is AOL = " & .AOL & vbCrLf
        ''    's &= "Is Win16 = " & .Win16 & vbCrLf
        ''    's &= "Is Win32 = " & .Win32 & vbCrLf
        ''    s &= "Supports Frames = " & .Frames & vbCrLf
        ''    's &= "Supports Tables = " & .Tables & vbCrLf
        ''    s &= "Supports Cookies = " & .Cookies & vbCrLf
        ''End With
        ''Return s

    End Function

    Public Shared Function Get_Uploaded_Files(upl1 As Telerik.Web.UI.RadUpload, strDestFolder As String) As List(Of String)
        'vrací seznam uploadnutých souborů do složky strDestFolder
        Dim lis As New List(Of String)
        For Each validFile As Telerik.Web.UI.UploadedFile In upl1.UploadedFiles
            validFile.SaveAs(strDestFolder & "\" & validFile.FileName, True)
            lis.Add(strDestFolder & "\" & validFile.FileName)

        Next
        Return lis
    End Function

End Class
