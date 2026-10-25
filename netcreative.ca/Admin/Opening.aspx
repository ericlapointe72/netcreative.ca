<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Opening.aspx.cs" Inherits="netcreative.ca.Opening" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_OpeningMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_OpeningMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card opening-card">
        <p class="opening-intro"><%: Global.Text("Opening_Intro") %></p>

        <div class="opening-days" id="opening-days">
            <div class="opening-head" aria-hidden="true">
                <span></span>
                <span></span>
                <span><%: Global.Admin_OpeningLabel %></span>
                <span><%: Global.Admin_ClosingLabel %></span>
            </div>

            <asp:Repeater ID="Repeater_Days" runat="server">
                <ItemTemplate>
                    <div class="opening-day" role="group" aria-label="<%#: Eval("Name") %>" data-name="<%#: Eval("Name") %>">
                        <span class="opening-day__name"><%#: Eval("Name") %></span>
                        <label class="opening-switch">
                            <asp:CheckBox ID="CheckBox_Open" runat="server" Checked='<%# Eval("Open") %>' />
                            <span class="opening-switch__track"></span>
                            <span class="opening-switch__on"><%: Global.Text("Opening_Open") %></span>
                            <span class="opening-switch__off"><%: Global.Master_Closed %></span>
                        </label>
                        <label class="opening-time">
                            <span class="opening-time__label"><%: Global.Admin_OpeningLabel %></span>
                            <asp:TextBox ID="TextBox_From" runat="server" TextMode="Time" Text='<%# Eval("From") %>'></asp:TextBox>
                        </label>
                        <label class="opening-time">
                            <span class="opening-time__label"><%: Global.Admin_ClosingLabel %></span>
                            <asp:TextBox ID="TextBox_To" runat="server" TextMode="Time" Text='<%# Eval("To") %>'></asp:TextBox>
                        </label>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <button type="button" class="opening-copy" id="opening-copy"><%: Global.Text("Opening_CopyMonday") %></button>

        <h2 class="opening-preview__title"><%: Global.Text("Opening_PreviewTitle") %></h2>
        <div class="opening-preview" id="opening-preview"
            data-english="<%= PreviewEnglish ? "1" : "0" %>"
            data-closed="<%: Global.Master_Closed %>"
            data-hour-separator="<%: Global.Master_HourSeparator %>"
            data-time-separator="<%: Global.Master_TimeSeparator %>"
            data-am="<%: Global.Master_TimeAm %>"
            data-pm="<%: Global.Master_TimePm %>"
            data-invalid="<%: Global.Text("Opening_PreviewInvalid") %>">
            <asp:Literal ID="Literal_Preview" runat="server"></asp:Literal>
        </div>
        <p class="opening-note"><%: Global.Text("Opening_PreviewNote") %></p>

        <div class="card__button">
            <asp:Button class="button" ID="Button_Opening_Save" runat="server" OnClick="Button_Opening_Save_Click" />
        </div>
    </div>
    <script src="<%= ResolveUrl("~/Content/js/opening.js?v=1") %>"></script>
</asp:Content>
