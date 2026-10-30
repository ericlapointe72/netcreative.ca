<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="netcreative.ca.Dashboard" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_DashboardMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_DashboardMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <% if (Stats.Incomplete) { %>
    <div class="admin-mfa-reminder"><%: Global.Text("Dash_Incomplete") %></div>
    <% } %>

    <%-- one card per thing worth a look; each one is a link to the page that has the detail --%>
    <div class="dash-grid">
        <a class="dash-stat dash-stat--ok" href="Subscribers.aspx">
            <span class="dash-stat__label"><%: Global.Text("Dash_Subscribers") %></span>
            <span class="dash-stat__value"><%: Stats.SubscribersConfirmed %></span>
            <span class="dash-stat__sub"><%: Global.Format(Global.Text("Dash_SubscribersSub"), Stats.SubscribersPending, Stats.SubscribersNew30) %></span>
        </a>
        <a class="dash-stat dash-stat--ok" href="ContactLog.aspx">
            <span class="dash-stat__label"><%: Global.Text("Dash_Messages") %></span>
            <span class="dash-stat__value"><%: Stats.ContactMonth %></span>
            <span class="dash-stat__sub"><%: MessagesSub %></span>
        </a>
        <a class="dash-stat dash-stat--ok" href="Newsletter.aspx">
            <span class="dash-stat__label"><%: Global.Text("Dash_Newsletter") %></span>
            <span class="dash-stat__value dash-stat__value--text"><%: Stats.NewsletterDate.HasValue ? ShortDay(Stats.NewsletterDate.Value) : "-" %></span>
            <span class="dash-stat__sub"><%: NewsletterSub %></span>
        </a>
        <a class="dash-stat <%= FailedClass %>" href="LoginLog.aspx">
            <span class="dash-stat__label"><%: Global.Text("Dash_FailedLogins") %></span>
            <span class="dash-stat__value"><%: Stats.FailedSignIns24h %></span>
            <span class="dash-stat__sub"><%: Global.Format(Global.Text("Dash_FailedLoginsSub"), Stats.FailedSignIns7d) %></span>
        </a>
        <a class="dash-stat <%= ErrorsClass %>" href="Logs.aspx">
            <span class="dash-stat__label"><%: Global.Text("Dash_Errors") %></span>
            <span class="dash-stat__value"><%: Stats.ErrorsToday %></span>
            <span class="dash-stat__sub"><%: Global.Format(Global.Text("Dash_ErrorsSub"), Stats.Errors7d) %></span>
        </a>
    </div>

    <div class="dash-panels">
        <div class="admin-card">
            <h2 class="dash-panel__title"><%: Global.Text("Dash_Recent") %></h2>
            <% if (Stats.Recent.Count == 0) { %>
            <p class="dash-empty"><%: Global.Text("Dash_MessagesNone") %></p>
            <% } else { %>
            <ul class="dash-list">
                <% foreach (var message in Stats.Recent) { %>
                <li>
                    <span class="dash-list__main"><%: message.Name %></span>
                    <span class="dash-list__detail"><%: message.ProjectType %></span>
                    <span class="dash-list__date"><%: Day(message.Date) %></span>
                </li>
                <% } %>
            </ul>
            <a class="dash-link" href="ContactLog.aspx"><%: Global.Text("Dash_RecentAll") %></a>
            <% } %>
        </div>

        <div class="admin-card">
            <h2 class="dash-panel__title"><%: Global.Text("Dash_PrevLogin") %></h2>
            <% if (Stats.PreviousSignIn.HasValue) { %>
            <p class="dash-empty"><%: Global.Format(Global.Text("Dash_PrevLoginText"), DayTime(Stats.PreviousSignIn.Value), Stats.PreviousSignInIp) %></p>
            <p class="dash-hint"><%: Global.Text("Dash_PrevLoginHint") %></p>
            <% } else { %>
            <p class="dash-empty"><%: Global.Text("Dash_PrevLoginNone") %></p>
            <% } %>
        </div>
    </div>

</asp:Content>
