<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyAccountCtl.ascx.cs" Inherits="GRA.SRP.Controls.MyAccountCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<div class="row">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label runat="server" Text="myaccount-title"></asp:Label></span>
    </div>
</div>

<div class="form-horizontal">
    <div class="row">
        <div class="col-xs-12 col-sm-8 col-sm-offset-2">
            <asp:ValidationSummary runat="server"
                HeaderText='<span class="glyphicon glyphicon-exclamation-sign margin-halfem-right"></span>Please correct the following errors:' />
        </div>
    </div>
    <asp:Repeater ID="rptr" runat="server" OnItemDataBound="rptr_ItemDataBound" OnItemCommand="rptr_ItemCommand">
        <ItemTemplate>
            <div class="margin-halfem-top row">
                <asp:HyperLink ID="FamilyAccountList"
                    CausesValidation="false"
                    CommandName="familyAccountList"
                    CssClass="btn btn-default margin-halfem-bottom"
                    runat="server"
                    NavigateUrl="~/Account/FamilyAccountList.aspx">
                        <span class="glyphicon glyphicon-th-list margin-halfem-right"></span>
                        <asp:Label runat="server" Text="myaccount-family"></asp:Label>
                </asp:HyperLink>
                <asp:HyperLink CausesValidation="false"
                    ID="FamilyAccountAdd"
                    CssClass="btn btn-default margin-halfem-bottom"
                    runat="server"
                    NavigateUrl="~/Account/AddFamilyMemberAccount.aspx">
                    <span class="glyphicon glyphicon-plus-sign margin-halfem-right"></span>
                    <asp:Label runat="server" Text="myaccount-add-family-member"></asp:Label>
                </asp:HyperLink>
                <a href="ChangePassword.aspx" class="btn btn-default margin-halfem-bottom">
                    <span class="glyphicon glyphicon-lock margin-halfem-right"></span>
                    Change Password</a>
                <a href="ActivityHistory.aspx" class="btn btn-default margin-halfem-bottom">
                    <span class="glyphicon glyphicon-folder-open margin-halfem-right"></span>
                    Activity History</a>
                <asp:LinkButton runat="server"
                    CommandName="save"
                    CausesValidation="true"
                    data-loading-text="Saving..."
                    OnClientClick="return saveButtonClick();"
                    CssClass="btn btn-success account-save-button margin-halfem-bottom">
                        <span class="glyphicon glyphicon-save margin-halfem-right"></span>
                        <%=this.SaveButtonText %>
                </asp:LinkButton>
            </div>

            <div class="form-group margin-1em-top">
                <label class="col-sm-3 control-label">
                    Username:
                </label>
                <div class="col-sm-9 form-control-static">
                    <%# Eval("Username") %>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("FirstName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-name-first"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="FirstName" runat="server" CssClass="form-control"
                        Text='<%# Eval("FirstName") %>'
                        Enabled='<%# (bool)Eval("FirstName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                        ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group" runat="server" visible='<%# (bool)Eval("MiddleName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-name-middle"></asp:Label>
                </label>

                <div class="col-sm-6">
                    <asp:TextBox ID="MiddleName" runat="server" CssClass="form-control"
                        Text='<%# Eval("MiddleName") %>'
                        Enabled='<%# (bool)Eval("MiddleName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("MiddleName_Req") %>'
                        ControlToValidate="MiddleName" Display="Dynamic" ErrorMessage="Middle name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("LastName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-name-last"></asp:Label>
                </label>

                <div class="col-sm-6">
                    <asp:TextBox ID="LastName" runat="server" CssClass="form-control"
                        Text='<%# Eval("LastName") %>'
                        Enabled='<%# (bool)Eval("LastName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LastName_Req") %>'
                        ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-avatar"></asp:Label>
                </label>
                <div class="col-sm-9">
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
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolGrade_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-grade"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="SchoolGrade" runat="server" CssClass="form-control"
                        Text='<%# Eval("SchoolGrade") %>'
                        Enabled='<%# (bool)Eval("SchoolGrade_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("SchoolGrade_Req") %>'
                        ControlToValidate="SchoolGrade" Display="Dynamic" ErrorMessage="School grade is required."
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>


            <div class="form-group" runat="server" visible='<%# (bool)Eval("DOB_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-dob"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="DOB" runat="server" CssClass="form-control datepicker"
                        Text='<%# Eval("DOB").ToString().Length > 0 ? ((DateTime)Eval("DOB")).ToShortDateString() : string.Empty%>'
                        Enabled='<%# (bool)Eval("DOB_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("Age_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-age"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Age" runat="server" CssClass="form-control"
                        Text='<%# ((int)Eval("Age") == 0 ? "" : Eval("Age"))%>'
                        Enabled='<%# (bool)Eval("Age_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("Gender_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-gender"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="GenderTxt" runat="server" Text='<%# Eval("Gender") %>' Enabled='<%# (bool)Eval("Gender_Edit") %>' Visible="False"></asp:TextBox>
                    <asp:DropDownList ID="Gender" runat="server" CssClass="form-control"
                        Enabled='<%# (bool)Eval("Gender_Edit") %>'
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("EmailAddress_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-email"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="EmailAddress" runat="server" CssClass="form-control"
                        Text='<%# Eval("EmailAddress") %>'
                        Enabled='<%# (bool)Eval("EmailAddress_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("PhoneNumber_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-phone"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="PhoneNumber" runat="server" CssClass="form-control"
                        Text='<%# Eval("PhoneNumber") %>' placeholder="602-555-1212"
                        Enabled='<%# (bool)Eval("PhoneNumber_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress1_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-address1"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="StreetAddress1" runat="server" CssClass="form-control"
                        Text='<%# Eval("StreetAddress1") %>'
                        Enabled='<%# (bool)Eval("StreetAddress1_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("StreetAddress1_Req") %>'
                        ControlToValidate="StreetAddress1" Display="Dynamic" ErrorMessage="Street address is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("StreetAddress2_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-address2"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="StreetAddress2" runat="server" CssClass="form-control"
                        Text='<%# Eval("StreetAddress2") %>'
                        Enabled='<%# (bool)Eval("StreetAddress2_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("StreetAddress2_Req") %>'
                        ControlToValidate="StreetAddress2" Display="Dynamic" ErrorMessage="Street address 2 is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("City_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-city"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="City" runat="server" CssClass="form-control"
                        Text='<%# Eval("City") %>'
                        Enabled='<%# (bool)Eval("City_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("City_Req") %>'
                        ControlToValidate="City" Display="Dynamic" ErrorMessage="City is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("State_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-state"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="State" runat="server" CssClass="form-control"
                        Text='<%# Eval("State") %>'
                        Enabled='<%# (bool)Eval("State_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("State_Req") %>'
                        ControlToValidate="State" Display="Dynamic" ErrorMessage="State is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>

                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("ZipCode_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-zip"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ZipCode" runat="server" CssClass="form-control"
                        Text='<%# Eval("ZipCode") %>' placeholder="85004 or 85004-1140"
                        Enabled='<%# (bool)Eval("ZipCode_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("Country_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-country"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Country" runat="server" CssClass="form-control"
                        Text='<%# Eval("Country") %>'
                        Enabled='<%# (bool)Eval("Country_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Country_Req") %>'
                        ControlToValidate="Country" Display="Dynamic" ErrorMessage="Country is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("County_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-county"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="County" runat="server" CssClass="form-control"
                        Text='<%# Eval("County") %>'
                        Enabled='<%# (bool)Eval("County_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("County_Req") %>'
                        ControlToValidate="County" Display="Dynamic" ErrorMessage="County is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianFirstName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-guardian-name-first"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ParentGuardianFirstName" runat="server" CssClass="form-control"
                        Text='<%# Eval("ParentGuardianFirstName") %>'
                        Enabled='<%# (bool)Eval("ParentGuardianFirstName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianFirstName_Req") %>'
                        ControlToValidate="ParentGuardianFirstName" Display="Dynamic" ErrorMessage="Parent/guardian first name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianMiddleName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-guardian-name-middle"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ParentGuardianMiddleName" runat="server" CssClass="form-control"
                        Text='<%# Eval("ParentGuardianMiddleName") %>'
                        Enabled='<%# (bool)Eval("ParentGuardianMiddleName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianMiddleName_Req") %>'
                        ControlToValidate="ParentGuardianMiddleName" Display="Dynamic" ErrorMessage="Parent/guardian middle name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("ParentGuardianLastName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-guardian-name-last"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="ParentGuardianLastName" runat="server" CssClass="form-control"
                        Text='<%# Eval("ParentGuardianLastName") %>'
                        Enabled='<%# (bool)Eval("ParentGuardianLastName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("ParentGuardianLastName_Req") %>'
                        ControlToValidate="ParentGuardianLastName" Display="Dynamic" ErrorMessage="Parent/guardian last name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("District_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school-district"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList
                        CssClass="form-control"
                        ID="District"
                        runat="server"
                        DataSourceID="odsDDDistrict"
                        DataTextField="Description"
                        DataValueField="CID"
                        AppendDataBoundItems="True"
                        Enabled='<%# (bool)Eval("District_Edit") %>'
                        OnSelectedIndexChanged="District_SelectedIndexChanged"
                        AutoPostBack="true">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="DistrictTxt" runat="server"
                        Text='<%# (FormatHelper.SafeToInt(Eval("District").ToString()) == 0 ? "" : Eval("District") ) %>'
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("PrimaryLibrary_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-library"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="PrimaryLibrary"
                        CssClass="form-control"
                        runat="server"
                        DataSourceID="odsDDBranch"
                        DataTextField="Description"
                        DataValueField="CID"
                        AppendDataBoundItems="True"
                        Enabled='<%# (bool)Eval("PrimaryLibrary_Edit") %>'>
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="PrimaryLibraryTxt" runat="server"
                        Text='<%# ((int) Eval("PrimaryLibrary") ==0 ? "" : Eval("PrimaryLibrary")) %>'
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("LibraryCard_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-library-card"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="LibraryCard" runat="server" CssClass="form-control"
                        Text='<%# Eval("LibraryCard") %>'
                        Enabled='<%# (bool)Eval("LibraryCard_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                        ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library card # is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolType_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school-type"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="SchoolType"
                        CssClass="form-control"
                        runat="server"
                        DataSourceID="odsDDSchoolType"
                        DataTextField="Description"
                        DataValueField="CID"
                        AppendDataBoundItems="True"
                        Enabled='<%# (bool)Eval("SchoolType_Edit") %>'
                        AutoPostBack="true"
                        OnSelectedIndexChanged="SchoolType_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SchoolTypeTxt" runat="server"
                        Text='<%# ((int) Eval("SchoolType") ==0 ? "" : Eval("SchoolType")) %>'
                        Visible="False"></asp:TextBox>

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

            <div class="form-group" runat="server" visible='<%# (bool)Eval("SDistrict_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school-district"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="SDistrict"
                        CssClass="form-control"
                        runat="server"
                        DataSourceID="odsDDSDistrict"
                        DataTextField="Description"
                        DataValueField="CID"
                        AppendDataBoundItems="True"
                        Enabled='<%# (bool)Eval("SDistrict_Edit") %>'
                        AutoPostBack="true"
                        OnSelectedIndexChanged="SDistrict_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SDistrictTxt" runat="server"
                        Text='<%# ((int) Eval("SDistrict") == 0 ? "" : Eval("SDistrict")) %>'
                        Visible="false"></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("SchoolName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-school"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:DropDownList ID="SchoolName"
                        CssClass="form-control"
                        runat="server"
                        DataSourceID="odsDDSchool"
                        DataTextField="Description"
                        DataValueField="CID"
                        AppendDataBoundItems="True"
                        Enabled='<%# (bool)Eval("SchoolName_Edit") %>'>
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="SchoolNameTxt"
                        runat="server"
                        Text='<%# (FormatHelper.SafeToInt(Eval("SchoolName").ToString()) == 0 ? "" : Eval("SchoolName") ) %>'
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("Teacher_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-teacher"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Teacher" runat="server" CssClass="form-control"
                        Text='<%# Eval("Teacher") %>'
                        Enabled='<%# (bool)Eval("Teacher_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("Teacher_Req") %>'
                        ControlToValidate="Teacher" Display="Dynamic" ErrorMessage="Teacher is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("GroupTeamName_Show")%>'>
                <label class="col-sm-3 control-label">
                    <asp:Label runat="server" Text="registration-form-group"></asp:Label>
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="GroupTeamName" runat="server" CssClass="form-control"
                        Text='<%# Eval("GroupTeamName") %>'
                        Enabled='<%# (bool)Eval("GroupTeamName_Edit") %>'></asp:TextBox>
                </div>
                <div class="col-sm-3 form-control-static">
                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("GroupTeamName_Req") %>'
                        ControlToValidate="GroupTeamName" Display="Dynamic" ErrorMessage="Group/team name is required"
                        SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel1_Show")%>'>
                <label class="col-sm-3 control-label">
                    <%# Eval("Literacy1Label")%>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="LiteracyLevel1" runat="server" CssClass="form-control"
                        Text='<%# ((int) Eval("LiteracyLevel1") ==0 ? "" : Eval("LiteracyLevel1")) %>'
                        Enabled='<%# (bool)Eval("LiteracyLevel1_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("LiteracyLevel2_Show")%>'>
                <label class="col-sm-3 control-label">
                    <%# Eval("Literacy2Label")%>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="LiteracyLevel2" runat="server" CssClass="form-control"
                        Text='<%# ((int) Eval("LiteracyLevel2") ==0 ? "" : Eval("LiteracyLevel2")) %>'
                        Enabled='<%# (bool)Eval("LiteracyLevel2_Edit") %>'></asp:TextBox>
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
            <div class="form-group" runat="server" visible='<%# (bool)Eval("Custom1_Show") %>'>
                <label class="col-sm-3 control-label">
                    <%# this.CustomFields.Label1 %>:
                </label>
                <div class="col-sm-6">
                    <asp:TextBox ID="Custom1" runat="server" CssClass="form-control"
                        Enabled='<%# (bool)Eval("Custom1_Edit") %>'
                        Text='<%# Eval("Custom1") %>'
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom1DD" runat="server" DataTextField="Description" DataValueField="Code"
                        Enabled='<%# (bool)Eval("Custom1_Edit") %>'
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues1)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom1DDTXT" runat="server" Visible="False"
                        Text='<%# Eval("Custom1") %>'></asp:TextBox>
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
                        Enabled='<%# (bool)Eval("Custom2_Edit") %>'
                        Text='<%# Eval("Custom2") %>'
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom2DD" runat="server" DataTextField="Description"
                        Enabled='<%# (bool)Eval("Custom2_Edit") %>'
                        DataValueField="Code"
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues2)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom2DDTXT" runat="server" Visible="False"
                        Text='<%# Eval("Custom2") %>'></asp:TextBox>
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
                        Enabled='<%# (bool)Eval("Custom3_Edit") %>'
                        Text='<%# Eval("Custom3") %>'
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom3DD" runat="server" DataTextField="Description" DataValueField="Code"
                        Enabled='<%# (bool)Eval("Custom3_Edit") %>'
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues3)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom3DDTXT" runat="server" Visible="False"
                        Text='<%# Eval("Custom3") %>'></asp:TextBox>
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
                        Enabled='<%# (bool)Eval("Custom4_Edit") %>'
                        Text='<%# Eval("Custom4") %>'
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom4DD" runat="server" DataTextField="Description" DataValueField="Code"
                        Enabled='<%# (bool)Eval("Custom4_Edit") %>'
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues4)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom4DDTXT" runat="server" Visible="False"
                        Text='<%# Eval("Custom4") %>'></asp:TextBox>
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
                        Enabled='<%# (bool)Eval("Custom5_Edit") %>'
                        Text='<%# Eval("Custom5") %>'
                        Visible='<%#string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'></asp:TextBox>
                    <asp:DropDownList ID="Custom5DD" runat="server" DataTextField="Description" DataValueField="Code"
                        Enabled='<%# (bool)Eval("Custom5_Edit") %>'
                        AppendDataBoundItems="True" Visible='<%#!string.IsNullOrEmpty(this.CustomFields.DDValues5)%>'
                        CssClass="form-control">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="Custom5DDTXT" runat="server" Visible="False"
                        Text='<%# Eval("Custom5") %>'></asp:TextBox>
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
            <div class="form-group clearfix">
                <div class="col-sm-9">
                    <div class="pull-right">
                        <asp:LinkButton runat="server"
                            CommandName="save"
                            CausesValidation="true"
                            data-loading-text="Saving..."
                            OnClientClick="return saveButtonClick();"
                            CssClass="btn btn-success account-save-button">
                        <span class="glyphicon glyphicon-save margin-halfem-right"></span>
                        <%=this.SaveButtonText %>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

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

<script>
    $(function () {
        $(".datepicker").datepick({
            changeMonth: true,
            showOtherMonths: true,
            selectOtherMonths: true,
            showSpeed: 'fast'
        });
    });
    function saveButtonClick() {
        $('.account-save-button').button('loading');
        return true;
    }
</script>
