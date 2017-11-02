<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_setting.aspx.vb" Inherits="UI.p31_setting" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <p></p>
    <asp:Panel ID="panT" runat="server" CssClass="content-box2">
        <div class="title">
            <%=Resources.p31_setting.Header_T%>
        </div>
        <div class="content">
            
            <asp:RadioButtonList ID="opgHoursEntryFlag" runat="server" AutoPostBack="true">
                <asp:ListItem Text="<%$Resources:p31_setting,opgHoursEntryFlag_1%>" Value="1" Selected="true"></asp:ListItem>
                               
                <asp:ListItem Text="<%$Resources:p31_setting,opgHoursEntryFlag_2%>" Value="2"></asp:ListItem>
            </asp:RadioButtonList>

            <div class="div6">
                <asp:CheckBox ID="p31_PreFillP32ID" runat="server" Text="Před-vyplňovat aktivitu z naposledy zadávaných úkonů" CssClass="chk" />
            </div>

            <table cellpadding="6" id="responsive">
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="chkShowTimeInterval" runat="server" Text="<%$Resources:p31_setting, chkShowTimeInterval %>" CssClass="chk" meta:resourcekey="chkShowTimeInterval" />
                    </td>
                </tr>
                <tr>                    
                    <td>
                        <%=Resources.p31_setting.NabidkaIntervalu%>
                    </td>
                    <td>
                        <asp:DropDownList ID="p31_HoursInputInterval" runat="server">
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputInterval_5 %>" Value="5"></asp:ListItem>
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputInterval_15 %>" Value="15"></asp:ListItem>
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputInterval_30 %>" Value="30"></asp:ListItem>
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputInterval_60 %>" Value="60"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>                    
                    <td>
                        <%=Resources.p31_setting.NabidkaFormat%>
                    </td>
                    <td>
                        <asp:DropDownList ID="p31_HoursInputFormat" runat="server">
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputFormat_dec%>" Value="dec" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputFormat_hhmm%>" Value="hhmm"></asp:ListItem>
                            
                        </asp:DropDownList>
                    </td>
                </tr>                       
                <tr>                    
                    <td>
                        <%=Resources.p31_setting.NabidkaIntervaluCas%>
                    </td>
                    <td>
                        <asp:DropDownList ID="p31_TimeInputInterval" runat="server">                            
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputInterval_30%>" Value="30"></asp:ListItem>
                            <asp:ListItem Text="<%$Resources:p31_setting, p31_HoursInputInterval_60%>" Value="60"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>                    
                    <td>
                        <%=Resources.p31_setting.NabidkaStartOd%>
                    </td>
                    <td>
                        <asp:DropDownList ID="p31_TimeInput_Start" runat="server">   
                            <asp:ListItem Text="05:00" Value="5"></asp:ListItem>                         
                            <asp:ListItem Text="06:00" Value="6"></asp:ListItem>
                            <asp:ListItem Text="07:00" Value="7"></asp:ListItem>
                            <asp:ListItem Text="08:00" Value="8"></asp:ListItem>
                            <asp:ListItem Text="09:00" Value="9"></asp:ListItem>
                            <asp:ListItem Text="10:00" Value="10"></asp:ListItem>
                            <asp:ListItem Text="11:00" Value="11"></asp:ListItem>
                            <asp:ListItem Text="12:00" Value="12"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>                    
                    <td>
                        <%=Resources.p31_setting.NabidkaEndOd%>
                    </td>
                    <td>
                        <asp:DropDownList ID="p31_TimeInput_End" runat="server">   
                            <asp:ListItem Text="15:00" Value="15"></asp:ListItem>
                            <asp:ListItem Text="16:00" Value="16"></asp:ListItem>
                            <asp:ListItem Text="17:00" Value="17"></asp:ListItem>                         
                            <asp:ListItem Text="18:00" Value="18"></asp:ListItem>
                            <asp:ListItem Text="19:00" Value="19"></asp:ListItem>
                            <asp:ListItem Text="20:00" Value="20"></asp:ListItem>
                            <asp:ListItem Text="21:00" Value="21"></asp:ListItem>
                            <asp:ListItem Text="22:00" Value="22"></asp:ListItem>
                            <asp:ListItem Text="23:00" Value="23"></asp:ListItem>
                            
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
