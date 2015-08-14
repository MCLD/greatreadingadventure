<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGOnlineBookPagesAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGOnlineBookPagesAddEdit" 
    ValidateRequest="false"
%>

<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>
<%@ Register TagPrefix="uc2" TagName="AudioUploadCtl1" Src="~/Controls/AudioUploadCtl.ascx"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblOBID" runat="server" Text="" Visible="False"></asp:Label>



    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label>
    </h1>

    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>



        <asp:TemplateField HeaderText="Game ID: " SortExpression="MGID" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                <asp:TextBox ID="OBPGID" runat="server" Text='<%# Eval("OBPGID") %>' Visible="False"></asp:TextBox>
                <%# Eval("MGID") %>
            </EditItemTemplate>

            <ItemTemplate>
                <%# Eval("MGID") %>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Page Number: " SortExpression="PageNumber" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                <%#Eval("PageNumber") %>
            </EditItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Page Image: " HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                
                    <uc1:FileUploadCtl_1 ID="FileUploadCtl" runat="server" 
                        FileName='<%# Eval("OBPGID") %>'
                        ImgWidth="512" 
                        CreateSmallThumbnail="True"         
                        CreateMediumThumbnail="False"
                        SmallThumbnailWidth="64" 
                        MediumThumbnailWidth="128"
                        Folder="~/Images/Games/Books/"
                        Extension="png"
                    />


            </EditItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Text - Easy: " SortExpression="TextEasy" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="TextEasy" runat="server" Text='<%# Eval("TextEasy") %>' ReadOnly="False" Width="90%" 
                   TextMode="MultiLine" Rows="4"
                ></asp:TextBox>
                
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="TextEasy" runat="server" Text='<%# Eval("TextEasy") %>' ReadOnly="False" Width="90%" 
                   TextMode="MultiLine" Rows="4"
                ></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="TextEasy" runat="server" Text='<%# Eval("TextEasy") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" Width="150px" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Text - Medium: " SortExpression="TextMedium" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="TextMedium" runat="server" Text='<%# Eval("TextMedium") %>' ReadOnly="False" Width="90%" 
                   TextMode="MultiLine" Rows="4"
                ></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="TextMedium" runat="server" Text='<%# Eval("TextMedium") %>' ReadOnly="False" Width="90%" 
                   TextMode="MultiLine" Rows="4"
                ></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="TextMedium" runat="server" Text='<%# Eval("TextMedium") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Text - Hard: " SortExpression="TextHard" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="TextHard" runat="server" Text='<%# Eval("TextHard") %>' ReadOnly="False" Width="90%" 
                   TextMode="MultiLine" Rows="4"
                ></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="TextHard" runat="server" Text='<%# Eval("TextHard") %>' ReadOnly="False" Width="90%" 
                   TextMode="MultiLine" Rows="4"
                ></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="TextHard" runat="server" Text='<%# Eval("TextHard") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Audio - Easy: " SortExpression="AudioEasy" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                <asp:TextBox ID="AudioEasy" runat="server" Text='<%# Eval("AudioEasy") %>' ReadOnly="False" Visible="False"></asp:TextBox>
                <uc2:AudioUploadCtl1 ID="AudioUploadCtlE" runat="server" 
                    FileName='<%# "e_" + Eval("OBPGID") %>'
                    Folder="~/Images/Games/Books/"
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
                <asp:TextBox ID="AudioMedium" runat="server" Text='<%# Eval("AudioMedium") %>' ReadOnly="False" Visible="False"></asp:TextBox>
                <uc2:AudioUploadCtl1 ID="AudioUploadCtlM" runat="server" 
                    FileName='<%# "m_" + Eval("OBPGID") %>'
                    Folder="~/Images/Games/Books/"
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
                <asp:TextBox ID="AudioHard" runat="server" Text='<%# Eval("AudioHard") %>' ReadOnly="False" Visible="False"></asp:TextBox>
                <uc2:AudioUploadCtl1 ID="AudioUploadCtlH" runat="server" 
                    FileName='<%# "h_" + Eval("OBPGID") %>'
                    Folder="~/Images/Games/Books/"
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
        TypeName="GRA.SRP.DAL.MGOnlineBookPages">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="OBPGID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

