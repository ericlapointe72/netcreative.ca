<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Newsletter.aspx.cs" Inherits="netcreative.ca.Newsletter" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_NewsletterMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_NewsletterMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card news-card">
        <p class="news-intro"><%: Global.Text("News_Intro") %></p>

        <div class="news-modes">
            <label class="news-mode">
                <asp:RadioButton ID="RadioButton_Newsletter_Test" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton_Newsletter_Test_CheckedChanged" />
                <span class="news-mode__box"></span>
                <span class="news-mode__title"><%: Global.Text("News_ModeTest") %></span>
                <span class="news-mode__text"><%: Global.Text("News_ModeTestText") %></span>
            </label>
            <label class="news-mode news-mode--all">
                <asp:RadioButton ID="RadioButton_Newsletter" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton_Newsletter_CheckedChanged" />
                <span class="news-mode__box"></span>
                <span class="news-mode__title"><%: Global.Text("News_ModeAll") %></span>
                <span class="news-mode__text"><%: Global.Format(Global.Text("News_ModeAllText"), SubscriberCount) %></span>
            </label>
        </div>

        <asp:Panel ID="Panel_Test" runat="server" CssClass="news-field">
            <label for="<%= TextBox_Newsletter_Email.ClientID %>"><%: Global.Admin_NewsletterEmailLabel %></label>
            <asp:TextBox ID="TextBox_Newsletter_Email" runat="server" CssClass="news-input" TextMode="Email"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel_Warning" runat="server" CssClass="news-warning">
            <%: Global.Text("News_EverybodyWarning") %>
        </asp:Panel>

        <div class="news-layout">
            <div class="news-form">
                <div class="news-field">
                    <label for="<%= TextBox_Newsletter_Object.ClientID %>"><%: Global.Admin_NewsletterObjectLabel %></label>
                    <asp:TextBox ID="TextBox_Newsletter_Object" runat="server" CssClass="news-input" MaxLength="150"></asp:TextBox>
                    <span class="news-count" id="news-count" data-template="<%: Global.Text("News_SubjectCount") %>" data-ideal="60"></span>
                </div>
                <div class="news-field">
                    <label for="<%= TextBox_Newsletter_Intro.ClientID %>"><%: Global.Admin_NewsletterIntroLabel %></label>
                    <asp:TextBox ID="TextBox_Newsletter_Intro" runat="server" CssClass="news-input"></asp:TextBox>
                </div>
                <div class="news-field">
                    <label for="<%= TextBox_Newsletter_Body.ClientID %>"><%: Global.Admin_NewsletterBodyLabel %></label>
                    <asp:TextBox ID="TextBox_Newsletter_Body" runat="server" CssClass="news-input news-body" Rows="10" TextMode="MultiLine"></asp:TextBox>
                </div>
                <div class="news-field">
                    <label for="<%= TextBox_Newsletter_Greeting.ClientID %>"><%: Global.Admin_NewsletterGreetingLabel %></label>
                    <asp:TextBox ID="TextBox_Newsletter_Greeting" runat="server" CssClass="news-input"></asp:TextBox>
                </div>
            </div>

            <div class="news-side">
                <h2 class="news-preview__title"><%: Global.Text("News_PreviewTitle") %></h2>
                <div class="news-preview" id="news-preview" data-empty="<%: Global.Text("News_PreviewEmpty") %>">
                    <div class="news-preview__subject"><span><%: Global.Text("News_PreviewSubject") %></span> <strong id="np-subject"></strong></div>
                    <div class="news-mail">
                        <p id="np-intro"></p>
                        <p id="np-body"></p>
                        <p id="np-greeting"></p>
                        <p><img src="<%= ResolveUrl("~/Content/images/site/signature.png") %>" alt="Net.Cr&eacute;ative" /></p>
                        <p class="news-mail__unsub"><%: Global.Admin_NewsletterUnsubscribe1 %><u><%: Global.Admin_NewsletterUnsubscribe2 %></u><%: Global.Admin_NewsletterUnsubscribe3 %></p>
                    </div>
                </div>
            </div>
        </div>

        <div class="card__button" data-sending="<%: Global.Text("News_Sending") %>">
            <asp:Button class="button" ID="Button_Newsletter_Send" runat="server" OnClick="Button_Newsletter_Send_Click" />
        </div>

        <asp:Panel ID="Panel_Report" runat="server" CssClass="news-report" Visible="false" EnableViewState="false">
            <asp:Literal ID="Literal_Report" runat="server" EnableViewState="false"></asp:Literal>
        </asp:Panel>
    </div>
    <script src="<%= ResolveUrl("~/Content/js/newsletter.js?v=1") %>"></script>
</asp:Content>
