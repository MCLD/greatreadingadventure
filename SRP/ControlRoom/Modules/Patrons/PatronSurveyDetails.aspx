<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="PatronSurveyDetails.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronSurveyDetails" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="../../Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronContext ID="pcCtl" runat="server" />
    <br />
<asp:Label ID="SRID" runat="server" Text="" Visible="false"></asp:Label>
<table cellpadding="5">
    <tr>
        <td><b>Survey/Test Name: </b></td>
        <td colspan = "3"><asp:Label ID="Name" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td width="25%"><b>Completed:</b> <asp:Label ID="C" runat="server" Text=""></asp:Label></td>
        <td width="25%"><b>Raw Score:</b> <asp:Label ID="S" runat="server" Text=""></asp:Label></td>
        <td width="25%"><b>% Score:</b> <asp:Label ID="P" runat="server" Text=""></asp:Label></td>
        <td width="25%"><b>Date:</b> <asp:Label ID="D" runat="server" Text=""></asp:Label></td>
    </tr>
</table>

<table width="100%" border=0>
<asp:Repeater ID="rptr" runat="server">
    <ItemTemplate>
        <tr id="Tr1" runat='server' 
            visible='<%# ((int)Eval("ShowQText") == 1 && (int)Eval("QType") != 4) || ((int)Eval("ShowQText") == 1 && (int)Eval("QType") == 4) %>' ><td colspan="2"> <hr /></td></tr>
        <tr>
            <td colspan="2"><%# (int)Eval("ShowQText") == 1 ? Eval("QText") : "<hr width='95%' style='float: right; '>" %></td>
        </tr>
        <tr>
            <td width="50px"></td>
            <td >
                <asp:Label ID="FreeFormAnswer" runat="server" Text='<%# Eval("FreeFormAnswer") %>' Visible='<%# (int)Eval("QType") == 3 %>'></asp:Label>
                <asp:Label ID="AnswerIDs" runat="server" Text='<%# Eval("ChoiceAnswerIDs") %>' Visible="false"></asp:Label>
                <asp:Label ID="Answers" runat="server" Text='<%# Eval("ChoiceAnswerText") %>' Visible="false"></asp:Label>
                <asp:Label ID="Clarification" runat="server" Text='<%# Eval("ClarificationText") %>'  Visible="false"></asp:Label>
                

                
                <%# DisplayAnswers((int)Eval("QType"), (int)Eval("QID"), (int)Eval("SQMLID"), Eval("ChoiceAnswerIDs").ToString(), Eval("ChoiceAnswerText").ToString(), Eval("ClarificationText").ToString(),
                                                (int)Eval("ShowQText") == 1)%>
                


            </td>
        </tr>
        
    </ItemTemplate>
</asp:Repeater>
</table>
</asp:Content>
