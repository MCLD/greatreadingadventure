<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="ProgramGameLevelAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.ProgramGameLevelAddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #element_to_pop_up1 {
            display: none;
            background-color: #fff;
            border-radius: 10px 10px 10px 10px;
            box-shadow: 0 0 25px 5px #999;
            color: #111;
            display: none;
            min-width: 50px;
            padding: 25px;
        }

        #element_to_pop_up2 {
            display: none;
            background-color: #fff;
            border-radius: 10px 10px 10px 10px;
            box-shadow: 0 0 25px 5px #999;
            color: #111;
            display: none;
            min-width: 50px;
            padding: 25px;
        }
    </style>
    <script type="text/javascript" src="/Scripts/jquery.bpopup.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />

    <h1>
        <asp:Label ID="lblGameName" runat="server" Text="" ForeColor="#0033CC"></asp:Label></h1>
    <asp:Label ID="PGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        OnItemCommand="DvItemCommand" OnDataBinding="dv_DataBinding"
        OnDataBound="dv_DataBound"
        Width="100%">
        <Fields>

            <asp:BoundField DataField="PGLID" HeaderText="Level ID: " SortExpression="PGLID" ReadOnly="True" InsertVisible="False" Visible="false">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>

            <asp:TemplateField>
                <EditItemTemplate>
                    <table width="100%">

                        <tr>
                            <td colspan="8">
                                <div style="font-size: x-large;"><b>Level #: <%# Eval("LevelNumber") %><hr /></hr></div>
                            </td>

                        </tr>
                        <tr>
                            <td valign="top"><b># Points To Complete: </b></td>
                            <td valign="top" colspan="3">
                                <asp:TextBox ID="PointNumber" runat="server" Text='<%# ((int) Eval("PointNumber") ==0 ? "" : Eval("PointNumber")) %>'
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPointNumber" runat="server"
                                    ControlToValidate="PointNumber" Display="Dynamic" ErrorMessage="# Points is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revPointNumber"
                                    ControlToValidate="PointNumber"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'># Points must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * # Points must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvPointNumber"
                                    ControlToValidate="PointNumber"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'># Points must be from 0 to 9999!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * # Points must be from 0 to 9999! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />

                            </td>

                            <td valign="top"><b></b></td>
                            <td valign="top" colspan="3"><b></b></td>

                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <div style="font-size: large; color: Maroon;"><b>Normal Play Mode</b></div>
                            </td>
                            <td colspan="2">
                                <asp:HyperLink ID="PreviewImage1" runat="server" rel='lightbox[Image 90% 90%]' CssClass="pop1"
                                    ImageUrl="~/images/preview-file-md.png"
                                    NavigateUrl='<%# string.Format("~/Images/Games/Board/{0}.png",Eval("PGID")) %>'>HyperLink</asp:HyperLink>

                                <div id='element_to_pop_up1'>
                                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# string.Format("~/Images/Games/Board/{0}.png",Eval("PGID")) %>' Width="500px" />
                                </div>
                            </td>
                            <td colspan="2">
                                <div style="font-size: large; color: Maroon;"><b>Bonus Play Mode</b></div>
                            </td>
                            <td colspan="2">
                                <asp:HyperLink ID="HyperLink1" runat="server" rel='lightbox[Image 90% 90%]' CssClass="pop2"
                                    ImageUrl="~/images/preview-file-md.png"
                                    NavigateUrl='<%# string.Format("~/Images/Games/Board/bonus_{0}.png",Eval("PGID")) %>'>HyperLink</asp:HyperLink>

                                <div id='element_to_pop_up2'>
                                    <asp:Image ID="Image2" runat="server" ImageUrl='<%# string.Format("~/Images/Games/Board/bonus_{0}.png",Eval("PGID")) %>' Width="500px" />
                                </div>

                                <script>

                                    (function ($) {

                                        // DOM Ready
                                        $(function () {

                                            $('.pop1').bind('click', function (e) {
                                                e.preventDefault();
                                                $('#element_to_pop_up1').bPopup();
                                            });

                                            $('.pop2').bind('click', function (e) {
                                                e.preventDefault();
                                                $('#element_to_pop_up2').bPopup();
                                            });

                                        });

                                    })(jQuery);
                                </script>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>

                        <tr>
                            <td valign="top"><b>X Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationX" runat="server" Text='<%# ((int) Eval("LocationX") ==0 ? "" : Eval("LocationX")) %>'
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLocationX" runat="server"
                                    ControlToValidate="LocationX" Display="Dynamic" ErrorMessage="X Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLocationX"
                                    ControlToValidate="LocationX"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLocationX"
                                    ControlToValidate="LocationX"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>
                            <td valign="top"><b>Y Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationY" runat="server" Text='<%# ((int) Eval("LocationY") ==0 ? "" : Eval("LocationY")) %>'
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLocationY" runat="server"
                                    ControlToValidate="LocationY" Display="Dynamic" ErrorMessage="Y Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLocationY"
                                    ControlToValidate="LocationY"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLocationY"
                                    ControlToValidate="LocationY"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>

                            <td valign="top" valign="top"><b>X Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationXBonus" runat="server" Text='<%# ((int) Eval("LocationXBonus") ==0 ? "" : Eval("LocationXBonus")) %>'
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="LocationXBonus" Display="Dynamic" ErrorMessage="X Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                    ControlToValidate="LocationX"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="RangeValidator1"
                                    ControlToValidate="LocationXBonus"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>
                            <td valign="top" valign="top"><b>Y Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationYBonus" runat="server" Text='<%# ((int) Eval("LocationYBonus") ==0 ? "" : Eval("LocationYBonus")) %>'
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLocationYBonus" runat="server"
                                    ControlToValidate="LocationYBonus" Display="Dynamic" ErrorMessage="Y Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLocationYBonus"
                                    ControlToValidate="LocationYBonus"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLocationYBonus"
                                    ControlToValidate="LocationYBonus"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>

                        </tr>



                        <tr>
                            <td><b>'Primary' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame1ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="Minigame1IDLbl" runat="server" Text='<%# Eval("Minigame1ID") %>' Visible="False"></asp:Label>
                            </td>

                            <td><b>'Primary' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame1IDBonus" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="Minigame1IDBonusLbl" runat="server" Text='<%# Eval("Minigame1IDBonus") %>' Visible="False"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td><b>'Regular' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame2ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="Minigame2IDLbl" runat="server" Text='<%# Eval("Minigame2ID") %>' Visible="False"></asp:Label>
                            </td>
                            <td><b>'Regular' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame2IDBonus" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="Minigame2IDBonusLbl" runat="server" Text='<%# Eval("Minigame2IDBonus") %>' Visible="False"></asp:Label>
                            </td>

                        </tr>
                        <tr>
                            <td><b>Level Completion Badge: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="AwardBadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="AwardBadgeIDLbl" runat="server" Text='<%# Eval("AwardBadgeID") %>' Visible="False"></asp:Label>
                            </td>
                            <td><b>Level Completion Badge: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="AwardBadgeIDBonus" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="AwardBadgeIDBonusLbl" runat="server" Text='<%# Eval("AwardBadgeIDBonus") %>' Visible="False"></asp:Label>
                            </td>

                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>

                    </table>


                </EditItemTemplate>
                <InsertItemTemplate>
                    <table width="100%">

                        <tr>
                            <td colspan="8">
                                <div style="font-size: x-large;"><b>New Level!<hr /></hr></div>
                            </td>

                        </tr>
                        <tr>
                            <td valign="top"><b># Points To Complete: </b></td>
                            <td valign="top" colspan="3">
                                <asp:TextBox ID="PointNumber" runat="server" Text=''
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPointNumber" runat="server"
                                    ControlToValidate="PointNumber" Display="Dynamic" ErrorMessage="# Points is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revPointNumber"
                                    ControlToValidate="PointNumber"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'># Points must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * # Points must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvPointNumber"
                                    ControlToValidate="PointNumber"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'># Points must be from 0 to 9999!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * # Points must be from 0 to 9999! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />

                            </td>

                            <td valign="top"><b></b></td>
                            <td valign="top" colspan="3"><b></b></td>

                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <div style="font-size: large; color: Maroon;"><b>Normal Play Mode</b></div>
                            </td>
                            <td colspan="2">
                                <asp:HyperLink ID="PreviewImage1" runat="server" rel='lightbox[Image 90% 90%]' CssClass="pop1"
                                    ImageUrl="~/images/preview-file-md.png"
                                    NavigateUrl='<%# PGID.Text%>'>HyperLink</asp:HyperLink>

                                <div id='element_to_pop_up1'>
                                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# string.Format("~/Images/Games/Board/{0}.png",PGID.Text) %>' Width="500px" />
                                </div>
                            </td>
                            <td colspan="2">
                                <div style="font-size: large; color: Maroon;"><b>Bonus Play Mode</b></div>
                            </td>
                            <td colspan="2">
                                <asp:HyperLink ID="HyperLink1" runat="server" rel='lightbox[Image 90% 90%]' CssClass="pop2"
                                    ImageUrl="~/images/preview-file-md.png"
                                    NavigateUrl='<%# string.Format("~/Images/Games/Board/bonus_{0}.png",PGID.Text) %>'>HyperLink</asp:HyperLink>

                                <div id='element_to_pop_up2'>
                                    <asp:Image ID="Image2" runat="server" ImageUrl='<%# string.Format("~/Images/Games/Board/bonus_{0}.png",PGID.Text) %>' Width="500px" />
                                </div>

                                <script>

                                    (function ($) {

                                        // DOM Ready
                                        $(function () {

                                            $('.pop1').bind('click', function (e) {
                                                e.preventDefault();
                                                $('#element_to_pop_up1').bPopup();
                                            });

                                            $('.pop2').bind('click', function (e) {
                                                e.preventDefault();
                                                $('#element_to_pop_up2').bPopup();
                                            });

                                        });

                                    })(jQuery);
                                </script>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>

                        <tr>
                            <td valign="top"><b>X Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationX" runat="server" Text=''
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLocationX" runat="server"
                                    ControlToValidate="LocationX" Display="Dynamic" ErrorMessage="X Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLocationX"
                                    ControlToValidate="LocationX"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLocationX"
                                    ControlToValidate="LocationX"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>
                            <td valign="top"><b>Y Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationY" runat="server" Text=''
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLocationY" runat="server"
                                    ControlToValidate="LocationY" Display="Dynamic" ErrorMessage="Y Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLocationY"
                                    ControlToValidate="LocationY"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLocationY"
                                    ControlToValidate="LocationY"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>

                            <td valign="top" valign="top"><b>X Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationXBonus" runat="server" Text=''
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="LocationXBonus" Display="Dynamic" ErrorMessage="X Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                    ControlToValidate="LocationX"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="RangeValidator1"
                                    ControlToValidate="LocationXBonus"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>X Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * X Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>
                            <td valign="top" valign="top"><b>Y Location: </b></td>
                            <td valign="top">
                                <asp:TextBox ID="LocationYBonus" runat="server" Text=''
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLocationYBonus" runat="server"
                                    ControlToValidate="LocationYBonus" Display="Dynamic" ErrorMessage="Y Location is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revLocationYBonus"
                                    ControlToValidate="LocationYBonus"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvLocationYBonus"
                                    ControlToValidate="LocationYBonus"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<br><font color='red'>Y Location must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<br><font color='red'> * Y Location must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>

                        </tr>



                        <tr>
                            <td><b>'Primary' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame1ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>

                            <td><b>'Primary' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame1IDBonus" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td><b>'Regular' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame2ID" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td><b>'Regular' Adventure: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="Minigame2IDBonus" runat="server" DataSourceID="odsDDMiniGame" DataTextField="AdminName" DataValueField="MGID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>

                        </tr>
                        <tr>
                            <td><b>Level Completion Badge: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="AwardBadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td><b>Level Completion Badge: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="AwardBadgeIDBonus" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="90%">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>

                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>

                    </table>
                </InsertItemTemplate>
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
                        CausesValidation="True"
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png"
                        Height="25"
                        Text="Save" ToolTip="Save"
                        AlternateText="Save" />
                    &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server"
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
        TypeName="GRA.SRP.DAL.ProgramGameLevel">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PGLID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>


    <asp:ObjectDataSource ID="odsDDBadges" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Badge"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDMiniGame" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Minigame"></asp:ObjectDataSource>

</asp:Content>

