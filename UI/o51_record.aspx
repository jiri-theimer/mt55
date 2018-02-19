<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o51_record.aspx.vb" Inherits="UI.o51_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label></td>
            <td>
                <asp:TextBox ID="o51Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>

    </table>

    <div>
        <asp:CheckBox ID="o51ScopeFlag" runat="server" CssClass="chk" Text="Použitelný pro všechny entity" Checked="true" AutoPostBack="true" />

    </div>
    <asp:Panel ID="panEntities" runat="server">
        <asp:CheckBox ID="o51IsP41" Text="Projekty" runat="server" />
        <asp:CheckBox ID="o51IsP28" Text="Klienti" runat="server" />
        <asp:CheckBox ID="o51IsP56" Text="Úkoly" runat="server" />
        <asp:CheckBox ID="o51IsP91" Text="Faktury" runat="server" />
        <asp:CheckBox ID="o51IsP31" Text="Worksheet" runat="server" />
        <asp:CheckBox ID="o51IsO23" Text="Dokumenty" runat="server" />
        <asp:CheckBox ID="o51IsJ02" Text="Osoby" runat="server" />
        <asp:CheckBox ID="o51IsP90" Text="Zálohy" runat="server" />
    </asp:Panel>
    <table>
        <tr valign="top">
            <td>
                <div>Barva pozadí:</div>
                <div>
                    <telerik:RadColorPicker ID="o51BackColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="bez barvy" ShowIcon="false" Preset="none" RenderMode="Lightweight">
                        <telerik:ColorPickerItem Value="#FFFFFF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#C0C0C0"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#808080"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#000000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FF0000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#800000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFFF00"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#808000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#00FF00"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#008000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#00FFFF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#008080"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#0000FF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#000080"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FF00FF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#800080"></telerik:ColorPickerItem>
                    </telerik:RadColorPicker>
                </div>
            </td>
            <td>
                <div>Barva písma:</div>
                <div>
                    <telerik:RadColorPicker ID="o51ForeColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="bez barvy" ShowIcon="false" Preset="none" RenderMode="Lightweight">
                        <telerik:ColorPickerItem Value="#008080"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFFFFF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FF0000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFFF00"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#0000FF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#800000"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FF00FF"></telerik:ColorPickerItem>
                    </telerik:RadColorPicker>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
