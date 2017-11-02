<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p45_vysledovka.ascx.vb" Inherits="UI.p45_vysledovka" %>
<div class="content-box2">
    <div class="title">
        Výsledovka rozpočtu
            
    </div>
    <div class="content">
        <table cellpadding="10">
            <tr>
                <th></th>
                
                <th colspan="2">Náklady</th>

                <th colspan="2">Výnosy</th>
                <th>
                    <img src="Images/sum.png" />
                </th>
            </tr>
            <tr>
                <td>[Časový rozpočet]</td>
                
                <td>Nákladová cena hodin:</td>
                <td align="right">
                    <asp:Label ID="total_costfee" runat="server" CssClass="val" ForeColor="red"></asp:Label>
                </td>
                <td>Fakturační cena hodin:

                </td>
                <td align="right">
                    <asp:Label ID="total_billingfee" runat="server" CssClass="val" ForeColor="green"></asp:Label>
                </td>
                <td align="right">
                    <asp:Label ID="result_t" runat="server" CssClass="val"></asp:Label>                    
                </td>
                <td></td>
            </tr>
            <tr>
                <td>[Finanční rozpočet]</td>
                
                <td>Výdaje:

                </td>
                <td align="right">
                    <asp:Label ID="total_expense" runat="server" CssClass="val" ForeColor="red"></asp:Label>
                </td>
                <td>Pevné (paušální) odměny:

                </td>
                <td align="right">
                    <asp:Label ID="total_income" runat="server" CssClass="val" ForeColor="green"></asp:Label>
                </td>
                <td align="right">
                    <asp:Label ID="result_m" runat="server" CssClass="val"></asp:Label>                    
                </td>
                <td></td>
            </tr>
            <tr style="border-top: solid 1px gray;">
                <td colspan="2">
                    <img src="Images/sum.png" />
                    <img src="Images/finplan.png" />
                    
                </td>
                <td>
                    <asp:Label ID="total_cost" runat="server" CssClass="valboldred"></asp:Label>
                </td>
                <td></td>
                <td>
                    <asp:Label ID="total_billing" runat="server" CssClass="valbold" ForeColor="green"></asp:Label>
                </td>
                <td align="right">
                    <asp:Label ID="result_lost" runat="server" CssClass="valboldred"></asp:Label>
                    <asp:Label ID="result_profit" runat="server" CssClass="valboldblue"></asp:Label>
                </td>
                <td>
                    <asp:Image ID="imgEmotion" runat="server" ImageUrl="Images/emotion_amazing.png" />
                </td>
            </tr>
            
        </table>
    </div>
</div>
