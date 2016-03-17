<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="LibraryDistrict.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.LibraryDistrict" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />
    <table>
        <tr>
            <td><b>District</b></td>
            <td><b>Branch</b></td>
            <td><b>Link</b></td>
            <td><b>Address</b></td>
            <td><b>Telephone</b></td>
        </tr>

        <asp:Repeater ID="rptrCW" runat="server" OnItemDataBound="rptrCW_ItemDataBound">
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:DropDownList ID="DistrictID" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID"
                            AppendDataBoundItems="True"
                            Enabled='True' CssClass="form-control">
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="DistrictIDTxt" runat="server" Text='<%# ((int) Eval("DistrictID") ==0 ? "" : Eval("DistrictID")) %>' Visible="false"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="ID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                        <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID"
                            AppendDataBoundItems="True"
                            Enabled='False'
                            CssClass="form-control">
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="BranchIDTxt" runat="server" Text='<%# ((int) Eval("BranchID") ==0 ? "" : Eval("BranchID")) %>' Visible="false"></asp:TextBox>
                        <asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' Visible="False" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="BranchLink" runat="server" Text='<%# Eval("BranchLink") %>' Width="200px" CssClass="form-control"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="BranchAddress" runat="server" Text='<%# Eval("BranchAddress") %>' Width="200px" CssClass="form-control"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="BranchTelephone" runat="server" Text='<%# Eval("BranchTelephone") %>' Width="200px" CssClass="form-control"></asp:TextBox>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

    <table style="margin-top: 2em; margin-bottom: 2em;">
        <tr>
            <td>
                <asp:ImageButton ID="btnBack" runat="server"
                    CausesValidation="false"
                    CommandName="Back"
                    ImageUrl="~/ControlRoom/Images/back.png"
                    Height="25"
                    Text="Back/Cancel" ToolTip="Back/Cancel"
                    AlternateText="Back/Cancel" OnClick="btnBack_Click" />
                &nbsp;
        &nbsp;
    <asp:ImageButton ID="btnRefresh" runat="server"
        CausesValidation="false"
        CommandName="Refresh"
        ImageUrl="~/ControlRoom/Images/refresh.png"
        Height="25"
        Text="Refresh Record" ToolTip="Refresh Record"
        AlternateText="Refresh Record" OnClick="btnRefresh_Click" />
                &nbsp;
    <asp:ImageButton ID="btnSave" runat="server"
        CausesValidation="True"
        CommandName="Save"
        ImageUrl="~/ControlRoom/Images/save.png"
        Height="25"
        Text="Save" ToolTip="Save"
        AlternateText="Save" OnClick="btnSave_Click" />
                &nbsp;
    <asp:ImageButton ID="btnSaveback" runat="server"
        CausesValidation="True"
        CommandName="Saveandback"
        ImageUrl="~/ControlRoom/Images/saveback.png"
        Height="25"
        Text="Save and return" ToolTip="Save and return"
        AlternateText="Save and return" OnClick="btnSaveback_Click" />
                &nbsp;
            </td>
            <td>Import from Excel: 
            </td>
            <td>
                <asp:FileUpload runat="server" ID="ExcelFileUpload" />
            </td>
            <td>
                <asp:LinkButton ID="UploadButton" ForeColor="White"
                    OnClick="UploadButton_Click" CssClass="btn btn-xs btn-info"
                    runat="server"><span class="glyphicon glyphicon-upload"></span> Upload Excel file
                </asp:LinkButton>
                <asp:LinkButton ID="DownloadButton" ForeColor="White"
                    OnClick="DownloadButton_Click" CssClass="btn btn-xs btn-success"
                    runat="server"><span class="glyphicon glyphicon-download"></span> Download Excel file
                </asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.LibraryCrosswalk"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDDistrict" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>
