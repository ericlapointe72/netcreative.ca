<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="netcreative.ca.admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- jquery -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js" integrity="sha384-1H217gwSVyLSIfaLxHbE7dRb3v4mYCKbpQvzx0cegeju1MVsGrX5xXxAvs/HgeFs" crossorigin="anonymous"></script>
    <!-- datatables -->
    <script src="https://cdn.datatables.net/1.13.11/js/jquery.dataTables.min.js" integrity="sha384-xbKh5PcHqYD2znaTJ+mPamIq8ERw8yRfp72NI8CGRg51FffJAXidmePtdkmCjEh9" crossorigin="anonymous"></script>
    <link href="https://cdn.datatables.net/1.13.11/css/jquery.dataTables.min.css" integrity="sha384-1QCPE0Isvkd7i/DO1KTZ5GqTLhlGv8kAWBNQDo2jFBkImeSuFxZcEPU0SANETyv5" crossorigin="anonymous" rel="stylesheet" />

    <script type="text/javascript">
        $(document).ready(function () {
            $(".grid").prepend($("<thead></thead>").append($(this).find("tr:first"))).dataTable();
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="admin__container">
        <div class="card">
            <div class="card__menu">
                <asp:Button class="button button__blue" ID="Button_Contact" runat="server" OnClick="Button_Contact_Click" />
                <asp:Button class="button button__blue" ID="Button_Login" runat="server" OnClick="Button_Login_Click" />
                <asp:Button class="button button__blue" ID="Button_Navigation" runat="server" OnClick="Button_Navigation_Click" />
                <asp:Button class="button button__blue" ID="Button_Newsletter" runat="server" OnClick="Button_Newsletter_Click" />
                <asp:Button class="button button__blue" ID="Button_Opening" runat="server" OnClick="Button_Opening_Click" />
                <asp:Button class="button button__blue" ID="Button_Subscriber" runat="server" OnClick="Button_Subscriber_Click" />
                <asp:Button class="button button__blue" ID="Button_User" runat="server" OnClick="Button_User_Click" />
                <asp:Button class="button button__blue" ID="Button_Visitor" runat="server" OnClick="Button_Visitor_Click" />
                <asp:Button class="button button__red" ID="Button_Sql" runat="server" OnClick="Button_Sql_Click" />
            </div>

            <div class="card__content">
                <asp:MultiView ID="MultiView_Admin" runat="server">
                    <asp:View ID="View_Blank" runat="server">
                        <h1 class="card__title" id="H1_Blank" runat="server"></h1>
                    </asp:View>
                    <asp:View ID="View_Contact" runat="server">
                        <h1 class="card__title" id="H1_Contact" runat="server"></h1>
                        <asp:Repeater ID="Repeater" runat="server">
                            <ItemTemplate>
                                <asp:Label class="card__line" ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line1") %>'></asp:Label>
                                <asp:Label class="card__line" ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line2") %>'></asp:Label>
                                <asp:Label class="card__line" ID="Label3" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line3") %>'></asp:Label>
                                <asp:Label class="card__line" ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line4") %>'></asp:Label>
                                <asp:Label class="card__line" ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line5") %>'></asp:Label>
                                <asp:Label class="card__line card__line-last" ID="Label6" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Line6") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:Repeater>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_Contact_Delete" runat="server"></asp:Label>
                            <asp:TextBox CssClass="textbox" ID="TextBox_Contact_Delete" placeholder="#" MaxLength="5" runat="server"></asp:TextBox>
                            <asp:Button class="button button__red" ID="Button_Contact_Delete" runat="server" OnClick="Button_Contact_Delete_Click" />
                        </div>
                    </asp:View>
                    <asp:View ID="View_Login" runat="server">
                        <h1 class="card__title" id="H1_Login" runat="server"></h1>
                        <asp:GridView class="grid display cell-border" ID="GridView_Login" runat="server"></asp:GridView>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_Login_Delete" runat="server"></asp:Label>
                            <asp:TextBox CssClass="textbox" ID="TextBox_Login_Delete" placeholder="#" MaxLength="5" runat="server"></asp:TextBox>
                            <asp:Button class="button button__red" ID="Button_Login_Delete" runat="server" OnClick="Button_Login_Delete_Click" />
                        </div>
                    </asp:View>
                    <asp:View ID="View_Navigation" runat="server">
                        <h1 class="card__title" id="H1_Navigation" runat="server"></h1>
                        <asp:GridView class="grid display cell-border" ID="GridView_Navigation" runat="server"></asp:GridView>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_Navigation_Delete" runat="server"></asp:Label>
                            <asp:TextBox CssClass="textbox" ID="TextBox_Navigation_Delete" placeholder="Date" MaxLength="10" runat="server"></asp:TextBox>
                            <asp:Button class="button button__red" ID="Button_Navigation_Delete" runat="server" OnClick="Button_Navigation_Delete_Click" />
                        </div>
                    </asp:View>
                    <asp:View ID="View_Newsletter" runat="server">
                        <h1 class="card__title" id="H1_Newsletter" runat="server"></h1>
                        <div class="card__choice">
                            <asp:RadioButton class="radio__choice" ID="RadioButton_Newsletter_Test" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton_Newsletter_Test_CheckedChanged" />
                            <asp:Label class="label__email" ID="Label_Newsletter_Email" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Email" runat="server"></asp:TextBox>
                            <asp:RadioButton class="radio__choice" ID="RadioButton_Newsletter" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton_Newsletter_CheckedChanged" />
                        </div>
                        <div class="card__email">
                            <asp:Label class="label__email" ID="Label_Newsletter_Object" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Object" runat="server"></asp:TextBox>
                            <asp:Label class="label__email" ID="Label_Newsletter_Intro" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Intro" runat="server"></asp:TextBox>
                            <asp:Label class="label__email" ID="Label_Newsletter_Body" runat="server"></asp:Label>
                            <asp:TextBox class="textbox textbox__multiline" ID="TextBox_Newsletter_Body" runat="server" Rows="6" TextMode="MultiLine"></asp:TextBox>
                            <asp:Label class="label__email" ID="Label_Newsletter_Greeting" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_Newsletter_Greeting" runat="server"></asp:TextBox>
                        </div>
                        <div class="card__button">
                            <asp:Button class="button button__green" ID="Button_Newsletter_Send" runat="server" OnClick="Button_Newsletter_Send_Click" />
                        </div>
                    </asp:View>
                    <asp:View ID="View_Opening" runat="server">
                        <h1 class="card__title" id="H1_Opening" runat="server"></h1>
                        <div class="card__opening">
                            <div class="header">
                                <asp:Label class="label__opening" ID="Label_Empty" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Monday" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Tuesday" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Wednesday" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Thursday" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Friday" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Saturday" runat="server"></asp:Label>
                                <asp:Label class="label__opening" ID="Label_Sunday" runat="server"></asp:Label>
                            </div>
                            <div class="header">
                                <asp:Label class="label__opening" ID="Label_Opening" runat="server"></asp:Label>
                            </div>
                            <div class="openingHour">
                                <asp:Label class="label__opening" ID="Label_Hour1" runat="server"></asp:Label>
                                <asp:TextBox class="textbox__opening" ID="mon_oh" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="tue_oh" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="wed_oh" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="thu_oh" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="fri_oh" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="sat_oh" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="sun_oh" runat="server" MaxLength="2"></asp:TextBox>
                            </div>
                            <div class="openingMinute">
                                <asp:Label class="label__opening" ID="Label_Minute1" runat="server"></asp:Label>
                                <asp:TextBox class="textbox__opening" ID="mon_om" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="tue_om" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="wed_om" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="thu_om" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="fri_om" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="sat_om" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="sun_om" runat="server" MaxLength="2"></asp:TextBox>
                            </div>
                            <div class="header">
                                <asp:Label class="label__opening" ID="Label_Closing" runat="server"></asp:Label>
                            </div>
                            <div class="closingHour">
                                <asp:Label class="label__opening" ID="Label_Hour2" runat="server"></asp:Label>
                                <asp:TextBox class="textbox__opening" ID="mon_ch" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="tue_ch" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="wed_ch" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="thu_ch" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="fri_ch" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="sat_ch" runat="server" MaxLength="2"></asp:TextBox>
                                <asp:TextBox class="textbox__opening" ID="sun_ch" runat="server" MaxLength="2"></asp:TextBox>
                            </div>
                            <div class="closingMinute">
                                <asp:Label class="label__opening" ID="Label_Minute2" runat="server"></asp:Label>
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
                            <asp:Button class="button button__green" ID="Button_Opening_Save" runat="server" OnClick="Button_Opening_Save_Click" />
                        </div>
                    </asp:View>
                    <asp:View ID="View_Subscriber" runat="server">
                        <h1 class="card__title" id="H1_Subscriber" runat="server"></h1>
                        <div class="card__subscribercontent">
                            <asp:GridView class="grid display cell-border" ID="GridView_Subscribe" runat="server"></asp:GridView>
                            <div class="card__enterdata">
                                <asp:Label class="label__data" ID="Label_Subscriber_Delete" runat="server"></asp:Label>
                                <asp:TextBox class="textbox" ID="TextBox_Subscriber_Delete" placeholder="#" runat="server" MaxLength="5"></asp:TextBox>
                                <asp:Button class="button button__red" ID="Button_Subscriber_Delete" runat="server" OnClick="Button_Subscriber_Delete_Click" />
                            </div>
                        </div>
                    </asp:View>

                    <asp:View ID="View_User" runat="server">
                        <h1 class="card__title" id="H1_User" runat="server"></h1>
                        <div class="card__dropdown">
                            <asp:Label class="label__data" ID="Label_User_List" runat="server"></asp:Label>
                            <asp:DropDownList class="textbox" ID="DropDown_User" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDown_User_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_User_Name" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_User_Name" runat="server" MaxLength="20"></asp:TextBox>
                        </div>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_User_Password" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_User_Password" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_User_FirstName" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_User_FirstName" runat="server" MaxLength="20"></asp:TextBox>
                        </div>
                        <div class="card__enterdata">
                            <asp:Label class="label__data" ID="Label_User_LastName" runat="server"></asp:Label>
                            <asp:TextBox class="textbox" ID="TextBox_User_LastName" runat="server" MaxLength="20"></asp:TextBox>
                        </div>
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Contact" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Login" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Navigation" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Newsletter" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Opening" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Subscriber" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_User" runat="server" />
                        <asp:CheckBox class="checkbox" ID="CheckBox_User_Visitor" runat="server" />
                        <div class="card__button">
                            <asp:Button class="button button__green" ID="Button_User_Save" runat="server" OnClick="Button_User_Save_Click"/>
                            <asp:Button class="button button__green" ID="Button_User_SaveNew" runat="server" OnClick="Button_User_SaveNew_Click" />
                            <asp:Button class="button button__blue button__last" ID="Button_User_New" runat="server" OnClick="Button_User_New_Click" />
                            <asp:Button class="button button__red" ID="Button_User_Delete" runat="server" OnClick="Button_User_Delete_Click" />
                        </div>
                    </asp:View>

                    <asp:View ID="View_Visitor" runat="server">
                        <h1 class="card__title" id="H1_Visitor" runat="server"></h1>
                        <div class="card__visitorcontent">
                            <asp:GridView class="grid display cell-border" ID="GridView_Visitor" runat="server"></asp:GridView>
                            <div class="card__enterdata">
                                <asp:Label class="label__data" ID="Label_Visitor_Delete" runat="server"></asp:Label>
                                <asp:TextBox CssClass="textbox" ID="TextBox_Visitor_Delete" placeholder="Date" MaxLength="10" runat="server"></asp:TextBox>
                                <asp:Button class="button button__red" ID="Button_Visitor_Delete" runat="server" OnClick="Button_Visitor_Delete_Click"/>
                            </div>
                        </div>
                    </asp:View>

                    <asp:View ID="View_Sql" runat="server">
                        <h1 class="card__title" id="H1_Sql" runat="server"></h1>
                        <asp:Label class="label__data label__sqlwarning" ID="Label_Sql_Warning" runat="server"></asp:Label>
                        <asp:TextBox class="textbox textbox__multiline" ID="TextBox_Sql_Query" runat="server" Rows="8" TextMode="MultiLine"></asp:TextBox>
                        <div class="card__button">
                            <asp:Button class="button button__red" ID="Button_Sql_Execute" runat="server" OnClick="Button_Sql_Execute_Click" />
                        </div>
                        <asp:Label class="label__data" ID="Label_Sql_Status" runat="server"></asp:Label>
                        <asp:GridView class="grid display cell-border" ID="GridView_Sql_Result" runat="server"></asp:GridView>
                    </asp:View>

                </asp:MultiView>
            </div>
        </div>
    </div>

</asp:Content>
