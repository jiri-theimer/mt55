<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="flexibee.aspx.vb" Inherits="UI.flexibee" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content-box2">
        <div class="title">
            Došlé faktury ve FlexiBee
        </div>
        <div class="content">
            <table>
                <tr>
                    <td>
                        <asp:Button ID="cmdLoadFaktury" runat="server" Text="Zobrazit došlé faktury z FlexiBee" CssClass="cmd" />
                    </td>
                    <td>Počet načtených faktur:
                    </td>
                    <td>
                        <asp:DropDownList ID="cbxTopRecs" runat="server">   
                            <asp:ListItem Value="500" Text="500"></asp:ListItem>                         
                            <asp:ListItem Value="200" Text="200"></asp:ListItem>
                            <asp:ListItem Value="100" Text="100" Selected="true"></asp:ListItem>
                            <asp:ListItem Value="50" Text="50"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <table cellpadding="6" cellspacing="2" style="background-color: white;">
        <tr>
            <th>ID</th>            
            <th>Vystaveno</th>
            <th>Splatnost</th>            
            <th>Firma</th>
            <th>Vč.DPH</th>
            <th>Popis</th>
            <th>Poslední změna</th>
            <th>MARKTIME</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Label ID="kod" runat="server"></asp:Label>
                        <asp:HiddenField ID="hidID" runat="server" />
                    </td>
                    
                    <td>
                        <asp:Label ID="datVyst" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="datSplat" runat="server" ForeColor="Brown"></asp:Label>
                    </td>
                  
                    <td>
                        <asp:Label ID="firma" runat="server" ForeColor="ForestGreen"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:Label ID="sumCelkem" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="popis" runat="server" Font-Italic="true"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lastupdate" runat="server" CssClass="timestamp"></asp:Label>
                    </td>
                    <td>
                        
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</asp:Content>
