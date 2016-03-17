<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="AvatarAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.AvatarAddEdit"
	 %>

<%@ Register src="~/Controls/FileUploadCtl.ascx" tagname="FileUploadCtl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<meta Http-Equiv="Cache-Control" Content="no-cache">
<meta Http-Equiv="Pragma" Content="no-cache">
<meta Http-Equiv="Expires" Content="0">
<meta Http-Equiv="Pragma-directive: no-cache">
<meta Http-Equiv="Cache-directive: no-cache">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData" 
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound" Width="100%">
        <Fields>

        <asp:BoundField DataField="AID" HeaderText="Avatar Id: " SortExpression="APID" ReadOnly="True" InsertVisible="False" Visible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top"/>
        </asp:BoundField>

        <asp:TemplateField HeaderText="Avatar Name: " SortExpression="Name" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="Name" runat="server" Text='<%# Bind("Name") %>' ReadOnly="False" Width="400px"  CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                    ControlToValidate="Name" Display="Dynamic" ErrorMessage="Name is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>

                <b>Component/Layer:</b>

                <asp:DropDownList ID="ComponentID" runat="server"  Width="500px" SelectedValue='<%# Eval("ComponentID") %>' CssClass="form-control">
                     <asp:ListItem Value="0" Text="0 (Body)"></asp:ListItem>
                     <asp:ListItem Value="1" Text="1 (Arms)"></asp:ListItem>
                     <asp:ListItem Value="2" Text="2 (Head)"></asp:ListItem>
                 </asp:DropDownList>

                 <b>Unlock with badge:</b>

                 <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="500px"  SelectedValue='<%# Eval("BadgeID") %>' CssClass="form-control">
                                    <asp:ListItem Value="-1" Text="[Select a Badge]"></asp:ListItem>
                 </asp:DropDownList>

            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="Name" runat="server" Text='<%# Bind("Name") %>' Width="400px"  CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                    ControlToValidate="Name" Display="Dynamic" ErrorMessage="Name is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>

                
                <b>Component/Layer:</b>

                <asp:DropDownList ID="ComponentID" runat="server"  Width="500px" CssClass="form-control">
                     <asp:ListItem Value="0" Text="0 (Body)"></asp:ListItem>
                     <asp:ListItem Value="1" Text="1 (Arms)"></asp:ListItem>
                     <asp:ListItem Value="2" Text="2 (Head)"></asp:ListItem>
                 </asp:DropDownList>

                 <b>Unlock with badge:</b>

                 <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="500px" CssClass="form-control">
                                    <asp:ListItem Value="-1" Text="[Select a Badge]"></asp:ListItem>
                 </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Name" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

       
        <asp:TemplateField InsertVisible="False" ShowHeader="False"  HeaderText="Image: " >
		    <EditItemTemplate>
                <hr />
                <div style="padding-left:105px;">
                <uc1:FileUploadCtl ID="FileUploadCtl" runat="server" 
                    FileName='<%# Bind("APID") %>'
                    ImgWidth="280" 
                    CreateSmallThumbnail="True" 
                    SmallThumbnailWidth="64" 
                    Folder="~/Images/AvatarParts/"
                    Extension="png"
                    Width="400px"
                />
                </div>
                <hr />
            </EditItemTemplate>
            <InsertItemTemplate>
            </InsertItemTemplate>
            <ItemTemplate>
                <uc1:FileUploadCtl ID="FileUploadCtl" runat="server" />
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>



            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: "  Visible="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " Visible="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " Visible="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " Visible="False"
                SortExpression="AddedUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>


           



            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"    Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                        &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" 
                        CausesValidation="True" 
                        CommandName="Add" 
                        ImageUrl="~/ControlRoom/Images/add.png" 
                        Height="25"
                        Text="Add"   Tooltip="Add"
                        AlternateText="Add" />                        

                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"    Tooltip="Refresh Record"
                        AlternateText="Refresh Record" />   
                        &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server" 
                        CausesValidation="True" 
                        CommandName="Save" 
                        ImageUrl="~/ControlRoom/Images/save.png" 
                        Height="25"
                        Text="Save"   Tooltip="Save"
                        AlternateText="Save"/>     
                        &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Text="Save and return"   Tooltip="Save and return"
                        AlternateText="Save and return" />    
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:ObjectDataSource ID="odsDDBadges" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Badge"></asp:ObjectDataSource>



    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAvatarPart" TypeName="GRA.SRP.DAL.AvatarPart">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="APID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    

</asp:Content>
