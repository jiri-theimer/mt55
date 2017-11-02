

Imports Telerik.Web.UI
Imports System.Web.Script.Serialization

'Imports Aspose.Words




Public Class pokus
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    'Private fileFormatProvider As IFormatProvider



    Private Sub pokus_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master


    End Sub


   
    Private Sub pokus_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    
    

    
    Private Sub cmdPokus_Click(sender As Object, e As EventArgs) Handles cmdPokus.Click

        Master.Factory.ChangeConnectString("server=Sql.mycorecloud.net\MARKTIME;database=marktime50_vrajik;uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;")

        txt1.Text = Master.Factory.p28ContactBL.GetList(New BO.myQueryP28).OrderByDescending(Function(p) p.PID)(0).p28Name

        Master.Factory.ChangeConnectString("server=Sql.mycorecloud.net\MARKTIME;database=marktime50_strabag;uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;")
        txt1.Text += vbCrLf & Master.Factory.p28ContactBL.GetList(New BO.myQueryP28).OrderByDescending(Function(p) p.PID)(0).p28Name

        Master.Factory.ChangeConnectString("server=Sql.mycorecloud.net\MARKTIME;database=marktime50_rutland;uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;")
        txt1.Text += vbCrLf & Master.Factory.p28ContactBL.GetList(New BO.myQueryP28).OrderByDescending(Function(p) p.PID)(0).p28Name


    End Sub
End Class