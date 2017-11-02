<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            
            Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
            With cRec
                Me.Client.Text = .Client
                If .p51ID_Billing > 0 Then
                    Me.p51Name_Billing.Text = .p51Name_Billing
                    If .p51Name_Billing.IndexOf(cRec.p41Name) >= 0 Then
                        'sazby na míru
                        p51Name_Billing.Text = "Tento projekt má sazby na míru"
                    End If
                Else
                    If .p28ID_Client > 0 Then
                        Dim cClient As BO.p28Contact = Master.Factory.p28ContactBL.Load(.p28ID_Client)
                        If cClient.p51ID_Billing > 0 Then
                            Me.p51Name_Billing.Text = cClient.p51Name_Billing & " (dědí se z klienta)"
                        End If
                    End If
                    
                End If
               
            End With
            
            
        End If
        
    End Sub
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        
        
    End Sub

    
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float:left;padding:6px;">
        Klient:
        <asp:Label ID="Client" runat="server" CssClass="valbold"></asp:Label>
    </div>
    <div style="float:left;padding:6px;">
        Ceník fakturačních sazeb:
        <asp:Label ID="p51Name_Billing" runat="server" CssClass="valbold"></asp:Label>
    </div>
    <div style="clear:both;"></div>
</asp:Content>

