Public Class entity_worksheet_summary
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    ''Public Sub DisableApprovingButton()
    ''    cmdApproving.NavigateUrl = ""
    ''    cmdApproving.Enabled = False
    ''End Sub
    
    Public Sub RefreshData(cWorksheetSum As BO.p31WorksheetSum, strPrefix As String, intRecordPID As Integer, bolAllowHonorar As Boolean, dblLimitHours_Notification As Double, dblLImitFee_Notification As Double)
        If Not bolAllowHonorar Then dblLImitFee_Notification = 0
        With cWorksheetSum
            Me.p31Hours_Orig.Text = BO.BAS.FN(.p31Hours_Orig)
            Dim b As Boolean = False
            If .WaitingOnApproval_Hours_Count > 0 Then
                Me.WaitingOnApproval_Hours_Sum.Text = "<span class='badgebox1red'>" & .WaitingOnApproval_Hours_Count.ToString & "x</span> " & BO.BAS.FN(.WaitingOnApproval_Hours_Sum)

                ''If Me.Factory.SysUser.IsApprovingPerson Then
                ''    'cmdApproving.NavigateUrl = "entity_framework_detail_approving.aspx?prefix=" & strPrefix & "&pid=" & intRecordPID.ToString
                ''    cmdApproving.NavigateUrl = "javascript:approve()"
                ''End If

                b = True


            End If
            If dblLimitHours_Notification > 0 Then
                trLimitHours.Visible = True
                p41LimitHours_Notification.Text = BO.BAS.FNI(dblLimitHours_Notification)
                If .WaitingOnApproval_Hours_Count > 0 Then
                    Me.Perc1.Text = CInt(100 * .WaitingOnApproval_Hours_Sum / dblLimitHours_Notification).ToString & "%"
                    If dblLimitHours_Notification < .WaitingOnApproval_Hours_Sum Then
                        'zvýraznit překročení limitu
                        Me.Perc1.Text += "<img src='Images/warning.png' title='Překročen limit " & dblLimitHours_Notification.ToString & "hodin!'/>"
                    End If
                End If
            End If
            If bolAllowHonorar And .WaitingOnApproval_Hours_Count > 0 Then
                trRealFee.Visible = True
                WaitingOnApproval_HoursFee.Text = BO.BAS.FN(.WaitingOnApproval_HoursFee) & ",-"
            End If
            If dblLImitFee_Notification > 0 Then
                trLimitFee.Visible = True
                p41LimitFee_Notification.Text = BO.BAS.FN(dblLImitFee_Notification) & ",-"
                If .WaitingOnApproval_HoursFee > 0 Then
                    Me.Perc2.Text = CInt(100 * .WaitingOnApproval_HoursFee / dblLImitFee_Notification).ToString & "%"
                    If dblLImitFee_Notification < .WaitingOnApproval_HoursFee Then
                        'zvýraznit překročení limitu
                        Me.Perc2.Text += "<img src='Images/warning.png' title='Překročen limit " & dblLImitFee_Notification.ToString & ",-!'/>"
                    End If
                End If
            End If

            If .WaitingOnApproval_Other_Count > 0 Then
                Me.WaitingOnApproval_Other_Sum.Text = BO.BAS.FN(.WaitingOnApproval_Other_Sum) & " (" & .WaitingOnApproval_Other_Count.ToString & "x)"
                b = True
            End If
            Me.trWait4Approval.Visible = b
            b = False
            If .WaitingOnInvoice_Hours_Count > 0 Then
                Me.WaitingOnInvoice_Hours_Sum.Text = BO.BAS.FN(.WaitingOnInvoice_Hours_Sum) & " (" & .WaitingOnInvoice_Hours_Count.ToString & "x)"
                b = True
            End If
            If .WaitingOnInvoice_Other_Count > 0 Then
                Me.WaitingOnInvoice_Other_Sum.Text = BO.BAS.FN(.WaitingOnInvoice_Other_Sum) & " (" & .WaitingOnInvoice_Other_Count.ToString & "x)"
                b = True
            End If
            Me.trWait4Invoice.Visible = b
        End With
    End Sub
End Class