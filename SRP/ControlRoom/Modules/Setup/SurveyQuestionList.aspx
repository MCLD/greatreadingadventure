<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="SurveyQuestionList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyQuestionList" 

%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="SID" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.SurveyQuestion">
        <SelectParameters>
            <asp:ControlParameter ControlID="SID" Name="SID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>
    <asp:Label ID="lblSurvey" runat="server" Text="Label" Font-Bold="True" Font-Size="Large"></asp:Label>
    <asp:Label ID="ReadOnly" runat="server" Text="" Visible="false"></asp:Label>
    <hr />

    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False" Width="100%"
        DataKeys="QID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    &nbsp;&nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" 
                        Visible='<%# ReadOnly.Text.Length == 0 %>'
                        />
                    &nbsp;
                    <asp:ImageButton ID="ImageButton3" runat="server" AlternateText="Back/Cancel" Tooltip="Back/Cancel"
                        CausesValidation="False" CommandName="back" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/back.png" Width="20px" />
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("QID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" 
                        Visible='<%# (int)Eval("QType") != 5 && (int)Eval("QType") != 6  %>'
                        />
                    <asp:Image ID="ImageButton4" runat="server"   
                        ImageUrl="~/ControlRoom/Images/spacer.gif" Width="20px" 
                        Visible='<%# (int)Eval("QType") == 5 || (int)Eval("QType") == 6  %>'
                        />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("QID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"
                        Visible='<%# ReadOnly.Text.Length == 0 %>'
                        />
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="40px"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    &nbsp;&nbsp;
                    <asp:ImageButton ID="btnPreview" runat="server" AlternateText="Preview" Tooltip="Preview"
                        CausesValidation="False" CommandName="Preview" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/preview.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnResults" runat="server" AlternateText="Results" Tooltip="Results" 
                        CausesValidation="False" CommandName="Results" CommandArgument='-1'  
                        ImageUrl="~/ControlRoom/Images/Result.png" Width="20px" />   
                </HeaderTemplate>              
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" Tooltip="Move Up" 
                        CausesValidation="False" CommandName="MoveUp" CommandArgument='<%# Bind("QID") %>'  
                        ImageUrl="~/ControlRoom/Images/Up.gif"  
                        Visible='<%# ((int)Eval("QNumber")==1 ? false : ReadOnly.Text.Length == 0) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("QNumber")==1 ? true : false) %>' Width="21px"/>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" Tooltip="Move Down" 
                        CausesValidation="False" CommandName="MoveDn" CommandArgument='<%# Bind("QID") %>' 
                        ImageUrl="~/ControlRoom/Images/Dn.gif"   Visible='<%# ((int)Eval("QNumber")==(int)Eval("MAX") ? false : ReadOnly.Text.Length == 0) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton2" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("QNumber")==(int)Eval("MAX") ? true : false) %>' Width="21px"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="80px"></ItemStyle>
            </asp:TemplateField>			 

            <asp:TemplateField   SortExpression="QNumber" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Q #">              
                <ItemTemplate> 
                    <%# Eval("QNumber") %> &nbsp;&nbsp;&nbsp;
                </ItemTemplate> 
                <ItemStyle Width="30px" VerticalAlign="Top" Wrap="False" HorizontalAlign="Right"></ItemStyle> 
            </asp:TemplateField>	 

            <asp:TemplateField   SortExpression="QType" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Type">              
                <ItemTemplate> 
                    <%# SurveyQuestion.TypeDescription((int)Eval("QType"))%> 
                </ItemTemplate> 
                <ItemStyle    Width="170px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

			<asp:BoundField ReadOnly="True" HeaderText="Admin Name" 
                DataField="QName" SortExpression="QName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:Templatefield HeaderText="Patron Text" 
                SortExpression="QText" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate> 
                    <%# RenderHTML(Eval("QText").ToString()) %> 
                </ItemTemplate> 
				 <ControlStyle  />
                <ItemStyle   VerticalAlign="Top" Wrap="true"></ItemStyle>
            </asp:Templatefield> 

        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

