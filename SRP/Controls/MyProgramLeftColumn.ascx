<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyProgramLeftColumn.ascx.cs" Inherits="STG.SRP.Controls.MyProgramLeftColumn" %>

<%@ Register src="MyBadgesListControl.ascx" tagname="MyBadgesListControl" tagprefix="uc1" %>
<%@ Register src="NotificationCounterControl.ascx" tagname="NotificationCounterControl" tagprefix="uc2" %>

<%@ Register src="MyGamemapNavControl.ascx" tagname="MyGamemapNavControl" tagprefix="uc3" %>

<uc3:MyGamemapNavControl ID="MyGamemapNavControl1" runat="server" />

<uc2:NotificationCounterControl ID="NotificationCounterControl1" runat="server" />

 <uc1:MyBadgesListControl ID="MyBadgesListControl1" runat="server" />
 
 <br />


 