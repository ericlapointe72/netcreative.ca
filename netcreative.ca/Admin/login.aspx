<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="netcreative.ca.login" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.Login_Title;
    Master.SetPageHero("../Content/images/site/hero-login.jpg", Global.Login_HeroEyebrow, Global.Login_Welcome, Global.Login_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card login__card" data-network-error="<%: Global.Login_NetworkError %>">
            <span class="section-label contact__label"><%: Global.Login_Title %></span>

            <div class="card__user">
                <img class="card__icon" src="../Content/images/site/login.png" />
                <asp:TextBox class="tb__user textbox" ID="TextBox_User" required="required" runat="server" MaxLength="20"></asp:TextBox>
                <span class="lbl__user"><%: Global.Login_User %></span>
            </div>

            <div class="card__password">
                <img class="card__icon" src="../Content/images/site/lock.png" />
                <asp:TextBox class="tb__pass textbox" ID="TextBox_Password" required="required" runat="server" maxLength="20" TextMode="Password"></asp:TextBox>
                <span class="lbl__pass"><%: Global.Login_Password %></span>
            </div>

            <button type="button" class="button login__submit"><%: Global.Login_ConnectionButton %></button>
        </div>
    </div>

    <script src="../Content/js/login.js"></script>

</asp:Content>
