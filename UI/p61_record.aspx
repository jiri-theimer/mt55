<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p61_record.aspx.vb" Inherits="UI.p61_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="8">
        <tr>
            <td>
                <asp:Label ID="lblName" Text="Název klastru:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="p61Name" runat="server" Style="width: 300px;"></asp:TextBox>
            </td>
        </tr>
    </table>


    <div class="content-box2">
        <div class="title">
            Seznam aktivit v klastru
        </div>
        <div class="content">
            <table cellpadding="8">
                <tr>
                    <td>
                        <asp:Label ID="label0" runat="server" Text="Aktivita:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p32ID" runat="server" AutoPostBack="false" DataTextField="NameWithSheet" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" Filter="Contains"></uc:datacombo>
                        <asp:Button ID="cmdAdd" runat="server" Text="Přidat aktivitu do klastru" CssClass="cmd" />
                    </td>

                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Sešit:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p34ID" runat="server" AutoPostBack="false" DataTextField="p34Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                        <asp:Button ID="cmdAddP34ID" runat="server" Text="Přidat/aktualizovat všechny aktivity sešitu" CssClass="cmd" />
                    </td>

                </tr>
            </table>


            <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" AllowMultiSelect="true"></uc:datagrid>
            <div>
                <asp:Button ID="cmdRemoveSelected" runat="server" Text="Odebrat označené" CssClass="cmd" />
            </div>
        </div>
    </div>




</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
