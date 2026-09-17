<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="netcreative.ca.Default" %>
<%@ MasterType VirtualPath="~/Site1.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <span class="section-label"><asp:Label ID="Label_PillarsLabel" runat="server"></asp:Label></span>

    <div class="pillar-grid">
        <a class="pillar-card pillar-card--orange" href="boutique.aspx">
            <span class="pillar-card__title"><asp:Label ID="Label_Pillar1Title" runat="server"></asp:Label></span>
            <span class="pillar-card__text"><asp:Label ID="Label_Pillar1Text" runat="server"></asp:Label></span>
        </a>
        <a class="pillar-card pillar-card--blue" href="dutyfreeops.aspx">
            <span class="pillar-card__title"><asp:Label ID="Label_Pillar2Title" runat="server"></asp:Label></span>
            <span class="pillar-card__text"><asp:Label ID="Label_Pillar2Text" runat="server"></asp:Label></span>
        </a>
        <a class="pillar-card" href="service.aspx">
            <span class="pillar-card__title"><asp:Label ID="Label_Pillar3Title" runat="server"></asp:Label></span>
            <span class="pillar-card__text"><asp:Label ID="Label_Pillar3Text" runat="server"></asp:Label></span>
        </a>
    </div>

    <div class="default__container">
        <div class="card">
            <img class="card__icon-large" src="Imgs_Site/problem-solving.png" alt="problem solving" />
            <img class="card__icon-popup observer" src="Imgs_Site/problem-solving.png" alt="problem solving" />
            <h2 runat="server" class="section-label" id="H2slideshowtitle"></h2>
            
            <div class="slideshow__container" role="region" aria-roledescription="carousel" aria-label="<%= System.Web.HttpUtility.HtmlAttributeEncode(Global.Home_SlideshowTitle) %>">
                <div class="slide">
                    <asp:Label class="slide__title" ID="Label_Slide1_Title" runat="server"></asp:Label>
                    <asp:Label class="slide__text" ID="Label_Slide1_Text" runat="server"></asp:Label>
                </div>
                <div class="slide">
                    <asp:Label class="slide__title" ID="Label_Slide2_Title" runat="server"></asp:Label>
                    <asp:Label class="slide__text" ID="Label_Slide2_Text" runat="server"></asp:Label>
                </div>
                <div class="slide">
                    <asp:Label class="slide__title" ID="Label_Slide3_Title" runat="server"></asp:Label>
                    <asp:Label class="slide__text" ID="Label_Slide3_Text" runat="server"></asp:Label>
                </div>
                <div class="slide">
                    <asp:Label class="slide__title" ID="Label_Slide4_Title" runat="server"></asp:Label>
                    <asp:Label class="slide__text" ID="Label_Slide4_Text" runat="server"></asp:Label>
                </div>
                <div class="slide">
                    <asp:Label class="slide__title" ID="Label_Slide5_Title" runat="server"></asp:Label>
                    <asp:Label class="slide__text" ID="Label_Slide5_Text" runat="server"></asp:Label>
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

            <h2 runat="server" class="card__title observer" id="H2whoiamtitle"></h2>

            <div class="bottom__container">
                <div class="section5">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">01</div>
                        <asp:Label class="card__paragraphe" ID="Label_WhoIamParagraphe1" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="section6">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">02</div>
                        <asp:Label class="card__paragraphe" ID="Label_WhoIamParagraphe2" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="section7">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">03</div>
                        <asp:Label class="card__paragraphe" ID="Label_WhoIamParagraphe3" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="section8">
                    <div class="card__paragraphe-ctn observer">
                        <div class="card__number">04</div>
                        <asp:Label class="card__paragraphe" ID="Label_WhoIamParagraphe4" runat="server"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="card__facebook">
                <img class="card__icon-popup_fb observer" src="Imgs_Site/trust.png" alt="hand shake"/>
                <asp:Label class="card__facebook-text observer" ID="Label_Facebook" runat="server"></asp:Label>
                <a href="https://www.facebook.com/NetCr%C3%A9ative-101923751954399" target="_blank"><img class="facebook__logo observer" src="Imgs_Site/facebook-logo.png" /></a>
            </div>
            
            <asp:Label class="card__Signature observer" ID="Label_WhoIamName" runat="server"></asp:Label>

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