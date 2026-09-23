<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Subscribers.aspx.cs" Inherits="netcreative.ca.Subscribers" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_SubscriberMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_SubscriberMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <div class="card__subscribercontent">
            <asp:GridView class="grid" ID="GridView_Subscribe" runat="server"></asp:GridView>
            <div class="card__enterdata">
                <span class="label__data"><%: Global.Admin_SubscriberDeleteLabel %></span>
                <asp:TextBox class="textbox" ID="TextBox_Subscriber_Delete" placeholder="#" runat="server" MaxLength="5"></asp:TextBox>
                <asp:Button class="button button__red" ID="Button_Subscriber_Delete" runat="server" OnClick="Button_Subscriber_Delete_Click" />
            </div>
        </div>
    </div>
</asp:Content>
