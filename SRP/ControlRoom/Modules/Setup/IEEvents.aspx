<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="IEEvents.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.IEEvents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td>Import from Excel: 
            </td>
            <td>
                <asp:FileUpload runat="server" ID="ExcelFileUpload" />
            </td>
            <td>
                <asp:LinkButton ID="UploadTestButton" ForeColor="White"
                    OnClick="UploadTest_Click" CssClass="btn btn-xs btn-warning"
                    runat="server"><span class="glyphicon glyphicon-upload"></span> Upload and test Excel file
                </asp:LinkButton>
                <asp:LinkButton ID="UploadButton" ForeColor="White"
                    OnClick="UploadButton_Click" CssClass="btn btn-xs btn-info"
                    runat="server"><span class="glyphicon glyphicon-upload"></span> Upload and import Excel file
                </asp:LinkButton>
                <asp:LinkButton ID="DownloadButton" ForeColor="White"
                    OnClick="DownloadButton_Click" CssClass="btn btn-xs btn-success"
                    runat="server"><span class="glyphicon glyphicon-download"></span> Download Excel file
                </asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
