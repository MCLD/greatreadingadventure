<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="ProgramGameLevelList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.ProgramGameLevelList" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblPGID" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.ProgramGameLevel">
        <SelectParameters>
            <asp:ControlParameter ControlID="PGID" Name="PGID" DefaultValue="0"
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDPG" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.ProgramGame">
    </asp:ObjectDataSource>
<table>
<thead>
    <th colspan="7">
    </th>
</thead>
<tr>
<td><b>Game:</b> </td>
<td>
                <asp:DropDownList ID="PGID" runat="server" DataSourceID="odsDDPG" DataTextField="GameName" DataValueField="PGID" 
                    AppendDataBoundItems="True" Width="400px"
                 >
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
</td>
<td>
    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click" CssClass="btn-lg btn-green"/>
        &nbsp;
    <asp:Button ID="btnClear" runat="server" Text="Clear/All" onclick="btnClear_Click"  CssClass="btn-lg btn-blue"
         />
        &nbsp;
    <asp:Button ID="btnBoard" runat="server" Text="Edit Board Game"   
        CssClass="btn-lg btn-green" Visible="False" onclick="btnBoard_Click"
         />
</td>
</tr>
</table>    

<asp:Panel ID="pnlList" Visible="false" runat="server">
<h2><asp:Label runat="server" ID="GameName"></asp:Label></h2>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False" Width="100%"
        DataKeys="PGLID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("PGLID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("PGLID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;

                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" Tooltip="Move Up" 
                        CausesValidation="False" CommandName="MoveUp" CommandArgument='<%# Bind("PGLID") %>'  
                        ImageUrl="~/ControlRoom/Images/Up.gif"  Visible='<%# ((int)Eval("LevelNumber")==1 ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("LevelNumber")==1 ? true : false) %>' Width="21px"/>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" Tooltip="Move Down" 
                        CausesValidation="False" CommandName="MoveDn" CommandArgument='<%# Bind("PGLID") %>' 
                        ImageUrl="~/ControlRoom/Images/Dn.gif"   Visible='<%# ((int)Eval("LevelNumber")==(int)Eval("MAX") ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton2" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("LevelNumber")==(int)Eval("MAX") ? true : false) %>' Width="21px"/>
                   &nbsp;



                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="PGLID" DataField="PGLID" 
                SortExpression="PGLID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          


            <asp:TemplateField   SortExpression="PGID" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="PGID">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("PGID"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="LevelNumber" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="LevelNumber">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("LevelNumber"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="LocationX" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="X Location">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("LocationX"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="LocationY" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Y Location">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("LocationY"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

            <asp:TemplateField   SortExpression="PointNumber" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Points">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("PointNumber"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 
 			 

			<asp:BoundField ReadOnly="True" HeaderText="Modified On" 
                DataField="LastModDate" SortExpression="LastModDate" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Modified By" 
                DataField="LastModUser" SortExpression="LastModUser" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Added On" 
                DataField="AddedDate" SortExpression="AddedDate" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Added By" 
                DataField="AddedUser" SortExpression="AddedUser" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>			


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
</asp:Panel>
</asp:Content>

