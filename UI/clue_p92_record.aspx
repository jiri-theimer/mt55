<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p92_record.aspx.vb" Inherits="UI.clue_p92_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
           <img src="Images/setting_32.png" />
           <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
       </div>
        <table cellpadding="10" cellspacing="2">
            <tr>
                <td>Cílová měna:</td>
                <td>
                    <asp:Label ID="j27Code" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td>Cílový stát:</td>
                <td>
                    <asp:Label ID="j17Name" runat="server" CssClass="valbold"></asp:Label>

                </td>
            </tr>
            <tr>
                <td>Cílová DPH sazba:</td>
                <td>
                    <asp:Label ID="x15Name" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td>Číselná řada dokladu:</td>
                <td>
                    <asp:Label ID="x38Name" runat="server" CssClass="valbold"></asp:Label>

                </td>
            </tr>

        </table>
        <fieldset>
            <legend>Nastavené bankovní účty</legend>
            <table cellpadding="6">
                <asp:Repeater ID="rpP88" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td style="font-weight:bold;color:blue;">
                                <%# Eval("j27Code")%>
                            </td>
                            <td>
                                <%# Eval("Account")%>
                                
                            </td>
                           
                        </tr>
                            
                        
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </fieldset>
        <fieldset>
            <legend>Hlavička vystavovatele faktury</legend>
            <table cellpadding="6">
                <tr>
                    <td>Firma:</td><td><asp:Label ID="p93Company" runat="server" CssClass="valbold"></asp:Label></td>
                </tr>
                <tr>
                    <td>Adresa:</td><td><asp:Label ID="Address" runat="server" CssClass="valbold"></asp:Label></td>
                </tr>
                <tr>
                    <td>IČ | DIČ:</td><td><asp:Label ID="RegIDVatID" runat="server" CssClass="valbold"></asp:Label></td>
                </tr>
                <tr>
                    <td>Kontaktní info:</td><td><asp:Label ID="p93Contact" runat="server" CssClass="valbold"></asp:Label></td>
                </tr>
                <tr>
                    <td>Referent:</td><td><asp:Label ID="p93Referent" runat="server" CssClass="valbold"></asp:Label></td>
                </tr>
                <tr>
                    <td>Podpis:</td><td><asp:Label ID="p93Signature" runat="server" CssClass="valbold"></asp:Label></td>
                </tr>
            </table>
        </fieldset>
        
    </div>
</asp:Content>

