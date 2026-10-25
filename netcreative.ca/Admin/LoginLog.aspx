<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="LoginLog.aspx.cs" Inherits="netcreative.ca.LoginLog" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_LoginMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_LoginMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <asp:Label class="label__data" ID="Label_Login_Info" runat="server"></asp:Label>
        <asp:GridView class="grid" ID="GridView_Login" runat="server" AutoGenerateColumns="true" AllowPaging="true" AllowCustomPaging="true" PageSize="50"
            DataKeyNames="Number" OnPageIndexChanging="GridView_Login_PageIndexChanging" OnRowDeleting="GridView_Login_RowDeleting"
            OnRowDataBound="GridView_Login_RowDataBound">
            <Columns>
                <asp:CommandField ShowDeleteButton="true" />
            </Columns>
            <PagerStyle CssClass="grid__pager" />
        </asp:GridView>
        <div class="card__button">
            <asp:Button class="button button__red" ID="Button_Login_Purge" runat="server" OnClick="Button_Login_Purge_Click" />
        </div>
    </div>
</asp:Content>
