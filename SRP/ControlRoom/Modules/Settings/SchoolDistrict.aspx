<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="SchoolDistrict.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.SchoolDistrict" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />

    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.SchoolCrosswalk"></asp:ObjectDataSource>

    <asp:GridView ID="gv" runat="server" AllowSorting="False"
        AutoGenerateColumns="False" AllowPaging="True" PageSize="100"
        DataKeys="AID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated" OnPageIndexChanging="gv_PageIndexChanging" OnRowDataBound="gv_RowDataBound">

        <Columns>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <%# Eval("Rank") %>.&nbsp;                    
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>


            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    School
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="ID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                    <asp:TextBox ID="SchoolID" runat="server" Text='<%# ((int) Eval("SchoolID") ==0 ? "" : Eval("SchoolID")) %>' Visible="false"></asp:TextBox>
                    <asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' Visible="False" Width="200px"></asp:TextBox>
                    <%# Eval("SchoolName") %>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    School Type
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList ID="SchTypeID" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Code" DataValueField="CID"
                        AppendDataBoundItems="True" CssClass="form-control"
                        Enabled='True'>
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SchTypeIDTxt" runat="server" Text='<%# ((int) Eval("SchTypeID") ==0 ? "" : Eval("SchTypeID")) %>' Visible="false"></asp:TextBox>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    District
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList ID="DistrictID" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID"
                        AppendDataBoundItems="True" CssClass="form-control"
                        Enabled='True'>
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="DistrictIDTxt" runat="server" Text='<%# ((int) Eval("DistrictID") ==0 ? "" : Eval("DistrictID")) %>' Visible="false"></asp:TextBox>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    Min Grade
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:TextBox ID="MinGrade" runat="server" Text='<%# ((int) Eval("MinGrade") ==0 ? "" : Eval("MinGrade")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right form-control"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                        ControlToValidate="MinGrade"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Grade must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Min Grade must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="RangeValidator1"
                        ControlToValidate="MinGrade"
                        MinimumValue="0"
                        MaximumValue="12"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Grade must be from 0 to 99!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Min Grade must be from 0 to 99! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    Max Grade
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:TextBox ID="MaxGrade" runat="server" Text='<%# ((int) Eval("MaxGrade") ==0 ? "" : Eval("MaxGrade")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right form-control"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="revMaxGrade"
                        ControlToValidate="MaxGrade"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Grade must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Max Grade must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvMaxGrade"
                        ControlToValidate="MaxGrade"
                        MinimumValue="0"
                        MaximumValue="12"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Grade must be from 0 to 99!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Max Grade must be from 0 to 99! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    Min Age
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:TextBox ID="MinAge" runat="server" Text='<%# ((int) Eval("MinAge") ==0 ? "" : Eval("MinAge")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right form-control"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2"
                        ControlToValidate="MinAge"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Age must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Min Age must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="RangeValidator2"
                        ControlToValidate="MinAge"
                        MinimumValue="0"
                        MaximumValue="200"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Age must be from 0 to 200!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Min Age must be from 0 to 200! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    Max Age
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:TextBox ID="MaxAge" runat="server" Text='<%# ((int) Eval("MaxAge") ==0 ? "" : Eval("MaxAge")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right form-control"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3"
                        ControlToValidate="MaxAge"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Age must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Max Age must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="RangeValidator3"
                        ControlToValidate="MaxAge"
                        MinimumValue="0"
                        MaximumValue="200"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Age must be from 0 to 200!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'><br> * Max Age must be from 0 to 200! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight: bold;">
                No records were found. &nbsp; 
                    
            </div>
        </EmptyDataTemplate>
    </asp:GridView>

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

    <asp:ObjectDataSource ID="odsDDDistrict" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="School District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>


    <asp:ObjectDataSource ID="odsDDSchool" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchoolType" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="School Type" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>




