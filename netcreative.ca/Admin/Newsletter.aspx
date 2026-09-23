<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Newsletter.aspx.cs" Inherits="netcreative.ca.Newsletter" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_NewsletterMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_NewsletterMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <div class="card__choice">
            <asp:RadioButton class="radio__choice" ID="RadioButton_Newsletter_Test" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton_Newsletter_Test_CheckedChanged" />
            <asp:Label class="label__email" ID="Label_Newsletter_Email" runat="server"></asp:Label>
            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Email" runat="server"></asp:TextBox>
            <asp:RadioButton class="radio__choice" ID="RadioButton_Newsletter" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton_Newsletter_CheckedChanged" />
        </div>
        <div class="card__email">
            <span class="label__email"><%: Global.Admin_NewsletterObjectLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Object" runat="server"></asp:TextBox>
            <span class="label__email"><%: Global.Admin_NewsletterIntroLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Intro" runat="server"></asp:TextBox>
            <span class="label__email"><%: Global.Admin_NewsletterBodyLabel %></span>
            <asp:TextBox class="textbox textbox__multiline" ID="TextBox_Newsletter_Body" runat="server" Rows="6" TextMode="MultiLine"></asp:TextBox>
            <span class="label__email"><%: Global.Admin_NewsletterGreetingLabel %></span>
            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Greeting" runat="server"></asp:TextBox>
        </div>
        <div class="card__button">
            <asp:Button class="button" ID="Button_Newsletter_Send" runat="server" OnClick="Button_Newsletter_Send_Click" />
        </div>
    </div>
</asp:Content>
