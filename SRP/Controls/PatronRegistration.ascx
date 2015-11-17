<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronRegistration.ascx.cs" Inherits="GRA.SRP.Controls.PatronRegistration" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<script>
    function ClientValidate(source, arguments) {
        arguments.IsValid = ($(".pwd").val() == $(".pwd2").val());
    }
    function ParentPermFlagValidation(source, arguments) {
        arguments.IsValid = $(".gra-parent-perm-container input:checkbox").is(':checked');
    }
    function TermsOfUseflagValidation(source, arguments) {
        arguments.IsValid = $(".gra-terms-of-use-container input:checkbox").is(':checked');
    }
</script>
<asp:Label ID="Step" runat="server" Text="1" Visible="False"></asp:Label>
<asp:Label ID="RegistrationAge" runat="server" Text="0" Visible="False"></asp:Label>
<asp:Label ID="RegisteringFamily" runat="server" Text="0" Visible="False"></asp:Label>
<asp:Label ID="MasterPID" runat="server" Text="" Visible="False"></asp:Label>
<asp:TextBox ID="city" runat="server" Visible="false"></asp:TextBox>
<asp:TextBox ID="district" runat="server" Visible="false" Text="0"></asp:TextBox>
<asp:TextBox ID="schType" runat="server" Visible="false" Text="0"></asp:TextBox>
<asp:TextBox ID="sdistrict" runat="server" Visible="false" Text="0"></asp:TextBox>
<asp:TextBox ID="grade" runat="server" Visible="false" Text="0"></asp:TextBox>
<asp:TextBox ID="age" runat="server" Visible="false" Text="0"></asp:TextBox>


<asp:Panel runat="server" class="panel panel-default" DefaultButton="btnNext">
    <div class="panel-heading">
        <asp:Label runat="server" CssClass="lead" Text="registration-title"></asp:Label>
    </div>
    <div class="panel-body">
        <div class="form-horizontal">
            <div class="row">
                <div class="col-xs-12">
                    <p>
                        <asp:Label ID="Label2" runat="server" Text="registration-instructions"></asp:Label>
                    </p>
                </div>

                <div class="col-xs-12 col-sm-8 col-sm-offset-2">
                    <asp:Label ID="lblError" runat="server" Text="" CssClass="text-danger"></asp:Label>
                </div>

                <div class="col-xs-12 col-sm-8 col-sm-offset-2 margin-1em-top margin-1em-bottom">
                    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
                        HeaderText='<span class="glyphicon glyphicon-exclamation-sign margin-halfem-right"></span>Please correct the following errors:' />
                </div>
            </div>
            <div class="row">
                <asp:Panel ID="Panel0" runat="server" Visible="False" CssClass="col-xs-12">
                    <asp:Label ID="Label3" runat="server" CssClass="lead" Text="registraton-create-family-accounts"></asp:Label>
                </asp:Panel>
            </div>
            <asp:Repeater ID="rptr" runat="server" OnItemDataBound="rptr_ItemDataBound">
                <ItemTemplate>
                    <asp:Panel ID="Panel1" runat="server" Visible="True">
                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolGrade_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-grade"></asp:Label>
                            </label>

                            <div class="col-sm-6">
                                <asp:TextBox ID="SchoolGrade" runat="server" CssClass="form-control"
                                    OnTextChanged="SchoolGrade_TextChanged"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvSchoolGrade" runat="server" Enabled='<%# Eval("SchoolGrade_Req") %>'
                                    ControlToValidate="SchoolGrade" Display="Dynamic" ErrorMessage="School grade is required."
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>


                        <div class="form-group" runat="server" visible='<%# (bool)Eval("DOB_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-dob"></asp:Label>
                            </label>

                            <div class="col-sm-6">
                                <asp:TextBox ID="DOB" runat="server" CssClass="form-control datepicker"
                                    Enabled='<%# (bool)Eval("DOB_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvDOB" runat="server"
                                    ControlToValidate="DOB" Display="Dynamic" ErrorMessage="Date of Birth is required." Enabled='<%# Eval("DOB_Req") %>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="DOB"
                                    ErrorMessage="Date of birth must be a date" Type="Date"
                                    Operator="DataTypeCheck" Display="Dynamic" Text="* please enter a date"></asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Age_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-age"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Age" runat="server" CssClass="form-control" MaxLength="2"
                                    Enabled='<%# (bool)Eval("Age_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvAge" runat="server" Enabled='<%# Eval("Age_Req") %>'
                                    ControlToValidate="Age" Display="Dynamic" ErrorMessage="Age is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revAge"
                                    ControlToValidate="Age"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="Age must be a number."
                                    runat="server"
                                    Text="* must be a number."
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel2" runat="server" Visible="False">
                        <div class="form-group">
                            <label class="col-sm-6 control-label">
                                <asp:Label runat="server" Text="registration-form-family-account"></asp:Label>
                            </label>
                            <div class="col-sm-3">
                                <asp:DropDownList ID="FamilyAccount" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                    <asp:ListItem Value="No" Text="No" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel3" runat="server" Visible="False">
                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-program"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" DataTextField="TabName" DataValueField="PID"
                                    AppendDataBoundItems="True" CssClass="form-control">
                                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvProgram" runat="server" Enabled='true'
                                    ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" Enabled='true'
                                    ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("FirstName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-name-first"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="FirstName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                                    ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("MiddleName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-name-middle"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="MiddleName" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("MiddleName_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvMiddleName" runat="server" Enabled='<%# Eval("MiddleName_Req") %>'
                                    ControlToValidate="MiddleName" Display="Dynamic" ErrorMessage="Middle name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LastName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-name-last"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LastName" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("LastName_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" Enabled='<%# Eval("LastName_Req") %>'
                                    ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Gender_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-gender"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="Gender" runat="server" CssClass="form-control"
                                    Enabled='<%# (bool)Eval("Gender_Prompt") %>'
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                    <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                    <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                    <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvGender" runat="server" Enabled='<%# Eval("Gender_Req") %>'
                                    ControlToValidate="Gender" Display="Dynamic" ErrorMessage="Gender is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("EmailAddress_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-email"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="EmailAddress" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("EmailAddress_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server" Enabled='<%# Eval("EmailAddress_Req") %>'
                                    ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revEM" runat="server" ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is not valid"
                                    SetFocusOnError="True"
                                    ValidationExpression="[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?">* invalid</asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("PhoneNumber_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-phone"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="PhoneNumber" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("PhoneNumber_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" Enabled='<%# Eval("PhoneNumber_Req") %>'
                                    ControlToValidate="PhoneNumber" Display="Dynamic" ErrorMessage="Phone number is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="PhoneNumber" Display="Dynamic"
                                    ErrorMessage="Phone number is not valid"
                                    SetFocusOnError="True"
                                    ValidationExpression="\(?\d{3}\)?-? *\d{3}-? *-?\d{4}">* invalid</asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress1_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-address1"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="StreetAddress1" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("StreetAddress1_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvStreetAddress1" runat="server" Enabled='<%# Eval("StreetAddress1_Req") %>'
                                    ControlToValidate="StreetAddress1" Display="Dynamic" ErrorMessage="Street address is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress2_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-address2"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="StreetAddress2" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("StreetAddress2_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvStreetAddress2" runat="server" Enabled='<%# Eval("StreetAddress2_Req") %>'
                                    ControlToValidate="StreetAddress2" Display="Dynamic" ErrorMessage="Street address 2 is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("City_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-city"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="City" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("City_Prompt") %>' OnTextChanged="City_TextChanged"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCity" runat="server" Enabled='<%# Eval("City_Req") %>'
                                    ControlToValidate="City" Display="Dynamic" ErrorMessage="City is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("State_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-state"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="State" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("State_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvState" runat="server" Enabled='<%# Eval("State_Req") %>'
                                    ControlToValidate="State" Display="Dynamic" ErrorMessage="State is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ZipCode_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-zip"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ZipCode" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("ZipCode_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Enabled='<%# Eval("ZipCode_Req") %>'
                                    ControlToValidate="ZipCode" Display="Dynamic" ErrorMessage="ZIP code is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="ZipCode" Display="Dynamic"
                                    ErrorMessage="ZIP code is not valid"
                                    SetFocusOnError="True"
                                    ValidationExpression="\d{5}-?(\d{4})?$">* invalid</asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Country_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-country"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Country" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("Country_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCountry" runat="server" Enabled='<%# Eval("Country_Req") %>'
                                    ControlToValidate="Country" Display="Dynamic" ErrorMessage="Country is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("County_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-county"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="County" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("County_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCounty" runat="server" Enabled='<%# Eval("County_Req") %>'
                                    ControlToValidate="County" Display="Dynamic" ErrorMessage="County is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel4" runat="server" Visible="False">
                        <asp:TextBox ID="Panel4Visibility" runat="server"
                            Text='<%# ((bool)Eval("ParentGuardianFirstName_Prompt") || (bool)Eval("ParentGuardianMiddleName_Prompt") || (bool)Eval("ParentGuardianLastName_Prompt")  ? "1" : "0") %>'
                            Visible="false"></asp:TextBox>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianFirstName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-guardian-name-first"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ParentGuardianFirstName" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("ParentGuardianFirstName_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvParentGuardianFirstName" runat="server" Enabled='<%# Eval("ParentGuardianFirstName_Req") %>'
                                    ControlToValidate="ParentGuardianFirstName" Display="Dynamic" ErrorMessage="Parent/guardian first name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianMiddleName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-guardian-name-middle"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ParentGuardianMiddleName" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("ParentGuardianMiddleName_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvParentGuardianMiddleName" runat="server" Enabled='<%# Eval("ParentGuardianMiddleName_Req") %>'
                                    ControlToValidate="ParentGuardianMiddleName" Display="Dynamic" ErrorMessage="Parent/guardian middle name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianLastName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-guardian-name-last"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ParentGuardianLastName" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("ParentGuardianLastName_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvParentGuardianLastName" runat="server" Enabled='<%# Eval("ParentGuardianLastName_Req") %>'
                                    ControlToValidate="ParentGuardianLastName" Display="Dynamic" ErrorMessage="Parent/guardian last name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel5" runat="server" Visible="False">
                        <asp:TextBox ID="Panel5Visibility" runat="server"
                            Text='<%# ((bool)Eval("PrimaryLibrary_Prompt") || (bool)Eval("LibraryCard_Prompt") || (bool)Eval("SchoolName_Prompt") || (bool)Eval("District_Prompt")  || (bool)Eval("SDistrict_Prompt") || (bool)Eval("Teacher_Prompt") || (bool)Eval("GroupTeamName_Prompt") || (bool)Eval("SchoolType_Prompt") || (bool)Eval("LiteracyLevel1_Prompt") || (bool)Eval("LiteracyLevel2_Prompt")   ? "1" : "0") %>'
                            Visible="false"></asp:TextBox>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("District_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-district"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID"
                                    AppendDataBoundItems="True"
                                    AutoPostBack="true"
                                    CssClass="form-control"
                                    OnSelectedIndexChanged="District_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" Enabled='<%# Eval("District_Req") %>'
                                    ControlToValidate="District" Display="Dynamic" ErrorMessage="Library district is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator6" runat="server" Enabled='<%# Eval("District_Req") %>'
                                    ControlToValidate="District" Display="Dynamic" ErrorMessage="Library district is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("PrimaryLibrary_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-library"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID"
                                    AppendDataBoundItems="True" CssClass="form-control"
                                    Enabled='<%# (bool)Eval("PrimaryLibrary_Prompt") %>'>
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                                    ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary library is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                                    ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary library is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LibraryCard_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-library-card"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LibraryCard" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("LibraryCard_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvLibraryCard" runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                                    ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library card # is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolType_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-school-type"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Code" DataValueField="CID"
                                    AppendDataBoundItems="True"
                                    Enabled='<%# (bool)Eval("SchoolType_Prompt") %>' CssClass="form-control"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="SchoolType_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">

                                <asp:RequiredFieldValidator ID="rfvSchoolType" runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                                    ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School type is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator4" runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                                    ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School type is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SDistrict_Show")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-school-district"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSDistrict" DataTextField="Code" DataValueField="CID"
                                    AppendDataBoundItems="True"
                                    Enabled='<%# (bool)Eval("SDistrict_Edit") %>'
                                    AutoPostBack="true" CssClass="form-control"
                                    OnSelectedIndexChanged="SDistrict_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                                    ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator3" runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                                    ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-school"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Code" DataValueField="CID"
                                    AppendDataBoundItems="True" CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvSchoolName" runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                                    ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator5" runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                                    ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School name is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Teacher_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-teacher"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Teacher" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("Teacher_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvTeacher" runat="server" Enabled='<%# Eval("Teacher_Req") %>'
                                    ControlToValidate="Teacher" Display="Dynamic" ErrorMessage="Teacher is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("GroupTeamName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-group"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="GroupTeamName" runat="server" CssClass="form-control" Enabled='<%# (bool)Eval("GroupTeamName_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvGroupTeamName" runat="server" Enabled='<%# Eval("GroupTeamName_Req") %>'
                                    ControlToValidate="GroupTeamName" Display="Dynamic" ErrorMessage="Group/team name is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel1_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <%# Eval("Literacy1Label")%>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LiteracyLevel1" runat="server" CssClass="form-control"
                                    Enabled='<%# (bool)Eval("LiteracyLevel1_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvLiteracyLevel1" runat="server" Enabled='<%# Eval("LiteracyLevel1_Req") %>'
                                    ControlToValidate="LiteracyLevel1" Display="Dynamic" ErrorMessage='<%# Eval("Literacy1Label", "{0} is required")%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLiteracyLevel1"
                                    ControlToValidate="LiteracyLevel1"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy1Label", "{0} must be a number.") %>'
                                    runat="server"
                                    Text='* must be a number'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLiteracyLevel1"
                                    ControlToValidate="LiteracyLevel1"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy1Label", "{0} must be a number between 0 and 99.") %>'
                                    runat="server"
                                    Text='* must be between 0 and 99'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel2_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <%# Eval("Literacy2Label")%>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LiteracyLevel2" runat="server" CssClass="form-control"
                                    Enabled='<%# (bool)Eval("LiteracyLevel2_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvLiteracyLevel2" runat="server" Enabled='<%# Eval("LiteracyLevel2_Req") %>'
                                    ControlToValidate="LiteracyLevel2" Display="Dynamic" ErrorMessage='<%# Eval("Literacy2Label", "{0} is required")%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLiteracyLevel2"
                                    ControlToValidate="LiteracyLevel2"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy2Label", "{0} must be a number.") %>'
                                    runat="server"
                                    Text='* must be a number'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLiteracyLevel2"
                                    ControlToValidate="LiteracyLevel2"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy2Label", "{0} must be a number between 0 and 99.") %>'
                                    runat="server"
                                    Text='* must be between 0 and 99'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel6" runat="server" Visible="False">
                        <asp:TextBox ID="Panel6Visibility" runat="server"
                            Text='<%# ((bool)Eval("Custom1_Prompt") || (bool)Eval("Custom2_Prompt") || (bool)Eval("Custom3_Prompt")|| (bool)Eval("Custom4_Prompt") || (bool)Eval("Custom5_Prompt") || (bool)Eval("TermsOfUseflag_Prompt")  ? "1" : "0") %>'
                            Visible="false"></asp:TextBox>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom1_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# CustomRegistrationFields.FetchObject().Label1 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom1" runat="server" CssClass="form-control"
                                    Visible='<%#string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues1)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom1DD" runat="server" DataTextField="Code" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues1)%>'
                                    CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom1DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom1_Edit") %>' Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCustom1DD" runat="server"
                                    Enabled='<%# (bool)Eval("Custom1_Prompt") && !string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues1)%>'
                                    ControlToValidate="Custom1DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label1 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvCustom1" runat="server"
                                    Enabled='<%# (bool)Eval("Custom1_Prompt") && string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues1)%>'
                                    ControlToValidate="Custom1" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label1 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom2_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# CustomRegistrationFields.FetchObject().Label2 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom2" runat="server" CssClass="form-control"
                                    Visible='<%#string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues2)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom2DD" runat="server" DataTextField="Code" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues2)%>'
                                    CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom2DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom2_Edit") %>' Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCustom2DD" runat="server"
                                    Enabled='<%# (bool)Eval("Custom2_Prompt") && !string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues2)%>'
                                    ControlToValidate="Custom2DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label2 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvCustom2" runat="server"
                                    Enabled='<%# (bool)Eval("Custom2_Prompt") && string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues2)%>'
                                    ControlToValidate="Custom2" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label2 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom3_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# CustomRegistrationFields.FetchObject().Label3 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom3" runat="server" CssClass="form-control"
                                    Visible='<%#string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues3)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom3DD" runat="server" DataTextField="Code" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues3)%>'
                                    CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom3DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom3_Edit") %>' Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCustom3DD" runat="server"
                                    Enabled='<%# (bool)Eval("Custom3_Prompt") && !string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues3)%>'
                                    ControlToValidate="Custom3DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label3 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvCustom3" runat="server"
                                    Enabled='<%# (bool)Eval("Custom3_Prompt") && string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues3)%>'
                                    ControlToValidate="Custom3" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label3 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom4_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# CustomRegistrationFields.FetchObject().Label4 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom4" runat="server" CssClass="form-control"
                                    Visible='<%#string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues4)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom4DD" runat="server" DataTextField="Code" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues4)%>'
                                    CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom4DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom4_Edit") %>' Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCustom4DD" runat="server"
                                    Enabled='<%# (bool)Eval("Custom4_Prompt") && !string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues4)%>'
                                    ControlToValidate="Custom4DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label4 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvCustom4" runat="server"
                                    Enabled='<%# (bool)Eval("Custom4_Prompt") && string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues4)%>'
                                    ControlToValidate="Custom4" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label4 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>


                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom5_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# CustomRegistrationFields.FetchObject().Label5 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom5" runat="server" CssClass="form-control"
                                    Visible='<%#string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues5)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom5DD" runat="server" DataTextField="Code" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues5)%>'
                                    CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom5DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom5_Edit") %>' Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="rfvCustom5DD" runat="server"
                                    Enabled='<%# (bool)Eval("Custom5_Prompt") && !string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues5)%>'
                                    ControlToValidate="Custom5DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label5 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvCustom5" runat="server"
                                    Enabled='<%# (bool)Eval("Custom5_Prompt") && string.IsNullOrEmpty(CustomRegistrationFields.FetchObject().DDValues5)%>'
                                    ControlToValidate="Custom5" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label5 + " is required"%>'
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("TermsOfUseflag_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-terms"></asp:Label>
                            </label>
                            <label class="col-sm-9 form-control-static">
                                <div class="row">
                                    <asp:CheckBox ID="TermsOfUseflag" runat="server" ReadOnly="False" Checked="true" CssClass="col-xs-1 gra-registration-checkbox gra-terms-of-use-container"></asp:CheckBox>
                                    <asp:Label runat="server" Text="registraton-terms-agreement" CssClass="col-xs-10"></asp:Label>
                                </div>
                            </label>
                            <div class="col-sm-9 col-sm-offset-3">
                                <asp:CustomValidator ID="cvTermsOfUseflag"
                                    ClientValidationFunction="TermsOfUseflagValidation"
                                    EnableClientScript="true"
                                    runat="server" Enabled='<%# Eval("TermsOfUseflag_Prompt") %>'
                                    ErrorMessage="You must accept the terms of use." SetFocusOnError="True">* you must accept the terms</asp:CustomValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ShareFlag_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-information-sharing"></asp:Label>
                            </label>
                            <label class="col-sm-9 form-control-static">
                                <div class="row">
                                    <asp:CheckBox ID="ShareFlag" runat="server" ReadOnly="False" Checked="true" CssClass="col-xs-1 gra-registration-checkbox"></asp:CheckBox>
                                    <asp:Label runat="server" Text="registration-privacy-details" CssClass="col-xs-10"></asp:Label>
                                </div>
                            </label>
                        </div>

                        <asp:Panel ID="pnlConsent" runat="server" Visible="False">
                            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentPermFlag_Prompt") && int.Parse(RegistrationAge.Text.Length==0 ? "0" : RegistrationAge.Text) < 18 %>'>
                                <label class="col-sm-3 control-label">
                                    <asp:Label runat="server" Text="registration-form-consent"></asp:Label>
                                </label>
                                <label class="col-sm-9 form-control-static">
                                    <asp:CheckBox ID="ParentPermFlag" runat="server" ReadOnly="False" Checked="true" CssClass="col-xs-1 gra-registration-checkbox gra-parent-perm-container"></asp:CheckBox>
                                    <asp:Label ID="lblConsent" runat="server" CssClass="col-xs-10"></asp:Label>
                                </label>
                                <div class="col-sm-9 col-sm-offset-3">
                                    <asp:CustomValidator ID="cvParentPermFlag"
                                        ClientValidationFunction="ParentPermFlagValidation"
                                        EnableClientScript="true"
                                        runat="server" Enabled='<%# (bool)Eval("ParentPermFlag_Prompt") && int.Parse(RegistrationAge.Text.Length == 0 ? "0": RegistrationAge.Text) < 18 %>'
                                        ErrorMessage="You must have parental consent." SetFocusOnError="True">* must have parental consent</asp:CustomValidator>
                                </div>
                            </div>
                        </asp:Panel>
                    </asp:Panel>

                    <asp:Panel ID="Panel7" runat="server" Visible="False">
                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                Username:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Username" runat="server" CssClass="form-control" Enabled="true"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Enabled="true"
                                    ControlToValidate="Username" Display="Dynamic" ErrorMessage="Username is required"
                                    SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                                    runat="server" ControlToValidate="Username" ValidationExpression="^[a-zA-Z0-9_-]{5,25}$"
                                    Display="Dynamic"
                                    ErrorMessage="Username must be at least five characters long.">* username must be more than four characters long
                                </asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-3 control-label">Password:</label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Password" runat="server" CssClass="pwd form-control" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="NPasswordReq" runat="server" Display="Dynamic"
                                    ControlToValidate="Password" ErrorMessage="Password is required"
                                    ToolTip="Password required" SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="uxNewPasswordStrengthValidator"
                                    runat="server" ControlToValidate="Password" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                                    Display="Dynamic"
                                    ErrorMessage="Please select a password of at least seven characters with at least one number and at least one letter.">* password not secure
                                </asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-3 control-label">Verify password:</label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Password2" runat="server" CssClass="pwd2 form-control" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Display="Dynamic"
                                    ControlToValidate="Password2" ErrorMessage="Password validation is required"
                                    ToolTip="Password Re-entry required" SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="uxConfirmNewPasswordCustomValidator"
                                    runat="server" ControlToValidate="Password2"
                                    ErrorMessage="The password and validation do not match."
                                    ClientValidationFunction="ClientValidate">* password does not match</asp:CustomValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-avatar"></asp:Label></label>
                            <div class="col-sm-6">
                                <select id="ddAvatar" class="form-control"></select>
                                <input id="AvatarID" class="avatar selected-avatar" runat="server" visible="true" type="text" style="display: none;" value="1" />
                                <script>
                                    var ddData = [<%# Avatar.GetJSONForSelection(1) %>];
                                    $('#ddAvatar').ddslick({
                                        width: 230,
                                        background: "transparent",
                                        showSelectedHTML: true,
                                        selectText: "Select an avatar",
                                        data: ddData,
                                        onSelected: function (data) {
                                            $('.selected-avatar').val(data.selectedData.value);
                                        }
                                    });
                                    $('#ddAvatar').ddslick('select', { index: 1 });
                                </script>
                            </div>
                        </div>
                    </asp:Panel>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="Panel8" runat="server" Visible="False" CssClass="col-xs-12">
                <asp:Label runat="server" Text="registraton-success"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="Panel9" runat="server" Visible="False" CssClass="col-xs-12">
                <asp:Label runat="server" Text="registration-success-family-account"></asp:Label>
            </asp:Panel>
        </div>
    </div>
    <div class="panel-footer clearfix">
        <div class="pull-right">
            <asp:Button ID="btnPrev" runat="server" Text="registraton-button-previous"
                CausesValidation="True" CssClass="btn btn-default" Enabled="False"
                OnClick="btnPrev_Click" />
            <asp:Button ID="btnNext" runat="server" Text="registraton-button-next"
                CausesValidation="True" CssClass="btn btn-success" OnClick="btnNext_Click" />

            <asp:Button ID="btnDone" runat="server" Text="registraton-button-done"
                CausesValidation="True" CssClass="btn btn-default" Visible="False" OnClick="btnDone_Click" />
        </div>
    </div>
</asp:Panel>

<asp:ObjectDataSource ID="odsDDSchoolType" runat="server"
    SelectMethod="GetAlByTypeName"
    TypeName="GRA.SRP.DAL.Codes">
    <SelectParameters>
        <asp:Parameter Name="Name" DefaultValue="School Type" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
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

<asp:ObjectDataSource ID="odsDDPrograms" runat="server"
    SelectMethod="GetAllActive"
    TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>

<script>
    $(function () {
        $(".datepicker").datepick({
            changeMonth: false,
            showOtherMonths: true,
            selectOtherMonths: true,
            showSpeed: 'fast'
        });
    });
</script>
