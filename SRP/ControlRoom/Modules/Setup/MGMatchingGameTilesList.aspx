<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGMatchingGameTilesList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGMatchingGameTilesList" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblMAGID" runat="server" Text="" Visible="False"></asp:Label>

    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label>
    </h1>


	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.MGMatchingGameTiles">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblMGID" Name="MGID" DefaultValue="0"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>



    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        DataKeys="MAGTID"
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
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("MAGTID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("MAGTID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="MAGTID" DataField="MAGTID" 
                SortExpression="MAGTID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          
			 

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                        <center>Tile 1</center>
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# String.Format("~/Images/Games/MatchingGame/sm_t1_{0}.png?{1}", Eval("MAGTID").ToString(), DateTime.Now.ToString()) %>' width="24px"/>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="300px" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                        <center>Tile 2</center>
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Image ID="Image2" runat="server" ImageUrl='<%# String.Format("~/Images/Games/MatchingGame/sm_t2_{0}.png?{1}", Eval("MAGTID").ToString(), DateTime.Now.ToString()) %>'  width="24px"/>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="300px" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>

 
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                        <center>Tile 3</center>
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Image ID="Image3" runat="server" ImageUrl='<%# String.Format("~/Images/Games/MatchingGame/sm_t3_{0}.png?{1}", Eval("MAGTID").ToString(), DateTime.Now.ToString()) %>'  width="24px"/>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="300px" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>           		


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

