<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ControlRoom/Control.Master" 
    CodeBehind="BulkNotification.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Notifications.BulkNotification" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%"  style="border:double 3px #A3C0E8;">
<tr>
    <th colspan="2"><b>Program</b></th>
    <th><b>Library System</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" 
            DataTextField="AdminName" DataValueField="PID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="LibSys" runat="server" DataSourceID="odsDDLibSys" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <th colspan="2"><b>Library/Branch</b></th>
    <th><b>School</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="School" runat="server" DataSourceID="odsDDSchool" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>

<tr>
<td colspan="2"> 

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    
</td>
<td>&nbsp;</td>
</tr>
    
</table>
    <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Programs">
    </asp:ObjectDataSource>
   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:ObjectDataSource ID="odsDDLibSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>


    <asp:Panel ID="pnlResults" runat="server" Visible="False">
        <br />
        <asp:Label ID="lblCount" runat="server" Font-Size="Large" ForeColor="#CC0000"></asp:Label>
        <asp:Label ID="lblSent" runat="server" Font-Size="Medium" ForeColor="#009933"></asp:Label>
        <br />
        <br />
        <table width="100%">
            <tr>
                <td width="50px"><b>Subject: </b></td>
                <td>
                    <asp:TextBox ID="txtSubject" runat="server" Width="99%"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2" height="350px">
                    <b>Message: </b><br />
                    <CKEditor:CKEditorControl ID="Body" 
                            BasePath="/ckeditor/" 
                            runat="server" 
                            Skin="office2003" 
                            BodyId="wrapper" 
                            ContentsCss="/css/EditorStyles.css"
                            DisableNativeSpellChecker="False" 
                            DisableNativeTableHandles="False" 
                            DocType="&lt;!DOCTYPE html&gt;" 
                            ForcePasteAsPlainText="True" 
                            Height="250px" UIColor="#D3D3D3" 
                            Visible="True" 
                            Width="99%"
                            Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                            / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                            / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                            / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                            "
                            AutoGrowOnStartup="True" 
                            ></CKEditor:CKEditorControl>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
        <asp:Button ID="btnSend" runat="server" CSSClass="btn-lg btn-green" 
            onclick="btnSend_Click" Text="Send Message" Width="150px" />
        <br />
        <br />
        
    </asp:Panel>
    <br />




</asp:Content>