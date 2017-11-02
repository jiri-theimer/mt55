Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization

Public Class datovka_service_info
    Inherits System.Web.UI.Page

    Public Class ServiceInfo
        Public Property logo_svg As String
        Public Property name As String
        Public Property token_name As String
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "application/json"

        
        Dim c As New ServiceInfo
        c.name = "MARKTIME"
        c.logo_svg = "iVBORw0KGgoAAAANSUhEUgAAAKMAAAASCAMAAAAAJJ5iAAAAb1BMVEUAAAAEdsT///8AAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQAAAAEdsQEBgQEAgQAAAAEdsSQbrM1AAAAI3RSTlMAAAAQECAgMDBAQFBQYGBwcH9/j4+fn6+vv7/Pz9/f7+/3+354NzAAAAK8SURBVHgBndaNcqo6FAVg00ZFBYuoXIhoc928/zMesvZPmSmZ0a6ZTkJOjnzumMDKOXcJIXSO04QpzlKnywu6PBHpao/rIl0U3EP34+NElt2OaJXSrzTa/5n18UKScUzZuxSvfUnEtZerMGoiZHud7DGvdstGqoxY0d+MG9y15TKiXyqxZFE9MypyMzcO/AmTMQlgkxbN3Yw32rFRa/uaEXfSYj3QbdTYjcgwM8Yw6HcyY5va4DLGLwJMRr4yRqvq8/n8bWxGgx3HWU1RYU6hRlCKOLUPM/L/GnzOeCb6T0RXomvOKO1T/uZGK9ZDEHAgrI+KNqO7pHEzFvoDzRh7IlpjYD31bn8yDiOndIX0okOw8l1rA2Zs58YyaqUzxqmMqF6q6BVDubXOG0dJ58CR+xsB++Y4M/oatdYJUf89Y9zSfU3fn1Pv85u2d9rBKOmB0FDOiOJh9dBrfw4fqWCEX42aEkb7enljNdWsp695720j6uSxDdKfV4DzIMvKbuZGjKtRxTnjmU5y/NzpMOnO+d9j1tgkGUOwu+3wqeXmpY2Ysds7Mw66ZTLGnirmHRL0QP2iET9I6BbOHmzroMdM9G7QKuHmYYruehgfuvS2rwOkPmck2mKZGbslev8MB6XRGrW2eXWTa/a6Z2otqxo9Dv6wbAQK2+WAjQPy20Y87PT43vChGLFjNKoXflCyGPlMHy/LxkpPRKIzPw2rBeP/vzI32jP3AYAc3PKaEAOCrlfjZsSlGfX7HReNZ37IrPUgv9L5bSM+34ttP3/AoTbIBQQxYiZaM/KEWCwZ+2lbs+2KFrwT3Xacl9a60TN7Ixuj4O0c9MTR5/ZgRjeI34w8O/oFo75QbNFi8O13s4u9xDahRBumHH3AuKRNV97JXH2jnb3j+g7z2bjt+5W2n32/XiEnbtZpoOo1rxj/AUe9pSVHPjycAAAAAElFTkSuQmCC"
        c.token_name = "MARKTIME-DATOVKA-TOKEN"

        Dim serializer As New JavaScriptSerializer()
        Dim serializedResult = serializer.Serialize(c)

        Response.Write(serializedResult)

    End Sub

End Class