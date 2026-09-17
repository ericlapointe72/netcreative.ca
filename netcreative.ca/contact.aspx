<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="contact.aspx.cs" Inherits="netcreative.ca.contact" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="contact__container">
        <div class="card">
            <div class="section1">
                <img class="card__contact" src="Imgs_Site/contact.png" alt="contact">
                <h1 runat="server" class="card__title" id="H1title"></h1>    
            </div>
                                    
            <div class="section2">
                <div class="card__lastName">
                    <asp:TextBox class="tb__lastName textbox" ID="TextBox_LastName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <asp:Label class="lbl__lastName" ID="Label_LastName" runat="server"></asp:Label>
                </div>

                <div class="card__firstName">
                    <asp:TextBox class="tb__firstName textbox" ID="TextBox_FirstName" required="required" runat="server" MaxLength="30"></asp:TextBox>
                    <asp:Label class="lbl__firstName" ID="Label_FirstName" runat="server"></asp:Label>
                </div>

                <div class="card__mail">
                    <asp:TextBox class="tb__mail textbox" ID="TextBox_EMail" required="required" runat="server" MaxLength="50"></asp:TextBox>
                    <asp:Label class="lbl__mail" ID="Label_EMail" runat="server"></asp:Label>
                </div>

                <div class="card__phone">
                    <asp:TextBox class="tb__phone textbox" ID="TextBox_Phone" required="required" runat="server" MaxLength="20"></asp:TextBox>
                    <asp:Label class="lbl__phone" ID="Label_Phone" runat="server"></asp:Label>
                </div>

                <div class="card__company">
                    <asp:TextBox class="tb__company textbox" ID="TextBox_Company" required="required" runat="server" MaxLength="50"></asp:TextBox>
                    <asp:Label class="lbl__company" ID="Label_Company" runat="server"></asp:Label>
                </div>
            </div>

            <div class="section3">
                <div class="card__dropdown">
                    <asp:Label ID="Label_Project_type" runat="server"></asp:Label>
                    <asp:DropDownList class="textbox" ID="DropDownList_Type" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <asp:Label ID="Label_Project_budjet" runat="server"></asp:Label>
                    <asp:DropDownList class="textbox" ID="DropDownList_Budjet" runat="server"></asp:DropDownList>
                </div>

                <div class="card__dropdown">
                    <asp:Label ID="Label_Project_deadline" runat="server"></asp:Label>    
                    <asp:DropDownList class="textbox" ID="DropDownList_Deadline" runat="server"></asp:DropDownList>
                </div>
            </div>

            <div class="section4">
                <div class="card__description">
                    <asp:TextBox class="tb__description textbox" ID="TextBox_Description" required="required" runat="server" MaxLength="500" Rows="4" TextMode="MultiLine"></asp:TextBox>
                    <asp:Label class="lbl__description" ID="Label_Description" runat="server"></asp:Label>
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

</asp:Content>
