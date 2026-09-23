<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="privacy.aspx.cs" Inherits="netcreative.ca.privacy" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.Privacy_Title;
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Privacy_HeroEyebrow, Global.Privacy_H1Title1, Global.Privacy_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="privacy__container">
        <div class="card">
            <h2 class="card__subTitle"><%: Global.Privacy_SubTitle1 %></h2>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph1 %></span>
            <h2 class="card__subTitle"><%: Global.Privacy_SubTitle2 %></h2>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph2 %></span>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet1 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet2 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet3 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet4 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet5 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet6 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet7 %></span>
            </div>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph3 %></span>
            <h2 class="card__subTitle"><%: Global.Privacy_SubTitle3 %></h2>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph4 %></span>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet8 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet9 %></span>
            </div>
            <h2 class="card__subTitle"><%: Global.Privacy_SubTitle4 %></h2>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph5 %></span>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet10 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet11 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet12 %></span>
            </div>
            <div class="card__ctn">
                <i class="fas fa-square"></i>
                <span class="card__bullet"><%: Global.Privacy_Bullet13 %></span>
            </div>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph6 %></span>
            <h2 class="card__subTitle"><%: Global.Privacy_SubTitle5 %></h2>
            <span class="card__paragraphe"><%: Global.Privacy_Paragraph7 %></span>
        </div>
    </div>

</asp:Content>
