<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master"
    AutoEventWireup="true" CodeBehind="SurveyQuestionAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyQuestionAddEdit" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />
    <hr />
    <asp:Label ID="lblSurvey" runat="server" Text="Label" Font-Bold="True" Font-Size="Large"></asp:Label>
    <asp:Label ID="SID" runat="server" Text="Label" Font-Bold="True" Font-Size="Large"></asp:Label>
    <asp:Label ID="ReadOnly" runat="server" Text="" Visible="false"></asp:Label>
    <hr />

    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        OnItemCommand="DvItemCommand" OnDataBinding="dv_DataBinding"
        OnDataBound="dv_DataBound"
        Width="100%">
        <Fields>

            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>

                    <table width="100%">
                        <tr>
                            <td nowrap width="110px"><b>Question Type: </b></td>
                            <td colspan="3">
                                <asp:Label ID="QType" runat="server" Text='<%# Eval("QType") %>' Visible="false"></asp:Label><%# SurveyQuestion.TypeDescription((int)Eval("QType")) %>
                            <td nowrap><b></b></td>
                            <td colspan="3"></td>
                        </tr>
                    </table>

                    <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
                        Height="500px"
                        Width="100%"
                        AutoPostBack="false"
                        TabStripPlacement="Top"
                        CssClass="ajax__tab_xp"
                        ScrollBars="None"
                        UseVerticalStripPlacement="false"
                        VerticalStripWidth="120px">
                        <ajaxToolkit:TabPanel runat="server"
                            HeaderText="Question"
                            ID="TabPanel1"
                            Enabled="true"
                            ScrollBars="Auto">
                            <ContentTemplate>


                                <asp:Panel ID="pnlType1" runat="server"
                                    BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Instructions/Text/Description Details" ScrollBars="Auto" CssClass="OrangePanel"
                                    Width="100%" Height="475px" Visible="false">
                                    <table width="100%" height="400px">
                                        <tr>
                                            <td height="350px"><b>Instructions/Text/Description: </b>
                                                <textarea id="QText" runat="server" class="gra-editor"><%#Eval("QText") %></textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="100%"></td>
                                        </tr>
                                    </table>

                                </asp:Panel>

                                <asp:Panel ID="pnlType3" runat="server"
                                    BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Free Form Details" ScrollBars="Auto" CssClass="OrangePanel"
                                    Width="100%" Height="475px" Visible="false">
                                    <table width="100%" height="430px">
                                        <tr>
                                            <td><b>Admin Description: </b></td>
                                            <td>
                                                <asp:TextBox ID="QName3" runat="server" Text='<%#Eval("QName") %>' Width="500" MaxLength="150"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b>Is Answer Required?: </b></td>
                                            <td>
                                                <asp:CheckBox ID="IsRequired3" runat="server" ReadOnly="False" Checked='<%# (bool)Eval("IsRequired") %>'></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="250px" colspan="2"><b>Question To Patron: </b>
                                                <textarea id="QText3" runat="server" class="gra-editor"><%#Eval("QText") %></textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="100%"></td>
                                        </tr>

                                    </table>
                                </asp:Panel>

                                <asp:Panel ID="pnlType2" runat="server"
                                    BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Multiple Choice Details" ScrollBars="Auto" CssClass="OrangePanel"
                                    Width="100%" Height="525px" Visible="false">
                                    <table width="100%" height="400px">
                                        <tr>
                                            <td><b>Admin Description: </b></td>
                                            <td colspan="3">
                                                <asp:TextBox ID="QName2" runat="server" Text='<%#Eval("QName") %>' Width="500" MaxLength="150"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b>Is Answer Required?: </b></td>
                                            <td>
                                                <asp:CheckBox ID="IsRequired2" runat="server" ReadOnly="False" Checked='<%# (bool)Eval("IsRequired") %>'></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b>How To Display Answers: </b></td>
                                            <td>
                                                <asp:DropDownList ID="DisplayControl2" runat="server">
                                                    <asp:ListItem Value="1" Text="Checkboxes - Patron can select MULTIPLE answers"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Radiobuttons - Patron can select ONLY ONE answer"></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="Dropdown list - Patron can select ONLY ONE answer"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="DisplayControl2Lbl" runat="server" Text='<%# Eval("DisplayControl") %>' Visible="False"></asp:Label>
                                            </td>
                                            <td><b>Multiple Choice Direction (applies to Checkboxes and Radiobuttons only): </b></td>
                                            <td>
                                                <asp:DropDownList ID="DisplayDirection2" runat="server">
                                                    <asp:ListItem Value="1" Text="Vertical"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Horizontal"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="DisplayDirection2Lbl" runat="server" Text='<%# Eval("DisplayDirection") %>' Visible="False"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="300px" colspan="4"><b>Question To Patron: </b>
                                                <textarea id="QText2" runat="server" class="gra-editor"><%#Eval("QText") %></textarea>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td height="100%"></td>

                                        </tr>
                                    </table>
                                </asp:Panel>

                                <asp:Panel ID="pnlType4" runat="server"
                                    BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Matrix of Questions Details" ScrollBars="Auto" CssClass="OrangePanel"
                                    Width="100%" Height="525px" Visible="false">
                                    <table width="100%" height="400px">
                                        <tr>
                                            <td><b>Admin Description: </b></td>
                                            <td colspan="3">
                                                <asp:TextBox ID="QName4" runat="server" Text='<%#Eval("QName") %>' Width="500" MaxLength="150"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b>Is Answer Required?: </b></td>
                                            <td>
                                                <asp:CheckBox ID="IsRequired4" runat="server" ReadOnly="False" Checked='<%# (bool)Eval("IsRequired") %>'></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b>How To Display Answers/Choices: </b></td>
                                            <td>
                                                <asp:DropDownList ID="DisplayControl4" runat="server">
                                                    <asp:ListItem Value="1" Text="Checkboxes - Patron can select MULTIPLE answers"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Radiobuttons - Patron can select ONLY ONE answer"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="DisplayControl4Lbl" runat="server" Text='<%# Eval("DisplayControl") %>' Visible="False"></asp:Label>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td height="300px" colspan="4"><b>Question To Patron: </b>
                                                <textarea id="QText4" runat="server" class="gra-editor"><%#Eval("QText") %></textarea>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td height="100%" valign="bottom"></td>
                                        </tr>

                                    </table>
                                </asp:Panel>


                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel runat="server"
                            HeaderText="Question Answers"
                            ID="TabPanel2"
                            Enabled="true" Visible="false"
                            ScrollBars="Auto">
                            <ContentTemplate>


                                <asp:Panel ID="pnlType2Answers" runat="server"
                                    BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Multiple Choice Answers/Choices" ScrollBars="Auto" CssClass="OrangePanel"
                                    Width="100%" Height="525px" Visible="false">
                                    <table width="100%" height="400px">
                                        <tr>
                                            <td><b>Multiple Choice Answer: </b></td>

                                        </tr>
                                        <tr runat="server" id="id11" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td width=""><b>Value</b> </td>
                                            <td width="50px" align="center"><b>Assoc. Score</b> </td>
                                            <td width="250px"><b>Jump to Question</b> </td>
                                            <td width="170px"><b>Ask for Clarification?</b> </td>
                                            <td width="170px"><b>Clarification Required?</b> </td>
                                            <td></td>
                                        </tr>
                                        <tr runat="server" id="id1" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td>
                                                <asp:TextBox ID="ChoiceText2" runat="server" Text='' ReadOnly="False" Width="550px"></asp:TextBox>

                                            </td>
                                            <td align="right">
                                                <asp:TextBox ID="Score2" runat="server" Text='' Width="50px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revScore2"
                                                    ControlToValidate="Score2"
                                                    ValidationExpression="\d+"
                                                    Display="Dynamic"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Score must be numeric.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True"
                                                    Text="<br><font color='red'> * Score must be numeric. </font>"
                                                    EnableTheming="True"
                                                    SetFocusOnError="True" />
                                                <asp:RangeValidator ID="rvScore2"
                                                    ControlToValidate="Score2"
                                                    MinimumValue="0"
                                                    MaximumValue="9999"
                                                    Display="Dynamic"
                                                    Type="Integer"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Score must be from 0 to 99!</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True"
                                                    Text="<br><font color='red'> * Score must be from 0 to 99! </font>"
                                                    EnableTheming="True"
                                                    SetFocusOnError="True" />

                                            </td>
                                            <td>
                                                <asp:DropDownList ID="JumpToQuestion2" runat="server">
                                                    <asp:ListItem Value="0" Text="No Skip Logic On This Answer"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="AskClarification2" runat="server" ReadOnly="False"></asp:CheckBox></td>
                                            <td>
                                                <asp:CheckBox ID="ClarificationRequired2" runat="server" ReadOnly="False"></asp:CheckBox></td>
                                            <td>
                                                <asp:Button ID="btnAddAnswer2" runat="server" Text=" Add " CommandName="addrecord21" CommandArgument="-1" />
                                            </td>
                                        </tr>
                                        <tr runat="server" id="id111" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td colspan="6">
                                                <hr />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6" height="330px">

                                                <asp:Panel ID="Panel1" runat="server" Width="100%" ScrollBars="Vertical" Height="330px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">


                                                    <asp:GridView ID="gv21" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
                                                        DataKeys="SQCID"
                                                        DataSourceID="odsData41"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:BoundField ReadOnly="True" HeaderText="ChoiceText"
                                                                DataField="ChoiceText" SortExpression="ChoiceText" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>

                                                            <asp:TemplateField SortExpression="Score" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                                HeaderText="Score">
                                                                <ItemTemplate>
                                                                    <%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score"))%>
                                                                </ItemTemplate>
                                                                <ControlStyle Width="50px" />
                                                                <ItemStyle Width="50px" VerticalAlign="Top" Wrap="False" HorizontalAlign="Right"></ItemStyle>
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField SortExpression="JumpToQuestion" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                                HeaderText="Jump To Question">
                                                                <ItemTemplate>
                                                                    <%# FormatHelper.ToWidgetDisplayInt((int)Eval("JumpToQuestion"))%>
                                                                </ItemTemplate>
                                                                <ControlStyle Width="100px" />
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="100px" VerticalAlign="Top" HorizontalAlign="Center"></ItemStyle>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField SortExpression="AskClarification" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                                HeaderText="Ask For Clarification">
                                                                <ItemTemplate>
                                                                    <%# FormatHelper.ToYesNo((bool)Eval("AskClarification"))%>
                                                                </ItemTemplate>
                                                                <ControlStyle Width="100px" />
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="100px" VerticalAlign="Top" HorizontalAlign="Center"></ItemStyle>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField SortExpression="ClarificationRequired" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                                HeaderText="Clarification Required">
                                                                <ItemTemplate>
                                                                    <%# FormatHelper.ToYesNo((bool)Eval("ClarificationRequired"))%>
                                                                </ItemTemplate>
                                                                <ControlStyle Width="100px" />
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="100px" VerticalAlign="Top" HorizontalAlign="Center"></ItemStyle>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                                                                        CausesValidation="False" CommandName="DeleteRecord21" CommandArgument='<%# Bind("SQCID") %>'
                                                                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                                        Visible='<%# ReadOnly.Text.Length == 0 %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle VerticalAlign="Top" Wrap="False" Width="20px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" ToolTip="Move Up"
                        CausesValidation="False" CommandName="MoveUp21" CommandArgument='<%# Bind("SQCID") %>'
                        ImageUrl="~/ControlRoom/Images/Up.gif" Visible='<%# ((int)Eval("ChoiceOrder")==1 ? false : ReadOnly.Text.Length == 0) %>' Width="21px" />
                                                                    <asp:ImageButton ID="ImageButton1" runat="server"
                                                                        CausesValidation="False"
                                                                        ImageUrl="~/ControlRoom/Images/Spacer.gif" Visible='<%# ((int)Eval("ChoiceOrder")==1 ? true : false) %>' Width="21px" />
                                                                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" ToolTip="Move Down"
                        CausesValidation="False" CommandName="MoveDn21" CommandArgument='<%# Bind("SQCID") %>'
                        ImageUrl="~/ControlRoom/Images/Dn.gif" Visible='<%# ((int)Eval("ChoiceOrder")==(int)Eval("MAX") ? false : ReadOnly.Text.Length == 0) %>' Width="21px" />
                                                                    <asp:ImageButton ID="ImageButton2" runat="server"
                                                                        CausesValidation="False"
                                                                        ImageUrl="~/ControlRoom/Images/Spacer.gif" Visible='<%# ((int)Eval("ChoiceOrder")==(int)Eval("MAX") ? true : false) %>' Width="21px" />
                                                                    &nbsp;
                                                                </ItemTemplate>
                                                                <ItemStyle VerticalAlign="Top" Wrap="False" Width="40px"></ItemStyle>
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>


                                                </asp:Panel>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="100%"></td>

                                        </tr>

                                    </table>
                                </asp:Panel>

                                <asp:Panel ID="pnlType4Answers" runat="server"
                                    BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Matrix of Questions Answers/Choices" ScrollBars="Auto" CssClass="OrangePanel"
                                    Width="100%" Height="525px" Visible="false">
                                    <table width="100%" height="400px">
                                        <tr>
                                            <td colspan="4"><b>Multiple Choice Answer: </b></td>
                                        </tr>
                                        <tr runat="server" id="id12" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td width="250px"><b>Value</b> </td>
                                            <td width="100px"><b>Associated Score</b> </td>
                                            <td width="250px"><b>Jump to Question</b> </td>
                                            <td></td>
                                        </tr>
                                        <tr runat="server" id="id2" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td>
                                                <asp:TextBox ID="ChoiceText4" runat="server" Text='' ReadOnly="False" Width="750px"></asp:TextBox>
                                            </td>
                                            <td align="right">
                                                <asp:TextBox ID="Score4" runat="server" Text='' Width="50px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                                    ControlToValidate="Score4"
                                                    ValidationExpression="\d+"
                                                    Display="Dynamic"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Score must be numeric.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True"
                                                    Text="<br><font color='red'> * Score must be numeric. </font>"
                                                    EnableTheming="True"
                                                    SetFocusOnError="True" />
                                                <asp:RangeValidator ID="RangeValidator1"
                                                    ControlToValidate="Score4"
                                                    MinimumValue="0"
                                                    MaximumValue="9999"
                                                    Display="Dynamic"
                                                    Type="Integer"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Score must be from 0 to 99!</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True"
                                                    Text="<br><font color='red'> * Score must be from 0 to 99! </font>"
                                                    EnableTheming="True"
                                                    SetFocusOnError="True" />

                                            </td>
                                            <td>
                                                <asp:DropDownList ID="JumpToQuestion4" runat="server">
                                                    <asp:ListItem Value="0" Text="No Skip Logic On This Answer"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Button ID="Button1" runat="server" Text=" Add " CommandName="addrecord41" CommandArgument="-1" />
                                            </td>
                                        </tr>
                                        <tr runat="server" id="id15" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td colspan="4">
                                                <hr />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" height="125px">

                                                <asp:Panel ID="Panel2" runat="server" Width="100%" ScrollBars="Vertical" Height="125px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">

                                                    <asp:GridView ID="gv41" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
                                                        DataKeys="SQCID"
                                                        DataSourceID="odsData41"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:BoundField ReadOnly="True" HeaderText="ChoiceText"
                                                                DataField="ChoiceText" SortExpression="ChoiceText" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>

                                                            <asp:TemplateField SortExpression="Score" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                                HeaderText="Score">
                                                                <ItemTemplate>
                                                                    <%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score"))%>
                                                                </ItemTemplate>
                                                                <ControlStyle Width="50px" />
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                                <ItemStyle Width="50px" VerticalAlign="Top" Wrap="False" HorizontalAlign="Right"></ItemStyle>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField SortExpression="JumpToQuestion" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                                HeaderText="Jump To Question">
                                                                <ItemTemplate>
                                                                    <%# FormatHelper.ToWidgetDisplayInt((int)Eval("JumpToQuestion"))%>
                                                                </ItemTemplate>
                                                                <ControlStyle Width="150px" />
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="150px" VerticalAlign="Top" Wrap="False" HorizontalAlign="Center"></ItemStyle>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                                                                        CausesValidation="False" CommandName="DeleteRecord41" CommandArgument='<%# Bind("SQCID") %>'
                                                                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                                        Visible='<%# ReadOnly.Text.Length == 0 %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle VerticalAlign="Top" Wrap="False" Width="20px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" ToolTip="Move Up"
                        CausesValidation="False" CommandName="MoveUp41" CommandArgument='<%# Bind("SQCID") %>'
                        ImageUrl="~/ControlRoom/Images/Up.gif" Visible='<%# ((int)Eval("ChoiceOrder")==1 ? false : ReadOnly.Text.Length == 0) %>' Width="21px" />
                                                                    <asp:ImageButton ID="ImageButton1" runat="server"
                                                                        CausesValidation="False"
                                                                        ImageUrl="~/ControlRoom/Images/Spacer.gif" Visible='<%# ((int)Eval("ChoiceOrder")==1 ? true : false) %>' Width="21px" />
                                                                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" ToolTip="Move Down"
                        CausesValidation="False" CommandName="MoveDn41" CommandArgument='<%# Bind("SQCID") %>'
                        ImageUrl="~/ControlRoom/Images/Dn.gif" Visible='<%# ((int)Eval("ChoiceOrder")==(int)Eval("MAX") ? false : ReadOnly.Text.Length == 0) %>' Width="21px" />
                                                                    <asp:ImageButton ID="ImageButton2" runat="server"
                                                                        CausesValidation="False"
                                                                        ImageUrl="~/ControlRoom/Images/Spacer.gif" Visible='<%# ((int)Eval("ChoiceOrder")==(int)Eval("MAX") ? true : false) %>' Width="21px" />
                                                                    &nbsp;
                                                                </ItemTemplate>
                                                                <ItemStyle VerticalAlign="Top" Wrap="False" Width="40px"></ItemStyle>
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>



                                                </asp:Panel>

                                            </td>
                                        </tr>

                                        <tr>
                                            <td><b>Matrix Lines: </b></td>
                                        </tr>
                                        <tr runat="server" id="id13" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td colspan="6"><b>Line Text</b> </td>
                                            <td></td>
                                        </tr>
                                        <tr runat="server" id="id3" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td>
                                                <asp:TextBox ID="LineText4" runat="server" Text='' ReadOnly="False" Width="750px"></asp:TextBox>

                                            </td>

                                            <td colspan="3">
                                                <asp:Button ID="Button2" runat="server" Text=" Add " CausesValidation="True" CommandName="addrecordl" CommandArgument="-1" />
                                            </td>
                                        </tr>
                                        <tr runat="server" id="id14" visible='<%# ReadOnly.Text.Length == 0 %>'>
                                            <td colspan="6">
                                                <hr />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6" height="125px">

                                                <asp:Panel ID="Panel3" runat="server" Width="100%" ScrollBars="Vertical" Height="125px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">

                                                    <asp:GridView ID="gv42" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
                                                        DataKeys="SQCID"
                                                        DataSourceID="odsData42"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:BoundField ReadOnly="True" HeaderText="LineText"
                                                                DataField="LineText" SortExpression="LineText" Visible="True"
                                                                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
                                                            </asp:BoundField>

                                                            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                                                                        CausesValidation="False" CommandName="DeleteRecordL" CommandArgument='<%# Bind("SQMLID") %>'
                                                                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                                        Visible='<%# ReadOnly.Text.Length == 0 %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle VerticalAlign="Top" Wrap="False" Width="20px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    &nbsp;
                    <asp:ImageButton ID="btnMoveUp" runat="server" AlternateText="Move Up" ToolTip="Move Up"
                        CausesValidation="False" CommandName="MoveUpL" CommandArgument='<%# Bind("SQMLID") %>'
                        ImageUrl="~/ControlRoom/Images/Up.gif" Visible='<%# ((int)Eval("LineOrder")==1 ? false : ReadOnly.Text.Length == 0) %>' Width="21px" />
                                                                    <asp:ImageButton ID="ImageButton1" runat="server"
                                                                        CausesValidation="False"
                                                                        ImageUrl="~/ControlRoom/Images/Spacer.gif" Visible='<%# ((int)Eval("LineOrder")==1 ? true : false) %>' Width="21px" />
                                                                    &nbsp;
                    <asp:ImageButton ID="btnMoveDn" runat="server" AlternateText="Move Down" ToolTip="Move Down"
                        CausesValidation="False" CommandName="MoveDnL" CommandArgument='<%# Bind("SQMLID") %>'
                        ImageUrl="~/ControlRoom/Images/Dn.gif" Visible='<%# ((int)Eval("LineOrder")==(int)Eval("MAX") ? false : ReadOnly.Text.Length == 0) %>' Width="21px" />
                                                                    <asp:ImageButton ID="ImageButton2" runat="server"
                                                                        CausesValidation="False"
                                                                        ImageUrl="~/ControlRoom/Images/Spacer.gif" Visible='<%# ((int)Eval("LineOrder")==(int)Eval("MAX") ? true : false) %>' Width="21px" />
                                                                    &nbsp;
                                                                </ItemTemplate>
                                                                <ItemStyle VerticalAlign="Top" Wrap="False" Width="40px"></ItemStyle>
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>



                                                </asp:Panel>

                                            </td>
                                        </tr>



                                        <tr>
                                            <td height="100%"></td>

                                        </tr>

                                    </table>
                                </asp:Panel>

                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>

                </EditItemTemplate>
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
                        Visible='<%# ReadOnly.Text.Length == 0 %>'
                        CausesValidation="True"
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png"
                        Height="25"
                        Text="Save" ToolTip="Save"
                        AlternateText="Save" />
                    &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server"
                        Visible='<%# ReadOnly.Text.Length == 0 %>'
                        CausesValidation="True"
                        CommandName="Saveandback"
                        ImageUrl="~/ControlRoom/Images/saveback.png"
                        Height="25"
                        Text="Save and return" ToolTip="Save and return"
                        AlternateText="Save and return" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="FetchObject"
        TypeName="GRA.SRP.DAL.SurveyQuestion">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="QID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>

    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsData41" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.SQChoices">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="QID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsData42" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.SQMatrixLines">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="QID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
