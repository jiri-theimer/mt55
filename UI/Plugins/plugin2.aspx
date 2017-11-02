<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            Master.Notify("ahoj", 1)
        End If
        
    End Sub

    

    Protected Sub cmdPokus_Click(sender As Object, e As EventArgs)
        Me.txtPokus.Text = Now.ToString
    End Sub
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">


        function hardrefresh(pid, flag) {
            alert(flag);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Plugin 2</h1>
    <img src="../Images/logo_transparent.png" />
    <p>Ahoj</p>

    <table cellpadding="10">
        <tr>
            <td>
                <span>Přihlášený uživatel:</span>
            </td>
            <td align="right">
                <%=Master.Factory.SysUser.Person%>
            </td>
        </tr>
        <tr>
            <td>
                <%=Master.Factory.x31reportbl.getlist().count%>
            </td>
            <td align="right">
                <asp:TextBox runat="server" ID="txtPokus"></asp:TextBox>
                <asp:Button ID="cmdPokus" runat="server" Text="Pokus" OnClick="cmdPokus_Click" />
                <asp:TextBox runat="server" ID="txtPokus2"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
