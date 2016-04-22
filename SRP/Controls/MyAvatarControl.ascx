<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyAvatarControl.ascx.cs" Inherits="GRA.SRP.Controls.MyAvatarControl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<div class="row">
    <div class="col-sm-12 hidden-print">
        <span class="h1">
            <asp:Label ID="Label1" runat="server" Text="avatar-title"></asp:Label></span>
    </div>
</div>

<asp:Panel ID="pnlList" runat="server" Visible="true">


    <div class="row clearfix">
        <div class="col-sm-8 col-sm-offset-2 clearfix">

            <ul class="avatar-btn-group" style="float: left;">
                <li>
                    <button class="btn btn-primary btn-lg avatar-layer-btn avatar-layer-btn-left" type="button" data-component="2">&lt;</button>
                </li>
                <li>
                    <button class="btn btn-primary btn-lg avatar-layer-btn avatar-layer-btn-left" type="button" data-component="1">&lt;</button>
                </li>
                <li>
                    <button class="btn btn-primary btn-lg avatar-layer-btn avatar-layer-btn-left" type="button" data-component="0">&lt;</button>
                </li>
            </ul>

            <ul class="avatar-btn-group" style="float: right;">
                <li>
                    <button class="btn btn-primary btn-lg avatar-layer-btn avatar-layer-btn-right" type="button" data-component="2">&gt;</button>
                </li>
                <li>
                    <button class="btn btn-primary btn-lg avatar-layer-btn avatar-layer-btn-right" type="button" data-component="1">&gt;</button>
                </li>
                <li>
                    <button class="btn btn-primary btn-lg avatar-layer-btn avatar-layer-btn-right" type="button" data-component="0">&gt;</button>
                </li>
            </ul>


            <div style="max-width: 280px; max-height: 400px; margin-left: auto; margin-right: auto; position: relative;">
                <asp:HiddenField ID='componentState0' runat="server" />
                <img id="componentImg0" class="avatar-layer" src="/images/AvatarCache/no_avatar.png" />

                <asp:HiddenField ID='componentState1' runat="server" />
                <img id="componentImg1" class="avatar-layer" src="/images/AvatarCache/no_avatar.png" />

                <asp:HiddenField ID='componentState2' runat="server" />
                <img id="componentImg2" class="avatar-layer" src="/images/AvatarCache/no_avatar.png" />
            </div>

        </div>
    </div>

    <div class="row">
        <div class="col-xs-10 col-xs-offset-1 col-sm-8 col-sm-offset-2 text-center margin-1em-bottom">
            <asp:Label runat="server" Text="avatar-description"></asp:Label>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-8 col-sm-offset-2">
            <div class="pull-right">
                <asp:LinkButton runat="server"
                    CommandName="save"
                    OnCommand="SaveShareButton_Command"
                    data-loading-text="Saving..."
                    OnClientClick="return saveButtonClick();"
                    CssClass="btn btn-success avatar-save-button">
                        <span class="glyphicon glyphicon-share margin-halfem-right"></span>
                        Save and share this
                </asp:LinkButton>

                <asp:LinkButton runat="server"
                    CommandName="save"
                    OnCommand="SaveButton_Command"
                    data-loading-text="Saving..."
                    OnClientClick="return saveButtonClick();"
                    CssClass="btn btn-success avatar-save-button">
                        <span class="glyphicon glyphicon-save margin-halfem-right"></span>
                        Save
                </asp:LinkButton>
            </div>
        </div>

    </div>
    <script type="text/javascript">
        var ASP_avatar_components = JSON.parse('<%= json.Serialize(jsAvatarComponents) %>');

        var ASP_avatar_fields = {
            /* [ComponentdID]: [Field] */
            "0": "<%=componentState0.ClientID %>",
            "1": "<%=componentState1.ClientID %>",
            "2": "<%=componentState2.ClientID %>"
        };

        function saveButtonClick() {
            $('.avatar-save-button').button('loading');
            return true;
        }



        var components = new Array();

        function ComponentState() {
            this.componentID = 0;
            this.index = 0;
            this.partIDs = [];

            this.$stateField = null;
            this.$imgField = null;

            this.update = function () {
                var partID = this.partIDs[this.index];

                this.$stateField.val(partID);

                var imageUrl = "/images/AvatarParts/" + partID;
                imageUrl += ".png";

                this.$imgField.attr("src", imageUrl);
            }
        }

        function updateButtonStates() {
            $(".avatar-layer-btn-right").each(function (index) {
                var $button = $(this);

                var component = components[$button.data("component")];

                if (component.index + 1 == component.partIDs.length) {
                    $button.prop('disabled', true);
                } else {
                    $button.prop('disabled', false);
                }
            });

            $(".avatar-layer-btn-left").each(function (index) {
                var $button = $(this);

                var component = components[$button.data("component")];

                if (component.index == 0) {
                    $button.prop('disabled', true);
                } else {
                    $button.prop('disabled', false);
                }
            });
        }


        $(document).ready(function () {
            var i = 0;

            for (var key in ASP_avatar_fields) {

                var component = new ComponentState();
                component.componentID = parseInt(key);

                component.$stateField = $("#" + ASP_avatar_fields[key]);
                component.$imgField = $("#componentImg" + key);

                component.partIDs = ASP_avatar_components[key];


                var selectedPartID = -1;

                if (component.$stateField.val().length != 0) {
                    selectedPartID = parseInt(component.$stateField.val());
                }

                var index = component.partIDs.indexOf(selectedPartID);

                if (index != -1) {
                    component.index = index;
                } else {
                    component.index = 0;
                }


                component.update();
                components[key] = component;
                i += 1;
            }

            updateButtonStates();



            $(".avatar-layer-btn-right").click(function (event) {
                var $target = $(event.target);
                var component = components[$target.data("component")];

                if (component.index + 1 < component.partIDs.length) {
                    component.index += 1;
                }

                component.update();
                updateButtonStates();
            });

            $(".avatar-layer-btn-left").click(function (event) {
                var $target = $(event.target);
                var component = components[$target.data("component")];

                if (component.index > 0) {
                    component.index -= 1;
                }

                component.update();
                updateButtonStates();
            });


        });
    </script>

</asp:Panel>
