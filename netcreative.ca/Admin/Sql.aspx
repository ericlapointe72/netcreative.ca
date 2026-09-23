<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Sql.aspx.cs" Inherits="netcreative.ca.Sql" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_SqlMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_SqlMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <span class="label__data label__sqlwarning"><%: Global.Admin_SqlWarning %></span>
        <asp:TextBox class="textbox textbox__multiline" ID="TextBox_Sql_Query" runat="server" Rows="8" TextMode="MultiLine"></asp:TextBox>
        <div class="card__button">
            <asp:Button class="button button__red" ID="Button_Sql_Execute" runat="server" OnClick="Button_Sql_Execute_Click" />
        </div>
        <asp:Label class="label__data" ID="Label_Sql_Status" runat="server"></asp:Label>
        <asp:GridView class="grid" ID="GridView_Sql_Result" runat="server"></asp:GridView>
    </div>
</asp:Content>
