<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Events.ascx.cs" Inherits="STG.SRP.Controls.Events" %>
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
        window.print();

        document.location.href = "Events.aspx";
    }
</script>

<asp:Panel ID="pnlList" runat="server" Visible="true">

<div class="row" style="min-height: 400px;">
	<div class="span12">
        <h1><asp:Label ID="Label1" runat="server" Text="Events Title"></asp:Label></h1>
        
        <table width="100%" cellpadding="5" cellspacing="0" border="0">
        <tr><td><hr /></td></tr>

            <tr>
                <td>
                
<table  width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td width="90px" nowrap=nowrap ><b>Start Date:</b> &nbsp;</td>
        <td>
                        <asp:TextBox ID="StartDate" runat="server" Width="75px"                         
                            Text='' CssClass="datepicker"></asp:TextBox>

        </td>
        <td  width="90px" nowrap=nowrap><b>End Date:</b> &nbsp;</td>
        <td>
                        <asp:TextBox ID="EndDate" runat="server" Width="75px"                         
                            Text='' CssClass="datepicker"></asp:TextBox>

        </td>

        <td  width="150px" nowrap=nowrap><b>Branch/Library:</b> &nbsp;</td>
        <td>
                        <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True" Width="200px"
                         >
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
        </td>
        <td>
            <asp:Button ID="btnFilter" runat="server" Text="Events btn Filter" 
                onclick="btnFilter_Click" CssClass="btn e"/>
                &nbsp;
            <asp:Button ID="btnClear" runat="server" Text="Events btn Clear" onclick="btnClear_Click"  CssClass="btn e"
                 />

        </td>
    </tr>
</table>

                
                </td>
            </tr>
            <tr><td><hr /></td></tr>
            <asp:Repeater runat="server" ID="rptr" onitemcommand="rptr_ItemCommand" >
                <ItemTemplate>
            
            <tr  style = "border-left: 0px solid black;">
            
                <td width="100%" valign="bottom" align="left" style="padding-left:300px; padding-right: 300px;padding-bottom: 15px;">
                    <h5><asp:Label ID="Label1" runat="server" Text="Events Label When"></asp:Label> 
                        <%# ((DateTime)Eval("EventDate")).ToNormalDate() %> <%# Eval("EventTime") %> 

                        <%# Eval("EndDate") == DBNull.Value ? "" : " thru " + ((DateTime)Eval("EndDate")).ToNormalDate().Replace("01/01/1900","")%> 

                        <%# (Eval("EndTime").ToString() != "" ? (Eval("EndDate") == DBNull.Value ? " - " + Eval("EndTime") : Eval("EndTime")) : "")%>


                    </h5>
                    <h5><asp:Label ID="Label2" runat="server" Text="Events Label What"></asp:Label> <%# Eval("EventTitle") %></h5>
                    <h6><asp:Label ID="Label3" runat="server" Text="Events Label Where"></asp:Label> <%# Eval("Branch")%></h6>
                    <%# (Eval("HTML").ToString().Length > 300 ?  Eval("HTML").ToString().Substring(0, 300) + " ..." : Eval("HTML"))%>
                    <br /><br />
                    <asp:Button ID="btnView" runat="server" Text="Events Btn Details" CommandArgument='<%# Eval("EID") %>' Visible="true" CssClass="" />
                    <hr />
                </td>
            
            </tr>              
                
                
                </ItemTemplate>
            </asp:Repeater>
        
        </table>
	</div> 
</div> 

</asp:Panel>



<asp:Panel ID="pnlDetail" runat="server" Visible="false">

<div class="row" style="min-height: 400px;" >
<div class="span2"></div>
	<div class="span8">
        <div id="printarea"> 
            <center>
            <h2><asp:Label ID="lblTitle" runat="server" Text=""></asp:Label></h2>
            <br />
            <h3><asp:Label ID="lblWhen" runat="server" Text=""></asp:Label> - <asp:Label ID="lblWhere" runat="server" Text=""></asp:Label></h3>
            <br /><br />
            <div style="text-align: left;">
                <asp:Label ID="lblHtml" runat="server" Text=""></asp:Label>

                <asp:Panel ID="Panel1" runat="server" Visible="false">
                    <b><asp:Label ID="CF1Label" runat="server" Text="Label"></asp:Label>: </b><asp:Label ID="CF1Value" runat="server" Text="Label"></asp:Label>
                </asp:Panel>
                <asp:Panel ID="Panel2" runat="server" Visible="false">
                    <b><asp:Label ID="CF2Label" runat="server" Text="Label"></asp:Label>: </b><asp:Label ID="CF2Value" runat="server" Text="Label"></asp:Label>
                </asp:Panel>
                <asp:Panel ID="Panel3" runat="server" Visible="false">
                    <b><asp:Label ID="CF3Label" runat="server" Text="Label"></asp:Label>: </b><asp:Label ID="CF3Value" runat="server" Text="Label"></asp:Label>
                </asp:Panel>

            </div>
            </center>
        </div>
        <center>
        <br /><br /><br /><br />
        
        <asp:Button ID="Button1" runat="server" Text="Events btn Print"  CssClass="btn e" Width="150px" OnClientClick="printDiv('printarea')"/> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:Button ID="btnList" runat="server" Text="Events btn Back" onclick="btnList_Click" CssClass="btn e"  Width="150px"/>
        </center>
	</div> 
</div> 

</asp:Panel>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>


<script language="javascript" type="text/javascript">
    $(function () {
        $(".datepicker").datepicker();
    });
</script>


</ContentTemplate>
</asp:UpdatePanel>