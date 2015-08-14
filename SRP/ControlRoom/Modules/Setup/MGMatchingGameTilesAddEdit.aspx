<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGMatchingGameTilesAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGMatchingGameTilesAddEdit" 
    
%>

<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblMAGID" runat="server" Text="" Visible="False"></asp:Label>



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
                <asp:TextBox ID="MAGTID" runat="server" Text='<%# Eval("MAGTID") %>' Visible="False"></asp:TextBox>
                <%# Eval("MGID") %>
            </EditItemTemplate>
            <InsertItemTemplate>
            
            </InsertItemTemplate>
            <ItemTemplate>
                <%# Eval("MGID") %>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="" HeaderStyle-Wrap="False" InsertVisible="True">
		    <EditItemTemplate>
                <table>
                    <tr>
                        <td width="33%" valign="top" align="left"><b>Tile 1:</b></td>
                        <td width="33%" valign="top" align="left"><b>Tile 2:</b></td>
                        <td width="33%" valign="top" align="left"><b>Tile 3:</b></td>
                    </tr>
                    <tr>
                        <td width="33%" valign="top" align="left">
                                <uc1:FileUploadCtl_1 ID="FileUploadCtl_1" runat="server" 
                                    FileName='<%# "t1_" + Eval("MAGTID") %>'
                                    ImgWidth="256" 
                                    CreateSmallThumbnail="True"         
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/MatchingGame/"
                                    Extension="png"
                                />                        
                        </td>
                        <td width="33%" valign="top" align="left">
                                <uc1:FileUploadCtl_1 ID="FileUploadCtl_2" runat="server" 
                                    FileName='<%# "t2_" + Eval("MAGTID") %>'
                                    ImgWidth="256" 
                                    CreateSmallThumbnail="True"         
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/MatchingGame/"
                                    Extension="png"
                                />                        
                        </td>
                        <td width="33%" valign="top" align="left">
                                <uc1:FileUploadCtl_1 ID="FileUploadCtl_3" runat="server" 
                                    FileName='<%# "t3_" + Eval("MAGTID") %>'
                                    ImgWidth="256" 
                                    CreateSmallThumbnail="True"         
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/MatchingGame/"
                                    Extension="png"
                                />                        
                        </td>
                    </tr>    
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>                                  
                    <tr>
                        <td width="33%" valign="top" align="left">

                            <asp:CheckBox ID="Tile1UseMedium" runat="server" Checked='<%# (bool)Eval("Tile1UseMedium") %>' ReadOnly="False" Text="Use Tile 1 for Medium Level"></asp:CheckBox>
                            <br />
                            <asp:CheckBox ID="Tile1UseHard" runat="server" Checked='<%# (bool)Eval("Tile1UseHard") %>' ReadOnly="False" Text="Use Tile 1 for Hard Level"></asp:CheckBox>
                        </td>
                        <td width="33%" valign="top" align="left">

                            <asp:CheckBox ID="Tile2UseMedium" runat="server" Checked='<%# (bool)Eval("Tile2UseMedium") %>' ReadOnly="False" Text="Use Tile 2 for Medium Level" /></asp:CheckBox>
                            <br />
                            <asp:CheckBox ID="Tile2UseHard" runat="server" Checked='<%# (bool)Eval("Tile2UseHard") %>' ReadOnly="False" Text="Use Tile 2 for Hard Level"></asp:CheckBox>
                        </td>                                       
                        <td width="33%" valign="top" align="left">

                            <asp:CheckBox ID="Tile3UseMedium" runat="server" Checked='<%# (bool)Eval("Tile3UseMedium") %>' ReadOnly="False" Text="Use Tile 3 for Medium Level"></asp:CheckBox>
                            <br />
                            <asp:CheckBox ID="Tile3UseHard" runat="server" Checked='<%# (bool)Eval("Tile3UseHard") %>' ReadOnly="False" Text="Use Tile 3 for Hard Level"></asp:CheckBox>
                        </td>
                     </tr>   
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>                                     
                  </table>

               </EditItemTemplate>
               <InsertItemTemplate>
                <table>
                    <tr>
                        <td width="33%" valign="top" align="left"><b>Tile 1:</b></td>
                        <td width="33%" valign="top" align="left"><b>Tile 2:</b></td>
                        <td width="33%" valign="top" align="left"><b>Tile 3:</b></td>
                    </tr>

                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>                
                    <tr>
                        <td width="33%" valign="top" align="left">
                            <asp:CheckBox ID="Tile1UseMedium" runat="server"  ReadOnly="False" Text="Use Tile 1 for Medium Level"></asp:CheckBox>
                            <br />
                            <asp:CheckBox ID="Tile1UseHard" runat="server"  ReadOnly="False" Text="Use Tile 1 for Hard Level"></asp:CheckBox>
                        </td>
                        <td width="33%" valign="top" align="left">

                            <asp:CheckBox ID="Tile2UseMedium" runat="server"  ReadOnly="False" Text="Use Tile 2 for Medium Level" /></asp:CheckBox>
                            <br />
                            <asp:CheckBox ID="Tile2UseHard" runat="server"  ReadOnly="False" Text="Use Tile 2 for Hard Level"></asp:CheckBox>
                        </td>                                       
                        <td width="33%" valign="top" align="left">

                            <asp:CheckBox ID="Tile3UseMedium" runat="server"  ReadOnly="False" Text="Use Tile 3 for Medium Level"></asp:CheckBox>
                            <br />
                            <asp:CheckBox ID="Tile3UseHard" runat="server"  ReadOnly="False" Text="Use Tile 3 for Hard Level"></asp:CheckBox>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>
                  </table>               
               
               </InsertItemTemplate>
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
        TypeName="GRA.SRP.DAL.MGMatchingGameTiles">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="MAGTID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

