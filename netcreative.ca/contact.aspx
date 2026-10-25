<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="contact.aspx.cs" Inherits="netcreative.ca.contact" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
    Master.SetPageHero("Content/images/site/hero-contact.jpg", Global.Contact_HeroEyebrow, Global.Contact_H1Title, Global.Contact_HeroText);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card">
            <h2 class="section-label contact__label"><%: Global.Contact_CoordinatesLabel %></h2>

            <div class="contact__grid">
                <div class="card__lastName">
                    <asp:TextBox class="tb__lastName textbox" ID="TextBox_LastName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <label class="lbl__lastName" for="<%= TextBox_LastName.ClientID %>"><%: Global.Contact_LastName %></label>
                </div>

                <div class="card__firstName">
                    <asp:TextBox class="tb__firstName textbox" ID="TextBox_FirstName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <label class="lbl__firstName" for="<%= TextBox_FirstName.ClientID %>"><%: Global.Contact_FirstName %></label>
                </div>

                <div class="card__mail">
                    <asp:TextBox class="tb__mail textbox" ID="TextBox_EMail" required="required" runat="server" MaxLength="50" TextMode="Email"></asp:TextBox>
                    <label class="lbl__mail" for="<%= TextBox_EMail.ClientID %>"><%: Global.Contact_Email %></label>
                </div>

                <div class="card__phone">
                    <asp:TextBox class="tb__phone textbox" ID="TextBox_Phone" required="required" runat="server" MaxLength="14" TextMode="Phone"></asp:TextBox>
                    <label class="lbl__phone" for="<%= TextBox_Phone.ClientID %>"><%: Global.Contact_Phone %></label>
                </div>

                <div class="card__company contact__full">
                    <asp:TextBox class="tb__company textbox" ID="TextBox_Company" required="required" runat="server" MaxLength="50"></asp:TextBox>
                    <label class="lbl__company" for="<%= TextBox_Company.ClientID %>"><%: Global.Contact_Company %></label>
                </div>
            </div>

            <h2 class="section-label contact__label"><%: Global.Contact_ProjectLabel %></h2>

            <div class="contact__grid contact__grid--project">
                <div class="card__dropdown">
                    <label class="lbl__dropdown" for="<%= DropDownList_Type.ClientID %>"><%: Global.Contact_ProjectTypeLabel %></label>
                    <asp:DropDownList class="textbox" ID="DropDownList_Type" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <label class="lbl__dropdown" for="<%= DropDownList_Budjet.ClientID %>"><%: Global.Contact_BudgetLabel %></label>
                    <asp:DropDownList class="textbox" ID="DropDownList_Budjet" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <label class="lbl__dropdown" for="<%= DropDownList_Deadline.ClientID %>"><%: Global.Contact_DeadlineLabel %></label>
                    <asp:DropDownList class="textbox" ID="DropDownList_Deadline" runat="server"></asp:DropDownList>
                </div>
            </div>

            <div class="card__description contact__full">
                <asp:TextBox class="tb__description textbox" ID="TextBox_Description" required="required" runat="server" MaxLength="500" Rows="4" TextMode="MultiLine"></asp:TextBox>
                <label class="lbl__description" for="<%= TextBox_Description.ClientID %>"><%: Global.Contact_DescriptionLabel %></label>
            </div>

            <div class="card__verify">
                <label class="card__verify-label" for="<%= TextBox_Question.ClientID %>"><%: Global.Contact_VerifyLabel %></label>
                <div class="card__question">
                    <asp:Image ID="Image_Question" runat="server" CssClass="image__question" Width="220" Height="64" />
                    <asp:TextBox class="textbox__question" ID="TextBox_Question" runat="server" MaxLength="2" inputmode="numeric" autocomplete="off"></asp:TextBox>
                </div>
                <span class="card__verify-help"><%: Global.Text("Contact_QuestionHelp") %></span>
            </div>

            <%-- the field only a robot fills, and the stamp of the time the form was shown (see FormTimer) --%>
            <asp:TextBox ID="TextBox_Website" runat="server" CssClass="hp-field" TabIndex="-1" autocomplete="off" aria-hidden="true"></asp:TextBox>
            <asp:HiddenField ID="HiddenField_Stamp" runat="server" />

            <asp:Button class="button" ID="Button_Send" runat="server" OnClick="Button_Send_Click"/>
        </div>
    </div>

    <script src="Content/js/phone-mask.js"></script>

</asp:Content>
