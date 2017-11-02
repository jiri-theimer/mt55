<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="query_builder.aspx.vb" Inherits="UI.query_builder" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="period" Src="~/period.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background-color: #F1F1F1;">
        <div>

            <asp:Image ID="imgEntity" runat="server" ImageUrl="Images/griddesigner_32.png" />
            <asp:Label ID="ph1" runat="server" Text="Návrhář filtrů" CssClass="page_header_span"></asp:Label>
            <span style="padding-left: 20px;"></span>
            <button id="cmdNew" runat="server" type="button" onclick="trycreate()"><img src="Images/new.png" /> Nový přehled</button>
            

        </div>

        <div class="div6" style="border-bottom: dashed 1px gray;">
            <asp:Label ID="lblJ70ID" runat="server" Text="Pojmenovaný přehled:" CssClass="val" AssociatedControlID="j70ID"></asp:Label>
            <asp:DropDownList ID="j70ID" runat="server" AutoPostBack="true" DataTextField="NameWithMark" DataValueField="pid" Style="width: 300px; background-color: yellow;" Font-Bold="true"></asp:DropDownList>


            <asp:HiddenField ID="j70IsSystem" runat="server" />
            <asp:HiddenField ID="hidIsOwner" runat="server" Value="0" />

            <asp:Label ID="lblName" runat="server" Text="Název přehledu:" CssClass="lbl" AssociatedControlID="j70Name"></asp:Label>
            <asp:TextBox ID="j70Name" runat="server" Style="width: 300px; background-color: #99CCFF;" />
            <asp:ImageButton ID="cmdSave" runat="server" ImageUrl="Images/save.png" ToolTip="Uložit změny" CssClass="button-link" />
            <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" ToolTip="Odstranit šablonu přehledu" CssClass="button-link" OnClientClick="return trydel();" />


        </div>
    </div>
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true" Skin="MetroTouch">
        <Tabs>
            <telerik:RadTab Text="Filtrovací podmínka" Value="query"></telerik:RadTab>
            <telerik:RadTab Text="Nastavení sloupců" Value="columns" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Přístupová práva" Value="perm"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="query" runat="server">

            <asp:Panel ID="panEditHeader" runat="server">
            </asp:Panel>




            <asp:Panel ID="panQueryCondition" runat="server" CssClass="content-box2">
                <div class="title">
                    <asp:DropDownList ID="cbxQueryField" runat="server" AutoPostBack="true" ToolTip="Filtrovací pole" Style="min-width: 400px;"></asp:DropDownList>

                </div>
                <div class="content">



                    <asp:Panel ID="panQueryItems" runat="server" CssClass="div6" Visible="false">
                        <asp:Label ID="lbl1" runat="server" CssClass="lbl" Text="Hodnoty filtrovacího pole:"></asp:Label>
                        <uc:datacombo ID="cbxItems" runat="server" DataValueField="pid" AutoPostBack="false" IsFirstEmptyRow="false" Width="400px" AllowCheckboxes="true"></uc:datacombo>

                        <uc:datacombo ID="cbxItemsExtension" runat="server" DataValueField="pid" AutoPostBack="false" IsFirstEmptyRow="true" Width="200px" AllowCheckboxes="true"></uc:datacombo>

                        <asp:Button ID="cmdAdd2Query" runat="server" CssClass="cmd" Text="Přidat do podmínky filtru" />

                    </asp:Panel>
                    <asp:Panel ID="panQueryNonItems" runat="server" CssClass="div6" Visible="false">

                        <span>Hodnota od:</span>
                        <asp:TextBox ID="j71ValueFrom" runat="server" Width="100px"></asp:TextBox>
                        <span>do: </span>
                        <asp:TextBox ID="j71ValueUntil" runat="server" Width="100px"></asp:TextBox>

                        <asp:Button ID="cmdAdd2QueryNonItems" runat="server" CssClass="cmd" Text="Přidat do podmínky filtru" />
                    </asp:Panel>
                    <asp:Panel ID="panQueryPeriod" runat="server" CssClass="div6" Visible="false">
                        <uc:period ID="period1" runat="server" Caption="Filtrované období" />
                        <asp:Button ID="cmdAdd2QueryPeriod" runat="server" CssClass="cmd" Text="Přidat do podmínky filtru" />
                    </asp:Panel>
                    <asp:Panel ID="panQueryString" runat="server" CssClass="div6" Visible="false">
                        <asp:DropDownList ID="cbxStringOperator" runat="server" AutoPostBack="true">
                            <asp:ListItem Value="CONTAIN" Text="Obsahuje"></asp:ListItem>
                            <asp:ListItem Value="START" Text="Začíná na"></asp:ListItem>
                            <asp:ListItem Value="EQUAL" Text="Je rovno"></asp:ListItem>
                            <asp:ListItem Value="NOTEMPTY" Text="Není prázdné"></asp:ListItem>
                            <asp:ListItem Value="EMPTY" Text="Je prázdné"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtStringValue" runat="server" Width="100px"></asp:TextBox>
                        <asp:Button ID="cmdAdd2QueryString" runat="server" CssClass="cmd" Text="Přidat do podmínky filtru" />
                    </asp:Panel>
                </div>
            </asp:Panel>
            <asp:Panel ID="panJ71" runat="server" CssClass="content-box2" Style="margin-top: 20px;">
                <div class="title">
                    Podmínka celého filtru
            <asp:Button ID="cmdClear" runat="server" CssClass="cmd" Text="Vyčistit podmínku filtru" UseSubmitBehavior="false" Style="margin-left: 40px;" />
                    <asp:CheckBox ID="j70IsNegation" runat="server" Text="Negovat podmínku celého filtru" ToolTip="Pokud zaškrtnuto, filtr vrací záznamy nevyhovující filtrovací podmínce." Style="float: right;" />
                </div>
                <div class="content" >
                    <table cellpadding="3" cellspacing="2">
                        <asp:Repeater ID="rpJ71" runat="server">
                            <ItemTemplate>
                                <tr class="trHover">
                                    <td style="min-width: 150px;">
                                        <asp:Label ID="x29Name" runat="server" CssClass="val" Style="color: Gray;"></asp:Label>
                                        <asp:HiddenField ID="x29id" runat="server" />
                                        <asp:HiddenField ID="p85id" runat="server" />
                                        <asp:HiddenField ID="j71Field" runat="server" />
                                    </td>
                                    <td>
                                        <i>
                                            <asp:Label ID="nebo" runat="server" CssClass="val" Style="color: blue;" Text="nebo"></asp:Label>
                                        </i>
                                        <asp:Label ID="j71RecordName" runat="server" CssClass="val"></asp:Label>
                                        <asp:HiddenField ID="j71RecordPID" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="j71RecordName_Extension" runat="server" CssClass="val" Style="color: red;"></asp:Label>
                                        <asp:HiddenField ID="j71RecordPID_Extension" runat="server" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                    </td>
                                </tr>
                            </ItemTemplate>

                        </asp:Repeater>
                    </table>
                </div>
            </asp:Panel>


            <div class="div6" style="margin-top: 40px;">
                <span>Rámcová podmínku filtru:</span>
                <asp:DropDownList ID="opgBin" runat="server">
                    <asp:ListItem Text="Otevřené i archivované záznamy" Value="0" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Pouze otevřené záznamy" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Pouze záznamy přesunuté do archivu" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </div>

        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="columns" Selected="true">

            

            <div style="float: left; width: 300px;">
                <div><%=Resources.grid_designer.DostupneSloupce %></div>
                <telerik:RadTreeView ID="tr1" runat="server" Skin="Default" ShowLineImages="false" SingleExpandPath="true" Height="500px">
                </telerik:RadTreeView>
            </div>
            <div style="float: left; padding: 10px;">
                <asp:Button ID="cmdAdd" runat="server" CssClass="cmd" Text=">" ToolTip="Vybrat sloupec do přehledu" />
                <p></p>
                <asp:Button ID="cmdRemove" runat="server" CssClass="cmd" Text="<" ToolTip="Odstranit sloupec z přehledu" />
            </div>
            <div style="float: left;">
                <div><%=Resources.grid_designer.VybraneSloupce %></div>
                <telerik:RadListBox ID="lt1" runat="server" AllowReorder="true" AllowTransferOnDoubleClick="false" Culture="cs-CZ" Width="350px" SelectionMode="Single">

                    <EmptyMessageTemplate>
                        <div style="padding-top: 50px;">
                            <%=Resources.grid_designer.ZadneVybraneSloupce %>
                        </div>
                    </EmptyMessageTemplate>
                </telerik:RadListBox>

                <div style="margin-top: 20px;">
                    <asp:RadioButtonList ID="j70ScrollingFlag" runat="server" RepeatDirection="Vertical">
                        <asp:ListItem Text="Pevné ukotvení záhlaví tabulky (názvy sloupců)" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Bez podpory ukotvení" Value="0"></asp:ListItem>
                    </asp:RadioButtonList>

                </div>
               
                <div style="margin-top: 20px;">
                    <asp:CheckBox ID="j70IsFilteringByColumn" runat="server" CssClass="chk" Checked="true" Text="V přehledu nabízet sloupcový filtr" />
                </div>

                <div style="margin-top: 10px;">
                    <span><%=Resources.grid_designer.AutomatickyTriditPodle %> 1):</span>
                    <asp:DropDownList ID="cbxOrderBy1" runat="server" DataTextField="ColumnHeader" DataValueField="ColumnSqlSyntax_OrderBy"></asp:DropDownList>
                    <asp:DropDownList ID="cbxOrderBy1Dir" runat="server">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="<%$Resources:grid_designer,Sestupne %>" Value="DESC"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    <span><%=Resources.grid_designer.AutomatickyTriditPodle %> 2):</span>
                    <asp:DropDownList ID="cbxOrderBy2" runat="server" DataTextField="ColumnHeader" DataValueField="ColumnSqlSyntax_OrderBy"></asp:DropDownList>
                    <asp:DropDownList ID="cbxOrderBy2Dir" runat="server">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="<%$Resources:grid_designer,Sestupne %>" Value="DESC"></asp:ListItem>
                    </asp:DropDownList>
                </div>


                <div style="margin-top: 20px;">
                    <div>
                        Rozvržení panelů tohoto přehledu:
                    </div>
                    <asp:RadioButtonList ID="j70PageLayoutFlag" runat="server" RepeatDirection="Vertical">
                        <asp:ListItem Text="Výchozí nastavení uživatele" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Levý + pravý panel" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Pouze jeden panel -> Přehled na celou stránku" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Horní + spodní panel -> Přehled na celou šířku stránky" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>

                </div>
               
            </div>
            <div style="clear:both;"></div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="perm">
            <asp:Panel ID="panRoles" runat="server" CssClass="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="Label1" runat="server" Text="Přístupová práva k přehledu pro další uživatele"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="j70QueryTemplate" EmptyDataMessage="K přehledu zatím nejsou definována přístupová práva, proto bude přístupný pouze Vám."></uc:entityrole_assign>

                </div>
            </asp:Panel>
        </telerik:RadPageView>
    </telerik:RadMultiPage>


    <div style="padding: 6px; margin-top: 30px;">
        <i>
            <asp:Label ID="lblTimeStamp" runat="server"></asp:Label>
        </i>
    </div>

    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidNewJ70Name" runat="server" />
    <asp:HiddenField ID="hidModeFlag" runat="server" Value="1" />
    <asp:HiddenField ID="hidMasterprefixFlag" runat="server" />
    <asp:Button ID="cmdSaveNewTemplate" runat="server" style="display:none;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
    <script type="text/javascript">
        function trydel() {

            if (confirm("Opravdu odstranit tento pojmenovaný přehled?")) {
                return (true);
            }
            else {
                return (false);
            }
        }

        function trycreate() {
            var s = window.prompt("Zadejte název nového přehledu");

            if (s != '' && s != null) {
                self.document.getElementById("<%=hidNewJ70Name.clientid%>").value = s;

                <%=Me.ClientScript.GetPostBackEventReference(Me.cmdSaveNewTemplate, "", False)%>;
            }

            return (false);
        }
    </script>
</asp:Content>
