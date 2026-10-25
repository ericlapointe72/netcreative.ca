<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Sql.aspx.cs" Inherits="netcreative.ca.Sql" %>
<%@ Import Namespace="netcreative.ca.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server"><%: Global.Admin_SqlMenu %></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server"><%: Global.Admin_SqlMenu %></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-card">
        <p class="sql-warning"><%= Global.Text("Sql_Warning") %></p>

        <div class="sql-shortcuts">
            <span class="label__data"><%: Global.Text("Sql_Templates") %></span>
            <% foreach (var t in Templates) { %>
            <button type="button" class="button button__sm" data-sql="<%= AttributeText(t.Sql) %>"><%: t.Label %></button>
            <% } %>
        </div>

        <asp:TextBox class="textbox textbox__multiline sql-box" ID="TextBoxSql" runat="server" Rows="10" TextMode="MultiLine" spellcheck="false"></asp:TextBox>

        <div class="card__button sql-actions">
            <asp:Button class="button button__red" ID="ButtonRun" runat="server" OnClick="ButtonRun_Click" OnClientClick="return ncSqlConfirm();" />
            <label class="sql-simulation">
                <asp:CheckBox ID="CheckSimulation" runat="server" Checked="true" />
                <span><%: Global.Text("Sql_Simulation") %></span>
            </label>
        </div>

        <%-- the password again, to change real data: a stolen session alone is not enough --%>
        <div class="sql-password">
            <span class="label__data"><%: Global.Text("Sql_PasswordLabel") %></span>
            <asp:TextBox class="textbox" ID="TextBoxPassword" runat="server" TextMode="Password" MaxLength="200" autocomplete="current-password"></asp:TextBox>
        </div>

        <asp:Label ID="LabelStatus" runat="server" EnableViewState="false"></asp:Label>
        <asp:Literal ID="LiteralResult" runat="server" Mode="PassThrough" EnableViewState="false" />
    </div>

    <script>
        (function () {
            var box = document.getElementById('<%= TextBoxSql.ClientID %>');
            var simulation = document.getElementById('<%= CheckSimulation.ClientID %>');
            var confirmText = <%= ConfirmTextJson %>;

            document.querySelectorAll('.sql-shortcuts [data-sql]').forEach(function (button) {
                button.addEventListener('click', function () {
                    box.value = button.getAttribute('data-sql');
                    box.focus();
                });
            });

            // a real run (simulation off) asks for a confirmation first
            window.ncSqlConfirm = function () {
                if (simulation.checked) return true;
                return confirm(confirmText);
            };
        })();
    </script>
</asp:Content>
