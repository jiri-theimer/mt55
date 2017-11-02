<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="vat_registration.aspx.vb" Inherits="UI.vat_registration" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                DIČ:
            </td>
            <td>
                <asp:TextBox ID="txtDIC" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="cmdVerify" runat="server" Text="Ověřit" CssClass="cmd" />
            </td>
        </tr>
    </table>

    <div class="content-box2">
        <div class="title">
            <img src="Images/eu_32.png" height="22px" />
            VIES | VAT Information Exchange System | <a href="http://ec.europa.eu/taxation_customs/vies/vatRequest.html" target="_blank">link</a>
        </div>
        <div class="content">
            <asp:Label ID="vies_error" runat="server" CssClass="infoNotificationRed"></asp:Label>
            <table cellpadding="6">
                <tr>
                    <td>Member State:</td>
                    <td>
                        <asp:Label ID="vies_country" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>VAT Number:</td>
                    <td>
                        <asp:Label ID="vies_vatnumber" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Is Valid:</td>
                    <td>
                        <asp:Label ID="vies_isvalid" runat="server" CssClass="valboldblue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Name:</td>
                    <td>
                        <asp:Label ID="vies_name" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr valign="top">
                    <td>Address:</td>
                    <td>
                        <asp:Label ID="vies_address" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
            </table>
            
        </div>
    </div>
    <br />
    <div class="content-box2">
        <div class="title">
            <img src="Images/Flags/czechrepublic.gif" />
            Registr plátců DPH | Ministerstvo financí ČR | <a href="http://adisreg.mfcr.cz/cgi-bin/adis/idph/int_dp_prij.cgi?ZPRAC=FDPHI1" target="_blank">link</a>
        </div>
        <div class="content">
            <asp:Label ID="mf_error" runat="server" CssClass="infoNotificationRed"></asp:Label>
            <table cellpadding="6">
                <tr>
                    <td>DIČ:</td>
                    <td>
                        <asp:Label ID="dic_mf" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>Klasifikace plátce:</td>
                    <td>
                        <asp:Label ID="nespolehlivyPlatce" runat="server" CssClass="valboldblue"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr valign="top">
                    <td>Zveřejněné bankovní účty:</td>
                    <td>
                        <asp:Label ID="bankovni_ucet" runat="server" CssClass="valbold"></asp:Label>
                        
                    </td>
                    <td>
                        <asp:Label ID="bankovni_ucet_datum" runat="server" ></asp:Label>
                    </td>
                </tr>
              
                <tr>
                    <td>Číslo FU:</td>
                    <td>
                        <asp:Label ID="fu_mf" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    <td></td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
