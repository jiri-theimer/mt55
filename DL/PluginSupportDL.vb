Public Class PluginSupportDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub

    Public Function GetDataset(strSQL As String, Optional ByRef intRetRows As Integer = 0, Optional dbPars As List(Of BO.PluginDbParameter) = Nothing) As DataSet
        strSQL = bas.ClearSqlForAttacks(strSQL)
        Return _cDB.GetDataSet(strSQL, intRetRows, dbPars)
    End Function
    Public Function GetDatasetBySP(strSP As String, Optional ByRef intRetRows As Integer = 0, Optional dbPars As List(Of BO.PluginDbParameter) = Nothing) As DataSet
        strSP = bas.ClearSqlForAttacks(strSP)
        Return _cDB.GetDataSetBySP(strSP, intRetRows, dbPars)
    End Function
    
End Class
