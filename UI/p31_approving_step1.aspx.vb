Public Class p31_approving_step1
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_approving_step1_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_approving_start"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Request.Item("masterprefix") = "" Or (BO.BAS.IsNullInt(Request.Item("masterpid")) = 0 And Request.Item("masterpids") = "") Then
                Master.StopPage("masterprefix or masterpid/masterpids missing.")
            End If
            Dim mq As New BO.myQueryP31
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForDoApprove  'schválit rozpracovanost
            If Request.Item("reapprove") = "1" Or Request.Item("clearapprove") = "1" Then
                mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForReApprove  'pře-schválit nebo vyčistit již schválený worksheet
                If Request.Item("approving_level") <> "" Then
                    mq.p31ApprovingLevel = BO.BAS.IsNullInt(Request.Item("approving_level"))
                End If
            End If
            If Request.Item("aw") <> "" Then
                mq.MG_AdditionalSqlWHERE = Replace(Server.UrlDecode(Request.Item("aw")), "xxx", "=")
            End If

            Dim masterpids As New List(Of Integer)
            If Request.Item("masterpid") <> "" Then
                masterpids.Add(BO.BAS.IsNullInt(Request.Item("masterpid")))
            End If
            If Request.Item("masterpids") <> "" Then
                masterpids = BO.BAS.ConvertPIDs2List(Request.Item("masterpids")).Distinct.ToList
            End If
          
            Select Case Request.Item("masterprefix")
                Case "p41"
                    If masterpids.Count = 1 Then
                        mq.p41ID = masterpids(0)
                    Else
                        mq.p41IDs = masterpids
                    End If
                Case "p28"
                    If masterpids.Count = 1 Then
                        mq.p28ID_Client = masterpids(0)
                    Else
                        mq.p28IDs_Client = masterpids
                    End If
                Case "j02"
                    If masterpids.Count = 1 Then
                        mq.j02ID = masterpids(0)
                    Else
                        mq.j02IDs = masterpids
                    End If
                Case "p56"
                    mq.p56IDs = masterpids
            End Select


            Select Case Request.Item("prefix")
                Case "p41"
                    mq.p41ID = BO.BAS.IsNullInt(Request.Item("pid"))
                Case "p34"
                    mq.p34ID = BO.BAS.IsNullInt(Request.Item("pid"))
                Case "j02"
                    mq.j02ID = BO.BAS.IsNullInt(Request.Item("pid"))
                Case "p56"
                    mq.p56IDs = BO.BAS.ConvertPIDs2List(Request.Item("pid"))
                    If mq.p56IDs.Count = 0 Then Master.StopPage("pid is missing")
            End Select
            If Request.Item("datefrom") <> "" Then
                mq.DateFrom = BO.BAS.ConvertString2Date(Request.Item("datefrom"))
            End If
            If Request.Item("datefrom") <> "" Then
                mq.DateFrom = BO.BAS.ConvertString2Date(Request.Item("datefrom"))
            End If
            If Request.Item("dateuntil") <> "" Then
                mq.DateUntil = BO.BAS.ConvertString2Date(Request.Item("dateuntil"))
            End If


            Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
            If lis.Count = 0 Then
                Master.Notify("Ani jeden záznam není přístupný pro schvalování.", NotifyLevel.ErrorMessage)
                Return
            End If
            Dim strGUID As String = BO.BAS.GetGUID
            Dim cTemp00 As New BO.p85TempBox
            cTemp00.p85GUID = strGUID & "-00"
            cTemp00.p85FreeText01 = Request.Item("approving_level")
            cTemp00.p85Message = "pids=" & String.Join(",", masterpids) & "&prefix=" & Request.Item("masterprefix")
            Master.Factory.p85TempBoxBL.Save(cTemp00)

            For Each c In lis
                Dim cTemp As New BO.p85TempBox
                cTemp.p85GUID = strGUID
                cTemp.p85DataPID = c.PID
                Master.Factory.p85TempBoxBL.Save(cTemp)
            Next
            Dim strURL As String = "p31_approving_step2.aspx?guid=" & strGUID & "&masterpid=" & Request.Item("masterpid") & "&masterprefix=" & Request.Item("masterprefix")
            If Request.Item("clearapprove") = "1" Then
                strURL += "&clearapprove=1"
            End If
            If Request.Item("approvingset") <> "" Then
                strURL += "&approvingset=" & Request.Item("approvingset")
            End If
            If Request.Item("approving_level") <> "" Then
                strURL += "&approving_level=" & Request.Item("approving_level")
            End If
            Response.Redirect(strURL)



        End If
    End Sub

End Class