﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyNoGameProgramCenterColumn.ascx.cs" Inherits="STG.SRP.Controls.MyNoGameProgramCenterColumn" %>

<%@ Register src="SimpleLoggingControl.ascx" tagname="SimpleLoggingControl" tagprefix="uc1" %>

<center><asp:Image ID="imgAvatar" runat="server" /></center>


<uc1:SimpleLoggingControl ID="SimpleLoggingControl1" 
    runat="server" />

<br />
<asp:Label ID="lblSponsor" runat="server" Text="Label"></asp:Label>
<hr />
<asp:Label ID="lblFooter" runat="server" Text="Label"></asp:Label>