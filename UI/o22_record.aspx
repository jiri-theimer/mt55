<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o22_record.aspx.vb" Inherits="UI.o22_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function o22DateUntil_DateChanged(sender, eventArgs) {
            <%If Me.CurrentO21Flag <> BO.o21FlagEnum.DeadlineOrMilestone Then%>
            return;
            <%End If%>
            var d1 = eventArgs.get_newDate();
            var d2 = new Date(d1);
            d2.setHours(d1.getHours() - 1)
            var reminder = $find("<%=o22ReminderDate.ClientID%>");
            reminder.set_selectedDate(d2);

        }

        function o22DateFrom_DateChanged(sender, eventArgs) {
            <%If Me.CurrentO21Flag = BO.o21FlagEnum.EventFromUntil Then%>

            var d1 = eventArgs.get_newDate();
            var d2 = new Date(d1);
            d2.setHours(d1.getHours() - 1)
            var reminder = $find("<%=o22ReminderDate.ClientID%>");
            reminder.set_selectedDate(d2);

            var konec = $find("<%=Me.o22DateUntil.ClientID%>");
            var d3 = konec.get_selectedDate();

            if (d3 < d1) {
                var d4 = new Date(d1);
                d4.setHours(d1.getHours() + 1)
                konec.set_selectedDate(d4);
            }


            <%End If%>
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



    <table cellpadding="5" cellspacing="2">
        <tr>
            <td style="width: 140px;">
                <asp:Label ID="lblO21ID" Text="Typ události:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="o21ID" runat="server" DataTextField="o21Name" DataValueField="pid" IsFirstEmptyRow="false" AutoPostBack="true" Width="400px"></uc:datacombo>
                <asp:Image ID="imgO21Flag" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblObject" runat="server" CssClass="lbl" Text="Entita:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="BoundObject" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr style="border-top: dashed gray 1px;">
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název (předmět):"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="o22Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDateFrom" Text="Začátek:" runat="server" AssociatedControlID="o22DateFrom" CssClass="lbl"></asp:Label></td>
            <td>
                <telerik:RadDateTimePicker ID="o22DateFrom" runat="server" Width="190px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                    <TimePopupButton Visible="true" />
                    <TimeView StartTime="06:00" EndTime="23:59" ShowHeader="false" ShowFooter="false"></TimeView>
                    <ClientEvents OnDateSelected="o22DateFrom_DateChanged" />
                </telerik:RadDateTimePicker>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDateUntil" Text="Konec:" runat="server" AssociatedControlID="o22DateUntil" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <telerik:RadDateTimePicker ID="o22DateUntil" runat="server" Width="190px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                    <TimePopupButton Visible="true" />
                    <TimeView StartTime="06:00" EndTime="23:59" ShowHeader="false" ShowFooter="false"></TimeView>
                    <ClientEvents OnDateSelected="o22DateUntil_DateChanged" />
                </telerik:RadDateTimePicker>
                <asp:CheckBox ID="o22IsAllDay" runat="server" Text="Událost trvá celý den" AutoPostBack="true" />
            </td>
        </tr>

        <tr style="border-top: dashed gray 1px;">
            <td>
                <asp:Label ID="lblLocation" Text="Lokalita:" runat="server" AssociatedControlID="o22Location" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="o22Location" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo22ReminderDate" Text="Čas připomenutí:" runat="server" AssociatedControlID="o22ReminderDate" CssClass="lbl"></asp:Label></td>
            <td>
                <telerik:RadDateTimePicker ID="o22ReminderDate" runat="server" Width="190px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                    <TimePopupButton Visible="true" />
                    <TimeView runat="server" StartTime="06:00" EndTime="23:59" ShowHeader="false" ShowFooter="false"></TimeView>
                </telerik:RadDateTimePicker>

            </td>
        </tr>

    </table>
    <asp:Panel ID="panO20" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/person.png" style="margin-right: 5px;" />
            Zapojení osob do události

        </div>
        <div class="content">
            <div class="div6">
                <asp:Label ID="lblSelectJ02ID" runat="server" Text="Vybrat osobu:" CssClass="lbl"></asp:Label>
                <uc:person ID="j02ID_Search" runat="server" Width="200px" AutoPostBack="true" Flag="all" />
                <asp:Label ID="lblSelectJ11ID" runat="server" Text=" nebo tým osob:" CssClass="lbl"></asp:Label>
                <uc:datacombo ID="cbxSelectJ11ID" runat="server" DataTextField="j11Name" DataValueField="pid" IsFirstEmptyRow="true" Filter="Contains" AutoPostBack="true" Width="200px"></uc:datacombo>
            </div>
            <div class="div6">
                <asp:Repeater ID="rpO20" runat="server">
                    <ItemTemplate>
                        <asp:Image ID="img1" runat="server" />
                        <asp:Label ID="Source" runat="server" CssClass="valboldblue"></asp:Label>
                        <asp:ImageButton ID="cmdDelete" runat="server" CommandName="delete" ImageUrl="Images/delete.png" CssClass="button-link" ToolTip="Odstranit položku"></asp:ImageButton>
                        <span style="padding-left: 20px;"></span>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </div>
    </asp:Panel>
    
    <uc:freefields ID="ff1" runat="server" />

    <div class="content-box2">
        <div class="title">Poznámka</div>
        <div class="content">
            <asp:TextBox ID="o22Description" runat="server" Style="height: 90px; width: 99%;" TextMode="MultiLine"></asp:TextBox>
        </div>

    </div>
    <div class="div6">
        <asp:CheckBox ID="o22IsNoNotify" runat="server" Text="V události vypnout automatické e-mail notifikace" CssClass="chk" />
    </div>
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td style="width: 140px;">
                <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>

            </td>
            <td>
                <uc:person ID="j02ID_Owner" runat="server" Width="150px" />

            </td>
        </tr>
    </table>

    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>
    <asp:HiddenField ID="hidX29ID" runat="server" />
    <asp:HiddenField ID="hidMasterDataPID" runat="server" />
    <asp:HiddenField ID="hidO21Flag" runat="server" Value="1" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
