<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilyAccountCtl.ascx.cs" Inherits="GRA.SRP.Controls.FamilyAccountCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<script src="/Scripts/jquery.ddslick.js" type="text/javascript"></script>
<div class="container">
    <div class="form-wrapper form-wide">
        <h3 class="title-divider">
            <asp:Label ID="Label1" runat="server" Text="myaccount-family-title"></asp:Label>
        </h3>
        <hr>
        <asp:Label ID="SA" runat="server" Text="" Visible="False"></asp:Label>
        <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000"></asp:Label>

        <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
            BorderColor="Black" BorderStyle="None" BorderWidth="1px"
            Font-Bold="True"
            HeaderText="Please correct the following errors: " ForeColor="Red" />

        <table style="font-weight: bold;">

            <asp:Repeater ID="rptr" runat="server"
                OnItemCommand="rptr_ItemCommand" OnItemDataBound="rptr_ItemDataBound">
                <ItemTemplate>

                    <tr>
                        <td>Username:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <%# Eval("Username") %>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("FirstName_Show")? "normal" : "none") %>'>
                        <td>First Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="FirstName" runat="server" Text='<%# Eval("FirstName") %>' Enabled='<%# (bool)Eval("FirstName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                                ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("MiddleName_Show")? "normal" : "none") %>'>
                        <td>Middle Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="MiddleName" runat="server" Text='<%# Eval("MiddleName") %>' Enabled='<%# (bool)Eval("MiddleName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvMiddleName" runat="server" Enabled='<%# Eval("MiddleName_Req") %>'
                                ControlToValidate="MiddleName" Display="Dynamic" ErrorMessage="Middle Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("LastName_Show")? "normal" : "none") %>'>
                        <td>Last Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="LastName" runat="server" Text='<%# Eval("LastName") %>' Enabled='<%# (bool)Eval("LastName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" Enabled='<%# Eval("LastName_Req") %>'
                                ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />


                    <tr>
                        <td>Avatar:</td>
                        <td>
                            <select id="ddAvatar"></select>
                            <input id="AvatarID" class="avatar selected-avatar" runat="server" visible="true" type="text" style="display: none;" value="1" />
                            <script>
                                var ddData = <%# Avatar.GetJSONForSelection((int)Eval("AvatarID")) %>;
                                $('#ddAvatar').ddslick({
                                    data: ddData,
                                    background: "transparent",
                                    selectText: "Select an avatar",
                                    onSelected: function (data) {
                                        $('.selected-avatar').first().val(data.selectedData.value);
                                    }
                                });
                            </script>
                        </td>
                    </tr>

                    <tr style='display: <%# ((bool)Eval("SchoolGrade_Show")? "normal" : "none") %>'>
                        <td>School Grade:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="SchoolGrade" runat="server" Text='<%# Eval("SchoolGrade") %>'
                                Enabled='<%# (bool)Eval("SchoolGrade_Edit") %>'
                                AutoPostBack="true" OnTextChanged="SchoolGrade_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvSchoolGrade" runat="server" Enabled='<%# Eval("SchoolGrade_Req") %>'
                                ControlToValidate="SchoolGrade" Display="Dynamic" ErrorMessage="School Grade is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("DOB_Show")? "normal" : "none") %>'>
                        <td>Date of Birth:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="DOB" runat="server" Text='<%# (Eval("DOB").ToString() == "" ? "" : FormatHelper.ToNormalDate((DateTime)Eval("DOB"))) %>'
                                Width="100px" Enabled='<%# (bool)Eval("DOB_Edit") %>' CssClass="datepicker"></asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvDOB" runat="server"
                                ControlToValidate="DOB" Display="Dynamic" ErrorMessage="Date of Birth is required" Enabled='<%# Eval("DOB_Req") %>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="DOB"
                                ErrorMessage="Invalid Date of Birth format" Type="Date"
                                Operator="DataTypeCheck" Display="Dynamic" Text="* Invalid format" ForeColor="Red"></asp:CompareValidator>

                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Age_Show")? "normal" : "none") %>'>
                        <td>Age:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Age" runat="server" Text='<%# ((int) Eval("Age") ==0 ? "" : Eval("Age")) %>'
                                Enabled='<%# (bool)Eval("Age_Edit") %>' Width="50px" CssClass="align-right"
                                AutoPostBack="true" OnTextChanged="Age_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAge" runat="server" Enabled='<%# Eval("Age_Req") %>'
                                ControlToValidate="Age" Display="Dynamic" ErrorMessage="Age is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revAge"
                                ControlToValidate="Age"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Age must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True"
                                Text="<font color='red'> * Age must be numeric. </font>"
                                EnableTheming="True"
                                SetFocusOnError="True" />
                            <asp:RangeValidator ID="rvAge"
                                ControlToValidate="Age"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Age must be from 0 to 99!</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True"
                                Text="<font color='red'> * Age must be from 0 to 99! </font>"
                                EnableTheming="True"
                                SetFocusOnError="True" />
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Gender_Show")? "normal" : "none") %>'>
                        <td>Gender:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="GenderTxt" runat="server" Text='<%# Eval("Gender") %>' Enabled='<%# (bool)Eval("Gender_Edit") %>' Visible="False"></asp:TextBox>
                            <asp:DropDownList ID="Gender" runat="server"
                                Enabled='<%# (bool)Eval("Gender_Edit") %>'
                                AppendDataBoundItems="True">
                                <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvGender" runat="server" Enabled='<%# Eval("Gender_Req") %>'
                                ControlToValidate="Gender" Display="Dynamic" ErrorMessage="Gender is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                        </td>
                        <tr />


                    <tr style='display: <%# ((bool)Eval("EmailAddress_Show")? "normal" : "none") %>'>
                        <td>Email Address:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="EmailAddress" runat="server" Text='<%# Eval("EmailAddress") %>' Enabled='<%# (bool)Eval("EmailAddress_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server" Enabled='<%# Eval("EmailAddress_Req") %>'
                                ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email Address is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                            <asp:RegularExpressionValidator ID="revEM" runat="server" ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email Address is not valid"
                                SetFocusOnError="True" Font-Bold="True"
                                ValidationExpression="[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"><font color='red'> * Not valid </font></asp:RegularExpressionValidator>

                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("PhoneNumber_Show")? "normal" : "none") %>'>
                        <td>Phone Number:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="PhoneNumber" runat="server" Text='<%# Eval("PhoneNumber") %>' Enabled='<%# (bool)Eval("PhoneNumber_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" Enabled='<%# Eval("PhoneNumber_Req") %>'
                                ControlToValidate="PhoneNumber" Display="Dynamic" ErrorMessage="Phone Number is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="PhoneNumber" Display="Dynamic"
                                ErrorMessage="Phone Number is not valid"
                                SetFocusOnError="True" Font-Bold="True"
                                ValidationExpression="\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"><font color='red'> * Not valid </font></asp:RegularExpressionValidator>


                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("StreetAddress1_Show")? "normal" : "none") %>'>
                        <td>Street Address 1:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="StreetAddress1" runat="server" Text='<%# Eval("StreetAddress1") %>' Enabled='<%# (bool)Eval("StreetAddress1_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvStreetAddress1" runat="server" Enabled='<%# Eval("StreetAddress1_Req") %>'
                                ControlToValidate="StreetAddress1" Display="Dynamic" ErrorMessage="Street Address 1 is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("StreetAddress2_Show")? "normal" : "none") %>'>
                        <td>Street Address 2:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="StreetAddress2" runat="server" Text='<%# Eval("StreetAddress2") %>' Enabled='<%# (bool)Eval("StreetAddress2_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvStreetAddress2" runat="server" Enabled='<%# Eval("StreetAddress2_Req") %>'
                                ControlToValidate="StreetAddress2" Display="Dynamic" ErrorMessage="Street Address 2 is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("City_Show")? "normal" : "none") %>'>
                        <td>City:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' Enabled='<%# (bool)Eval("City_Edit") %>'
                                AutoPostBack="true" OnTextChanged="City_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" Enabled='<%# Eval("City_Req") %>'
                                ControlToValidate="City" Display="Dynamic" ErrorMessage="City is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("State_Show")? "normal" : "none") %>'>
                        <td>State:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="State" runat="server" Text='<%# Eval("State") %>' Enabled='<%# (bool)Eval("State_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvState" runat="server" Enabled='<%# Eval("State_Req") %>'
                                ControlToValidate="State" Display="Dynamic" ErrorMessage="State is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />


                    <tr style='display: <%# ((bool)Eval("ZipCode_Show")? "normal" : "none") %>'>
                        <td>Zip Code:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="ZipCode" runat="server" Text='<%# Eval("ZipCode") %>' Enabled='<%# (bool)Eval("ZipCode_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Enabled='<%# Eval("ZipCode_Req") %>'
                                ControlToValidate="ZipCode" Display="Dynamic" ErrorMessage="Zip Code is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="ZipCode" Display="Dynamic"
                                ErrorMessage="Zip Code is not valid"
                                SetFocusOnError="True" Font-Bold="True"
                                ValidationExpression="\d{5}-?(\d{4})?$"><font color='red'> * Not valid </font></asp:RegularExpressionValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Country_Show")? "normal" : "none") %>'>
                        <td>Country:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Country" runat="server" Text='<%# Eval("Country") %>' Enabled='<%# (bool)Eval("Country_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" Enabled='<%# Eval("Country_Req") %>'
                                ControlToValidate="Country" Display="Dynamic" ErrorMessage="Country is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("County_Show")? "normal" : "none") %>'>
                        <td>County:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="County" runat="server" Text='<%# Eval("County") %>' Enabled='<%# (bool)Eval("County_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCounty" runat="server" Enabled='<%# Eval("County_Req") %>'
                                ControlToValidate="County" Display="Dynamic" ErrorMessage="County is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("ParentGuardianFirstName_Show")? "normal" : "none") %>'>
                        <td>Parent or Guardian First Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="ParentGuardianFirstName" runat="server" Text='<%# Eval("ParentGuardianFirstName") %>' Enabled='<%# (bool)Eval("ParentGuardianFirstName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvParentGuardianFirstName" runat="server" Enabled='<%# Eval("ParentGuardianFirstName_Req") %>'
                                ControlToValidate="ParentGuardianFirstName" Display="Dynamic" ErrorMessage="Parent/Guardian First Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("ParentGuardianMiddleName_Show")? "normal" : "none") %>'>
                        <td>Parent or Guardian Middle Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="ParentGuardianMiddleName" runat="server" Text='<%# Eval("ParentGuardianMiddleName") %>' Enabled='<%# (bool)Eval("ParentGuardianMiddleName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvParentGuardianMiddleName" runat="server" Enabled='<%# Eval("ParentGuardianMiddleName_Req") %>'
                                ControlToValidate="ParentGuardianMiddleName" Display="Dynamic" ErrorMessage="Parent/Guardian Middle Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("ParentGuardianLastName_Show")? "normal" : "none") %>'>
                        <td>Parent or Guardian Last Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="ParentGuardianLastName" runat="server" Text='<%# Eval("ParentGuardianLastName") %>' Enabled='<%# (bool)Eval("ParentGuardianLastName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvParentGuardianLastName" runat="server" Enabled='<%# Eval("ParentGuardianLastName_Req") %>'
                                ControlToValidate="ParentGuardianLastName" Display="Dynamic" ErrorMessage="Parent/Guardian Last Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("District_Show")? "normal" : "none") %>'>
                        <td>
                            <asp:Label ID="Label26" runat="server" Text="registration-form-school-district"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("District_Edit") %>'
                                OnSelectedIndexChanged="District_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="DistrictTxt" runat="server" Text='<%# (FormatHelper.SafeToInt(Eval("District").ToString()) == 0 ? "" : Eval("District") ) %>'
                                Enabled='<%# (bool)Eval("District_Edit") %>' Width="50px" CssClass="align-right"
                                Visible="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Enabled='<%# Eval("District_Req") %>'
                                ControlToValidate="District" Display="Dynamic" ErrorMessage="Library District is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator6" runat="server" Enabled='<%# Eval("District_Req") %>'
                                ControlToValidate="District" Display="Dynamic" ErrorMessage="Library District is required"
                                SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>
                        </td>
                        <tr />


                    <tr style='display: <%# ((bool)Eval("PrimaryLibrary_Show")? "normal" : "none") %>'>
                        <td>
                            <asp:Label ID="Label22" runat="server" Text="registration-form-library"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("PrimaryLibrary_Edit") %>'>
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="PrimaryLibraryTxt" runat="server" Text='<%# ((int) Eval("PrimaryLibrary") ==0 ? "" : Eval("PrimaryLibrary")) %>'
                                Enabled='<%# (bool)Eval("PrimaryLibrary_Edit") %>' Width="50px" CssClass="align-right"
                                Visible="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPrimaryLibrary" runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                                ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary Library is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator5" runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                                ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary Library  is required"
                                SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("LibraryCard_Show")? "normal" : "none") %>'>
                        <td>
                            <asp:Label ID="Label23" runat="server" Text="registration-form-library-card"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="LibraryCard" runat="server" Text='<%# Eval("LibraryCard") %>' Enabled='<%# (bool)Eval("LibraryCard_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvLibraryCard" runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                                ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library Card # is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("SchoolType_Show")? "normal" : "none") %>'>
                        <td>
                            <asp:Label ID="Label28" runat="server" Text="registration-form-school-type"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Code" DataValueField="CID"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("SchoolType_Edit") %>' CssClass="align-right"
                                OnSelectedIndexChanged="SchoolType_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="SchoolTypeTxt" runat="server" Text='<%# ((int) Eval("SchoolType") ==0 ? "" : Eval("SchoolType")) %>'
                                Enabled='<%# (bool)Eval("SchoolType_Edit") %>' Width="50px" CssClass="align-right" Visible="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvSchoolType" runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                                ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School Type is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator4" runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                                ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School Type is required"
                                SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>

                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("SDistrict_Show")? "normal" : "none") %>'>
                        <td>School District:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSDistrict" DataTextField="Code" DataValueField="CID"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("SDistrict_Edit") %>'
                                OnSelectedIndexChanged="SDistrict_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="SDistrictTxt" runat="server" Text='<%# ((int) Eval("SDistrict") == 0 ? "" : Eval("SDistrict")) %>'
                                Enabled='<%# (bool)Eval("SDistrict_Edit") %>' Width="50px" CssClass="align-right"
                                Visible="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                                ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator3" runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                                ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                                SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>

                        </td>
                        <tr />


                    <tr style='display: <%# ((bool)Eval("SchoolName_Show")? "normal" : "none") %>'>
                        <td>
                            <asp:Label ID="Label24" runat="server" Text="registration-form-school"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Code" DataValueField="CID"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("SchoolName_Edit") %>'>
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="SchoolNameTxt" runat="server" Text='<%# (FormatHelper.SafeToInt(Eval("SchoolName").ToString()) == 0 ? "" : Eval("SchoolName") ) %>'
                                Enabled='<%# (bool)Eval("SchoolName_Edit") %>' Width="50px" CssClass="align-right"
                                Visible="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvSchoolName" runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                                ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                                ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School Name is required"
                                SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>


                        </td>
                        <tr />


                    <tr style='display: <%# ((bool)Eval("Teacher_Show")? "normal" : "none") %>'>
                        <td>Teacher:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Teacher" runat="server" Text='<%# Eval("Teacher") %>' Enabled='<%# (bool)Eval("Teacher_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTeacher" runat="server" Enabled='<%# Eval("Teacher_Req") %>'
                                ControlToValidate="Teacher" Display="Dynamic" ErrorMessage="Teacher is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("GroupTeamName_Show")? "normal" : "none") %>'>
                        <td>Group/Team Name:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="GroupTeamName" runat="server" Text='<%# Eval("GroupTeamName") %>' Enabled='<%# (bool)Eval("GroupTeamName_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvGroupTeamName" runat="server" Enabled='<%# Eval("GroupTeamName_Req") %>'
                                ControlToValidate="GroupTeamName" Display="Dynamic" ErrorMessage="Group/Team Name is required"
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />


                    <tr style='display: <%# ((bool)Eval("LiteracyLevel1_Show")? "normal" : "none") %>'>
                        <td><%# Eval("Literacy1Label")%>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="LiteracyLevel1" runat="server" Text='<%# ((int) Eval("LiteracyLevel1") ==0 ? "" : Eval("LiteracyLevel1")) %>'
                                Enabled='<%# (bool)Eval("LiteracyLevel1_Edit") %>' Width="50px" CssClass="align-right"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvLiteracyLevel1" runat="server" Enabled='<%# Eval("LiteracyLevel1_Req") %>'
                                ControlToValidate="LiteracyLevel1" Display="Dynamic" ErrorMessage='<%# Eval("Literacy1Label")%> + " is required"'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revLiteracyLevel1"
                                ControlToValidate="LiteracyLevel1"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage='<%# String.Format("<font color=\"red\">{0} must be numeric.</font>", Eval("Literacy1Label")) %>'
                                runat="server"
                                Font-Bold="True" Font-Italic="True"
                                Text='<%# String.Format("<font color=\"red\"> * {0} must be numeric.</font>", Eval("Literacy1Label")) %>'
                                EnableTheming="True"
                                SetFocusOnError="True" />
                            <asp:RangeValidator ID="rvLiteracyLevel1"
                                ControlToValidate="LiteracyLevel1"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage='<%# String.Format("<font color=\"red\">{0}must be from 0 to 99!</font>", Eval("Literacy1Label")) %>'
                                runat="server"
                                Font-Bold="True" Font-Italic="True"
                                Text='<%# String.Format("<font color=\"red\"> * {0} must be from 0 to 99!</font>", Eval("Literacy1Label")) %>'
                                EnableTheming="True"
                                SetFocusOnError="True" />
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("LiteracyLevel2_Show")? "normal" : "none") %>'>
                        <td><%# Eval("Literacy1Label")%>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="LiteracyLevel2" runat="server" Text='<%# ((int) Eval("LiteracyLevel2") ==0 ? "" : Eval("LiteracyLevel2")) %>'
                                Enabled='<%# (bool)Eval("LiteracyLevel2_Edit") %>' Width="50px" CssClass="align-right"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvLiteracyLevel2" runat="server" Enabled='<%# Eval("LiteracyLevel2_Req") %>'
                                ControlToValidate="LiteracyLevel2" Display="Dynamic" ErrorMessage='<%# Eval("Literacy2Label")%> + " is required"'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revLiteracyLevel2"
                                ControlToValidate="LiteracyLevel2"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage='<%# String.Format("<font color=\"red\">{0} must be numeric.</font>", Eval("Literacy2Label")) %>'
                                runat="server"
                                Font-Bold="True" Font-Italic="True"
                                Text='<%# String.Format("<font color=\"red\"> * {0} must be numeric.</font>", Eval("Literacy2Label")) %>'
                                EnableTheming="True"
                                SetFocusOnError="True" />
                            <asp:RangeValidator ID="rvLiteracyLevel2"
                                ControlToValidate="LiteracyLevel2"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage='<%# String.Format("<font color=\"red\">{0}must be from 0 to 99!</font>", Eval("Literacy2Label")) %>'
                                runat="server"
                                Font-Bold="True" Font-Italic="True"
                                Text='<%# String.Format("<font color=\"red\"> * {0} must be from 0 to 99!</font>", Eval("Literacy2Label")) %>'
                                EnableTheming="True"
                                SetFocusOnError="True" />
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom1_Show") && CustomRegistrationFields.FetchObject().DDValues1 == "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label1 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Custom1" runat="server" Text='<%# Eval("Custom1") %>' Enabled='<%# (bool)Eval("Custom1_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCustom1" runat="server" Enabled='<%# (bool) Eval("Custom1_Req") && CustomRegistrationFields.FetchObject().DDValues1 == "" %>'
                                ControlToValidate="Custom1" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label1 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom1_Show") && CustomRegistrationFields.FetchObject().DDValues1 != "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label1 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="Custom1DD" runat="server" DataTextField="Code" DataValueField="Code"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("Custom1_Edit") %>' CssClass="align-right">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="Custom1DDTXT" runat="server" Text='<%# Eval("Custom1") %>' Enabled='<%# (bool)Eval("Custom1_Edit") %>' Visible="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Enabled='<%# (bool)Eval("Custom1_Req") && CustomRegistrationFields.FetchObject().DDValues1 != ""  %>'
                                ControlToValidate="Custom1DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label1 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom2_Show") && CustomRegistrationFields.FetchObject().DDValues2 == "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label2 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Custom2" runat="server" Text='<%# Eval("Custom2") %>' Enabled='<%# (bool)Eval("Custom2_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCustom2" runat="server" Enabled='<%# (bool) Eval("Custom2_Req") && CustomRegistrationFields.FetchObject().DDValues2 == "" %>'
                                ControlToValidate="Custom2" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label2 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom2_Show") && CustomRegistrationFields.FetchObject().DDValues2 != "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label2 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="Custom2DD" runat="server" DataTextField="Code" DataValueField="Code"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("Custom2_Edit") %>' CssClass="align-right">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="Custom2DDTXT" runat="server" Text='<%# Eval("Custom2") %>' Enabled='<%# (bool)Eval("Custom2_Edit") %>' Visible="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Enabled='<%# (bool)Eval("Custom2_Req") && CustomRegistrationFields.FetchObject().DDValues2 != ""  %>'
                                ControlToValidate="Custom2DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label2 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom3_Show") && CustomRegistrationFields.FetchObject().DDValues3 == "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label3 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Custom3" runat="server" Text='<%# Eval("Custom3") %>' Enabled='<%# (bool)Eval("Custom3_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCustom3" runat="server" Enabled='<%# (bool) Eval("Custom3_Req") && CustomRegistrationFields.FetchObject().DDValues3 == "" %>'
                                ControlToValidate="Custom3" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label3 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom3_Show") && CustomRegistrationFields.FetchObject().DDValues3 != "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label3 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="Custom3DD" runat="server" DataTextField="Code" DataValueField="Code"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("Custom3_Edit") %>' CssClass="align-right">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="Custom3DDTXT" runat="server" Text='<%# Eval("Custom3") %>' Enabled='<%# (bool)Eval("Custom3_Edit") %>' Visible="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Enabled='<%# (bool)Eval("Custom3_Req") && CustomRegistrationFields.FetchObject().DDValues3 != ""  %>'
                                ControlToValidate="Custom3DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label3 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom4_Show") && CustomRegistrationFields.FetchObject().DDValues4 == "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label4 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Custom4" runat="server" Text='<%# Eval("Custom4") %>' Enabled='<%# (bool)Eval("Custom4_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCustom4" runat="server" Enabled='<%# (bool) Eval("Custom4_Req") && CustomRegistrationFields.FetchObject().DDValues4 == "" %>'
                                ControlToValidate="Custom4" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label4 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom4_Show") && CustomRegistrationFields.FetchObject().DDValues4 != "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label4 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="Custom4DD" runat="server" DataTextField="Code" DataValueField="Code"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("Custom4_Edit") %>' CssClass="align-right">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="Custom4DDTXT" runat="server" Text='<%# Eval("Custom4") %>' Enabled='<%# (bool)Eval("Custom4_Edit") %>' Visible="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Enabled='<%# (bool)Eval("Custom4_Req") && CustomRegistrationFields.FetchObject().DDValues4 != ""  %>'
                                ControlToValidate="Custom4DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label4 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom5_Show") && CustomRegistrationFields.FetchObject().DDValues5 == "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label5 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:TextBox ID="Custom5" runat="server" Text='<%# Eval("Custom5") %>' Enabled='<%# (bool)Eval("Custom5_Edit") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCustom5" runat="server" Enabled='<%# (bool) Eval("Custom5_Req") && CustomRegistrationFields.FetchObject().DDValues5 == "" %>'
                                ControlToValidate="Custom5" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label5 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <tr style='display: <%# ((bool)Eval("Custom5_Show") && CustomRegistrationFields.FetchObject().DDValues5 != "" ? "normal" : "none") %>'>
                        <td><%# CustomRegistrationFields.FetchObject().Label5 %>:&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="Custom5DD" runat="server" DataTextField="Code" DataValueField="Code"
                                AppendDataBoundItems="True"
                                Enabled='<%# (bool)Eval("Custom5_Edit") %>' CssClass="align-right">
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="Custom5DDTXT" runat="server" Text='<%# Eval("Custom5") %>' Enabled='<%# (bool)Eval("Custom5_Edit") %>' Visible="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Enabled='<%# (bool)Eval("Custom5_Req") && CustomRegistrationFields.FetchObject().DDValues5 != ""  %>'
                                ControlToValidate="Custom5DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label5 + " is required"%>'
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <tr />

                    <!--
        <tr  style='display: <%# ((bool)Eval("FirstName_Show")? "normal" : "none") %>'>
            <td>Username:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                
            </td>
        <tr/>
        -->
                    <tr>
                        <td colspan="2">
                            <hr>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:Button ID="btnSave" runat="server" Text="myaccount-save" CommandName="save" CausesValidation="True" CssClass="btn a" />
                            &nbsp;
                <asp:Button ID="Button1" runat="server" Text="MyAccount CancelBtn" CommandName="cancel" CausesValidation="false" CssClass="btn a" />
                        </td>
                        <tr />

                </ItemTemplate>
            </asp:Repeater>


        </table>

        <asp:TextBox ID="city" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="district" runat="server" Visible="false" Text="0"></asp:TextBox>
        <asp:TextBox ID="schType" runat="server" Visible="false" Text="0"></asp:TextBox>
        <asp:TextBox ID="sdistrict" runat="server" Visible="false" Text="0"></asp:TextBox>
        <asp:TextBox ID="grade" runat="server" Visible="false" Text="0"></asp:TextBox>
        <asp:TextBox ID="age" runat="server" Visible="false" Text="0"></asp:TextBox>

        <asp:ObjectDataSource ID="odsDDSchoolType" runat="server"
            SelectMethod="GetAlByTypeName"
            TypeName="GRA.SRP.DAL.Codes">
            <SelectParameters>
                <asp:Parameter Name="Name" DefaultValue="School Type" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <asp:ObjectDataSource ID="odsDDPrograms" runat="server"
            SelectMethod="GetAllActive"
            TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>

        <asp:ObjectDataSource ID="odsDDDistrict" runat="server"
            SelectMethod="GetFilteredDistrictDDValues"
            TypeName="GRA.SRP.DAL.LibraryCrosswalk">
            <SelectParameters>
                <asp:ControlParameter ControlID="city" DefaultValue="" Name="city"
                    PropertyName="Text" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>


        <asp:ObjectDataSource ID="odsDDBranch" runat="server"
            SelectMethod="GetFilteredBranchDDValues"
            TypeName="GRA.SRP.DAL.LibraryCrosswalk">
            <SelectParameters>
                <asp:ControlParameter ControlID="district" DefaultValue="0" Name="districtID"
                    PropertyName="Text" Type="Int32" />
                <asp:ControlParameter ControlID="city" DefaultValue="" Name="city"
                    PropertyName="Text" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>


        <asp:ObjectDataSource ID="odsDDSDistrict" runat="server"
            SelectMethod="GetAlByTypeName"
            TypeName="GRA.SRP.DAL.Codes">
            <SelectParameters>
                <asp:Parameter Name="Name" DefaultValue="School District" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <asp:ObjectDataSource ID="odsDDSchool" runat="server"
            SelectMethod="GetFilteredSchoolDDValues"
            TypeName="GRA.SRP.DAL.SchoolCrosswalk">
            <SelectParameters>
                <asp:ControlParameter ControlID="schType" DefaultValue="0" Name="schTypeID"
                    PropertyName="Text" Type="Int32" />
                <asp:ControlParameter ControlID="sdistrict" DefaultValue="0" Name="districtID"
                    PropertyName="Text" Type="Int32" />
                <asp:ControlParameter ControlID="city" DefaultValue="" Name="city"
                    PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="grade" DefaultValue="0" Name="grade"
                    PropertyName="Text" Type="Int32" />
                <asp:ControlParameter ControlID="age" DefaultValue="0" Name="age"
                    PropertyName="Text" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>

    </div>

</div>



<script language="javascript" type="text/javascript">
    $(function () {
        $(".datepicker").datepicker();
    });
</script>
