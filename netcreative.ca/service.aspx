<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="service.aspx.cs" Inherits="netcreative.ca.service" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <div id="topPage"></div>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Messenger Plug-in Discussion Code -->
    <div id="fb-root"></div>
    <script>
        window.fbAsyncInit = function () {
            FB.init({
                xfbml: true,
                version: 'v10.0'
            });
        };

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = 'https://connect.facebook.net/fr_FR/sdk/xfbml.customerchat.js';
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>

    <!-- Your Plug-in Discussion code -->
    <div class="fb-customerchat"
        attribution="biz_inbox"
        page_id="101923751954399">
    </div>

    <div class="content-section">
        <span class="content-section__text"><%: Global.Service_Intro %></span>
    </div>

    <span class="section-label"><%: Global.Service_ServicesLabel %></span>

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
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Item7Title %></span>
            <span class="feature-card__text"><%: Global.Service_Item7 %></span>
        </div>
    </div>

    <span class="section-label"><%: Global.Service_PrepLabel %></span>

    <div class="feature-grid">
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Client1Title %></span>
            <span class="feature-card__text"><%: Global.Service_Client1 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Client2Title %></span>
            <span class="feature-card__text"><%: Global.Service_Client2 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Client3Title %></span>
            <span class="feature-card__text"><%: Global.Service_Client3 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Client4Title %></span>
            <span class="feature-card__text"><%: Global.Service_Client4 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Client5Title %></span>
            <span class="feature-card__text"><%: Global.Service_Client5 %></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><%: Global.Service_Client6Title %></span>
            <span class="feature-card__text"><%: Global.Service_Client6 %></span>
        </div>
    </div>

    <div class="button__top">
        <asp:LinkButton ID="LinkButton_Top" runat="server"><a href="#topPage">
            <i class="fas fa-chevron-up chevron__up"></i></a>
        </asp:LinkButton>
    </div>

    <script src="js/onscroll.js"></script>

</asp:Content>
