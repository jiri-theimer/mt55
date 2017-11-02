<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_batch_sendmail.aspx.vb" Inherits="UI.p91_batch_sendmail" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="email_receiver" Src="~/email_receiver.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">Výchozí text zprávy</div>
        <div class="content">
            <asp:textbox ID="txtBody" runat="server" TextMode="MultiLine" Width="99%" Height="100px"></asp:textbox>
            <div>
                <span class="lbl">Úvod předmětu zprávy:</span>
                <asp:TextBox ID="txtSubjectPre" runat="server" Width="300px"></asp:TextBox>
            </div>
        </div>
    </div>
    <div>
        <asp:CheckBox ID="chkAtt" runat="server" Text="Posílat i přílohu faktury" Checked="true" AutoPostBack="true" CssClass="chk" />
    </div>

    <table cellpadding="6">
        <tr>
            <th></th>
            <th>Příjemce</th>
            <th>Sestava faktury</th>
            <th>Sestava přílohy</th>
            <th>Předmět zprávy</th>
            <th>Text zprávy</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr valign="top">
                    <td>
                        <asp:Label ID="p91Code" runat="server" CssClass="valbold"></asp:Label>
                        <asp:HiddenField ID="p91ID" runat="server" />
                    </td>
                    <td>
                        <uc:email_receiver ID="receiver" runat="server" Width="200px" />
                    </td>
                    <td>
                        <asp:DropDownList ID="rep1" runat="server" DataTextField="x31Name" DataValueField="pid" Width="200px"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="rep2" runat="server" DataTextField="x31Name" DataValueField="pid" Width="200px"></asp:DropDownList>
                    </td>
                   
                    
                    <td>
                        <asp:TextBox ID="txtSubject" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:DropDownList ID="j61ID" runat="server" DataTextField="j61Name" DataValueField="pid" Width="200px"></asp:DropDownList>
                    </td>
                </tr>
                
            </ItemTemplate>
        </asp:Repeater>
    </table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
