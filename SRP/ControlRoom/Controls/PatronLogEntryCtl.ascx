<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronLogEntryCtl.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.PatronLogEntryCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
 <%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
 
<asp:Label ID="PID" runat="server" Text="Label" Visible="false"></asp:Label>
<asp:Label ID="PPID" runat="server" Text="Label" Visible="false"></asp:Label>

<hr />
    <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000" ></asp:Label>

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="None" BorderWidth="1px" 
        Font-Bold="True" 
        HeaderText="Please correct the following errors: "  ForeColor="Red"
    />

<table>
    <tr>
        <td valign="top" style="font-weight: bold;">Log Entry Type: <br /><i>Reason</i></td>
        <td valign="top" >  
            <asp:DropDownList ID="AwardReasonCd" runat="server" AutoPostBack="true" 
                onselectedindexchanged="AwardReason_SelectedIndexChanged">
                <asp:ListItem Value="0">Reading</asp:ListItem>
                <asp:ListItem Value="1">Attended Event</asp:ListItem>
                <asp:ListItem Value="2">Completed Challenge</asp:ListItem>
                <asp:ListItem Value="4">Won a Mini-Game</asp:ListItem>
            </asp:DropDownList>  
        </td>
    </tr>

    <tr>
        <td style="font-weight: bold;">Award Date: </td>
        <td>
                <asp:TextBox ID="AwardDate" runat="server" Text='' 
                    Width="100px"  CssClass="datepicker"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceEventDate" runat="server" TargetControlID="AwardDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meEventDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="AwardDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>    

                <asp:RequiredFieldValidator ID="rfvDOB" runat="server" 
                    ControlToValidate="AwardDate" Display="Dynamic" ErrorMessage="Award Date is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="AwardDate" 
                    ErrorMessage="Invalid Award Date format" Type="Date" 
                    Operator="DataTypeCheck" Display="Dynamic" Text="* Invalid format" ForeColor="Red" ></asp:CompareValidator>
        
        </td>
    </tr>

    <tr>
        <td style="font-weight: bold;"># Points Awarded: </td>
        <td>
                <asp:TextBox ID="NumPoints" runat="server" Text='' 
                     Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumPoints" runat="server" 
                    ControlToValidate="NumPoints" Display="Dynamic" ErrorMessage="Num Points is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revNumPoints"
                    ControlToValidate="NumPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Num Points must be numeric.</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Num Points must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumPoints"
                    ControlToValidate="NumPoints"
                    MinimumValue="0"
                    MaximumValue="99999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Num Points must be from 0 to 99,999!</font>"
                    runat="server"  
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Num Points must be from 0 to 99,999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
                    
                    
                            
        </td>
    </tr>
        
    <asp:Panel ID="pnlEditOnly" runat="server" Visible="true">

    <tr>
        <td style="font-weight: bold;">Badge Awarded? </td>
        <td>
             &nbsp; <asp:Label ID="BadgeAwarded" runat="server" Text="" Visible="true"></asp:Label>
        </td>
    </tr>
    </asp:Panel>

    <asp:Panel ID="pnlEvent" runat="server" Visible="false">
    <tr>
        <td style="font-weight: bold;">Event Code:  </td>
        <td>
                  <asp:TextBox ID="EventCode" runat="server"></asp:TextBox>
        </td>
    </tr>
    </asp:Panel>

    <asp:Panel ID="pnlBook" runat="server" Visible="false">
    <tr>
        <td style="font-weight: bold;">Challenge:  </td>
        <td>
                  <asp:TextBox ID="lblBookList" runat="server"></asp:TextBox>
        </td>
    </tr>
    </asp:Panel>


    <asp:Panel ID="pnlGame" runat="server" Visible="false">
    <tr>
        <td style="font-weight: bold;">Game:  </td>
        <td>
                  <asp:TextBox ID="lblGame" runat="server"></asp:TextBox>
        </td>
    </tr>
    </asp:Panel>


    <asp:Panel ID="pnlMini" runat="server" Visible="false">
    <tr>
        <td style="font-weight: bold;">Mini-Game:  </td>
        <td>
                  <asp:TextBox ID="lblMGame" runat="server"></asp:TextBox>
        </td>
    </tr>
    </asp:Panel>

</table>

                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnBack_Click" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"    Tooltip="Refresh Record"
                        AlternateText="Refresh Record" 
    onclick="btnRefresh_Click" />   
                        &nbsp;
                    <asp:ImageButton ID="ImageButton1" runat="server" 
                        CausesValidation="True" 
                        CommandName="Save" 
                        ImageUrl="~/ControlRoom/Images/save.png" 
                        Height="25"
                        Text="Save"   Tooltip="Save"
                        AlternateText="Save"  
                         oncommand="ImageButton1_Command"/>     
                        &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Text="Save and return"   Tooltip="Save and return"
                        AlternateText="Save and return"  oncommand="ImageButton1_Command"/> 

