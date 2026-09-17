<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="netcreative.ca.login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="login__container">
        <div class="card">
            <img class="card__avatar" src="Imgs_Site/avatar.png">
            <asp:Label class="card__title" ID="Label_Title" runat="server"></asp:Label>
            <div class="card__user">
                <img class="card__icon" src="Imgs_Site/login.png" />
                <asp:TextBox class="tb__user textbox" ID="TextBox_User" required="required" runat="server" MaxLength="20" ></asp:TextBox>
                <asp:Label class="lbl__user" ID="Label_User" runat="server"></asp:Label>
            </div>
            <div class="card__password">
                <img class="card__icon" src="Imgs_Site/lock.png" />
                <asp:TextBox class="tb__pass textbox" ID="TextBox_Password" required="required" runat="server" maxLength="20" TextMode="Password" ></asp:TextBox>
                <asp:Label class="lbl__pass" ID="Label_Password" runat="server"></asp:Label>
            </div>
            <asp:Button class="button button__green" ID="Button_Connection" runat="server" OnClick="Button_Connection_Click"/>
        </div>
    </div>

</asp:Content>
