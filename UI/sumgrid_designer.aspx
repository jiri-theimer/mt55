<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="sumgrid_designer.aspx.vb" Inherits="UI.sumgrid_designer" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div>


        <asp:Label ID="ph1" runat="server" Text="Návrhář WORKSHEET statistik" CssClass="page_header_span"></asp:Label>
        <span style="padding-left: 20px;"></span>
        <asp:ImageButton ID="cmdNew" runat="server" ImageUrl="Images/new.png" CssClass="button-link" ToolTip="Založit novou pojmenovanou šablonu" />

    </div>

    <div class="div6">
        <asp:Label ID="lblJ77ID" runat="server" Text="Pracujete s šablonou:" CssClass="val" AssociatedControlID="j77ID"></asp:Label>
        <asp:DropDownList ID="j77ID" runat="server" AutoPostBack="true" DataTextField="j77Name" DataValueField="pid" Style="width: 300px;" Font-Bold="true"></asp:DropDownList>


        <asp:HiddenField ID="hidIsOwner" runat="server" Value="0" />

        <asp:Label ID="lblName" runat="server" Text="Název pojmenované šablony:" CssClass="lbl" AssociatedControlID="j77Name"></asp:Label>
        <asp:TextBox ID="j77Name" runat="server" Style="width: 300px; background-color: #99CCFF;" />
        <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" ToolTip="Odstranit pojmenovanou šablonu" CssClass="button-link" OnClientClick="return trydel();" />

    </div>

    <div class="div6">
        <span>První sloupec (agregační úroveň):</span><telerik:RadComboBox ID="dd1" runat="server" AutoPostBack="true"></telerik:RadComboBox>

        <span>Druhá agregační úroveň:</span>
        <telerik:RadComboBox ID="dd2" runat="server" AutoPostBack="true"></telerik:RadComboBox>
    </div>

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Další agregační sloupce statistiky" Value="core" meta:resourcekey="RadTabStrip1_core"></telerik:RadTab>
            <telerik:RadTab Text="SUM veličiny statistiky" Value="sums" meta:resourcekey="RadTabStrip1_sums" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="other"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server">

            <table cellpadding="8">
                <tr valign="top">
                    <td>
                        
                        <telerik:RadListBox ID="colsSource" Height="500px" runat="server" AllowTransfer="true" TransferMode="Move" TransferToID="colsDest" SelectionMode="Single" Culture="cs-CZ" AllowTransferOnDoubleClick="true" Width="350px" AutoPostBackOnReorder="false" AutoPostBackOnDelete="false" AutoPostBackOnTransfer="false">
                            <ButtonSettings TransferButtons="All" ShowTransferAll="false" />

                            <Localization ToRight="Přesunout" ToLeft="Odebrat" AllToRight="Přesunout vše" AllToLeft="Odbrat vše" MoveDown="Posunout dolu" MoveUp="Posunout nahoru" />
                        </telerik:RadListBox>
                    </td>
                    <td>
                        <div><%=Resources.grid_designer.VybraneSloupce %></div>
                        <telerik:RadListBox ID="colsDest" runat="server" AllowReorder="true" AllowTransferOnDoubleClick="true" Culture="cs-CZ" Width="250px" SelectionMode="Single">

                            <EmptyMessageTemplate>
                                <div style="padding-top: 50px;">
                                    <%=Resources.grid_designer.ZadneVybraneSloupce %>
                                </div>
                            </EmptyMessageTemplate>
                        </telerik:RadListBox>
                        <div>

                            <asp:DropDownList ID="cbxMaxMinAll" runat="server" ToolTip="Okruh zobrazovaných hodnot pole ve statistice" AutoPostBack="true">
                                <asp:ListItem Text="Změnit u pole typ agregace hodnot na:" Value=""></asp:ListItem>
                                <asp:ListItem Text="Všechny hodnoty pole (ALL)" Value="all"></asp:ListItem>
                                <asp:ListItem Text="Maximální hodnota pole (MAX)" Value="max"></asp:ListItem>
                                <asp:ListItem Text="Minimální hodnota pole (MIN)" Value="min"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="sum2" runat="server" Selected="true">
            <table cellpadding="8">
                <tr valign="top">
                    <td>
                        
                        <telerik:RadListBox ID="sumsSource" Height="500px" runat="server" AllowTransfer="true" TransferMode="Move" TransferToID="sumsDest" SelectionMode="Single" Culture="cs-CZ" AllowTransferOnDoubleClick="true" Width="350px" AutoPostBackOnReorder="false" AutoPostBackOnDelete="false" AutoPostBackOnTransfer="false">
                            <ButtonSettings TransferButtons="All" ShowTransferAll="false" />

                            <Localization ToRight="Přesunout" ToLeft="Odebrat" AllToRight="Přesunout vše" AllToLeft="Odbrat vše" MoveDown="Posunout dolu" MoveUp="Posunout nahoru" />
                        </telerik:RadListBox>
                    </td>
                    <td>
                        <div>Vybrané veličiny</div>
                        <telerik:RadListBox ID="sumsDest" runat="server" AllowReorder="true" AllowTransferOnDoubleClick="true" Culture="cs-CZ" Width="350px" SelectionMode="Single">

                            <EmptyMessageTemplate>
                                <div style="padding-top: 50px;">
                                    <%=Resources.grid_designer.ZadneVybraneSloupce %>
                                </div>
                            </EmptyMessageTemplate>
                        </telerik:RadListBox>

                    </td>

                </tr>
            </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="other" runat="server">

            <asp:Panel ID="panRoles" runat="server" CssClass="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="Label1" runat="server" Text="Přístupová práva k šabloně pro další osoby"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="j77WorksheetStatTemplate" EmptyDataMessage="K šabloně nejsou definována přístupová práva, proto bude přístupná pouze Vám."></uc:entityrole_assign>

                </div>
            </asp:Panel>

            <asp:Panel ID="panJ70ID" runat="server" CssClass="div6">
                <div>
                    <span>Svázat šablonu s pojmenovaným filtrem:</span>
                    <asp:DropDownList ID="j70ID" runat="server" DataTextField="NameWithMark" DataValueField="pid"></asp:DropDownList>
                </div>
                <div>
                    <span>Druh úkonů</span>
                    <asp:DropDownList ID="j77TabQueryFlag" runat="server">
                        <asp:ListItem Text="Bez filtrování" Value="p31" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Pouze hodiny" Value="time"></asp:ListItem>
                        <asp:ListItem Text="Pouze výdaje" Value="expense"></asp:ListItem>
                        <asp:ListItem Text="Paušální odměny" Value="fee"></asp:ListItem>
                        <asp:ListItem Text="Pouze kusovník" Value="kusovnik"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    <asp:Label runat="server" ID="Label2" Text="Index pořadí:"></asp:Label>
                    <telerik:RadNumericTextBox ID="j77Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                </div>
            </asp:Panel>
            <div style="padding: 6px; margin-top: 30px;">
                <i>
                    <asp:Label ID="lblTimeStamp" runat="server"></asp:Label>
                </i>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
    <script type="text/javascript">
        function trydel() {

            if (confirm("Opravdu odstranit záznam?")) {

                return (true);
            }
            else {
                return (false);
            }
        }
    </script>
</asp:Content>
