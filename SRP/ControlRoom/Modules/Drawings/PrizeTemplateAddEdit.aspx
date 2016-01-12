<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master"
    AutoEventWireup="true" CodeBehind="PrizeTemplateAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Drawings.PrizeTemplateAddEdit" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>


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

            <asp:BoundField DataField="TID" HeaderText="TID: " SortExpression="TID" ReadOnly="True" InsertVisible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>


            <asp:TemplateField HeaderText="TName: " SortExpression="TName" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:TextBox ID="TName" runat="server" Text='<%# Eval("TName") %>' ReadOnly="False" Width="600px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTName" runat="server"
                        ControlToValidate="TName" Display="Dynamic" ErrorMessage="TName is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="TName" runat="server" Text='' Width="600px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTName" runat="server"
                        ControlToValidate="TName" Display="Dynamic" ErrorMessage="TName is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="TName" runat="server" Text='<%# Eval("TName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>


            <asp:TemplateField HeaderText="Num Prizes: " SortExpression="NumPrizes" HeaderStyle-Wrap="False" InsertVisible="false">
                <EditItemTemplate>
                    <asp:TextBox ID="NumPrizes" runat="server" Text='<%# ((int) Eval("NumPrizes") ==0 ? "" : Eval("NumPrizes")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNumPrizes" runat="server"
                        ControlToValidate="NumPrizes" Display="Dynamic" ErrorMessage="NumPrizes is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revNumPrizes"
                        ControlToValidate="NumPrizes"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Num Prizes must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Num Prizes must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvNumPrizes"
                        ControlToValidate="NumPrizes"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Num Prizes must be from 0 to 9999!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Num Prizes must be from 0 to 9999! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </EditItemTemplate>
                <InsertItemTemplate>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="NumPrizes" runat="server" Text='<%# Eval("NumPrizes") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Include Previous Winners? " SortExpression="IncPrevWinnersFlag" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:CheckBox ID="IncPrevWinnersFlag" runat="server" Checked='<%# (bool)Eval("IncPrevWinnersFlag") %>' ReadOnly="False"></asp:CheckBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="IncPrevWinnersFlag" runat="server" Text='<%# Eval("IncPrevWinnersFlag") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Send Notification? " SortExpression="SendNotificationFlag" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:CheckBox ID="SendNotificationFlag" runat="server" Checked='<%# (bool)Eval("SendNotificationFlag") %>' ReadOnly="False"></asp:CheckBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="SendNotificationFlag" runat="server" Text='<%# Eval("SendNotificationFlag") %>'></asp:Label>
                </ItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="NotificationSubject: " SortExpression="NotificationSubject" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="NotificationSubject" runat="server" Text='<%# Eval("NotificationSubject") %>' ReadOnly="False" Width="600px"></asp:TextBox>

                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="NotificationSubject" runat="server" Text='<%# Eval("NotificationSubject") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Notification Message: " SortExpression="NotificationMessage" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <textarea id="NotificationMessage" runat="server" class="gra-editor"><%# Eval("NotificationMessage") %></textarea>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="NotificationMessage" runat="server" Text='<%# Eval("NotificationMessage") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Program: " SortExpression="ProgID" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID"
                        AppendDataBoundItems="True">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="ProgIDLbl" runat="server" Text='<%# Eval("ProgID") %>' Visible="False"></asp:Label>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="ProgID" runat="server" Text='<%# Eval("ProgID") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Gender: " SortExpression="Gender" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:DropDownList ID="Gender" runat="server">
                        <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                        <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                        <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                        <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                    </asp:DropDownList>

                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Gender" runat="server" Text='<%# Eval("Gender") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="School Name: " SortExpression="SchoolName" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool"
                        DataTextField="Code" DataValueField="CID"
                        AppendDataBoundItems="True">
                        <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
                    </asp:DropDownList>

                    <asp:Label ID="SchoolNameLbl" runat="server" Text='<%# Eval("SchoolName") %>' ReadOnly="False" Width="400px"></asp:Label>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="SchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="PrimaryLibrary: " SortExpression="PrimaryLibrary" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID"
                        AppendDataBoundItems="True">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="PrimaryLibraryLbl" runat="server" Text='<%# Eval("PrimaryLibrary") %>' Visible="False"></asp:Label>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Min Points: " SortExpression="MinPoints" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="MinPoints" runat="server" Text='<%# ((int) Eval("MinPoints") ==0 ? "" : Eval("MinPoints")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revMinPoints"
                        ControlToValidate="MinPoints"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Points must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Min Points must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvMinPoints"
                        ControlToValidate="MinPoints"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Points must be from 0 to 99!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Min Points must be from 0 to 9999! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="MinPoints" runat="server" Text='<%# Eval("MinPoints") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="MaxPoints: " SortExpression="MaxPoints" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="MaxPoints" runat="server" Text='<%# ((int) Eval("MaxPoints") ==0 ? "" : Eval("MaxPoints")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revMaxPoints"
                        ControlToValidate="MaxPoints"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Points must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Max Points must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvMaxPoints"
                        ControlToValidate="MaxPoints"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Points must be from 0 to 99!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Max Points must be from 0 to 9999! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="MaxPoints" runat="server" Text='<%# Eval("MaxPoints") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Log Date Start: " SortExpression="LogDateStart" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="LogDateStart" runat="server" Text='<%# (Eval("LogDateStart").ToString().StartsWith("1/1/0001") ? "" : DateTime.Parse(Eval("LogDateStart").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="ceLogDateStart" runat="server" TargetControlID="LogDateStart">
                    </ajaxToolkit:CalendarExtender>
                    <ajaxToolkit:MaskedEditExtender ID="meLogDateStart" runat="server"
                        UserDateFormat="MonthDayYear" TargetControlID="LogDateStart" MaskType="Date" Mask="99/99/9999">
                    </ajaxToolkit:MaskedEditExtender>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LogDateStart" runat="server" Text='<%# Eval("LogDateStart") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Log Date End: " SortExpression="LogDateEnd" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="LogDateEnd" runat="server" Text='<%# (Eval("LogDateStart").ToString().StartsWith("1/1/0001") ? "" : DateTime.Parse(Eval("LogDateEnd").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="ceLogDateEnd" runat="server" TargetControlID="LogDateEnd">
                    </ajaxToolkit:CalendarExtender>
                    <ajaxToolkit:MaskedEditExtender ID="meLogDateEnd" runat="server"
                        UserDateFormat="MonthDayYear" TargetControlID="LogDateEnd" MaskType="Date" Mask="99/99/9999">
                    </ajaxToolkit:MaskedEditExtender>

                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LogDateEnd" runat="server" Text='<%# Eval("LogDateEnd") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Min Reviews: " SortExpression="MinReviews" HeaderStyle-Wrap="False" InsertVisible="False">

                <EditItemTemplate>
                    <asp:TextBox ID="MinReviews" runat="server" Text='<%# ((int) Eval("MinReviews") ==0 ? "" : Eval("MinReviews")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revMinReviews"
                        ControlToValidate="MinReviews"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Reviews must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Min Reviews must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvMinReviews"
                        ControlToValidate="MinReviews"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Min Reviews must be from 0 to 99!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Min Reviews must be from 0 to 99! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="MinReviews" runat="server" Text='<%# Eval("MinReviews") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Max Reviews: " SortExpression="MaxReviews" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="MaxReviews" runat="server" Text='<%# ((int) Eval("MaxReviews") ==0 ? "" : Eval("MaxReviews")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="revMaxReviews"
                        ControlToValidate="MaxReviews"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Reviews must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Max Reviews must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvMaxReviews"
                        ControlToValidate="MaxReviews"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'>Max Reviews must be from 0 to 99!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * Max eviews must be from 0 to 99! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="MaxReviews" runat="server" Text='<%# Eval("MaxReviews") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Review Date Start: " SortExpression="ReviewDateStart" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="ReviewDateStart" runat="server" Text='<%# (Eval("ReviewDateStart").ToString().StartsWith("1/1/0001") ? "" : DateTime.Parse(Eval("ReviewDateStart").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px">></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="ceReviewDateStart" runat="server" TargetControlID="ReviewDateStart">
                    </ajaxToolkit:CalendarExtender>
                    <ajaxToolkit:MaskedEditExtender ID="meReviewDateStart" runat="server"
                        UserDateFormat="MonthDayYear" TargetControlID="ReviewDateStart" MaskType="Date" Mask="99/99/9999">
                    </ajaxToolkit:MaskedEditExtender>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="ReviewDateStart" runat="server" Text='<%# Eval("ReviewDateStart") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Review Date End: " SortExpression="ReviewDateEnd" HeaderStyle-Wrap="False" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="ReviewDateEnd" runat="server" Text='<%# (Eval("ReviewDateEnd").ToString().StartsWith("1/1/0001") ? "" : DateTime.Parse(Eval("ReviewDateEnd").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px">></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="ceReviewDateEnd" runat="server" TargetControlID="ReviewDateEnd">
                    </ajaxToolkit:CalendarExtender>
                    <ajaxToolkit:MaskedEditExtender ID="meReviewDateEnd" runat="server"
                        UserDateFormat="MonthDayYear" TargetControlID="ReviewDateEnd" MaskType="Date" Mask="99/99/9999">
                    </ajaxToolkit:MaskedEditExtender>
                </EditItemTemplate>
                <InsertItemTemplate></InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="ReviewDateEnd" runat="server" Text='<%# Eval("ReviewDateEnd") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>


            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Date Created: " HeaderStyle-Wrap="False" Visible="true"
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
        TypeName="GRA.SRP.DAL.PrizeTemplate">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="TID"
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

    <asp:ObjectDataSource ID="odsProg" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

