<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="netcreative.ca.Users" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_UserMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_UserMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <%-- the users: who exists, who has two-factor sign-in; a click on a name selects it for the form below --%>
    <asp:Panel ID="Panel_Management" runat="server">
        <div class="admin-card user-card">
            <asp:GridView ID="GridView_Users" runat="server" CssClass="grid user-grid" AutoGenerateColumns="false" DataKeyNames="UserName"
                OnSelectedIndexChanged="GridView_Users_SelectedIndexChanged" SelectedRowStyle-CssClass="user-grid__selected" BorderWidth="0">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton_Select" runat="server" CommandName="Select" Text='<%# Server.HtmlEncode(Convert.ToString(Eval("UserName"))) %>' CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="FullName" />
                    <asp:TemplateField>
                        <ItemTemplate><%# Eval("TfaHtml") %></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p class="tfa-hint"><%: Global.Text("Tfa_UsersHint") %></p>
        </div>

        <%-- the form: the selected user, or a new one --%>
        <div class="admin-card user-card">
            <h2 class="tfa-title"><asp:Literal ID="Literal_FormTitle" runat="server" /></h2>
            <div class="user-form">
                <div class="user-field">
                    <span class="label__data"><%: Global.Admin_UserNameLabel %></span>
                    <asp:TextBox class="textbox" ID="TextBox_User_Name" runat="server" MaxLength="20"></asp:TextBox>
                </div>
                <div class="user-field">
                    <span class="label__data"><%: Global.Admin_UserFirstNameLabel %></span>
                    <asp:TextBox class="textbox" ID="TextBox_User_FirstName" runat="server" MaxLength="20"></asp:TextBox>
                </div>
                <div class="user-field">
                    <span class="label__data"><%: Global.Admin_UserPasswordLabel %></span>
                    <asp:TextBox class="textbox" ID="TextBox_User_Password" runat="server" MaxLength="64" TextMode="Password" autocomplete="new-password"></asp:TextBox>
                </div>
                <div class="user-field">
                    <span class="label__data"><%: Global.Admin_UserLastNameLabel %></span>
                    <asp:TextBox class="textbox" ID="TextBox_User_LastName" runat="server" MaxLength="20"></asp:TextBox>
                </div>
                <div class="user-field">
                    <span class="label__data"><%: Global.Text("Admin_UserPasswordConfirmLabel") %></span>
                    <asp:TextBox class="textbox" ID="TextBox_User_Password2" runat="server" MaxLength="64" TextMode="Password" autocomplete="new-password"></asp:TextBox>
                </div>
                <div class="user-field">
                    <span class="label__data"><%: Global.Text("Admin_UserTfaLabel") %></span>
                    <span class="user-tfa"><asp:Literal ID="Literal_User_Tfa" runat="server" /></span>
                </div>
            </div>
            <div class="card__button">
                <asp:Button class="button" ID="Button_User_Save" runat="server" OnClick="Button_User_Save_Click"/>
                <asp:Button class="button" ID="Button_User_SaveNew" runat="server" OnClick="Button_User_SaveNew_Click" />
                <asp:Button class="button" ID="Button_User_New" runat="server" OnClick="Button_User_New_Click" />
                <asp:Button class="button" ID="Button_User_Reset2fa" runat="server" OnClick="Button_User_Reset2fa_Click" />
                <asp:Button class="button button__red" ID="Button_User_Delete" runat="server" OnClick="Button_User_Delete_Click" />
            </div>
        </div>
    </asp:Panel>

    <%-- the signed-in user's own two-factor sign-in (see Totp and LoginHandler.ashx) --%>
    <h2 class="user-section"><%: Global.Text("Tfa_Mine_Title") %></h2>
    <p class="tfa-help"><%: Global.Text("Tfa_Intro") %></p>

    <asp:Literal ID="LiteralNotice" runat="server" Mode="PassThrough" EnableViewState="false" />
    <asp:Label ID="LabelMessage" runat="server" CssClass="tfa-error" EnableViewState="false" />

    <%-- the columns of USERS are not created yet --%>
    <asp:Panel ID="Panel_Unavailable" runat="server" Visible="false" CssClass="admin-card tfa-card">
        <h2 class="tfa-title"><%: Global.Text("Tfa_Unavailable_Title") %></h2>
        <p><%: Global.Text("Tfa_Unavailable_Text") %></p>
    </asp:Panel>

    <%-- not turned on --%>
    <asp:Panel ID="Panel_Start" runat="server" Visible="false" CssClass="admin-card tfa-card">
        <h2 class="tfa-title"><%: Global.Text("Tfa_Start_Title") %></h2>
        <p><%: Global.Text("Tfa_Start_Text") %></p>
        <div class="card__button">
            <asp:Button ID="Button_Start" runat="server" CssClass="button" OnClick="Button_Start_Click" />
        </div>
    </asp:Panel>

    <%-- being turned on: the key, and the first code to prove the app is set up --%>
    <asp:Panel ID="Panel_Confirm" runat="server" Visible="false" CssClass="admin-card tfa-card" DefaultButton="Button_Confirm">
        <h2 class="tfa-title"><%: Global.Text("Tfa_Confirm_Title") %></h2>
        <ol class="tfa-steps">
            <li><%: Global.Text("Tfa_Step1") %></li>
            <li>
                <%: Global.Text("Tfa_Step2_Account") %> <strong><asp:Literal ID="Literal_Account" runat="server" /></strong><br />
                <%: Global.Text("Tfa_Step2_Key") %> <strong class="tfa-key"><asp:Literal ID="Literal_Secret" runat="server" /></strong> <%: Global.Text("Tfa_Step2_KeyType") %><br />
                <span class="tfa-hint"><%: Global.Text("Tfa_LinkHint") %> <asp:Literal ID="Literal_Link" runat="server" /></span>
            </li>
            <li><%: Global.Text("Tfa_Step3") %></li>
        </ol>
        <div class="tfa-field">
            <label for="<%= TextBox_Confirm.ClientID %>" class="label__data"><%: Global.Text("Tfa_CodeLabel") %></label>
            <asp:TextBox ID="TextBox_Confirm" runat="server" CssClass="textbox" MaxLength="10" autocomplete="off" inputmode="numeric" />
        </div>
        <div class="card__button">
            <asp:Button ID="Button_Confirm" runat="server" CssClass="button" OnClick="Button_Confirm_Click" />
            <asp:Button ID="Button_Cancel" runat="server" CssClass="button" OnClick="Button_Cancel_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <%-- turned on --%>
    <asp:Panel ID="Panel_Manage" runat="server" Visible="false" CssClass="admin-card tfa-card">
        <h2 class="tfa-title"><%: Global.Text("Tfa_Manage_Title") %></h2>
        <p class="tfa-hint"><%: Global.Text("Tfa_Manage_Text") %></p>
        <div class="tfa-field">
            <label for="<%= TextBox_Current.ClientID %>" class="label__data"><%: Global.Text("Tfa_CurrentLabel") %></label>
            <%-- Enter does nothing here: the two buttons below change the account, each must be clicked on purpose --%>
            <asp:TextBox ID="TextBox_Current" runat="server" CssClass="textbox" MaxLength="10" autocomplete="off" inputmode="numeric"
                onkeydown="if (event.key === 'Enter') { event.preventDefault(); }" />
        </div>
        <div class="card__button">
            <asp:Button ID="Button_Regenerate" runat="server" CssClass="button" OnClick="Button_Regenerate_Click" />
            <asp:Button ID="Button_Disable" runat="server" CssClass="button button__red" OnClick="Button_Disable_Click" />
        </div>
    </asp:Panel>

</asp:Content>
