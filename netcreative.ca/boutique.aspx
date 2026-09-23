<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="boutique.aspx.cs" Inherits="netcreative.ca.boutique" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.Boutique_Title;
    Master.SetPageHero("Content/images/site/hero-boutique.jpg", Global.Boutique_HeroEyebrow, Global.Boutique_HeroTitle, Global.Boutique_HeroText, Global.Boutique_HeroCta, "https://www.beaucedutyfree.com", "_blank");
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content-section">
        <span class="content-section__text"><%: Global.Boutique_Intro %></span>
    </div>

    <span class="section-label"><%: Global.Boutique_InfoLabel %></span>

    <div class="feature-grid">
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Boutique_Category1Title %></span>
            <span class="feature-card__text"><%: Global.Boutique_Category1Text %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Boutique_Category2Title %></span>
            <span class="feature-card__text"><%: Global.Boutique_Category2Text %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Boutique_Info1Title %></span>
            <span class="feature-card__text"><%: Global.Boutique_Info1Text %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Boutique_Info2Title %></span>
            <span class="feature-card__text"><%: Global.Boutique_Info2Text %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Boutique_Info3Title %></span>
            <span class="feature-card__text"><%: Global.Boutique_Info3Text %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Boutique_Info4Title %></span>
            <span class="feature-card__text"><%: Global.Boutique_Info4Text %></span>
        </div>
    </div>

</asp:Content>
