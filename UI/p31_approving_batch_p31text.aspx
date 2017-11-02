<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_approving_batch_p31text.aspx.vb" Inherits="UI.p31_approving_batch_p31text" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <span>Třidit podle:</span>
        <asp:DropDownList ID="cbxSort1" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Datum sestupně" Value="1"></asp:ListItem>
            <asp:ListItem Text="Datum vzestupně" Value="2"></asp:ListItem>
            <asp:ListItem Text="Osoba" Value="3"></asp:ListItem>
            <asp:ListItem Text="Projekt" Value="4"></asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="cbxSort2" runat="server" AutoPostBack="true">
            <asp:ListItem Text="" Value=""></asp:ListItem>
            <asp:ListItem Text="Datum sestupně" Value="1"></asp:ListItem>
            <asp:ListItem Text="Datum vzestupně" Value="2"></asp:ListItem>
            <asp:ListItem Text="Osoba" Value="3"></asp:ListItem>
            <asp:ListItem Text="Projekt" Value="4"></asp:ListItem>
        </asp:DropDownList>
        <i>Změnou třídění se ztratí dosavadní ručně provedené změny!</i>
    </div>
    <table cellpadding="5" cellspacing="2">
        <tr>
            <th></th>
            <th></th>
            <th>Datum/Osoba</th>
           
            <th>Projekt</th>
            <th>Aktivita</th>
            <th>Hodnota<div>k fakturaci</div></th>
            <th>Fakturační<div>sazba</div></th>
            <th>Popis</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr valign="top" class="trHover">
                    <td style="width:35px;">
                        <asp:Label ID="RowIndex" runat="server"></asp:Label>
                        <asp:Image ID="img1" runat="server" />
                        <asp:HiddenField ID="p31id" runat="server" />
                        <asp:HyperLink ID="go2pid" runat="server"></asp:HyperLink>
                    </td>
                    <td>
                        <asp:DropDownList ID="p72ID" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="Zůstane rozpracované" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Fakturovat" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Zahrnout do paušálu" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Viditelný odpis" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Skrytý odpis" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Fakturovat později" Value="7"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label ID="p31date" runat="server" CssClass="val"></asp:Label>
                        <div>
                            <asp:Label ID="Person" runat="server" CssClass="val" ForeColor="blue"></asp:Label>
                        </div>
                    </td>
                
                    <td>
                        <div>
                            <asp:Label ID="Client" runat="server" CssClass="val" ForeColor="blue"></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="p41Name" runat="server" CssClass="val"></asp:Label>
                        </div>
                        
                    </td>
                    <td style="max-width:200px;">
                        <div>
                            <asp:Label ID="p34Name" runat="server" CssClass="val"></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="p32Name" runat="server" CssClass="val"></asp:Label>
                        </div>
                        
                    </td>
                    <td align="right">
                        <div>
                            <asp:TextBox ID="Value_Edit" runat="server" style="width:40px;text-align:right;"></asp:TextBox>
                        </div>
                        <asp:Label ID="p31Value_Orig" runat="server" CssClass="val"></asp:Label>
                        <div>
                            <asp:Label ID="lblValue_Edit_FixPrice" runat="server" Text="Hodnota v paušálu:"></asp:Label>
                            <asp:TextBox ID="Value_Edit_FixPrice" runat="server" style="width:40px;text-align:right;"></asp:TextBox>
                            
                        </div>
                    </td>
                    <td align="right">
                        <div>
                        <telerik:RadNumericTextBox ID="Rate_Edit" runat="server" NumberFormat-DecimalDigits="2" Width="60px">
                        </telerik:RadNumericTextBox>
                        </div>
                        <asp:Label ID="Currency" runat="server" CssClass="val"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p31Text" runat="server" TextMode="MultiLine" style="width:400px;height:50px;"></asp:TextBox>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            
        });

        
        function go2id(id) {
            
            window.location.href = "#" + id;
            
           
        }
    </script>
</asp:Content>
