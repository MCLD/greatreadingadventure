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
<asp:TextBox ID="parentGuardianFirst" runat="server" Visible="false" Text=""></asp:TextBox>
<asp:TextBox ID="parentGuardianMiddle" runat="server" Visible="false" Text=""></asp:TextBox>
<asp:TextBox ID="parentGuardianLast" runat="server" Visible="false" Text=""></asp:TextBox>

<asp:Panel runat="server" class="panel panel-default" DefaultButton="btnNext">
    <div class="panel-heading">
        <asp:Label runat="server" CssClass="lead" Text="registration-title"></asp:Label>
    </div>
    <div class="panel-body">
        <div class="form-horizontal">
            <div class="row">
                <div class="col-xs-12 margin-1em-bottom">
                    <asp:Label runat="server" Text="registration-instructions"></asp:Label>
                </div>

                <div class="col-xs-12 col-sm-8 col-sm-offset-2">
                    <asp:ValidationSummary runat="server"
                        HeaderText='<span class="glyphicon glyphicon-exclamation-sign margin-halfem-right"></span>Please correct the following errors:' />
                </div>
            </div>
            <asp:Panel ID="Panel0" runat="server" Visible="False" CssClass="row">
                <div class="col-xs-12 col-sm-8 col-sm-offset-2 margin-1em-bottom">
                    <div class="alert alert-info">
                        <span class="glyphicon glyphicon-info-sign"></span>
                        <asp:Label ID="Label3" runat="server" Text="registration-create-family-accounts"></asp:Label>
                    </div>
                </div>
            </asp:Panel>
            <asp:Repeater ID="rptr" runat="server" OnItemDataBound="rptr_ItemDataBound">
                <ItemTemplate>
                    <asp:Panel ID="Panel1" runat="server" Visible="True">
                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolGrade_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-grade"></asp:Label>
                            </label>

                            <div class="col-sm-6">
                                <asp:TextBox ID="SchoolGrade" runat="server" CssClass="form-control required-asterisk" data-asterisk="SchoolGradeReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("SchoolGrade_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm SchoolGradeReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolGrade_Req") %>'
                                    ControlToValidate="SchoolGrade" Display="Dynamic" ErrorMessage="School grade is required."
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>


                        <div class="form-group" runat="server" visible='<%# (bool)Eval("DOB_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-dob"></asp:Label>
                            </label>

                            <div class="col-sm-6">
                                <asp:TextBox ID="DOB" runat="server" CssClass="form-control datepicker" data-asterisk="DOBReq"
                                    Enabled='<%# (bool)Eval("DOB_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("DOB_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm DOBReq"></span>
                                <asp:RequiredFieldValidator runat="server"
                                    ControlToValidate="DOB" Display="Dynamic" ErrorMessage="Date of Birth is required." Enabled='<%# Eval("DOB_Req") %>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" ControlToValidate="DOB"
                                    ErrorMessage="Date of birth must be a date" Type="Date"
                                    Operator="DataTypeCheck" Display="Dynamic" Text="please enter a date"></asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Age_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-age"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Age" runat="server" CssClass="form-control required-asterisk" data-asterisk="AgeReq" MaxLength="2"
                                    Enabled='<%# (bool)Eval("Age_Prompt") %>'></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Age_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm AgeReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Age_Req") %>'
                                    ControlToValidate="Age" Display="Static" ErrorMessage="Age is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator
                                    ControlToValidate="Age"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="Age must be a number."
                                    runat="server"
                                    Text="must be a number."
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel2" runat="server" Visible="False">
                        <div class="row">
                            <div class="col-xs-12 col-sm-8 col-sm-offset-2 margin-1em-bottom">
                                <div class="alert alert-info">
                                    <span class="glyphicon glyphicon-info-sign"></span>
                                    <asp:Label runat="server" Text="registration-form-family-details"></asp:Label>
                                </div>
                            </div>
                        </div>
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
                                    AppendDataBoundItems="True" CssClass="form-control required-asterisk-dropdown" data-asterisk="ProgIdReq">
                                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm ProgIdReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='true'
                                    ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='true'
                                    ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("FirstName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-name-first"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="FirstName" runat="server" CssClass="form-control required-asterisk" data-asterisk="FirstNameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("FirstName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm FirstNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                                    ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("MiddleName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-name-middle"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="MiddleName" runat="server" CssClass="form-control required-asterisk" data-asterisk="MiddleNameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("MiddleName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm MiddleNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("MiddleName_Req") %>'
                                    ControlToValidate="MiddleName" Display="Dynamic" ErrorMessage="Middle name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LastName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-name-last"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LastName" runat="server" CssClass="form-control required-asterisk" data-asterisk="LastNameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("LastName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm LastNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LastName_Req") %>'
                                    ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Gender_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-gender"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="Gender" runat="server" CssClass="form-control required-asterisk-dropdown" data-asterisk="GenderReq"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                    <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                    <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                    <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Gender_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm GenderReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Gender_Req") %>'
                                    ControlToValidate="Gender" Display="Dynamic" ErrorMessage="Gender is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("EmailAddress_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-email"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="EmailAddress" runat="server" CssClass="form-control required-asterisk" data-asterisk="EmailAddressReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("EmailAddress_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm EmailAddressReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("EmailAddress_Req") %>'
                                    ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is not valid"
                                    SetFocusOnError="True"
                                    ValidationExpression="[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?">invalid</asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("PhoneNumber_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-phone"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="PhoneNumber" runat="server" CssClass="form-control required-asterisk" data-asterisk="PhoneNumberReq" placeholder="602-555-1212"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("PhoneNumber_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm PhoneNumberReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("PhoneNumber_Req") %>'
                                    ControlToValidate="PhoneNumber" Display="Dynamic" ErrorMessage="Phone number is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="PhoneNumber" Display="Dynamic"
                                    ErrorMessage="Phone number is not valid"
                                    SetFocusOnError="True"
                                    ValidationExpression="\(?\d{3}\)?-? *\d{3}-? *-?\d{4}">invalid</asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress1_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-address1"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="StreetAddress1" runat="server" CssClass="form-control required-asterisk" data-asterisk="StreetAddress1Req"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("StreetAddress1_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm StreetAddress1Req"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("StreetAddress1_Req") %>'
                                    ControlToValidate="StreetAddress1" Display="Dynamic" ErrorMessage="Street address is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress2_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-address2"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="StreetAddress2" runat="server" CssClass="form-control required-asterisk" data-asterisk="StreetAddress2Req"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("StreetAddress2_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm StreetAddress2Req"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("StreetAddress2_Req") %>'
                                    ControlToValidate="StreetAddress2" Display="Dynamic" ErrorMessage="Street address 2 is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("City_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-city"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="City" runat="server" CssClass="form-control required-asterisk" data-asterisk="CityReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("City_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm CityReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("City_Req") %>'
                                    ControlToValidate="City" Display="Dynamic" ErrorMessage="City is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("State_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-state"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="State" runat="server" CssClass="form-control required-asterisk" data-asterisk="StateReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("State_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm StateReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("State_Req") %>'
                                    ControlToValidate="State" Display="Dynamic" ErrorMessage="State is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ZipCode_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-zip"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ZipCode" runat="server" CssClass="form-control required-asterisk" data-asterisk="ZipCodeReq" placeholder="85004 or 85004-1140"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("ZipCode_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm ZipCodeReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ZipCode_Req") %>'
                                    ControlToValidate="ZipCode" Display="Dynamic" ErrorMessage="ZIP code is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="ZipCode" Display="Dynamic"
                                    ErrorMessage="ZIP code is not valid"
                                    SetFocusOnError="True"
                                    ValidationExpression="\d{5}-?(\d{4})?$">invalid</asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Country_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-country"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Country" runat="server" CssClass="form-control required-asterisk" data-asterisk="CountryReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Country_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm CountryReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Country_Req") %>'
                                    ControlToValidate="Country" Display="Dynamic" ErrorMessage="Country is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("County_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-county"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="County" runat="server" CssClass="form-control required-asterisk" data-asterisk="CountyReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("County_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm CountyReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("County_Req") %>'
                                    ControlToValidate="County" Display="Dynamic" ErrorMessage="County is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
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
                                <asp:TextBox ID="ParentGuardianFirstName" runat="server" CssClass="form-control required-asterisk" data-asterisk="ParentGuardianFirstnameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("ParentGuardianFirstName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm ParentGuardianFirstnameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianFirstName_Req") %>'
                                    ControlToValidate="ParentGuardianFirstName" Display="Dynamic" ErrorMessage="Parent/guardian first name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianMiddleName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-guardian-name-middle"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ParentGuardianMiddleName" runat="server" CssClass="form-control required-asterisk" data-asterisk="ParentGuardianMiddleNameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("ParentGuardianMiddleName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm ParentGuardianMiddleNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianMiddleName_Req") %>'
                                    ControlToValidate="ParentGuardianMiddleName" Display="Dynamic" ErrorMessage="Parent/guardian middle name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianLastName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-guardian-name-last"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="ParentGuardianLastName" runat="server" CssClass="form-control required-asterisk" data-asterisk="ParentGuardianLastNameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("ParentGuardianLastName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm ParentGuardianLastNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianLastName_Req") %>'
                                    ControlToValidate="ParentGuardianLastName" Display="Dynamic" ErrorMessage="Parent/guardian last name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel5" runat="server" Visible="False">
                        <asp:TextBox ID="Panel5Visibility" runat="server"
                            Text='<%# ((bool)Eval("PrimaryLibrary_Prompt") || (bool)Eval("LibraryCard_Prompt") || (bool)Eval("SchoolName_Prompt") || (bool)Eval("District_Prompt")  || (bool)Eval("SDistrict_Prompt") || (bool)Eval("Teacher_Prompt") || (bool)Eval("GroupTeamName_Prompt") || (bool)Eval("SchoolType_Prompt") || (bool)Eval("LiteracyLevel1_Prompt") || (bool)Eval("LiteracyLevel2_Prompt")   ? "1" : "0") %>'
                            Visible="false"></asp:TextBox>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("District_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-library-district"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDDistrict" DataTextField="Description" DataValueField="CID"
                                    AppendDataBoundItems="True"
                                    AutoPostBack="true"
                                    CssClass="form-control required-asterisk-dropdown" data-asterisk="DistrictReq"
                                    OnSelectedIndexChanged="District_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("District_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm DistrictReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("District_Req") %>'
                                    ControlToValidate="District" Display="Dynamic" ErrorMessage="Library district is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='<%# Eval("District_Req") %>'
                                    ControlToValidate="District" Display="Dynamic" ErrorMessage="Library district is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("PrimaryLibrary_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-library"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Description" DataValueField="CID"
                                    AppendDataBoundItems="True" CssClass="form-control required-asterisk-dropdown" data-asterisk="PrimaryLibraryReq">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("PrimaryLibrary_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm PrimaryLibraryReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                                    ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary library is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                                    ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary library is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LibraryCard_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-library-card"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LibraryCard" runat="server" CssClass="form-control required-asterisk" data-asterisk="LibraryCardReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("LibraryCard_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm LibraryCardReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                                    ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library card # is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SDistrict_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-school-district"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSDistrict" DataTextField="Description" DataValueField="CID"
                                    AppendDataBoundItems="True"
                                    AutoPostBack="true" CssClass="form-control required-asterisk-dropdown" data-asterisk="SDistrictReq"
                                    OnSelectedIndexChanged="SDistrict_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("SDistrict_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm SDistrictReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                                    ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                                    ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolType_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-school-type"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Description" DataValueField="CID"
                                    AppendDataBoundItems="True" CssClass="form-control required-asterisk-dropdown" data-asterisk="SchoolTypeReq"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="SchoolType_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("SchoolType_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm SchoolTypeReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                                    ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School type is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                                    ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School type is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-school"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Description" DataValueField="CID"
                                    AppendDataBoundItems="True" CssClass="form-control required-asterisk-dropdown" data-asterisk="SchoolNameReq">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("SchoolName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm SchoolNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                                    ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CompareValidator runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                                    ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School name is required"
                                    SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">required</asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Teacher_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-teacher"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Teacher" runat="server" CssClass="form-control required-asterisk" data-asterisk="TeacherReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Teacher_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm TeacherReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Teacher_Req") %>'
                                    ControlToValidate="Teacher" Display="Dynamic" ErrorMessage="Teacher is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("GroupTeamName_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-group"></asp:Label>
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="GroupTeamName" runat="server" CssClass="form-control required-asterisk" data-asterisk="GroupTeamNameReq"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("GroupTeamName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm GroupTeamNameReq"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("GroupTeamName_Req") %>'
                                    ControlToValidate="GroupTeamName" Display="Dynamic" ErrorMessage="Group/team name is required"
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel1_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <%# Eval("Literacy1Label")%>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LiteracyLevel1" runat="server" CssClass="form-control required-asterisk" data-asterisk="LiteracyLevel1Req"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("LiteracyLevel1_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm LiteracyLevel1Req"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LiteracyLevel1_Req") %>'
                                    ControlToValidate="LiteracyLevel1" Display="Dynamic" ErrorMessage='<%# Eval("Literacy1Label", "{0} is required")%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator
                                    ControlToValidate="LiteracyLevel1"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy1Label", "{0} must be a number.") %>'
                                    runat="server"
                                    Text='must be a number'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator
                                    ControlToValidate="LiteracyLevel1"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy1Label", "{0} must be a number between 0 and 99.") %>'
                                    runat="server"
                                    Text='must be between 0 and 99'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel2_Prompt")%>'>
                            <label class="col-sm-3 control-label">
                                <%# Eval("Literacy2Label")%>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="LiteracyLevel2" runat="server" CssClass="form-control required-asterisk" data-asterisk="LiteracyLevel2Req"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("LiteracyLevel2_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm LiteracyLevel2Req"></span>
                                <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LiteracyLevel2_Req") %>'
                                    ControlToValidate="LiteracyLevel2" Display="Dynamic" ErrorMessage='<%# Eval("Literacy2Label", "{0} is required")%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator
                                    ControlToValidate="LiteracyLevel2"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy2Label", "{0} must be a number.") %>'
                                    runat="server"
                                    Text='must be a number'
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator
                                    ControlToValidate="LiteracyLevel2"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage='<%# Eval("Literacy2Label", "{0} must be a number between 0 and 99.") %>'
                                    runat="server"
                                    Text='must be between 0 and 99'
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
                                <%# this.CustomFields.Label1 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom1" runat="server" CssClass="form-control required-asterisk" data-asterisk="Custom1Req"
                                    Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom1DD" runat="server" DataTextField="Description" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                                    CssClass="form-control required-asterisk-dropdown" data-asterisk="Custom1Req">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom1DDTXT" runat="server" Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Custom1_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm Custom1Req"></span>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom1_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                                    ControlToValidate="Custom1DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label1 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom1_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                                    ControlToValidate="Custom1" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label1 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom2_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# this.CustomFields.Label2 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom2" runat="server" CssClass="form-control required-asterisk" data-asterisk="Custom2Req"
                                    Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom2DD" runat="server" DataTextField="Description" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                                    CssClass="form-control required-asterisk-dropdown" data-asterisk="Custom2Req">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom2DDTXT" runat="server" Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Custom2_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm Custom2Req"></span>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom2_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                                    ControlToValidate="Custom2DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label2 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom2_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                                    ControlToValidate="Custom2" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label2 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom3_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# this.CustomFields.Label3 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom3" runat="server" CssClass="form-control required-asterisk" data-asterisk="Custom3Req"
                                    Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom3DD" runat="server" DataTextField="Description" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                                    CssClass="form-control required-asterisk-dropdown" data-asterisk="Custom3Req">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom3DDTXT" runat="server" Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Custom3_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm Custom3Req"></span>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom3_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                                    ControlToValidate="Custom3DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label3 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom3_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                                    ControlToValidate="Custom3" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label3 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom4_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# this.CustomFields.Label4 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom4" runat="server" CssClass="form-control required-asterisk" data-asterisk="Custom4Req"
                                    Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom4DD" runat="server" DataTextField="Description" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                                    CssClass="form-control required-asterisk-dropdown" data-asterisk="Custom4Req">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom4DDTXT" runat="server" Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Custom4_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm Custom4Req"></span>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom4_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                                    ControlToValidate="Custom4DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label4 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom4_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                                    ControlToValidate="Custom4" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label4 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>


                        <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom5_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <%# this.CustomFields.Label5 %>:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Custom5" runat="server" CssClass="form-control required-asterisk" data-asterisk="Custom5Req"
                                    Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'></asp:TextBox>
                                <asp:DropDownList ID="Custom5DD" runat="server" DataTextField="Description" DataValueField="Code"
                                    AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                                    CssClass="form-control required-asterisk-dropdown" data-asterisk="Custom5Req">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="Custom5DDTXT" runat="server" Visible="False"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" visible='<%# Eval("Custom5_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm Custom5Req"></span>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom5_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                                    ControlToValidate="Custom5DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label5 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator runat="server"
                                    Enabled='<%# (bool)Eval("Custom5_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                                    ControlToValidate="Custom5" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label5 + " is required"%>'
                                    SetFocusOnError="True">required</asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group" runat="server" visible='<%# (bool)Eval("TermsOfUseflag_Prompt") %>'>
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-terms"></asp:Label>
                            </label>
                            <label class="col-sm-9 form-control-static">
                                <div class="row">
                                    <asp:CheckBox ID="TermsOfUseflag" runat="server" ReadOnly="False" data-asterisk="TermsOfUseflagReq" CssClass="col-xs-1 gra-registration-checkbox gra-terms-of-use-container required-asterisk-aspcheckbox"></asp:CheckBox>
                                    <span runat="server" visible='<%# Eval("TermsOfUseflag_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm TermsOfUseflagReq"></span>
                                    <asp:Label runat="server" Text="registration-terms-agreement" CssClass="col-xs-10"></asp:Label>
                                </div>
                            </label>
                            <div class="col-sm-9 col-sm-offset-3">
                                <asp:CustomValidator
                                    ClientValidationFunction="TermsOfUseflagValidation"
                                    EnableClientScript="true"
                                    runat="server" Enabled='<%# Eval("TermsOfUseflag_Req") %>'
                                    ErrorMessage="You must accept the terms of use." SetFocusOnError="True">you must accept the terms</asp:CustomValidator>
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
                                    <span runat="server" visible='<%# (bool)Eval("ParentPermFlag_Prompt") && int.Parse(RegistrationAge.Text.Length == 0 ? "0": RegistrationAge.Text) < 18 %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span>
                                    <asp:CustomValidator
                                        ClientValidationFunction="ParentPermFlagValidation"
                                        EnableClientScript="true"
                                        runat="server" Enabled='<%# (bool)Eval("ParentPermFlag_Prompt") && int.Parse(RegistrationAge.Text.Length == 0 ? "0": RegistrationAge.Text) < 18 %>'
                                        ErrorMessage="You must have parental consent." SetFocusOnError="True">must have parental consent</asp:CustomValidator>
                                </div>
                            </div>
                        </asp:Panel>
                    </asp:Panel>

                    <asp:Panel ID="Panel7" runat="server" Visible="False">
                        <div class="row">
                            <div class="col-xs-12 col-sm-8 col-sm-offset-2 margin-1em-bottom">
                                <div class="alert alert-info">
                                    <span class="glyphicon glyphicon-info-sign"></span>
                                    <asp:Label runat="server" Text="registration-form-username-password-details"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                Username:
                            </label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Username" runat="server" CssClass="form-control input-lg gra-register-username required-asterisk" data-asterisk="UsernameReq" Enabled="true" MaxLength="25"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span class="glyphicon glyphicon-ok-sign gra-reg-glyph gra-reg-available text-success"></span>
                                <span class="glyphicon glyphicon-remove-sign gra-reg-glyph gra-reg-unavailable text-danger"></span>
                                <div style="display: inline-block; vertical-align: middle;">
                                    <span class="gra-username-verification text-success gra-reg-available">available</span>
                                    <span class="gra-username-verification text-danger gra-reg-unavailable">not available</span>
                                    <span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm UsernameReq"></span>
                                    <asp:RequiredFieldValidator runat="server" Enabled="true"
                                        ControlToValidate="Username" Display="Dynamic" ErrorMessage="Username is required"
                                        SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator
                                        runat="server" ControlToValidate="Username" ValidationExpression="^[a-zA-Z0-9_-]{5,25}$"
                                        Display="Dynamic"
                                        ErrorMessage="Username must be between 5 and 25 characters and contain no spaces.">username must be between 5 and 25 characters and contain no spaces
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-3 control-label">Password:</label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Password" runat="server" CssClass="pwd form-control input-lg required-asterisk" data-asterisk="PasswordReq" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm PasswordReq"></span>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic"
                                    ControlToValidate="Password" ErrorMessage="Password is required"
                                    ToolTip="Password required" SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator
                                    runat="server" ControlToValidate="Password" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                                    Display="Dynamic"
                                    ErrorMessage="Please select a password of at least seven characters with at least one number and at least one letter.">password not secure
                                </asp:RegularExpressionValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-3 control-label">Verify password:</label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="Password2" runat="server" CssClass="pwd2 form-control input-lg required-asterisk" data-asterisk="Password2Req" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 form-control-static">
                                <span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm Password2Req"></span>
                                <asp:RequiredFieldValidator runat="server" Display="Dynamic"
                                    ControlToValidate="Password2" ErrorMessage="Password validation is required"
                                    ToolTip="Password Re-entry required" SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                <asp:CustomValidator
                                    runat="server" ControlToValidate="Password2"
                                    ErrorMessage="The password and validation do not match."
                                    ClientValidationFunction="ClientValidate">password does not match</asp:CustomValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                <asp:Label runat="server" Text="registration-form-avatar"></asp:Label></label>
                            <div class="col-sm-6">
                                <select id="ddAvatar"></select>
                                <input id="AvatarID" class="avatar selected-avatar" runat="server" visible="true" type="text" style="display: none;" value="1" />
                                <script>
                                    var ddData = <% =Avatar.GetJSONForSelection(1) %>;
                                    $('#ddAvatar').ddslick({
                                        data: ddData,
                                        background: "transparent",
                                        selectText: "Select an avatar",
                                        onSelected: function (data) {
                                            $('.selected-avatar').first().val(data.selectedData.value);
                                        }
                                    });
                                </script>
                            </div>
                        </div>
                    </asp:Panel>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="Panel8" runat="server" Visible="False">
                <div class="row">
                    <div class="col-xs-12">
                        <asp:Label runat="server" Text="registration-success"></asp:Label>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="Panel9" runat="server" Visible="False">
                <div class="row">
                    <div class="col-xs-12">
                        <asp:Label runat="server" Text="registration-success-family-account"></asp:Label>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <div class="panel-footer clearfix">
        <div class="pull-right">
            <asp:Button ID="btnPrev"
                runat="server"
                Text="registration-button-previous"
                CausesValidation="True"
                CssClass="btn btn-default"
                Enabled="False"
                OnClick="btnPrev_Click" />
            <asp:Button ID="btnNext"
                runat="server"
                Text="registration-button-next"
                CausesValidation="True"
                CssClass="btn btn-success"
                OnClick="btnNext_Click"
                OnClientClick="return nextClick();" />

            <asp:Button ID="btnDone"
                runat="server"
                Text="registration-button-done"
                CausesValidation="True"
                CssClass="btn btn-default"
                Visible="False"
                OnClick="btnDone_Click" />
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

<div class="modal fade" style="padding-top: 15%;" id="processingAccountCreation">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <asp:Label runat="server" CssClass="lead" Text="registration-processing"></asp:Label>
            </div>
            <div class="modal-body text-center">
                <div class="progress">
                    <div class="progress-bar progress-bar-striped progress-bar-success active"
                        role="progressbar"
                        aria-valuenow="100"
                        aria-valuemin="0"
                        aria-valuemax="100"
                        style="width: 100%">
                    </div>
                </div>
                <em><span id="processingAccountCreationMessage">Beginning account creation...</span></em>
            </div>
        </div>
    </div>
</div>

<script>
    
    $('.required-asterisk').focusout(function () {
        var asteriskClass = $(this).data('asterisk');
        if (asteriskClass) {
            if ($(this).val().length == 0) {
                $('.' + asteriskClass).show();
            } else {
                $('.' + asteriskClass).hide();
            }
        }
    });

    $('.required-asterisk-dropdown').change(function () {
        var asteriskClass = $(this).data('asterisk');
        if (asteriskClass) {
            if ($(this).val() == 0) {
                $('.' + asteriskClass).show();
            } else {
                $('.' + asteriskClass).hide();
            }
        }
    });

    $().ready(function () {
        $(".datepicker").datepick({
            changeMonth: true,
            showOtherMonths: true,
            selectOtherMonths: true,
            showSpeed: 'fast',
            onSelect: function(dates) {
                var asteriskClass = $(this).data('asterisk');
                if (asteriskClass) {
                    if (dates.length == 0) {
                        $('.' + asteriskClass).show();
                    } else {
                        $('.' + asteriskClass).hide();
                    }
                }
            }
        });

        $('.required-asterisk').each(function (index, element) {
            var asteriskClass = $(element).data('asterisk');
            if (asteriskClass) {
                if ($(element).val().length == 0) {
                    $('.' + asteriskClass).show();
                } else {
                    $('.' + asteriskClass).hide();
                }
            }
        });

        $('.required-asterisk-dropdown').each(function (index, element) {
            var asteriskClass = $(element).data('asterisk');
            if (asteriskClass) {
                if ($(element).val() == 0) {
                    $('.' + asteriskClass).show();
                } else {
                    $('.' + asteriskClass).hide();
                }
            }
        });

        $('.required-asterisk-aspcheckbox :input').each(function (index, element) {
            aspCheckClickCheck($(element));
            $(element).on('click', function(eventData) {
                aspCheckClickCheck($(eventData.target));
            });
        });
    });

    function aspCheckClickCheck(checkboxElement) {
        var asteriskClass = checkboxElement.parent().data('asterisk');
        if (asteriskClass) {
            if (checkboxElement.attr('checked')) {
                $('.' + asteriskClass).hide();
            } else {
                $('.' + asteriskClass).show();
            }
        }
    }

    function usernameAvailableUnavailable(available, unavailable) {
        $.each($('.gra-reg-available'), function (index, value) {
            if (available == true) {
                $(value).show();
            } else {
                $(value).hide();
            }
        });
        $.each($('.gra-reg-unavailable'), function (index, value) {
            if (unavailable == true) {
                $(value).show();
            } else {
                $(value).hide();
            }
        });
    }

    $('.gra-register-username').focusout(function () {
        var potentialUsername = $('.gra-register-username').first().val();
        // maximum 
        if (potentialUsername.length < 5 || potentialUsername.indexOf(' ') >= 0) {
            usernameAvailableUnavailable(false, false);
            return;
        }
        var jqxhr = $.ajax('<%=Request.ApplicationPath%>Handlers/IsUsernameAvailable.ashx?Username=' + potentialUsername)
        .done(function (data, textStatus, jqXHR) {
            if (data && data.IsAvailable && data.IsAvailable == true) {
                usernameAvailableUnavailable(true, false);
            } else {
                usernameAvailableUnavailable(false, true);
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            usernameAvailableUnavailable(false, false);
        });
    });

    function nextClick() {
        if (Page_ClientValidate()) {
            if ('<%=this.CurrentStep%>' == '7') {
                setTimeout(loadingMessage, 5000);
                $('#processingAccountCreation').modal({ backdrop: 'static' });
            }
            return true;
        } else {
            return false;
        }
    }

    var loadingMessageCounter = 0;
    var loadingMessageMessages = ["Reserving username...", "Encrypting password...", "Wrangling bits...", "Characterizing bytes...", "Reticulating splines...", "This is taking a while...", "Sorry about that..."];
    function loadingMessage() {
        $('#processingAccountCreationMessage').text(loadingMessageMessages[loadingMessageCounter]);
        loadingMessageCounter++;
        if (loadingMessageCounter > loadingMessageMessages.length - 1) {
            loadingMessageCounter = 0;
        }
        setTimeout(loadingMessage, 5000);
    }
</script>
