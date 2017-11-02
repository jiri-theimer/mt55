<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p31_record.aspx.vb" Inherits="UI.clue_p31_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/worksheet_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span" Text="Worksheet úkon"></asp:Label>
            <asp:Image ID="imgClone" runat="server" ImageUrl="Images/copy.png" style="margin-left:20px;" Visible="false"/>
            <asp:HyperLink ID="cmdClone" runat="server" Text="Kopírovat" NavigateUrl="#" visible="false"></asp:HyperLink>

            <img src="Images/edit.png" style="margin-left:20px;" />
            
            <asp:HyperLink ID="cmdEdit" runat="server" Text="Otevřít" NavigateUrl="#"></asp:HyperLink>
        </div>
        
        <table cellpadding="10" cellspacing="2">
            <tr>
                <td>Datum:</td>
                <td>
                    <asp:Label ID="p31Date" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td>Vykázaná hodnota:</td>
                <td>
                    <asp:Label ID="p31Value_Orig" runat="server" CssClass="valbold"></asp:Label>
                    <asp:Label ID="TimePeriod" runat="server" CssClass="valboldblue" style="padding-left:20px;"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Jméno:</td>
                <td>
                    <asp:Label ID="Person" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td>
                    Kontaktní osoba:
                </td>
                <td>
                    <asp:Label ID="ContactPerson" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Klient:</td>
                <td colspan="3">
                    <asp:Label ID="Client" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Projekt:</td>
                <td colspan="3">
                    <asp:Label ID="Project" runat="server" CssClass="valbold"></asp:Label>
                    
                    
                </td>
            </tr>
            
            <tr>
               
                <td>Aktivita:</td>
                <td>
                    <asp:Label ID="p32Name" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td><asp:Label ID="p57Name" runat="server"></asp:Label></td>
                <td>
                    <asp:Label ID="Task" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
            


        </table>
        
        <asp:panel ID="panFiles" runat="server" cssclass="content-box2">
            <div class="title">
                <img src="Images/notepad.png" />
                <asp:Label ID="o23Name" runat="server"></asp:Label>
                
            </div>
            <div class="content">
                <uc:fileupload_list ID="files1" runat="server" />
            </div>
        </asp:panel>
       
        <div class="bigtext">
            <asp:Label ID="p31Text" runat="server"></asp:Label>
        </div>
        <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>


    </asp:Panel>
</asp:Content>
