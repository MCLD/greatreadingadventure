<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="SurveyAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyAddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        OnItemCommand="DvItemCommand" OnDataBinding="dv_DataBinding"
        OnDataBound="dv_DataBound"
        Width="100%">
        <Fields>



            <asp:TemplateField>
                <EditItemTemplate>

                    <table width="100%">
                        <tr>
                            <td width="100px"><b>Admin Name: </b></td>
                            <td>
                                <asp:TextBox ID="Name" runat="server" Text='<%# Eval("Name") %>' ReadOnly="False" Width="250px" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                    ControlToValidate="Name" Display="Dynamic" ErrorMessage="<font color='red'>Admin Name is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                            <td align="right"><b>Status: </b></td>
                            <td colspan="3">

                                <asp:DropDownList ID="Status" runat="server">
                                    <asp:ListItem Value="1" Text="Work In Progress"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Locked / Active"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="StatusLbl" runat="server" Text='<%# Eval("Status") %>' Visible="False"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td><b>Patron Name: </b></td>
                            <td colspan="5">
                                <asp:TextBox ID="LongName" runat="server" Text='<%# Eval("LongName") %>' ReadOnly="False" Width="600px" MaxLength="150"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLongName" runat="server"
                                    ControlToValidate="LongName" Display="Dynamic" ErrorMessage="<font color='red'>Patron Name is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <b>Description:</b><br />
                                <asp:TextBox ID="Description" runat="server" Text='<%# Eval("Description") %>' ReadOnly="False" Width="98%" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" height="200px">
                                <b>Patron Preamble:</b><br />
                                <textarea id="Preamble" runat="server" class="gra-editor"><%#Eval("Preamble") %></textarea>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td align="left"><b>Is A Scorable Test/Survey:</b></td>
                            <td><%# ((bool)Eval("CanBeScored")) %></td>
                            <td align="right"><b># Times Taken:</b></td>
                            <td>0<%# ((int) Eval("TakenCount") ==0 ? "" : Eval("TakenCount")) %></td>
                            <td align="right"><b># Patrons Who Took It:</b></td>
                            <td>0<%# ((int) Eval("PatronCount") ==0 ? "" : Eval("PatronCount")) %></td>
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
                            <td width="100px"><b>Admin Name: </b></td>
                            <td>
                                <asp:TextBox ID="Name" runat="server" Text='' ReadOnly="False" Width="250px" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                    ControlToValidate="Name" Display="Dynamic" ErrorMessage="<font color='red'>Admin Name is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                            <td align="right"><b>Status: </b></td>
                            <td colspan="3">

                                <asp:DropDownList ID="Status" runat="server">
                                    <asp:ListItem Value="1" Text="Work In Progress"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Locked / Active" Enabled="false"></asp:ListItem>
                                </asp:DropDownList>

                            </td>
                        </tr>
                        <tr>
                            <td><b>Patron Name: </b></td>
                            <td colspan="5">
                                <asp:TextBox ID="LongName" runat="server" Text='' ReadOnly="False" Width="600px" MaxLength="150"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLongName" runat="server"
                                    ControlToValidate="LongName" Display="Dynamic" ErrorMessage="<font color='red'>Patron Name is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <b>Description:</b><br />
                                <asp:TextBox ID="Description" runat="server" Text='' ReadOnly="False" Width="98%" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" height="200px">
                                <b>Patron Preamble:</b>
                                <textarea id="Preamble" runat="server" class="gra-editor"></textarea>
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
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>


            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="Back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="Back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                    &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server"
                        CausesValidation="True"
                        CommandName="Add"
                        ImageUrl="~/ControlRoom/Images/add.png"
                        Height="25"
                        Text="Add" ToolTip="Add"
                        AlternateText="Add" />

                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="Back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                    &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server"
                        CausesValidation="false"
                        CommandName="Refresh"
                        ImageUrl="~/ControlRoom/Images/refresh.png"
                        Height="25"
                        Text="Refresh Record" ToolTip="Refresh Record"
                        AlternateText="Refresh Record" />
                    &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server"
                        Visible='<%# (Eval("Status").ToString() == "1" ? true : false) %>'
                        CausesValidation="True"
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png"
                        Height="25"
                        Text="Save" ToolTip="Save"
                        AlternateText="Save" />
                    &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server"
                        Visible='<%# (Eval("Status").ToString() == "1" ? true : false) %>'
                        CausesValidation="True"
                        CommandName="Saveandback"
                        ImageUrl="~/ControlRoom/Images/saveback.png"
                        Height="25"
                        Text="Save and return" ToolTip="Save and return"
                        AlternateText="Save and return" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="btnQuestions" runat="server" AlternateText="Questions" ToolTip="Questions"
                        CausesValidation="False" CommandName="Questions" CommandArgument='<%# Bind("SID") %>'
                        ImageUrl="~/ControlRoom/Images/Questions.png" Width="28px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="btnResults" runat="server" AlternateText="Results" ToolTip="Results"
                        CausesValidation="False" CommandName="Results" CommandArgument='<%# Bind("SID") %>'
                        ImageUrl="~/ControlRoom/Images/Result.png" Width="28px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="ImageButton1" runat="server" AlternateText="Generate Embedding Code" ToolTip="Generate Embedding Code"
                        CausesValidation="False" CommandName="embed" CommandArgument='<%# Bind("SID") %>'
                        ImageUrl="~/ControlRoom/Images/EmbedTestRecipe.png" Width="28px" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="FetchObject"
        TypeName="GRA.SRP.DAL.Survey">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="SID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

