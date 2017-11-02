Imports System.Web
Imports System.Web.Services

Public Class handler_time
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"

        Dim factory As BL.Factory = Nothing

        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            context.Response.Write(" ")
            Return
        End If

        Dim strOperation As String = Trim(context.Request.Item("oper"))
        Dim cT As New BO.clsTime, strRetMsg As String = " "

        Select Case strOperation
            Case "duration"
                Dim t1 As Integer = cT.ConvertTimeToSeconds(Trim(context.Request.Item("t1")))
                Dim t2 As Integer = cT.ConvertTimeToSeconds(Trim(context.Request.Item("t2")))
                If t2 >= 24 * 60 * 60 Then
                    t2 = cT.ConvertTimeToSeconds("23:59")
                    strRetMsg = "[Konec] musí být menší než 24:00."
                End If
                If t1 > t2 Then
                    strRetMsg = "[Konec] musí být větší než [Začátek]."
                End If
                If t1 < 0 Then
                    t1 = cT.ConvertTimeToSeconds("01:00")
                    strRetMsg = "[Začátek] musí být kladný čas."
                End If


                Dim intDur As Integer = t2 - t1
                If intDur < 0 Then
                    context.Response.Write(strRetMsg)
                    Return
                End If
                context.Response.Write(cT.GetTimeFromSeconds(t1) & "|" & cT.GetTimeFromSeconds(t2) & "|" & cT.GetTimeFromSeconds(intDur) & "|" & strRetMsg)
            Case "hours", "minutes"
                Dim strT As String = Trim(context.Request.Item("hours"))
                
                Dim seconds As Integer = 0, strH As String = "[Hodiny]"
                If strOperation = "minutes" Then
                    strH = "[Minuty]"
                    If Not IsNumeric(strT) Then
                        context.Response.Write(strH & " nejsou zadané ve správném formátu.")
                        Return
                    End If
                    seconds = BO.BAS.IsNullInt(strT) * 60
                Else
                    If Not IsNumeric(strT.Replace(":", "")) Then
                        context.Response.Write(strH & " nejsou zadané ve správném formátu.")
                        Return
                    End If
                    seconds = cT.ConvertTimeToSeconds(strT)
                End If
                If seconds = 0 Then
                    context.Response.Write(strH & " nesmí být NULA.")
                    Return
                End If
                If strT.IndexOf(":") <= 0 Then
                    strRetMsg = cT.ShowAsDec(CDbl(seconds) / 60 / 60).ToString & "h. (dekadicky) -> " & cT.GetTimeFromSeconds(seconds, True) & " (HH:mm:ss)"
                Else
                    strRetMsg = cT.GetTimeFromSeconds(seconds) & " (HH:mm) -> " & cT.ShowAsDec(CDbl(seconds) / 60 / 60).ToString & "h. (dekadicky)"
                End If
                

                Dim round2minutes As Integer = factory.x35GlobalParam.GetValueString("Round2Minutes")
                Dim seconds_rounded As Integer = cT.RoundSeconds(seconds, round2minutes * 60)
                If seconds <> seconds_rounded Then
                    strRetMsg = strRetMsg & ", po " & round2minutes.ToString & "ti minutovém zaokrouhlení systém uloží: " & cT.GetTimeFromSeconds(seconds_rounded) & " (" & cT.GetDecTimeFromSeconds(seconds_rounded).ToString & ")."
                End If
                context.Response.Write(strRetMsg)
        End Select

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class