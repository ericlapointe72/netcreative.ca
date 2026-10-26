<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ContactLog.aspx.cs" Inherits="netcreative.ca.ContactLog" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_ContactMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_ContactMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <asp:Label class="label__data" ID="Label_Contact_Info" runat="server"></asp:Label>
        <asp:Repeater ID="Repeater" runat="server" OnItemCommand="Repeater_ItemCommand" OnItemDataBound="Repeater_ItemDataBound">
            <ItemTemplate>
                <article class="msg">
                    <div class="msg__head">
                        <span class="msg__number">#<%# Eval("Number") %></span>
                        <span class="msg__date"><%#: Eval("When") %></span>
                    </div>
                    <h2 class="msg__name"><%#: Eval("Name") %></h2>
                    <asp:PlaceHolder runat="server" Visible='<%# Eval("HasCompany") %>'>
                        <div class="msg__company"><%#: Eval("Company") %></div>
                    </asp:PlaceHolder>
                    <div class="msg__contact">
                        <a class="msg__link msg__link--mail" href="mailto:<%#: Eval("Email") %>"><%#: Eval("Email") %></a>
                        <asp:PlaceHolder runat="server" Visible='<%# Eval("HasPhone") %>'>
                            <a class="msg__link msg__link--phone" href="tel:<%#: Eval("PhoneLink") %>"><%#: Eval("Phone") %></a>
                        </asp:PlaceHolder>
                    </div>
                    <div class="msg__tags">
                        <span class="msg__tag"><b><%: Global.Text("Msg_Project") %></b> <%#: Eval("ProjectType") %></span>
                        <span class="msg__tag"><b><%: Global.Text("Msg_Budget") %></b> <%#: Eval("Budget") %></span>
                        <span class="msg__tag"><b><%: Global.Text("Msg_Deadline") %></b> <%#: Eval("Deadline") %></span>
                    </div>
                    <div class="msg__text"><%#: Eval("Message") %></div>
                    <div class="msg__actions">
                        <a class="msg__reply" href="mailto:<%#: Eval("Email") %>"><%: Global.Text("Msg_Reply") %></a>
                        <asp:Button class="button button__red msg__delete" ID="Button_Row_Delete" runat="server" CommandName="delete" CommandArgument='<%# Eval("Number") %>' />
                    </div>
                </article>
            </ItemTemplate>
        </asp:Repeater>
        <div class="card__button">
            <asp:Button class="button" ID="Button_Newer" runat="server" OnClick="Button_Newer_Click" CausesValidation="false" />
            <asp:Button class="button" ID="Button_Older" runat="server" OnClick="Button_Older_Click" CausesValidation="false" />
        </div>
        <p class="logs-hint"><%: Global.Text("Admin_ContactRetention") %></p>
        <div class="card__button">
            <a class="button" href="<%= ResolveUrl("~/Admin/Export.ashx?what=contact") %>"><%: Global.Text("Admin_ExportCsv") %></a>
        </div>
    </div>
</asp:Content>
