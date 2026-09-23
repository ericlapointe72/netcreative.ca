<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="unsubscribe.aspx.cs" Inherits="netcreative.ca.unsubscribe" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.Unsubscribe_Title;
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Unsubscribe_HeroEyebrow, Global.Unsubscribe_H1Title, Global.Unsubscribe_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="unsubscribe__container">
        <div class="card" data-network-error="<%: Global.Unsubscribe_NetworkError %>">
            <span class="label__unsubscribe"><%: Global.Unsubscribe_Label %></span>
            <asp:TextBox CssClass="textbox tb__unsubscribe" ID="TextBox_Unsubscribe" runat="server"></asp:TextBox>
            <button type="button" class="button unsubscribe__submit"><%: Global.Unsubscribe_Button %></button>
        </div>
    </div>

    <script src="Content/js/unsubscribe.js"></script>

</asp:Content>
