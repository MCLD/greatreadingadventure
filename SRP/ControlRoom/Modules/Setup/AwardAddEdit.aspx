<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="AwardAddEdit.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.AwardAddEdit" 
    
%>


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

        <asp:BoundField DataField="AID" HeaderText="AID: " SortExpression="AID" ReadOnly="True" InsertVisible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>


        <asp:TemplateField HeaderText="Award Name: " SortExpression="AwardName" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="AwardName" runat="server" Text='<%# Eval("AwardName") %>' ReadOnly="False" Width="500px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAwardName" runat="server" 
                    ControlToValidate="AwardName" Display="Dynamic" ErrorMessage="AwardName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AwardName" runat="server" Text=''  Width="500px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAwardName" runat="server" 
                    ControlToValidate="AwardName" Display="Dynamic" ErrorMessage="AwardName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AwardName" runat="server" Text='<%# Eval("AwardName") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Badge Awarded: " SortExpression="BadgeID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID" 
                    AppendDataBoundItems="True" Width="600px"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="BadgeIDLbl" runat="server" Text='<%# Eval("BadgeID") %>' Visible="False"></asp:Label>    
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID" 
                    AppendDataBoundItems="True" Width="600px"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BadgeID" runat="server" Text='<%# Eval("BadgeID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="# Points: " SortExpression="NumPoints" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:TextBox ID="NumPoints" runat="server" Text='<%# ((int) Eval("NumPoints") == 0 ? "0" : Eval("NumPoints")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumPoints" runat="server" 
                    ControlToValidate="NumPoints" Display="Dynamic" ErrorMessage="NumPoints is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revNumPoints"
                    ControlToValidate="NumPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Num Points must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Num Points must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumPoints"
                    ControlToValidate="NumPoints"
                    MinimumValue="0"
                    MaximumValue="99999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Num Points must be from 0 to 99,999!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Num Points must be from 0 to 99,999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="NumPoints" runat="server" Text=''></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumPoints" runat="server" 
                    ControlToValidate="NumPoints" Display="Dynamic" ErrorMessage="Num Points is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revNumPoints"
                    ControlToValidate="NumPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Num Points must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Num Points must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumPoints"
                    ControlToValidate="NumPoints"
                    MinimumValue="0"
                    MaximumValue="99999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>NumPoints must be from 0 to 99,999!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * NumPoints must be from 0 to 99,999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="NumPoints" runat="server" Text='<%# Eval("NumPoints") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Branch: " SortExpression="BranchID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True" Width="97%"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="BranchIDLbl" runat="server" Text='<%# Eval("BranchID") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True" Width="97%"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                 
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BranchID" runat="server" Text='<%# Eval("BranchID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


        <asp:TemplateField HeaderText="Program: " SortExpression="ProgramID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="ProgramID" runat="server" DataSourceID="odsDDPrograms" 
                DataTextField="AdminName" DataValueField="PID" 
                AppendDataBoundItems="True"  Width="97%"
                >
                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList> 
                <asp:Label ID="ProgramIDLbl" runat="server" Text='<%# Eval("ProgramID") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="ProgramID" runat="server" DataSourceID="odsDDPrograms" 
                DataTextField="AdminName" DataValueField="PID" 
                AppendDataBoundItems="True"  Width="97%"
                >
                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem> 
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="ProgramID" runat="server" Text='<%# Eval("ProgramID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="District: " SortExpression="District" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDLibSys" 
                    DataTextField="Code" DataValueField="CID"  
                    AppendDataBoundItems="True"  Width="97%"
                    >
                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem> 
                </asp:DropDownList>
                <asp:Label ID="DistrictLbl" runat="server" Text='<%# Eval("District") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDLibSys" 
                    DataTextField="Code" DataValueField="CID"  
                    AppendDataBoundItems="True"  Width="97%"
                    >
                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem> 
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="District" runat="server" Text='<%# Eval("District") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="School: " SortExpression="SchoolName" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" 
                    DataTextField="Code" DataValueField="CID"  
                    AppendDataBoundItems="True"  Width="97%"
                    >
                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem> 
                </asp:DropDownList>
                <asp:Label ID="SchoolNameLbl" runat="server" Text='<%# Eval("SchoolName") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" 
                    DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"  Width="97%"
                    >
                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem> 
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="SchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Earned Badges List: " SortExpression="BadgeList" HeaderStyle-Wrap="False" InsertVisible="true">
		    <EditItemTemplate>
            <table style="width: 400px" cellpadding=3px;>
    <tr>
        <td align="center" width="400px"> <b>All Defined Badges</b> </td>       
    </tr>    
    <tr>
        <td valign="top">
                <asp:TextBox ID="BadgeList" runat="server" Text='<%# Eval("BadgeList") %>' ReadOnly="False" Visible="false"></asp:TextBox>
                <asp:ObjectDataSource ID="odsBadgeMembership" runat="server" 
                SelectMethod="GetBadgeListMembership" TypeName="STG.SRP.DAL.Award">
                <SelectParameters>
                    <asp:ControlParameter ControlID="BadgeList" DefaultValue="" Name="list" 
                        PropertyName="Text" Type="string" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
<div style="height: 200px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd;  ">       
            <asp:GridView ID="gvBadgeMembership" ShowHeader="false"  runat="server" DataSourceID="odsBadgeMembership" AutoGenerateColumns="false" Width="100%">
                <Columns>
                    <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("isMember")).ToString()=="1"?true:false) %>' runat="server" />   <%# Eval("AdminName") %>
                        <asp:Label ID="BID" runat="server" Text='<%# Eval("BID") %>' Visible="False"></asp:Label>
                    </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
</div>        
        </td>    
        </table>





               
            </EditItemTemplate>
            <InsertItemTemplate>
<table style="width: 400px" cellpadding=3px;>
    <tr>
        <td align="center" width="400px"> <b>All Defined Badges</b> </td>       
    </tr>    
    <tr>
        <td valign="top">
            <asp:ObjectDataSource ID="odsBadgeMembership" runat="server" 
                SelectMethod="GetBadgeListMembership" TypeName="STG.SRP.DAL.Award">
                <SelectParameters>
                    <asp:ControlParameter ControlID="BadgeList" DefaultValue="" Name="list" 
                        PropertyName="Text" Type="string" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
<div style="height: 200px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd;  ">       
            <asp:GridView ID="gvBadgeMembership" ShowHeader="false"  runat="server" DataSourceID="odsBadgeMembership" AutoGenerateColumns="false" Width="100%">
                <Columns>
                    <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("isMember")).ToString()=="1"?true:false) %>' runat="server" />   <%# Eval("AdminName") %>
                        <asp:Label ID="BID" runat="server" Text='<%# Eval("BID") %>' Visible="False"></asp:Label>
                    </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
</div>        
        </td>    
        </table>




                <asp:TextBox ID="BadgeList" runat="server" Text='' ReadOnly="False" Visible="false"></asp:TextBox>            
            
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BadgeList" runat="server" Text='<%# Eval("BadgeList") %>'></asp:Label>
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
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="STG.SRP.DAL.Award">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="AID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

       <asp:ObjectDataSource ID="odsDDBadges" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Badge">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Programs">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDLibSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>


</asp:Content>

