<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="boutique.aspx.cs" Inherits="netcreative.ca.boutique" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="hero hero--orange observer">
        <span class="hero__eyebrow"><asp:Label ID="Label_HeroEyebrow" runat="server"></asp:Label></span>
        <h1 runat="server" class="hero__title" id="H1title"></h1>
        <asp:Label class="hero__text" ID="Label_HeroText" runat="server"></asp:Label>
        <a class="hero__cta" href="https://www.beaucedutyfree.com" target="_blank"><asp:Label ID="Label_HeroCta" runat="server"></asp:Label></a>
    </div>

    <div class="content-section">
        <asp:Label class="content-section__text" ID="Label_Intro" runat="server"></asp:Label>
    </div>

    <span class="section-label"><asp:Label ID="Label_CategoriesLabel" runat="server"></asp:Label></span>

    <div class="feature-grid">
        <div class="feature-card">
            <span class="feature-card__title"><asp:Label ID="Label_Category1Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Category1Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><asp:Label ID="Label_Category2Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Category2Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card">
            <span class="feature-card__title"><asp:Label ID="Label_Category3Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Category3Text" runat="server"></asp:Label></span>
        </div>
    </div>

</asp:Content>
