<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilyCodeControl.ascx.cs" Inherits="GRA.SRP.Controls.FamilyCodeControl" %>
<asp:Panel runat="server" ID="familyCodeControlPanel" DefaultButton="submitButton">
    <div class="row">
        <div class="col-xs-12 text-center">
            <span class="lead">
                <asp:Label runat="server" Text="family-secret-code-header"></asp:Label></span>
            <br />
            Logging activity for: <strong>
                <asp:Label runat="server" ID="lblAccount"></asp:Label></strong>
            <hr style="margin-bottom: 5px !important; margin-top: 5px !important;" />
        </div>
    </div>
    <div class="row">
        <div class="form-inline text-center">
            <div class="col-xs-12" style="display: none;" id="codeControlMessageError">
                <p class="text-danger" style="font-weight: bold;">Please enter a code.</p>
            </div>
            <div class="col-xs-12">
                <div class="form-group has-feedback" id="codeControlFormGroup">
                    <label for="codeEntryField" runat="server" cssclass="code-spacing">
                        <asp:Label runat="server" Text="family-secret-code-prompt"></asp:Label></label>
                    <div class="block-center" style="display: inline-block;">
                        <asp:TextBox ID="codeEntryField"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="50"
                            Width="10em"></asp:TextBox>
                    </div>
                    <asp:LinkButton runat="server"
                        CssClass="btn btn-default btn-sm btn-success code-submit"
                        data-loading-text="Submitting..."
                        Text="family-secret-code-submit"
                        ID="submitButton"
                        OnClientClick="return ValidateCodeEntry();"
                        OnClick="submitButton_Click"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<script>
    function ValidateCodeEntry() {
        $('.code-submit').button('loading');
        var elems = $("input[id$='codeEntryField']");
        var valid = false;
        elems.each(function () {
            if (!valid && $(this).val().length > 0) {
                valid = true;
                return false;
            }
        });

        if (valid) {
            if ($('#codeControlFormGroup').hasClass('has-error')) {
                $('#codeControlFormGroup').removeClass('has-error');
            }
            $('#codeControlMessageError').hide();

        } else {
            if (!$('#codeControlFormGroup').hasClass('has-error')) {
                $('#codeControlFormGroup').addClass('has-error');
            }
            $('#codeControlMessageError').show();

        }
        if (!valid) {
            $('.code-submit').button('reset');
        }
        return valid;
    }
</script>
