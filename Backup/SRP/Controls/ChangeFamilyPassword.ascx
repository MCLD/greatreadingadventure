<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeFamilyPassword.ascx.cs" Inherits="STG.SRP.Classes.ChangeFamilyPassword" %>
<%@ Import Namespace="STG.SRP.DAL" %>
            
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
              <asp:Label ID="Label1" runat="server" Text="FamPwdResetForm Title"></asp:Label>
          </h3>
              
          <h4 class="">
              Changing password for: <asp:Label ID="lblAccount" runat="server" Text=""></asp:Label>
          </h4>

          <br />
          <br />

        <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000" ></asp:Label>
        <asp:Panel ID="pnlfields" runat="server" Visible="true">

        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
        ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" ForeColor="#CC0000" Font-Bold="True" />

	      <h5><asp:Label ID="Label6" runat="server" Text="FamPwdResetForm current"></asp:Label></h5>
          <asp:TextBox ID="CPass" runat="server" cssclass="input-block-level" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="CPassRequired" runat="server" 
            ControlToValidate="CPass" ErrorMessage="Current password is required" 
            ToolTip="Current password is required" ValidationGroup="uxLogin" 
                Display="None" EnableClientScript="false"></asp:RequiredFieldValidator>

	      <h5><asp:Label ID="Label7" runat="server" Text="FamPwdResetForm newpassword"></asp:Label></h5>
          <asp:TextBox ID="NPassword" runat="server" cssclass="input-block-level" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="NPasswordReq" runat="server" 
            ControlToValidate="NPassword" ErrorMessage="New password is required"
            ToolTip="New password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
    <asp:RegularExpressionValidator ID="uxNewPasswordStrengthValidator" CssClass="MessageFailure"
        runat="server" ControlToValidate="NPassword" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
        Display="Dynamic" ValidationGroup="uxLogin" EnableClientScript="false"
        ErrorMessage="New Password must be at least seven characters in length and contain one alpha and one numeric character.&lt;br&gt;"></asp:RegularExpressionValidator>

	      
	      <h5><asp:Label ID="Label18" runat="server" Text="FamPwdResetForm newpassword repeat"></asp:Label></h5>
          <asp:TextBox ID="NPasswordR" runat="server" cssclass="input-block-level" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
            ControlToValidate="NPasswordR" ErrorMessage="New password re-entry is required"
            ToolTip="New password re-entry required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
    <asp:CustomValidator ID="uxConfirmNewPasswordCustomValidator" CssClass="MessageFailure" ValidationGroup="uxLogin"
        runat="server" ControlToValidate="NPasswordR" EnableClientScript="false" Display="Dynamic"
        ErrorMessage="The New Password and Confirmation of New Password do not match.&lt;br&gt;" 
                            ClientValidationFunction="ClientValidate"></asp:CustomValidator>   
          <!--
	      <label class="checkbox">
              <asp:CheckBox ID="chkAgree" runat="server" />
	        <asp:Label ID="Label8" runat="server" Text="FamPwdResetForm terms"></asp:Label>
          </label>
          -->
          <br /><br />
        <asp:Button ID="btnLogin" runat="server" cssclass="btn a" 
              Text="FamPwdResetForm button" CausesValidation="true" Width="150px"
                ValidationGroup="uxLogin" onclick="btnLogin_Click"  />
                 &nbsp;
         <asp:Button ID="btnCancel" runat="server" cssclass="btn a" 
              Text="FamPwdResetForm button2" CausesValidation="false" Width="150px" onclick="btnCancel_Click"
                  />
                  <asp:TextBox ID="SA" runat="server" Text="" Visible="False"></asp:TextBox>
                </asp:Panel>

	    </div>
                
</div>