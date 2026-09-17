<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="dutyfreeops.aspx.cs" Inherits="netcreative.ca.dutyfreeops" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="hero hero--blue observer">
        <span class="hero__eyebrow"><asp:Label ID="Label_HeroEyebrow" runat="server"></asp:Label></span>
        <h1 runat="server" class="hero__title" id="H1title"></h1>
        <asp:Label class="hero__text" ID="Label_HeroText" runat="server"></asp:Label>
        <a class="hero__cta" href="contact.aspx"><asp:Label ID="Label_HeroCta" runat="server"></asp:Label></a>
    </div>

    <div class="content-section">
        <asp:Label class="content-section__text" ID="Label_Intro" runat="server"></asp:Label>
    </div>

    <span class="section-label"><asp:Label ID="Label_ModulesLabel" runat="server"></asp:Label></span>

    <div class="feature-grid">
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><asp:Label ID="Label_Module1Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Module1Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><asp:Label ID="Label_Module2Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Module2Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><asp:Label ID="Label_Module3Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Module3Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><asp:Label ID="Label_Module4Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Module4Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><asp:Label ID="Label_Module5Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Module5Text" runat="server"></asp:Label></span>
        </div>
        <div class="feature-card feature-card--blue">
            <span class="feature-card__title"><asp:Label ID="Label_Module6Title" runat="server"></asp:Label></span>
            <span class="feature-card__text"><asp:Label ID="Label_Module6Text" runat="server"></asp:Label></span>
        </div>
    </div>

</asp:Content>
