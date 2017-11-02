<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="fileupload.ascx.vb" Inherits="UI.fileupload" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<script type="text/javascript">
    function ClientFileSelected(radUpload, eventArgs) {
        document.getElementById("p1").style.display = "block";
        
    }

    function addTitle(radUpload, args) {
        var curLiEl = args.get_row();
        var firstInput = curLiEl.getElementsByTagName("input")[0];

        //Create a simple HTML template.
        var table = document.createElement("table");
        //table.className = 'AdditionalUploadInputs';

        //A new row for a Title field
        row = table.insertRow(-1);
        cell = row.insertCell(-1);
        var input = CreateInput("Title", "text");
        input.className = "TextField";
        input.id = input.name = radUpload.getID(input.name);
        var label = CreateLabel("Popis přílohy:", input.id);
        input.size = 60;
        cell.appendChild(label);
        cell = row.insertCell(-1);
        cell.appendChild(input);

        //Add a File label in front of the file input
        var fileInputSpan = curLiEl.getElementsByTagName("span")[0];
        var firstNode = curLiEl.childNodes[0];
        label = CreateLabel("", radUpload.getID());

        curLiEl.insertBefore(label, firstNode);

        //curLiEl.insertBefore(table, label);
        curLiEl.insertBefore(table, label);
    }

    function CreateLabel(text, associatedControlId) {
        var label = document.createElement("label");
        label.innerHTML = text;
        label.setAttribute("for", associatedControlId);
        label.style.fontSize = 12;

        return label;
    }

    function CreateInput(inputName, type) {
        var input = document.createElement("input");
        input.type = type;
        input.name = inputName;
        input.size = 45;

        return input;
    }

    function StartUpload() {
        if (document.getElementById("<%=o13ID.clientid%>").value == null || document.getElementById("<%=o13ID.clientid%>").value == "")
        {
            alert("Musíte specifikovat typ přílohy.");
            return false;
        }
    }
</script>

<div>
<telerik:RadUpload ID="upload1" runat="server" OnClientAdded="addTitle"  InputSize="30" InitialFileInputsCount="0" OnClientFileSelected="ClientFileSelected" RenderMode="Auto" Skin="Default">
</telerik:RadUpload>
   
    <p id="p1" style="display:none;">
    <asp:button ID="cmdUpload" runat="server" text="Nahrát přílohy na server" CssClass="cmd" OnClientClick="return StartUpload();"  />
    <asp:Label ID="lblLimitsInfo" runat="server"></asp:Label>
    <asp:Label ID="lblO13ID" runat="server" Text="Typ přílohy:" Visible="false"></asp:Label>
    <asp:DropDownList ID="o13ID" runat="server" datatextfield="o13Name" datavaluefield="pid" style="width:150px;" Visible="false"></asp:DropDownList>    
    </p>
</div>
<asp:Label ID="lblError" runat="server" CssClass="failureNotification"></asp:Label>

<telerik:radprogressarea id="RadProgressArea1" runat="server" displaycancelbutton="True" progressindicators="FilesCountBar, FilesCount, FilesCountPercent, SelectedFilesCount, CurrentFileName"></telerik:radprogressarea>
<telerik:RadProgressManager ID="RadProgressManager1" runat="server" />

<asp:HiddenField ID="hidGUID" runat="server" />
<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidMaxFileUploadedCount" runat="server" Value="0" />



