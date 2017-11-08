<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="pokus.aspx.vb" Inherits="UI.pokus" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <script type="text/javascript" src="https://addevent.com/libs/atc/1.6.1/atc.min.js" async defer></script>


    <script type="text/javascript">
        window.addeventasync = function () {
            addeventatc.settings({
                appleical: { show: true, text: "Apple kalendář" },
                google: { show: true, text: "Google kalendář" },
                outlook: { show: true, text: "Outlook" },
                outlookcom: { show: true, text: "Outlook.com <em>(online)</em>" },
                yahoo: { show: false, text: "Yahoo <em>(online)</em>" }
            });
        };

        function pokus() {
            var mykey = 'AIzaSyDtq1Bj5G4CGDkLcbyCy_bEwtTiSv6L6Y4'; // typically like Gtg-rtZdsreUr_fLfhgPfgff
            var calendarid = 'uffle5uumc8f5f4njnm5angahk@group.calendar.google.com'; // will look somewhat like 3ruy234vodf6hf4sdf5sd84f@group.calendar.google.com

            $.ajax({
                type: 'GET',
                url: encodeURI('https://www.googleapis.com/calendar/v3/calendars/' + calendarid + '/events?key=' + mykey),
                dataType: 'json',
                success: function (response) {
                    //do whatever you want with each
                    alert(response);
                },
                error: function (response) {
                    //tell that an error has occurred
                    alert("chyba");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <button type="button" onclick="pokus()">google api calendar</button>

    <asp:Button ID="cmdPokus" runat="server" Text="pokus" />
    <hr />
    <asp:TextBox ID="txt1" runat="server" TextMode="MultiLine" Width="600px" Height="100px"></asp:TextBox>
    <hr />
    <div title="Odeslat do kalendáře" class="addeventatc">
    Add to Calendar
    <span class="start">21-11-2017 16:00</span>
    <span class="end">21-11-2017 18:00</span>
        <span class="alarm_reminder">26</span>
    <span class="timezone">Europe/Prague</span>
    <span class="title">Oprava kotle</span>
    <span class="description">Normální výjezd<hr />Projekt:<a href="https://ak.marktime50.net" target="_blank">Odkaz</a></span>
    <span class="location">Dolní Břežany</span>
    	<span class="calname">udalost</span>
</div>
</asp:Content>



