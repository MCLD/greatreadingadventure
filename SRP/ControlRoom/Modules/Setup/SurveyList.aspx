<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="SurveyList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyList" 

%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Survey">
	</asp:ObjectDataSource>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="SID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    &nbsp;&nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnResults" runat="server" AlternateText="Results" Tooltip="Results" 
                        CausesValidation="False" CommandName="Results" CommandArgument='-1'  
                        ImageUrl="~/ControlRoom/Images/Result.png" Width="20px" /> 
                    &nbsp;
                    <asp:ImageButton ID="ImageButton1" runat="server" AlternateText="Generate Embedding Code" Tooltip="Generate Embedding Code" 
                        CausesValidation="False" CommandName="embed" CommandArgument='-1'  
                        ImageUrl="~/ControlRoom/Images/EmbedTestRecipe.png" Width="20px" />     
                    &nbsp;&nbsp;&nbsp;


                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("SID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="ImageButton1" runat="server" AlternateText="Clone" Tooltip="Clone" 
                        CausesValidation="False" CommandName="Clone" CommandArgument='<%# Bind("SID") %>'  
                        ImageUrl="~/ControlRoom/Images/clone.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnQuestions" runat="server" AlternateText="Questions" Tooltip="Questions" 
                        CausesValidation="False" CommandName="Questions" CommandArgument='<%# Bind("SID") %>'  
                        ImageUrl="~/ControlRoom/Images/Questions.png" Width="20px" />
  
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("SID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?\n\nThis will delete all questions, answers, statistics, and any EVERYTHING else related to this test/survey.');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="SID" DataField="SID" 
                SortExpression="SID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          

			<asp:BoundField ReadOnly="True" HeaderText="Name" 
                DataField="Name" SortExpression="Name" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Long Name" 
                DataField="LongName" SortExpression="LongName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

            <asp:TemplateField   SortExpression="Status" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Status">              
                <ItemTemplate> 
                    <%# Survey.StatusDescription((int)Eval("Status"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="TakenCount" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Taken Count">              
                <ItemTemplate> 
                    <%# ((int)Eval("TakenCount")).ToInt()%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="PatronCount" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Patron Count">              
                <ItemTemplate> 
                    <%# ((int)Eval("PatronCount")).ToInt()%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="CanBeScored" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="CanBeScored">              
                <ItemTemplate> 
                    <%# ((bool)Eval("CanBeScored")).ToYesNo()%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            		


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

