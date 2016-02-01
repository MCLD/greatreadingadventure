<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="MGChooseAdvSlidesList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGChooseAdvSlidesList" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblCAID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblDiff" runat="server" Text="1" Visible="False"></asp:Label>

    <h1>Mini Game: <asp:Label ID="AdminName" runat="server" Text=""></asp:Label></h1>

    	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAllByDifficulty" 
        TypeName="GRA.SRP.DAL.MGChooseAdvSlides">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblMGID" Name="MGID" DefaultValue="0"
                PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="lblDiff" Name="Difficulty" DefaultValue="1"
                PropertyName="Text" Type="Int32" />        
        </SelectParameters>
	</asp:ObjectDataSource>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="CASID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        OnRowDataBound="GvRowDataBound"
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="24px" />

                        &nbsp;&nbsp;

                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </HeaderTemplate>                    
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("CASID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("CASID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                   &nbsp;
                   &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" Tooltip="Move Up" 
                        CausesValidation="False" CommandName="MoveUp" CommandArgument='<%# Bind("CASID") %>'  
                        ImageUrl="~/ControlRoom/Images/Up.gif"  Visible='<%# ((int)Eval("StepNumber")==1 ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("StepNumber")==1 ? true : false) %>' Width="21px"/>
                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" Tooltip="Move Down" 
                        CausesValidation="False" CommandName="MoveDn" CommandArgument='<%# Bind("CASID") %>' 
                        ImageUrl="~/ControlRoom/Images/Dn.gif"   Visible='<%# ((int)Eval("StepNumber")==(int)Eval("MAX") ? false : true) %>' Width="21px"/>
                    <asp:ImageButton ID="ImageButton2" runat="server"
                        CausesValidation="False" 
                        ImageUrl="~/ControlRoom/Images/Spacer.gif"  Visible='<%# ((int)Eval("StepNumber")==(int)Eval("MAX") ? true : false) %>' Width="21px"/>
                   &nbsp;

                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="CASID" DataField="CASID" 
                SortExpression="CASID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          

            <asp:TemplateField   SortExpression="StepNumber" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Step Number">              
                <ItemTemplate> 
                    <%# FormatHelper.ToInt((int)Eval("StepNumber"))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 

			<asp:BoundField ReadOnly="True" HeaderText="Slide Text" 
                DataField="SlideText" SortExpression="SlideText" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="true"></ItemStyle>
            </asp:BoundField> 

            <asp:TemplateField HeaderText="Slide Text"   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">             
                <ItemTemplate>
                    <asp:Literal ID="SlideTextDisplay" runat="server"></asp:Literal>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="True" HorizontalAlign="Left"></ItemStyle>
                <HeaderStyle  Wrap="True" HorizontalAlign="Left" width="800px"/>
            </asp:TemplateField> 

            <asp:TemplateField HeaderText="Image 1"   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">             
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server"  width="24px"
                        ImageUrl='<%# String.Format("~/Images/Games/ChooseAdv/sm_i1_{0}.png?{1}", Eval("CASID").ToString(), DateTime.Now.ToString()) %>' />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
                <HeaderStyle  Wrap="False" HorizontalAlign="Left" />
            </asp:TemplateField> 


            <asp:TemplateField   SortExpression="FirstImageGoToStep" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Image 1 Goes To Step">              
                <ItemTemplate> 
                    <%# (FormatHelper.ToInt((int)Eval("FirstImageGoToStep"))=="0" ? "" : FormatHelper.ToInt((int)Eval("FirstImageGoToStep")))%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>
            
            <asp:TemplateField  HeaderText="Image 2"  ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">           
                <ItemTemplate>
                    <asp:Image ID="Image2" runat="server"   width="24px"
                        ImageUrl='<%# String.Format("~/Images/Games/ChooseAdv/sm_i2_{0}.png?{1}", Eval("CASID").ToString(), DateTime.Now.ToString()) %>' />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" HorizontalAlign="Left"></ItemStyle>
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:TemplateField> 
                        	 			 

            <asp:TemplateField   SortExpression="SecondImageGoToStep" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="Image 2 Goes To Step">              
                <ItemTemplate>
                    <%# (FormatHelper.ToInt((int)Eval("SecondImageGoToStep"))=="0" ? "" : FormatHelper.ToInt((int)Eval("SecondImageGoToStep")))%> 

                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 


			<asp:BoundField ReadOnly="True" HeaderText="Modified On" 
                DataField="LastModDate" SortExpression="LastModDate" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Modified By" 
                DataField="LastModUser" SortExpression="LastModUser" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Added On" 
                DataField="AddedDate" SortExpression="AddedDate" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Added By" 
                DataField="AddedUser" SortExpression="AddedUser" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>			


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />


                        &nbsp;&nbsp;

                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

