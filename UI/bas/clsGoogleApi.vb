
Imports System.IO
Imports System.Threading

Imports Google.Apis.Calendar.v3
Imports Google.Apis.Calendar.v3.Data
Imports Google.Apis.Calendar.v3.EventsResource
Imports Google.Apis.Services
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Util.Store
Imports Google.Apis.Requests
Public Class clsGoogleApi
    Public Property factory As BL.Factory
    Public Property ErrorMessage As String
    Private _gservice As CalendarService
    Private _scopes As IList(Of String) = New List(Of String)()

    Public Sub New(f As BL.Factory)
        Me.factory = f
    End Sub
    Public Function Authenticate()     'Function that gets authenticates with google servers

        ' Add the calendar specific scope to the scopes list.
        _scopes.Add(CalendarService.Scope.Calendar)


        Dim credential As UserCredential

        Using stream As New FileStream(factory.x35GlobalParam.UploadFolder & "\client_id.json", FileMode.Open, FileAccess.Read)
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, _scopes, "user", CancellationToken.None, New FileDataStore("Calendar.VB.Sample")).Result

        End Using

        ' Create the calendar service using an initializer instance
        Dim initializer As New BaseClientService.Initializer()
        initializer.HttpClientInitializer = credential
        initializer.ApplicationName = "marktime"
        _gservice = New CalendarService(initializer)
        Return 0
    End Function

    Public Function ExportEvent2Calendar(intO25ID As Integer, cRec As BO.o22Milestone) As Boolean
        Me.ErrorMessage = ""
        Authenticate()
        Dim cO25 As BO.o25App = factory.o25AppBL.Load(intO25ID)

        Dim CalendarEvent As New Data.Event, bolNewEvent As Boolean = True
        If cRec.o22AppID <> "" Then
            Try
                CalendarEvent = _gservice.Events.Get(cO25.o25Code, cRec.o22AppID).Execute()
            Catch ex As Exception
                Me.ErrorMessage = String.Format("Chyba v přípojení k službě kalendáře: {0}", ex.Message)
                Return False
            End Try

            bolNewEvent = False
        End If

        Dim StartDateTime As New Data.EventDateTime
        If cRec.o22DateFrom Is Nothing Then
            StartDateTime.DateTime = cRec.o22DateUntil
        Else
            StartDateTime.DateTime = cRec.o22DateFrom
        End If
        Dim EndDateTime As New Data.EventDateTime
        EndDateTime.DateTime = cRec.o22DateUntil

        With CalendarEvent
            .Start = StartDateTime
            .End = EndDateTime


            .Summary = cRec.o22Name & " (" & cRec.o21Name & ")"
            If cRec.o22Description <> "" Then
                .Description = cRec.o22Description & vbCrLf
            Else
                .Description = ""
            End If

            If cRec.p41ID <> 0 Then
                .Description += String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", factory.x35GlobalParam.AppHostUrl, cRec.p41ID, cRec.Project)
            End If
            If cRec.p28ID <> 0 Then
                .Description += String.Format("<a href='{0}/dr.aspx?prefix=p28&pid={1}'>{2}</a>", factory.x35GlobalParam.AppHostUrl, cRec.p28ID, cRec.Contact)
            End If
            If cRec.p56ID <> 0 Then
                Dim cTask As BO.p56Task = factory.p56TaskBL.Load(cRec.p56ID)
                .Description += String.Format("<a href='{0}/dr.aspx?prefix=p56&pid={1}'>{2}</a>", factory.x35GlobalParam.AppHostUrl, cTask.PID, cTask.NameWithTypeAndCode)
                .Description += vbCrLf & String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", factory.x35GlobalParam.AppHostUrl, cTask.p41ID, cTask.Client & " - " & cTask.ProjectCodeAndName)

            End If
            If cRec.p91ID <> 0 Then
                Dim cInvoice As BO.p91Invoice = factory.p91InvoiceBL.Load(cRec.p91ID)
                .Description += String.Format("<a href='{0}/dr.aspx?prefix=p91&pid={1}'>{2}</a>", factory.x35GlobalParam.AppHostUrl, cRec.p91ID, cInvoice.p92Name & ": " & cInvoice.p91Code)
            End If
            .Location = cRec.o22Location
            If cRec.o22ColorID <> "" Then .ColorId = cRec.o22ColorID
        End With
        CalendarEvent.Attendees = New List(Of EventAttendee)
        Dim lisO20 As IEnumerable(Of BO.o20Milestone_Receiver) = factory.o22MilestoneBL.GetList_o20(cRec.PID)
        For Each c In lisO20
            Dim att As New EventAttendee()
            att.DisplayName = c.Person
            att.Email = c.Email
            att.ResponseStatus = "accepted"

            CalendarEvent.Attendees.Add(att)
        Next

        If cRec.o22ReminderBeforeUnits > 0 Then
            Dim eventReminder As New List(Of EventReminder)(), intMinutes As Integer = cRec.o22ReminderBeforeUnits
            Select Case cRec.o22ReminderBeforeMetric
                Case "d" : intMinutes = cRec.o22ReminderBeforeUnits * 24 * 60
                Case "h" : intMinutes = cRec.o22ReminderBeforeUnits * 60
            End Select
            eventReminder.Add(New EventReminder() With {.Minutes = intMinutes, .Method = "email"})
            Dim de As New Data.Event.RemindersData()
            de.UseDefault = False
            de.[Overrides] = eventReminder

            CalendarEvent.Reminders = de
        End If

        Dim ret As Data.Event = Nothing
        If bolNewEvent Then
            Dim Request As New InsertRequest(_gservice, CalendarEvent, cO25.o25Code)
            Request.CreateRequest()
            ret = Request.Execute()
        Else
            Dim Request As New UpdateRequest(_gservice, CalendarEvent, cO25.o25Code, cRec.o22AppID)
            Request.CreateRequest()
            ret = Request.Execute()
        End If

        cRec.o22AppUrl = ret.HtmlLink
        cRec.o22AppID = ret.Id
        factory.o22MilestoneBL.Save(cRec, Nothing)

        Return True
    End Function
End Class
