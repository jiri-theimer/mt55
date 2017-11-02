<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_j02_oplan.aspx.vb" Inherits="UI.clue_j02_oplan" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="periodmonth" Src="~/periodmonth.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <asp:Panel ID="panHeader" runat="server">
            <img src="Images/person_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>

            <asp:Label ID="Mesic" runat="server" CssClass="valboldblue" Style="padding-left: 60px;"></asp:Label>

        </asp:Panel>
        <div class="div6">
            <uc:periodmonth ID="period1" runat="server"></uc:periodmonth>
        </div>

        <table class="tabulka" cellpadding="10" border="1">
            <tr>
                <th>Projekt</th>
                <th>
                    <img src="Images/plan.png" />Kapacitní plán
                </th>
                <th>
                    <img src="Images/oplan.png" />Operativní plán
                </th>
                <th>
                    <img src="Images/worksheet.png" />Vykázané hodiny
                </th>
            </tr>
            <asp:Repeater ID="rp1" runat="server">
                <ItemTemplate>
                    <tr class="trHover">
                        <td>
                            <%# Eval("Project")%>                           
                        </td>
                        <td align="right">
                            <%# BO.BAS.FN2(Eval("p47Hours"))%>   
                        </td>
                        <td align="right">
                            <%# BO.BAS.FN2(Eval("p48Hours"))%>  
                        </td>
                        <td align="right">
                            <%# BO.BAS.FN2(Eval("p31Hours"))%>  
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <th></th>
                <th align="right">
                    <asp:Label ID="p47Total" runat="server"></asp:Label></th>
                <th align="right">
                    <asp:Label ID="p48Total" runat="server"></asp:Label></th>
                <th align="right">
                    <asp:Label ID="p31Total" runat="server"></asp:Label></th>
            </tr>
        </table>

    </asp:Panel>
</asp:Content>
