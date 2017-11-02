<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="helpdesk_default.aspx.vb" Inherits="UI.helpdesk_default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function sw_local(url, img, is_maximize) {
            sw_master(url,img, is_maximize);
        }

        function wd(pid) {
            document.getElementById("<%=hidCurPID.ClientID%>").value = pid;
            sw_master("workflow_dialog.aspx?prefix=p56&pid="+pid, "Images/workflow_32.png");


        }

        function hardrefresh(pid, flag) {
            document.getElementById("<%=hidHardRefreshFlag.clientid%>").value = flag;
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefreshOnBehind, "", False)%>;

        }

        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 10px; background-color: white;">
        <img src="Images/helpdesk_32.png" alt="Helpdesk | Zapisování požadavků" />
        <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span" Style="font-size: 200%;" Text="Helpdesk | Zapisování požadavků"></asp:Label>

        <asp:LinkButton ID="cmdCreateTicket" runat="server" Text="Zapsat nový požadavek" Style="margin-left: 20px;"></asp:LinkButton>

        <asp:Panel ID="panTicket" runat="server" CssClass="content-box2" Visible="false" style="margin-top:20px;">
            <div class="title">

                <asp:Label ID="ph1" runat="server" Text="Zapsat nový požadavek" Style="display: inline-block; min-width: 150px;"></asp:Label>
                <asp:Button ID="cmdSaveTicket" runat="server" CssClass="cmd" Text="Odeslat" />
                <asp:Button ID="cmdClearTicket" runat="server" CssClass="cmd" Text="Zrušit" style="margin-left:40px;" />
            </div>
            <div class="content">
                <table cellpadding="6">
                    <tr>
                        <td>Projekt:</td>
                        <td>
                            <asp:DropDownList ID="p41ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="FullName"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Typ:</td>
                        <td>
                            <asp:DropDownList ID="p57ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p57Name"></asp:DropDownList>
                        </td>
                    </tr>

                </table>
                <div style="text-align: center;">
                    <asp:TextBox ID="p56Description" runat="server" TextMode="MultiLine" Style="width: 99%; height: 200px;"></asp:TextBox>
                </div>
                <table>
                    <tr>
                        <td>
                            <uc:fileupload ID="upload1" runat="server" EntityX29ID="p56Task" />
                        </td>
                        <td>
                            <uc:fileupload_list ID="uploadlist1" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>


        <asp:Panel ID="panGrid" runat="server" CssClass="content-box2" style="margin-top:20px;">
            <div class="title">

                <asp:Label ID="lblHeaderMy" runat="server" Text="Mé požadavky" Style="display: inline-block; min-width: 150px;"></asp:Label>
                <asp:DropDownList ID="cbxGridQuery" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="Pouze otevřené požadavky" Value="open"></asp:ListItem>
                    <asp:ListItem Text="Pouze uzavřené požadavky" Value="close"></asp:ListItem>
                    <asp:ListItem Text="Všechny požadavky" Value="all"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="content">
                <table cellpadding="6">
                    <tr>
                        <th>ID</th>
                        <th>Typ</th>
                        <th>Stav</th>
                        <th>Založeno</th>
                        <th>Projekt</th>
                        <th></th>
                    </tr>
                    <asp:Repeater ID="rpGrid" runat="server">
                        <ItemTemplate>
                            <tr class="trHover">
                                <td>
                                    <asp:HyperLink ID="link1" runat="server"></asp:HyperLink>
                                </td>
                                <td>
                                    <asp:Label ID="p57Name" runat="server"></asp:Label>
                                </td>

                                <td>
                                    <asp:Label ID="b02Color" runat="server" text="&nbsp;&nbsp;&nbsp;"></asp:Label>
                                    <asp:Label ID="b02Name" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="p56DateInsert" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Project" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:HyperLink ID="cmdWorkflowDialog" runat="server" Text="Posunout/doplnit"></asp:HyperLink>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </asp:Panel>
    </div>

    <asp:HiddenField ID="hidCurPID" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:LinkButton ID="cmdRefreshOnBehind" runat="server" Text="refreshonbehind" Style="display: none;"></asp:LinkButton>
</asp:Content>
