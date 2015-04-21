<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="BookListAddEdit.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.BookListAddEdit" 
    validateRequest="false"
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

            <asp:BoundField DataField="BLID" HeaderText="BLID: " SortExpression="BLID" ReadOnly="True" InsertVisible="False" Visible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>

            <asp:TemplateField>
		        <EditItemTemplate>
                    <table width="100%">
                        <tr>
                            <td nowrap valign="top"> <b> Admin Name: </b> </td>
                            <td colspan="6"  valign="top">
                                <asp:TextBox ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>' ReadOnly="False" Width="90%"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                                    ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required" 
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                            <td  valign="top">
                                <asp:Button ID="btnReplace" runat="server" 
                                    Text="Books on this Book List" CssClass="btn-sm btn-purple"
                                    CommandName="Saveandbooks" 
                                />
                            </td>
                        
                        </tr>
                    
                        <tr>
                            <td nowrap valign="top"> <b> List Name (for Patrons): </b> </td>
                            <td colspan="6"  valign="top">
                                <asp:TextBox ID="ListName" runat="server" Text='<%# Eval("ListName") %>' ReadOnly="False" Width="90%"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                    ControlToValidate="ListName" Display="Dynamic" ErrorMessage="List Name is required" 
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                    
                        </tr>

                        <tr>
                            <td nowrap valign="top"> <b> Admin Description: </b> </td>
                            <td colspan="6"  valign="top">
                                <asp:TextBox ID="AdminDescription" runat="server" Text='<%# Eval("AdminDescription") %>' ReadOnly="False" Width="90%" Rows="3" TextMode="MultiLine"></asp:TextBox>
                            </td>
                            <td  valign="top">

                            </td>
                        </tr>

                        <tr>
                            <td nowrap valign="top"> <b> Patron Description: </b> </td>
                            <td colspan="7"  valign="top">
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
                            </td>
                        </tr>

                        <tr>
                            <td nowrap valign="top"> <b> Program: </b> </td>
                            <td colspan="1"  valign="top">
                                <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                                    AppendDataBoundItems="True"
                                    >
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="ProgIDLbl" runat="server" Text='<%# Eval("ProgID") %>' Visible="False"></asp:Label>  
                            </td>

                            <td nowrap valign="top"> <b> Branch/Library: </b> </td>
                            <td colspan="1"  valign="top">
                                <asp:DropDownList ID="LibraryID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                                    AppendDataBoundItems="True"
                                 >
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="LibraryIDLbl" runat="server" Text='<%# Eval("LibraryID") %>' Visible="false" ></asp:Label>
                            </td>

                            <td nowrap valign="top"> <b> Literacy Level 1: </b> </td>
                            <td colspan="1"  valign="top">
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
                            </td>

                            <td nowrap valign="top"> <b> Literacy Level 2: </b> </td>
                            <td colspan="1"  valign="top">
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
                            </td>
                        </tr>

                        <tr>
                            <td nowrap valign="top"> <b> # Books Req.: </b> </td>
                            <td colspan="1"  valign="top">
                                <asp:TextBox ID="NumBooksToComplete" runat="server" Text='<%# ((int) Eval("NumBooksToComplete") ==0 ? "" : Eval("NumBooksToComplete")) %>' 
                                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
          
                                <asp:RegularExpressionValidator id="RegularExpressionValidator1"
                                    ControlToValidate="NumBooksToComplete"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Num Books Required must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Num Books Required must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator1"
                                    ControlToValidate="NumBooksToComplete"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'Num Books Required must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Num Books Required must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" /> 
                            </td>

                            <td nowrap valign="top"> <b> # Points Awarded: </b> </td>
                            <td colspan="1"  valign="top">
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
                            </td>

                            <td nowrap valign="top"> <b> Badge Awarded: </b> </td>
                            <td colspan="3"  valign="top">
                                <asp:DropDownList ID="AwardBadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                                    AppendDataBoundItems="True"
                                    >
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="AwardBadgeIDLbl" runat="server" Text='<%# Eval("AwardBadgeID") %>' Visible="false" ></asp:Label>
                            </td>

                        </tr>
                        <tr>
                            <td colspan="8"><hr /></td>
                        </tr>
                    </table>
                </EditItemTemplate>

                <InsertItemTemplate>
                </InsertItemTemplate>
                <ItemTemplate>
                    

                </ItemTemplate>
            </asp:TemplateField>


            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False" Visible="false"
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

