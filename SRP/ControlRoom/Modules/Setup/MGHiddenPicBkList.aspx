<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGHiddenPicBkList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGHiddenPicBkList" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblHPID" runat="server" Text="" Visible="False"></asp:Label>

    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label></h1>

	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.MGHiddenPicBk">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblMGID" Name="MGID" DefaultValue="0"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="HPBID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"   
        width="100%"   
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
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("HPBID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("HPBID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="HPBID" DataField="HPBID" 
                SortExpression="HPBID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          


            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                        Background/Coloring Page Thumbnail
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server"  width="24px"
                    ImageUrl='<%# String.Format("~/Images/Games/HiddenPic/sm_{0}.png?{1}", Eval("HPBID").ToString(), DateTime.Now.ToString()) %>' />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
                <HeaderStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left" />
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

