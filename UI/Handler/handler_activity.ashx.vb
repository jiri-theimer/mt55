Imports System.Web
Imports System.Web.Services

Public Class handler_activity
    Implements System.Web.IHttpHandler
    Private _ret As BO.ActivityHandler
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        'context.Response.ContentType = "text/plain"
        context.Response.ContentType = "application/json"
        _ret = New BO.ActivityHandler


        Dim intP32ID As Integer = BO.BAS.IsNullInt(context.Request.Item("pid"))
        Dim intP41ID As Integer = BO.BAS.IsNullInt(context.Request.Item("p41id"))
        Dim intJ27ID As Integer = BO.BAS.IsNullInt(context.Request.Item("j27id"))
        
        If intP32ID = 0 Then
            _ret.ErrorMessage = "p32id is missing"
            RR(context) : Return            
        End If
        Dim factory As BL.Factory = Nothing

        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            _ret.ErrorMessage = "factory is nothing"
            RR(context) : Return
        End If
        Dim cRec As BO.p32Activity = factory.p32ActivityBL.Load(intP32ID)
        _ret.IsTextRequired = cRec.p32IsTextRequired
        _ret.Default_p31Value = cRec.p32Value_Default
        If _ret.Default_p31Value <> 0 Then
            _ret.Default_p31Value_String = cRec.p32Value_Default.ToString
        End If
        _ret.p32ManualFeeFlag = cRec.p32ManualFeeFlag
        _ret.p32ManualFeeDefAmount = cRec.p32ManualFeeDefAmount

        If cRec Is Nothing Then
            _ret.ErrorMessage = "cRec is missing"
            RR(context) : Return
        End If
        Dim cP34 As BO.p34ActivityGroup = factory.p34ActivityGroupBL.Load(cRec.p34ID)

        Dim cP41 As BO.p41Project = Nothing
        _ret.Default_p31Text = cRec.p32DefaultWorksheetText
        If intP41ID > 0 Then
            cP41 = factory.p41ProjectBL.Load(intP41ID)
            Dim intP87ID As Integer = cP41.p87ID_Client 'fakturační jazyk klienta projektu
            If cP41.p87ID > 0 Then intP87ID = cP41.p87ID 'fakturační jazyk projektu má přednost
            Select Case intP87ID
                Case 1 : _ret.Default_p31Text = cRec.p32DefaultWorksheetText_Lang1
                Case 2 : _ret.Default_p31Text = cRec.p32DefaultWorksheetText_Lang2
                Case 3 : _ret.Default_p31Text = cRec.p32DefaultWorksheetText_Lang3
                Case 4 : _ret.Default_p31Text = cRec.p32DefaultWorksheetText_Lang4
            End Select
            If _ret.Default_p31Text <> "" Then
                If _ret.Default_p31Text.IndexOf("[%") >= 0 Then
                    've výchozím popisu aktivity jsou slučovací pole z projektu
                    Dim matches As MatchCollection = Regex.Matches(_ret.Default_p31Text, "\[%.*?\%]")
                    For Each m As Match In matches
                        Dim strField As String = Replace(m.Value, "[%", "").Replace("%]", "")
                        _ret.Default_p31Text = Replace(_ret.Default_p31Text, m.Value, BO.BAS.GetPropertyValue(cP41, strField))
                    Next
                End If
            End If
        End If

        If cP34.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
            If cRec.x15ID > BO.x15IdEnum.Nic Then
                If intJ27ID = 0 Then
                    intJ27ID = BO.BAS.IsNullInt(factory.x35GlobalParam.GetValueString("j27ID_Domestic"))
                End If
                Dim lisP53 As IEnumerable(Of BO.p53VatRate) = factory.p53VatRateBL.GetList(New BO.myQuery).Where(Function(p) p.j27ID = intJ27ID And p.x15ID = cRec.x15ID)
                If lisP53.Count > 0 Then
                    _ret.DefaultVatRate = lisP53(0).p53Value
                    _ret.IsDefaultVatRate = True
                End If
            End If
        End If

        RR(context)
        
    End Sub

    Private Sub RR(context As HttpContext)
        Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer
        Dim s As String = jss.Serialize(_ret)
        context.Response.Write(s)
    End Sub


    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class