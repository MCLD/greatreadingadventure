<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecoverPassword.ascx.cs" Inherits="STG.SRP.Classes.RecoverPassword" %>
<%@ Import Namespace="STG.SRP.DAL" %>
            

<div class="container">
    <div class="form-wrapper form-medium">
	      <h3 class="title-divider">
              <asp:Label ID="Label1" runat="server" Text="RecoverForm Title"></asp:Label>
          </h3>
              <asp:Label ID="Label2" runat="server" Text="RecoverForm no account"></asp:Label> 
              <a href="/Register.aspx"><asp:Label ID="Label3" runat="server" Text="RecoverForm register"/></a>.
              <br />
              <asp:Label ID="Label4" runat="server" Text="RecoverForm go login"></asp:Label> 
              <a href="/Login.aspx"><asp:Label ID="Label5" runat="server" Text="RecoverForm login"/></a>.
          <br />
          <br />

        <asp:Label ID="lbMessage" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000" ></asp:Label>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
        ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxRecover" ForeColor="#CC0000" Font-Bold="True" />

	      <h5><asp:Label ID="Label6" runat="server" Text="RecoverForm email"></asp:Label></h5>
          <asp:TextBox ID="PUsername" runat="server" cssclass="input-block-level"></asp:TextBox>
        <asp:RequiredFieldValidator ID="EmailRequired" runat="server" 
            ControlToValidate="PUsername" ErrorMessage="Email address is required" 
            ToolTip="Email address is required" ValidationGroup="uxRecover" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>

	      
          <!--
	      <label class="checkbox">
              <asp:CheckBox ID="chkAgree" runat="server" />
	        <asp:Label ID="Label8" runat="server" Text="RecoverForm terms"></asp:Label>
          </label>
          -->
          <br /><br />
        <asp:Button ID="btnEmail" runat="server" cssclass="btn btn-primary" 
              Text="RecoverForm button" CausesValidation="true" Width="150px"
                ValidationGroup="uxRecover" onclick="btnEmail_Click"   />

	    </div>

</div>