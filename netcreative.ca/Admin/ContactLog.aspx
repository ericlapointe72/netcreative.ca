<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ContactLog.aspx.cs" Inherits="netcreative.ca.ContactLog" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_ContactMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_ContactMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <asp:Repeater ID="Repeater" runat="server">
            <ItemTemplate>
                <asp:Label class="card__line" ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line1") %>'></asp:Label>
                <asp:Label class="card__line" ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line2") %>'></asp:Label>
                <asp:Label class="card__line" ID="Label3" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line3") %>'></asp:Label>
                <asp:Label class="card__line" ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line4") %>'></asp:Label>
                <asp:Label class="card__line" ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line5") %>'></asp:Label>
                <asp:Label class="card__line card__line-last" ID="Label6" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line6") %>'></asp:Label>
            </ItemTemplate>
        </asp:Repeater>
        <div class="card__enterdata">
            <span class="label__data"><%: Global.Admin_ContactDeleteLabel %></span>
            <asp:TextBox CssClass="textbox" ID="TextBox_Contact_Delete" placeholder="#" MaxLength="5" runat="server"></asp:TextBox>
            <asp:Button class="button button__red" ID="Button_Contact_Delete" runat="server" OnClick="Button_Contact_Delete_Click" />
        </div>
    </div>
</asp:Content>
