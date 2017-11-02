<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="help.aspx.vb" Inherits="UI.help" %>
<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="height:40px;">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width: 20px">
                    <img src="Images/help_32.png" />
                    
                </td>
               <td>
                   <span class="page_header_span">Nápověda</span>
                   
                   <asp:Label ID="lblFormHeader" runat="server" CssClass="page_header_span" Style="margin-left: 20px;"></asp:Label>
               </td>
              
                 <td style="width:200px;">
                    <img src="Images/logo_transparent.png" />
                </td>
                <td>
                    <button type="button" onclick="window.close()">Zavřít</button>
                </td>
                <td>
                    <asp:HyperLink ID="x50ExternalURL" runat="server"></asp:HyperLink>
                </td>
                <td align="right">
                    <asp:Button ID="cmdNew" runat="server" Text="Psát nápovědu pro tuto oblast (stránku)" CssClass="cmd" />

                </td>
            </tr>
        </table>
    </div>
    <uc:fileupload_list ID="upl1" runat="server" />
    <div style="padding:10px;">
        <asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
    </div>
    
</asp:Content>
