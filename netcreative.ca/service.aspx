<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="service.aspx.cs" Inherits="netcreative.ca.service" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("Content/images/site/hero-service.jpg", Global.Service_HeroEyebrow, Global.Service_HeroTitle, Global.Service_HeroText, Global.Service_HeroCta, "~/contact.aspx");
%>

    <div id="topPage"></div>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content-section">
        <span class="content-section__text"><%: Global.Service_Intro %></span>
        <span class="content-section__text"><%: Global.Text("Service_NoWordPress") %></span>
    </div>

    <h2 class="section-label"><%: Global.Service_ServicesLabel %></h2>

    <div class="feature-grid">
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item1Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item1 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item2Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item2 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item3Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item3 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item4Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item4 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item5Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item5 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item6Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item6 %></span>
        </div>
    </div>

    <h2 class="section-label"><%: Global.Service_PrepLabel %></h2>

    <div class="checklist-grid">
        <div class="checklist-card">
            <span class="checklist-card__number">01</span>
            <div>
                <span class="checklist-card__title"><%: Global.Service_Client1Title %></span>
                <span class="checklist-card__text"><%: Global.Service_Client1 %></span>
            </div>
        </div>
        <div class="checklist-card">
            <span class="checklist-card__number">02</span>
            <div>
                <span class="checklist-card__title"><%: Global.Service_Client2Title %></span>
                <span class="checklist-card__text"><%: Global.Service_Client2 %></span>
            </div>
        </div>
        <div class="checklist-card">
            <span class="checklist-card__number">03</span>
            <div>
                <span class="checklist-card__title"><%: Global.Service_Client3Title %></span>
                <span class="checklist-card__text"><%: Global.Service_Client3 %></span>
            </div>
        </div>
        <div class="checklist-card">
            <span class="checklist-card__number">04</span>
            <div>
                <span class="checklist-card__title"><%: Global.Service_Client4Title %></span>
                <span class="checklist-card__text"><%: Global.Service_Client4 %></span>
            </div>
        </div>
        <div class="checklist-card">
            <span class="checklist-card__number">05</span>
            <div>
                <span class="checklist-card__title"><%: Global.Service_Client5Title %></span>
                <span class="checklist-card__text"><%: Global.Service_Client5 %></span>
            </div>
        </div>
        <div class="checklist-card">
            <span class="checklist-card__number">06</span>
            <div>
                <span class="checklist-card__title"><%: Global.Service_Client6Title %></span>
                <span class="checklist-card__text"><%: Global.Service_Client6 %></span>
            </div>
        </div>
    </div>

    <h2 class="section-label"><%: Global.Text("Faq_Label") %></h2>

    <div class="faq">
        <% for (int q = 1; q <= 8; q++) { %>
        <details class="faq__item">
            <summary class="faq__question"><%: Global.Text("Faq_Q" + q) %></summary>
            <p class="faq__answer"><%: Global.Text("Faq_A" + q) %></p>
        </details>
        <% } %>
    </div>

    <%-- the same questions for Google (FAQPage): the answers it shows under the result are the ones written above --%>
    <script type="application/ld+json"><%= FaqJsonLd() %></script>

    <div class="button__top">
        <a class="chevron__up" href="#topPage" aria-label="<%: Global.Text("BackToTop") %>"><i class="ic ic-chevron-up" aria-hidden="true"></i></a>
    </div>

    <script src="Content/js/onscroll.js"></script>

</asp:Content>
