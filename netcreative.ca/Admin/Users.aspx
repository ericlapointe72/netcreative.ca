<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="netcreative.ca.Users" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_UserMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_UserMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <div class="card__dropdown">
            <span class="label__data"><%: Global.Admin_UserListLabel %></span>
            <asp:DropDownList class="textbox" ID="DropDown_User" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDown_User_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class="card__enterdata">
            <span class="label__data"><%: Global.Admin_UserNameLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_User_Name" runat="server" MaxLength="20"></asp:TextBox>
        </div>
        <div class="card__enterdata">
            <span class="label__data"><%: Global.Admin_UserPasswordLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_User_Password" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
        </div>
        <div class="card__enterdata">
            <span class="label__data"><%: Global.Admin_UserFirstNameLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_User_FirstName" runat="server" MaxLength="20"></asp:TextBox>
        </div>
        <div class="card__enterdata">
            <span class="label__data"><%: Global.Admin_UserLastNameLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_User_LastName" runat="server" MaxLength="20"></asp:TextBox>
        </div>
        <div class="card__button">
            <asp:Button class="button" ID="Button_User_Save" runat="server" OnClick="Button_User_Save_Click"/>
            <asp:Button class="button" ID="Button_User_SaveNew" runat="server" OnClick="Button_User_SaveNew_Click" />
            <asp:Button class="button" ID="Button_User_New" runat="server" OnClick="Button_User_New_Click" />
            <asp:Button class="button button__red" ID="Button_User_Delete" runat="server" OnClick="Button_User_Delete_Click" />
        </div>
    </div>
</asp:Content>
