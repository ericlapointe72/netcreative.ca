<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Opening.aspx.cs" Inherits="netcreative.ca.Opening" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_OpeningMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_OpeningMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <div class="card__opening">
            <div class="header">
                <span class="label__opening"> </span>
                <span class="label__opening"><%: Global.Admin_MondayAbbrev %></span>
                <span class="label__opening"><%: Global.Admin_TuesdayAbbrev %></span>
                <span class="label__opening"><%: Global.Admin_WednesdayAbbrev %></span>
                <span class="label__opening"><%: Global.Admin_ThursdayAbbrev %></span>
                <span class="label__opening"><%: Global.Admin_FridayAbbrev %></span>
                <span class="label__opening"><%: Global.Admin_SaturdayAbbrev %></span>
                <span class="label__opening"><%: Global.Admin_SundayAbbrev %></span>
            </div>
            <div class="header">
                <span class="label__opening"><%: Global.Admin_OpeningLabel %></span>
            </div>
            <div class="openingHour">
                <span class="label__opening"><%: Global.Admin_HourLabel %></span>
                <asp:TextBox class="textbox__opening" ID="mon_oh" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="tue_oh" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="wed_oh" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="thu_oh" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="fri_oh" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sat_oh" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sun_oh" runat="server" MaxLength="2"></asp:TextBox>
            </div>
            <div class="openingMinute">
                <span class="label__opening"><%: Global.Admin_MinuteLabel %></span>
                <asp:TextBox class="textbox__opening" ID="mon_om" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="tue_om" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="wed_om" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="thu_om" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="fri_om" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sat_om" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sun_om" runat="server" MaxLength="2"></asp:TextBox>
            </div>
            <div class="header">
                <span class="label__opening"><%: Global.Admin_ClosingLabel %></span>
            </div>
            <div class="closingHour">
                <span class="label__opening"><%: Global.Admin_HourLabel %></span>
                <asp:TextBox class="textbox__opening" ID="mon_ch" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="tue_ch" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="wed_ch" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="thu_ch" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="fri_ch" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sat_ch" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sun_ch" runat="server" MaxLength="2"></asp:TextBox>
            </div>
            <div class="closingMinute">
                <span class="label__opening"><%: Global.Admin_MinuteLabel %></span>
                <asp:TextBox class="textbox__opening" ID="mon_cm" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="tue_cm" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="wed_cm" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="thu_cm" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="fri_cm" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sat_cm" runat="server" MaxLength="2"></asp:TextBox>
                <asp:TextBox class="textbox__opening" ID="sun_cm" runat="server" MaxLength="2"></asp:TextBox>
            </div>
        </div>

        <div class="card__button">
            <asp:Button class="button" ID="Button_Opening_Save" runat="server" OnClick="Button_Opening_Save_Click" />
        </div>
    </div>
</asp:Content>
