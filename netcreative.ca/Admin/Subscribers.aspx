<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Subscribers.aspx.cs" Inherits="netcreative.ca.Subscribers" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_SubscriberMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_SubscriberMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <div class="card__subscribercontent">
            <asp:Label class="label__data" ID="Label_Subscribe_Info" runat="server"></asp:Label>
            <asp:GridView class="grid" ID="GridView_Subscribe" runat="server" AutoGenerateColumns="true" AllowPaging="true" AllowCustomPaging="true" PageSize="50"
                DataKeyNames="Number" OnPageIndexChanging="GridView_Subscribe_PageIndexChanging" OnRowDeleting="GridView_Subscribe_RowDeleting"
                OnRowDataBound="GridView_Subscribe_RowDataBound">
                <Columns>
                    <asp:CommandField ShowDeleteButton="true" />
                </Columns>
                <PagerStyle CssClass="grid__pager" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
