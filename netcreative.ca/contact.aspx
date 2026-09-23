<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="contact.aspx.cs" Inherits="netcreative.ca.contact" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Page.Title = Global.Contact_Title;
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Contact_HeroEyebrow, Global.Contact_H1Title, Global.Contact_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card">
            <span class="section-label contact__label"><%: Global.Contact_CoordinatesLabel %></span>

            <div class="contact__grid">
                <div class="card__lastName">
                    <asp:TextBox class="tb__lastName textbox" ID="TextBox_LastName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <span class="lbl__lastName"><%: Global.Contact_LastName %></span>
                </div>

                <div class="card__firstName">
                    <asp:TextBox class="tb__firstName textbox" ID="TextBox_FirstName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <span class="lbl__firstName"><%: Global.Contact_FirstName %></span>
                </div>

                <div class="card__mail">
                    <asp:TextBox class="tb__mail textbox" ID="TextBox_EMail" required="required" runat="server" MaxLength="50" TextMode="Email"></asp:TextBox>
                    <span class="lbl__mail"><%: Global.Contact_Email %></span>
                </div>

                <div class="card__phone">
                    <asp:TextBox class="tb__phone textbox" ID="TextBox_Phone" required="required" runat="server" MaxLength="14" TextMode="Phone"></asp:TextBox>
                    <span class="lbl__phone"><%: Global.Contact_Phone %></span>
                </div>

                <div class="card__company contact__full">
                    <asp:TextBox class="tb__company textbox" ID="TextBox_Company" required="required" runat="server" MaxLength="50"></asp:TextBox>
                    <span class="lbl__company"><%: Global.Contact_Company %></span>
                </div>
            </div>

            <span class="section-label contact__label"><%: Global.Contact_ProjectLabel %></span>

            <div class="contact__grid contact__grid--project">
                <div class="card__dropdown">
                    <span class="lbl__dropdown"><%: Global.Contact_ProjectTypeLabel %></span>
                    <asp:DropDownList class="textbox" ID="DropDownList_Type" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <span class="lbl__dropdown"><%: Global.Contact_BudgetLabel %></span>
                    <asp:DropDownList class="textbox" ID="DropDownList_Budjet" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <span class="lbl__dropdown"><%: Global.Contact_DeadlineLabel %></span>
                    <asp:DropDownList class="textbox" ID="DropDownList_Deadline" runat="server"></asp:DropDownList>
                </div>
            </div>

            <div class="card__description contact__full">
                <asp:TextBox class="tb__description textbox" ID="TextBox_Description" required="required" runat="server" MaxLength="500" Rows="4" TextMode="MultiLine"></asp:TextBox>
                <span class="lbl__description"><%: Global.Contact_DescriptionLabel %></span>
            </div>

            <div class="card__verify">
                <span class="card__verify-label"><%: Global.Contact_VerifyLabel %></span>
                <div class="card__question">
                    <img class="image__question" src="Content/images/site/question_fr.png" id="Image_QuestionFR" runat="server"/>
                    <img class="image__question" src="Content/images/site/question_en.png" id="Image_QuestionEN" runat="server"/>
                    <asp:TextBox class="textbox__question" ID="TextBox_Question" runat="server"></asp:TextBox>
                </div>
            </div>

            <asp:Button class="button" ID="Button_Send" runat="server" OnClick="Button_Send_Click"/>
        </div>
    </div>

    <script src="Content/js/phone-mask.js"></script>

</asp:Content>
