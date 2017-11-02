<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p48_record.aspx.vb" Inherits="UI.clue_p48_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/oplan_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span" Text="Záznam operativního plánu"></asp:Label>
           

            <asp:Image ID="imgEdit" runat="server" ImageUrl="Images/edit.png" style="margin-left:20px;" Visible="false"/>
            <asp:HyperLink ID="cmdEdit" runat="server" Text="Upravit" NavigateUrl="#" visible="false"></asp:HyperLink>

            <asp:Image ID="imgWorksheet" runat="server" ImageUrl="Images/worksheet.png" style="margin-left:20px;" Visible="false"/>
            <asp:HyperLink ID="cmdConvert" runat="server" Text="Překlopit plán do reality" NavigateUrl="#" visible="false"></asp:HyperLink>
            <asp:HyperLink ID="cmdWorksheet" runat="server" Text="Časový úkon (realita)" NavigateUrl="#" visible="false"></asp:HyperLink>
        </div>
        
        <table cellpadding="10" cellspacing="2">
            <tr>
                <td>Datum:</td>
                <td>
                    <asp:Label ID="p48Date" runat="server" CssClass="valbold"></asp:Label>
                </td>
                <td>Hodiny:</td>
                <td>
                    <asp:Label ID="p48Hours" runat="server" CssClass="valbold"></asp:Label>
                    <asp:Label ID="TimePeriod" runat="server" CssClass="valboldblue" style="padding-left:20px;"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Osoba:</td>
                <td>
                    <asp:Label ID="Person" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td>Aktivita:</td>
                <td>
                    <asp:Label ID="p32Name" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
       
            <tr>
                <td>Projekt:</td>
                <td colspan="3">
                    <asp:Label ID="Project" runat="server" CssClass="valbold"></asp:Label>
                    
                    
                </td>
            </tr>
            
        </table>
        
        <div class="bigtext">
            <asp:Label ID="p48Text" runat="server"></asp:Label>
        </div>
        <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>


    </asp:Panel>
</asp:Content>

