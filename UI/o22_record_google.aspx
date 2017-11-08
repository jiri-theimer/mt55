<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o22_record_google.aspx.vb" Inherits="UI.o22_record_google" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        // date variables
        
        // Google api console clientID and apiKey 
        var clientId = '811901017740-0aajbhv1j9lsc9kussh19k9rojt93iq0.apps.googleusercontent.com';
        var apiKey = 'AIzaSyDtq1Bj5G4CGDkLcbyCy_bEwtTiSv6L6Y4';

        // enter the scope of current project (this API must be turned on in the Google console)
        var scopes = 'https://www.googleapis.com/auth/calendar';

        // OAuth2 functions
        function handleClientLoad() {
            gapi.client.setApiKey(apiKey);
            window.setTimeout(checkAuth, 1);
        }

        function checkAuth() {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: true }, handleAuthResult);
        }

        // show/hide the 'authorize' button, depending on application state
        function handleAuthResult(authResult) {
            document.getElementById("imgLoading").style.display = "none";
            if (authResult && !authResult.error) {
                document.getElementById("infoMessage").innerText = "Úvodní ověření s Google kalendářem proběhlo v pořádku.";
                document.getElementById("cmdOdeslat").style.display = "block";
            } else {
                document.getElementById("cmdOverit").style.display = "block";
                document.getElementById("cmdOverit").onclick = handleAuthClick;
                document.getElementById("infoMessage").innerText = "Zatím nedošlo k úvodnímu ověření s Google kalendářem. Klikněte na tlačítko [Ověřit komunikaci s Google kalendářem].";
            }
            
            
        }

        // function triggered when user authorizes app
        function handleAuthClick(event) {
            
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: false }, handleAuthResult);
            return false;
        }

      
        // setup event details

        // function load the calendar api and make the api call
        function makeApiCall() {
            
            gapi.client.load('calendar', 'v3', function () {					// load the calendar api (version 3)
                var request = gapi.client.calendar.events.insert
                ({
                    'calendarId': document.getElementById("<%=o25Code.ClientID%>").value, // calendar ID
                    "resource": udalost							// pass event details with api call
                });

                // handle the response from our api call
                request.execute(function (resp) {
                    if (resp.status == 'confirmed') {
                        var pid="<%=master.DataPID%>";
                        console.log("Událost založena");
                        console.log(resp);
                        console.log("id udalosti: " + resp.id);

                        $.post("Handler/handler_calendar.ashx", { o22id: pid, eventID: resp.id, eventLink: resp.htmlLink,eventiCalUID: resp.iCalUID, oper: "save" }, function (data) {
                            if (data == '1') {
                                window.close();
                            }
                            else{
                                alert("Chyba při ukládání zpětné vazby do MARKTIME.");
                            }
                        });


                    } else {
                        console.log(resp);
                    }
                });
            });
        }
       
        var udalost = {
            'summary': "<%=me.o22Name.text%>",
            'location': "<%=Me.o22Location.Text%>",
            'description': "<%=ViewState("description")%>",
            'start': {
                'dateTime': "<%=hidStart.Value%>",
                'timeZone': "Europe/Prague"
            },
            'colorId': '<%=hidColorID.value%>',
            'end': {
                'dateTime': "<%=hidEnd.Value%>",
                'timeZone': "Europe/Prague"
            },
            'attendees': [
              { 'email': 'info@marktime.cz' },
              { 'email': 'sbrin@example.com' }
            ]
            <%If hidMinutesBefore.Value <> "" Then%>
            ,'reminders': {
                'useDefault': false,
                'overrides': [
                  { 'method': 'email', 'minutes': <%=hidMinutesBefore.Value%> },
                  { 'method': 'popup', 'minutes': <%=hidMinutesBefore.Value%> }
                ]
            }
            <%end if%>
        };

       
		</script>
    <script src="https://apis.google.com/js/client.js?onload=handleClientLoad" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6" style="height:40px;">
        <img id="imgLoading" src="Images/loading.gif" alt="Loading..." />
        <button type="button" id="cmdOdeslat" onclick="makeApiCall()" style="display:none;font-size:x-large;" class="cmd">Odeslat událost do Google kalendáře</button>

        <button type="button" id="cmdOverit" style="display:none;font-size:x-large;" class="cmd">Ověřit komunikaci s Google kalendářem</button>
    </div>
    
    <div class="div6">
        <span id="infoMessage" class="infoNotification"></span>
    </div>
            

    <div class="content-box2">
        <div class="title">
            <img src="Images/calendar.png" />
            <asp:Label ID="lblHeader" runat="server" Text="Kalendářová událost"></asp:Label>
        </div>
        <div class="content">
            <table cellpadding="8">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="o22Name" runat="server" CssClass="valboldblue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        Cílový Google kalendář:
                    </td>
                    <td>
                        <asp:Label ID="o25Name" runat="server" CssClass="valbold"></asp:Label>
                        <asp:HiddenField ID="o25Code" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Kdy:
                    </td>
                    <td>
                        <asp:Label ID="o22DateFrom" runat="server" CssClass="valbold"></asp:Label>
                        
                        <asp:Label ID="o22DateUntil" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Připomenutí (oznámení):</td>
                    <td>
                        <asp:Label ID="o22ReminderBeforeUnits" runat="server"></asp:Label>
                        <asp:Label ID="o22ReminderBeforeMetric" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Kde:</td>
                    <td>
                        <asp:Label ID="o22Location" runat="server" ForeColor="Blue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Příjemci události:</td>
                    <td>
                        <asp:Label ID="Attendees" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <div class="div6">
                <asp:Label ID="o22Description" runat="server" Font-Italic="true"></asp:Label>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidColorID" runat="server" />
    <asp:HiddenField ID="hidMinutesBefore" runat="server" />
    <asp:HiddenField ID="hidStart" runat="server" />
    <asp:HiddenField ID="hidEnd" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
