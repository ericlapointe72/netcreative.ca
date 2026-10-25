<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="netcreative.ca.login" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("../Content/images/site/hero-login.jpg", Global.Login_HeroEyebrow, Global.Login_Welcome, Global.Login_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card login__card" data-network-error="<%: Global.Login_NetworkError %>">
            <h2 class="section-label contact__label"><%: Global.Login_Title %></h2>

            <div class="card__user">
                <img class="card__icon" src="../Content/images/site/login.png" alt="" aria-hidden="true" />
                <asp:TextBox class="tb__user textbox" ID="TextBox_User" required="required" runat="server" MaxLength="20"></asp:TextBox>
                <label class="lbl__user" for="<%= TextBox_User.ClientID %>"><%: Global.Login_User %></label>
            </div>

            <div class="card__password">
                <img class="card__icon" src="../Content/images/site/lock.png" alt="" aria-hidden="true" />
                <asp:TextBox class="tb__pass textbox" ID="TextBox_Password" required="required" runat="server" maxLength="20" TextMode="Password"></asp:TextBox>
                <label class="lbl__pass" for="<%= TextBox_Password.ClientID %>"><%: Global.Login_Password %></label>
            </div>

            <%-- second step of two-factor sign-in: shown by login.js when the server asks for a code --%>
            <div class="card__code" hidden>
                <img class="card__icon" src="../Content/images/site/lock.png" alt="" aria-hidden="true" />
                <input type="text" id="login_code" class="tb__code textbox" required="required" maxlength="20" autocomplete="one-time-code" spellcheck="false" />
                <label class="lbl__code" for="login_code"><%: Global.Login_CodeLabel %></label>
            </div>
            <p class="login__hint" hidden><%: Global.Login_CodeHint %></p>

            <button type="button" class="button login__submit" data-label-code="<%: Global.Login_CodeButton %>"><%: Global.Login_ConnectionButton %></button>
        </div>
    </div>

    <script src="../Content/js/login.js?v=20261022"></script>

</asp:Content>
