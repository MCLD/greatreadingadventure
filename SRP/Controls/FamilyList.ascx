<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilyList.ascx.cs" Inherits="STG.SRP.Controls.FamilyList" %>


<div class="row">
	<div class="span2">
	</div>
    <div class="span8">

         <h3 class="title-divider">
              <asp:Label ID="Label1" runat="server" Text="FamAccts Title"></asp:Label>
         </h3>
        
        <table width="100%" cellpadding="3" cellspacing="0" border="0">
                <th style="font-weight: bold; background-color: #f1f1f1;" nowrap="nowrap" align="left">
                    <asp:Label ID="Label2" runat="server" Text="FamAccts List"></asp:Label>
                </th>
                <th align="right">
                    <asp:Button ID="btn" runat="server" Text="Add Child Account" 
                        CausesValidation="false"  CssClass="btn a"
                        Visible='True' onclick="btn_Click" />
                </th>

            <asp:Repeater ID="rptr" runat="server" onitemcommand="rptr_ItemCommand"  
                >
                <ItemTemplate>
                        
                     <tr class="RowColor">
                        <td nowrap="nowrap" >  
                             <%# Eval("FirstName") + " " + Eval("LastName")%>  ( <%# Eval("username") %> )             
                        </td>
                        <td width="100%" align="right">
                            <asp:Button runat="server" id="btnLog" CommandName="log" CommandArgument='<%# Eval("PID") %>' Text="FamAccts Log" CssClass="btn a"/>
                            &nbsp;
                            <asp:Button runat="server" id="Button1" CommandName="act" CommandArgument='<%# Eval("PID") %>' Text="FamAccts Manage Account" CssClass="btn a"/>
                            &nbsp;
                            <asp:Button runat="server" id="btnPwd" CommandName="pwd" CommandArgument='<%# Eval("PID") %>' Text="FamAccts Change Password" CssClass="btn a"/>

                        </td>
                    </tr>
                        
                </ItemTemplate>

                <AlternatingItemTemplate>
                     <tr class="RowColorAlt">
                        <td nowrap="nowrap" >  
                             <%# Eval("FirstName") + " " + Eval("LastName")%>  ( <%# Eval("username") %> )             
                        </td>
                        <td width="100%" align="right">
                            <asp:Button runat="server" id="btnLog" CommandName="log" CommandArgument='<%# Eval("PID") %>' Text="FamAccts Log" CssClass="btn a"/>
                            &nbsp;
                            <asp:Button runat="server" id="Button1" CommandName="act" CommandArgument='<%# Eval("PID") %>' Text="FamAccts Manage Account" CssClass="btn a"/>
                            &nbsp;
                            <asp:Button runat="server" id="btnPwd" CommandName="pwd" CommandArgument='<%# Eval("PID") %>' Text="FamAccts Change Password" CssClass="btn a"/>

                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        
        </table>

		
        <p>&nbsp;</p>
        <p>&nbsp;</p>
        <p> &nbsp;</p>
        <p> &nbsp;</p>
        <p>&nbsp; </p>
        <p>&nbsp; </p>
	</div>
    <div class="span2">
	</div>
</div>
