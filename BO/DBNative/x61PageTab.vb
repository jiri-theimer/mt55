Public Class x61PageTab
    Public Property x61ID As Integer
    Public Property x61Code As String
    Public Property x29ID As BO.x29IdEnum
    Public Property x61Name As String
    Public Property x61Ordinary As Integer

    Public Function GetPageUrl(strMasterPrefix As String, intMasterPID As Integer, strIsApprovingPerson As String) As String
        Dim s As String = "masterprefix=" & strMasterPrefix & "&masterpid=" & intMasterPID.ToString
        If strIsApprovingPerson <> "" Then s += "&IsApprovingPerson=" & strIsApprovingPerson
        Select Case x61Code
            Case "summary"
                Return "entity_framework_rec_summary.aspx?" & s
            Case "p31"
                Return "entity_framework_rec_p31.aspx?" & s
            Case "time", "expense", "fee", "kusovnik"
                Return "entity_framework_rec_p31.aspx?p31tabautoquery=" & x61Code & "&" & s
            Case "p56"
                Return "entity_framework_rec_p56.aspx?" & s
            Case "p91"
                Return "entity_framework_rec_p91.aspx?" & s
            Case "p90"
                Return "p28_framework_detail_p90.aspx?" & s
            Case "budget"
                Return "p41_framework_rec_budget.aspx?pid=" & intMasterPID.ToString
            Case "o23"
                Return "entity_framework_rec_o23.aspx?" & s
                ''Case "workflow"
                ''    Return "entity_framework_b07subform.aspx?" & s
            Case "p41"
                Return "entity_framework_rec_p41.aspx?" & s
            Case Else
                Return strMasterPrefix & "_framework_detail.aspx?pid=" & intMasterPID.ToString  'výchozí board stránka
        End Select
    End Function

End Class
