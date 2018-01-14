<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p11_pass.aspx.vb" Inherits="UI.p11_pass" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div style="background-color:white;">
    <telerik:RadDatePicker ID="datToday" runat="server" Width="120px" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red" AutoPostBack="true" SharedCalendarID="SharedCalendar">
        <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
    </telerik:RadDatePicker>
    <asp:Button ID="cmdNew" runat="server" CssClass="cmd" Text="Zapsat záznam docházky" />

    <asp:Panel ID="panRecord" runat="server" CssClass="content-box2" style="width:600px;" Visible="false">
        <div class="title">
            <img src="Images/new.png" />
            Záznam docházky
            <asp:Button ID="cmdSave" runat="server" Text="Uložit záznam" CssClass="cmd" />
        </div>
        <div class="content">
            
            <table cellpadding="8" cellspacing="4">
              
                <tr>
                    <td>
                        <asp:RadioButtonList ID="p12Flag" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Příchod" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Odchod" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="3"></asp:ListItem>
                        </asp:RadioButtonList>

                        
                    </td>
                    <td>
                        <asp:DropDownList ID="p32ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p32Name" style="max-width:300px;"></asp:DropDownList>
                    </td>
                </tr>
                
                
                
            </table>
            <table cellpadding="8" cellspacing="4">
                <tr>
                    <td>
                        Čas:
                    </td>
                    <td>
                        <telerik:RadDateTimePicker ID="p12TimeStamp" runat="server" Width="80px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput2" DisplayDateFormat="HH:mm" DateFormat="HH:mm" runat="server"></DateInput>
                            <TimePopupButton Visible="true" />
                            <DatePopupButton Visible="false" />
                            <TimeView StartTime="05:00" EndTime="23:00" ShowHeader="false" ShowFooter="false"></TimeView>

                        </telerik:RadDateTimePicker>
                    </td>
                </tr>
                <tr>
                    <td>
                        Poznámka:
                    </td>
                    <td>
                        <asp:TextBox ID="p12Description" runat="server" Width="400px"></asp:TextBox>
                    </td>
                </tr>
            </table>
          
        </div>

    </asp:Panel>


    <table cellpadding="6">
        <tr>
            <th>
            </th>
            <th>Čas
            </th>
            <th>Trvání
            </th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr class="trHover">
                    <td>
                        <asp:Label ID="p32Name" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="p12TimeStamp" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Duration" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="p12Description" runat="server" Font-Italic="true"></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

    <asp:HiddenField ID="hidP11ID" runat="server" />


    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>

</div>
</asp:Content>
