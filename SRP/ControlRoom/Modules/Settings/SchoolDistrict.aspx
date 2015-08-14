<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="SchoolDistrict.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.SchoolDistrict" 
    
%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


    <asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.SchoolCrosswalk">

    </asp:ObjectDataSource>

    <asp:GridView ID="gv" runat="server" AllowSorting="False" 
        AutoGenerateColumns="False" AllowPaging="True" PageSize="100"
        DataKeys="AID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" onpageindexchanging="gv_PageIndexChanging" onrowdatabound="gv_RowDataBound"       
        >

        <Columns> 
            
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    
                </HeaderTemplate>                
                <ItemTemplate>
                    <%# Eval("Rank") %>.&nbsp;                    
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>   


            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    School
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Label ID="ID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                    <asp:TextBox ID="SchoolID" runat="server" Text='<%# ((int) Eval("SchoolID") ==0 ? "" : Eval("SchoolID")) %>' Visible="false"></asp:TextBox>
                    <%# Eval("SchoolName") %>                    
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>                   

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    School Type
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:DropDownList ID="SchTypeID" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='True' 
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="SchTypeIDTxt" runat="server" Text='<%# ((int) Eval("SchTypeID") ==0 ? "" : Eval("SchTypeID")) %>' Visible="false"></asp:TextBox>               
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>   

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    District
                </HeaderTemplate>                
                <ItemTemplate>
                <asp:DropDownList ID="DistrictID" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='True' 
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="DistrictIDTxt" runat="server" Text='<%# ((int) Eval("DistrictID") ==0 ? "" : Eval("DistrictID")) %>' Visible="false"></asp:TextBox>            
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>   

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    City
                </HeaderTemplate>                
                <ItemTemplate>
                 <asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' Visible="True" Width="200px"></asp:TextBox>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>  

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    Min Grade
                </HeaderTemplate>                
                <ItemTemplate>
                <asp:TextBox ID="MinGrade" runat="server" Text='<%# ((int) Eval("MinGrade") ==0 ? "" : Eval("MinGrade")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="RegularExpressionValidator1"
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
                <ItemStyle VerticalAlign="Top" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    Max Grade
                </HeaderTemplate>                
                <ItemTemplate>
                <asp:TextBox ID="MaxGrade" runat="server" Text='<%# ((int) Eval("MaxGrade") ==0 ? "" : Eval("MaxGrade")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="revMaxGrade"
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
                <ItemStyle VerticalAlign="Top" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    Min Age
                </HeaderTemplate>                
                <ItemTemplate>
                <asp:TextBox ID="MinAge" runat="server" Text='<%# ((int) Eval("MinAge") ==0 ? "" : Eval("MinAge")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="RegularExpressionValidator2"
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
                    MaximumValue="999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Min Age must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'><br> * Min Age must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                  
                 </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    Max Age
                </HeaderTemplate>                
                <ItemTemplate>
                <asp:TextBox ID="MaxAge" runat="server" Text='<%# ((int) Eval("MaxAge") ==0 ? "" : Eval("MaxAge")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="RegularExpressionValidator3"
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
                    MaximumValue="999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Max Age must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'><br> * Max Age must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                   
                 </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="true" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    
            </div>
        </EmptyDataTemplate>
    </asp:GridView>

    <br /><br />

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
        AlternateText="Refresh Record" onclick="btnRefresh_Click" /> 
        &nbsp;
    <asp:ImageButton ID="btnSave" runat="server" 
        CausesValidation="True" 
        CommandName="Save"
        ImageUrl="~/ControlRoom/Images/save.png" 
        Height="25"
        Text="Save"   Tooltip="Save"
        AlternateText="Save" onclick="btnSave_Click"/>  
        &nbsp;
    <asp:ImageButton ID="btnSaveback" runat="server" 
        CausesValidation="True" 
        CommandName="Saveandback" 
        ImageUrl="~/ControlRoom/Images/saveback.png" 
        Height="25"
        Text="Save and return"   Tooltip="Save and return"
        AlternateText="Save and return" onclick="btnSaveback_Click" />    
                

   <asp:ObjectDataSource ID="odsDDDistrict" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>              

    
   <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource> 

   <asp:ObjectDataSource ID="odsDDSchoolType" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School Type" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource> 

</asp:Content>




    