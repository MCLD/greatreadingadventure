<%@ Page Title="Badge Gallery" Language="C#" AutoEventWireup="true" 
    MasterPageFile="~/Layout/SRP.Master" 
    CodeBehind="BadgeGallery.aspx.cs" Inherits="STG.SRP.BadgeGallery" %>
<%@ Import Namespace="STG.SRP.DAL" %>
<%@ Register src="~/Controls/ProgramTabs.ascx" tagname="ProgramTabs" tagprefix="uc1" %>
<%@ Register src="~/Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.bpopup.min.js"></script>


<style>
    #element_to_pop_up {display:none; }
    #element_to_pop_up {background-color:#fff;border-radius:10px 10px 10px 10px;box-shadow:0 0 25px 5px #999;color:#111;display:none;min-width:250px;padding:25px; 
                        width: 80%; min-width: 350px; max-width: 600px;
                        height: 80%; min-height: 500px; max-height: 600px;}                    
</style>              


<div id='element_to_pop_up'>
      <iframe id="the_iframe" src=""  frameborder="0" style="padding: 10px; margin: 10px; background-color: White; width: 95%; height:90%; "></iframe>
</div>

<script>
    function LoadBadgeDetail(BID) {
        $('#the_iframe').attr('src', 'BadgeDetail.aspx?BID=' + BID);
        $('#element_to_pop_up').bPopup();
    }
</script>




<uc1:ProgramTabs ID="ProgramTabs1" runat="server" />
<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />

<div class="row"> 
    <div class="span12"> 
        <h1 class="title-divider" style="text-align: center;">
            Badge Gallery    
        </h1>
        <table align="center" style="margin-top:-20px;">
        
            <tr><td colspan="5"> Filter badges ... </td></tr>
            <tr>
                <td>
                        <asp:DropDownList ID="CategoryId" runat="server" DataSourceID="odsDDCat" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True" Width="200px"  ondatabound="dd_DataBound"
                         >
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
                </td>
                <td>
                        <asp:DropDownList ID="AgeGroupId" runat="server" DataSourceID="odsDDAge" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True" Width="200px"  ondatabound="dd_DataBound"
                         >
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
                </td>
                <td>
                        <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True" Width="200px"  ondatabound="dd_DataBound"
                         >
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
                </td>
                <td>
                        <asp:DropDownList ID="LocationID" runat="server" DataSourceID="odsDDLoc" 
                            DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True" Width="200px" ondatabound="dd_DataBound"
                         >
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
                </td>
                <td valign="top" style="padding-left:5px;">
                    <asp:Button ID="btnFilter" runat="server" Text="Events btn Filter" 
                        CssClass="btn e" onclick="btnFilter_Click"/>
                </td>
            </tr>   
                           
        </table>
        <hr />
    </div>
</div>                        
<div class="row">
    <div class="span12"> 
        
        <asp:Repeater runat="server" ID="rptr"  DataSourceID="odsBadges">
        <ItemTemplate>
            <div style="text-align: center; padding: 10px; display: inline-block; border: 1px solid silver; width: 230px; margin: 10px; height: 230px; vertical-align: top;">
                <div style="height:75px;">
                    <h4><%# Eval("Name") %></h4>
                </div>
                <img src='/images/badges/sm_<%# Eval("BID") %>.png' /> 
                <br />
                <a id='A<%# Eval("BID") %>' href="javascript: LoadBadgeDetail('<%# Eval("BID") %>')" > More Info ... </a>
            </div>
        </ItemTemplate>
        </asp:Repeater>
    </div>
</div>   

<asp:ObjectDataSource ID="odsBadges" runat="server" 
    SelectMethod="GetForGallery" 
    TypeName="STG.SRP.DAL.Badge">
    <SelectParameters>
        <asp:ControlParameter ControlID="AgeGroupId" DefaultValue="0" Name="Age" PropertyName="SelectedValue" Type="Int32" />
        <asp:ControlParameter ControlID="BranchId" DefaultValue="0" Name="Branch" PropertyName="SelectedValue" Type="Int32" />
        <asp:ControlParameter ControlID="CategoryId" DefaultValue="0" Name="Category" PropertyName="SelectedValue" Type="Int32" />
        <asp:ControlParameter ControlID="LocationID" DefaultValue="0" Name="Location" PropertyName="SelectedValue" Type="Int32" />
    </SelectParameters>

</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsDDCat" runat="server" 
    SelectMethod="GetAlByTypeName" 
    TypeName="STG.SRP.DAL.Codes">
    <SelectParameters>
        <asp:Parameter Name="Name" DefaultValue = "Badge Category" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsDDAge" runat="server" 
    SelectMethod="GetAlByTypeName" 
    TypeName="STG.SRP.DAL.Codes">
    <SelectParameters>
        <asp:Parameter Name="Name" DefaultValue = "Badge Age Group" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsDDBranch" runat="server" 
    SelectMethod="GetAlByTypeName" 
    TypeName="STG.SRP.DAL.Codes">
    <SelectParameters>
        <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsDDLoc" runat="server" 
    SelectMethod="GetAlByTypeName" 
    TypeName="STG.SRP.DAL.Codes">
    <SelectParameters>
        <asp:Parameter Name="Name" DefaultValue = "Badge Location" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>





</ContentTemplate>
</asp:UpdatePanel>

</asp:Content>
