<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="pokus2.aspx.vb" Inherits="UI.pokus2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>pokus</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="x-ua-compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    



    <script type="text/javascript" src="https://apis.google.com/js/api.js"></script>

    <script type="text/javascript">
        function start() {
            // 2. Initialize the JavaScript client library.            
            gapi.client.init({
                'apiKey': 'AIzaSyDtq1Bj5G4CGDkLcbyCy_bEwtTiSv6L6Y4',
                // clientId and scope are optional if auth is not required.
                'clientId': '811901017740-0aajbhv1j9lsc9kussh19k9rojt93iq0.apps.googleusercontent.com',
                'scope': 'profile',
            }).then(function () {
                // 3. Initialize and make the API request.                
                return gapi.client.request({
                    'path': 'https://people.googleapis.com/v1/people/me?requestMask.includeField=person.names',
                })
            }).then(function (response) {
                //ok
            }, function (reason) {
                console.log(reason);
            });
        };
        // 1. Load the JavaScript client library.
        gapi.load('client', start);
       
        function pokus() {
            
            var event = {
                'summary': 'Google I/O 2015',
                'location': '800 Howard St., San Francisco, CA 94103',
                'description': 'A chance to hear more about Google\'s developer products.',
                'start': {
                    'dateTime': '2017-11-28T09:00:00-07:00',
                    'timeZone': 'Europe/Prague'
                },
                'end': {
                    'dateTime': '2017-11-28T17:00:00-07:00',
                    'timeZone': 'Europe/Prague'
                },                
                'attendees': [
                  {'email': 'info@marktime.cz'},
                  {'email': 'sbrin@example.com'}
                ],
                'reminders': {
                    'useDefault': false,
                    'overrides': [
                      {'method': 'email', 'minutes': 24 * 60},
                      {'method': 'popup', 'minutes': 10}
                    ]
                }
            };
            

            var request = gapi.client.calendar.events.insert({
                'calendarId': 'uffle5uumc8f5f4njnm5angahk@group.calendar.google.com',
                'resource': event
            });

            

            request.execute(function (event) {                
                alert('Event created: ' + event.htmlLink);
            });
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">

        <button type="button" onclick="pokus()">pokus</button>
    </form>
</body>
</html>
