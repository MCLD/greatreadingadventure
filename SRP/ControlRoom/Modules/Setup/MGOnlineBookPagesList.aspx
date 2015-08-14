<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGOnlineBookPagesList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGOnlineBookPagesList" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblOBID" runat="server" Text="" Visible="False"></asp:Label>

	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.MGOnlineBookPages">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblMGID" Name="MGID" DefaultValue="0"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label>
    </h1>
    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False" Width="100%"
        DataKeys="OBPGID"
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
                        ImageUrl="~/ControlRoom/Images/add.png" Width="24px" />

                        &nbsp;&nbsp;

                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("OBPGID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("OBPGID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                   &nbsp;

                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" Tooltip="Move Up" 
                        CausesValidation="False" CommandName="MoveUp" CommandArgument='<%# Bind("OBPGID") %>'  
                        ImageUrl="~/ControlRoom/Images/Up.gif"  Visible='<%# ((int)Eval("PageNumber")==1 ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("PageNumber")==1 ? true : false) %>' Width="21px"/>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" Tooltip="Move Down" 
                        CausesValidation="False" CommandName="MoveDn" CommandArgument='<%# Bind("OBPGID") %>' 
                        ImageUrl="~/ControlRoom/Images/Dn.gif"   Visible='<%# ((int)Eval("PageNumber")==(int)Eval("MAX") ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton2" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("PageNumber")==(int)Eval("MAX") ? true : false) %>' Width="21px"/>
                   &nbsp;

                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="75px"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="OBPGID" DataField="OBPGID" 
                SortExpression="OBPGID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          
 			 

            <asp:TemplateField   SortExpression="PageNumber" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Center" HeaderStyle-HorizontalAlign="Center" 
                HeaderText="Page #">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("PageNumber"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="50px" /> 
                <ItemStyle    Width="50px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

			<asp:BoundField ReadOnly="True" HeaderText="Text (Easy)" 
                DataField="TextEasy" SortExpression="TextEasy" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">

                <ItemStyle    VerticalAlign="Top" Wrap="True" Width="100%"></ItemStyle>
            </asp:BoundField> 

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# String.Format("~/Images/Games/Books/sm_{0}.png?{1}", Eval("OBPGID").ToString(), DateTime.Now.ToString()) %>'  width="24px"/>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="150px" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>






			<asp:BoundField ReadOnly="True" HeaderText="TextMedium" 
                DataField="TextMedium" SortExpression="TextMedium" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="TextHard" 
                DataField="TextHard" SortExpression="TextHard" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="AudioEasy" 
                DataField="AudioEasy" SortExpression="AudioEasy" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="AudioMedium" 
                DataField="AudioMedium" SortExpression="AudioMedium" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="AudioHard" 
                DataField="AudioHard" SortExpression="AudioHard" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 


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
                        ImageUrl="~/ControlRoom/Images/add.png" Width="24px" />
                        &nbsp;&nbsp;&nbsp;&nbsp;

                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />


            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

