<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="BookListAddEdit.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.BookListAddEdit" 
    
%>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

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

            <asp:BoundField DataField="BLID" HeaderText="BLID: " SortExpression="BLID" ReadOnly="True" InsertVisible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>



            <asp:TemplateField HeaderText="Admin Name: " SortExpression="AdminName" HeaderStyle-Wrap="False">
		        <EditItemTemplate>
                    <asp:TextBox ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>' ReadOnly="False" Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                        ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required" 
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <br />
                <asp:Button ID="btnReplace" runat="server" 
                    Text="Books on this Book List" CssClass="btn-sm btn-purple"
                    CommandName="Saveandbooks" 
                />

                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="AdminName" runat="server" Text='' Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                        ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required" 
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="List Name: " SortExpression="ListName" HeaderStyle-Wrap="False">
		        <EditItemTemplate>
                    <asp:TextBox ID="ListName" runat="server" Text='<%# Eval("ListName") %>' ReadOnly="False" Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvListName" runat="server" 
                        ControlToValidate="ListName" Display="Dynamic" ErrorMessage="ListName is required" 
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="ListName" runat="server" Text='' Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvListName" runat="server" 
                        ControlToValidate="ListName" Display="Dynamic" ErrorMessage="ListName is required" 
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="ListName" runat="server" Text='<%# Eval("ListName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Admin Description: " SortExpression="AdminDescription" HeaderStyle-Wrap="False">
		        <EditItemTemplate>
                    <CKEditor:CKEditorControl ID="AdminDescription" 
                            BasePath="/ckeditor/" 
                            runat="server" 
                            Skin="office2003" 
                            BodyId="wrapper" 
                            ContentsCss="/css/EditorStyles.css"
                            DisableNativeSpellChecker="False" 
                            DisableNativeTableHandles="False" 
                            DocType="&lt;!DOCTYPE html&gt;" 
                            ForcePasteAsPlainText="True" 
                            Height="150px" UIColor="#D3D3D3" 
                            Visible="True" 
                            Width="98%"
                            Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                            / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                            / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                            / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                            "
                            AutoGrowOnStartup="True" 
					         Text='<%# Eval("AdminDescription") %>' 
                            ></CKEditor:CKEditorControl>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <CKEditor:CKEditorControl ID="AdminDescription" 
                            BasePath="/ckeditor/" 
                            runat="server" 
                            Skin="office2003" 
                            BodyId="wrapper" 
                            ContentsCss="/css/EditorStyles.css"
                            DisableNativeSpellChecker="False" 
                            DisableNativeTableHandles="False" 
                            DocType="&lt;!DOCTYPE html&gt;" 
                            ForcePasteAsPlainText="True" 
                            Height="150px" UIColor="#D3D3D3" 
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
                    <asp:Label ID="AdminDescription" runat="server" Text='<%# Eval("AdminDescription") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Description: " SortExpression="Description" HeaderStyle-Wrap="False">
		        <EditItemTemplate>
                    <CKEditor:CKEditorControl ID="Description" 
                            BasePath="/ckeditor/" 
                            runat="server" 
                            Skin="office2003" 
                            BodyId="wrapper" 
                            ContentsCss="/css/EditorStyles.css"
                            DisableNativeSpellChecker="False" 
                            DisableNativeTableHandles="False" 
                            DocType="&lt;!DOCTYPE html&gt;" 
                            ForcePasteAsPlainText="True" 
                            Height="150px" UIColor="#D3D3D3" 
                            Visible="True" 
                            Width="98%"
                            Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                            / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                            / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                            / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                            "
                            AutoGrowOnStartup="True" 
					         Text='<%# Eval("Description") %>' 
                            ></CKEditor:CKEditorControl>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <CKEditor:CKEditorControl ID="Description" 
                            BasePath="/ckeditor/" 
                            runat="server" 
                            Skin="office2003" 
                            BodyId="wrapper" 
                            ContentsCss="/css/EditorStyles.css"
                            DisableNativeSpellChecker="False" 
                            DisableNativeTableHandles="False" 
                            DocType="&lt;!DOCTYPE html&gt;" 
                            ForcePasteAsPlainText="True" 
                            Height="150px" UIColor="#D3D3D3" 
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
                    <asp:Label ID="Description" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Literacy Level 1: " SortExpression="LiteracyLevel1" HeaderStyle-Wrap="False">
		        <EditItemTemplate>
                    <asp:TextBox ID="LiteracyLevel1" runat="server" Text='<%# ((int) Eval("LiteracyLevel1") ==0 ? "" : Eval("LiteracyLevel1")) %>' 
                         ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RegularExpressionValidator id="revLiteracyLevel1"
                        ControlToValidate="LiteracyLevel1"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 1 must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 1 must be numeric. </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" />      
                    <asp:RangeValidator ID="rvLiteracyLevel1"
                        ControlToValidate="LiteracyLevel1"
                        MinimumValue="0"
                        MaximumValue="99"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 1 must be from 0 to 99!</font>"
                        runat="server" 
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 1 must be from 0 to 99! </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" /> 
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="LiteracyLevel1" runat="server" Text='' Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RegularExpressionValidator id="revLiteracyLevel1"
                        ControlToValidate="LiteracyLevel1"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 1 must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 1 must be numeric. </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" />      
                    <asp:RangeValidator ID="rvLiteracyLevel1"
                        ControlToValidate="LiteracyLevel1"
                        MinimumValue="0"
                        MaximumValue="99"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 1 must be from 0 to 99!</font>"
                        runat="server" 
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 1 must be from 0 to 99! </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" /> 
                </InsertItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Literacy Level 2: " SortExpression="LiteracyLevel2" HeaderStyle-Wrap="False">
		        <EditItemTemplate>
                    <asp:TextBox ID="LiteracyLevel2" runat="server" Text='<%# ((int) Eval("LiteracyLevel2") ==0 ? "" : Eval("LiteracyLevel2")) %>' 
                         ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                
                    <asp:RegularExpressionValidator id="revLiteracyLevel2"
                        ControlToValidate="LiteracyLevel2"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 2 must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 2 must be numeric. </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" />      
                    <asp:RangeValidator ID="rvLiteracyLevel2"
                        ControlToValidate="LiteracyLevel2"
                        MinimumValue="0"
                        MaximumValue="99"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 2 must be from 0 to 99!</font>"
                        runat="server" 
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 2 must be from 0 to 99! </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" /> 
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="LiteracyLevel2" runat="server" Text='' Width="50px" CssClass="align-right"></asp:TextBox>
                
                    <asp:RegularExpressionValidator id="revLiteracyLevel2"
                        ControlToValidate="LiteracyLevel2"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 2 must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 2 must be numeric. </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" />      
                    <asp:RangeValidator ID="rvLiteracyLevel2"
                        ControlToValidate="LiteracyLevel2"
                        MinimumValue="0"
                        MaximumValue="99"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Literacy Level 2 must be from 0 to 99!</font>"
                        runat="server" 
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Literacy Level 2 must be from 0 to 99! </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" /> 
                </InsertItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

        <asp:TemplateField HeaderText="Program: " SortExpression="ProgID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                    <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                        AppendDataBoundItems="True"
                        >
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="ProgIDLbl" runat="server" Text='<%# Eval("ProgID") %>' Visible="False"></asp:Label>  
                
            </EditItemTemplate>
            <InsertItemTemplate>
                    <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                        AppendDataBoundItems="True"
                        >
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="ProgID" runat="server" Text='<%# Eval("ProgID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Library: " SortExpression="LibraryID" HeaderStyle-Wrap="False">
            <EditItemTemplate>
                <asp:DropDownList ID="LibraryID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="LibraryIDLbl" runat="server" Text='<%# Eval("LibraryID") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="LibraryID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
		    <ItemTemplate>
                <asp:Label ID="LibraryID" runat="server" Text='<%# Eval("LibraryID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
         </asp:TemplateField>

        <asp:TemplateField HeaderText="Badge Awarded: " SortExpression="AwardBadgeID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="AwardBadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="AwardBadgeIDLbl" runat="server" Text='<%# Eval("AwardBadgeID") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
               <asp:DropDownList ID="AwardBadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
           
            <ItemTemplate>
                <asp:Label ID="AwardBadgeID" runat="server" Text='<%# Eval("AwardBadgeID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    

             </asp:TemplateField>

        <asp:TemplateField HeaderText="Points Awarded: " SortExpression="AwardPoints" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="AwardPoints" runat="server" Text='<%# ((int) Eval("AwardPoints") ==0 ? "" : Eval("AwardPoints")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
          
                <asp:RegularExpressionValidator id="revAwardPoints"
                    ControlToValidate="AwardPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Points Awarded must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Points Awarded must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvAwardPoints"
                    ControlToValidate="AwardPoints"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Points Awarded must be from 0 to 9999!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Points Awarded must be from 0 to 9999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AwardPoints" runat="server" Text='' Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="revAwardPoints"
                    ControlToValidate="AwardPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Points Awarded must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Points Awarded must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvAwardPoints"
                    ControlToValidate="AwardPoints"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Points Awarded must be from 0 to 9999!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Points Awarded must be from 0 to 9999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AwardPoints" runat="server" Text='<%# Eval("AwardPoints") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
                    </asp:TemplateField>

            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False"
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

                        

                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="ImageButton2" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandbooks" 
                        ImageUrl="~/ControlRoom/Images/library.png" 
                        Height="25"
                        Text="Save and see books"   Tooltip="Save and see books"
                        AlternateText="Save and return" />    
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="STG.SRP.DAL.BookList">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BLID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsBadge" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Badge">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsProg" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Programs">
    </asp:ObjectDataSource>

</asp:Content>

