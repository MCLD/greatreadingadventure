<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="PatronSurveys.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronSurveys" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="../../Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>

<%@ Register src="../../Controls/PatronLitTestsCtl.ascx" tagname="PatronLitTestsCtl" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronContext ID="pcCtl" runat="server" />
    <br />
    <uc2:PatronLitTestsCtl ID="PatronLitTestsCtl1" runat="server" />
    <br />

    <asp:GridView ID="gv" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
        onrowcommand="GvRowCommand"     
        width="100%" 
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="View" Tooltip="View" 
                        CausesValidation="False" CommandName="ViewRecord" CommandArgument='<%# Bind("SRID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    
                    &nbsp;

                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField> 	 			 

            <asp:TemplateField   SortExpression="Source" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Test Source">              
                <ItemTemplate> 
                    <%# Eval("Source")%> <%# Eval("SourceName")%>  
                </ItemTemplate> 
				 <ControlStyle  /> 
                <ItemStyle     VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="Name" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Test Name">              
                <ItemTemplate> 
                    <%# Eval("Name")%> 
                </ItemTemplate> 
				 <ControlStyle  /> 
                <ItemStyle     VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	
            
            <asp:TemplateField   SortExpression="isComplete?" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Test Name">              
                <ItemTemplate> 
                    <%# ((bool)Eval("IsComplete")).ToYesNo() %> 
                </ItemTemplate> 
				 <ControlStyle  /> 
                <ItemStyle     VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	

            <asp:TemplateField   SortExpression="Score" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Raw Score">              
                <ItemTemplate> 
                    <%# ((bool)Eval("IsComplete")) && ((bool)Eval("IsScorable")) ?  ((int)Eval("Score")).ToWidgetDisplayInt() : "" %> 
                </ItemTemplate> 
				 <ControlStyle  /> 
                <ItemStyle     VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	


            <asp:TemplateField   SortExpression="Score" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Score %">              
                <ItemTemplate> 
                    <%# ((bool)Eval("IsComplete")) && ((bool)Eval("IsScorable")) ?  string.Format("{0:0}%",Eval("ScorePct")) : "" %> 
                </ItemTemplate> 
				 <ControlStyle  /> 
                <ItemStyle     VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

