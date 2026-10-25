<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="privacy.aspx.cs" Inherits="netcreative.ca.privacy" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Privacy_HeroEyebrow, Global.Privacy_H1Title1, Global.Privacy_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="privacy__container">
        <div class="card">
            <span class="card__paragraphe privacy__updated"><%: Global.Text("Priv_Updated") %></span>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T1") %></h2>
            <%-- the paragraphs that hold a link are written as HTML in the resources (<%= %>); the others are plain text (<%: %>) --%>
            <span class="card__paragraphe"><%= Global.Text("Priv_P1") %></span>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T2") %></h2>
            <span class="card__paragraphe"><%: Global.Text("Priv_P2") %></span>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B1") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B2") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B3") %></span></div>
            <span class="card__paragraphe"><%: Global.Text("Priv_P3") %></span>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T3") %></h2>
            <span class="card__paragraphe"><%: Global.Text("Priv_P4") %></span>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B4") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B5") %></span></div>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T4") %></h2>
            <span class="card__paragraphe"><%: Global.Text("Priv_P5") %></span>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B6") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B7") %></span></div>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T5") %></h2>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B8") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B9") %></span></div>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T6") %></h2>
            <span class="card__paragraphe"><%: Global.Text("Priv_P6") %></span>
            <span class="card__paragraphe"><%= Global.Text("Priv_P7") %></span>

            <h2 class="card__subTitle"><%: Global.Text("Priv_T7") %></h2>
            <span class="card__paragraphe"><%: Global.Text("Priv_P8") %></span>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B10") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B11") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B12") %></span></div>
            <div class="card__ctn"><i class="ic ic-square" aria-hidden="true"></i><span class="card__bullet"><%: Global.Text("Priv_B13") %></span></div>
            <span class="card__paragraphe"><%: Global.Text("Priv_P9") %></span>
        </div>
    </div>

</asp:Content>
