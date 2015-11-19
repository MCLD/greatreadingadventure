<%@ Page Title="Password Recovery"
    Language="C#"
    MasterPageFile="~/Layout/SRP.Master"
    AutoEventWireup="true"
    CodeBehind="PasswordRecovery.aspx.cs"
    Inherits="GRA.SRP.PasswordRecovery" %>

<%@ Register Src="~/Controls/ProgramTabs.ascx" TagName="ProgramTabs" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ProgramBanner.ascx" TagName="ProgramBanner" TagPrefix="uc2" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript" type="text/javascript">
        function ClientValidate(source, arguments) {
            if (document.getElementById("<%=NPassword.ClientID %>").value == document.getElementById("<%=NPasswordR.ClientID %>").value) {
                arguments.IsValid = true;
            }
            else {
                arguments.IsValid = false;
            }
        }
    </script>

    <div class="container">
        <div class="form-wrapper form-medium">
            <h3 class="title-divider">
                <asp:Label ID="Label1" runat="server" Text="change-password-title"></asp:Label>
            </h3>
            <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
            <asp:Panel ID="pnlfields" runat="server" Visible="true">

                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                    ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" ForeColor="#CC0000" Font-Bold="True" />
                <h5>
                    <asp:Label ID="Label7" runat="server" Text="change-password-new"></asp:Label>
                </h5>
                <asp:TextBox ID="NPassword" runat="server" CssClass="input-block-level" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NPasswordReq" runat="server"
                    ControlToValidate="NPassword" ErrorMessage="New password is required"
                    ToolTip="New password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="uxNewPasswordStrengthValidator" CssClass="MessageFailure"
                    runat="server" ControlToValidate="NPassword" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                    Display="Dynamic" ValidationGroup="uxLogin" EnableClientScript="false"
                    ErrorMessage="New Password must be at least seven characters in length and contain one alpha and one numeric character.&lt;br&gt;"></asp:RegularExpressionValidator>
                <h5>
                    <asp:Label ID="Label18" runat="server" Text="change-password-verify"></asp:Label>
                </h5>
                <asp:TextBox ID="NPasswordR" runat="server" CssClass="input-block-level" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                    ControlToValidate="NPasswordR" ErrorMessage="New password re-entry is required"
                    ToolTip="New password re-entry required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                <asp:CustomValidator ID="uxConfirmNewPasswordCustomValidator" CssClass="MessageFailure" ValidationGroup="uxLogin"
                    runat="server" ControlToValidate="NPasswordR" EnableClientScript="false" Display="Dynamic"
                    ErrorMessage="The New Password and Confirmation of New Password do not match.&lt;br&gt;"
                    ClientValidationFunction="ClientValidate"></asp:CustomValidator>
                <br />
                <br />
                <asp:Button ID="btnLogin" runat="server" CssClass="btn a"
                    Text="change-password-submit" CausesValidation="true" Width="150px"
                    ValidationGroup="uxLogin" OnClick="btnLogin_Click" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
