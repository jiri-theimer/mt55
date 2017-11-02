<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_periodcombo.aspx.vb" Inherits="UI.clue_periodcombo" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function fireperiod(radioButtonList) {
            var val = GetRadioButtonListSelectedValue(radioButtonList);           
            window.parent.hardrefresh_periodcombo(val);

        }


        function GetRadioButtonListSelectedValue(radioButtonList) {

            for (var i = 0; i < radioButtonList.rows.length; ++i) {

                if (radioButtonList.rows[i].cells[0].firstChild.checked) {

                    var s = radioButtonList.rows[i].cells[0].firstChild.value;
                    window.parent.hardrefresh_periodcombo(s);

                }

            }

        }

        function periodcombo_setting() {
            window.parent.periodcombo_setting();
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float: left;">
        <fieldset style="padding: 6px;">
            <legend>Datumový filtr (období)</legend>

            <div style="float: left;">
                <telerik:RadDatePicker ID="d1" runat="server" SharedCalendarID="SharedCalendar" Width="120px" MaxDate="1.1.3000" MinDate="1.1.1900">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </div>
            <div style="float: left;">
                <telerik:RadDatePicker ID="d2" runat="server" SharedCalendarID="SharedCalendar" Width="120px" MaxDate="1.1.3000" MinDate="1.1.1900">
                    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </div>
            <div style="float: left;">
                <asp:Button ID="cmdSubmit" runat="server" Text="OK" CssClass="cmd" />
            </div>



        </fieldset>
    </div>

    <div style="float: right;">
        <fieldset style="padding: 6px;">
            <legend>Uložit rozsah datumů do pojmenovaných období</legend>
            <span>Název období:</span>
            <asp:TextBox ID="txtPeriodName" runat="server" Style="width: 140px;"></asp:TextBox>
            <asp:Button ID="cmdSave" runat="server" Text="Uložit" CssClass="cmd" />
        </fieldset>
    </div>

    <div style="clear: both;">
        <br />
    </div>
    <fieldset>
        <legend>Seznam mých pojmenovaných období
        </legend>
        <asp:RadioButtonList ID="period1" runat="server" AutoPostBack="false" onclick="GetRadioButtonListSelectedValue(this);" RepeatDirection="Vertical" CellPadding="3" DataValueField="ComboValue" DataTextField="NameWithDates">
        </asp:RadioButtonList>
       
    </fieldset>
    <div style="float: left;">
        <button type="button" onclick="periodcombo_setting()">Více nastavení</button>
    </div>
    <div style="float: right;">
        <img src="Images/datepicker_32.png" />
    </div>



    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">

        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>
    <asp:HiddenField ID="hidCustomQueries" runat="server" />

</asp:Content>
