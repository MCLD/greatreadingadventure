<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronLogin.ascx.cs" Inherits="STG.SRP.Classes.PatronLogin" %>
<%@ Import Namespace="STG.SRP.DAL" %>
            

<div class="container">
    <div class="form-wrapper form-medium">
	      <h3 class="title-divider">
              <asp:Label ID="Label1" runat="server" Text="LoginForm Title"></asp:Label>
          </h3>
              <asp:Label ID="Label2" runat="server" Text="LoginForm no account"></asp:Label> 
              <a href="/Register.aspx"><asp:Label ID="Label3" runat="server" Text="LoginForm register"/></a>.
              <br />
              <asp:Label ID="Label4" runat="server" Text="LoginForm forgot pass"></asp:Label> 
              <a href="/Recover.aspx"><asp:Label ID="Label5" runat="server" Text="LoginForm recover"/></a>.
          <br />
          <br />

        <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000" ></asp:Label>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
        ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" ForeColor="#CC0000" Font-Bold="True" />

	      <h5><asp:Label ID="Label6" runat="server" Text="LoginForm username"></asp:Label></h5>
          <asp:TextBox ID="PUserName" runat="server" cssclass="input-block-level"></asp:TextBox>
        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
            ControlToValidate="PUserName" ErrorMessage="Username is required" 
            ToolTip="Username required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>

	      <h5><asp:Label ID="Label7" runat="server" Text="LoginForm password"></asp:Label></h5>
          <asp:TextBox ID="PPassword" runat="server" cssclass="input-block-level" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
            ControlToValidate="PPassword" ErrorMessage="Password is required"
            ToolTip="Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
	      
          <!--
	      <label class="checkbox">
              <asp:CheckBox ID="chkAgree" runat="server" />
	        <asp:Label ID="Label8" runat="server" Text="LoginForm terms"></asp:Label>
          </label>
          -->
          <br /><br />
        <asp:Button ID="btnLogin" runat="server" cssclass="btn btn-primary" 
              Text="LoginForm button" CausesValidation="true" Width="150px"
                ValidationGroup="uxLogin" onclick="btnLogin_Click"  />

	    </div>

</div>