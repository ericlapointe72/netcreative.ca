<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="unsubscribe.aspx.cs" Inherits="netcreative.ca.unsubscribe" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="unsubscribe__container">
        <div class="card">
            <h1 class="card__title"><%: Global.Unsubscribe_H1Title %></h1>
            <span class="label__unsubscribe"><%: Global.Unsubscribe_Label %></span>
            <asp:TextBox class="textbox" ID="TextBox_Unsubscribe" runat="server"></asp:TextBox>
            <asp:Button class="button button__blue" ID="Button_Unsubscribe" runat="server" OnClick="Button_Unsubscribe_Click" />
        </div>
    </div>

</asp:Content>
