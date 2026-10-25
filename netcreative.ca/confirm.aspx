<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="confirm.aspx.cs" Inherits="netcreative.ca.confirm" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Text("Confirm_HeroEyebrow"), Global.Text("Confirm_H1Title"), Global.Text("Confirm_HeroText"));
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="unsubscribe__container">
        <%-- the link of the confirmation e-mail: opening it only shows a button, the sign-up is made by the click --%>
        <asp:Panel ID="Panel_Confirm" runat="server" CssClass="card" Visible="false">
            <span class="unsubscribe__text"><%: Global.Text("Confirm_Intro") %></span>
            <asp:Button CssClass="button" ID="Button_Confirm" runat="server" OnClick="Button_Confirm_Click" />
            <asp:HiddenField ID="HiddenField_Token" runat="server" />
        </asp:Panel>

        <asp:Panel ID="Panel_Message" runat="server" CssClass="card" Visible="false">
            <span class="unsubscribe__text"><asp:Literal ID="Literal_Message" runat="server"></asp:Literal></span>
            <a class="button unsubscribe__back" href="/"><%: Global.Text("Sub_BackToSite") %></a>
        </asp:Panel>
    </div>

</asp:Content>
