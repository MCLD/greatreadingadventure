<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master"
    AutoEventWireup="true" CodeBehind="SurveyQuestionAddWizard.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyQuestionAddWizard" %>

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

    <hr />

    <table width="100%">
        <tr>
            <td nowrap width="110px"><b>Question Type: </b></td>
            <td colspan="3">
                <asp:DropDownList ID="QType" runat="server" AutoPostBack="True"
                    OnSelectedIndexChanged="QType_SelectedIndexChanged">
                    <asp:ListItem Value="0" Text="[Select A Choice]"></asp:ListItem>
                    <asp:ListItem Value="1" Text="Instructions/Text/Description"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Multiple Choice"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Free Form Text"></asp:ListItem>
                    <asp:ListItem Value="4" Text="Matrix of Questions"></asp:ListItem>
                    <asp:ListItem Value="5" Text="Page Break"></asp:ListItem>
                    <asp:ListItem Value="6" Text="Survey/Test END"></asp:ListItem>
                </asp:DropDownList>



            <td nowrap><b></b></td>
            <td colspan="3"></td>
        </tr>
    </table>
    <asp:Panel ID="pnlEnd" runat="server" Width="100%" Visible="false"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Page Break & Survey/Test END Details " ScrollBars="Auto" CssClass="OrangePanel">
        <table width="100%" height="100px">
            <tr>
                <td colspan="6" height="100%" valign="bottom">For these types of pseudo-questions there are no additional details.  Click Continue to save and return to the List of Questions.
                <hr />
                </td>
            </tr>
            <tr>
                <td colspan="6" height="100%" valign="bottom">
                    <asp:ImageButton ID="btnCancel0" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                &nbsp;&nbsp;
                &nbsp;&nbsp;
                &nbsp;&nbsp;

                &nbsp;

                <asp:ImageButton ID="btnSave0" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnSave0_Click" />
                </td>
            </tr>

        </table>
    </asp:Panel>

    <asp:Panel ID="pnlType1" runat="server"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Instructions/Text/Description Details" ScrollBars="Auto" CssClass="OrangePanel"
        Width="100%" Height="585px" Visible="false">
        <table width="100%" height="550px">
            <tr>
                <td height="400px"><b>Instructions/Text/Description: </b>
                    <textarea id="QText" runat="server" class="gra-editor"></textarea>
                </td>
            </tr>
            <tr>
                <td height="100%"></td>
            </tr>

            <tr>
                <td height="100%" valign="bottom">
                    <asp:ImageButton ID="btnCancel1" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                &nbsp;&nbsp;
                &nbsp;&nbsp;
                &nbsp;&nbsp;

                &nbsp;

                <asp:ImageButton ID="btnSave1" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnSave1_Click" />
                </td>
            </tr>

        </table>

    </asp:Panel>

    <asp:Panel ID="pnlType3" runat="server"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Free Form Details" ScrollBars="Auto" CssClass="OrangePanel"
        Width="100%" Height="585px" Visible="false">
        <table width="100%" height="550px">
            <tr>
                <td><b>Admin Description: </b></td>
                <td>
                    <asp:TextBox ID="QName3" runat="server" Text='' Width="500" MaxLength="150"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td><b>Is Answer Required?: </b></td>
                <td>
                    <asp:CheckBox ID="IsRequired3" runat="server" ReadOnly="False"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td height="400px" colspan="2"><b>Question To Patron: </b>
                    <textarea id="QText3" runat="server" class="gra-editor"></textarea>
                </td>

            </tr>
            <tr>
                <td height="100%"></td>

            </tr>

            <tr>
                <td height="100%" valign="bottom">
                    <asp:ImageButton ID="btnCancel3" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                &nbsp;&nbsp;
                &nbsp;&nbsp;
                &nbsp;&nbsp;

                &nbsp;

                <asp:ImageButton ID="btnSave3" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnSave3_Click" />
                </td>
            </tr>

        </table>
    </asp:Panel>

    <asp:Panel ID="pnlType2" runat="server"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Multiple Choice Details" ScrollBars="Auto" CssClass="OrangePanel"
        Width="100%" Height="585px" Visible="false">
        <table width="100%" height="400px">
            <tr>
                <td><b>Admin Description: </b></td>
                <td colspan="3">
                    <asp:TextBox ID="QName2" runat="server" Text='' Width="500" MaxLength="150"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td><b>Is Answer Required?: </b></td>
                <td>
                    <asp:CheckBox ID="IsRequired2" runat="server" ReadOnly="False"></asp:CheckBox>
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
                </td>
                <td><b>Multiple Choice Direction (applies to Checkboxes and Radiobuttons only): </b></td>
                <td>
                    <asp:DropDownList ID="DisplayDirection2" runat="server">
                        <asp:ListItem Value="1" Text="Vertical"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Horizontal"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td height="300px" colspan="4"><b>Question To Patron: </b>
                    <textarea id="QText2" runat="server" class="gra-editor"></textarea>
                </td>

            </tr>
            <tr>
                <td height="100%"></td>

            </tr>

            <tr>
                <td height="100%" valign="bottom">
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                &nbsp;&nbsp;
                &nbsp;&nbsp;
                &nbsp;&nbsp;

                &nbsp;

                <asp:ImageButton ID="btnContinue2" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnContinue2_Click" />
                </td>
            </tr>

        </table>
    </asp:Panel>

    <asp:Panel ID="pnlType4" runat="server"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Matrix of Questions Details" ScrollBars="Auto" CssClass="OrangePanel"
        Width="100%" Height="585px" Visible="false">
        <table width="100%" height="400px">
            <tr>
                <td><b>Admin Description: </b></td>
                <td colspan="3">
                    <asp:TextBox ID="QName4" runat="server" Text='' Width="500" MaxLength="150"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td><b>Is Answer Required?: </b></td>
                <td>
                    <asp:CheckBox ID="IsRequired4" runat="server" ReadOnly="False"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td><b>How To Display Answers/Choices: </b></td>
                <td>
                    <asp:DropDownList ID="DisplayControl4" runat="server">
                        <asp:ListItem Value="1" Text="Checkboxes - Patron can select MULTIPLE answers"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Radiobuttons - Patron can select ONLY ONE answer"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td height="300px" colspan="4"><b>Question To Patron: </b>
                    <textarea id="QText4" runat="server" class="gra-editor"></textarea>
                </td>
            </tr>

            <tr>
                <td height="100%" valign="bottom">
                    <asp:ImageButton ID="ImageButton3" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                &nbsp;&nbsp;
                &nbsp;&nbsp;
                &nbsp;&nbsp;

                &nbsp;

                <asp:ImageButton ID="btnContinue4" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnContinue4_Click" />
                </td>
            </tr>

        </table>
    </asp:Panel>

    <asp:Panel ID="pnlType2Answers" runat="server"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Multiple Choice Answers/Choices" ScrollBars="Auto" CssClass="OrangePanel"
        Width="100%" Height="585px" Visible="false">
        <table width="100%" height="530px">
            <tr>
                <td><b>Multiple Choice Answer: </b></td>

            </tr>
            <tr>
                <td width="250px"><b>Value</b> </td>
                <td width="100px"><b>Associated Score</b> </td>
                <td width="250px"><b>Jump to Question</b> </td>
                <td width="170px"><b>Ask for Clarification?</b> </td>
                <td width="170px"><b>Is Clarification Required?</b> </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="ChoiceText2" runat="server" Text='' ReadOnly="False" Width="250px"></asp:TextBox>

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
                    <asp:Button ID="btnAddAnswer2" runat="server" Text=" Add " OnClick="btnAddAnswer2_Click" CausesValidation="True" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <hr />
                </td>
            </tr>
            <tr>
                <td colspan="6" height="350px">

                    <asp:Panel ID="Panel1" runat="server" Width="100%" ScrollBars="Vertical" Height="350px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">


                        <table width="100%">

                            <asp:Repeater ID="rptrAnswers2" runat="server" OnItemCommand="rptrAnswers2_ItemCommand">
                                <ItemTemplate>

                                    <tr>
                                        <td width="250px">&nbsp;&nbsp;<%# Eval("ChoiceText")%> </td>
                                        <td width="100px" align="right">&nbsp;&nbsp;<%# ((int)Eval("Score")).ToWidgetDisplayInt() %> </td>
                                        <td width="250px" align="center">&nbsp;&nbsp; <%# ((int)Eval("JumpToQuestion")).ToWidgetDisplayInt() %> </td>
                                        <td width="170px">&nbsp;&nbsp; <%# ((bool)Eval("AskClarification")).ToYesNo() %> </td>
                                        <td width="170px">&nbsp;&nbsp; <%# ((bool)Eval("ClarificationRequired")).ToYesNo()%> </td>
                                        <td>
                                            <asp:Button ID="btnAddAnswer2" runat="server" Text=" Delete " CausesValidation="False" CommandArgument='<%# Eval("ChoiceOrder")%>' CommandName="delete" /></td>
                                    </tr>

                                </ItemTemplate>
                            </asp:Repeater>


                        </table>


                    </asp:Panel>

                </td>
            </tr>
            <tr>
                <td height="100%"></td>

            </tr>

            <tr>
                <td height="100%" valign="bottom">
                    <asp:ImageButton ID="btncancel22" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                <asp:ImageButton ID="btnPrevious22" runat="server"
                    CausesValidation="false"
                    ImageUrl="~/ControlRoom/Images/Previous_sm.png"
                    Height="25"
                    Text="Previous" ToolTip="Previous"
                    AlternateText="Previous" OnClick="btnPrevious22_Click" />

                    &nbsp;&nbsp;

                <asp:ImageButton ID="btnSave22" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnSave22_Click" />
                </td>
            </tr>

        </table>
    </asp:Panel>

    <asp:Panel ID="pnlType4Answers" runat="server"
        BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Matrix of Questions Answers/Choices" ScrollBars="Auto" CssClass="OrangePanel"
        Width="100%" Height="585px" Visible="false">
        <table width="100%" height="550px">
            <tr>
                <td><b>Multiple Choice Answer: </b></td>
            </tr>
            <tr>
                <td width="250px"><b>Value</b> </td>
                <td width="100px"><b>Associated Score</b> </td>
                <td width="250px"><b>Jump to Question</b> </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="ChoiceText4" runat="server" Text='' ReadOnly="False" Width="250px"></asp:TextBox>

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
                    <asp:Button ID="Button1" runat="server" Text=" Add " OnClick="btnAddAnswer4_Click" CausesValidation="True" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <hr />
                </td>
            </tr>
            <tr>
                <td colspan="6" height="150px">

                    <asp:Panel ID="Panel2" runat="server" Width="100%" ScrollBars="Vertical" Height="150px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">


                        <table width="100%">

                            <asp:Repeater ID="rptrAnswers4" runat="server" OnItemCommand="rptrAnswers4_ItemCommand">
                                <ItemTemplate>

                                    <tr>
                                        <td width="250px">&nbsp;&nbsp;<%# Eval("ChoiceText")%> </td>
                                        <td width="100px" align="right">&nbsp;&nbsp;<%# ((int)Eval("Score")).ToWidgetDisplayInt() %> </td>
                                        <td width="250px" align="center">&nbsp;&nbsp; <%# ((int)Eval("JumpToQuestion")).ToWidgetDisplayInt() %> </td>
                                        <td>
                                            <asp:Button ID="btnAddAnswer2" runat="server" Text=" Delete " CausesValidation="False" CommandArgument='<%# Eval("ChoiceOrder")%>' CommandName="delete" /></td>
                                    </tr>

                                </ItemTemplate>
                            </asp:Repeater>


                        </table>


                    </asp:Panel>

                </td>
            </tr>

            <tr>
                <td><b>Matrix Lines: </b></td>
            </tr>
            <tr>
                <td colspan="6"><b>Line Text</b> </td>
                <td></td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:TextBox ID="LineText4" runat="server" Text='' ReadOnly="False" Width="600px"></asp:TextBox>

                </td>

                <td>
                    <asp:Button ID="Button2" runat="server" Text=" Add " OnClick="btnAddLines4_Click" CausesValidation="True" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <hr />
                </td>
            </tr>
            <tr>
                <td colspan="6" height="150px">

                    <asp:Panel ID="Panel3" runat="server" Width="100%" ScrollBars="Vertical" Height="150px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">


                        <table width="100%">

                            <asp:Repeater ID="rptrLines4" runat="server" OnItemCommand="rptrLines4_ItemCommand">
                                <ItemTemplate>

                                    <tr>
                                        <td width="600px">&nbsp;&nbsp;<%# Eval("LineText")%> </td>
                                        <td>
                                            <asp:Button ID="btnDel4" runat="server" Text=" Delete " CausesValidation="False" CommandArgument='<%# Eval("LineOrder")%>' CommandName="delete" /></td>
                                    </tr>

                                </ItemTemplate>
                            </asp:Repeater>


                        </table>


                    </asp:Panel>

                </td>
            </tr>



            <tr>
                <td height="100%"></td>

            </tr>

            <tr>
                <td height="100%" valign="bottom">
                    <asp:ImageButton ID="btnCancel44" runat="server"
                        CausesValidation="false"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" OnClick="btnCancel0_Click" />
                    &nbsp;&nbsp;

                <asp:ImageButton ID="btnPrevious44" runat="server"
                    CausesValidation="false"
                    ImageUrl="~/ControlRoom/Images/Previous_sm.png"
                    Height="25"
                    Text="Previous" ToolTip="Previous"
                    AlternateText="Previous" OnClick="btnPrevious44_Click" />

                    &nbsp;&nbsp;

                <asp:ImageButton ID="btnSave44" runat="server"
                    CausesValidation="True"
                    ImageUrl="~/ControlRoom/Images/Next_sm.png"
                    Height="25"
                    Text="Continue" ToolTip="Continue"
                    AlternateText="Continue" OnClick="btnSave44_Click" />
                </td>
            </tr>

        </table>
    </asp:Panel>
</asp:Content>
