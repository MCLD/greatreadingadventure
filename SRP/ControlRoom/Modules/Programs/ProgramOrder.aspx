<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="ProgramOrder.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Programs.ProgramOrder" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAllOrdered" 
        TypeName="GRA.SRP.DAL.Programs">
	</asp:ObjectDataSource>


    <asp:GridView ID="gv" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="PID"
        DataSourceID="odsData"
        onrowcommand="GvRowCommand"      
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" Tooltip="Move Up" 
                        CausesValidation="False" CommandName="MoveUp" CommandArgument='<%# Bind("PID") %>'  
                        ImageUrl="~/ControlRoom/Images/Up.gif"  Visible='<%# ((int)Eval("POrder")==1 ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("POrder")==1 ? true : false) %>' Width="21px"/>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" Tooltip="Move Down" 
                        CausesValidation="False" CommandName="MoveDn" CommandArgument='<%# Bind("PID") %>' 
                        ImageUrl="~/ControlRoom/Images/Dn.gif"   Visible='<%# ((int)Eval("POrder")==(int)Eval("MAX") ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton2" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("POrder")==(int)Eval("MAX") ? true : false) %>' Width="21px"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="PID" DataField="PID" 
                SortExpression="PID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          


			<asp:BoundField ReadOnly="True" HeaderText="Program Name" 
                DataField="AdminName" SortExpression="AdminName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Title" 
                DataField="Title" SortExpression="Title" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                 <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

            <asp:TemplateField   SortExpression="IsActive" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Is Active?" >               
                <ItemTemplate>
                    <%# FormatHelper.ToYesNo((bool)Eval("IsActive"))%>
                </ItemTemplate>
				 <ControlStyle Width="250px" />
                 <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   SortExpression="IsHidden" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left"
                 HeaderText="Is Hidden?">              
                <ItemTemplate>
                    <%# FormatHelper.ToYesNo((bool)Eval("IsHidden"))%>
                </ItemTemplate>
				 <ControlStyle Width="250px" />
                 <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:TemplateField   SortExpression="StartDate" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Start Date">            
                <ItemTemplate>
                    <%# FormatHelper.ToNormalDate((DateTime)Eval("StartDate"))%>
                </ItemTemplate>
				 <ControlStyle Width="250px" />
                 <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   SortExpression="EndDate" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="End Date">             
                <ItemTemplate>
                    <%# FormatHelper.ToNormalDate((DateTime)Eval("EndDate"))%>
                </ItemTemplate>
				 <ControlStyle Width="250px" />
                 <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>	
            
            
        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. 
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

