<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master"
    AutoEventWireup="true" CodeBehind="EventAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.EventAddEdit" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../../Controls/EvtCustFldCtl.ascx" TagName="EvtCustFldCtl" TagPrefix="uc1" %>

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

            <asp:BoundField DataField="EID" HeaderText="Event ID: " SortExpression="EID" ReadOnly="True" InsertVisible="False" Visible="false">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>

            <asp:TemplateField>


                <InsertItemTemplate>
                    <table width="100%">
                        <tr>
                            <td nowrap><b>Event Title: </b></td>
                            <td colspan="7">
                                <asp:TextBox ID="EventTitle" runat="server" Width="99%"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server"
                                    ControlToValidate="EventTitle" Display="Dynamic" ErrorMessage="EventTitle is required"
                                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Start Date: </b></td>
                            <td>
                                <asp:TextBox ID="EventDate" runat="server" Width="75px"
                                    Text=''></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceEventDate" runat="server" TargetControlID="EventDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meEventDate" runat="server"
                                    UserDateFormat="MonthDayYear" TargetControlID="EventDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                                <asp:RequiredFieldValidator ID="rfvEventDate" runat="server"
                                    ControlToValidate="EventDate" Display="Dynamic" ErrorMessage="EventDate is required"
                                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                            </td>
                            <td nowrap><b>Start Time: </b></td>
                            <td>
                                <asp:TextBox ID="EventTime" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEventTime" runat="server"
                                    ControlToValidate="EventTime" Display="Dynamic" ErrorMessage="EventTime is required"
                                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                            </td>

                            <td nowrap><b>End Date: </b></td>
                            <td>
                                <asp:TextBox ID="EndDate" runat="server" Width="75px"
                                    Text=''></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="EndDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                    UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                            </td>
                            <td nowrap><b>End Time: </b></td>
                            <td>
                                <asp:TextBox ID="EndTime" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap><b>Short Description: </b></td>
                            <td colspan="7">
                                <asp:TextBox ID="ShortDescription" runat="server" Width="99%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap valign="top"><b>Event Description: </b></td>
                            <td colspan="7" style="height: 350px;">
                                <textarea id="HTML" runat="server" class="gra-editor"></textarea>
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Secret Code: </b></td>
                            <td>
                                <asp:TextBox ID="SecretCode" runat="server" Width="250"></asp:TextBox>
                            </td>
                            <td nowrap><b>Number Points: </b></td>
                            <td>
                                <asp:TextBox ID="NumberPoints" runat="server"
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revNumberPoints"
                                    ControlToValidate="NumberPoints"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color=red>Number Points must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<font color=red> * Number Points must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvNumberPoints"
                                    ControlToValidate="NumberPoints"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color=red>Number Points must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<font color=red> * Number Points must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Branch/Library: </b></td>
                            <td>
                                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" Width="254"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td nowrap><b>Badge Awarded: </b></td>
                            <td colspan="5">
                                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" Width="500"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>

                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Custom 1: </b></td>
                            <td>
                                <uc1:EvtCustFldCtl ID="Custom1" runat="server" Value='' FieldNumber="1" />
                            </td>

                            <td nowrap><b>Custom 2: </b></td>
                            <td>
                                <uc1:EvtCustFldCtl ID="Custom2" runat="server" Value='' FieldNumber="2" />
                            </td>

                            <td nowrap><b>Custom 3: </b></td>
                            <td>
                                <uc1:EvtCustFldCtl ID="Custom3" runat="server" Value='' FieldNumber="3" />
                            </td>

                        </tr>

                    </table>
                </InsertItemTemplate>

                <EditItemTemplate>
                    <table width="100%">
                        <tr>
                            <td nowrap><b>Event Title: </b></td>
                            <td colspan="7">
                                <asp:TextBox ID="EventTitle" runat="server" Width="99%" Text='<%# Eval("EventTitle") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server"
                                    ControlToValidate="EventTitle" Display="Dynamic" ErrorMessage="EventTitle is required"
                                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Start Date: </b></td>
                            <td>
                                <asp:TextBox ID="EventDate" runat="server" Width="75px"
                                    Text='<%# (Eval("EventDate").ToString()=="" ? "" : DateTime.Parse(Eval("EventDate").ToString()).ToShortDateString().Replace("1/1/0001","") ) %>'></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceEventDate" runat="server" TargetControlID="EventDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meEventDate" runat="server"
                                    UserDateFormat="MonthDayYear" TargetControlID="EventDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                                <asp:RequiredFieldValidator ID="rfvEventDate" runat="server"
                                    ControlToValidate="EventDate" Display="Dynamic" ErrorMessage="EventDate is required"
                                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                            </td>
                            <td nowrap><b>Start Time: </b></td>
                            <td>
                                <asp:TextBox ID="EventTime" runat="server" Text='<%# Eval("EventTime") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEventTime" runat="server"
                                    ControlToValidate="EventTime" Display="Dynamic" ErrorMessage="EventTime is required"
                                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                            </td>

                            <td nowrap><b>End Date: </b></td>
                            <td>
                                <asp:TextBox ID="EndDate" runat="server" Width="75px"
                                    Text='<%# (Eval("EndDate").ToString()== "" ? "" : DateTime.Parse(Eval("EndDate").ToString()).ToShortDateString().Replace("1/1/0001","") ) %>'></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="EndDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                    UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                            </td>
                            <td nowrap><b>End Time: </b></td>
                            <td>
                                <asp:TextBox ID="EndTime" runat="server" Text='<%# Eval("EndTime") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap><b>Short Description: </b></td>
                            <td colspan="7">
                                <asp:TextBox ID="ShortDescription" runat="server" Width="99%" Text='<%# Eval("ShortDescription") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap valign="top"><b>Event Description: </b></td>
                            <td colspan="7" style="height: 350px;">
                                <textarea id="HTML" runat="server" class="gra-editor"><%# Eval("HTML") %></textarea>
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Secret Code: </b></td>
                            <td>
                                <asp:TextBox ID="SecretCode" runat="server" Text='<%# Eval("SecretCode") %>' Width="250"></asp:TextBox>
                                <%# Eval("SecretCode").ToString() != "" ? CheckDups(Eval("SecretCode").ToString(), (int)Eval("EID")) :"" %>
                            </td>
                            <td nowrap><b>Number Points: </b></td>
                            <td>
                                <asp:TextBox ID="NumberPoints" runat="server" Text='<%# ((int) Eval("NumberPoints") ==0 ? "" : Eval("NumberPoints")) %>'
                                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revNumberPoints"
                                    ControlToValidate="NumberPoints"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color=red>Number Points must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<font color=red> * Number Points must be numeric. </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                                <asp:RangeValidator ID="rvNumberPoints"
                                    ControlToValidate="NumberPoints"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color=red>Number Points must be from 0 to 99!</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True"
                                    Text="<font color=red> * Number Points must be from 0 to 99! </font>"
                                    EnableTheming="True"
                                    SetFocusOnError="True" />
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Branch/Library: </b></td>
                            <td>
                                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" Width="254"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="BranchIdLbl" runat="server" Text='<%# Eval("BranchId") %>' Visible="false"></asp:Label>
                            </td>
                            <td nowrap><b>Badge Awarded: </b></td>
                            <td colspan="5">
                                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" Width="500"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="BadgeIDLbl" runat="server" Text='<%# Eval("BadgeID") %>' Visible="false"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td nowrap><b>Custom 1: </b></td>
                            <td>
                                <uc1:EvtCustFldCtl ID="Custom1" runat="server" FieldNumber="1" Value='<%# Eval("Custom1") %>' />
                            </td>

                            <td nowrap><b>Custom 2: </b></td>
                            <td>
                                <uc1:EvtCustFldCtl ID="Custom2" runat="server" FieldNumber="2" Value='<%# Eval("Custom2") %>' />
                            </td>

                            <td nowrap><b>Custom 3: </b></td>
                            <td>
                                <uc1:EvtCustFldCtl ID="Custom3" runat="server" FieldNumber="3" Value='<%# Eval("Custom3") %>' />
                            </td>

                        </tr>
                        <tr>
                            <td colspan="8">
                                <hr />
                            </td>
                        </tr>
                    </table>

                </EditItemTemplate>


                <ItemTemplate>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>









            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " Visible="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " Visible="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " Visible="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " Visible="False"
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
        TypeName="GRA.SRP.DAL.Event">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="EID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>



    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsBadge" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Badge"></asp:ObjectDataSource>

</asp:Content>

