<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="confirmation.aspx.cs" Inherits="netcreative.ca.confirmation" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.Confirmation_Title;
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Contact_HeroEyebrow, Global.Contact_H1Title, Global.Contact_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card confirmation__card">
            <i class="fas fa-circle-check confirmation__icon"></i>
            <h1 class="confirmation__title"><%: Global.Confirmation_Heading %></h1>
            <span class="confirmation__text"><%: Global.Confirmation_Text %></span>
            <a class="button" href="/default.aspx"><%: Global.Confirmation_BackButton %></a>
        </div>
    </div>

</asp:Content>
