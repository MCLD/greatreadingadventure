<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="RegistrationSettingsAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.RegistrationSettingsAddEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.DAL" %>


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
                        <td width="250px" align="right" valign="middle">First Literacy Measure Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Literacy1Label" runat="server" Text='<%# Eval("Literacy1Label") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>                    <tr>
                        <td width="250px" align="right" valign="middle">Second Literacy Measure Name: &nbsp;</td>
                        <td width="250px" align="left" valign="middle" colspan="4">
                            <asp:TextBox ID="Literacy2Label" runat="server" Text='<%# Eval("Literacy2Label") %>' ReadOnly="False"></asp:TextBox>
                        </td>                    
                    </tr>
                   </table>
                   <hr />
                   <table style="font-weight: bold;">
                   <tr style="font-size: 13px; color: Blue; border-bottom:1px solid blue; border-top:1px solid blue;">
                        <td width="250px" align="right" valign="middle">Registration Field &nbsp;</td>
                        <td width="100px" align="center" valign="middle">Ask for Field?</td>
                        <td width="100px" align="center" valign="middle">Required?</td>
                        <td width="100px" align="center" valign="middle">Show on Profile?</td>
                        <td width="100px" align="center" valign="middle">Allow Edit?</td>
                    </tr>


                 </table>
                 <div  style="font-weight: bold; height:350px; overflow:scroll; width:700px; border: 1px solid silver;" >
                 
                <table style="font-weight: bold; ">
                    <tr>
                        <td width="250px" align="right" valign="middle">DOB: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="DOB_Prompt" runat="server" Checked='<%# (bool)Eval("DOB_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="DOB_Req" runat="server" Checked='<%# (bool)Eval("DOB_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="DOB_Show" runat="server" Checked='<%# (bool)Eval("DOB_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="DOB_Edit" runat="server" Checked='<%# (bool)Eval("DOB_Edit") %>' ReadOnly="False"></asp:CheckBox></td>                    
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Age: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Age_Prompt" runat="server" Checked='<%# (bool)Eval("Age_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Age_Req" runat="server" Checked='<%# (bool)Eval("Age_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Age_Show" runat="server" Checked='<%# (bool)Eval("Age_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Age_Edit" runat="server" Checked='<%# (bool)Eval("Age_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">School Grade: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolGrade_Prompt" runat="server" Checked='<%# (bool)Eval("SchoolGrade_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolGrade_Req" runat="server" Checked='<%# (bool)Eval("SchoolGrade_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolGrade_Show" runat="server" Checked='<%# (bool)Eval("SchoolGrade_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolGrade_Edit" runat="server" Checked='<%# (bool)Eval("SchoolGrade_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">First Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="FirstName_Prompt" runat="server" Checked='<%# (bool)Eval("FirstName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="FirstName_Req" runat="server" Checked='<%# (bool)Eval("FirstName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="FirstName_Show" runat="server" Checked='<%# (bool)Eval("FirstName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="FirstName_Edit" runat="server" Checked='<%# (bool)Eval("FirstName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Middle Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="MiddleName_Prompt" runat="server" Checked='<%# (bool)Eval("MiddleName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="MiddleName_Req" runat="server" Checked='<%# (bool)Eval("MiddleName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="MiddleName_Show" runat="server" Checked='<%# (bool)Eval("MiddleName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="MiddleName_Edit" runat="server" Checked='<%# (bool)Eval("MiddleName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Last Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LastName_Prompt" runat="server" Checked='<%# (bool)Eval("LastName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LastName_Req" runat="server" Checked='<%# (bool)Eval("LastName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LastName_Show" runat="server" Checked='<%# (bool)Eval("LastName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LastName_Edit" runat="server" Checked='<%# (bool)Eval("LastName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Gender: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Gender_Prompt" runat="server" Checked='<%# (bool)Eval("Gender_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Gender_Req" runat="server" Checked='<%# (bool)Eval("Gender_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Gender_Show" runat="server" Checked='<%# (bool)Eval("Gender_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Gender_Edit" runat="server" Checked='<%# (bool)Eval("Gender_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Email Address: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="EmailAddress_Prompt" runat="server" Checked='<%# (bool)Eval("EmailAddress_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="EmailAddress_Req" runat="server" Checked='<%# (bool)Eval("EmailAddress_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="EmailAddress_Show" runat="server" Checked='<%# (bool)Eval("EmailAddress_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="EmailAddress_Edit" runat="server" Checked='<%# (bool)Eval("EmailAddress_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Phone Number: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PhoneNumber_Prompt" runat="server" Checked='<%# (bool)Eval("PhoneNumber_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PhoneNumber_Req" runat="server" Checked='<%# (bool)Eval("PhoneNumber_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PhoneNumber_Show" runat="server" Checked='<%# (bool)Eval("PhoneNumber_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PhoneNumber_Edit" runat="server" Checked='<%# (bool)Eval("PhoneNumber_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Street Address 1: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress1_Prompt" runat="server" Checked='<%# (bool)Eval("StreetAddress1_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress1_Req" runat="server" Checked='<%# (bool)Eval("StreetAddress1_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress1_Show" runat="server" Checked='<%# (bool)Eval("StreetAddress1_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress1_Edit" runat="server" Checked='<%# (bool)Eval("StreetAddress1_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Street Address 2: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress2_Prompt" runat="server" Checked='<%# (bool)Eval("StreetAddress2_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress2_Req" runat="server" Checked='<%# (bool)Eval("StreetAddress2_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress2_Show" runat="server" Checked='<%# (bool)Eval("StreetAddress2_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="StreetAddress2_Edit" runat="server" Checked='<%# (bool)Eval("StreetAddress2_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">City: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="City_Prompt" runat="server" Checked='<%# (bool)Eval("City_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="City_Req" runat="server" Checked='<%# (bool)Eval("City_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="City_Show" runat="server" Checked='<%# (bool)Eval("City_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="City_Edit" runat="server" Checked='<%# (bool)Eval("City_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">State: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="State_Prompt" runat="server" Checked='<%# (bool)Eval("State_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="State_Req" runat="server" Checked='<%# (bool)Eval("State_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="State_Show" runat="server" Checked='<%# (bool)Eval("State_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="State_Edit" runat="server" Checked='<%# (bool)Eval("State_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Zip Code: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ZipCode_Prompt" runat="server" Checked='<%# (bool)Eval("ZipCode_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ZipCode_Req" runat="server" Checked='<%# (bool)Eval("ZipCode_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ZipCode_Show" runat="server" Checked='<%# (bool)Eval("ZipCode_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ZipCode_Edit" runat="server" Checked='<%# (bool)Eval("ZipCode_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Country: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Country_Prompt" runat="server" Checked='<%# (bool)Eval("Country_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Country_Req" runat="server" Checked='<%# (bool)Eval("Country_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Country_Show" runat="server" Checked='<%# (bool)Eval("Country_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Country_Edit" runat="server" Checked='<%# (bool)Eval("Country_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">County: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="County_Prompt" runat="server" Checked='<%# (bool)Eval("County_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="County_Req" runat="server" Checked='<%# (bool)Eval("County_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="County_Show" runat="server" Checked='<%# (bool)Eval("County_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="County_Edit" runat="server" Checked='<%# (bool)Eval("County_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Parent/Guardian First Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianFirstName_Prompt" runat="server" Checked='<%# (bool)Eval("ParentGuardianFirstName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianFirstName_Req" runat="server" Checked='<%# (bool)Eval("ParentGuardianFirstName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianFirstName_Show" runat="server" Checked='<%# (bool)Eval("ParentGuardianFirstName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianFirstName_Edit" runat="server" Checked='<%# (bool)Eval("ParentGuardianFirstName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Parent/Guardian Last Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianLastName_Prompt" runat="server" Checked='<%# (bool)Eval("ParentGuardianLastName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianLastName_Req" runat="server" Checked='<%# (bool)Eval("ParentGuardianLastName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianLastName_Show" runat="server" Checked='<%# (bool)Eval("ParentGuardianLastName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianLastName_Edit" runat="server" Checked='<%# (bool)Eval("ParentGuardianLastName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Parent/Guardian Middle Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianMiddleName_Prompt" runat="server" Checked='<%# (bool)Eval("ParentGuardianMiddleName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianMiddleName_Req" runat="server" Checked='<%# (bool)Eval("ParentGuardianMiddleName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianMiddleName_Show" runat="server" Checked='<%# (bool)Eval("ParentGuardianMiddleName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentGuardianMiddleName_Edit" runat="server" Checked='<%# (bool)Eval("ParentGuardianMiddleName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Library District: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="District_Prompt" runat="server" Checked='<%# (bool)Eval("District_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="District_Req" runat="server" Checked='<%# (bool)Eval("District_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="District_Show" runat="server" Checked='<%# (bool)Eval("District_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="District_Edit" runat="server" Checked='<%# (bool)Eval("District_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Primary Library: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PrimaryLibrary_Prompt" runat="server" Checked='<%# (bool)Eval("PrimaryLibrary_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PrimaryLibrary_Req" runat="server" Checked='<%# (bool)Eval("PrimaryLibrary_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PrimaryLibrary_Show" runat="server" Checked='<%# (bool)Eval("PrimaryLibrary_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="PrimaryLibrary_Edit" runat="server" Checked='<%# (bool)Eval("PrimaryLibrary_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Library Card: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LibraryCard_Prompt" runat="server" Checked='<%# (bool)Eval("LibraryCard_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LibraryCard_Req" runat="server" Checked='<%# (bool)Eval("LibraryCard_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LibraryCard_Show" runat="server" Checked='<%# (bool)Eval("LibraryCard_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LibraryCard_Edit" runat="server" Checked='<%# (bool)Eval("LibraryCard_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">School Type: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolType_Prompt" runat="server" Checked='<%# (bool)Eval("SchoolType_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolType_Req" runat="server" Checked='<%# (bool)Eval("SchoolType_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolType_Show" runat="server" Checked='<%# (bool)Eval("SchoolType_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolType_Edit" runat="server" Checked='<%# (bool)Eval("SchoolType_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">School Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolName_Prompt" runat="server" Checked='<%# (bool)Eval("SchoolName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolName_Req" runat="server" Checked='<%# (bool)Eval("SchoolName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolName_Show" runat="server" Checked='<%# (bool)Eval("SchoolName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SchoolName_Edit" runat="server" Checked='<%# (bool)Eval("SchoolName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">School District: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SDistrict_Prompt" runat="server" Checked='<%# (bool)Eval("SDistrict_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SDistrict_Req" runat="server" Checked='<%# (bool)Eval("SDistrict_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SDistrict_Show" runat="server" Checked='<%# (bool)Eval("SDistrict_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="SDistrict_Edit" runat="server" Checked='<%# (bool)Eval("SDistrict_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Teacher: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Teacher_Prompt" runat="server" Checked='<%# (bool)Eval("Teacher_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Teacher_Req" runat="server" Checked='<%# (bool)Eval("Teacher_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Teacher_Show" runat="server" Checked='<%# (bool)Eval("Teacher_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Teacher_Edit" runat="server" Checked='<%# (bool)Eval("Teacher_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Group/Team Name: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="GroupTeamName_Prompt" runat="server" Checked='<%# (bool)Eval("GroupTeamName_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="GroupTeamName_Req" runat="server" Checked='<%# (bool)Eval("GroupTeamName_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="GroupTeamName_Show" runat="server" Checked='<%# (bool)Eval("GroupTeamName_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="GroupTeamName_Edit" runat="server" Checked='<%# (bool)Eval("GroupTeamName_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">First Literacy Level: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel1_Prompt" runat="server" Checked='<%# (bool)Eval("LiteracyLevel1_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel1_Req" runat="server" Checked='<%# (bool)Eval("LiteracyLevel1_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel1_Show" runat="server" Checked='<%# (bool)Eval("LiteracyLevel1_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel1_Edit" runat="server" Checked='<%# (bool)Eval("LiteracyLevel1_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Second Literacy Level: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel2_Prompt" runat="server" Checked='<%# (bool)Eval("LiteracyLevel2_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel2_Req" runat="server" Checked='<%# (bool)Eval("LiteracyLevel2_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel2_Show" runat="server" Checked='<%# (bool)Eval("LiteracyLevel2_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="LiteracyLevel2_Edit" runat="server" Checked='<%# (bool)Eval("LiteracyLevel2_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Parent Permission Flag: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentPermFlag_Prompt" runat="server" Checked='<%# (bool)Eval("ParentPermFlag_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentPermFlag_Req" runat="server" Checked='<%# (bool)Eval("ParentPermFlag_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentPermFlag_Show" runat="server" Checked='<%# (bool)Eval("ParentPermFlag_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ParentPermFlag_Edit" runat="server" Checked='<%# (bool)Eval("ParentPermFlag_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Over 18 Flag: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Over18Flag_Prompt" runat="server" Checked='<%# (bool)Eval("Over18Flag_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Over18Flag_Req" runat="server" Checked='<%# (bool)Eval("Over18Flag_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Over18Flag_Show" runat="server" Checked='<%# (bool)Eval("Over18Flag_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Over18Flag_Edit" runat="server" Checked='<%# (bool)Eval("Over18Flag_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Share Flag: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ShareFlag_Prompt" runat="server" Checked='<%# (bool)Eval("ShareFlag_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ShareFlag_Req" runat="server" Checked='<%# (bool)Eval("ShareFlag_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ShareFlag_Show" runat="server" Checked='<%# (bool)Eval("ShareFlag_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="ShareFlag_Edit" runat="server" Checked='<%# (bool)Eval("ShareFlag_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr>
                        <td width="250px" align="right" valign="middle">Terms Of Use Flag: &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="TermsOfUseflag_Prompt" runat="server" Checked='<%# (bool)Eval("TermsOfUseflag_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="TermsOfUseflag_Req" runat="server" Checked='<%# (bool)Eval("TermsOfUseflag_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="TermsOfUseflag_Show" runat="server" Checked='<%# (bool)Eval("TermsOfUseflag_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="TermsOfUseflag_Edit" runat="server" Checked='<%# (bool)Eval("TermsOfUseflag_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr style='<%# (CustomRegistrationFields.FetchObject().Use1 ? "" : "display: none;") %> ' >
                        <td width="250px" align="right" valign="middle"><%# CustomRegistrationFields.F1Label() %> &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom1_Prompt" runat="server" Checked='<%# (bool)Eval("Custom1_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom1_Req" runat="server" Checked='<%# (bool)Eval("Custom1_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom1_Show" runat="server" Checked='<%# (bool)Eval("Custom1_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom1_Edit" runat="server" Checked='<%# (bool)Eval("Custom1_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr style='<%# (CustomRegistrationFields.FetchObject().Use2 ? "" : "display: none;") %> '>
                        <td width="250px" align="right" valign="middle"><%# CustomRegistrationFields.F2Label() %> &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom2_Prompt" runat="server" Checked='<%# (bool)Eval("Custom2_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom2_Req" runat="server" Checked='<%# (bool)Eval("Custom2_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom2_Show" runat="server" Checked='<%# (bool)Eval("Custom2_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom2_Edit" runat="server" Checked='<%# (bool)Eval("Custom2_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr style='<%# (CustomRegistrationFields.FetchObject().Use3 ? "" : "display: none;") %> '>
                        <td width="250px" align="right" valign="middle"><%# CustomRegistrationFields.F3Label() %> &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom3_Prompt" runat="server" Checked='<%# (bool)Eval("Custom3_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom3_Req" runat="server" Checked='<%# (bool)Eval("Custom3_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom3_Show" runat="server" Checked='<%# (bool)Eval("Custom3_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom3_Edit" runat="server" Checked='<%# (bool)Eval("Custom3_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr style='<%# (CustomRegistrationFields.FetchObject().Use4 ? "" : "display: none;") %> '>
                        <td width="250px" align="right" valign="middle"><%# CustomRegistrationFields.F4Label() %> &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom4_Prompt" runat="server" Checked='<%# (bool)Eval("Custom4_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom4_Req" runat="server" Checked='<%# (bool)Eval("Custom4_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom4_Show" runat="server" Checked='<%# (bool)Eval("Custom4_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom4_Edit" runat="server" Checked='<%# (bool)Eval("Custom4_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>

                    <tr style='<%# (CustomRegistrationFields.FetchObject().Use5 ? "" : "display: none;") %> '>
                        <td width="250px" align="right" valign="middle"><%# CustomRegistrationFields.F5Label() %> &nbsp;</td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom5_Prompt" runat="server" Checked='<%# (bool)Eval("Custom5_Prompt") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom5_Req" runat="server" Checked='<%# (bool)Eval("Custom5_Req") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom5_Show" runat="server" Checked='<%# (bool)Eval("Custom5_Show") %>' ReadOnly="False"></asp:CheckBox></td>
                        <td width="100px" align="center" valign="middle"><asp:CheckBox ID="Custom5_Edit" runat="server" Checked='<%# (bool)Eval("Custom5_Edit") %>' ReadOnly="False"></asp:CheckBox></td>
                    </tr>
                
                </table>
                
                </div>
                <hr />
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

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.RegistrationSettings">

	</asp:ObjectDataSource>

</asp:Content>

