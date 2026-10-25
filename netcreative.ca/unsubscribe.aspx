<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="unsubscribe.aspx.cs" Inherits="netcreative.ca.unsubscribe" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Unsubscribe_HeroEyebrow, Global.Unsubscribe_H1Title, Global.Unsubscribe_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="unsubscribe__container">
        <%-- the link of a newsletter: the address is shown and one click unsubscribes it --%>
        <asp:Panel ID="Panel_Confirm" runat="server" CssClass="card" Visible="false">
            <span class="unsubscribe__text"><asp:Literal ID="Literal_ConfirmText" runat="server"></asp:Literal></span>
            <asp:Button CssClass="button" ID="Button_Confirm" runat="server" OnClick="Button_Confirm_Click" />
            <asp:HiddenField ID="HiddenField_Token" runat="server" />
        </asp:Panel>

        <%-- the result (unsubscribed, or a link that is not valid) --%>
        <asp:Panel ID="Panel_Message" runat="server" CssClass="card" Visible="false">
            <span class="unsubscribe__text"><asp:Literal ID="Literal_Message" runat="server"></asp:Literal></span>
            <a class="button unsubscribe__back" href="/"><%: Global.Text("Sub_BackToSite") %></a>
        </asp:Panel>

        <%-- whoever lost the link: the address typed gets a new link by e-mail (nothing is removed from this page) --%>
        <asp:Panel ID="Panel_Form" runat="server" CssClass="card">
            <label class="label__unsubscribe" for="<%= TextBox_Unsubscribe.ClientID %>"><%: Global.Unsubscribe_Label %></label>
            <asp:TextBox CssClass="textbox tb__unsubscribe" ID="TextBox_Unsubscribe" runat="server" TextMode="Email" MaxLength="254"></asp:TextBox>
            <button type="button" class="button unsubscribe__submit"><%: Global.Text("Unsub_FormButton") %></button>
        </asp:Panel>
    </div>

    <script src="Content/js/unsubscribe.js?v=2"></script>

</asp:Content>
