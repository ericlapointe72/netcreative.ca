<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="dutyfreeops.aspx.cs" Inherits="netcreative.ca.dutyfreeops" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.DutyFreeOps_Title;
    Master.SetPageHero("Content/images/site/hero-dutyfreeops.jpg", Global.DutyFreeOps_HeroEyebrow, Global.DutyFreeOps_HeroTitle, Global.DutyFreeOps_HeroText, Global.DutyFreeOps_HeroCta, "~/contact.aspx");
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content-section">
        <span class="content-section__text"><%: Global.DutyFreeOps_Intro %></span>
    </div>
    
    <span class="section-label"><%: Global.DutyFreeOps_ModulesLabel %></span>

    <div class="feature-grid">
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><%: Global.DutyFreeOps_Module1Title %></span>
            <span class="feature-card__text"><%: Global.DutyFreeOps_Module1Text %></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><%: Global.DutyFreeOps_Module2Title %></span>
            <span class="feature-card__text"><%: Global.DutyFreeOps_Module2Text %></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><%: Global.DutyFreeOps_Module3Title %></span>
            <span class="feature-card__text"><%: Global.DutyFreeOps_Module3Text %></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><%: Global.DutyFreeOps_Module4Title %></span>
            <span class="feature-card__text"><%: Global.DutyFreeOps_Module4Text %></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><%: Global.DutyFreeOps_Module5Title %></span>
            <span class="feature-card__text"><%: Global.DutyFreeOps_Module5Text %></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><%: Global.DutyFreeOps_Module6Title %></span>
            <span class="feature-card__text"><%: Global.DutyFreeOps_Module6Text %></span>
        </div>
    </div>

</asp:Content>
