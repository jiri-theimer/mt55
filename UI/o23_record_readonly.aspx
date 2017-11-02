<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o23_record_readonly.aspx.vb" Inherits="UI.o23_record_readonly1" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_record_readonly" Src="~/o23_record_readonly.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="div6">
        <uc:o23_record_readonly ID="rec1" runat="server" />
    </div>
    <div class="div6">
        <uc:entityrole_assign_inline ID="roles1" runat="server" EntityX29ID="o23Doc" NoDataText=""></uc:entityrole_assign_inline>
    </div>

    <div style="clear: both;"></div>
    <uc:b07_list ID="comments1" runat="server" ShowHeader="true" ShowInsertButton="false" />

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
