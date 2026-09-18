<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="netcreative.ca.Default" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <span class="section-label"><%: Global.Home_PillarsLabel %></span>

    <div class="pillar-grid">
        <a class="pillar-card pillar-card--orange" href="boutique.aspx">
            <span class="pillar-card__title"><%: Global.Home_Pillar1Title %></span>
            <span class="pillar-card__text"><%: Global.Home_Pillar1Text %></span>
        </a>
        <a class="pillar-card pillar-card--blue" href="dutyfreeops.aspx">
            <span class="pillar-card__title"><%: Global.Home_Pillar2Title %></span>
            <span class="pillar-card__text"><%: Global.Home_Pillar2Text %></span>
        </a>
        <a class="pillar-card" href="service.aspx">
            <span class="pillar-card__title"><%: Global.Home_Pillar3Title %></span>
            <span class="pillar-card__text"><%: Global.Home_Pillar3Text %></span>
        </a>
    </div>

    <div class="default__container">
        <div class="card">
            <img class="card__icon-large" src="Imgs_Site/problem-solving.png" alt="problem solving" />
            <img class="card__icon-popup observer" src="Imgs_Site/problem-solving.png" alt="problem solving" />
            <h2 class="section-label"><%: Global.Home_SlideshowTitle %></h2>

            <div class="slideshow__container" role="region" aria-roledescription="carousel" aria-label="<%= System.Web.HttpUtility.HtmlAttributeEncode(Global.Home_SlideshowTitle) %>">
                <div class="slide">
                    <span class="slide__title"><%: Global.Home_Slide1Title %></span>
                    <span class="slide__text"><%: Global.Home_Slide1Text %></span>
                </div>
                <div class="slide">
                    <span class="slide__title"><%: Global.Home_Slide2Title %></span>
                    <span class="slide__text"><%: Global.Home_Slide2Text %></span>
                </div>
                <div class="slide">
                    <span class="slide__title"><%: Global.Home_Slide3Title %></span>
                    <span class="slide__text"><%: Global.Home_Slide3Text %></span>
                </div>
                <div class="slide">
                    <span class="slide__title"><%: Global.Home_Slide4Title %></span>
                    <span class="slide__text"><%: Global.Home_Slide4Text %></span>
                </div>
                <div class="slide">
                    <span class="slide__title"><%: Global.Home_Slide5Title %></span>
                    <span class="slide__text"><%: Global.Home_Slide5Text %></span>
                </div>
                <button type="button" class="prev"><i class="fas fa-chevron-left" aria-hidden="true"></i></button>
                <button type="button" class="next"><i class="fas fa-chevron-right" aria-hidden="true"></i></button>
            </div>

            <div class="dot__container">
                <button type="button" class="dot"></button>
                <button type="button" class="dot"></button>
                <button type="button" class="dot"></button>
                <button type="button" class="dot"></button>
                <button type="button" class="dot"></button>
            </div>

            <h2 class="card__title observer"><%: Global.Home_WhoIamTitle %></h2>

            <div class="bottom__container">
                <div class="section5">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">01</div>
                        <span class="card__paragraphe"><%: Global.Home_WhoIamParagraph1 %></span>
                    </div>
                </div>
                <div class="section6">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">02</div>
                        <span class="card__paragraphe"><%: Global.Home_WhoIamParagraph2 %></span>
                    </div>
                </div>
                <div class="section7">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">03</div>
                        <span class="card__paragraphe"><%: Global.Home_WhoIamParagraph3 %></span>
                    </div>
                </div>
                <div class="section8">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">04</div>
                        <span class="card__paragraphe"><%: Global.Home_WhoIamParagraph4 %></span>
                    </div>
                </div>
            </div>

            <div class="card__facebook">
                <img class="card__icon-popup_fb observer" src="Imgs_Site/trust.png" alt="hand shake"/>
                <span class="card__facebook-text observer"><%: Global.Home_Facebook %></span>
                <a href="https://www.facebook.com/NetCr%C3%A9ative-101923751954399" target="_blank"><img class="facebook__logo observer" src="Imgs_Site/facebook-logo.png" /></a>
            </div>

            <span class="card__Signature observer"><%: Global.Home_WhoIamName %></span>

            <div class="carousel__container observer" role="region" aria-roledescription="carousel" aria-label="<%= System.Web.HttpUtility.HtmlAttributeEncode(Global.Home_CarouselRegionLabel) %>">
                <div class="carousel__slide" tabindex="0">
                    <img class="carousel__image" src="Imgs_Carousel/carousel1.jpg" alt="carousel image 1" loading="lazy" decoding="async"/>
                    <img class="carousel__image" src="Imgs_Carousel/carousel2.jpg" alt="carousel image 2" loading="lazy" decoding="async"/>
                    <img class="carousel__image" src="Imgs_Carousel/carousel3.jpg" alt="carousel image 3" loading="lazy" decoding="async"/>
                    <img class="carousel__image" src="Imgs_Carousel/carousel4.jpg" alt="carousel image 4" loading="lazy" decoding="async"/>
                    <img class="carousel__image" src="Imgs_Carousel/carousel5.jpg" alt="carousel image 5" loading="lazy" decoding="async"/>
                    <img class="carousel__image" src="Imgs_Carousel/carousel6.jpg" alt="carousel image 6" loading="lazy" decoding="async"/>
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
        </div>
    </div>

    <script>
        window.__i18n = {
            slidePrev: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.Home_SlideshowPrevLabel) %>",
            slideNext: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.Home_SlideshowNextLabel) %>",
            slideDot: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.Home_SlideshowDotLabel) %>",
            carouselPrev: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.Home_CarouselPrevLabel) %>",
            carouselNext: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.Home_CarouselNextLabel) %>",
            carouselDot: "<%= System.Web.HttpUtility.JavaScriptStringEncode(Global.Home_CarouselDotLabel) %>"
        };
    </script>
    <script src="js/observer.js"></script>
    <script src="js/carousel.js"></script>
    <script src="js/slideshow.js"></script>
</asp:Content>