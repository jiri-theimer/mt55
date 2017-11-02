<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j05_record.aspx.vb" Inherits="UI.j05_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblMaster" runat="server" CssClass="lblReq" Text="Nadřízená osoba:"></asp:Label></td>
            <td>
                <uc:person ID="j02ID_Master" runat="server" Width="400px" />
                
            </td>
            <td>
                <img src="Images/master_32.png" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblSlavePerson" Text="Podřízená osoba:" runat="server" CssClass="lbl"></asp:Label>
                
            </td>
            <td>
                <uc:person ID="j02ID_Slave" runat="server" Width="400px" />
                
            </td>
            <td rowspan="2">
                <img src="Images/slave_32.png" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblSlaveTeam" Text="nebo podřízený tým:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>

                <uc:datacombo ID="j11ID_Slave" runat="server" DataTextField="j11Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td></td>
            <td></td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label1" Text="Vztak k worksheet záznamům:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <div>
                    <asp:CheckBox ID="j05IsCreate_p31" runat="server" Text="Možnost zapisovat za podřízeného nové úkony" />
                </div>
                <fieldset>
                    <legend>Přístup k uloženým úkonům podřízeného</legend>
                    <asp:RadioButtonList ID="j05Disposition_p31" runat="server">
                        <asp:ListItem Text="Číst" Value="1" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Číst+upravovat" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Číst a schvalovat" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Číst, upravovat a schvalovat" Value="4"></asp:ListItem>
                    </asp:RadioButtonList>
                </fieldset>

            </td>
        </tr>
        <tr>
            <td></td>
            <td></td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label2" Text="Vztak k operativnímu plánu:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <div>
                    <asp:CheckBox ID="j05IsCreate_p48" runat="server" Text="Možnost zapisovat za podřízeného nový plán" />
                </div>
                <fieldset>
                    <legend>Přístup k uloženým plánům podřízeného</legend>
                    <asp:RadioButtonList ID="j05Disposition_p48" runat="server">
                        <asp:ListItem Text="Číst" Value="1" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Číst+upravovat" Value="2"></asp:ListItem>
                   
                    </asp:RadioButtonList>
                </fieldset>

            </td>
        </tr>
    </table>
    <span class="infoInForm">Nadřízený disponuje automaticky přístupem ke čtení kalendářových událostí a úkolů podřízeného.</span>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
