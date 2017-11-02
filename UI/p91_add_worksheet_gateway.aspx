<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_add_worksheet_gateway.aspx.vb" Inherits="UI.p91_add_worksheet_gateway" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<asp:Panel ID="panFindInvoice" runat="server" Visible="false">
    <div class="div6">
    <span>Vyberte klienta:</span>
        <uc:contact ID="p28ID_Find" runat="server" AutoPostBack="true" Width="600px" />
    </div>
    <div class="div6">
    <span>Vyberte fakturu:</span>
    <uc:datacombo ID="p91ID" runat="server" DataTextField="CodeAndAmount" DataValueField="pid" AutoPostBack="false" Filter="Contains" IsFirstEmptyRow="true" Width="600px"></uc:datacombo>
    </div>
</asp:Panel>
<asp:Panel ID="panPage" runat="server" Visible="false">
    <asp:RadioButtonList ID="opgCreateOrAppend" runat="server" CellPadding="6" RepeatDirection="Vertical" AutoPostBack="true">
        <asp:ListItem Text="Vytvořit nový worksheet úkon" Value="create" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Vložit do faktury worksheet úkony z fronty schválených" Value="append"></asp:ListItem>
    </asp:RadioButtonList>

    <table cellpadding="10">
        <tr>
            <td>
                <asp:Label ID="lblP41ID" runat="server" Text="Vyberte projekt:"></asp:Label>
            </td>
            <td>
                <uc:project ID="p41id" runat="server" AutoPostBack="true" Width="400px" />
            </td>

        </tr>
    </table>
    <asp:Panel ID="panP34" runat="server" CssClass="content-box2">

        <div class="title">Zvolte sešit pro nově vytvářený úkon</div>
        <div class="content">
            <asp:RadioButtonList ID="p34ID" runat="server" RepeatDirection="Vertical" DataTextField="p34Name" DataValueField="pid">
            </asp:RadioButtonList>
        </div>
    </asp:Panel>
    <asp:Panel ID="panStats" runat="server" CssClass="content-box2">

        <div class="title">Statistika vybraného projektu</div>
        <div class="content">
            <table cellpadding="10">
                <tr>
                    <td>Schválené hodiny čekající na fakturaci:</td>
                    <td>
                        <asp:Label ID="lblApproved_Hours" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    <td>Ostatní čekající úkony na fakturaci:</td>
                    <td>
                        <asp:Label ID="lblApproved_OtherCount" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Rozpracované hodiny, které čekají na schvalování:</td>
                    <td>
                        <asp:Label ID="lblEditing_Hours" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    <td>Ostatní rozpracované úkony:</td>
                    <td>
                        <asp:Label ID="lblEditing_OtherCount" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
