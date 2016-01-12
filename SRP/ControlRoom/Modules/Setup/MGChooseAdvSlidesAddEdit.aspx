<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGChooseAdvSlidesAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGChooseAdvSlidesAddEdit" 
    
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
    <asp:Label ID="lblCAID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblDiff" runat="server" Text="1" Visible="False"></asp:Label>

    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label></h1>


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:TemplateField HeaderText="Game ID: " SortExpression="MGID" HeaderStyle-Wrap="False" InsertVisible="False">
		    <EditItemTemplate>
                <asp:TextBox ID="CASID" runat="server" Text='<%# Eval("CASID") %>' Visible="False"></asp:TextBox>
                <%# Eval("MGID")%>
            </EditItemTemplate>
            <InsertItemTemplate>
            
            </InsertItemTemplate>
            <ItemTemplate>
                <%# Eval("MGID")%>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


        <asp:TemplateField HeaderText="StepNumber: " SortExpression="StepNumber" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <%# ((int) Eval("StepNumber") ==0 ? "" : Eval("StepNumber")) %> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="StepNumber" runat="server" Text=''></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="StepNumber" runat="server" Text='<%# Eval("StepNumber") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="SlideText: " SortExpression="SlideText" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <textarea id="SlideText" runat="server" class="gra-editor"><%# Eval("SlideText") %></textarea>                      

                <br />
                <uc2:AudioUploadCtl1 ID="AudioUploadCtlE" runat="server" 
                    FileName='<%# Eval("CASID") + "_" +  Eval("Difficulty") %>'
                    Folder="~/Images/Games/ChooseAdv/"
                    Extension="mp3"
                />
            </EditItemTemplate>
            <InsertItemTemplate>
                
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="SlideText" runat="server" Text='<%# Eval("SlideText") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="" SortExpression="" HeaderStyle-Wrap="False">
		    <EditItemTemplate>


                <table width="100%">
                    <tr>
                        <td width="50%" valign="top" align="left"><b>Image 1:</b></td>
                        <td width="50%" valign="top" align="left"><b>Image 2:</b></td>
                    </tr>
                    <tr>
                        <td width="50%" valign="top" align="left">
                                <uc1:FileUploadCtl_1 ID="FileUploadCtl_1" runat="server" 
                                    FileName='<%# "i1_" + Eval("CASID") %>'
                                    ImgWidth="400" 
                                    CreateSmallThumbnail="True"         
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/ChooseAdv/"
                                    Extension="png"
                                />                        
                        </td>
                        <td width="50%" valign="top" align="left">
                                <uc1:FileUploadCtl_1 ID="FileUploadCtl_2" runat="server" 
                                    FileName='<%# "i2_" + Eval("CASID") %>'
                                    ImgWidth="400" 
                                    CreateSmallThumbnail="True"         
                                    CreateMediumThumbnail="False"
                                    SmallThumbnailWidth="64" 
                                    MediumThumbnailWidth="128"
                                    Folder="~/Images/Games/ChooseAdv/"
                                    Extension="png"
                                />                        
                        </td>
                    </tr>                
                    <tr>
                        <td width="50%" valign="top" align="left">
                            <br />
                            <b>Image 1 Goes To Step: </b>
                <asp:TextBox ID="FirstImageGoToStep" runat="server" Text='<%# ((int) Eval("FirstImageGoToStep") ==0 ? "" : Eval("FirstImageGoToStep")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RegularExpressionValidator id="RegularExpressionValidator1"
                    ControlToValidate="FirstImageGoToStep"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Image 1 Goes To Step must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Image 1 Goes To Step must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="RangeValidator1"
                    ControlToValidate="FirstImageGoToStep"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Image 1 Goes To Step must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Image 1 Goes To Step must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />
                                                

                        </td>
                        <td width="50%" valign="top" align="left">
                            <br />
                            <b>Image 2 Goes To Step: </b>
                <asp:TextBox ID="SecondImageGoToStep" runat="server" Text='<%# ((int) Eval("SecondImageGoToStep") ==0 ? "" : Eval("SecondImageGoToStep")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RegularExpressionValidator id="revSecondImageGoToStep"
                    ControlToValidate="SecondImageGoToStep"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Image 2 Goes To Step must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Image 2 Goes To Step must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvSecondImageGoToStep"
                    ControlToValidate="SecondImageGoToStep"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Image 2 Goes To Step must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Image 2 Goes To Step must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                             
                        </td>
                     </tr>                   
                  </table>

            </EditItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
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
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="GRA.SRP.DAL.MGChooseAdvSlides">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="CASID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

