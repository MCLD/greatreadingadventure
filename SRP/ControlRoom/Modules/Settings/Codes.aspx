<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="Codes.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.Codes" 
    
%>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:ObjectDataSource ID="odsCT" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.CodeType">
	</asp:ObjectDataSource>

    <asp:Panel ID="pnlSelect" runat="server" Visible="True">
        <b>Code Types:</b>
        <asp:DropDownList ID="ddlCodeTypes" runat="server" DataSourceID="odsCT" 
            DataTextField="CodeTypeName" DataValueField="CTID" 
            AppendDataBoundItems="True" AutoPostBack="True" onselectedindexchanged="ddlCodeTypes_SelectedIndexChanged" 
            Width="50%" 
        >
            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
        </asp:DropDownList>
        &nbsp;&nbsp;
        <asp:Button ID="btnSelect" runat="server" Text="Load Codes"  CausesValidation="false"/> &nbsp;
        <asp:Button ID="btnEdit" runat="server" Text="Edit Code Type" Visible="False" 
            CausesValidation="false" onclick="btnEdit_Click"/> &nbsp;
        <asp:Button ID="btnAdd1" runat="server" Text="Add Code Type" Visible="True" 
            CausesValidation="false" onclick="btnAdd1_Click"/> &nbsp;
    </asp:Panel>
    <asp:Panel ID="pnlEdit" runat="server" Visible="False">
        <hr />
        <div style="text-align: left;">
        <table >
            <tr>
                <td colspan="3" style="font-weight: bold; padding-left: 50px; " Class="PageTitle" > <asp:Label id="lblAE" runat="server"></asp:Label> Code Type </td>
            </tr>
            <tr>
                <td width="100px" align="center" valign="bottom">
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Cancel" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Width="22px"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnBack_Click" />
                        &nbsp;
                        &nbsp;

                    <asp:ImageButton ID="btnCTSave" runat="server" AlternateText="Save Record" Tooltip="Save Record"
                        CausesValidation="True" CommandName="SaveRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/save.png" Width="20px" onclick="btnCTSave_Click" 
                         />
                    &nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="btnCTDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("CTID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" 
                        OnClientClick="return confirm('Are you sure you want to delete this record?');" 
                        onclick="btnCTDelete_Click"/>
                    <asp:Label ID="CTID" runat="server" Text="" Visible="False"></asp:Label>
                </td>
                <td width="250px">
                    &nbsp;<b>Code Type Value:</b><br />
                    <asp:TextBox ID="CodeTypeValue" runat="server" Text='' MaxLength="255" Width="400px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodeTypeValue" runat="server" 
                    ControlToValidate="CodeTypeValue" Display="Dynamic" ErrorMessage="Code Type is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </td>
                <td width="450px">
                    &nbsp;<b>Code Type Description:</b><br />
                   <asp:TextBox ID="CodeTypeDesc" runat="server" Text='' MaxLength="255"  Width="400px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodeTypeDesc" runat="server" 
                        ControlToValidate="CodeTypeDesc" Display="Dynamic" ErrorMessage="Description is required" 
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font>
                     </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
         <hr />
    </asp:Panel>

    <asp:Panel ID="pnlCodes" runat="server" Visible="False">
        <hr />
        <div style="text-align: left;">
        <table >
            <tr>
                <td colspan="3" style="font-weight: bold; padding-left:60px;"  Class="PageTitle" > Add New Code For This Code Type </td>
            </tr>
            <tr>
                <td width="50px" align="center" valign="bottom">
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="True" CommandName="AddRecord" CommandArgument="-1"  ValidationGroup="Add"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" 
                        onclick="btnAdd_Click" />
                </td>
                <td width="250px">
                    &nbsp;<b>Code Value:</b><br />
                    <asp:TextBox ID="CodeAdd" runat="server" Text='' MaxLength="255" Width="400px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodeAdd" runat="server" 
                    ControlToValidate="CodeAdd" Display="Dynamic" ErrorMessage="Code is required" 
                    SetFocusOnError="True" Font-Bold="True"
                    ValidationGroup="Add"
                    ><font color='red'> * Required </font>
                    </asp:RequiredFieldValidator>
                </td>
                <td width="450px">
                    &nbsp;<b>Code Description:</b><br />
                    <asp:TextBox ID="DescriptionAdd" runat="server" Text='' MaxLength="255"  Width="400px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescriptionAdd" runat="server" 
                        ControlToValidate="DescriptionAdd" Display="Dynamic" ErrorMessage="Description is required" 
                        SetFocusOnError="True" Font-Bold="True"
                        ValidationGroup="Add"
                    ><font color='red'> * Required </font>
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <hr />
        <table style="">
            <tr>
                <td colspan="3" style="font-weight: bold; padding-left:60px;"  Class="PageTitle" > Existing Codes For This Code Type </td>
            </tr>
            <tr>
                <td width="50px" align="center" valign="bottom">
                    <asp:ImageButton ID="btnCodeSave" runat="server" AlternateText="Save All Records" Tooltip="Save All Records"
                        CausesValidation="True"  ValidationGroup="Save"
                        ImageUrl="~/ControlRoom/Images/Save.png" Width="20px" 
                        onclick="btnCodeSave_Click" />
                </td>
                <td width="250px">
                    &nbsp;<b>Code Value:</b><br />                    
                </td>
                <td width="450px">
                    &nbsp;<b>Code Description:</b><br />
                </td>
            </tr>
            <asp:Repeater ID="rptrCodes" runat="server" onitemcommand="rptrCodes_ItemCommand">
                <ItemTemplate>
                    <tr>
                        <td width="50px" align="center" valign="bottom" >
                    

                            <asp:ImageButton ID="btnCTDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                                CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("CID") %>' 
                                ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" 
                                OnClientClick="return confirm('Are you sure you want to delete this record?');" 
                                />

                            <asp:TextBox ID="CID" runat="server" Text='<%# Eval("CID") %>' Visible="False"></asp:TextBox>
                            <asp:TextBox ID="CTID" runat="server" Text='<%# Eval("CTID") %>' Visible="False"></asp:TextBox>

                        </td>
                        <td width="250px">
                            <asp:TextBox ID="Code" runat="server" Text='<%# Eval("Code") %>'  MaxLength="255" Width="400px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCode" runat="server" 
                            ControlToValidate="Code" Display="Dynamic" ErrorMessage="Code is required" 
                            SetFocusOnError="True" Font-Bold="True" ValidationGroup="Edit"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <td width="450px" >
                            <asp:TextBox ID="Description" runat="server" Text='<%# Eval("Description") %>'  MaxLength="255"  Width="400px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" 
                                ControlToValidate="Description" Display="Dynamic" ErrorMessage="Description is required" 
                                SetFocusOnError="True" Font-Bold="True" ValidationGroup="Edit"><font color='red'> * Required </font>
                             </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
    </table>
    </div>
    </asp:Panel>

</asp:Content>