<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_j02_record.aspx.vb" Inherits="UI.clue_j02_record" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function go2module() {

            window.open("j02_framework.aspx?pid=<%=Master.DataPID%>", "_top");

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/person_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
            <asp:HyperLink ID="j02Email" runat="server" CssClass="wake_link"></asp:HyperLink>
            <uc:mytags ID="tags1" ModeUi="2" Prefix="j02" runat="server" />
        </div>
        <div>
            <asp:hyperlink runat="server" ID="linkGoTo" NavigateUrl="javascript:go2module()">Přejít na stránku osoby</asp:hyperlink>
        </div>
        <asp:Panel ID="panIntraOnly" runat="server" CssClass="content-box2">
            <div class="title">
                Interní osoba v systému s uživatelským účtem
            </div>
            <div class="content">
                <table cellpadding="6">
                    <tr>
                        <td>Pozice:</td>
                        <td>
                            <asp:Label ID="j07Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        <td>Kód:</td>
                        <td>
                            <asp:Label ID="j02Code" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                    </tr>
                    <tr>
                        <td>Pracovní fond:</td>
                        <td>
                            <asp:Label ID="c21Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        <td>Středisko:</td>
                        <td>
                            <asp:Label ID="j18Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                    </tr>

                    <tr>
                        <td>Členství v týmech:</td>
                        <td colspan="3">
                            <asp:Label ID="j11Names" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                </table>


                <fieldset style="padding: 5px;">
                    <legend>
                        <img src="Images/user.png" />Uživatelský účet</legend>
                    <span class="lbl">Přihlašovací jméno:</span>
                    <asp:Label ID="j03Login" runat="server" CssClass="valboldblue"></asp:Label>
                    <span class="lbl">Aplikační role:</span>

                    <asp:Label ID="j04Name" runat="server" CssClass="valboldblue"></asp:Label>

                </fieldset>
                
            </div>
        </asp:Panel>
        <asp:Panel ID="panContacts" runat="server" CssClass="content-box2">
            <div class="title">
                Klienti
            </div>
            <div class="content">
                <asp:Repeater ID="rpP30" runat="server">
                    <ItemTemplate>
                        <div class="div6">
                            <asp:image ID="imgBind" runat="server" ImageUrl="Images/contact.png" />
                            <asp:hyperlink ID="BindLink" runat="server" Target="_top" CssClass="value_link"></asp:hyperlink>
                            <asp:Label ID="p27Name" runat="server"></asp:Label>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="div6">
                    <asp:Label ID="j02Mobile" runat="server" CssClass="valbold"></asp:Label>
                </div>
                <div class="div6">
                    <asp:Label ID="j02Phone" runat="server" CssClass="valbold"></asp:Label>
                </div>
                <div class="div6">
                    <asp:Label ID="j02JobTitle" runat="server" CssClass="valbold"></asp:Label>
                </div>
                <div class="div6">
                    <asp:Label ID="j02Office" runat="server" CssClass="valbold"></asp:Label>
                </div>
                
            </div>
        </asp:Panel>


    </div>
</asp:Content>

