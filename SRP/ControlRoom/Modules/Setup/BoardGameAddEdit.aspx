<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="BoardGameAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.BoardGameAddEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
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

        <asp:BoundField DataField="PGID" HeaderText="Board Game ID: " SortExpression="PGID" ReadOnly="True" InsertVisible="False" Visible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>

        <asp:TemplateField>
		    <EditItemTemplate>
<table width="100%">
    <tr>
        <td nowrap>
            <strong>Game Name: </strong>
        </td>
        <td colspan="5"> 
                <asp:TextBox ID="GameName" runat="server" Text='<%# Eval("GameName") %>' ReadOnly="False" Width="99%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGameName" runat="server" 
                    ControlToValidate="GameName" Display="Dynamic" ErrorMessage="GameName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>            
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td nowrap>
            <strong>Board Size: </strong>
        </td>
        <td>
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
        </td>
        <td colspan="2"  align="center">
                <asp:Button ID="btnReplace" runat="server" Text="Game Levels" CssClass="btn-sm btn-purple" CommandName="levels" />
        </td>
        <td nowrap align="right">
            <strong>Bonus Level Point Multiplier: </strong>
        </td>
        <td>
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
        </td>
    </tr>
    <tr>
        <td>
            <strong>Default 'Primary' <br />Adventure:</strong>
        </td>
        <td colspan="2">
                <asp:DropDownList ID="Minigame1ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID" 
                    AppendDataBoundItems="True"  width="100%"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="Minigame1IDLbl" runat="server" Text='<%# Eval("Minigame1ID") %>' Visible="False"></asp:Label>                  
        </td>

        <td style="padding-left:40px;">
            <strong>Default 'Regular' <br />Adventure:</strong>
        </td>
        <td colspan="2">
                <asp:DropDownList ID="Minigame2ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID" 
                    AppendDataBoundItems="True" width="100%"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="Minigame2IDLbl" runat="server" Text='<%# Eval("Minigame2ID") %>' Visible="False"></asp:Label>      
            
        </td>
    </tr>

    <tr>
        <td colspan="6">
            <hr />
        </td>
    </tr>
    <tr>
        <td colspan="6">
            <table width="100%">
                <tr>
                        <td  colspan="2" width="33%"><b> Map Image: </b><br />
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
                        <td  colspan="2"  width="33%"><b>Bonus Map Image: </b><br />
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
                        
                        <td  colspan="2"  width="34%"><b>Level Complete Image: </b><br />
                               <uc1:FileUploadCtl_1 ID="FileUploadCtlStamp" runat="server" 
                                        FileName='<%# "stamp_"+Eval("PGID") %>'
                                        ImgWidth="64" 
                                        CreateSmallThumbnail="False" 
                                        CreateMediumThumbnail="False"
                                        SmallThumbnailWidth="64" 
                                        MediumThumbnailWidth="128"
                                        Folder="~/Images/Games/Board/"
                                        Extension="png"
                                    />                   
                        </td> 
                </tr>
            </table>      
        </td>
    </tr>
    <tr>
        <td colspan="6">
            <hr />
        </td>
    </tr>

</table>


            </EditItemTemplate>
            <InsertItemTemplate>
<table width="100%">
    <tr>
        <td nowrap>
            <strong>Game Name: </strong>
        </td>
        <td colspan="5"> 
                <asp:TextBox ID="GameName" runat="server" Text='' ReadOnly="False" Width="99%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGameName" runat="server" 
                    ControlToValidate="GameName" Display="Dynamic" ErrorMessage="GameName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>            
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td nowrap>
            <strong>Board Size: </strong>
        </td>
        <td>
                <asp:TextBox ID="BoardWidth" runat="server" Text='' 
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
        </td>
        <td colspan="2">
        </td>
        <td nowrap align="right">
            <strong>Bonus Level Point Multiplier: </strong>
        </td>
        <td>
                <asp:TextBox ID="BonusLevelPointMultiplier" runat="server" Text='2' ReadOnly="False"
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
        </td>
    </tr>
    <tr>
        <td>
            <strong>Default 'Primary'<br />Adventure:</strong>
        </td>
        <td colspan="2">
                <asp:DropDownList ID="Minigame1ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID" 
                    AppendDataBoundItems="True" Width="100%"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
        </td>

        <td style="padding-left:40px;">
            <strong>Default 'Regular' <br />Adventure:</strong>
        </td>
        <td colspan="2">
                <asp:DropDownList ID="Minigame2ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID" 
                    AppendDataBoundItems="True"  Width="100%"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            
        </td>
    </tr>
    <tr>
        <td colspan="6">
            <hr />
        </td>
    </tr>
    <tr>
        <td colspan="6">
            Once you add your <b>Board Game</b>'s basic setup information, you will need to upload your boad game images (normal and bonus play mode board images and the level complete stamp).  
            <br />
            All images need to be in <b>png</b> formatIt is reccomended that the normal and bonus play mode board images be at least 800 x 800 pixels, 
            and that the level complete stamp be at least 256 x 256 pixels with a transparent background.
            <br />
        </td>
    </tr>
    <tr>
        <td colspan="6">
            <hr />
        </td>
    </tr>

</table>

            </InsertItemTemplate>
            <ItemTemplate>
            </ItemTemplate>  
        </asp:TemplateField>


 



            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False" Visible="False" 
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False" Visible="False" 
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False" Visible="False" 
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False" Visible="False" 
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
                        ImageUrl="~/ControlRoom/RibbonImages/Adventures.png" 
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
        TypeName="GRA.SRP.DAL.ProgramGame">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PGID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDMiniGame" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Minigame">
    </asp:ObjectDataSource>
</asp:Content>

