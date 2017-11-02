<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p93_record.aspx.vb" Inherits="UI.p93_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td style="width: 180px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název hlavičky vystavovatele:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p93Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" CssClass="lblReq" Text="Název firmy:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p93Company" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>

        <tr valign="top">
            <td>
                <asp:Label ID="Label3" runat="server" CssClass="lbl" Text="Ulice:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p93Street" runat="server" Style="width: 400px; height: 50px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Město:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p93City" runat="server" Style="width: 400px;"></asp:TextBox>


            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label4" runat="server" CssClass="lbl" Text="PSČ:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="p93Zip" runat="server" Style="width: 70px;"></asp:TextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label7" runat="server" CssClass="lbl" Text="Kontaktní informace:"></asp:Label>

            </td>
            <td>
                <asp:TextBox ID="p93Contact" runat="server" Style="width: 400px; height: 50px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label5" runat="server" CssClass="lbl" Text="IČ:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p93RegID" runat="server" Style="width: 120px;"></asp:TextBox>
                <asp:Label ID="Label6" runat="server" CssClass="lbl" Text="DIČ:" Style="padding-left: 30px;"></asp:Label>
                <asp:TextBox ID="p93VatID" runat="server" Style="width: 120px;"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="Label8" runat="server" CssClass="lbl" Text="Registrace v rejstříku:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p93Registration" runat="server" Style="width: 700px;"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="vertical-align: top;">
                <asp:Label ID="Label10" runat="server" CssClass="lbl" Text="Podpis:"></asp:Label>

            </td>
            <td style="vertical-align: top;">
                <asp:TextBox ID="p93Signature" runat="server" Style="width: 200px; height: 40px;" TextMode="MultiLine"></asp:TextBox>
                <asp:Label ID="Label9" runat="server" CssClass="lbl" Text="Referent:" Style="vertical-align: top;"></asp:Label>
                <asp:TextBox ID="p93Referent" runat="server" Style="width: 200px; height: 40px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
    </table>

    <div class="content-box2">
        <div class="title">
            <img src="Images/bank.png" width="16px" height="16px" />
            <asp:Label ID="phP88" runat="server" Text="Bankovní účty" Style="display: inline-block; min-width: 150px;"></asp:Label>
            <asp:Button ID="cmdAddP88" runat="server" CssClass="cmd" Text="Přidat" />
        </div>
        <div class="content">
            <table cellpadding="10">
                <tr>
                    <th>Měna</th>
                    <th>Bankovní účet</th>

                </tr>
                <asp:Repeater ID="rpP88" runat="server">
                    <ItemTemplate>
                        <tr valign="top">
                            <td>
                                <asp:DropDownList ID="j27ID" DataTextField="j27Code" DataValueField="pid" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="p86id" DataTextField="NameWithAccount" DataValueField="pid" runat="server">
                                </asp:DropDownList>
                            </td>


                            <td>
                                <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                <asp:HiddenField ID="p85id" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>

    <div>
        <asp:Label ID="Label11" runat="server" CssClass="lbl" Text="Volné pole #1:"></asp:Label>
    </div>
    <div>
    <asp:TextBox ID="p93FreeText01" runat="server" TextMode="MultiLine" Style="width: 100%;height:60px;"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="Label12" runat="server" CssClass="lbl" Text="Volné pole #2:"></asp:Label>
    </div>
    <div>
    <asp:TextBox ID="p93FreeText02" runat="server" TextMode="MultiLine" Style="width: 100%;height:50px;"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="Label13" runat="server" CssClass="lbl" Text="Volné pole #3:"></asp:Label>
    </div>
    <asp:TextBox ID="p93FreeText03" runat="server" Style="width: 100%;height:50px;" TextMode="MultiLine"></asp:TextBox>
    <div>
        <asp:Label ID="Label14" runat="server" CssClass="lbl" Text="Volné pole #4:"></asp:Label>
    </div>
    <div>
    <asp:TextBox ID="p93FreeText04" runat="server" Style="width: 100%;height:50px;" TextMode="MultiLine"></asp:TextBox>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>




