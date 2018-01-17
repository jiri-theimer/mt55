<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_proforma.aspx.vb" Inherits="UI.p91_proforma" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:CheckBox ID="chkClientOnly" runat="server" AutoPostBack="true" CssClass="chk" Text="Nabízet pouze zálohy klienta faktury" Checked="true" />
    </div>
    <table cellpadding="10">
        <tr valign="top">

            <td>
                <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Zálohová faktura:"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p90ID" runat="server" DataTextField="CodeWithClient" DataValueField="pid" IsFirstEmptyRow="true" Filter="Contains" Width="250px" AutoPostBack="true"></uc:datacombo>
                <asp:HyperLink ID="clue_p90" runat="server" CssClass="reczoom" Text="i" title="Detail zálohové faktury" Visible="false"></asp:HyperLink>

            </td>

        </tr>
        <tr id="trUhrada" runat="server">
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Úhrada zálohy:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="p82ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="DateWithAmount"></asp:DropDownList>
            </td>

        </tr>
        <tr id="trPerc" runat="server">
            <td>
                <asp:Label ID="lblPercentage" runat="server" Text="Podíl z úhrady (%):"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="perc" runat="server" AutoPostBack="true">
                </asp:DropDownList>


                <asp:Label ID="AfterPerc" runat="server" CssClass="valbold"></asp:Label>
                <div>
                <span>Bez DPH ručně:</span>
                <telerik:RadNumericTextBox ID="p99Amount_WithoutVat" runat="server" Width="100px"></telerik:RadNumericTextBox>
                
                </div>
            </td>
        </tr>
    </table>

    


    <asp:Panel ID="panP99" runat="server" CssClass="content-box2" Visible="false">
        <div class="title">
            Spárované úhrady záloh s daňovou fakturou
        </div>
        <div class="content">
            <table cellpadding="10">
                <asp:Repeater ID="rpP99" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%#Eval("p90Code")%>
                            </td>
                            <td style="font-weight: bold; text-align: right;">
                                <%#BO.BAS.FN(Eval("p99Amount"))%>
                                
                            </td>
                            <td>
                                (bez dph: <%#BO.BAS.FN(Eval("p99Amount_WithoutVat"))%>, dph: <%#BO.BAS.FN(Eval("p99Amount_Vat"))%>)
                            </td>
                            <td>
                                <asp:Button ID="cmdDelete" runat="server" CssClass="cmd" Text="Odstranit vazbu" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </table>
        </div>

    </asp:Panel>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
