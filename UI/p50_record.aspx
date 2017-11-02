<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p50_record.aspx.vb" Inherits="UI.p50_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function periodcombo_setting() {

            dialog_master("periodcombo_setting.aspx");
        }

        function sw_local(url, iconUrl, is_maximize) {
            var wnd = $find("<%=okno1.clientid%>");
        wnd.setUrl(url);
        if (iconUrl != null)
            wnd.set_iconUrl(iconUrl);
        else
            wnd.set_iconUrl("Images/window_32.png");

        wnd.show();

        if (is_maximize == true) {
            wnd.maximize();
        }
        else {
            wnd.center();
        }

    }

    function hardrefresh(pid, flag) {

    }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <span class="lbl">Druh počítaných sazeb:</span>
        <asp:DropDownList ID="p50RatesFlag" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Nákladové sazby" Value="1"></asp:ListItem>
            <asp:ListItem Text="Efektivní sazby" Value="2"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="div6">
        <span class="lbl">Ceník:</span>
        <uc:datacombo ID="p51ID" runat="server" DataTextField="NameWithCurr" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" AutoPostBack="true"></uc:datacombo>
        <asp:HyperLink ID="clue1" runat="server" CssClass="reczoom" Text="i" title="Detail ceníku"></asp:HyperLink>
    </div>
    
    <div style="padding: 10px;">
        <asp:Label ID="lblFrom" Text="Platnost sazeb od:" runat="server" CssClass="lbl" Width="140px"></asp:Label>
        <telerik:RadDatePicker ID="p50ValidFrom" runat="server" Width="120px" >
            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>            
        </telerik:RadDatePicker>
    </div>
    <div style="padding: 10px;">
        <asp:Label ID="lblUntil" Text="Platnost sazeb do:" runat="server" CssClass="lbl" Width="140px"></asp:Label>
        <telerik:RadDatePicker ID="p50ValidUntil" runat="server" Width="120px"  MaxDate="1.1.3000">
            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>            
        </telerik:RadDatePicker>
    </div>

    <asp:Panel ID="panRecalcCostRates" runat="server" CssClass="content-box2" Visible="false">
        <div class="title">
            Zpětný přepočet nákladových sazeb
        </div>
        <div class="content">
            <span>Časové období pro přepočet worksheet úkonů:</span>
            <uc:periodcombo ID="period2" runat="server" Width="250px"></uc:periodcombo>
            <asp:Button ID="cmdRecalcCostRates" Text="Přepočítat nákladové sazby." CssClass="cmd" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel ID="panRecalcFPR" runat="server" CssClass="content-box2" Visible="false">
        <div class="title">
            Zpětný přepočet efektivních sazeb
        </div>
        <div class="content">
            <span>Časové období podle zdanitelného plnění faktur:</span>
            <uc:periodcombo ID="period1" runat="server" Width="250px"></uc:periodcombo>
            <asp:Button ID="cmdRecalcFPR" Text="Přepočítat efektivní sazby vyfakturovaných úkonů za vybrané období." CssClass="cmd" runat="server" />
        </div>
    </asp:Panel>

    <telerik:RadWindow ID="okno1" runat="server" RenderMode="Lightweight" Width="900px" Height="700px" Modal="false" VisibleStatusbar="false" Skin="MetroTouch" IconUrl="Images/project_32.png" ShowContentDuringLoad="false" InitialBehaviors="None" Behaviors="Close,Move,Resize,Maximize" Style="z-index: 9000;">
        <Shortcuts>
            <telerik:WindowShortcut CommandName="Close" Shortcut="Esc" />
        </Shortcuts>
        <Localization Close="Zavřít" Restore="Základní velikost" Maximize="Maximalizovat" />
    </telerik:RadWindow>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
