<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" 
    CodeBehind="MGCodeBreakerKeySetup.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGCodeBreakerKeySetup" 
    
%>

<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblCBID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblDifficulty" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblSDifficulty" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblPrefix" runat="server" Text="" Visible="False"></asp:Label>

    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label></h1>
    <hr />
<asp:ImageButton ID="ImageButton1" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnBack_Click" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="ImageButton2" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"    Tooltip="Refresh Record"
                        AlternateText="Refresh Record" 
        onclick="btnRefresh_Click" /> 
                        &nbsp;
                        <hr />
    <asp:Repeater ID="rptr" runat="server">
        <ItemTemplate>
            <span style="padding-right: 30px; padding-left: 30px; border: 1px solid black; display:inline-block; ">
                <table width="230px">
                    <tr>
                        <td align="center">
                            <h1><%# Eval("Character")%></h1>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Key: <asp:Image ID="Image1" runat="server" ImageUrl='<%# String.Format("~/Images/Games/CodeBreaker/{0}.png?{1}", lblPrefix.Text + Eval("CBID") + "_" + Eval("Character_Num"), DateTime.Now.Ticks) %>' Width="24px" BorderColor="Black" BorderWidth="1px" BorderStyle="Solid"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:FileUploadCtl_1 ID="FileUploadCtl" runat="server" 
                                FileName='<%# lblPrefix.Text + Eval("CBID") + "_" + Eval("Character_Num") %>'
                                ImgWidth="36" 
                                CreateSmallThumbnail="False"         
                                CreateMediumThumbnail="False"
                                SmallThumbnailWidth="64" 
                                MediumThumbnailWidth="64"
                                Folder="~/Images/Games/CodeBreaker/"
                                Extension="png"
                            />
                        </td>
                    </tr>
                </table>
            </span>
        
        </ItemTemplate>
    </asp:Repeater>



<hr />
<asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnBack_Click" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"    Tooltip="Refresh Record"
                        AlternateText="Refresh Record" 
        onclick="btnRefresh_Click" /> 
                        &nbsp;

</asp:Content>
