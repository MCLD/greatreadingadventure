<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="PatronReviewEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronReviewEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="~/ControlRoom/Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

    <uc1:PatronContext ID="pcCtl" runat="server" />

    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:BoundField DataField="PRID" HeaderText="PRID: " SortExpression="PRID" ReadOnly="True" InsertVisible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>


        
        <asp:TemplateField HeaderText="Author: " SortExpression="Author" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:Label ID="Author" runat="server" Text='<%# Eval("Author") %>'></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:Label ID="Author" runat="server" Text='<%# Eval("Author") %>'></asp:Label>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Author" runat="server" Text='<%# Eval("Author") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Title: " SortExpression="Title" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:Label ID="Title" runat="server" Text='<%# Eval("Title") %>'></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:Label ID="Title" runat="server" Text='<%# Eval("Title") %>'></asp:Label>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Title" runat="server" Text='<%# Eval("Title") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Review: " SortExpression="Review" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:Label ID="Review" runat="server" Text='<%# Eval("Review") %>'></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:Label ID="Review" runat="server" Text='<%# Eval("Review") %>'></asp:Label>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Review" runat="server" Text='<%# Eval("Review") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="isApproved: " SortExpression="isApproved" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:CheckBox ID="isApproved" runat="server" Checked='<%# (bool)Eval("isApproved") %>' ReadOnly="False"></asp:CheckBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:CheckBox ID="isApproved" runat="server" ReadOnly="False"></asp:CheckBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="isApproved" runat="server" Text='<%# Eval("isApproved") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="ReviewDate: " SortExpression="ReviewDate" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <%# (Eval("ReviewDate") != null && Eval("ReviewDate") is DateTime ?
                                                            FormatHelper.ToNormalDate((DateTime)Eval("ReviewDate")) :
                                        "") %> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <%# (Eval("ReviewDate") != null && Eval("ReviewDate") is DateTime ?
                                                            FormatHelper.ToNormalDate((DateTime)Eval("ReviewDate")) :
                                        "") %> 
            </InsertItemTemplate>
            <ItemTemplate>
                <%# (Eval("ReviewDate") != null && Eval("ReviewDate") is DateTime ?
                                                            FormatHelper.ToNormalDate((DateTime)Eval("ReviewDate")) :
                                        "") %> 
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="ApprovalDate: " SortExpression="ApprovalDate" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <%# (Eval("ApprovalDate") != null && Eval("ApprovalDate") is DateTime ? FormatHelper.ToNormalDate((DateTime)Eval("ApprovalDate")) : "") %> 
            </EditItemTemplate>
            <InsertItemTemplate>
                <%# (Eval("ApprovalDate") != null && Eval("ApprovalDate") is DateTime ? FormatHelper.ToNormalDate((DateTime)Eval("ApprovalDate")) : "") %> 
            </InsertItemTemplate>
            <ItemTemplate>
                <%# (Eval("ApprovalDate") != null && Eval("ApprovalDate") is DateTime ? FormatHelper.ToNormalDate((DateTime)Eval("ApprovalDate")) : "") %> 
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="ApprovedBy: " SortExpression="ApprovedBy" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:Label ID="ApprovedBy" runat="server" Text='<%# Eval("ApprovedBy") %>'></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:Label ID="ApprovedBy" runat="server" Text='<%# Eval("ApprovedBy") %>'></asp:Label>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="ApprovedBy" runat="server" Text='<%# Eval("ApprovedBy") %>'></asp:Label>
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
        TypeName="GRA.SRP.DAL.PatronReview">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PRID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

</asp:Content>

