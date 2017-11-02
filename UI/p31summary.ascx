<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p31summary.ascx.vb" Inherits="UI.p31summary" %>
<table class="table table-hover">
<asp:Repeater ID="rp1" runat="server">
<ItemTemplate>    
    <tr>
        <td>
            Hodiny (rozpracované):           
        </td>
        <td style="text-align:right;">
            <asp:Label ID="hodiny_rozpracovano" runat="server"></asp:Label>
            <div>
                <asp:Label ID="honorar_rozpracovano" runat="server"></asp:Label>
            </div>
        </td>
        
    </tr>
    <tr>
        <td>Hodiny (schváleno k fakturaci):</td>
        <td style="text-align:right;">
            <asp:Label ID="hodiny_fakturovat" runat="server"></asp:Label>
            <div>
                <asp:Label ID="honorar_fakturovat" runat="server"></asp:Label>
            </div>
        </td>
       
    </tr>
    <tr>
        <td>Hodiny (paušál):</td>
        <td style="text-align:right;">
            <asp:Label ID="hodiny_pausal" runat="server"></asp:Label>
        </td>
        
    </tr>
    <tr>
        <td>Hodiny (odpis):</td>
        <td style="text-align:right;">
            <asp:Label ID="hodiny_odpis" runat="server"></asp:Label>
        </td>
       
    </tr>
    
    <tr>
        <td>
            Výdaje (rozpracované):            
        </td>
        <td style="text-align:right;">
            <asp:Label ID="vydaje_rozpracovano" runat="server"></asp:Label>
        </td>
        
    </tr>
    <tr>
        <td>
            Výdaje (schváleno k fakturaci):            
        </td>
        <td style="text-align:right;">
            <asp:Label ID="vydaje_fakturovat" runat="server"></asp:Label>
        </td>
        
    </tr>
    
    <tr>
        <td>
            Pevné odměny (rozpracované):            
        </td>
        <td style="text-align:right;">
            <asp:Label ID="odmeny_rozpracovano" runat="server"></asp:Label>
        </td>
        
    </tr>
    <tr>
        <td>
            Pevné odměny (k fakturaci):            
        </td>
        <td style="text-align:right;">
            <asp:Label ID="odmeny_fakturovat" runat="server"></asp:Label>
        </td>
        
    </tr>      
</ItemTemplate>
</asp:Repeater>
</table>

<asp:HiddenField ID="hidState" runat="server" Value="1" />
<asp:HiddenField ID="hidAllowShowRates" runat="server" Value="1" />