<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" CodeBehind="SRPSettings.aspx.cs" 
Inherits="GRA.SRP.ControlRoom.Modules.Settings.SRPSettings" 
    validateRequest="false" 
%>
<%@ Register src="~/ControlRoom/Controls/SettingEditor.ascx" tagname="SettingEditor" tagprefix="uc1" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script language="javascript" type="text/javascript">
    function flipHelp(id) {
        var tr = document.getElementById("help_" + id)
        if (tr != null) {
            if (tr.style.visibility == "hidden") tr.style.visibility= string.Empty
            else tr.style.visibility = "hidden"
            if (tr.style.display == "none") tr.style.display= string.Empty
            else tr.style.display = "none"
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ObjectDataSource ID="odsSRPSettings" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.SRPSettings">
        <SelectParameters>
            <asp:Parameter DefaultValue="true" Name="forCurrentTenantOnly" Type="Boolean" />
        </SelectParameters>

    </asp:ObjectDataSource>


    <asp:Label ID="lblMID" runat="server" Text="-1" Visible="False"></asp:Label>
    <asp:Label ID="lblModName" runat="server" Text="Summer Reading Program Settings" Visible="False"></asp:Label>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="SID"
        DataSourceID="odsSRPSettings"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >
        <Columns>
           <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                
                    <asp:ImageButton ID="btnSave" runat="server" AlternateText="Save All" 
                        CausesValidation="False" CommandName="Save" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/save.png" Width="20px" />
                                        
                </HeaderTemplate>                
                <ItemTemplate>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField  HeaderText="Name" SortExpression="Name" 
                ItemStyle-VerticalAlign="Top">
               
                <ItemTemplate>
                
<table width="100%" border=0>

<tr>
    <td valign="top" align="left" width="170px" style="width: 170px; ">
        <div style="width: 170px; "><b><%# Eval("Label") %>: </b></div>
    </td>
    <td width="950px">
        
            <uc1:SettingEditor ID="uxSettingEditor" runat="server" 
                 SID='<%# Eval("SID") %>'
                 ValueList='<%# Eval("ValueList") %>'
                 Default='<%# Eval("DefaultValue") %>'
               StorageType='<%# Eval("StorageType") %>'
               EditType='<%# Eval("EditType") %>'
                 Value='<%# Eval("Value") %>'
             />

           <!-- <%# Eval("Value") %> -->
    
    </td>
    <td width="50px" valign="top" >
        &nbsp;
        <img alt="Help Info" src="/ControlRoom/Images/info.png" Width="20px" onclick="flipHelp(<%# Eval("SID") %>)"/>
        &nbsp;
    </td>
</tr>
<tr id="help_<%# Eval("SID") %>" style="display: none; visibility:hidden;">
    <td></td>
    <td colspan="2">
        <%# Eval("Description") %>
    </td>
</tr>
</table>                
                
                


                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>            


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found.
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <br />
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel" 
                        AlternateText="Back/Cancel" onclick="btnBack_Click" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"  
                        AlternateText="Refresh Record" 
        onclick="btnRefresh_Click" />   
                        &nbsp;
                        &nbsp;
                        <asp:ImageButton ID="btnSave" runat="server" AlternateText="Save All" 
                        CausesValidation="False" CommandName="Save" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/save.png" Width="20px" 
        onclick="btnSave_Click" />
</asp:Content>


