Public Class FlexiBee_Rootobject
    Public Property winstrom As Winstrom
End Class

Public Class Winstrom
    Public Property version As String
    'Public Property fakturaprijata As FakturaPrijata()
    Public Property fakturaprijata As List(Of FakturaPrijata)
    ''Public Property fakturaprijata() As FakturaPrijata

End Class

Public Class FakturaPrijata
    Public Property id As String
    Public Property lastUpdate As Date
    Public Property kod As String
    Public Property stavUhrK As String
    Public Property datVyst As Date
    Public Property datSplat As Date
    Public Property sumCelkem As Double
    Public Property sumZalohy As Double
    Public Property sumZalohyMen As Double
    Public Property sumCelkemMen As Double
    Public Property zbyvaUhraditMen As Double
    Public Property zbyvaUhradit As Double
    Public Property mena As String
    Public Property menashowAs As String
    Public Property menainternalId As String
    Public Property menaref As String
    Public Property firma As String
    Public Property popis As String
    Public Property stavUhrKshowAs As String
    Public Property firmashowAs As String
    Public Property firmainternalId As String
    Public Property firmaref As String
    Public Property externalids() As String

    'Pole navíc (detail=full):
    Public Property duzpPuv As Date
    Public Property varSym As String
    Public Property sumZklCelkem As Double
    Public Property sumDphCelkem As Double
End Class

