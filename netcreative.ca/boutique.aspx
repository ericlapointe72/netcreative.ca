<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="boutique.aspx.cs" Inherits="netcreative.ca.boutique" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("Content/images/site/hero-boutique.jpg", Global.Boutique_HeroEyebrow, Global.Boutique_HeroTitle, Global.Boutique_HeroText, Global.Boutique_HeroCta, "https://www.beaucedutyfree.com", "_blank");
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content-section">
        <span class="content-section__text"><%: Global.Boutique_Intro %></span>
    </div>

    <%-- the shop is where everything started: DutyFreeOps, beaucedutyfree.com and the web know-how all come from it --%>
    <h2 class="section-label"><%: Global.Text("Boutique_OriginLabel") %></h2>

    <div class="content-section">
        <span class="content-section__text"><%: Global.Text("Boutique_OriginText") %></span>
    </div>

    <div class="pillar-grid">
        <a class="pillar-card pillar-card--blue" href="dutyfreeops.aspx">
            <span class="pillar-card__title"><%: Global.Home_Pillar2Title %></span>
            <span class="pillar-card__text"><%: Global.Home_Pillar2Text %></span>
        </a>
        <a class="pillar-card pillar-card--orange" href="https://www.beaucedutyfree.com" target="_blank" rel="noopener">
            <span class="pillar-card__title"><%: Global.Text("Boutique_OriginSiteTitle") %></span>
            <span class="pillar-card__text"><%: Global.Text("Boutique_OriginSiteText") %></span>
        </a>
        <a class="pillar-card" href="service.aspx">
            <span class="pillar-card__title"><%: Global.Home_Pillar3Title %></span>
            <span class="pillar-card__text"><%: Global.Home_Pillar3Text %></span>
        </a>
    </div>

    <h2 class="section-label"><%: Global.Boutique_InfoLabel %></h2>

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

    <h2 class="section-label"><%: Global.Text("Faq_Label") %></h2>

    <div class="faq">
        <%-- without script every answer is shown; faq.js folds them and opens one at a time, with a height animation --%>
        <% for (int q = 1; q <= FaqCount; q++) { %>
        <div class="faq__item">
            <h3 class="faq__heading">
                <button type="button" class="faq__question" id="faq-q<%= q %>" aria-expanded="false" aria-controls="faq-a<%= q %>"><%: Global.Text("BtqFaq_Q" + q) %></button>
            </h3>
            <div class="faq__panel" id="faq-a<%= q %>" role="region" aria-labelledby="faq-q<%= q %>">
                <p class="faq__answer"><%: Global.Text("BtqFaq_A" + q) %></p>
            </div>
        </div>
        <% } %>
    </div>

    <div class="faq__cta">
        <p class="faq__cta-text"><%: Global.Text("BtqFaq_Cta") %></p>
        <a class="button" href="https://www.beaucedutyfree.com" target="_blank" rel="noopener"><%: Global.Boutique_HeroCta %></a>
    </div>

    <%-- the same questions for Google (FAQPage): the answers it shows under the result are the ones written above --%>
    <script type="application/ld+json"><%= FaqJsonLd() %></script>

    <script src="Content/js/faq.js?v=1"></script>

</asp:Content>
