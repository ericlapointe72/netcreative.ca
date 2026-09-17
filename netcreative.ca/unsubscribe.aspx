<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="unsubscribe.aspx.cs" Inherits="netcreative.ca.unsubscribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="unsubscribe__container">
        <div class="card">
            <h1 runat="server" class="card__title" id="H1title"></h1>
            <asp:Label class="label__unsubscribe" ID="Label_Unsubscribe" runat="server"></asp:Label>
            <asp:TextBox class="textbox" ID="TextBox_Unsubscribe" runat="server"></asp:TextBox>
            <asp:Button class="button button__blue" ID="Button_Unsubscribe" runat="server" OnClick="Button_Unsubscribe_Click" />
        </div>
    </div>

</asp:Content>
