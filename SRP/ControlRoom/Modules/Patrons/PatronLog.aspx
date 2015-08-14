<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
    CodeBehind="PatronLog.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronLog" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<%@ Register src="~/ControlRoom/Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <uc1:PatronContext ID="pcCtl" runat="server" />


    <asp:GridView ID="gv1" runat="server"  
        AutoGenerateColumns="False" 
        AllowSorting="False"
        AllowPaging="False"
        Width="100%"
        OnRowCommand="GvRowCommand"
        >

        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" HeaderStyle-HorizontalAlign="Center" >
                <HeaderTemplate>
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>    
                            
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("PPID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("PPID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField  Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-HorizontalAlign ="Right" HeaderStyle-HorizontalAlign="Right" 
                HeaderStyle-Wrap="False"
                HeaderText="# Points Earned">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("NumPoints")) %> 
                </ItemTemplate> 
                <ItemStyle Wrap="False" HorizontalAlign="Right"></ItemStyle>
            </asp:TemplateField>	 

            <asp:TemplateField  Visible="True"  
                ItemStyle-Wrap="False"  ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderStyle-Wrap="False"
                HeaderText="Reason Points Were Awarded">              
                <ItemTemplate> 
                    
                    <%# Eval("AwardReason") %> 
                    <%# ((bool)Eval("isEvent")  ? " (Code: " + Eval("EventCode") + ")" : "")%>

                </ItemTemplate> 
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>	 

            <asp:TemplateField  Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderStyle-Wrap="False"
                HeaderText="Date Awarded">              
                <ItemTemplate> 
                    <%# FormatHelper.ToNormalDate((DateTime)Eval("AwardDate")) %>  
                </ItemTemplate> 
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="100%"></ItemStyle>
            </asp:TemplateField>	 


        </Columns> 
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>



</asp:Content>
    
