<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="contact.aspx.cs" Inherits="netcreative.ca.contact" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card">
            <div class="section2">
                <div class="card__lastName">
                    <asp:TextBox class="tb__lastName textbox" ID="TextBox_LastName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <span class="lbl__lastName"><%: Global.Contact_LastName %></span>
                </div>

                <div class="card__firstName">
                    <asp:TextBox class="tb__firstName textbox" ID="TextBox_FirstName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <span class="lbl__firstName"><%: Global.Contact_FirstName %></span>
                </div>

                <div class="card__mail">
                    <asp:TextBox class="tb__mail textbox" ID="TextBox_EMail" required="required" runat="server" MaxLength="50"></asp:TextBox>
                    <span class="lbl__mail"><%: Global.Contact_Email %></span>
                </div>

                <div class="card__phone">
                    <asp:TextBox class="tb__phone textbox" ID="TextBox_Phone" required="required" runat="server" MaxLength="14" TextMode="Phone" placeholder="(000) 000-0000"></asp:TextBox>
                    <span class="lbl__phone"><%: Global.Contact_Phone %></span>
                </div>

                <div class="card__company">
                    <asp:TextBox class="tb__company textbox" ID="TextBox_Company" required="required" runat="server" MaxLength="50"></asp:TextBox>
                    <span class="lbl__company"><%: Global.Contact_Company %></span>
                </div>
            </div>

            <div class="section3">
                <div class="card__dropdown">
                    <span><%: Global.Contact_ProjectTypeLabel %></span>
                    <asp:DropDownList class="textbox" ID="DropDownList_Type" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <span><%: Global.Contact_BudgetLabel %></span>
                    <asp:DropDownList class="textbox" ID="DropDownList_Budjet" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <span><%: Global.Contact_DeadlineLabel %></span>
                    <asp:DropDownList class="textbox" ID="DropDownList_Deadline" runat="server"></asp:DropDownList>
                </div>
            </div>

            <div class="section4">
                <div class="card__description">
                    <asp:TextBox class="tb__description textbox" ID="TextBox_Description" required="required" runat="server" MaxLength="500" Rows="4" TextMode="MultiLine"></asp:TextBox>
                    <span class="lbl__description"><%: Global.Contact_DescriptionLabel %></span>
                </div>
            </div>

            <div class="section5">
                <div class="card__question">
                    <img class="image__question" src="Imgs_Site/question_fr.png" id="Image_QuestionFR" runat="server"/>
                    <img class="image__question" src="Imgs_Site/question_en.png" id="Image_QuestionEN" runat="server"/>
                    <asp:TextBox class="textbox__question" ID="TextBox_Question" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="section6">
                <asp:Button class="button button__green" ID="Button_Send" runat="server" OnClick="Button_Send_Click"/>
            </div>
        </div>
    </div>

    <script src="js/phone-mask.js"></script>

</asp:Content>
