<%@ Page Title="Sign Up!" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="RegisterILS.aspx.cs" Inherits="GRA.SRP.RegisterILS" %>

<%@ Import Namespace="GRA.SRP.DAL" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-8 col-md-offset-2">

            <div class="panel panel-default">
                <div class="panel-heading">
                    <asp:Label runat="server" CssClass="lead" Text="registration-title"></asp:Label>

                </div>
                <div class="panel-body">
                    <div class="form-horizontal">
                        <asp:Panel ID="instructionsPanel" runat="server" CssClass="row">
                            <div class="col-xs-12">
                                <asp:Label runat="server" Text="registration-instructions"></asp:Label>
                            </div>

                            <div class="col-xs-12 col-sm-8 col-sm-offset-2">
                                <asp:ValidationSummary runat="server"
                                    HeaderText='<span class="glyphicon glyphicon-exclamation-sign margin-halfem-right"></span>Please correct the following errors:' />
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="familyMemberPanel" runat="server" Visible="False" CssClass="row">
                            <div class="col-xs-12 col-sm-8 col-sm-offset-2">
                                <div class="alert alert-info">
                                    <span class="glyphicon glyphicon-info-sign"></span>
                                    <asp:Label runat="server" Text="registration-create-family-accounts"></asp:Label>
                                </div>
                            </div>
                        </asp:Panel>


                        <asp:Panel runat="server" ID="step1" DefaultButton="NavigationNext">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-sm-offset-3 margin-1em-bottom">
                                    <strong>PIN is optional.</strong>
                                </div>
                            </div>
                            <div class="form-group" runat="server" visible='<%# (bool)Eval("LibraryCard_Prompt")%>'>
                                <label class="col-sm-3 control-label">
                                    <asp:Label runat="server" Text="registration-form-library-card"></asp:Label>
                                </label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="LibraryCard" data-asterisk="LibraryCardReq" runat="server" CssClass="form-control required-asterisk"></asp:TextBox>
                                </div>
                                <div class="col-sm-3 form-control-static">
                                    <span runat="server" visible='<%# Eval("LibraryCard_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm LibraryCardReq"></span>
                                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                                        ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library card # is required"
                                        SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group" runat="server" visible='<%# (bool)Eval("LibraryCard_Prompt")%>'>
                                <label class="col-sm-3 control-label">
                                    Library card PIN:
                                </label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="LibraryPin" runat="server" CssClass="form-control" TextMode="password" MaxLength="4"></asp:TextBox>
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:Panel runat="server" ID="step2" Visible="false" DefaultButton="NavigationNext">
                            <div class="form-group" runat="server" visible='<%# (bool)Eval("FirstName_Prompt")%>'>
                                <label class="col-sm-3 control-label">
                                    <asp:Label runat="server" Text="registration-form-name-first"></asp:Label>
                                </label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="FirstName" data-asterisk="FirstNameReq" runat="server" CssClass="form-control required-asterisk"></asp:TextBox>
                                </div>
                                <div class="col-sm-3 form-control-static">
                                    <span runat="server" visible='<%# Eval("FirstName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm FirstNameReq"></span>
                                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                                        ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First name is required"
                                        SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group" runat="server" visible='<%# (bool)Eval("LastName_Prompt")%>'>
                                <label class="col-sm-3 control-label">
                                    <asp:Label runat="server" Text="registration-form-name-last"></asp:Label>
                                </label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="LastName" data-asterisk="LastNameReq" runat="server" CssClass="form-control required-asterisk"></asp:TextBox>
                                </div>
                                <div class="col-sm-3 form-control-static">
                                    <span runat="server" visible='<%# Eval("LastName_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm LastNameReq"></span>
                                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("LastName_Req") %>'
                                        ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last name is required"
                                        SetFocusOnError="True">required</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group" runat="server" visible='<%# (bool)Eval("EmailAddress_Prompt")%>'>
                                <label class="col-sm-3 control-label">
                                    <asp:Label runat="server" Text="registration-form-email"></asp:Label>
                                </label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="EmailAddress" data-asterisk="EmailReq" runat="server" CssClass="form-control required-asterisk"></asp:TextBox>
                                </div>
                                <div class="col-sm-3 form-control-static">
                                    <span runat="server" visible='<%# Eval("EmailAddress_Req") %>' class="text-danger glyphicon glyphicon-asterisk glyphicon-sm EmailReq"></span>
                                    <asp:RequiredFieldValidator runat="server" Enabled='<%# Eval("EmailAddress_Req") %>'
                                        ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email address is required"
                                        SetFocusOnError="True">required</asp:RequiredFieldValidator>
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
                        </asp:Panel>

                        <asp:Panel runat="server" ID="step3" Visible="false" DefaultButton="NavigationNext">
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
                                    <asp:TextBox ID="Username" runat="server" data-asterisk="UsernameReq" CssClass="form-control input-lg gra-register-username required-asterisk" Enabled="true" MaxLength="25"></asp:TextBox>
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
                                    <asp:TextBox ID="Password" runat="server" data-asterisk="PasswordReq" CssClass="pwd form-control input-lg required-asterisk" TextMode="Password"></asp:TextBox>
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
                                    <asp:TextBox ID="Password2" data-asterisk="Password2Req" runat="server" CssClass="pwd2 form-control input-lg required-asterisk" TextMode="Password"></asp:TextBox>
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

                        <asp:Panel runat="server" ID="step4" Visible="false" DefaultButton="NavigationNext">
                            <div class="row">
                                <div class="col-sm-10 col-sm-offset-1 margin-1em-bottom">
                                    <asp:Label runat="server" Text="registration-form-family-simplified"></asp:Label>
                                </div>
                            </div>
                            <div class="row margin-1em-top margin-1em-bottom">
                                <div class="col-sm-6 col-sm-offset-3">
                                    <asp:Button
                                        runat="server"
                                        Text="registration-button-family"
                                        CssClass="btn btn-info btn-block btn-lg"
                                        OnClick="NavigationFamilyMember" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="panel-footer clearfix">
                    <div class="pull-right">
                        <asp:Button ID="NavigationBack"
                            runat="server"
                            Text="registration-button-previous"
                            CssClass="btn btn-default"
                            Enabled="False"
                            CausesValidation="False"
                            OnClick="NavigationBackClick" />

                        <asp:Button ID="NavigationNext"
                            runat="server"
                            Text="registration-button-next"
                            CausesValidation="True"
                            CssClass="btn btn-success"
                            OnClick="NavigationNextClick" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:TextBox Visible="false" runat="server" ID="MasterPID"></asp:TextBox>
    <asp:TextBox Visible="false" runat="server" ID="EarnedBadges"></asp:TextBox>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
        SelectMethod="GetFilteredBranchDDValues"
        TypeName="GRA.SRP.DAL.LibraryCrosswalk">
        <SelectParameters>
            <asp:Parameter Name="districtId" />
            <asp:Parameter Name="city" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomOfPage" runat="server">
    <script>
        function ClientValidate(source, arguments) {
            arguments.IsValid = ($(".pwd").val() == $(".pwd2").val());
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
        });
    </script>
</asp:Content>
