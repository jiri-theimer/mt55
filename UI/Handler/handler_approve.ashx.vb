Imports System.Web
Imports System.Web.Services

Public Class handler_approve
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

        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(context.Request.Item("pids"))
        Dim p72ID As BO.p72IdENUM = CType(BO.BAS.IsNullInt(context.Request.Item("p72id")), BO.p72IdENUM)

        If Not ValidateBefore(context, factory, pids) Then Return

        Dim errs As New List(Of String), x As Integer = 0

        For Each intPID As Integer In pids
            x += 1
            Dim cRec As BO.p31Worksheet = factory.p31WorksheetBL.Load(intPID)

            Dim cA As New BO.p31WorksheetApproveInput(intPID, cRec.p33ID)
            With cA
                .p31Date = cRec.p31Date
                .p71id = BO.p71IdENUM.Schvaleno
                .p72id = p72ID
                .p31ApprovingSet = cRec.p31ApprovingSet
                .p32ID = cRec.p32ID
                .p32ManualFeeFlag = cRec.p32ManualFeeFlag


                If p72ID = BO.p72IdENUM.Fakturovat Or p72ID = BO.p72IdENUM.FakturovatPozdeji Then
                    Select Case cRec.p33ID
                        Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                            .Rate_Billing_Approved = cRec.p31Rate_Billing_Orig
                            If cRec.p32ManualFeeFlag = 1 Then
                                .ManualFee_Approved = cRec.p31Amount_WithoutVat_Orig
                            End If
                    End Select
                    .Value_Approved_Billing = cRec.p31Value_Orig

                End If
                ''If Me.chkUseInternalApproving.Checked And (cRec.p33ID = BO.p33IdENUM.Cas Or cRec.p33ID = BO.p33IdENUM.Kusovnik) Then
                ''    'interní schvalování
                If .Value_Approved_Internal = 0 And .Rate_Internal_Approved = 0 Then

                    .Value_Approved_Internal = cRec.p31Value_Orig
                    .Rate_Internal_Approved = cRec.p31Rate_Internal_Orig
                End If
                ''End If
            End With
            If cA.p72id > BO.p72IdENUM._NotSpecified Then
                If Not factory.p31WorksheetBL.Save_Approving(cA, False) Then
                    errs.Add("#" & x.ToString & "/" & factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, intPID) & ":\n" & factory.p31WorksheetBL.ErrorMessage)
                End If
            Else
                Dim xx As New List(Of Integer)  'vyčistit schvalování
                xx.Add(intPID)
                If Not factory.p31WorksheetBL.RemoveFromApproving(xx) Then
                    errs.Add("#" & x.ToString & "/" & factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, intPID) & ":\n" & factory.p31WorksheetBL.ErrorMessage)
                End If
            End If
            
        Next
        If errs.Count = 0 Then
            context.Response.Write("1") 'vše schváleno
        Else
            If errs.Count = x Then
                context.Response.Write("_" & String.Join("\n", errs)) 'chyby v každém záznamu
            Else
                context.Response.Write(String.Join("\n", errs))
            End If

        End If
    End Sub

    Private Function ValidateBefore(context As HttpContext, factory As BL.Factory, pids As List(Of Integer)) As Boolean
        Dim x As Integer = 0
        For Each intPID As Integer In pids
            x += 1
            Dim cRec As BO.p31Worksheet = factory.p31WorksheetBL.Load(intPID)

            Dim cDisp As BO.p31WorksheetDisposition = factory.p31WorksheetBL.InhaleRecordDisposition(intPID)
            If cDisp.RecordDisposition = BO.p31RecordDisposition._NoAccess Then
                context.Response.Write(String.Format("_#{0}: K úkonu nemáte přístup:\n{1}.", x, factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, intPID))) : Return False
            End If
            If cRec.p91ID <> 0 Then
                context.Response.Write(String.Format("_#{0}: Úkon již byl vyfakturován:\n{1}.", x, factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, intPID))) : Return False
            End If
            If Not (cDisp.RecordDisposition = BO.p31RecordDisposition.CanApprove Or cDisp.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit) Then
                context.Response.Write(String.Format("_#{0}: Nemáte oprávnění schvalovat úkon:\n{1}.", x, factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, intPID))) : Return False
            End If
        Next
        Return True
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class