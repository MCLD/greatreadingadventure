<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="ReportAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.ReportAddEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
    <%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
    <%@ Register TagPrefix="ajax" Namespace="AjaxControlToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
    .pnlCSS{
        font-weight: bold;
        cursor: pointer;
        border: solid 1px #c0c0c0;
        width: 750px;
     }

    .linearBg2 {
      /* Safari 4-5, Chrome 1-9 */
      background: -webkit-gradient(linear, 0% 0%, 0% 100%, from(#1a82f7), to(#2F2727));

      /* Safari 5.1, Chrome 10+ */
      background: -webkit-linear-gradient(top, #2F2727, #1a82f7);

      /* Firefox 3.6+ */
      background: -moz-linear-gradient(top, #2F2727, #1a82f7);

      /* IE 10 */
      background: -ms-linear-gradient(top, #2F2727, #1a82f7);

      /* Opera 11.10+ */
      background: -o-linear-gradient(top, #2F2727, #1a82f7);
    }
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:BoundField DataField="RID" HeaderText="RID: " SortExpression="RID" ReadOnly="True" Visible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>
        <asp:TemplateField>
            <InsertItemTemplate>
            <table>
                <tr>
                    <td > <b>Program: </b></td>
                    <td colspan="2">
                        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[All Programs]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="ProgIDLbl" runat="server" Text='0' Visible="False"></asp:Label>   
                    </td>
                </tr>
                
                <tr>
                    <td> <b>Report Name: </b></td>
                    <td>
                        <asp:TextBox ID="ReportName" runat="server" Text='' Width="400px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvReportName" runat="server" 
                            ControlToValidate="ReportName" Display="Dynamic" ErrorMessage="Report Name is required" 
                            SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                    </td>
                    <td>
                        <asp:Button ID="btnSave" runat="server" Text="Save Report" CssClass="btn-lg btn-green" CommandName="savereport" CommandArgument='<%# Eval("RID") %>'/>
                    </td>

                </tr>
                <tr>
                    <td> <b>Source Template: </b></td>
                    <td>
                        <asp:DropDownList ID="RTID" runat="server" DataSourceID="odsTemplate" DataTextField="ReportName" DataValueField="RTID" 
                            AppendDataBoundItems="True" Width="400px"
                            >
                            <asp:ListItem Value="0" Text="[Select a Template]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="RTIDLbl" runat="server" Text='0' Visible="False"></asp:Label>  

                    </td>
                    <td>
                        <asp:Button ID="btnLoadTemplate" runat="server" Text="Load From Template" CssClass="btn-lg btn-green" CommandName="loadtemplate"  CausesValidation=false/>
                    </td>
                </tr>

                <tr>
                    <td> <b>Save as Template Name: </b></td>
                    <td>
                        <asp:TextBox ID="TemplateName" runat="server" Text='' Width="400px"></asp:TextBox>  

                    </td>
                    <td>
                        <asp:Button ID="btnsavetemplate" runat="server" Text="Save As Template" CssClass="btn-lg btn-green" CommandName="savetemplate"  CausesValidation=false/>
                    </td>
                </tr>

                <tr>
                    <td> <b>Display Filters: </b></td>
                    <td colspan="2">
                        <asp:CheckBox ID="DisplayFilters" runat="server" ReadOnly="False"></asp:CheckBox>  
                    </td>
                </tr>
                

                <tr>
                    <td> <b>Output Format: </b></td>
                    <td colspan="2">
                        <asp:DropDownList ID="ReportFormat" runat="server" 
                            >
                            <asp:ListItem Value="0" Text="HTML / Screen"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Excel"></asp:ListItem>
                        </asp:DropDownList>
                        <!--                            <asp:ListItem Value="1" Text="CSV"></asp:ListItem> -->
                        <asp:Label ID="ReportFormatLbl" runat="server" Text='0' Visible="False"></asp:Label>
                    </td>
                </tr>
              
                <tr><td colspan ="3"><hr /></td></tr>
                <tr>
                    <td colspan ="3">
                        <asp:Button ID="Button1" runat="server" Text="Run Report" CssClass="btn-lg btn-green" CommandName="runreport" CausesValidation=false/>
                    </td>
                </tr>
                <tr><td colspan ="3"><hr /></td></tr>
            </table>
            


<asp:Panel ID="pnlClickReg" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle; background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Registration Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageReg" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsReg" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>

<asp:Panel ID="pnlColReg" runat="server" Height="0" CssClass="pnlCSS" >
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Registration Information Filter </b></td>
                </tr>



                <tr>
                    <td><b> Patron ID: </b></td>
                    <td align="center"><asp:CheckBox ID="PIDInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td></td>
                </tr>
            
                <tr>
                    <td><b> Username: </b></td>
                    <td align="center"><asp:CheckBox ID="UsernameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td></td>
                </tr>
            
                <tr>
                    <td><b> First Name: </b></td>
                    <td align="center"><asp:CheckBox ID="FirstNameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="FirstName" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>
            
                <tr>
                    <td><b> Last Name: </b></td>
                    <td align="center"><asp:CheckBox ID="LastNameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="LastName" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>            
            

                <tr>
                    <td><b> Date of Birth: </b></td>
                    <td align="center"><asp:CheckBox ID="DOBInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="DOBFrom" runat="server"  Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceDOBFrom" runat="server" TargetControlID="DOBFrom">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meDOBFrom" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="DOBFrom" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="DOBTo" runat="server"  ReadOnly="False" Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceDOBTo" runat="server" TargetControlID="DOBTo">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meDOBTo" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="DOBTo" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        </b>
                    </td>
                </tr> 
                
                <tr>
                    <td><b> Age: </b></td>
                    <td align="center"><asp:CheckBox ID="AgeInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="AgeFrom" runat="server" Text='' 
                                     ReadOnly="False" Width="75px" CssClass="align-right"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revAgeFrom"
                                    ControlToValidate="AgeFrom"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Age From must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Age From must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvAgeFrom"
                                    ControlToValidate="AgeFrom"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>AgeFrom must be from 0 to 99!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * AgeFrom must be from 0 to 99! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                    <asp:TextBox ID="AgeTo"  Width="75px"  runat="server" Text=''></asp:TextBox>
                                    <asp:RegularExpressionValidator id="revAgeTo"
                                        ControlToValidate="AgeTo"
                                        ValidationExpression="\d+"
                                        Display="Dynamic"
                                        EnableClientScript="true"
                                        ErrorMessage="<font color='red'>Age To must be numeric.</font>"
                                        runat="server"
                                        Font-Bold="True" Font-Italic="True" 
                                        Text="<font color='red'> * Age To must be numeric. </font>" 
                                        EnableTheming="True" 
                                        SetFocusOnError="True" />      
                                    <asp:RangeValidator ID="rvAgeTo"
                                        ControlToValidate="AgeTo"
                                        MinimumValue="0"
                                        MaximumValue="99"
                                        Display="Dynamic"
                                        Type="Integer"
                                        EnableClientScript="true"
                                        ErrorMessage="<font color='red'>Age To must be from 0 to 99!</font>"
                                        runat="server" 
                                        Font-Bold="True" Font-Italic="True" 
                                        Text="<font color='red'> * Age To must be from 0 to 99! </font>" 
                                        EnableTheming="True" 
                                        SetFocusOnError="True" />
                        </b>
                    </td>
                </tr>                 
                
            
                <tr>
                    <td><b> School Grade: </b></td>
                    <td align="center"><asp:CheckBox ID="SchoolGradeInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="SchoolGrade" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>  
         
                <tr>
                    <td><b> Gender: </b></td>
                    <td align="center"><asp:CheckBox ID="GenderInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="Gender" runat="server" 
                            >
                            <asp:ListItem Value="" Text="[Any Gender]"></asp:ListItem>
                            <asp:ListItem Value="M" Text="M - Male"></asp:ListItem>
                            <asp:ListItem Value="F" Text="F - Female"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>                
            
                <tr>
                    <td><b> Email Address: </b></td>
                    <td align="center"><asp:CheckBox ID="EmailAddressInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="EmailAddress" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>  
                
            
                <tr>
                    <td><b> Phone Number: </b></td>
                    <td align="center"><asp:CheckBox ID="PhoneNumberInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="PhoneNumber" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>  
            
                <tr>
                    <td><b> City: </b></td>
                    <td align="center"><asp:CheckBox ID="CityInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="City" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>                 
            
                <tr>
                    <td><b> State: </b></td>
                    <td align="center"><asp:CheckBox ID="StateInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="State" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>                                                                
            
                <tr>
                    <td><b> Zip Code: </b></td>
                    <td align="center"><asp:CheckBox ID="ZipCodeInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="ZipCode" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>                 
                
                <tr>
                    <td><b> County: </b></td>
                    <td align="center"><asp:CheckBox ID="CountyInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="County" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr> 
                
                <tr>
                    <td><b>Library District: </b></td>
                    <td align="center"><asp:CheckBox ID="DistrictInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDLibSys" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList> 
                    </td>
                </tr>                                                                            


                <tr>
                    <td><b> Library: </b></td>
                    <td align="center"><asp:CheckBox ID="PrimaryLibraryInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>                    
                    </td>
                </tr> 
                
                <tr>
                    <td><b> School Type: </b></td>
                    <td align="center"><asp:CheckBox ID="SchoolTypeInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsSchoolType" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr> 
                

                <tr>
                    <td><b>School District: </b></td>
                    <td align="center"><asp:CheckBox ID="SDistrictInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSchSys" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList> 
                    </td>
                </tr>                                                                            

                <tr>
                    <td><b> School Name: </b></td>
                    <td align="center"><asp:CheckBox ID="SchoolNameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList> 
                    </td>
                </tr> 
                
                
                <tr>
                    <td><b> Teacher: </b></td>
                    <td align="center"><asp:CheckBox ID="TeacherInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Teacher" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>  
                
                <tr>
                    <td><b> Group/Team Name: </b></td>
                    <td align="center"><asp:CheckBox ID="GroupTeamNameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="GroupTeamName" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>              
            
                
                
                <tr>
                    <td><b> LiteracyLevel1: </b></td>
                    <td align="center"><asp:CheckBox ID="LiteracyLevel1Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                            <asp:TextBox ID="LiteracyLevel1" runat="server" Text='' Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revLiteracyLevel1"
                                ControlToValidate="LiteracyLevel1"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel1 must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel1 must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvLiteracyLevel1"
                                ControlToValidate="LiteracyLevel1"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel1 must be from 0 to 99!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel1 must be from 0 to 99! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />                     
                    
                    </td>
                </tr> 
                     
                <tr>
                    <td><b> LiteracyLevel2: </b></td>
                    <td align="center"><asp:CheckBox ID="LiteracyLevel2Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                            <asp:TextBox ID="LiteracyLevel2" runat="server" Text='' Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revLiteracyLevel2"
                                ControlToValidate="LiteracyLevel2"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel2 must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel2 must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvLiteracyLevel2"
                                ControlToValidate="LiteracyLevel2"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel2 must be from 0 to 99!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel2 must be from 0 to 99! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />                      
                    
                    </td>
                </tr>
                  
                     
                <tr>
                    <td><b> Custom1: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom1Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom1" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>
                
                     
                <tr>
                    <td><b> Custom2: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom2Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom2" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>
                
                <tr>
                    <td><b> Custom3: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom3Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom3" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>
                
                <tr>
                    <td><b> Custom4: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom4Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom4" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>
                 
                <tr>
                    <td><b> Custom5: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom5Inc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom5" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>    
                
                <tr>
                    <td><b> Registration Date: </b></td>
                    <td align="center"><asp:CheckBox ID="RegistrationDateInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="RegistrationDateStart" runat="server" Text='' Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="RegistrationDateStart">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RegistrationDateStart" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="RegistrationDateEnd" runat="server" Text='' ReadOnly="False" Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="RegistrationDateEnd">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RegistrationDateEnd" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        </b>
                    </td>
                </tr> 
                
                                
                                                                                                                                           
            </table>
</asp:Panel>

<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender1"
runat="server"
CollapseControlID="pnlClickReg"
Collapsed="false"
ExpandControlID="pnlClickReg"
TextLabelID="lblMessageReg"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsReg"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColReg"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>

<br /><br />

<asp:Panel ID="pnlTestResults" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Literacy Testing Results</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageTest" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsTest" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColTestResults" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Literacy Testing Results Filter </b></td>
                </tr>

                <tr>
                    <td><b> Score 1: </b></td>
                    <td align="center"><asp:CheckBox ID="Score1Inc"  runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score1From" runat="server" Text=''  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator2"
                                ControlToValidate="Score1From"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator2"
                                ControlToValidate="Score1From"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 From must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 From must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score1To" runat="server" Text=''  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator3"
                                    ControlToValidate="Score1To"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator3"
                                    ControlToValidate="Score1To"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 To must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 To must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>      
                
                <tr>
                    <td><b> Score 2: </b></td>
                    <td align="center"><asp:CheckBox ID="Score2Inc"  runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score2From" runat="server" Text=''  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator4"
                                ControlToValidate="Score2From"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator4"
                                ControlToValidate="Score2From"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 From must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 From must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score2To" runat="server" Text=''  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator5"
                                    ControlToValidate="Score2To"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator5"
                                    ControlToValidate="Score2To"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 To must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 To must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>    


                <tr>
                    <td><b> Score 1 %: </b></td>
                    <td align="center"><asp:CheckBox ID="Score1PctInc"  runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score1PctFrom" runat="server" Text=''  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator6"
                                ControlToValidate="Score1PctFrom"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 % From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 % From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator6"
                                ControlToValidate="Score1PctFrom"
                                MinimumValue="0"
                                MaximumValue="100"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 % From must be from 0 to 100!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 % From must be from 0 to 100! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score1PctTo" runat="server" Text=''  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator7"
                                    ControlToValidate="Score1PctTo"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 % To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 % To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator7"
                                    ControlToValidate="Score1PctTo"
                                    MinimumValue="0"
                                    MaximumValue="100"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 % To must be from 0 to 100!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 % To must be from 0 to 100! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>    

                <tr>
                    <td><b> Score 2 %: </b></td>
                    <td align="center"><asp:CheckBox ID="Score2PctInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score2PctFrom" runat="server" Text=''  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator8"
                                ControlToValidate="Score2PctFrom"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 % From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 % From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator8"
                                ControlToValidate="Score2PctFrom"
                                MinimumValue="0"
                                MaximumValue="100"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 % From must be from 0 to 100!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 % From must be from 0 to 100! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score2PctTo" runat="server" Text=''  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator9"
                                    ControlToValidate="Score2PctTo"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 % To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 % To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator9"
                                    ControlToValidate="Score2PctTo"
                                    MinimumValue="0"
                                    MaximumValue="100"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 % To must be from 0 to 100!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 % To must be from 0 to 100! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>    


                </table>
</asp:Panel>
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender6"
runat="server"
CollapseControlID="pnlTestResults"
Collapsed="true"
ExpandControlID="pnlTestResults"
TextLabelID="lblMessageTest"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsLog"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColTestResults"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>

<br /><br />


<asp:Panel ID="pnlClickLog" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle; background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Logging Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageLog" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsLog" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColLog" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Logging Information Filter </b></td>
                </tr>

                <tr>
                    <td><b> Points: </b></td>
                    <td align="center"><asp:CheckBox ID="PointsInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="PointsMin" runat="server" Text='' Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revPointsMin"
                                ControlToValidate="PointsMin"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Points From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Points From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvPointsMin"
                                ControlToValidate="PointsMin"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Points From must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Points From must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="PointsMax" runat="server" Text='' Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revPointsMax"
                                    ControlToValidate="PointsMax"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Points To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Points To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvPointsMax"
                                    ControlToValidate="PointsMax"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Points To must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Points To must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>        

                <tr>
                    <td><b> Points Date: </b></td>
                    <td align="center"></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="PointsStart" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePointsStart" runat="server" TargetControlID="PointsStart">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePointsStart" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PointsStart" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                            <asp:TextBox ID="PointsEnd" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePointsEnd" runat="server" TargetControlID="PointsEnd">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePointsEnd" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PointsEnd" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                                    </b>
                    </td>
                </tr> 

                <tr>
                    <td><b> Event Code Earned: </b></td>
                    <td align="center"></td>
                    <td><asp:TextBox ID="EventCode" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>    

            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender2"
runat="server"
CollapseControlID="pnlClickLog"
Collapsed="true"
ExpandControlID="pnlClickLog"
TextLabelID="lblMessageLog"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsLog"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColLog"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>


<br /><br />

<asp:Panel ID="pnlClickBadge" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Badge & Prize Related Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageBadge" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsBadge" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColBadge" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b>Badge &  Prize Related  Information Filter </b></td>
                </tr>

                <tr>
                    <td><b> Earned Badge: </b></td>
                    <td align="center"><asp:CheckBox ID="EarnedBadgeInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="EarnedBadge" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>                          
                    </td>
                </tr>

                <tr>
                    <td><b> Physical Prize Earned: </b></td>
                    <td align="center"><asp:CheckBox ID="PhysicalPrizeNameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="PhysicalPrizeEarned" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>  

                <tr>
                    <td><b> Physical Prize Redeemed: </b></td>
                    <td align="center"></td>
                    <td><asp:CheckBox ID="PhysicalPrizeRedeemed" runat="server" ReadOnly="False"></asp:CheckBox></td>
                </tr> 

                <tr>
                    <td><b> Physical Prize Date: </b></td>
                    <td align="center"><asp:CheckBox ID="PhysicalPrizeDateInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                             <asp:TextBox ID="PhysicalPrizeStartDate" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePhysicalPrizeStartDate" runat="server" TargetControlID="PhysicalPrizeStartDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePhysicalPrizeStartDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PhysicalPrizeStartDate" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                            <asp:TextBox ID="PhysicalPrizeEndDate" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePhysicalPrizeEndDate" runat="server" TargetControlID="PhysicalPrizeEndDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePhysicalPrizeEndDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PhysicalPrizeEndDate" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                        </b>
                    </td>
                </tr> 


            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender3"
runat="server"
CollapseControlID="pnlClickBadge"
Collapsed="true"
ExpandControlID="pnlClickBadge"
TextLabelID="lblMessageBadge"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsBadge"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColBadge"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>



<br /><br />

<asp:Panel ID="pnlClickRev" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Review Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageRev" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsRev" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColRev" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Review Information Filter </b></td>
                </tr>



                <tr>
                    <td><b> # Reviews: </b></td>
                    <td align="center"><asp:CheckBox ID="NumReviewsInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="ReviewsMin" runat="server" Text='' Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revReviewsMin"
                                ControlToValidate="ReviewsMin"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Min # Reviews must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Min # Reviews must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvReviewsMin"
                                ControlToValidate="ReviewsMin"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Min # Reviews must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Min # Reviews must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" /> 
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="ReviewsMax" runat="server" Text='' Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revReviewsMax"
                                    ControlToValidate="ReviewsMax"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Max # Reviews must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Max # Reviews must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvReviewsMax"
                                    ControlToValidate="ReviewsMax"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Max # Reviews must be from 0 to 99!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Max # Reviews must be from 0 to 99! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" /> 

                         </b>
                    </td>
                </tr> 

                <tr>
                    <td><b> Title: </b></td>
                    <td align="center"><asp:CheckBox ID="ReviewTitleInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="ReviewTitle" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>    

                <tr>
                    <td><b> Author: </b></td>
                    <td align="center"><asp:CheckBox ID="ReviewAuthorInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="ReviewAuthor" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>   

                <tr>
                    <td><b> Review Date: </b></td>
                    <td align="center"><asp:CheckBox ID="ReviewDateInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="ReviewStartDate" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceReviewStartDate" runat="server" TargetControlID="ReviewStartDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meReviewStartDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="ReviewStartDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="ReviewEndDate" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceReviewEndDate" runat="server" TargetControlID="ReviewEndDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meReviewEndDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="ReviewEndDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                        </b>
                    </td>
                </tr> 


            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender4"
runat="server"
CollapseControlID="pnlClickRev"
Collapsed="true"
ExpandControlID="pnlClickRev"
TextLabelID="lblMessageRev"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsRev"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColRev"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>


<br /><br />

<asp:Panel ID="pnlClickDraw" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Random Drawing Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageDraw" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsDraw" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColDraw" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Random Drawing Information Filter </b></td>
                </tr>



                <tr>
                    <td><b> Random Drawing Name: </b></td>
                    <td align="center"><asp:CheckBox ID="RandomDrawingNameInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="RandomDrawingName" runat="server" Text='' Width="200px"></asp:TextBox></td>
                </tr>   

                <tr>
                    <td><b> Random Drawing Number: </b></td>
                    <td align="center"><asp:CheckBox ID="RandomDrawingNumInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>                            
                            <asp:TextBox ID="RandomDrawingNum" runat="server" Text=''></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator1"
                                ControlToValidate="RandomDrawingNum"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Random Drawing Num must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Random Drawing Num must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator1"
                                ControlToValidate="RandomDrawingNum"
                                MinimumValue="0"
                                MaximumValue="99999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Random Drawing Num must be from 0 to 99999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Random Drawing Num must be from 0 to 99999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                                
                    </td>
                </tr> 

                <tr>
                    <td><b> Random Drawing Date: </b></td>
                    <td align="center"><asp:CheckBox ID="RandomDrawingDateInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="RandomDrawingStartDate" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceRandomDrawingStartDate" runat="server" TargetControlID="RandomDrawingStartDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meRandomDrawingStartDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RandomDrawingStartDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="RandomDrawingEndDate" runat="server" Width="75px" ReadOnly="False"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceRandomDrawingEndDate" runat="server" TargetControlID="RandomDrawingEndDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meRandomDrawingEndDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RandomDrawingEndDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                        </b>
                    </td>
                </tr> 

                <tr>
                    <td><b> User has been drawn: </b></td>
                    <td align="center"><asp:CheckBox ID="HasBeenDrawnInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:CheckBox ID="HasBeenDrawn" runat="server" ReadOnly="False"></asp:CheckBox></td>
                </tr> 

                <tr>
                    <td><b> User has redeemed prize: </b></td>
                    <td align="center"><asp:CheckBox ID="HasRedeemendInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:CheckBox ID="HasRedeemend" runat="server" ReadOnly="False"></asp:CheckBox></td>
                </tr> 
            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender5"
runat="server"
CollapseControlID="pnlClickDraw"
Collapsed="true"
ExpandControlID="pnlClickDraw"
TextLabelID="lblMessageDraw"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsDraw"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColDraw"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>


            </InsertItemTemplate>
            





            <EditItemTemplate>
            <table>
                <tr>
                    <td > <b>Program: </b></td>
                    <td colspan="2">
                        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[All Programs]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="ProgIDLbl" runat="server" Text='<%# Eval("ProgID") %>' Visible="False"></asp:Label>   
                    </td>
                </tr>
                
                <tr>
                    <td> <b>Report Name: </b></td>
                    <td>
                        <asp:TextBox ID="ReportName" runat="server" Text='<%# Eval("ReportName") %>' Width="400px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvReportName" runat="server" 
                            ControlToValidate="ReportName" Display="Dynamic" ErrorMessage="Report Name is required" 
                            SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                    </td>
                    <td>
                        <asp:Button ID="btnSave" runat="server" Text="Save Report" CssClass="btn-lg btn-green" CommandName="savereport" CommandArgument='<%# Eval("RID") %>'/>
                    </td>

                </tr>
                <tr>
                    <td> <b>Source Template: </b></td>
                    <td>
                        <asp:DropDownList ID="RTID" runat="server" DataSourceID="odsTemplate" DataTextField="ReportName" DataValueField="RTID" 
                            AppendDataBoundItems="True" Width="400px"
                            >
                            <asp:ListItem Value="0" Text="[Select a Template]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="RTIDLbl" runat="server" Text='<%# Eval("RTID") %>' Visible="False"></asp:Label>  

                    </td>
                    <td>
                        <asp:Button ID="btnLoadTemplate" runat="server" Text="Load From Template" CssClass="btn-lg btn-green" CommandName="loadtemplate"  CausesValidation=false/>
                    </td>
                </tr>

                <tr>
                    <td> <b>Save as Template Name: </b></td>
                    <td>
                        <asp:TextBox ID="TemplateName" runat="server" Text='' Width="400px"></asp:TextBox>  

                    </td>
                    <td>
                        <asp:Button ID="btnsavetemplate" runat="server" Text="Save As Template" CssClass="btn-lg btn-green" CommandName="savetemplate"  CausesValidation=false/>
                    </td>
                </tr>

                <tr>
                    <td> <b>Display Filters: </b></td>
                    <td colspan="2">
                        <asp:CheckBox ID="DisplayFilters" runat="server" Checked='<%# (bool)Eval("DisplayFilters") %>' ReadOnly="False"></asp:CheckBox>  
                    </td>
                </tr>
                

                <tr>
                    <td> <b>Output Format: </b></td>
                    <td colspan="2">
                        <asp:DropDownList ID="ReportFormat" runat="server" 
                            >
                            <asp:ListItem Value="0" Text="HTML / Screen"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Excel"></asp:ListItem>
                        </asp:DropDownList>
                        <!--                            <asp:ListItem Value="1" Text="CSV"></asp:ListItem> -->
                        <asp:Label ID="ReportFormatLbl" runat="server" Text='<%# Eval("ReportFormat") %>' Visible="False"></asp:Label>
                    </td>
                </tr>
              
                <tr><td colspan ="3"><hr /></td></tr>
                <tr>
                    <td colspan ="3">
                        <asp:Button ID="Button1" runat="server" Text="Run Report" CssClass="btn-lg btn-green" CommandName="runreport" CausesValidation=false/>
                    </td>
                </tr>
                <tr><td colspan ="3"><hr /></td></tr>
            </table>
            


<asp:Panel ID="pnlClickReg" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Registration Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageReg" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsReg" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>

<asp:Panel ID="pnlColReg" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Registration Information Filter </b></td>
                </tr>



                <tr>
                    <td><b> Patron ID: </b></td>
                    <td align="center"><asp:CheckBox ID="PIDInc" runat="server" Checked='<%# (bool)Eval("PIDInc") %>' ReadOnly="False"></asp:CheckBox></td>
                    <td></td>
                </tr>
            
                <tr>
                    <td><b> Username: </b></td>
                    <td align="center"><asp:CheckBox ID="UsernameInc" runat="server" Checked='<%# (bool)Eval("UsernameInc") %>'  ReadOnly="False"></asp:CheckBox></td>
                    <td></td>
                </tr>
            
                <tr>
                    <td><b> First Name: </b></td>
                    <td align="center"><asp:CheckBox ID="FirstNameInc" runat="server" Checked='<%# (bool)Eval("FirstNameInc") %>'   ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="FirstName" runat="server" Text='<%# Eval("FirstName") %>' Width="200px"></asp:TextBox></td>
                </tr>
            
                <tr>
                    <td><b> Last Name: </b></td>
                    <td align="center"><asp:CheckBox ID="LastNameInc" runat="server" Checked='<%# (bool)Eval("LastNameInc") %>' ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="LastName" runat="server" Text='<%# Eval("LastName") %>' Width="200px"></asp:TextBox></td>
                </tr>            
            

                <tr>
                    <td><b> Date of Birth: </b></td>
                    <td align="center"><asp:CheckBox ID="DOBInc" Checked='<%# (bool)Eval("DOBInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="DOBFrom" runat="server" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("DOBFrom")) %>' Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceDOBFrom" runat="server" TargetControlID="DOBFrom">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meDOBFrom" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="DOBFrom" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="DOBTo" runat="server" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("DOBTo")) %>' ReadOnly="False" Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceDOBTo" runat="server" TargetControlID="DOBTo">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meDOBTo" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="DOBTo" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        </b>
                    </td>
                </tr> 
                
                <tr>
                    <td><b> Age: </b></td>
                    <td align="center"><asp:CheckBox ID="AgeInc" Checked='<%# (bool)Eval("AgeInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="AgeFrom" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("AgeFrom")) %>' 
                                     ReadOnly="False" Width="75px" CssClass="align-right"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revAgeFrom"
                                    ControlToValidate="AgeFrom"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Age From must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Age From must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvAgeFrom"
                                    ControlToValidate="AgeFrom"
                                    MinimumValue="0"
                                    MaximumValue="99"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>AgeFrom must be from 0 to 99!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * AgeFrom must be from 0 to 99! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                    <asp:TextBox ID="AgeTo"  Width="75px"  runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("AgeTo")) %>' ></asp:TextBox>
                                    <asp:RegularExpressionValidator id="revAgeTo"
                                        ControlToValidate="AgeTo"
                                        ValidationExpression="\d+"
                                        Display="Dynamic"
                                        EnableClientScript="true"
                                        ErrorMessage="<font color='red'>Age To must be numeric.</font>"
                                        runat="server"
                                        Font-Bold="True" Font-Italic="True" 
                                        Text="<font color='red'> * Age To must be numeric. </font>" 
                                        EnableTheming="True" 
                                        SetFocusOnError="True" />      
                                    <asp:RangeValidator ID="rvAgeTo"
                                        ControlToValidate="AgeTo"
                                        MinimumValue="0"
                                        MaximumValue="99"
                                        Display="Dynamic"
                                        Type="Integer"
                                        EnableClientScript="true"
                                        ErrorMessage="<font color='red'>Age To must be from 0 to 99!</font>"
                                        runat="server" 
                                        Font-Bold="True" Font-Italic="True" 
                                        Text="<font color='red'> * Age To must be from 0 to 99! </font>" 
                                        EnableTheming="True" 
                                        SetFocusOnError="True" />
                        </b>
                    </td>
                </tr>                 
                
            
                <tr>
                    <td><b> School Grade: </b></td>
                    <td align="center"><asp:CheckBox ID="SchoolGradeInc" Checked='<%# (bool)Eval("SchoolGradeInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="SchoolGrade" runat="server" Text='<%# Eval("SchoolGrade") %>' Width="200px"></asp:TextBox></td>
                </tr>  
         
                <tr>
                    <td><b> Gender: </b></td>
                    <td align="center"><asp:CheckBox ID="GenderInc" Checked='<%# (bool)Eval("GenderInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="Gender" runat="server" 
                            >
                            <asp:ListItem Value="" Text="[Any Gender]"></asp:ListItem>
                            <asp:ListItem Value="M" Text="M - Male"></asp:ListItem>
                            <asp:ListItem Value="F" Text="F - Female"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="GenderLbl" runat="server" Text='<%# Eval("Gender") %>' Visible="False"></asp:Label>
                    </td>
                </tr>                
            
                <tr>
                    <td><b> Email Address: </b></td>
                    <td align="center"><asp:CheckBox ID="EmailAddressInc" Checked='<%# (bool)Eval("EmailAddressInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="EmailAddress" runat="server" Text='<%# Eval("EmailAddress") %>' Width="200px"></asp:TextBox></td>
                </tr>  
                
            
                <tr>
                    <td><b> Phone Number: </b></td>
                    <td align="center"><asp:CheckBox ID="PhoneNumberInc" Checked='<%# (bool)Eval("PhoneNumberInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="PhoneNumber" runat="server" Text='<%# Eval("PhoneNumber") %>' Width="200px"></asp:TextBox></td>
                </tr>  
            
                <tr>
                    <td><b> City: </b></td>
                    <td align="center"><asp:CheckBox ID="CityInc" Checked='<%# (bool)Eval("CityInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' Width="200px"></asp:TextBox></td>
                </tr>                 
            
                <tr>
                    <td><b> State: </b></td>
                    <td align="center"><asp:CheckBox ID="StateInc" Checked='<%# (bool)Eval("StateInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="State" runat="server" Text='<%# Eval("State") %>' Width="200px"></asp:TextBox></td>
                </tr>                                                                
            
                <tr>
                    <td><b> Zip Code: </b></td>
                    <td align="center"><asp:CheckBox ID="ZipCodeInc" Checked='<%# (bool)Eval("ZipCodeInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="ZipCode" runat="server" Text='<%# Eval("ZipCode") %>' Width="200px"></asp:TextBox></td>
                </tr>                 
                
                <tr>
                    <td><b> County: </b></td>
                    <td align="center"><asp:CheckBox ID="CountyInc" Checked='<%# (bool)Eval("CountyInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="County" runat="server" Text='<%# Eval("County") %>' Width="200px"></asp:TextBox></td>
                </tr> 
                
                <tr>
                    <td><b> Library District: </b></td>
                    <td align="center"><asp:CheckBox ID="DistrictInc" Checked='<%# (bool)Eval("DistrictInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDLibSys" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>       
                        <asp:Label ID="DistrictLbl" runat="server" Text='<%# Eval("District") %>' Visible="False"></asp:Label>             
                    </td>
                </tr>                                                                            


                <tr>
                    <td><b> Library: </b></td>
                    <td align="center"><asp:CheckBox ID="PrimaryLibraryInc" Checked='<%# (bool)Eval("PrimaryLibraryInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>       
                        <asp:Label ID="PrimaryLibraryLbl" runat="server" Text='<%# Eval("PrimaryLibrary") %>' Visible="False"></asp:Label>             
                    </td>
                </tr> 
                
                <tr>
                    <td><b> School Type: </b></td>
                    <td align="center"><asp:CheckBox ID="SchoolTypeInc" Checked='<%# (bool)Eval("SchoolTypeInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsSchoolType" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="SchoolTypeLbl" runat="server" Text='<%# Eval("SchoolType") %>' Visible="False"></asp:Label>
                    </td>
                </tr> 

                <tr>
                    <td><b> School District: </b></td>
                    <td align="center"><asp:CheckBox ID="SDistrictInc" Checked='<%# (bool)Eval("SDistrictInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSchSys" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>       
                        <asp:Label ID="SDistrictLbl" runat="server" Text='<%# Eval("SDistrict") %>' Visible="False"></asp:Label>             
                    </td>
                </tr>  

                <tr>
                    <td><b> School Name: </b></td>
                    <td align="center"><asp:CheckBox ID="SchoolNameInc" Checked='<%# (bool)Eval("SchoolNameInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Code" DataValueField="CID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList>       
                        <asp:Label ID="SchoolNameLbl" runat="server" Text='<%# Eval("SchoolName") %>' Visible="False"></asp:Label>             
                    </td>
                </tr>  
                                
                <tr>
                    <td><b> Teacher: </b></td>
                    <td align="center"><asp:CheckBox ID="TeacherInc" Checked='<%# (bool)Eval("TeacherInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Teacher" runat="server" Text='<%# Eval("Teacher") %>' Width="200px"></asp:TextBox></td>
                </tr>  
                
                <tr>
                    <td><b> Group/Team Name: </b></td>
                    <td align="center"><asp:CheckBox ID="GroupTeamNameInc" Checked='<%# (bool)Eval("GroupTeamNameInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="GroupTeamName" runat="server" Text='<%# Eval("GroupTeamName") %>' Width="200px"></asp:TextBox></td>
                </tr>                                                         
                
                <tr>
                    <td><b> LiteracyLevel1: </b></td>
                    <td align="center"><asp:CheckBox ID="LiteracyLevel1Inc" Checked='<%# (bool)Eval("LiteracyLevel1Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                            <asp:TextBox ID="LiteracyLevel1" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("LiteracyLevel1")) %>' Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revLiteracyLevel1"
                                ControlToValidate="LiteracyLevel1"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel1 must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel1 must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvLiteracyLevel1"
                                ControlToValidate="LiteracyLevel1"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel1 must be from 0 to 99!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel1 must be from 0 to 99! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />                     
                    
                    </td>
                </tr> 
                     
                <tr>
                    <td><b> LiteracyLevel2: </b></td>
                    <td align="center"><asp:CheckBox ID="LiteracyLevel2Inc" Checked='<%# (bool)Eval("LiteracyLevel2Inc") %>' runat="server" ReadOnly="False" ></asp:CheckBox></td>
                    <td>
                            <asp:TextBox ID="LiteracyLevel2" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("LiteracyLevel2")) %>' Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revLiteracyLevel2"
                                ControlToValidate="LiteracyLevel2"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel2 must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel2 must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvLiteracyLevel2"
                                ControlToValidate="LiteracyLevel2"
                                MinimumValue="0"
                                MaximumValue="99"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>LiteracyLevel2 must be from 0 to 99!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * LiteracyLevel2 must be from 0 to 99! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />                      
                    
                    </td>
                </tr>
                  
                     
                <tr>
                    <td><b> Custom1: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom1Inc" Checked='<%# (bool)Eval("Custom1Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom1" runat="server" Text='<%# Eval("Custom1") %>' Width="200px"></asp:TextBox></td>
                </tr>
                
                     
                <tr>
                    <td><b> Custom2: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom2Inc" Checked='<%# (bool)Eval("Custom2Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom2" runat="server" Text='<%# Eval("Custom2") %>' Width="200px"></asp:TextBox></td>
                </tr>
                
                <tr>
                    <td><b> Custom3: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom3Inc" Checked='<%# (bool)Eval("Custom3Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom3" runat="server" Text='<%# Eval("Custom3") %>' Width="200px"></asp:TextBox></td>
                </tr>
                
                <tr>
                    <td><b> Custom4: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom4Inc" Checked='<%# (bool)Eval("Custom4Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom4" runat="server" Text='<%# Eval("Custom4") %>' Width="200px"></asp:TextBox></td>
                </tr>
                 
                <tr>
                    <td><b> Custom5: </b></td>
                    <td align="center"><asp:CheckBox ID="Custom5Inc" Checked='<%# (bool)Eval("Custom5Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="Custom5" runat="server" Text='<%# Eval("Custom5") %>' Width="200px"></asp:TextBox></td>
                </tr>    
                
                <tr>
                    <td><b> Registration Date: </b></td>
                    <td align="center"><asp:CheckBox ID="RegistrationDateInc" Checked='<%# (bool)Eval("RegistrationDateInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="RegistrationDateStart" runat="server" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("RegistrationDateStart")) %>' Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="RegistrationDateStart">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RegistrationDateStart" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="RegistrationDateEnd" runat="server" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("RegistrationDateEnd")) %>' ReadOnly="False" Width="75px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="RegistrationDateEnd">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RegistrationDateEnd" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>
                        </b>
                    </td>
                </tr> 
                
                                
                                                                                                                                           
            </table>
</asp:Panel>

<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender1"
runat="server"
CollapseControlID="pnlClickReg"
Collapsed="false"
ExpandControlID="pnlClickReg"
TextLabelID="lblMessageReg"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsReg"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColReg"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>

<br /><br />

<asp:Panel ID="pnlTestResults" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Literacy Testing Results</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageTest" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsTest" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColTestResults" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Literacy Testing Results Filter </b></td>
                </tr>

                <tr>
                    <td><b> Score 1: </b></td>
                    <td align="center"><asp:CheckBox ID="Score1Inc" Checked='<%# (bool)Eval("Score1Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score1From" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score1From")) %>'  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator2"
                                ControlToValidate="Score1From"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator2"
                                ControlToValidate="Score1From"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 From must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 From must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score1To" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score1To")) %>'  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator3"
                                    ControlToValidate="Score1To"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator3"
                                    ControlToValidate="Score1To"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 To must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 To must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>      
                
                <tr>
                    <td><b> Score 2: </b></td>
                    <td align="center"><asp:CheckBox ID="Score2Inc" Checked='<%# (bool)Eval("Score2Inc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score2From" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score2From")) %>'  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator4"
                                ControlToValidate="Score2From"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator4"
                                ControlToValidate="Score2From"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 From must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 From must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score2To" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score2To")) %>'  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator5"
                                    ControlToValidate="Score2To"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator5"
                                    ControlToValidate="Score2To"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 To must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 To must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>    


                <tr>
                    <td><b> Score 1 %: </b></td>
                    <td align="center"><asp:CheckBox ID="Score1PctInc" Checked='<%# (bool)Eval("Score1PctInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score1PctFrom" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score1PctFrom")) %>'  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator6"
                                ControlToValidate="Score1PctFrom"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 % From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 % From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator6"
                                ControlToValidate="Score1PctFrom"
                                MinimumValue="0"
                                MaximumValue="100"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 1 % From must be from 0 to 100!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 1 % From must be from 0 to 100! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score1PctTo" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score1PctTo")) %>'  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator7"
                                    ControlToValidate="Score1PctTo"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 % To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 % To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator7"
                                    ControlToValidate="Score1PctTo"
                                    MinimumValue="0"
                                    MaximumValue="100"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 1 % To must be from 0 to 100!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 1 % To must be from 0 to 100! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>    

                <tr>
                    <td><b> Score 2 %: </b></td>
                    <td align="center"><asp:CheckBox ID="Score2PctInc" Checked='<%# (bool)Eval("Score2PctInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="Score2PctFrom" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score2PctFrom")) %>'  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator8"
                                ControlToValidate="Score2PctFrom"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 % From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 % From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator8"
                                ControlToValidate="Score2PctFrom"
                                MinimumValue="0"
                                MaximumValue="100"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Score 2 % From must be from 0 to 100!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Score 2 % From must be from 0 to 100! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="Score2PctTo" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("Score2PctTo")) %>'  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="RegularExpressionValidator9"
                                    ControlToValidate="Score2PctTo"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 % To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 % To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="RangeValidator9"
                                    ControlToValidate="Score2PctTo"
                                    MinimumValue="0"
                                    MaximumValue="100"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Score 2 % To must be from 0 to 100!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Score 2 % To must be from 0 to 100! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>    


                </table>
</asp:Panel>
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender6"
runat="server"
CollapseControlID="pnlTestResults"
Collapsed="true"
ExpandControlID="pnlTestResults"
TextLabelID="lblMessageTest"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsLog"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColTestResults"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>

<br /><br />


<asp:Panel ID="pnlClickLog" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Logging Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageLog" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsLog" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColLog" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Logging Information Filter </b></td>
                </tr>

                <tr>
                    <td><b> Points: </b></td>
                    <td align="center"><asp:CheckBox ID="PointsInc" Checked='<%# (bool)Eval("PointsInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="PointsMin" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("PointsMin")) %>'  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revPointsMin"
                                ControlToValidate="PointsMin"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Points From must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Points From must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvPointsMin"
                                ControlToValidate="PointsMin"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Points From must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Points From must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="PointsMax" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("PointsMax")) %>'  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revPointsMax"
                                    ControlToValidate="PointsMax"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Points To must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Points To must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvPointsMax"
                                    ControlToValidate="PointsMax"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Points To must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Points To must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />
                         </b>
                    </td>
                </tr>        

                <tr>
                    <td><b> Points Date: </b></td>
                    <td align="center"></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="PointsStart" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("PointsStart")) %>'></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePointsStart" runat="server" TargetControlID="PointsStart">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePointsStart" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PointsStart" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                            <asp:TextBox ID="PointsEnd" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("PointsEnd")) %>'></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePointsEnd" runat="server" TargetControlID="PointsEnd">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePointsEnd" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PointsEnd" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                                    </b>
                    </td>
                </tr> 

                <tr>
                    <td><b> Event Code Earned: </b></td>
                    <td align="center"></asp:CheckBox></td>
                    <td><asp:TextBox ID="EventCode" runat="server" Text='<%# Eval("EventCode") %>' Width="200px"></asp:TextBox></td>
                </tr>    

            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender2"
runat="server"
CollapseControlID="pnlClickLog"
Collapsed="true"
ExpandControlID="pnlClickLog"
TextLabelID="lblMessageLog"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsTest"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColLog"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>


<br /><br />

<asp:Panel ID="pnlClickBadge" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Badge & Prize Related Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageBadge" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsBadge" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColBadge" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b>Badge &  Prize Related  Information Filter </b></td>
                </tr>

                <tr>
                    <td><b> Earned Badge: </b></td>
                    <td align="center"><asp:CheckBox ID="EarnedBadgeInc" Checked='<%# (bool)Eval("EarnedBadgeInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <asp:DropDownList ID="EarnedBadge" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                            AppendDataBoundItems="True"
                            >
                            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        </asp:DropDownList> 
                        <asp:Label ID="EarnedBadgeLbl" runat="server" Text='<%# Eval("EarnedBadge") %>' Visible="False"></asp:Label>                         
                    </td>
                </tr>

                <tr>
                    <td><b> Physical Prize Earned: </b></td>
                    <td align="center"><asp:CheckBox ID="PhysicalPrizeNameInc" Checked='<%# (bool)Eval("PhysicalPrizeNameInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="PhysicalPrizeEarned" runat="server" Text='<%# Eval("PhysicalPrizeEarned") %>' Width="200px"></asp:TextBox></td>
                </tr>  

                <tr>
                    <td><b> Physical Prize Redeemed: </b></td>
                    <td align="center"></td>
                    <td><asp:CheckBox ID="PhysicalPrizeRedeemed" runat="server" Checked='<%# (bool)Eval("PhysicalPrizeRedeemed") %>' ReadOnly="False"></asp:CheckBox></td>
                </tr> 

                <tr>
                    <td><b> Physical Prize Date: </b></td>
                    <td align="center"><asp:CheckBox ID="PhysicalPrizeDateInc" Checked='<%# (bool)Eval("PhysicalPrizeDateInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                             <asp:TextBox ID="PhysicalPrizeStartDate" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("PhysicalPrizeStartDate")) %>'></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePhysicalPrizeStartDate" runat="server" TargetControlID="PhysicalPrizeStartDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePhysicalPrizeStartDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PhysicalPrizeStartDate" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                            <asp:TextBox ID="PhysicalPrizeEndDate" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("PhysicalPrizeEndDate")) %>'></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="cePhysicalPrizeEndDate" runat="server" TargetControlID="PhysicalPrizeEndDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="mePhysicalPrizeEndDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="PhysicalPrizeEndDate" MaskType="Date" Mask="99/99/9999">
                            </ajaxToolkit:MaskedEditExtender>    
                        </b>
                    </td>
                </tr> 


            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender3"
runat="server"
CollapseControlID="pnlClickBadge"
Collapsed="true"
ExpandControlID="pnlClickBadge"
TextLabelID="lblMessageBadge"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsBadge"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColBadge"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>



<br /><br />

<asp:Panel ID="pnlClickRev" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Review Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageRev" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsRev" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColRev" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Review Information Filter </b></td>
                </tr>



                <tr>
                    <td><b> # Reviews: </b></td>
                    <td align="center"><asp:CheckBox ID="NumReviewsInc" Checked='<%# (bool)Eval("NumReviewsInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                            <asp:TextBox ID="ReviewsMin" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("ReviewsMin")) %>'  Width="75px"></asp:TextBox>
                            <asp:RegularExpressionValidator id="revReviewsMin"
                                ControlToValidate="ReviewsMin"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Min # Reviews must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Min # Reviews must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="rvReviewsMin"
                                ControlToValidate="ReviewsMin"
                                MinimumValue="0"
                                MaximumValue="9999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Min # Reviews must be from 0 to 9999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Min # Reviews must be from 0 to 9999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" /> 
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="ReviewsMax" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("ReviewsMax")) %>'  Width="75px"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revReviewsMax"
                                    ControlToValidate="ReviewsMax"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Max # Reviews must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Max # Reviews must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvReviewsMax"
                                    ControlToValidate="ReviewsMax"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Max # Reviews must be from 0 to 99!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Max # Reviews must be from 0 to 99! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" /> 

                         </b>
                    </td>
                </tr> 

                <tr>
                    <td><b> Title: </b></td>
                    <td align="center"><asp:CheckBox ID="ReviewTitleInc" Checked='<%# (bool)Eval("ReviewTitleInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="ReviewTitle" runat="server" Text='<%# Eval("ReviewTitle") %>' Width="200px"></asp:TextBox></td>
                </tr>    

                <tr>
                    <td><b> Author: </b></td>
                    <td align="center"><asp:CheckBox ID="ReviewAuthorInc" Checked='<%# (bool)Eval("ReviewAuthorInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="ReviewAuthor" runat="server" Text='<%# Eval("ReviewAuthor") %>' Width="200px"></asp:TextBox></td>
                </tr>   

                <tr>
                    <td><b> Review Date: </b></td>
                    <td align="center"><asp:CheckBox ID="ReviewDateInc" Checked='<%# (bool)Eval("ReviewDateInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="ReviewStartDate" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("ReviewStartDate")) %>'></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceReviewStartDate" runat="server" TargetControlID="ReviewStartDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meReviewStartDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="ReviewStartDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="ReviewEndDate" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("ReviewEndDate")) %>'></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceReviewEndDate" runat="server" TargetControlID="ReviewEndDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meReviewEndDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="ReviewEndDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                        </b>
                    </td>
                </tr> 


            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender4"
runat="server"
CollapseControlID="pnlClickRev"
Collapsed="true"
ExpandControlID="pnlClickRev"
TextLabelID="lblMessageRev"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsRev"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColRev"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>


<br /><br />

<asp:Panel ID="pnlClickDraw" runat="server" CssClass="pnlCSS">

    <div style="height:30px; vertical-align:middle;  background-color: #0066FF;" class="linearBg2">
        <div style="float:left; color:White;padding:5px 5px 5px 5px">Random Drawing Information</div>
        <div style="float:right; color:White; padding:5px 5px 0 0"><asp:Label ID="lblMessageDraw" runat="server" Text="Label"/> &nbsp;<asp:Image ID="imgArrowsDraw" runat="server" /></div>
        <div style="clear:both"></div>
    </div>

</asp:Panel>
<asp:Panel ID="pnlColDraw" runat="server" Height="0" CssClass="pnlCSS">
            <table>             
                <tr  style="background-color: #dddddd; padding:20px" >
                    <td width="175px" style="padding-top:10px; padding-bottom: 10px; padding-left: 5px;"><b> Report Field </b></td>
                    <td width="75px" align="center"><b> Display </b></td>
                    <td width="500px" style="padding-left: 5px;"><b> Random Drawing Information Filter </b></td>
                </tr>



                <tr>
                    <td><b> Random Drawing Name: </b></td>
                    <td align="center"><asp:CheckBox ID="RandomDrawingNameInc" Checked='<%# (bool)Eval("RandomDrawingNameInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:TextBox ID="RandomDrawingName" runat="server" Text='<%# Eval("RandomDrawingName") %>' Width="200px"></asp:TextBox></td>
                </tr>   

                <tr>
                    <td><b> Random Drawing Number: </b></td>
                    <td align="center"><asp:CheckBox ID="RandomDrawingNumInc" runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>                            
                            <asp:TextBox ID="RandomDrawingNum" runat="server" Text='<%# FormatHelper.ToWidgetDisplayInt((int)Eval("RandomDrawingNum")) %>'></asp:TextBox>
                            <asp:RegularExpressionValidator id="RegularExpressionValidator1"
                                ControlToValidate="RandomDrawingNum"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Random Drawing Num must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Random Drawing Num must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator1"
                                ControlToValidate="RandomDrawingNum"
                                MinimumValue="0"
                                MaximumValue="99999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Random Drawing Num must be from 0 to 99999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Random Drawing Num must be from 0 to 99999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />
                                
                    </td>
                </tr> 

                <tr>
                    <td><b> Random Drawing Date: </b></td>
                    <td align="center"><asp:CheckBox ID="RandomDrawingDateInc" Checked='<%# (bool)Eval("RandomDrawingDateInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td>
                        <b>From: &nbsp;
                                <asp:TextBox ID="RandomDrawingStartDate" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("RandomDrawingStartDate")) %>'></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceRandomDrawingStartDate" runat="server" TargetControlID="RandomDrawingStartDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meRandomDrawingStartDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RandomDrawingStartDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        To: &nbsp;
                                <asp:TextBox ID="RandomDrawingEndDate" runat="server" Width="75px" ReadOnly="False" Text='<%# FormatHelper.ToWidgetDisplayDate((DateTime)Eval("RandomDrawingEndDate")) %>'></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceRandomDrawingEndDate" runat="server" TargetControlID="RandomDrawingEndDate">
                                </ajaxToolkit:CalendarExtender>
                                <ajaxToolkit:MaskedEditExtender ID="meRandomDrawingEndDate" runat="server" 
                                    UserDateFormat="MonthDayYear" TargetControlID="RandomDrawingEndDate" MaskType="Date" Mask="99/99/9999">
                                </ajaxToolkit:MaskedEditExtender>    
                        </b>
                    </td>
                </tr> 

                <tr>
                    <td><b> User has been drawn: </b></td>
                    <td align="center"><asp:CheckBox ID="HasBeenDrawnInc" Checked='<%# (bool)Eval("HasBeenDrawnInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:CheckBox ID="HasBeenDrawn" runat="server"  Checked='<%# (bool)Eval("HasBeenDrawn") %>' ReadOnly="False"></asp:CheckBox></td>
                </tr> 

                <tr>
                    <td><b> User has redeemed prize: </b></td>
                    <td align="center"><asp:CheckBox ID="HasRedeemendInc" Checked='<%# (bool)Eval("HasRedeemendInc") %>' runat="server" ReadOnly="False"></asp:CheckBox></td>
                    <td><asp:CheckBox ID="HasRedeemend" runat="server"  Checked='<%# (bool)Eval("HasRedeemend") %>' ReadOnly="False"></asp:CheckBox></td>
                </tr> 
            </table>
</asp:Panel>            
<ajaxToolkit:CollapsiblePanelExtender
ID="CollapsiblePanelExtender5"
runat="server"
CollapseControlID="pnlClickDraw"
Collapsed="true"
ExpandControlID="pnlClickDraw"
TextLabelID="lblMessageDraw"
CollapsedText="Show"
ExpandedText="Hide"
ImageControlID="imgArrowsDraw"
CollapsedImage="~/ControlRoom/Images/dn.gif"
ExpandedImage="~/ControlRoom/Images/up.gif"
ExpandDirection="Vertical"
TargetControlID="pnlColDraw"
ScrollContents="false">

</ajaxToolkit:CollapsiblePanelExtender>


            </EditItemTemplate>
        </asp:TemplateField>
   


            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False"
                SortExpression="AddedUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblSQl" runat="server" Text=""></asp:Label>

    <asp:GridView ID="gv" runat="server">
    </asp:GridView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="GRA.SRP.DAL.SRPReport">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="RID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsProg" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsBadge" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Badge">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsTemplate" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.ReportTemplate">
    </asp:ObjectDataSource>

   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

   <asp:ObjectDataSource ID="odsSchoolType" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School Type" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDLibSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>


    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>


</asp:Content>

