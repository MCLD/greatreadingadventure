<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilyList.ascx.cs" Inherits="STG.SRP.Controls.FamilyList" %>


<div class="row">
	<div class="span2">
	</div>
    <div class="span8">

         <h3 class="title-divider">
              <asp:Label ID="Label1" runat="server" Text="FamAccts Title"></asp:Label>
         </h3>
        <hr />
<asp:Panel ID="pnlChange" runat="server" Visible="False">
    <asp:Label ID="Label3" runat="server" Text="FamAccts Impersonate Instructions"></asp:Label>
    <br /><br />
    <table>
        <tr>
            <td>
                    <asp:DropDownList ID="ddAccounts" runat="server" Width="500px">
                        <asp:ListItem Value="0" Text="Select an account to swich to and click the 'Go' button ... "></asp:ListItem>
                    </asp:DropDownList>            
            </td>

            <td valign="top">
                    &nbsp;<asp:Button runat="server" id="btnGo" Text="FamAccts Impersonate Go" 
                        CssClass="btn a" onclick="btnGo_Click" />
            </td>
        </tr>
    </table>

    <asp:Button runat="server" id="btncancel" Text="FamAccts Impersonate Cancel" CssClass="btn a" onclick="btncancel_Click"/>
</asp:Panel>

<asp:Panel ID="pnlList" runat="server" Visible="True">

        <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click"><img src="../ControlRoom/Images/user_group.png" height="48px" width="48px" /> Log in as another family member ... </asp:LinkButton>

        <hr />
        <table width="100%" cellpadding="3" cellspacing="0" border="0">
            <tr style="border-bottom: 1px solid silver;" class="RowColorAlt">
                <th style="font-weight: bold; " nowrap="nowrap" align="left">
                    <asp:Label ID="Label2" runat="server" Text="FamAccts List"></asp:Label>
                </th>
                <th align="right">
                    <asp:Button ID="btn" runat="server" Text="Add Child Account" 
                        CausesValidation="false"  CssClass="btn a"
                        Visible='True' onclick="btn_Click" />
                </th>
            </tr>
            <asp:Repeater ID="rptr" runat="server" onitemcommand="rptr_ItemCommand"  
                >
                <ItemTemplate>
                        
                     <tr class="RowColor" >
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

</asp:Panel>		
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
