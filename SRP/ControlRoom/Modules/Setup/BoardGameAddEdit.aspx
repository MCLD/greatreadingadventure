<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="BoardGameAddEdit.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.BoardGameAddEdit" 
    
%>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>


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

        <asp:BoundField DataField="PGID" HeaderText="Board Game ID: " SortExpression="PGID" ReadOnly="True" InsertVisible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>


        <asp:TemplateField HeaderText="Game Name: " SortExpression="GameName" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="GameName" runat="server" Text='<%# Eval("GameName") %>' ReadOnly="False" Width="90%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGameName" runat="server" 
                    ControlToValidate="GameName" Display="Dynamic" ErrorMessage="GameName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="GameName" runat="server" Text='' Width="90%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGameName" runat="server" 
                    ControlToValidate="GameName" Display="Dynamic" ErrorMessage="GameName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="GameName" runat="server" Text='<%# Eval("GameName") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" Width="200px"/>    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                <table width ="100%">
                    <tr>
                        <td ><b> Map Image: </b><br />
                            <uc1:FileUploadCtl_1 ID="FileUploadCtlMap" runat="server" 
                                    FileName='<%# Eval("PGID") %>'
                                    ImgWidth="800" 
                                    CreateSmallThumbnail="False" 
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/Board/"
                                    Extension="png"
                                />                        
                        </td>
                        <td ><b>Bonus Map Image: </b><br />
                            <uc1:FileUploadCtl_1 ID="FileUploadCtlBonus" runat="server" 
                                    FileName='<%# "bonus_"+Eval("PGID") %>'
                                    ImgWidth="800" 
                                    CreateSmallThumbnail="False" 
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/Board/"
                                    Extension="png"
                                />                        
                        </td>
                        
                        <td ><b>Level Complete Image: </b><br />
                               <uc1:FileUploadCtl_1 ID="FileUploadCtlStamp" runat="server" 
                                        FileName='<%# "stamp_"+Eval("PGID") %>'
                                        ImgWidth="256" 
                                        CreateSmallThumbnail="True" 
                                        CreateMediumThumbnail="True"
                                        SmallThumbnailWidth="64" 
                                        MediumThumbnailWidth="128"
                                        Folder="~/Images/Games/Board/"
                                        Extension="png"
                                    />                   
                        </td>                        
                        </tr>
                                        
                </table>
            </EditItemTemplate>
            <HeaderTemplate>
            <br /><br /><br /><br /><br /><br />
                <asp:Button ID="btnReplace" runat="server" 
                Text="Game Levels" CssClass="btn-sm btn-purple"
                
                CommandName="levels" 
                />
                <br /><br /><br /><br /><br /><br />
            </HeaderTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Board Size: " SortExpression="BoardWidth" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="BoardWidth" runat="server" Text='<%# ((int) Eval("BoardWidth") ==0 ? "" : Eval("BoardWidth")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBoardWidth" runat="server" 
                    ControlToValidate="BoardWidth" Display="Dynamic" ErrorMessage="BoardWidth is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revBoardWidth"
                    ControlToValidate="BoardWidth"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>BoardWidth must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * BoardWidth must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvBoardWidth"
                    ControlToValidate="BoardWidth"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>BoardWidth must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * BoardWidth must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="BoardWidth" runat="server" Text=''></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBoardWidth" runat="server" 
                    ControlToValidate="BoardWidth" Display="Dynamic" ErrorMessage="BoardWidth is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revBoardWidth"
                    ControlToValidate="BoardWidth"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>BoardWidth must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * BoardWidth must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvBoardWidth"
                    ControlToValidate="BoardWidth"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>BoardWidth must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * BoardWidth must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BoardWidth" runat="server" Text='<%# Eval("BoardWidth") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


        <asp:TemplateField HeaderText="Bonus Level Point Multiplier: " SortExpression="BonusLevelPointMultiplier" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="BonusLevelPointMultiplier" runat="server" Text='<%# Eval("BonusLevelPointMultiplier") %>' ReadOnly="False"
                    Width="50px" CssClass="align-right"
                ></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBonusLevelPointMultiplier" runat="server" 
                    ControlToValidate="BonusLevelPointMultiplier" Display="Dynamic" ErrorMessage="BonusLevelPointMultiplier is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                <asp:RegularExpressionValidator id="revBonusLevelPointMultiplier"
                    ControlToValidate="BonusLevelPointMultiplier"
                    ValidationExpression="^[1-9]\d*(\.\d+)?$"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Bonus Level Point Multiplier must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Bonus Level Point Multiplier must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvBonusLevelPointMultiplier"
                    ControlToValidate="BonusLevelPointMultiplier"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Double"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Bonus Level Point Multiplier must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Bonus Level Point Multiplier must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
                    
                                </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="BonusLevelPointMultiplier" runat="server" Text=''></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBonusLevelPointMultiplier" runat="server" 
                    ControlToValidate="BonusLevelPointMultiplier" Display="Dynamic" ErrorMessage="BonusLevelPointMultiplier is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator id="revBonusLevelPointMultiplier"
                    ControlToValidate="BonusLevelPointMultiplier"
                    ValidationExpression="^[1-9]\d*(\.\d+)?$"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Bonus Level Point Multiplier must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Bonus Level Point Multiplier must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvBonusLevelPointMultiplier"
                    ControlToValidate="BonusLevelPointMultiplier"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Double"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Bonus Level Point Multiplier must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Bonus Level Point Multiplier must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 

            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BonusLevelPointMultiplier" runat="server" Text='<%# Eval("BonusLevelPointMultiplier") %>'></asp:Label>
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
                        
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

                    <asp:ImageButton ID="ImageButton1" runat="server" 
                        CausesValidation="True" 
                        CommandName="levels" 
                        ImageUrl="~/ControlRoom/Images/game_level.png" 
                        Height="25"
                        Text="Game Levels"   Tooltip="Game Levels"
                        AlternateText="Game Levels" /> 
                           
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="STG.SRP.DAL.ProgramGame">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PGID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

