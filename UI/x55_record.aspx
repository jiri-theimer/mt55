<%@ Page Title="" Language="vb" AutoEventWireup="false" ValidateRequest="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x55_record.aspx.vb" Inherits="UI.x55_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název pro gadget:"></asp:Label></td>
            <td>
                <asp:TextBox ID="x55Name" runat="server" Style="width: 400px;"></asp:TextBox>
                <asp:Label ID="lblx55Code" runat="server" CssClass="lbl" Text="Kód:"></asp:Label>
                <asp:TextBox ID="x55Code" runat="server" Style="width: 200px;"></asp:TextBox>
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Typ obsahu:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x55TypeFlag" runat="server" AutoPostBack="true" Width="200px">                    
                    <asp:ListItem Text="Dynamický HTML obsah" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Statický HTML obsah" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Externí WWW stránka" Value="3"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="lblHeight" runat="server" CssClass="lbl" Text="Výška boxu:"></asp:Label>
                <asp:TextBox ID="x55Height" runat="server" Style="width: 50px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="x55Ordinary"></asp:Label></td>
            <td>

                <telerik:RadNumericTextBox ID="x55Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblx55RecordSQL" runat="server" CssClass="lbl" Text="SQL zdroj záznamu:"></asp:Label></td>
            <td>
                <asp:TextBox ID="x55RecordSQL" runat="server" Style="width: 650px;height:50px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
    </table>

    <asp:panel ID="panParams" runat="server" CssClass="content-box2">
        <div class="title">
            Parametry
            <asp:Button ID="cmdAdd" runat="server" Text="Přidat parametr" CssClass="cmd" />
        </div>
        <div class="content">
            <table cellpadding="10">
               <tr>
                   <th>Prvek</th>
                   <th>Název parameteru</th>
                   <th>Hodnota parametru</th>
                   <th></th>
               </tr>
                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <tr valign="top">
                            <td>
                                <asp:TextBox ID="x56Control" runat="server" Width="100px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="x56ControlPropertyName" runat="server" Width="100px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="x56ControlPropertyValue" runat="server" Width="500px" Height="50px" TextMode="MultiLine"></asp:TextBox>
                            </td>
                          
                            <td>
                                <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" CommandName="delete" />
                                <asp:HiddenField ID="p85id" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:panel>



    <div>
        <asp:Label ID="lblContentLabel" runat="server" CssClass="lbl" Text="HTML obsah:"></asp:Label>
    </div>
    
    <telerik:RadEditor ID="x55Content" EnableTextareaMode="true"  runat="server" Width="99%" Height="400px" AllowScripts="true">
        
    </telerik:RadEditor>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
