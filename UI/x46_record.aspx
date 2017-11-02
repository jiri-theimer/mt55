<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x46_record.aspx.vb" Inherits="UI.x46_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblX45ID" Text="Druh události:" runat="server" CssClass="lblReq"></asp:Label>
        <uc:datacombo ID="x45ID" runat="server" Filter="Contains" DataTextField="NameWithCode" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" Width="500px"></uc:datacombo>
    </div>

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Příjemci notifikační zprávy" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Obsah notifikační zprávy"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="page1" runat="server" Selected="true">
            <asp:label ID="lblReceiverMessage" runat="server" CssClass="infoNotification"></asp:label>
            <div>
                <asp:CheckBox ID="x46IsExcludeAuthor" runat="server" Checked="true" Text="Z příjemců zprávy automaticky vyloučit osobu, která vyvolala tuto událost" ForeColor="red" />
            </div>
            <asp:panel ID="panReceiver" runat="server" CssClass="content-box2">
                <div class="title">Příjemci události</div>
                <div class="content">
                    <table cellpadding="6">
                        <tr>
                            <td>
                                <asp:Label ID="lblJ02ID" runat="server" Text="Osoba:" CssClass="lbl"></asp:Label>

                            </td>
                            <td>
                                <uc:person ID="j02ID" runat="server" Width="300px" />

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label1" Text="Tým osob:" runat="server" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <uc:datacombo ID="j11ID" runat="server" DataTextField="j11Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblX67ID" Text="Role:" runat="server" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <uc:datacombo ID="x67ID" runat="server" DataTextField="x67Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                                <asp:CheckBox ID="x46IsForAllRoles" runat="server" Text="Určeno pro všechny role" AutoPostBack="true" />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="x46IsForRecordOwner" runat="server" Text="Příjemcem bude i vlastník záznamu" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblMessage" runat="server" CssClass="infoNotification" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>

                    </table>
                </div>
            </asp:panel>

            <asp:Panel ID="panReference" runat="server" CssClass="content-box2">

                <div class="title">Referenční entita</div>
                <div class="content">
                    <table cellpadding="6">
                        <tr>
                            <td>
                                <asp:Label ID="lblX29ID" Text="Referenční entita:" runat="server" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="x29ID_Reference" runat="server" AutoPostBack="true">
                                    <asp:ListItem Text="--Všechny, které připadají v úvahu--" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                                    <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                                    <asp:ListItem Text="Faktura" Value="391"></asp:ListItem>
                                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                                    <asp:ListItem Text="Worksheet záznam" Value="331"></asp:ListItem>
                                    <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                                    <asp:ListItem Text="Kalendářová událost" Value="222"></asp:ListItem>
                                </asp:DropDownList>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblx67ID_Reference" Text="Příjemce (role z referenční entity):" runat="server" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <uc:datacombo ID="x67ID_Reference" runat="server" DataTextField="x67Name" Visible="false" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                                <asp:CheckBox ID="x46IsForAllReferenceRoles" runat="server" Text="Určeno pro všechny role z referenční entity" AutoPostBack="true" />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="x46IsForRecordOwner_Reference" runat="server" Text="Příjemcem bude i vlastník záznamu referenční entity" />
                            </td>
                        </tr>
                    </table>
                </div>

            </asp:Panel>
        </telerik:RadPageView>
        <telerik:RadPageView ID="page2" runat="server">
            <asp:CheckBox ID="x46IsUseSystemTemplate" runat="server" Text="Používat výchozí (systémovou) textaci zprávy" AutoPostBack="true" Visible="false" />
            <div class="div6" style="margin-top: 10px;">
                <asp:Label ID="lblSubject" runat="server" CssClass="lblReq" Text="Předmět zprávy:"></asp:Label>
                <asp:TextBox ID="x46MessageSubject" runat="server" Style="width: 700px;"></asp:TextBox>
            </div>
            <div>
                <asp:Label ID="Label3" runat="server" CssClass="lbl" Text="Obsah zprávy:"></asp:Label>
            </div>
            <asp:TextBox ID="x46MessageTemplate" runat="server" Style="width: 98%; height: 340px;" TextMode="MultiLine"></asp:TextBox>
            <asp:TextBox ID="x45MessageTemplate" runat="server" Style="width: 98%; height: 340px;" TextMode="MultiLine" Visible="false" ReadOnly="true"></asp:TextBox>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


