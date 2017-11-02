<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="entity_framework_detail_setting.aspx.vb" Inherits="UI.entity_framework_detail_setting" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:CheckBox ID="chkShowLevel1" runat="server" Text="Zobrazovat v menu výrazný odkaz (název) vybraného záznamu" CssClass="chk" Visible="false" />
    </div>
    <asp:panel ID="panMenuSkin" runat="server" CssClass="div6">
        <span>Vzhled (Skin) kontextového menu stránky:</span>
        <asp:DropDownList ID="skin0" runat="server">
            <asp:ListItem Text="--Výchozí--" Value="WebBlue" Selected="true"></asp:ListItem>                      

            
            <asp:ListItem Text="Metro" Value="Metro"></asp:ListItem>
            <asp:ListItem Text="Web20" Value="Web20"></asp:ListItem>
            
            <asp:ListItem Text="Outlook" Value="Outlook"></asp:ListItem>
            <asp:ListItem Text="Office2007" Value="Office2007"></asp:ListItem>
            <asp:ListItem Text="Office2010Blue" Value="Office2010Blue"></asp:ListItem>
            <asp:ListItem Text="Office2010Black" Value="Office2010Black"></asp:ListItem>
            <asp:ListItem Text="Office2010Silver" Value="Office2010Silver"></asp:ListItem>
            
            <asp:ListItem Text="Sunset" Value="Sunset"></asp:ListItem>
            
            <asp:ListItem Text="Vista" Value="Vista"></asp:ListItem>
            
        </asp:DropDownList>
    </asp:panel>

    <asp:Panel ID="panPlugin" runat="server" CssClass="content-box2" Style="margin-top: 20px;">
        <div class="title">
            Plugin umístěný nad záložkami
            
        </div>
        <div class="content">
            <asp:DropDownList ID="x31ID_Plugin" runat="server" DataValueField="pid" DataTextField="NameWithFormat"></asp:DropDownList>
        </div>
    </asp:Panel>

    <div class="content-box2" style="margin-top: 20px;">
        <div class="title">
            Záložky
        </div>
        <div class="content">
            <div class="div6">
                <asp:Button ID="cmdClearLockedTab" runat="server" Text="Vyčistit paměť o ukotvené záložce" CssClass="cmd" />
                
            </div>
            <div class="div6">
                <span>Vzhled (Skin) záložek:</span>
                <asp:DropDownList ID="skin1" runat="server">
                    <asp:ListItem Text="--Výchozí--" Value="Default" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Metro" Value="Metro"></asp:ListItem>
                    <asp:ListItem Text="MetroTouch" Value="MetroTouch"></asp:ListItem>
                    <asp:ListItem Text="Glow" Value="Glow"></asp:ListItem>
                    <asp:ListItem Text="Bootstrap" Value="Bootstrap"></asp:ListItem>
                    <asp:ListItem Text="WebBlue" Value="WebBlue"></asp:ListItem>
                    <asp:ListItem Text="Web20" Value="Web20"></asp:ListItem>
                    <asp:ListItem Text="Silk" Value="Silk"></asp:ListItem>
                    <asp:ListItem Text="Outlook" Value="Outlook"></asp:ListItem>
                    <asp:ListItem Text="Office2007" Value="Office2007"></asp:ListItem>
                    <asp:ListItem Text="Office2010Blue" Value="Office2010Blue"></asp:ListItem>
                    <asp:ListItem Text="Office2010Black" Value="Office2010Black"></asp:ListItem>
                    <asp:ListItem Text="Office2010Silver" Value="Office2010Silver"></asp:ListItem>
                    <asp:ListItem Text="Telerik" Value="Telerik"></asp:ListItem>
                    <asp:ListItem Text="Sunset" Value="Sunset"></asp:ListItem>
                    <asp:ListItem Text="Simple" Value="Simple"></asp:ListItem>
                    <asp:ListItem Text="Black" Value="Black"></asp:ListItem>
                    <asp:ListItem Text="Vista" Value="Vista"></asp:ListItem>
                    <asp:ListItem Text="Windows7" Value="Windows7"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:Panel ID="panTabs" runat="server">
                <table cellpadding="8">
                    <tr valign="top">
                        <td>
                            <div>Dostupné záložky</div>
                            <telerik:RadListBox ID="colsSource" Height="200px" runat="server" DataTextField="x61Name" DataValueField="x61ID" AllowTransfer="true" TransferMode="Move" TransferToID="colsDest" SelectionMode="Single" Culture="cs-CZ" AllowTransferOnDoubleClick="true" Width="350px" AutoPostBackOnReorder="false" AutoPostBackOnDelete="false" AutoPostBackOnTransfer="false">
                                <ButtonSettings TransferButtons="All" ShowTransferAll="false" />

                                <Localization ToRight="Přesunout" ToLeft="Odebrat" AllToRight="Přesunout vše" AllToLeft="Odbrat vše" MoveDown="Posunout dolu" MoveUp="Posunout nahoru" />
                            </telerik:RadListBox>
                        </td>
                        <td>
                            <div>Vybrané záložky</div>
                            <telerik:RadListBox ID="colsDest" runat="server" DataTextField="x61Name" DataValueField="x61ID" AllowReorder="true" AllowTransferOnDoubleClick="true" Culture="cs-CZ" Width="350px" SelectionMode="Single">

                                <EmptyMessageTemplate>
                                    <div style="padding-top: 50px;">
                                        žádné vybrané záložky
                                    </div>
                                </EmptyMessageTemplate>
                            </telerik:RadListBox>

                        </td>

                    </tr>
                </table>
            </asp:Panel>
        </div>
    </div>
    <div class="content-box2">
        <div class="title">Kalendář</div>
        <div class="content">
            <div class="div6">
        <asp:CheckBox ID="chkScheduler" runat="server" Text="Zobrazovat na stránce kalendář, pokud existují otevřené úkoly nebo termíny" CssClass="chk" Checked="true" />
    </div>
        </div>
    </div>

    <asp:HiddenField ID="hidPrefix" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
