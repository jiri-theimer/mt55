Public Class basSys
    Public Shared Sub SetupCloudBdsCombo(cbx As DropDownList)
        If System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\Plugins\cloud_dbs.licx") Then
            Dim cF As New BO.clsFile
            Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(cF.GetFileContents(BO.ASS.GetApplicationRootFolder & "\Plugins\cloud_dbs.licx"), vbCrLf)
            If lis.Count > 0 Then
                For Each s In lis
                    cbx.Items.Add(s)
                Next
            End If
        End If
    End Sub

    Public Shared Function GetCloudBdsList() As List(Of String)
        Dim cF As New BO.clsFile
        Return BO.BAS.ConvertDelimitedString2List(cF.GetFileContents(BO.ASS.GetApplicationRootFolder & "\Plugins\cloud_dbs.licx"), vbCrLf)

    End Function

End Class
