<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o48_record.aspx.vb" Inherits="UI.o48_record" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">
            <asp:Label ID="p28Name" runat="server"></asp:Label>
        </div>
        <div class="content">
            <table cellpadding="10">
                <tr>
                    <td>IČ:
                    </td>
                    <td>
                        <asp:Label ID="p28RecID" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>RČ:
                    </td>
                    <td>
                        <asp:Label ID="p28Person_BirthRegID" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>

            </table>
            <asp:Button ID="cmdAppend" runat="server" Text="Zařadit do seznamu monitorovaných subjektů" CssClass="cmd" Visible="false" OnClientClick="return isir_oper('insert');" />
            <asp:Button ID="cmdRemove" runat="server" Text="Odstranit ze seznamu monitorovaných subjektů" CssClass="cmd" Visible="false" OnClientClick="return isir_oper('remove');" />


            <div class="div6">
                Subjekt zařazen do monitoringu:
                <asp:Label ID="Timestamp" runat="server" CssClass="valbold" Text="NE"></asp:Label>
            </div>
        </div>
    </div>
    <div class="content-box2">
        <div class="title">
            Monitorovací služba
        </div>
        <div class="content">
            <div class="div6">
                Napojená monitorovací služba:
                <a href="http://www.isir.info" target="_blank">Insolvenční hlídač</a>
                
            </div>
            <div><img src="Images/insolvencni_hlidac.png" /></div>


        </div>
    </div>
    <div class="div6">
        <span class="infoNotification">Pokud máte zájem o napojení na jinou monitorovací službu, kontaktujte nás!</span>
    </div>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">

    <script type="text/javascript">


        function isir_oper(strOper) {
            var login = "<%=ViewState("isir_login")%>";
            var pwd = "<%=ViewState("isir_pwd")%>";
            var myxml = "<%=ViewState("subject")%>";
            var is_success = false;
            
            $.ajax({
                type: "POST",
                url: "http://www.isir.info/api/importsubjects",
                data: 'username=' + login + '&password=' + pwd + '&subjects=' + myxml,
                dataType: "xml",
                async: false,
                complete: function (data) {
                    is_success = true;
                },
                success: function (data) {
                    alert("Success");
                    is_success = true;

                },
                error: function (e, b, error) {                    
                    is_success = false;
                }
            });

            return (is_success);
        }



    </script>
</asp:Content>
