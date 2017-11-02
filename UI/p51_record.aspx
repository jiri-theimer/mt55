<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p51_record.aspx.vb" Inherits="UI.p51_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function add_p52() {
            dialog_master("p52_record.aspx?guid=<%=ViewState("guid")%>")


        }

        function edit_p52(p85id) {

            dialog_master("p52_record.aspx?guid=<%=ViewState("guid")%>&p85id=" + p85id);

            return (false);

        }

        function clone_p52(p85id) {


            dialog_master("p52_record.aspx?guid=<%=ViewState("guid")%>&clone=1&p85id=" + p85id);

            return (false);

        }

        function hardrefresh(pid, flag) {
        <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefreshOnBehind, "", False)%>

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Sazby" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="p52"></telerik:RadTab>


        </Tabs>
    </telerik:RadTabStrip>

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td style="width: 150px;">
                <asp:Label ID="lblJ27ID" Text="Měna sazeb:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j27ID" runat="server" DataTextField="j27Code" DataValueField="pid" AutoPostBack="true"></uc:datacombo>


            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název ceníku:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p51Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
    </table>

    <asp:Panel ID="panWizard" runat="server" Visible="false">
     
        <asp:RadioButtonList ID="opg1" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                <asp:ListItem Text="Sazby podle pozic" Value="j07" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Sazby podle lidí" Value="j02"></asp:ListItem>
            </asp:RadioButtonList>
        <table cellpadding="6">
            <tr>
                <th>Položka</th>
                <th>Výše sazby</th>
            </tr>
            <asp:Repeater ID="rpWizard" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="item1" runat="server" CssClass="val"></asp:Label>
                            <asp:HiddenField ID="pid" runat="server" />
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="rate1" runat="server" Width="80px"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="RadPageView1" runat="server" Selected="true">

            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td style="width: 150px;">

                        <asp:Label ID="lblDefaultRate" Text="Výchozí hodinová sazba:" runat="server" CssClass="lbl" AssociatedControlID="p51DefaultRateT"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p51DefaultRateT" runat="server" Width="90px"></telerik:RadNumericTextBox>
                    </td>
                </tr>
            </table>



            <div class="content-box2">
                <div class="title">
                    <asp:Label ID="boxTitle" runat="server" Text="Položkové sazby (vyjímky z výchozí sazby ceníku)"></asp:Label>
                    <button type="button" onclick="add_p52()">Přidat</button>
                </div>
                <div class="content">
                    <table cellpadding="5" cellspacing="2">
                        <asp:Repeater ID="rp1" runat="server">
                            <ItemTemplate>
                                <tr valign="top">
                                    <td>
                                        <asp:ImageButton ID="edit" runat="server" ToolTip="Upravit" ImageUrl="Images/edit.png" CssClass="button-link"></asp:ImageButton>
                                    </td>
                                    <td>
                                        <asp:Label ID="Subject" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="p34Name" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="p32Name" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="p52Rate" runat="server" Style="color: red;"></asp:Label>

                                    </td>
                                    <td>
                                        <i>
                                            <asp:Label ID="p52Name" runat="server"></asp:Label></i>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="clone" runat="server" ImageUrl="Images/copy.png" ToolTip="Kopírovat" CssClass="button-link" />

                                    </td>
                                    <td>
                                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />

                                    </td>
                                </tr>

                            </ItemTemplate>

                        </asp:Repeater>

                    </table>
                </div>
            </div>

        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <table cellpadding="10" cellspacing="2">
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="p51IsCustomTailor" runat="server" Text="Sazby na míru pouze pro jeden projekt/klient" />


                    </td>

                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="p51IsInternalPriceList" runat="server" Text="Jedná ceník interních (nákladových) sazeb" />


                    </td>

                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="p51IsMasterPriceList" runat="server" Text="Jedná se o MASTER ceník" AutoPostBack="true" />
                        
                    </td>
                </tr>
               
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblp51ID_Master" Text="Vazba na MASTER ceník:" runat="server" CssClass="lbl"></asp:Label>
                        <uc:datacombo ID="p51ID_Master" runat="server" DataTextField="NameWithCurr" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblCode" Text="Kód:" runat="server" CssClass="lbl" AssociatedControlID="p51Code"></asp:Label>

                    </td>
                    <td>
                        <asp:TextBox ID="p51Code" runat="server" Style="width: 100px;"></asp:TextBox>




                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="p51Ordinary"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p51Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <asp:Button ID="cmdHardRefreshOnBehind" runat="server" Style="display: none;" />


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
