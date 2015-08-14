<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FramedSurvey.aspx.cs" Inherits="GRA.SRP.FramedSurvey" %>
<%@ Import Namespace="GRA.SRP.Controls" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register src="Controls/PatronSurvey.ascx" tagname="PatronSurvey" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Survey</title>
</head>
<body style="background-color:white!important;">
    <form id="form1" runat="server">
    <div style="padding-left:10px !important; padding-right:10px!important; border: 0px solid red;">
    <uc1:PatronSurvey ID="PatronSurvey1" runat="server" />
    </div>
    </form>
</body>
</html>
