<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="Codes.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.Codes" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ObjectDataSource ID="odsCT" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.CodeType"></asp:ObjectDataSource>

    <asp:Panel ID="pnlSelect" runat="server" Visible="True" CssClass="form-horizontal" Width="100%">
        <table width="100%">
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCodeTypes" runat="server" DataSourceID="odsCT"
                        DataTextField="CodeTypeName" DataValueField="CTID"
                        AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="ddlCodeTypes_SelectedIndexChanged"
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList></td>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnSelect" runat="server" Text="Load Codes" CausesValidation="false" CssClass="btn btn-default" Visible="false" />
                    <asp:Button ID="btnEdit" runat="server" Text="Edit Code Type" Visible="False"
                        CausesValidation="false" OnClick="btnEdit_Click" CssClass="btn btn-default" />
                    <asp:Button ID="btnAdd1" runat="server" Text="Add Code Type" Visible="True"
                        CausesValidation="false" OnClick="btnAdd1_Click" CssClass="btn btn-default" /></td>
            </tr>

        </table>

    </asp:Panel>
    <asp:Panel ID="pnlEdit" runat="server" Visible="False">
        <hr />
        <div style="text-align: left;">
            <table>
                <tr>
                    <td colspan="3" style="font-weight: bold; padding-left: 50px;" class="PageTitle">
                        <asp:Label ID="lblAE" runat="server"></asp:Label>
                        Code Type </td>
                </tr>
                <tr>
                    <td width="100px" align="center" valign="bottom">
                        <asp:ImageButton ID="btnBack" runat="server"
                            CausesValidation="false"
                            CommandName="Cancel"
                            ImageUrl="~/ControlRoom/Images/back.png"
                            Width="22px"
                            Text="Back/Cancel" ToolTip="Back/Cancel"
                            AlternateText="Back/Cancel" OnClick="btnBack_Click" />
                        &nbsp;
                        &nbsp;

                    <asp:ImageButton ID="btnCTSave" runat="server" AlternateText="Save Record" ToolTip="Save Record"
                        CausesValidation="True" CommandName="SaveRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/save.png" Width="20px" OnClick="btnCTSave_Click" />
                        &nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="btnCTDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("CTID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px"
                        OnClientClick="return confirm('Are you sure you want to delete this record?');"
                        OnClick="btnCTDelete_Click" />
                        <asp:Label ID="CTID" runat="server" Text="" Visible="False"></asp:Label>
                    </td>
                    <td width="250px">&nbsp;<b>Code Type Value:</b><br />
                        <asp:TextBox ID="CodeTypeValue" runat="server" Text='' MaxLength="255" Width="400px" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCodeTypeValue" runat="server"
                            ControlToValidate="CodeTypeValue" Display="Dynamic" ErrorMessage="Code Type is required"
                            SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                    </td>
                    <td width="450px">&nbsp;<b>Code Type Description:</b><br />
                        <asp:TextBox ID="CodeTypeDesc" runat="server" Text='' MaxLength="255" Width="400px" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCodeTypeDesc" runat="server"
                            ControlToValidate="CodeTypeDesc" Display="Dynamic" ErrorMessage="Description is required"
                            SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font>
                        </asp:RequiredFieldValidator>
                        <script>
                            $().ready(function () {
                                $('#<%=CodeTypeValue.ClientID%>').on('blur', function () {
                                    if ($('#<%=CodeTypeDesc.ClientID%>').val() == '') {
                                        $('#<%=CodeTypeDesc.ClientID%>').val($('#<%=CodeTypeValue.ClientID%>').val())
                                    }
                                });
                            });
                        </script>
                    </td>
                </tr>
            </table>
            <hr />
    </asp:Panel>

    <asp:Panel ID="pnlCodes" runat="server" Visible="False" DefaultButton="btnAdd">
        <hr />
        <div style="text-align: left;">
            <table>
                <tr>
                    <td colspan="3" style="font-weight: bold; padding-left: 60px;" class="PageTitle">Add New Code For This Code Type </td>
                </tr>
                <tr>
                    <td width="50px" align="center" valign="bottom">
                        <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                            CausesValidation="True" CommandName="AddRecord" CommandArgument="-1" ValidationGroup="Add"
                            ImageUrl="~/ControlRoom/Images/add.png" Width="20px"
                            OnClick="btnAdd_Click" />
                    </td>
                    <td width="250px">&nbsp;<b>Code Value:</b><br />
                        <asp:TextBox ID="CodeAdd" runat="server" Text='' MaxLength="255" Width="400px" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCodeAdd" runat="server"
                            ControlToValidate="CodeAdd" Display="Dynamic" ErrorMessage="Code is required"
                            SetFocusOnError="True" Font-Bold="True"
                            ValidationGroup="Add"><font color='red'> * Required </font>
                        </asp:RequiredFieldValidator>
                    </td>
                    <td width="450px">&nbsp;<b>Code Description:</b><br />
                        <asp:TextBox ID="DescriptionAdd" runat="server" Text='' MaxLength="255" Width="400px" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvDescriptionAdd" runat="server"
                            ControlToValidate="DescriptionAdd" Display="Dynamic" ErrorMessage="Description is required"
                            SetFocusOnError="True" Font-Bold="True"
                            ValidationGroup="Add"><font color='red'> * Required </font>
                        </asp:RequiredFieldValidator>
                        <script>
                            $().ready(function () {
                                $('#<%=CodeAdd.ClientID%>').on('blur', function () {
                                    if ($('#<%=DescriptionAdd.ClientID%>').val() == '') {
                                        $('#<%=DescriptionAdd.ClientID%>').val($('#<%=CodeAdd.ClientID%>').val())
                                    }
                                });
                                $('#<%=CodeAdd.ClientID%>').focus();
                            });
                        </script>
                    </td>
                </tr>
            </table>
            <hr />
            <table style="">
                <tr>
                    <td colspan="3" style="font-weight: bold; padding-left: 60px;" class="PageTitle">Existing Codes For This Code Type </td>
                </tr>
                <tr>
                    <td width="50px" align="center" valign="bottom">
                        <asp:ImageButton ID="btnCodeSave" runat="server" AlternateText="Save All Records" ToolTip="Save All Records"
                            CausesValidation="True" ValidationGroup="Save"
                            ImageUrl="~/ControlRoom/Images/Save.png" Width="20px"
                            OnClick="btnCodeSave_Click" />
                    </td>
                    <td width="250px">&nbsp;<b>Code Value:</b><br />
                    </td>
                    <td width="450px">&nbsp;<b>Code Description:</b><br />
                    </td>
                </tr>
                <asp:Repeater ID="rptrCodes" runat="server" OnItemCommand="rptrCodes_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td width="50px" align="center" valign="bottom">


                                <asp:ImageButton ID="btnCTDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                                    CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("CID") %>'
                                    ImageUrl="~/ControlRoom/Images/delete.png" Width="20px"
                                    OnClientClick="return confirm('Are you sure you want to delete this record?');" />

                                <asp:TextBox ID="CID" runat="server" Text='<%# Eval("CID") %>' Visible="False"></asp:TextBox>
                                <asp:TextBox ID="CTID" runat="server" Text='<%# Eval("CTID") %>' Visible="False"></asp:TextBox>

                            </td>
                            <td width="250px">
                                <asp:TextBox ID="Code" runat="server" Text='<%# Eval("Code") %>' MaxLength="255" Width="400px" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCode" runat="server"
                                    ControlToValidate="Code" Display="Dynamic" ErrorMessage="Code is required"
                                    SetFocusOnError="True" Font-Bold="True" ValidationGroup="Edit"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                            <td width="450px">
                                <asp:TextBox ID="Description" runat="server" Text='<%# Eval("Description") %>' MaxLength="255" Width="400px" CssClass="form-control"></asp:TextBox>
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
