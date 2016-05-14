<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="AwardAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.AwardAddEdit" %>


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
            <asp:BoundField DataField="AID" HeaderText="AID: " SortExpression="AID" ReadOnly="True" InsertVisible="False" Visible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>

            <asp:TemplateField>
                <EditItemTemplate>
                    <table width="100%">
                        <tr>
                            <td nowrap height="20px"><b>Award Name: </b></td>
                            <td colspan="3">
                                <asp:TextBox ID="AwardName" runat="server" Text='<%# Eval("AwardName") %>' Width="500px" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAwardName" runat="server"
                                    ControlToValidate="AwardName" Display="Dynamic" ErrorMessage="<font color='red'>AwardName is required"
                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                            <td nowrap><b></b></td>
                            <td colspan="3"></td>
                        </tr>
                        <tr>
                            <td nowrap height="20px"><b>Badge Awarded: </b></td>
                            <td colspan="3">
                                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" Width="500px" CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="BadgeIDLbl" runat="server" Text='<%# Eval("BadgeID") %>' Visible="False"></asp:Label>
                            <td nowrap><b></b></td>
                            <td colspan="3"></td>
                        </tr>
                        <tr>
                            <td colspan="8" height="200px">
                                <asp:Panel ID="Panel1" runat="server" GroupingText=" Award Triggers " ScrollBars="Auto" Visible="True" CssClass="BluePanel">
                                    <table width="100%" height="200px">
                                        <tr>
                                            <td colspan="4">
                                                <p class="lead" style="margin-top: 0;">Choose what a patron must achieve in order to receive this award:</p>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap valign="top"><b>Must have earned this many points: </b></td>
                                            <td colspan="3" valign="top">
                                                <asp:TextBox ID="NumPoints" runat="server" Text='<%# ((int) Eval("NumPoints") == 0 ? "0" : Eval("NumPoints")) %>' Width="50px" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvNumPoints" runat="server"
                                                    ControlToValidate="NumPoints" Display="Dynamic" ErrorMessage="<font color='red'>Num Points is required"
                                                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revNumPoints"
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
                                            </td>

                                            <td colspan="4" rowspan="6">
                                                <div class="form-inline">
                                                    <b style="font-size: larger;">Must have earned
                                                        <asp:TextBox ID="BadgesAchieved" Width="3em" runat="server" CssClass="form-control BadgesAchievedField" Text='<%# ((int) Eval("BadgesAchieved") == 0 ? string.Empty : Eval("BadgesAchieved")) %>'></asp:TextBox>
                                                        of the <a id="CheckedBadgesCount" href="#"></a>
                                                        checked badges :</b>
                                                </div>



                                                <div style="height: 200px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd;">
                                                    <asp:TextBox ID="BadgeList" runat="server" Text='<%# Eval("BadgeList") %>' ReadOnly="False" Visible="false"></asp:TextBox>
                                                    <asp:ObjectDataSource ID="odsBadgeMembership" runat="server"
                                                        SelectMethod="GetBadgeListMembership" TypeName="GRA.SRP.DAL.Award">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="BadgeList" DefaultValue="" Name="list"
                                                                PropertyName="Text" Type="string" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                    <asp:GridView ID="gvBadgeMembership" ShowHeader="false" runat="server" DataSourceID="odsBadgeMembership" AutoGenerateColumns="false" Width="100%">
                                                        <Columns>
                                                            <asp:TemplateField ShowHeader="false">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("isMember")).ToString()=="1"?true:false) %>' runat="server" CssClass="BranchSelectionOption" />
                                                                    <%# Eval("AdminName") %>
                                                                    <asp:Label ID="BID" runat="server" Text='<%# Eval("BID") %>' Visible="False"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>


                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap valign="top"><b>Must have reached % Goal: </b></td>
                                            <td colspan="3" valign="top">
                                                <asp:TextBox ID="GoalPercent" runat="server" Text='<%# ((int) Eval("GoalPercent") == 0 ? "0" : Eval("GoalPercent")) %>' Width="50px" CssClass="form-control"></asp:TextBox>
                                                <asp:RegularExpressionValidator
                                                    ControlToValidate="GoalPercent"
                                                    ValidationExpression="\d+"
                                                    Display="Dynamic"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'> * Goal percent must be 0-100.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True"
                                                    Text="<font color='red'> * Goal percent must be 0-100.</font>"
                                                    EnableTheming="True"
                                                    SetFocusOnError="True" />
                                                <asp:RangeValidator
                                                    ControlToValidate="GoalPercent"
                                                    MinimumValue="0"
                                                    MaximumValue="1000"
                                                    Display="Dynamic"
                                                    Type="Integer"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'> * GoalPercent must be 0-1000.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True"
                                                    Text="<font color='red'> * GoalPercent must be 0-1000.</font>"
                                                    EnableTheming="True"
                                                    SetFocusOnError="True" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap valign="top"><b>Must be associated with branch/library: </b></td>
                                            <td colspan="3" valign="top">
                                                <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID"
                                                    AppendDataBoundItems="True" Width="97%" CssClass="form-control">
                                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="BranchIDLbl" runat="server" Text='<%# Eval("BranchID") %>' Visible="false"></asp:Label>
                                            </td>

                                        </tr>

                                        <tr>
                                            <td nowrap valign="top"><b>Must be enrolled in program: </b></td>
                                            <td colspan="3" valign="top">
                                                <asp:DropDownList ID="ProgramID" runat="server" DataSourceID="odsDDPrograms"
                                                    DataTextField="AdminName" DataValueField="PID"
                                                    AppendDataBoundItems="True" Width="97%" CssClass="form-control">
                                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="ProgramIDLbl" runat="server" Text='<%# Eval("ProgramID") %>' Visible="false"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap valign="top"><b>Must be associated with district: </b></td>
                                            <td colspan="3" valign="top">
                                                <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDLibSys"
                                                    DataTextField="Code" DataValueField="CID"
                                                    AppendDataBoundItems="True" Width="97%" CssClass="form-control">
                                                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="DistrictLbl" runat="server" Text='<%# Eval("District") %>' Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap valign="top"><b>Must be associated with school: </b></td>
                                            <td colspan="3" valign="top">
                                                <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool"
                                                    DataTextField="Code" DataValueField="CID"
                                                    AppendDataBoundItems="True" Width="97%" CssClass="form-control">
                                                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="SchoolNameLbl" runat="server" Text='<%# Eval("SchoolName") %>' Visible="false"></asp:Label>
                                            </td>

                                            <td nowrap valign="top"><b></b></td>
                                            <td colspan="1" valign="top"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" height="100%"></td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>

                        </tr>
                    </table>
                </EditItemTemplate>
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
        TypeName="GRA.SRP.DAL.Award">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="AID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBadges" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Badge"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDPrograms" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDLibSys" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        function updateCount() {
            $('#CheckedBadgesCount').text($('.BranchSelectionOption input:checkbox:checked').length);
        }

        $(function () {
            updateCount();
        });

        $('.BranchSelectionOption').change(function () {
            updateCount();
        });

        $('#CheckedBadgesCount').on('click', function () {
            $('.BadgesAchievedField').val($('#CheckedBadgesCount').text());
        });
    </script>
</asp:Content>
