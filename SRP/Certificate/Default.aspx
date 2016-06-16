<%@ Page Title="Certificate"
    Language="C#"
    MasterPageFile="~/Layout/SRP.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="GRA.SRP.Certificate.Default" %>

<asp:Content ContentPlaceHolderID="HeaderContent" runat="server">
    <style>
        @media print {
            @page {
                size: landscape;
            }
        }

        .certificate {
            font-size: 1.4em;
            margin-bottom: 2em;
        }

        .certificate-program {
            letter-spacing: 0.05em;
            font-size: 1.4em;
        }

        .certificate-award {
            letter-spacing: 0.05em;
            font-size: 1.8em;
            display: inline-block;
            margin-bottom: 0.5em;
        }

        .certificate-recipient {
            font-size: 1.7em;
            color: #307fe2 !important;
        }

        .certificate-progress {
            height: 2em;
            vertical-align: middle;
        }

        .certificate-footer-image {
            margin-top: 1.2em;
        }

        .certificate-progress-bar {
            font-size: 1.5em;
            padding-top: 0.3em;
        }
    </style>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
    <div class="row certificate">
        <% if (!this.Achiever)
            { %>
        <div class="col-xs-12 col-sm-6 col-sm-offset-3 text-center margin-1em-bottom">
            <p class="lead"><%=Explanation %></p>
        </div>
        <div class="col-xs-12 col-sm-10 col-sm-offset-1 text-center hidden-print margin-1em-bottom">
            <div class="progress certificate-progress">
                <div class="progress-bar progress-bar-primary progress-bar-striped certificate-progress-bar"
                    role="progressbar"
                    aria-valuenow="20"
                    aria-valuemin="0"
                    aria-valuemax="100"
                    runat="server"
                    id="progressBar"
                    style="min-width: 2.5em;">
                    <%=Status %>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-sm-offset-4 text-center hidden-print">
            <a href=".." class="btn btn-lg btn-success btn-block">
                <asp:Literal runat="server" Text="certificate-keep-reading-button"></asp:Literal></a>
        </div>
        <% }
            else
            { %>
        <div class="col-xs-12 text-center">
            <img src="~/images/gra150.png"
                runat="server"
                ID="CertificateLogo"
                style="width: 150px; max-height: 150px;"/>
            <p class="certificate-program">
                <asp:Literal runat="server">certificate-program</asp:Literal>
            </p>
            <p class="certificate-award">
                <asp:Literal runat="server">certificate-award</asp:Literal>
            </p>
            <p><small>This certificate is being awarded to</small></p>
            <p class="certificate-recipient"><% =string.Format("{0} {1}", Patron.FirstName, Patron.LastName) %></p>
            <p><small>in recognition of outstanding achievement in the</small></p>
            <p>
                <small>
                    <asp:Literal runat="server">certificate-summary</asp:Literal></small>
            </p>
            <img src="~/images/Banners/1.png"
                runat="server"
                ID="CertificateFooterImage"
                style="max-height: 240px;"
                class="img-responsive visible-print-block certificate-footer-image" />
        </div>
        <% } %>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="BottomOfPage" runat="server">
</asp:Content>
