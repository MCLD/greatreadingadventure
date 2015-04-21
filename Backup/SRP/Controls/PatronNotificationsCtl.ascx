<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronNotificationsCtl.ascx.cs" Inherits="STG.SRP.Controls.PatronNotificationsCtl" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>

<script language="javascript" type="text/javascript">

    function printDiv(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;

        document.location.href = "MyNotifications.aspx";

        window.print();

        //document.body.innerHTML = originalContents;

    }
</script>

<asp:Panel ID="pnlList" runat="server" Visible="true">

<div class="row" style="min-height: 400px;">
	<div class="span12">
        <h1><asp:Label ID="Label1" runat="server" Text="Notifications Title"></asp:Label></h1>
        
        <table width="100%" cellpadding="5" cellspacing="0" border="0" style="border:0px solid red;">
            <tr class="RowColor RowBottomBorder">
                    <td valign="Top" align="center" style="width: 50px; " >
                    </td>
                    <td valign="middle" align="left" style="padding-left:10px; padding-right: 10px; font-size:larger; ">
                          <b>Message</b>
                    </td>
                    <td valign="middle" align="left" style="padding-left:10px; padding-right: 10px;  font-size:larger;">
                          <b>Date</b>
                    </td>
            </tr>

            <asp:Repeater runat="server" ID="rptr" onitemcommand="rptr_ItemCommand" >
                <ItemTemplate>            
                    <tr class="RowColor RowBottomBorder">
                    <td valign="bottom" align="center" style="width: 50px; ">
                          <asp:Button ID="btnView" runat="server" Text="View" CommandArgument='<%# Eval("NID") %>' CssClass="btn e" style="" />
                    <td valign="middle" align="left" style="padding-left:10px; padding-right: 10px;  font-size:larger;">
                          <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# Eval("Subject") %>' CommandArgument='<%# Eval("NID") %>'
                            Font-Bold='<%# Eval("isUnread") %>' ></asp:LinkButton>
                    </td>
                    <td valign="middle" align="left" style="padding-left:10px; padding-right: 10px;  font-size:larger;">
                          <asp:LinkButton ID="LinkButton2" runat="server" Text='<%# FormatHelper.ToNormalDate((DateTime)Eval("AddedDate")) %>' CommandArgument='<%# Eval("NID") %>'
                            Font-Bold='<%# Eval("isUnread") %>' ></asp:LinkButton>
                    </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>            
                    <tr class="RowColorAlt RowBottomBorder">
                    <td valign="bottom" align="center" style="width: 50px; ">
                          <asp:Button ID="btnView" runat="server" Text="View" CommandArgument='<%# Eval("NID") %>' CssClass="btn e" style="" />
                    <td valign="middle" align="left" style="padding-left:10px; padding-right: 10px;  font-size:larger;">
                          <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# Eval("Subject") %>' CommandArgument='<%# Eval("NID") %>'
                          Font-Bold='<%# Eval("isUnread") %>'></asp:LinkButton>
                    </td>
                    <td valign="middle" align="left" style="padding-left:10px; padding-right: 10px;  font-size:larger;">
                          <asp:LinkButton ID="LinkButton2" runat="server" Text='<%# FormatHelper.ToNormalDate((DateTime)Eval("AddedDate")) %>' CommandArgument='<%# Eval("NID") %>'
                          Font-Bold='<%# Eval("isUnread") %>' ></asp:LinkButton>
                    </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        
        
        
        </table>
        <br />
        &nbsp;<asp:Button ID="Button3" runat="server" Text="Ask A Question"
        CssClass="btn e"  Width="150px" onclick="btnAsk_Click"/>
	</div> 
</div> 

</asp:Panel>


<asp:Panel ID="pnlDetail" runat="server" Visible="false">

<div class="row" style="min-height: 400px;" >

	<div class="span12">
        
        <div id="printarea">         
            <center>
            
            </center>
            <h2><asp:Label ID="lblTitle" runat="server" Text=""></asp:Label></h2>
            <br />
            Received: <asp:Label ID="lblReceived" runat="server" Text=""></asp:Label> <br /> <br /> 
            <asp:Label ID="lblBody" runat="server" Text=""></asp:Label>
            <asp:Label ID="NID" runat="server" Text="" Visible="false"></asp:Label>
        </div>
                
        
        <br /><br /><br /><br />
        
        <asp:Button ID="Button1" runat="server" Text="Notifications btn Print"  CssClass="btn e" Width="150px" OnClientClick="printDiv('printarea')"/> 
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:Button ID="btnList" runat="server" Text="Go Back" onclick="btnList_Click" CssClass="btn e"  Width="150px"/>
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn e"  Width="150px" onclick="btnDelete_Click"/>
         &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:Button ID="btnAsk" runat="server" Text="Ask A Question" CssClass="btn e"  Width="150px" onclick="btnAsk_Click"/>
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:Button ID="btnUnread" runat="server" Text="Mark Unread" CssClass="btn e"  Width="150px" onclick="btnUnread_Click"/>

	</div> 

</div> 

</asp:Panel>

<asp:Panel ID="pnlAsk" runat="server" Visible="false">

<div class="row" style="min-height: 400px;" >

	<div class="span12">
        <h1><asp:Label ID="Label2" runat="server" Text="Notifications Ask Question Title"></asp:Label></h1>
        <div id="Div1">         
            <b>Subject</b><br /><asp:TextBox ID="txtSubject" runat="server" Text="" Width="80%"></asp:TextBox>
            <br /><br />
            <b>Question</b><br /> 
            <asp:TextBox ID="txtBody" runat="server" Text="" Rows="10" 
                TextMode="MultiLine" Width="80%"></asp:TextBox>
        </div>
                
        
        <br /><br /><br /><br />
        

       
       
        <asp:Button ID="btnAskSubmit" runat="server" Text="Submit Question"
                        CssClass="btn e"  Width="150px" onclick="btnAskSubmit_Click" />
         &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
       <asp:Button ID="Button5" runat="server" Text="Go Back" onclick="btnList_Click" CssClass="btn e"  Width="150px"/>

	</div> 

</div> 

</asp:Panel>


<asp:Panel ID="pnlDone" runat="server" Visible="false">

<div class="row" style="min-height: 400px;" >

	<div class="span12">
        <h1><asp:Label ID="Label3" runat="server" Text="Notifications Ask Question Submitted Done"></asp:Label></h1>
        
        
        <br /><br /><br /><br />
        
        
        
        <br /><br /><br /><br />
       
      
       <asp:Button ID="Button6" runat="server" Text="Go Back" onclick="btnList_Click" CssClass="btn e"  Width="150px"/>

	</div> 

</div> 

</asp:Panel>




</ContentTemplate>
</asp:UpdatePanel>





















