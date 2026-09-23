<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="LoginLog.aspx.cs" Inherits="netcreative.ca.LoginLog" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_LoginMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_LoginMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <asp:GridView class="grid" ID="GridView_Login" runat="server"></asp:GridView>
        <div class="card__enterdata">
            <span class="label__data"><%: Global.Admin_LoginDeleteLabel %></span>
            <asp:TextBox CssClass="textbox" ID="TextBox_Login_Delete" placeholder="#" MaxLength="5" runat="server"></asp:TextBox>
            <asp:Button class="button button__red" ID="Button_Login_Delete" runat="server" OnClick="Button_Login_Delete_Click" />
        </div>
    </div>
</asp:Content>
