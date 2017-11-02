<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="b01_record.aspx.vb" Inherits="UI.b01_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>

            <td style="width: 120px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název šablony:"></asp:Label></td>
            <td>
                <asp:TextBox ID="b01Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
            
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCode" runat="server" CssClass="lbl" Text="Kód:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="b01Code" runat="server" Style="width: 100px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX29ID" runat="server" CssClass="lbl" Text="Entita:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server">
                    
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>                    
                    <asp:ListItem Text="Vystavená faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
                    <td>
                        <asp:Label ID="lblO40ID" Text="Individuální SMTP účet:" runat="server" CssClass="lbl"></asp:Label></td>
                    <td>
                        <uc:datacombo ID="o40ID" runat="server" DataTextField="o40Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                        
                    </td>
                </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
