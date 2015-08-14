<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGMixAndMatchItemsAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGMixAndMatchItemsAddEdit" 
    
%>

<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>
<%@ Register TagPrefix="uc2" TagName="AudioUploadCtl1" Src="~/Controls/AudioUploadCtl.ascx"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblMMID" runat="server" Text="" Visible="False"></asp:Label>

    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label></h1>

    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:TemplateField HeaderText="Game ID: " SortExpression="MGID" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                <asp:TextBox ID="MMIID" runat="server" Text='<%# Eval("MMIID") %>' Visible="False"></asp:TextBox>
                <%# Eval("MGID") %>
            </EditItemTemplate>

            <ItemTemplate>
                <%# Eval("MGID") %>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Item Image: " HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                
                    <uc1:FileUploadCtl_1 ID="FileUploadCtl" runat="server" 
                        FileName='<%# Eval("MMIID") %>'
                        ImgWidth="250" 
                        CreateSmallThumbnail="True"         
                        CreateMediumThumbnail="False"
                        SmallThumbnailWidth="64" 
                        MediumThumbnailWidth="256"
                        Folder="~/Images/Games/MixMatch/"
                        Extension="png"
                    />

            </EditItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


        <asp:TemplateField HeaderText="Easy Label: " SortExpression="EasyLabel" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="EasyLabel" runat="server" Text='<%# Eval("EasyLabel") %>' ReadOnly="False" Width="90%" MaxLength="150"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEasyLabel" runat="server" 
                    ControlToValidate="EasyLabel" Display="Dynamic" ErrorMessage="EasyLabel is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="EasyLabel" runat="server" Text=''  Width="90%" MaxLength="150"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEasyLabel" runat="server" 
                    ControlToValidate="EasyLabel" Display="Dynamic" ErrorMessage="EasyLabel is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="EasyLabel" runat="server" Text='<%# Eval("EasyLabel") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" Width="200px"/>    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Medium Label: " SortExpression="MediumLabel" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="MediumLabel" runat="server" Text='<%# Eval("MediumLabel") %>' ReadOnly="False" Width="90%" MaxLength="150"></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="MediumLabel" runat="server" Text='' Width="90%" MaxLength="150"></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="MediumLabel" runat="server" Text='<%# Eval("MediumLabel") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="HardLabel: " SortExpression="HardLabel" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="HardLabel" runat="server" Text='<%# Eval("HardLabel") %>' ReadOnly="False" Width="90%" MaxLength="150"></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="HardLabel" runat="server" Text='' Width="90%" MaxLength="150"></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="HardLabel" runat="server" Text='<%# Eval("HardLabel") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Audio - Easy: " SortExpression="AudioEasy" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>

                <uc2:AudioUploadCtl1 ID="AudioUploadCtlE" runat="server" 
                    FileName='<%# "e_" + Eval("MMIID") %>'
                    Folder="~/Images/Games/MixMatch/"
                    Extension="mp3"
                />

            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AudioEasy" runat="server" Text=''></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AudioEasy" runat="server" Text='<%# Eval("AudioEasy") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Audio - Medium: " SortExpression="AudioMedium" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>

                <uc2:AudioUploadCtl1 ID="AudioUploadCtlM" runat="server" 
                    FileName='<%# "m_" + Eval("MMIID") %>'
                    Folder="~/Images/Games/MixMatch/"
                    Extension="mp3"
                />

            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AudioMedium" runat="server" Text=''></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AudioMedium" runat="server" Text='<%# Eval("AudioMedium") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Audio - Hard: " SortExpression="AudioHard" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>

                <uc2:AudioUploadCtl1 ID="AudioUploadCtlH" runat="server" 
                    FileName='<%# "h_" + Eval("MMIID") %>'
                    Folder="~/Images/Games/MixMatch/"
                    Extension="mp3"
                />

            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AudioHard" runat="server" Text=''></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AudioHard" runat="server" Text='<%# Eval("AudioHard") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>
            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False"  Visible="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False"  Visible="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False"  Visible="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False"  Visible="False"
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

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="GRA.SRP.DAL.MGMixAndMatchItems">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="MMIID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

