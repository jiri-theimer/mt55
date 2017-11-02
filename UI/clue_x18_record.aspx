<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_x18_record.aspx.vb" Inherits="UI.clue_x18_record" %>
<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
           <img src="Images/notepad_32.png" />
           <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
       </div>

        <table cellpadding="5" cellspacing="2">
            
            <tr>
                <td>Max.povolená velikost 1 souboru:</td>
                <td>
                    <asp:Label ID="x18MaxOneFileSize" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Povolené formáty (přípony) nahrávaných souborů:</td>
                <td>
                    <asp:Label ID="x18AllowedFileExtensions" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
        </table>
        
        
       
       
        <div>
            <span class="lbl">Povinnost vazby k záznamu entity:</span>
            
        </div>
        <div>
            <asp:Label ID="lblBindInfo" runat="server" CssClass="valbold"></asp:Label>
        </div>
    </div>
</asp:Content>
