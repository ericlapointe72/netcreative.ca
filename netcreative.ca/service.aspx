<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="service.aspx.cs" Inherits="netcreative.ca.service" %>
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

    <div class="service__container">
        <h1 runat="server" class="card__title observer" id="H1title1"></h1>
                
        <div class="card__ctn card__ctn1 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service1" runat="server"></asp:Label>
        </div>

        <div class="card__ctn card__ctn2 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service2" runat="server"></asp:Label>
        </div>

        <div class="card__ctn card__ctn3 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service3" runat="server"></asp:Label>
        </div>
        
        <div class="card__ctn card__ctn4 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service4" runat="server"></asp:Label>
        </div>

        <div class="card__ctn card__ctn5 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service5" runat="server"></asp:Label>
        </div>

        <div class="card__ctn card__ctn6 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service6" runat="server"></asp:Label>
        </div>

        <div class="card__ctn card__ctn7 observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Service7" runat="server"></asp:Label>
        </div>

        <h1 runat="server" class="card__title observer" id="H1title2"></h1>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Client1" runat="server"></asp:Label>
        </div>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Client2" runat="server"></asp:Label>
        </div>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Client3" runat="server"></asp:Label>
        </div>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Client4" runat="server"></asp:Label>
        </div>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Client5" runat="server"></asp:Label>
        </div>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Client6" runat="server"></asp:Label>
        </div>

        <h1 runat="server" class="card__title observer" id="H1title3"></h1>

        <h2 runat="server" class="card__title observer" id="h2title1"></h2>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Techno1" runat="server"></asp:Label>
        </div>

        <h2 runat="server" class="card__title observer" id="h2title2"></h2>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Techno2" runat="server"></asp:Label>
        </div>

        <h2 runat="server" class="card__title observer" id="h2title3"></h2>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Techno3" runat="server"></asp:Label>
        </div>

        <h2 runat="server" class="card__title observer" id="h2title4"></h2>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Techno4" runat="server"></asp:Label>
        </div>

        <h2 runat="server" class="card__title observer" id="h2title5"></h2>

        <div class="card__ctn observer">
            <i class="fas fa-square"></i>
            <asp:Label class="card__text" ID="Label_Techno5" runat="server"></asp:Label>
        </div>
    </div>

    <div class="button__top">
        <asp:LinkButton ID="LinkButton_Top" runat="server"><a href="#topPage">
            <i class="fas fa-chevron-up chevron__up"></i></a>
        </asp:LinkButton>
    </div>

    <script src="js/observer.js"></script>
    <script src="js/onscroll.js"></script>

</asp:Content>
