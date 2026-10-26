<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="netcreative.ca.Logs" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Text("Admin_LogsMenu") %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Text("Admin_LogsMenu") %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <p class="logs-hint"><%: Global.Text("Logs_Hint") %></p>
        <asp:PlaceHolder ID="PlaceHolder_Picker" runat="server">
            <div class="card__line">
                <asp:Label ID="Label_Logs_File" runat="server" AssociatedControlID="DropDownList_Logs_File" />
                <asp:DropDownList ID="DropDownList_Logs_File" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_Logs_File_SelectedIndexChanged" />
            </div>
            <asp:Label class="label__data" ID="Label_Logs_Info" runat="server"></asp:Label>
            <pre class="logs-view" tabindex="0"><asp:Literal ID="Literal_Logs_Text" runat="server" Mode="Encode" /></pre>
            <div class="card__button">
                <asp:HyperLink class="button" ID="HyperLink_Logs_Download" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:Label class="label__data" ID="Label_Logs_Empty" runat="server" Visible="false"></asp:Label>
    </div>
</asp:Content>
