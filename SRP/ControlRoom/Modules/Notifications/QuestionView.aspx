<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="QuestionView.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Notifications.QuestionView" 
    
%>

<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register src="~/ControlRoom/Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    

    <uc1:PatronContext ID="pcCtl" runat="server" />
    
        <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:BoundField DataField="NID" HeaderText="NID: " SortExpression="NID" ReadOnly="True" InsertVisible="False" Visible="false">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>


        
        <asp:TemplateField HeaderText="Subject: " SortExpression="Subject" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <%# Eval("Subject") %>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="Subject" runat="server" Text='' Width="500px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSubject" runat="server" 
                    ControlToValidate="Subject" Display="Dynamic" ErrorMessage="Subject is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Subject" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            <ItemStyle Width="100%" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Body: " SortExpression="Body" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <%# Eval("Body") %>
                <hr /><b>Reply:</b><hr />
                <CKEditor:CKEditorControl ID="Reply" 
                        BasePath="/ckeditor/" 
                        runat="server" 
                        Skin="office2003" 
                        BodyId="wrapper" 
                        ContentsCss="/css/EditorStyles.css"
                        DisableNativeSpellChecker="False" 
                        DisableNativeTableHandles="False" 
                        DocType="&lt;!DOCTYPE html&gt;" 
                        ForcePasteAsPlainText="True" 
                        Height="250px" UIColor="#D3D3D3" 
                        Visible="True" 
                        Width="98%"
                        Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                        / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                        / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                        / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                        "
                        AutoGrowOnStartup="True" 
                        ></CKEditor:CKEditorControl>
            </EditItemTemplate>
            <InsertItemTemplate>
                <CKEditor:CKEditorControl ID="Body" 
                        BasePath="/ckeditor/" 
                        runat="server" 
                        Skin="office2003" 
                        BodyId="wrapper" 
                        ContentsCss="/css/EditorStyles.css"
                        DisableNativeSpellChecker="False" 
                        DisableNativeTableHandles="False" 
                        DocType="&lt;!DOCTYPE html&gt;" 
                        ForcePasteAsPlainText="True" 
                        Height="250px" UIColor="#D3D3D3" 
                        Visible="True" 
                        Width="98%"
                        Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                        / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                        / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                        / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                        "
                        AutoGrowOnStartup="True" 
                        ></CKEditor:CKEditorControl>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Body" runat="server" Text='<%# Eval("Body") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


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
                     &nbsp;

                    <asp:ImageButton ID="btnSaveback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Text="Reply and return"   Tooltip="Reply and return"
                        AlternateText="Save and return" />                   
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="STG.SRP.DAL.Notifications">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="NID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

