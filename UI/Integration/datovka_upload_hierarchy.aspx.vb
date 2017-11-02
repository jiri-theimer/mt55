Imports System.Web.Services
Imports System.Web.Script.Serialization

Public Class datovka_upload_hierarchy
    Inherits System.Web.UI.Page

    Private Class DatovkaRoot
        Public Property name As String
        Public Property id As String
        'Public Property metadata() As Object
        Public Property metadata As List(Of String)
        Public Property [sub] As List(Of DatovkaItem)
        Public Sub New()
            Me.sub = New List(Of DatovkaItem)
            Me.metadata = New List(Of String)
        End Sub
    End Class

    Private Class DatovkaItem
        Public Property name As String
        Public Property id As String
        'Public Property metadata() As Object
        Public Property metadata As List(Of String)
        Public Property [sub] As List(Of DatovkaItem)
        Public Sub New()
            Me.sub = New List(Of DatovkaItem)
            Me.metadata = New List(Of String)
        End Sub
    End Class
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.ContentType = "application/json"

        Dim factory As New BL.Factory(Nothing, "mtservice")


        Dim c0 As New DatovkaRoot
        Dim mqP28 As New BO.myQueryP28
        mqP28.Closed = BO.BooleanQueryMode.FalseQuery
        Dim lisP28 As IEnumerable(Of BO.p28Contact) = factory.p28ContactBL.GetList(mqP28)
        Dim mqP41 As New BO.myQueryP41
        mqP41.Closed = BO.BooleanQueryMode.FalseQuery
        ''mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
        Dim lisP41 As IEnumerable(Of BO.p41Project) = factory.p41ProjectBL.GetList(mqP41)

        For Each cP28 In lisP28
            Dim c1 As New DatovkaItem
            c1.id = "p28-" & cP28.PID.ToString
            c1.name = cP28.p28Name
            If c1.metadata Is Nothing Then c1.metadata = New List(Of String)

            For Each cP41 In lisP41.Where(Function(p) p.p28ID_Client = cP28.PID)
                Dim c2 As New DatovkaItem
                c2.id = "p41-" & cP41.PID.ToString
                c2.name = cP41.PrefferedName
                c2.metadata = New List(Of String)
                c2.sub = New List(Of DatovkaItem)

                'c1.metadata.Add(cP41.p41Code)
                If c1.sub Is Nothing Then c1.sub = New List(Of DatovkaItem)
                c1.sub.Add(c2)
            Next
            If c0.sub Is Nothing Then c0.sub = New List(Of DatovkaItem)
            c0.sub.Add(c1)


        Next

        Dim serializer As New JavaScriptSerializer()
        Dim serializedResult = serializer.Serialize(c0)


        Response.Write(serializedResult)


    End Sub

End Class