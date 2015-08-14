<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="PrizeDrawingAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Drawings.PrizeDrawingAddEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

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

        <asp:BoundField DataField="PDID" HeaderText="PDID: " SortExpression="PDID" ReadOnly="True" InsertVisible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>

        <asp:TemplateField HeaderText="Template: " SortExpression="TID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="TID" runat="server" DataSourceID="odsT" DataTextField="TName" DataValueField="TID" 
                    AppendDataBoundItems="True"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="TIDLbl" runat="server" Text='<%# Eval("TID") %>' Visible="False"></asp:Label>    
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="TID" runat="server" DataSourceID="odsT" DataTextField="TName" DataValueField="TID" 
                    AppendDataBoundItems="True"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="TIDLbl" runat="server" Text='' Visible="False"></asp:Label>    
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="TID" runat="server" Text='<%# Eval("TID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" Width="150px"/>    
        </asp:TemplateField>


        <asp:TemplateField HeaderText="# Winners: " SortExpression="NumWinners" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="NumWinners" runat="server" Text='<%# ((int) Eval("NumWinners") ==0 ? "" : Eval("NumWinners")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumWinners" runat="server" 
                    ControlToValidate="NumWinners" Display="Dynamic" ErrorMessage="NumWinners is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revNumWinners"
                    ControlToValidate="NumWinners"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'># Winners must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * # Winners must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumWinners"
                    ControlToValidate="NumWinners"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'># Winners must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * # Winners must be from 0 to 9999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="NumWinners" runat="server" Text='' Width="50px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumWinners" runat="server" 
                    ControlToValidate="NumWinners" Display="Dynamic" ErrorMessage="NumWinners is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revNumWinners"
                    ControlToValidate="NumWinners"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'># Winners must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * # Winners must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumWinners"
                    ControlToValidate="NumWinners"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'># Winners must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * # Winners must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="NumWinners" runat="server" Text='<%# Eval("NumWinners") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Prize Name: " SortExpression="PrizeName" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="PrizeName" runat="server" Text='<%# Eval("PrizeName") %>' ReadOnly="False" Width="400px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPrizeName" runat="server" 
                    ControlToValidate="PrizeName" Display="Dynamic" ErrorMessage="PrizeName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="PrizeName" runat="server" Text=''  Width="400px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPrizeName" runat="server" 
                    ControlToValidate="PrizeName" Display="Dynamic" ErrorMessage="PrizeName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="PrizeName" runat="server" Text='<%# Eval("PrizeName") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>



        <asp:TemplateField HeaderText="Drawing Date Time: " SortExpression="DrawingDateTime" HeaderStyle-Wrap="False" InsertVisible="false">
		    <EditItemTemplate>
                <%# FormatHelper.ToNormalDate((DateTime)Eval("DrawingDateTime"))%> <br /><hr />
                <asp:ImageButton ID="btnDraw" runat="server" 
                        CausesValidation="false" 
                        CommandName="draw" 
                        ImageUrl="~/ControlRoom/Images/DrawWinners.png" 
                        Height="25"
                        Text="Draw"  Tooltip="Draw"
                        AlternateText="Draw"  Visible='<%# FormatHelper.ToNormalDate((DateTime)Eval("DrawingDateTime")) == "N/A"%>'/>
                <asp:Panel ID="pnlWinners" runat="server" Visible='<%# FormatHelper.ToNormalDate((DateTime)Eval("DrawingDateTime")) != "N/A"%>'>

                    <table>
                        <tr>
                            <td>Draw Additional: </td>
                            <td><asp:TextBox ID="addl" runat="server" Text='' Width="50px"></asp:TextBox></td>
                            <td><asp:ImageButton ID="ImageButton1" runat="server" 
                        CausesValidation="true" 
                        CommandName="drawadd" 
                        ImageUrl="~/ControlRoom/Images/DrawWinners.png" 
                        Height="25"
                        Text="Draw"  Tooltip="Draw"
                        AlternateText="Draw" ImageAlign="Baseline" /></td>
                            <td> <asp:RegularExpressionValidator id="reva"
                        ControlToValidate="addl"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Additional  # Winners must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Additional # Winners must be numeric. </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" />      
                    <asp:RangeValidator ID="rva"
                        ControlToValidate="addl"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Additional # Winners must be from 0 to 99!</font>"
                        runat="server" 
                        Font-Bold="True" Font-Italic="True" 
                        Text="<font color='red'> * Additional # Winners must be from 0 to 99! </font>" 
                        EnableTheming="True" 
                        SetFocusOnError="True" /></td>
                        </tr>
                    </table>

                    <hr />

                    <asp:GridView ID="gv2" runat="server" DataSourceID="odsWinners"
                        AllowPaging="False"  AllowSorting="false"
                        Width="100%" AutoGenerateColumns="false"
                        >
                        <Columns>
                            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                <HeaderTemplate>
                                </HeaderTemplate>                
                                <ItemTemplate>
                                    &nbsp;
                                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("PDWID") %>' 
                                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                                    &nbsp;
                                </ItemTemplate>
                                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
                            </asp:TemplateField>

                            
                            <asp:TemplateField HeaderText="Picked Up?" HeaderStyle-Wrap="False">
                                 <ItemTemplate>

                                    <%# (bool)Eval("PrizePickedUpFlag") ? FormatHelper.ToYesNo((bool)Eval("PrizePickedUpFlag")) : ""%>
                                    
                                     <asp:LinkButton ID="LinkButton1" runat="server"
                                        CommandName="pickup" CommandArgument='<%# Bind("PDWID") %>' 
                                        Visible='<%#!(bool)Eval("PrizePickedUpFlag") %>'
                                     >NO</asp:LinkButton>

                                 </ItemTemplate>
                                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" VerticalAlign="Top" />    
                            </asp:TemplateField>

                            <asp:BoundField DataField="Username" HeaderText="Username" HeaderStyle-Wrap="False">
                                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" VerticalAlign="Top" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="FirstName" HeaderText="First Name" HeaderStyle-Wrap="False">
                                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" VerticalAlign="Top" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="LastName" HeaderText="Last Name" HeaderStyle-Wrap="False">
                                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" VerticalAlign="Top" />    
                            </asp:BoundField>                                                    
                        </Columns>
                    </asp:GridView>
                </asp:Panel>

            </EditItemTemplate>
            <InsertItemTemplate>
                
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="DrawingDateTime" runat="server" Text='<%# Eval("DrawingDateTime") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
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
                <hr />
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                <hr />
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
                <hr />
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
        TypeName="GRA.SRP.DAL.PrizeDrawing">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PDID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

	<asp:ObjectDataSource ID="odsWinners" runat="server" 
        SelectMethod="GetAllWinners" 
        TypeName="GRA.SRP.DAL.PrizeDrawing">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PDID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsT" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.PrizeTemplate">
    </asp:ObjectDataSource>

</asp:Content>

