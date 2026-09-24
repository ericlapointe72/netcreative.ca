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

    <span class="section-label"><%: Global.DutyFreeOps_ScreenshotsLabel %></span>

    <div class="carousel__container observer" role="region" aria-roledescription="carousel" aria-label="<%= System.Web.HttpUtility.HtmlAttributeEncode(Global.DutyFreeOps_CarouselRegionLabel) %>">
        <div class="carousel__slide" tabindex="0">
            <img class="carousel__image" src="Content/images/DutyFreeOps/DutyFreeOps1.jpg" alt="<%: Global.DutyFreeOps_Screenshot1Alt %>" loading="lazy" decoding="async"/>
            <img class="carousel__image" src="Content/images/DutyFreeOps/DutyFreeOps2.jpg" alt="<%: Global.DutyFreeOps_Screenshot2Alt %>" loading="lazy" decoding="async"/>
            <img class="carousel__image" src="Content/images/DutyFreeOps/DutyFreeOps3.jpg" alt="<%: Global.DutyFreeOps_Screenshot3Alt %>" loading="lazy" decoding="async"/>
            <img class="carousel__image" src="Content/images/DutyFreeOps/DutyFreeOps4.jpg" alt="<%: Global.DutyFreeOps_Screenshot4Alt %>" loading="lazy" decoding="async"/>
            <img class="carousel__image" src="Content/images/DutyFreeOps/DutyFreeOps5.jpg" alt="<%: Global.DutyFreeOps_Screenshot5Alt %>" loading="lazy" decoding="async"/>
            <img class="carousel__image" src="Content/images/DutyFreeOps/DutyFreeOps6.jpg" alt="<%: Global.DutyFreeOps_Screenshot6Alt %>" loading="lazy" decoding="async"/>
        </div>

        <button type="button" id="prev__btn"><i class="fas fa-chevron-left" aria-hidden="true"></i></button>
        <button type="button" id="next__btn"><i class="fas fa-chevron-right" aria-hidden="true"></i></button>

        <div class="carousel__navigation">
            <button type="button" class="nav__btn" id="nav__1"></button>
            <button type="button" class="nav__btn" id="nav__2"></button>
            <button type="button" class="nav__btn" id="nav__3"></button>
            <button type="button" class="nav__btn" id="nav__4"></button>
            <button type="button" class="nav__btn" id="nav__5"></button>
            <button type="button" class="nav__btn" id="nav__6"></button>
        </div>
    </div>

    <script>
        window.__i18n = {
            carouselPrev: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.DutyFreeOps_CarouselPrevLabel) %>",
            carouselNext: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.DutyFreeOps_CarouselNextLabel) %>",
            carouselDot: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.DutyFreeOps_CarouselDotLabel) %>"
        };
    </script>
    <script src="Content/js/carousel.js"></script>

</asp:Content>
