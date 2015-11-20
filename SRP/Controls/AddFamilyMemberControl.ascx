<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddFamilyMemberControl.ascx.cs" Inherits="GRA.SRP.Controls.AddFamilyMemberControl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<div class="row">
    <div class="col-sm-12 margin-1em-bottom">
        <span class="h1">
            <asp:Label runat="server" Text="family-member-add-title"></asp:Label></span>
    </div>
</div>
<asp:Label ID="SA" runat="server" Text="" Visible="False"></asp:Label>

<div class="row">
    <div class="col-xs-12 col-sm-8 col-sm-offset-2">
        <asp:ValidationSummary runat="server"
            HeaderText='<span class="glyphicon glyphicon-exclamation-sign margin-halfem-right"></span>Please correct the following errors:' />
    </div>
</div>
<div class="form-horizontal">
    <asp:Repeater ID="rptr" runat="server"
        OnItemCommand="rptr_ItemCommand" OnItemDataBound="rptr_ItemDataBound">
        <ItemTemplate>
            <div class="form-group">
                <label class="col-sm-3 control-label">
                    Username:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Username" runat="server" CssClass="form-control gra-register-username" Enabled="true" MaxLength="25"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <span class="glyphicon glyphicon-ok-sign gra-reg-glyph gra-reg-available text-success"></span>
                    <span class="glyphicon glyphicon-remove-sign gra-reg-glyph gra-reg-unavailable text-danger"></span>
                    <div style="display: inline-block; vertical-align: middle;">
                        <span class="gra-username-verification text-success gra-reg-available">available</span>
                        <span class="gra-username-verification text-danger gra-reg-unavailable">not available</span>
                        <asp:RequiredFieldValidator runat="server" Enabled="true"
                            ControlToValidate="Username" Display="Dynamic" ErrorMessage="Username is required"
                            SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator
                            runat="server" ControlToValidate="Username" ValidationExpression="^[a-zA-Z0-9_-]{5,25}$"
                            Display="Dynamic"
                            ErrorMessage="Username must be between 5 and 25 characters long.">* username must be between 5 and 25 characters long
                        </asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label">Password:</label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Password" runat="server" CssClass="pwd form-control" TextMode="Password"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Display="Dynamic"
                        ControlToValidate="Password" ErrorMessage="Password is required"
                        ToolTip="Password required" SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator
                        runat="server" ControlToValidate="Password" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                        Display="Dynamic"
                        ErrorMessage="Please select a password of at least seven characters with at least one number and at least one letter.">* password not secure
                    </asp:RegularExpressionValidator>
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
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                        ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("MiddleName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-name-middle"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="MiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("MiddleName_Req") %>'
                        ControlToValidate="MiddleName" Display="Dynamic" ErrorMessage="Middle name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("LastName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-name-last"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="LastName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LastName_Req") %>'
                        ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>


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
                    <asp:RequiredFieldValidator runat="server" Enabled='true'
                        ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:CompareValidator runat="server" Enabled='true'
                        ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required"
                        SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                </div>
            </div>

            <div class="form-group">
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-avatar"></asp:Label></label>
                <div class="col-sm-9">
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
                        $('#ddAvatar').ddslick('select', { index: 0 });
                    </script>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolGrade_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-grade"></asp:Label>
                </label>

                <div class="col-sm-6">
                    <asp:TextBox ID="SchoolGrade" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolGrade_Req") %>'
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
                    <asp:RequiredFieldValidator runat="server"
                        ControlToValidate="DOB" Display="Dynamic" ErrorMessage="Date of Birth is required." Enabled='<%# Eval("DOB_Req") %>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:CompareValidator runat="server" ControlToValidate="DOB"
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
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Age_Req") %>'
                        ControlToValidate="Age" Display="Dynamic" ErrorMessage="Age is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator
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

            <div class="form-group" runat="server" visible='<%# (bool)Eval("Gender_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-gender"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="Gender" runat="server" CssClass="form-control"
                        AppendDataBoundItems="True">
                        <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                        <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                        <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                        <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Gender_Req") %>'
                        ControlToValidate="Gender" Display="Dynamic" ErrorMessage="Gender is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("EmailAddress_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-email"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="EmailAddress" runat="server" CssClass="form-control" Text='<%# Eval("EmailAddress") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("EmailAddress_Req") %>'
                        ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is not valid"
                        SetFocusOnError="True"
                        ValidationExpression="[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?">* invalid</asp:RegularExpressionValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("PhoneNumber_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-phone"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="PhoneNumber" runat="server" CssClass="form-control" Text='<%# Eval("PhoneNumber") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("PhoneNumber_Req") %>'
                        ControlToValidate="PhoneNumber" Display="Dynamic" ErrorMessage="Phone number is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="PhoneNumber" Display="Dynamic"
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
                    <asp:TextBox ID="StreetAddress1" runat="server" CssClass="form-control" Text='<%# Eval("StreetAddress1") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("StreetAddress1_Req") %>'
                        ControlToValidate="StreetAddress1" Display="Dynamic" ErrorMessage="Street address is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress2_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-address2"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="StreetAddress2" runat="server" CssClass="form-control" Text='<%# Eval("StreetAddress2") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("StreetAddress2_Req") %>'
                        ControlToValidate="StreetAddress2" Display="Dynamic" ErrorMessage="Street address 2 is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("City_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-city"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="City" runat="server" CssClass="form-control" Text='<%# Eval("City") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("City_Req") %>'
                        ControlToValidate="City" Display="Dynamic" ErrorMessage="City is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("State_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-state"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="State" runat="server" CssClass="form-control" Text='<%# Eval("State") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("State_Req") %>'
                        ControlToValidate="State" Display="Dynamic" ErrorMessage="State is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("ZipCode_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-zip"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ZipCode" runat="server" CssClass="form-control" Text='<%# Eval("ZipCode") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ZipCode_Req") %>'
                        ControlToValidate="ZipCode" Display="Dynamic" ErrorMessage="ZIP code is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="ZipCode" Display="Dynamic"
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
                    <asp:TextBox ID="Country" runat="server" CssClass="form-control" Text='<%# Eval("Country") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Country_Req") %>'
                        ControlToValidate="Country" Display="Dynamic" ErrorMessage="Country is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("County_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-county"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="County" runat="server" CssClass="form-control" Text='<%# Eval("County") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("County_Req") %>'
                        ControlToValidate="County" Display="Dynamic" ErrorMessage="County is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianFirstName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-guardian-name-first"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ParentGuardianFirstName" runat="server" CssClass="form-control" Text='<%# Eval("ParentGuardianFirstName") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianFirstName_Req") %>'
                        ControlToValidate="ParentGuardianFirstName" Display="Dynamic" ErrorMessage="Parent/guardian first name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianMiddleName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-guardian-name-middle"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ParentGuardianMiddleName" runat="server" CssClass="form-control" Text='<%# Eval("ParentGuardianMiddleName") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianMiddleName_Req") %>'
                        ControlToValidate="ParentGuardianMiddleName" Display="Dynamic" ErrorMessage="Parent/guardian middle name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianLastName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-guardian-name-last"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ParentGuardianLastName" runat="server" CssClass="form-control" Text='<%# Eval("ParentGuardianLastName") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianLastName_Req") %>'
                        ControlToValidate="ParentGuardianLastName" Display="Dynamic" ErrorMessage="Parent/guardian last name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("District_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-library-district"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDDistrict" DataTextField="Description" DataValueField="CID"
                        AppendDataBoundItems="True"
                        AutoPostBack="true"
                        CssClass="form-control"
                        OnSelectedIndexChanged="District_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="DistrictTxt" runat="server" Text='<%# (FormatHelper.SafeToInt(Eval("District").ToString()) == 0 ? "" : Eval("District") ) %>'
                        Visible="false"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("District_Req") %>'
                        ControlToValidate="District" Display="Dynamic" ErrorMessage="Library district is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:CompareValidator runat="server" Enabled='<%# Eval("District_Req") %>'
                        ControlToValidate="District" Display="Dynamic" ErrorMessage="Library district is required"
                        SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("PrimaryLibrary_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-library"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Description" DataValueField="CID"
                        AppendDataBoundItems="True" CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="PrimaryLibraryTxt" runat="server" Text='<%# ((int) Eval("PrimaryLibrary") ==0 ? "" : Eval("PrimaryLibrary")) %>'
                        Visible="false"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
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
                    <asp:TextBox ID="LibraryCard" runat="server" CssClass="form-control" Text='<%# Eval("LibraryCard") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                        ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library card # is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("SDistrict_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school-district"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSDistrict" DataTextField="Description" DataValueField="CID"
                        AppendDataBoundItems="True"
                        AutoPostBack="true" CssClass="form-control"
                        OnSelectedIndexChanged="SDistrict_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SchoolTypeTxt" runat="server" Text='<%# ((int) Eval("SchoolType") ==0 ? "" : Eval("SchoolType")) %>'
                        Visible="False"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                        ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:CompareValidator runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                        ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required"
                        SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolType_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school-type"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Description" DataValueField="CID"
                        AppendDataBoundItems="True" CssClass="form-control"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="SchoolType_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SDistrictTxt" runat="server" Text='<%# ((int) Eval("SDistrict") == 0 ? "" : Eval("SDistrict")) %>'
                        Visible="false"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">

                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                        ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School type is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:CompareValidator runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                        ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School type is required"
                        SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Description" DataValueField="CID"
                        AppendDataBoundItems="True" CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SchoolNameTxt" runat="server" Text='<%# (FormatHelper.SafeToInt(Eval("SchoolName").ToString()) == 0 ? "" : Eval("SchoolName") ) %>'
                        Visible="false"></asp:TextBox>

                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                        ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:CompareValidator runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                        ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School name is required"
                        SetFocusOnError="True" Operator="GreaterThan" ValueToCompare="0">* required</asp:CompareValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("Teacher_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-teacher"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Teacher" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Teacher_Req") %>'
                        ControlToValidate="Teacher" Display="Dynamic" ErrorMessage="Teacher is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("GroupTeamName_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-group"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="GroupTeamName" runat="server" CssClass="form-control" Text='<%# Eval("GroupTeamName") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("GroupTeamName_Req") %>'
                        ControlToValidate="GroupTeamName" Display="Dynamic" ErrorMessage="Group/team name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel1_Prompt")%>'>
                <label class="col-sm-3 control-label">
                    <%# Eval("Literacy1Label")%>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="LiteracyLevel1" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LiteracyLevel1_Req") %>'
                        ControlToValidate="LiteracyLevel1" Display="Dynamic" ErrorMessage='<%# Eval("Literacy1Label", "{0} is required")%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator
                        ControlToValidate="LiteracyLevel1"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage='<%# Eval("Literacy1Label", "{0} must be a number.") %>'
                        runat="server"
                        Text='* must be a number'
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
                    <asp:TextBox ID="LiteracyLevel2" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LiteracyLevel2_Req") %>'
                        ControlToValidate="LiteracyLevel2" Display="Dynamic" ErrorMessage='<%# Eval("Literacy2Label", "{0} is required")%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator
                        ControlToValidate="LiteracyLevel2"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage='<%# Eval("Literacy2Label", "{0} must be a number.") %>'
                        runat="server"
                        Text='* must be a number'
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
                        Text='* must be between 0 and 99'
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom1_Prompt") %>'>
                <label class="col-sm-3 control-label">
                    <%# this.CustomFields.Label1 %>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Custom1" runat="server" CssClass="form-control"
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom1DD" runat="server" DataTextField="Description" DataValueField="Code"
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom1DDTXT" runat="server" Visible="False" Text='<%#Eval("Custom1") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom1_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                        ControlToValidate="Custom1DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label1 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom1_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                        ControlToValidate="Custom1" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label1 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom2_Prompt") %>'>
                <label class="col-sm-3 control-label">
                    <%# this.CustomFields.Label2 %>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Custom2" runat="server" CssClass="form-control"
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom2DD" runat="server" DataTextField="Description" DataValueField="Code"
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom2DDTXT" runat="server" Visible="False" Text='<%#Eval("Custom2") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom2_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                        ControlToValidate="Custom2DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label2 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom2_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                        ControlToValidate="Custom2" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label2 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom3_Prompt") %>'>
                <label class="col-sm-3 control-label">
                    <%# this.CustomFields.Label3 %>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Custom3" runat="server" CssClass="form-control"
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom3DD" runat="server" DataTextField="Description" DataValueField="Code"
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom3DDTXT" runat="server" Visible="False" Text='<%#Eval("Custom3") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom3_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                        ControlToValidate="Custom3DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label3 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom3_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                        ControlToValidate="Custom3" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label3 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom4_Prompt") %>'>
                <label class="col-sm-3 control-label">
                    <%# this.CustomFields.Label4 %>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Custom4" runat="server" CssClass="form-control"
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom4DD" runat="server" DataTextField="Description" DataValueField="Code"
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom4DDTXT" runat="server" Visible="False" Text='<%#Eval("Custom4") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom4_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                        ControlToValidate="Custom4DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label4 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom4_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                        ControlToValidate="Custom4" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label4 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>


            <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom5_Prompt") %>'>
                <label class="col-sm-3 control-label">
                    <%# this.CustomFields.Label5 %>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Custom5" runat="server" CssClass="form-control"
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom5DD" runat="server" DataTextField="Description" DataValueField="Code"
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom5DDTXT" runat="server" Visible="False" Text='<%#Eval("Custom5") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom5_Req") && !string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                        ControlToValidate="Custom5DD" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label5 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator runat="server"
                        Enabled='<%# (bool)Eval("Custom5_Req") && string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                        ControlToValidate="Custom5" Display="Dynamic" ErrorMessage='<%# this.CustomFields.Label5 + " is required"%>'
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="row">
                <div class="col-xs-9 clearfix">
                    <div class="pull-right">
                        <asp:HyperLink runat="server" CssClass="btn btn-default" NavigateUrl="~/Account/"><asp:Label runat="server" Text="family-member-add-cancel"></asp:Label></asp:HyperLink>
                        <asp:LinkButton runat="server"
                            CommandName="save"
                            CausesValidation="true"
                            OnClientClick="return saveButtonClick();"
                            CssClass="btn btn-success account-save-button">
                        <span class="glyphicon glyphicon-save margin-halfem-right"></span>
                        <%=this.SaveButtonText %>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
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
                    <asp:ControlParameter ControlID="blankCity" DefaultValue="" Name="city"
                        PropertyName="Text" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:TextBox runat="server" Visible="false" ID="blankCity"></asp:TextBox>

            <asp:ObjectDataSource ID="odsDDBranch" runat="server"
                SelectMethod="GetFilteredBranchDDValues"
                TypeName="GRA.SRP.DAL.LibraryCrosswalk">
                <SelectParameters>
                    <asp:ControlParameter ControlID="district" DefaultValue="0" Name="districtID"
                        PropertyName="Text" Type="Int32" />
                    <asp:ControlParameter ControlID="blankCity" DefaultValue="" Name="city"
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
                    <asp:ControlParameter ControlID="SchoolTypeTxt" DefaultValue="0" Name="schTypeID"
                        PropertyName="Text" Type="Int32" />
                    <asp:ControlParameter ControlID="sdistrict" DefaultValue="0" Name="districtID"
                        PropertyName="Text" Type="Int32" />
                    <asp:ControlParameter ControlID="blankCity" DefaultValue="" Name="city"
                        PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="SchoolGrade" DefaultValue="0" Name="grade"
                        PropertyName="Text" Type="Int32" />
                    <asp:ControlParameter ControlID="age" DefaultValue="0" Name="age"
                        PropertyName="Text" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </ItemTemplate>
    </asp:Repeater>
</div>

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
    $(function () {
        $(".datepicker").datepick({
            changeMonth: true,
            showOtherMonths: true,
            selectOtherMonths: true,
            showSpeed: 'fast'
        });
    });
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
        if (potentialUsername.length < 5) {
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

    function saveButtonClick() {
        if (Page_ClientValidate()) {
            setTimeout(loadingMessage, 5000);
            $('#processingAccountCreation').modal({ backdrop: 'static' });
        }
        return true;
    }

    var loadingMessageCounter = 0;
    var loadingMessageMessages = ["Reserving username...", "Encrypting password...", "Wrangling bits...", "Characterizing bytes...", "Reticulating splines...", "This is taking a while...", "Sorry about that..."];
    function loadingMessage() {
        console.log(loadingMessageMessages[loadingMessageCounter]);
        $('#processingAccountCreationMessage').text(loadingMessageMessages[loadingMessageCounter]);
        loadingMessageCounter++;
        if (loadingMessageCounter > loadingMessageMessages.length - 1) {
            loadingMessageCounter = 0;
        }
        setTimeout(loadingMessage, 5000);
    }
</script>
