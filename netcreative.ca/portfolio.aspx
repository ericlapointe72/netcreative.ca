<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="portfolio.aspx.cs" Inherits="netcreative.ca.portfolio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="portfolio__container">
        <div class="card">
            <h1 runat="server" class="card__title" id="H1title1"></h1>
            <p class="card__description"><asp:Label ID="Label_Description1" runat="server"></asp:Label></p>
            <div class="card__img">
                <div class="card__img_large observer">
                    <img src="Imgs_Portfolio/bht_large.png" alt="BHT Large"/>
                </div>
                <div class="card__img_medium observer">
                    <img src="Imgs_Portfolio/bht_medium.png" alt="BHT Medium"/>
                </div>
                <div class="card__img_small observer">
                    <img src="Imgs_Portfolio/bht_small.png" alt="BHT Small"/>
                </div>
            </div>
            <a class="card__link" href="https://beaucedutyfree.com" target="_blank"><asp:Label ID="Label_Link1" runat="server"></asp:Label></a>
        </div>
        <div class="card">
            <h1 runat="server" class="card__title" id="H1title2"></h1>
            <p class="card__description"><asp:Label ID="Label_Description2" runat="server"></asp:Label></p>
            <div class="card__img">
                <div class="card__img_large">
                    <img src="Imgs_Portfolio/mbh_large.png" alt="MBH Large"/>
                </div>
                <div class="card__img_medium">
                    <img src="Imgs_Portfolio/mbh_medium.png" alt="MBH Medium"/>
                </div>
                <div class="card__img_small">
                    <img src="Imgs_Portfolio/mbh_small.png" alt="MBH Small"/>
                </div>
            </div>
            <a class="card__link" href="https://murrybaie.ca" target="_blank"><asp:Label ID="Label_Link2" runat="server"></asp:Label></a>
        </div>
        <div class="card">
            <h1 runat="server" class="card__title" id="H1title3"></h1>
            <p class="card__description"><asp:Label ID="Label_Description3" runat="server"></asp:Label></p>
            <div class="card__img">
                <div class="card__img_large">
                    <img src="Imgs_Portfolio/te_large.png" alt="TE Large"/>
                </div>
                <div class="card__img_medium">
                    <img src="Imgs_Portfolio/te_medium.png" alt="TE Medium"/>
                </div>
                <div class="card__img_small">
                    <img src="Imgs_Portfolio/te_small.png" alt="TE Small"/>
                </div>
            </div>
            <a class="card__link" href="https://tablo-ecolo.ca" target="_blank"><asp:Label ID="Label_Link3" runat="server"></asp:Label></a>
        </div>
    </div>

</asp:Content>
