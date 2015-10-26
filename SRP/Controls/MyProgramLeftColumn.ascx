<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyProgramLeftColumn.ascx.cs" Inherits="GRA.SRP.Controls.MyProgramLeftColumn" %>
<%@ Register src="Avatar.ascx" tagname="AvatarControl" tagprefix="ac1" %>
<%@ Register src="MyBadgesListControl.ascx" tagname="MyBadgesListControl" tagprefix="uc1" %>
<%@ Register src="MyGameLoggingNavControl.ascx" tagname="MyGameLoggingNavControl" tagprefix="uc3" %>

<ac1:AvatarControl runat="server" />
<uc3:MyGameLoggingNavControl ID="MyGameLoggingNavControl1" runat="server" />
<uc1:MyBadgesListControl ID="MyBadgesListControl1" runat="server" />