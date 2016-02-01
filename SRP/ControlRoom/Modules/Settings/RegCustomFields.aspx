<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="RegCustomFields.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.RegCustomFields" 
    
%>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:TemplateField HeaderText=" " SortExpression="" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                 <table style="font-weight: bold;">
                    <tr>
                        <td width="250px" align="right" valign="middle">First Custom Field Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Label1" runat="server" Text='<%# Eval("Label1") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>                                        
                    <tr>
                        <td width="250px" align="right" valign="middle">Enable Field: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:CheckBox ID="Use1" runat="server" Checked='<%# (bool)Eval("Use1") %>' ReadOnly="False"></asp:CheckBox>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Field Dropdown Values: &nbsp;</td>
                        <td width="350px" align="left" valign="middle" colspan="4" nowrap="nowrap">
                            
                            <asp:DropDownList ID="DDValues1" runat="server" DataSourceID="odsDDCodeTypes" DataTextField="CodeTypeName" DataValueField="CTID" 
                                AppendDataBoundItems="True" Width="200px"
                             >
                                <asp:ListItem Value="" Text="No Dropdown/Free Form"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="DDValues1Lbl" runat="server" Text='<%# Eval("DDValues1") %>' Visible="False"></asp:Label>
                            &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="btnCodes" runat="server" 
                                CausesValidation="false" 
                                CommandName="codes" 
                                ImageUrl="~/ControlRoom/RibbonImages/CustomFields.png" 
                                Height="25"
                                Text="Codes"  Tooltip="Codes"
                                AlternateText="Codes" ImageAlign="AbsBottom" />
                        </td>                    
                    </tr>
                    <tr><td colspan="2"><hr /></td></tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Second Custom Field Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Label2" runat="server" Text='<%# Eval("Label2") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>                                        
                    <tr>
                        <td width="250px" align="right" valign="middle">Enable Field: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:CheckBox ID="Use2" runat="server" Checked='<%# (bool)Eval("Use2") %>' ReadOnly="False"></asp:CheckBox>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Field Dropdown Values: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            
                            <asp:DropDownList ID="DDValues2" runat="server" DataSourceID="odsDDCodeTypes" DataTextField="CodeTypeName" DataValueField="CTID" 
                                AppendDataBoundItems="True" Width="200px"
                             >
                                <asp:ListItem Value="" Text="No Dropdown/Free Form"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="DDValues2Lbl" runat="server" Text='<%# Eval("DDValues2") %>' Visible="False"></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="ImageButton1" runat="server" 
                                CausesValidation="false" 
                                CommandName="codes" 
                                ImageUrl="~/ControlRoom/RibbonImages/CustomFields.png" 
                                Height="25"
                                Text="Codes"  Tooltip="Codes"
                                AlternateText="Codes" ImageAlign="AbsBottom" />
                        </td>                    
                    </tr>
                    <tr><td colspan="2"><hr /></td></tr>

        
                    <tr>
                        <td width="250px" align="right" valign="middle">Third Custom Field Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Label3" runat="server" Text='<%# Eval("Label3") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>                                        
                    <tr>
                        <td width="250px" align="right" valign="middle">Enable Field: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:CheckBox ID="Use3" runat="server" Checked='<%# (bool)Eval("Use3") %>' ReadOnly="False"></asp:CheckBox>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Field Dropdown Values: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            
                            <asp:DropDownList ID="DDValues3" runat="server" DataSourceID="odsDDCodeTypes" DataTextField="CodeTypeName" DataValueField="CTID" 
                                AppendDataBoundItems="True" Width="200px"
                             >
                                <asp:ListItem Value="" Text="No Dropdown/Free Form"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="DDValues3Lbl" runat="server" Text='<%# Eval("DDValues3") %>' Visible="False"></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="ImageButton2" runat="server" 
                                CausesValidation="false" 
                                CommandName="codes" 
                                ImageUrl="~/ControlRoom/RibbonImages/CustomFields.png" 
                                Height="25"
                                Text="Codes"  Tooltip="Codes"
                                AlternateText="Codes" ImageAlign="AbsBottom" />
                        </td>                    
                    </tr>

                    <tr><td colspan="2"><hr /></td></tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Fourth Custom Field Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Label4" runat="server" Text='<%# Eval("Label4") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>                                        
                    <tr>
                        <td width="250px" align="right" valign="middle">Enable Field: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:CheckBox ID="Use4" runat="server" Checked='<%# (bool)Eval("Use4") %>' ReadOnly="False"></asp:CheckBox>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Field Dropdown Values: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            
                            <asp:DropDownList ID="DDValues4" runat="server" DataSourceID="odsDDCodeTypes" DataTextField="CodeTypeName" DataValueField="CTID" 
                                AppendDataBoundItems="True" Width="200px"
                             >
                                <asp:ListItem Value="" Text="No Dropdown/Free Form"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="DDValues4Lbl" runat="server" Text='<%# Eval("DDValues4") %>' Visible="False"></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="ImageButton3" runat="server" 
                                CausesValidation="false" 
                                CommandName="codes" 
                                ImageUrl="~/ControlRoom/RibbonImages/CustomFields.png" 
                                Height="25"
                                Text="Codes"  Tooltip="Codes"
                                AlternateText="Codes" ImageAlign="AbsBottom" />
                        </td>                    
                    </tr>
                    <tr><td colspan="2"><hr /></td></tr>

        
                    <tr>
                        <td width="250px" align="right" valign="middle">Fifth Custom Field Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Label5" runat="server" Text='<%# Eval("Label5") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>                                        
                    <tr>
                        <td width="250px" align="right" valign="middle">Enable Field: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:CheckBox ID="Use5" runat="server" Checked='<%# (bool)Eval("Use5") %>' ReadOnly="False"></asp:CheckBox>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Field Dropdown Values: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            
                            <asp:DropDownList ID="DDValues5" runat="server" DataSourceID="odsDDCodeTypes" DataTextField="CodeTypeName" DataValueField="CTID" 
                                AppendDataBoundItems="True" Width="200px"
                             >
                                <asp:ListItem Value="" Text="No Dropdown/Free Form"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="DDValues5Lbl" runat="server" Text='<%# Eval("DDValues5") %>' Visible="False"></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="ImageButton4" runat="server" 
                                CausesValidation="false" 
                                CommandName="codes" 
                                ImageUrl="~/ControlRoom/RibbonImages/CustomFields.png" 
                                Height="25"
                                Text="Codes"  Tooltip="Codes"
                                AlternateText="Codes" ImageAlign="AbsBottom" />
                        </td>                    
                    </tr>
                    <tr><td colspan="2"><hr /></td></tr>
                </table>
                <!--
                <table style="font-weight: bold;">
                    <tr>
                        <td width="250px" align="right" valign="middle">Modified Date: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <%# Eval("LastModDate") %>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Modified By: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <%# Eval("LastModUser")%>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Added Date: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <%# Eval("AddedDate")%>
                        </td>                    
                    </tr>
                    <tr>
                        <td width="250px" align="right" valign="middle">Added By: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <%# Eval("AddedUser")%>
                        </td>                    
                    </tr>                                                                           
                    </tr>
                 </table>
               -->
            </EditItemTemplate>
              
        </asp:TemplateField>



            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel"    Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                        &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" 
                        CausesValidation="True" 
                        CommandName="Add" 
                        ImageUrl="~/ControlRoom/Images/add.png" 
                        Height="25"
                        Text="Add"   Tooltip="Add"
                        AlternateText="Add" /> 

                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"    Tooltip="Refresh Record"
                        AlternateText="Refresh Record" /> 
                        &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server" 
                        CausesValidation="True" 
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png" 
                        Height="25"
                        Text="Save"   Tooltip="Save"
                        AlternateText="Save"/>  
                        &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Text="Save and return"   Tooltip="Save and return"
                        AlternateText="Save and return" />    
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="1" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="GRA.SRP.DAL.CustomRegistrationFields">
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDCodeTypes" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.CodeType">
    </asp:ObjectDataSource>

</asp:Content>
