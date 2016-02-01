<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Configure.aspx.cs" Theme="GRA" Inherits="GRA.SRP.ControlRoom.Configure" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, height=device-height, user-scalable=no, initial-scale=1, maximum-scale=1, minimum-scale=1">
    <title>Configure the Great Reading Adventure</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" runat="server" />
    <link href="~/Content/gra.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="mainForm" runat="server">
        <nav class="navbar navbar-default navbar-static-top">
            <div class="container">
                <div class="navbar-header">
                    <a class="navbar-brand" id="homeLink" href="/" runat="server">Great Reading Adventure</a>
                </div>
                <div class="nav navbar-nav navbar-right hidden-xs">
                    <p class="navbar-text" id="slogan"><em>Reimagining Summer Reading</em></p>
                </div>
            </div>
        </nav>
        <div class="container">
            <div class="row">
                <div class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-8 col-md-offset-2">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <span class="lead">Configure the Great Reading Adventure</span>
                        </div>
                        <div class="panel-body">
                            <div class="form-horizontal">

                                <asp:Panel runat="server" ID="step1" DefaultButton="NavigationNext">
                                    <div class="progress">
                                        <div class="progress-bar progress-bar-info progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 100%;">Progress: getting started</div>
                                    </div>
                                    <p class="margin-1em-bottom">The next few pages will guide you through the initial configuration of the Great Reading Adventure software.</p>

                                    <p>There are three steps:</p>
                                    <ul>
                                        <li>Configure <strong>database</strong> settings.</li>
                                        <li>Configure and test <strong>e-mail</strong> settings.</li>
                                        <li>Select a <strong>reading program configuration</strong>.</li>
                                    </ul>

                                    <p class="margin-1em-top">If you run into any issues, you can refer to the <strong><a href="http://manual.greatreadingadventure.com/" target="_blank">manual</a></strong> or post on the <strong><a href="http://forum.greatreadingadventure.com/" target="_blank">forum</a></strong> for assistance.</p>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="step2" Visible="false" DefaultButton="NavigationNext">
                                    <div class="progress">
                                        <div class="progress-bar progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 25%;">Step 1<span class="hidden-xs">: Database</span></div>
                                    </div>

                                    <div class="row margin-1em-top margin-1em-bottom">
                                        <div class="col-xs-12">
                                            <p>The first step is to configure the database settings. <strong>All of these values are required as indicated in red.</strong></p>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-8 col-sm-offset-2">
                                            <asp:Panel runat="server" ID="DatabaseIssuePanel" Visible="false" CssClass="alert alert-danger">
                                                <span class="glyphicon glyphicon-remove margin-halfem-right"></span>
                                                <asp:Label runat="server" ID="DatabaseIssueMessage"></asp:Label>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            Database server:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="DatabaseServer" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="DatabaseServer" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            Database/catalog name:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="DatabaseCatalog" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="DatabaseCatalog" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            DB owner user login:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="DatabaseOwnerUser" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="DatabaseOwnerUser" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            DB owner user password:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="DatabaseOwnerPassword" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="DatabaseOwnerPassword" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            DB regular user login:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="DatabaseUserUser" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="DatabaseUserUser" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            DB regular user password:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="DatabaseUserPassword" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="DatabaseUserPassword" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="step3" Visible="false" DefaultButton="NavigationNext">
                                    <div class="progress">
                                        <div class="progress-bar progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 50%;">Step 2<span class="hidden-xs">: Mail</span></div>
                                    </div>

                                    <div class="row margin-1em-top margin-1em-bottom">
                                        <div class="col-xs-12">
                                            <p>Step two is to configure the email settings. If you do not supply a mail server, mail will be queued and you will need mail server software to pick up the queued mail and deliver it. <strong>Required values are indicated in red.</strong></p>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-8 col-sm-offset-2">
                                            <asp:Panel runat="server" ID="MailIssuePanel" Visible="false" CssClass="alert alert-danger">
                                                <span class="glyphicon glyphicon-remove margin-halfem-right"></span>
                                                <asp:Label runat="server" ID="MailIssueMessage"></asp:Label>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-8 col-sm-offset-2">
                                            <asp:Panel runat="server" ID="MailSuccessPanel" Visible="false" CssClass="alert alert-success">
                                                <span class="glyphicon glyphicon-send margin-halfem-right"></span>
                                                Test email sent. Check your inbox to ensure it arrived.
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label text-danger">
                                            Your email address:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="MailAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1 col-xs-2 form-control-static">
                                            <asp:RequiredFieldValidator runat="server"
                                                ControlToValidate="MailAddress" Display="Dynamic" ErrorMessage=""
                                                SetFocusOnError="True"><span runat="server" class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label">
                                            Mail server:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="MailServer" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label">
                                            Mail server port:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="MailPort" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label">
                                            Mail server login:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="MailLogin" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-4 control-label">
                                            Mail server password:
                                        </label>
                                        <div class="col-sm-7 col-xs-10">
                                            <asp:TextBox ID="MailPassword" runat="server" TextMode="password" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-xs-8 col-xs-offset-2 col-sm-4 col-sm-offset-4">
                                            <button runat="server" onclick="testMessageShowStatus();" onserverclick="SendTestMail" class="btn btn-info btn-lg margin-1em-top">
                                                <span class="glyphicon glyphicon-send margin-halfem-right"></span>
                                                Send test message</button>
                                        </div>
                                    </div>

                                </asp:Panel>

                                <asp:Panel runat="server" ID="step4" Visible="false" DefaultButton="NavigationNext">
                                    <div class="progress">
                                        <div class="progress-bar progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 75%;">Step 3<span class="hidden-xs">: Reading Program</span></div>
                                    </div>

                                    <p>Finally, select your initial reading program configuration.</p>

                                    <p>Creating a <strong>single reading program</strong> is good if you:</p>
                                    <ul>
                                        <li>Are targeting a single age group</li>
                                        <li>Intend to have the same experience for all participants</li>
                                        <li>Aren't intending to require a literacy test for only some participants</li>
                                    </ul>

                                    <p><strong>Several age-specific reading programs</strong> are better if you:</p>
                                    <ul>
                                        <li>Want to report on age groups differently</li>
                                        <li>Intend to have Adventures, Badges, Challenges, and Events specific to age groups</li>
                                        <li>Want to allow readers to log their reading differently (e.g. some age groups by minute, some age groups by number of books read).</li>
                                    </ul>

                                    <p class="text-warning"><strong><em>Programs can be added or removed after the initial configuration process, this is just to get you started.</em></strong></p>

                                    <div class="row">
                                        <div class="col-sm-8 col-sm-offset-2">
                                            <select class="form-control input-lg margin-1em-top margin-1em-bottom" runat="server" id="ReadingProgram">
                                                <option id="one">A single reading program</option>
                                                <option id="four">Four age-specific reading programs</option>
                                            </select>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="step5" Visible="false" DefaultButton="NavigationNext">
                                    <div class="progress">
                                        <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 100%;">Confirm and go!</div>
                                    </div>

                                    <p style="font-size: larger;">Here's how you've configured the Great Reading Adventure:</p>
                                    <ul style="font-size: larger;">
                                        <li>Installing to the database named <strong>
                                            <asp:Label runat="server" ID="DatabaseNameLabel"></asp:Label></strong> on the server <strong>
                                                <asp:Label runat="server" ID="DatabaseServerLabel"></asp:Label></strong>.</li>
                                        <li>Sending mail through the <strong>
                                            <asp:Label runat="server" ID="MailServerLabel"></asp:Label></strong> server using <strong>
                                                <asp:Label runat="server" ID="MailAddressLabel"></asp:Label></strong> as the administrator email address.</li>
                                        <li>Setting up <strong>
                                            <asp:Label runat="server" ID="ReadingProgramLabel"></asp:Label></strong>.</li>
                                    </ul>
                                    <p style="font-size: larger;">If this looks good, click <span class="text-success">Next</span> to save this configuration and set everything up!</p>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="step6" Visible="false" DefaultButton="NavigationNext">
                                    <div class="progress">
                                        <div runat="server" id="FinalProgressBar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 100%;">
                                            <asp:Label ID="FinalProgressBarMessage" runat="server"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-8 col-sm-offset-2">
                                            <asp:Panel runat="server" ID="FinalIssuePanel" Visible="false" CssClass="alert alert-danger">
                                                <span class="glyphicon glyphicon-remove margin-halfem-right"></span>
                                                <asp:Label runat="server" ID="FinalIssueMessage"></asp:Label>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <asp:Panel runat="server" ID="FinalSuccessPanel" Visible="false">
                                        <div class="row">
                                            <div class="col-sm-8 col-sm-offset-2">
                                                <div class="alert alert-success">
                                                    <span class="glyphicon glyphicon-thumbs-up margin-halfem-right"></span>
                                                    Congratulations! You've successfully set up the Great Reading Adventure!
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-sm-10 col-sm-offset-1">
                                                <div class="lead margin-1em-bottom">Click next to proceed to the Control Room where you can continue configuring the Great Reading Adventure.</div>

                                                <p>The default login is: <strong class="lead"><code>sysadmin</code></strong> and the default password is: <strong class="lead"><code>changeme05!</code></strong></p>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </asp:Panel>

                            </div>
                        </div>
                        <div class="panel-footer clearfix">
                            <div class="pull-right">
                                <asp:Button ID="NavigationBack"
                                    runat="server"
                                    Text="< Back"
                                    CssClass="btn btn-default btn-lg margin-1em-right"
                                    Enabled="False"
                                    CausesValidation="False"
                                    OnClick="NavigationBackClick" />

                                <asp:Button ID="NavigationNext"
                                    runat="server"
                                    Text="Next >"
                                    CssClass="btn btn-success btn-lg next-button"
                                    OnClientClick="return navigationNextShowStatus();"
                                    OnClick="NavigationNextClick" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" style="padding-top: 15%;" id="processingConfiguration">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <span class="lead" id="processingConfigurationTitle">Validation</span>
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
                        <em><span id="processingConfigurationMessage">Validating entries...</span></em>
                    </div>
                </div>
            </div>
        </div>

        <asp:Label runat="server" Style="display: none;" ID="CurrentStep" CssClass="current-step"></asp:Label>
        <script src="<%=ResolveUrl("~/Scripts/jquery-2.2.0.min.js")%>"></script>
        <script src="<%=ResolveUrl("~/Scripts/bootstrap.min.js")%>"></script>
        <script>
            $(function () {
                $('[data-toggle="tooltip"]').tooltip()
            })
            $().ready(function () {
                $("#mainForm input:text, #mainForm textarea, #mainForm input:password").first().focus();
            })
            function testMessageShowStatus() {
                if (typeof Page_ClientValidate == "undefined") {
                    return true;
                }
                if (Page_ClientValidate()) {
                    $('#processingConfigurationMessage').text("Sending test email message...");
                    $('#processingConfiguration').modal({ backdrop: 'static' });
                    $('#processingConfigurationTitle').text("Validation");
                    return true;
                } else {
                    return false;
                }

            }
            function navigationNextShowStatus() {
                if (typeof Page_ClientValidate == "undefined") {
                    if ($('.current-step').text() == "5") {
                        $('#processingConfigurationMessage').text("Saving configuration to disk and the database...");
                        $('#processingConfiguration').modal({ backdrop: 'static' });
                        $('#processingConfigurationTitle').text("Configuration");
                    }
                    return true;
                }
                if (Page_ClientValidate()) {
                    if ($('.current-step').text() == "2") {
                        $('#processingConfigurationMessage').text("Validating database connection information...");
                        $('#processingConfiguration').modal({ backdrop: 'static' });
                        $('#processingConfigurationTitle').text("Validation");
                    }
                    return true;
                } else {
                    return false;
                }
            }
        </script>
    </form>
</body>
</html>
