<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronRegistration.ascx.cs" Inherits="STG.SRP.Controls.PatronRegistration" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Import Namespace="STG.SRP.DAL" %>
<script src="/Scripts/jquery.ddslick.js" type="text/javascript"></script>

<script language="javascript" type="text/javascript">
    function ClientValidate(source, arguments) {
        arguments.IsValid = ($(".pwd").val() == $(".pwd2").val() );
    }
    function ParentPermFlagValidation(source, arguments) {
        arguments.IsValid = jQuery(".ParentPermFlag input:checkbox").is(':checked');
    }
    function TermsOfUseflagValidation(source, arguments) {
        arguments.IsValid = jQuery(".TermsOfUseflag input:checkbox").is(':checked');
    }

    
</script>

<div class="container">
    <div class="form-wrapper form-wide" style="min-height: 400px!important;">
	      <h3 class="title-divider">
              <asp:Label ID="Label1" runat="server" Text="Registration Title"></asp:Label>
          </h3>
          <asp:Label ID="Label2" runat="server" Text="Registration Message"></asp:Label>
          <hr>    

    <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000" ></asp:Label>

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="None" BorderWidth="1px" 
        Font-Bold="True" 
        HeaderText="Please correct the following errors: "  ForeColor="Red"
    />

    <asp:Label ID="Step" runat="server" Text="1" Visible="False"></asp:Label>
    <asp:Label ID="RegistrationAge" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="RegisteringFamily" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="MasterPID" runat="server" Text="" Visible="False"></asp:Label>
    <table style="font-weight: bold;width: 100%; " border="0">
<asp:Panel ID="Panel0" runat="server" Visible="False">        
        <tr>
            <td colspan="2" > 
                <asp:Label ID="Label3" runat="server" Text="Registration Family Member Message"></asp:Label>
                <hr/> 
            </td>
        <tr/>
</asp:Panel>        

    <asp:Repeater ID="rptr" runat="server"  
        onitemcommand="rptr_ItemCommand" onitemdatabound="rptr_ItemDataBound"
        >
        <ItemTemplate>



<asp:Panel ID="Panel1" runat="server" Visible="True">

        <tr  style='display: <%# ((bool)Eval("SchoolGrade_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label3" runat="server" Text="Registration label School Grade"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                 <asp:TextBox ID="SchoolGrade" runat="server" Text=''
                    ontextchanged="SchoolGrade_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSchoolGrade" runat="server" Enabled='<%# Eval("SchoolGrade_Req") %>'
                    ControlToValidate="SchoolGrade" Display="Dynamic" ErrorMessage="School Grade is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("DOB_Prompt")? "normal" : "none") %>'>
            <td valign="top"><asp:Label ID="Label7" runat="server" Text="Registration label DOB"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="DOB" runat="server" Text='' 
                    Width="100px" Enabled='<%# (bool)Eval("DOB_Prompt") %>' CssClass="datepicker"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDOB" runat="server" 
                    ControlToValidate="DOB" Display="Dynamic" ErrorMessage="Date of Birth is required" Enabled='<%# Eval("DOB_Req") %>'
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="DOB" 
                    ErrorMessage="Invalid Date of Birth format" Type="Date" 
                    Operator="DataTypeCheck" Display="Dynamic" Text="* Invalid format" ForeColor="Red" ></asp:CompareValidator>
                    <br /><br /><br /><br /><br /><br /><br /><br /><br />
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("Age_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label8" runat="server" Text="Registration label Age"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Age" runat="server" Text='' 
                     Enabled='<%# (bool)Eval("Age_Prompt") %>' Width="50px" CssClass="align-right"  ontextchanged="Age_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAge" runat="server" Enabled='<%# Eval("Age_Req") %>'
                    ControlToValidate="Age" Display="Dynamic" ErrorMessage="Age is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revAge"
                    ControlToValidate="Age"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Age must be numeric.</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Age must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvAge"
                    ControlToValidate="Age"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Age must be from 0 to 99!</font>"
                    runat="server"  
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Age must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </td>
        <tr/>
</asp:Panel>

<asp:Panel ID="Panel2" runat="server" Visible="False">

        <tr>
            <td colspan="2">
                <asp:Label ID="Label9" runat="server" Text="Registration label Family Account"></asp:Label>
                
                <br />
                <br />
                <asp:RadioButtonList ID="FamilyAccount" runat="server" Width="200px" RepeatDirection="Horizontal" RepeatLayout="Table">
                    <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                    <asp:ListItem Value="No" Text="No" Selected="True"></asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Enabled="True"
                    ControlToValidate="FamilyAccount" Display="Dynamic" ErrorMessage="Yes or No is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                <br />
                <br />
                                <br />
                <br />
                                <br />
                <br />
                                <br />
 
            </td>
        <tr/>

</asp:Panel>

<asp:Panel ID="Panel3" runat="server" Visible="False">

        <tr style='display: normal'>
            <td><asp:Label ID="Label10" runat="server" Text="Registration label Program"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" DataTextField="TabName" DataValueField="PID" 
                    AppendDataBoundItems="True"
                    >
                    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvProgram" runat="server" Enabled='true'
                    ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator> 
              <asp:CompareValidator ID="CompareValidator1" runat="server" Enabled='true'
                        ControlToValidate="ProgID" Display="Dynamic" ErrorMessage="Program is required" 
                        SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>
            </td>
        <tr/>


        





        <tr  style='display: <%# ((bool)Eval("FirstName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label11" runat="server" Text="Registration label FirstName"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="FirstName" runat="server" Text='' Enabled='<%# (bool)Eval("FirstName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Enabled='<%# Eval("FirstName_Req") %>'
                    ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("MiddleName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label26" runat="server" Text="Registration label MiddleName"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="MiddleName" runat="server" Text='' Enabled='<%# (bool)Eval("MiddleName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMiddleName" runat="server" Enabled='<%# Eval("MiddleName_Req") %>'
                    ControlToValidate="MiddleName" Display="Dynamic" ErrorMessage="Middle Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("LastName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label12" runat="server" Text="Registration label LastName"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="LastName" runat="server" Text='' Enabled='<%# (bool)Eval("LastName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" Enabled='<%# Eval("LastName_Req") %>'
                    ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("Gender_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label13" runat="server" Text="Registration label Gender"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td>                 
                     <asp:DropDownList ID="Gender" runat="server" 
                        Enabled='<%# (bool)Eval("Gender_Prompt") %>'
                        AppendDataBoundItems="True"
                        >
                        <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                        <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                        <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                        <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                    </asp:DropDownList>
               <asp:RequiredFieldValidator ID="rfvGender" runat="server" Enabled='<%# Eval("Gender_Req") %>'
                    ControlToValidate="Gender" Display="Dynamic" ErrorMessage="Gender is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("EmailAddress_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label14" runat="server" Text="Registration label Email"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="EmailAddress" runat="server" Text='' Enabled='<%# (bool)Eval("EmailAddress_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server"  Enabled='<%# Eval("EmailAddress_Req") %>'
                    ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email Address is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

                <asp:RegularExpressionValidator ID="revEM" runat="server" ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email Address is not valid" 
                    SetFocusOnError="True" Font-Bold="True" 
                    ValidationExpression="[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?"><font color='red'> * Not valid </font></asp:RegularExpressionValidator>

            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("PhoneNumber_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label15" runat="server" Text="Registration label Phone"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="PhoneNumber" runat="server" Text='' Enabled='<%# (bool)Eval("PhoneNumber_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" Enabled='<%# Eval("PhoneNumber_Req") %>'
                    ControlToValidate="PhoneNumber" Display="Dynamic" ErrorMessage="Phone Number is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="PhoneNumber" Display="Dynamic" 
                    ErrorMessage="Phone Number is not valid" 
                    SetFocusOnError="True" Font-Bold="True" 
                    ValidationExpression="\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"
                ><font color='red'> * Not valid </font></asp:RegularExpressionValidator>


            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("StreetAddress1_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label16" runat="server" Text="Registration label Addr1"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="StreetAddress1" runat="server" Text='' Enabled='<%# (bool)Eval("StreetAddress1_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvStreetAddress1" runat="server" Enabled='<%# Eval("StreetAddress1_Req") %>'
                    ControlToValidate="StreetAddress1" Display="Dynamic" ErrorMessage="Street Address 1 is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("StreetAddress2_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label17" runat="server" Text="Registration label Addr2"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="StreetAddress2" runat="server" Text='' Enabled='<%# (bool)Eval("StreetAddress2_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvStreetAddress2" runat="server" Enabled='<%# Eval("StreetAddress2_Req") %>'
                    ControlToValidate="StreetAddress2" Display="Dynamic" ErrorMessage="Street Address 2 is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("City_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label18" runat="server" Text="Registration label City"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="City" runat="server" Text='' Enabled='<%# (bool)Eval("City_Prompt") %>' ontextchanged="City_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCity" runat="server" Enabled='<%# Eval("City_Req") %>'
                    ControlToValidate="City" Display="Dynamic" ErrorMessage="City is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("State_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label19" runat="server" Text="Registration label State"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="State" runat="server" Text='' Enabled='<%# (bool)Eval("State_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvState" runat="server" Enabled='<%# Eval("State_Req") %>'
                    ControlToValidate="State" Display="Dynamic" ErrorMessage="State is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("ZipCode_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label20" runat="server" Text="Registration label Zip"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="ZipCode" runat="server" Text='' Enabled='<%# (bool)Eval("ZipCode_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Enabled='<%# Eval("ZipCode_Req") %>'
                    ControlToValidate="ZipCode" Display="Dynamic" ErrorMessage="Zip Code is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="ZipCode" Display="Dynamic" 
                    ErrorMessage="Zip Code is not valid" 
                    SetFocusOnError="True" Font-Bold="True" 
                    ValidationExpression="\d{5}-?(\d{4})?$"
                ><font color='red'> * Not valid </font></asp:RegularExpressionValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("Country_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label21" runat="server" Text="Registration label Country"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Country" runat="server" Text='' Enabled='<%# (bool)Eval("Country_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCountry" runat="server" Enabled='<%# Eval("Country_Req") %>'
                    ControlToValidate="Country" Display="Dynamic" ErrorMessage="Country is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("County_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label22" runat="server" Text="Registration label County"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="County" runat="server" Text='' Enabled='<%# (bool)Eval("County_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCounty" runat="server" Enabled='<%# Eval("County_Req") %>'
                    ControlToValidate="County" Display="Dynamic" ErrorMessage="County is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

</asp:Panel>

<asp:Panel ID="Panel4" runat="server" Visible="False">
        <tr  style='display: <%# ((bool)Eval("ParentGuardianFirstName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label23" runat="server" Text="Registration label PGFN"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td>
                <asp:TextBox ID="Panel4Visibility" runat="server" 
                    Text='<%# ((bool)Eval("ParentGuardianFirstName_Prompt") || (bool)Eval("ParentGuardianMiddleName_Prompt") || (bool)Eval("ParentGuardianLastName_Prompt")  ? "1" : "0") %>' 
                    Visible="false"></asp:TextBox> 
                <asp:TextBox ID="ParentGuardianFirstName" runat="server" Text='' Enabled='<%# (bool)Eval("ParentGuardianFirstName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvParentGuardianFirstName" runat="server" Enabled='<%# Eval("ParentGuardianFirstName_Req") %>'
                    ControlToValidate="ParentGuardianFirstName" Display="Dynamic" ErrorMessage="Parent/Guardian First Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("ParentGuardianMiddleName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label24" runat="server" Text="Registration label PGMN"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="ParentGuardianMiddleName" runat="server" Text='' Enabled='<%# (bool)Eval("ParentGuardianMiddleName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvParentGuardianMiddleName" runat="server" Enabled='<%# Eval("ParentGuardianMiddleName_Req") %>'
                    ControlToValidate="ParentGuardianMiddleName" Display="Dynamic" ErrorMessage="Parent/Guardian Middle Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("ParentGuardianLastName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label25" runat="server" Text="Registration label PGLN"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="ParentGuardianLastName" runat="server" Text='' Enabled='<%# (bool)Eval("ParentGuardianLastName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvParentGuardianLastName" runat="server" Enabled='<%# Eval("ParentGuardianLastName_Req") %>'
                    ControlToValidate="ParentGuardianLastName" Display="Dynamic" ErrorMessage="Parent/Guardian Last Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>
            <tr>
                <td colspan="2">
                    <br /><br /><br />
                </td>
            </tr>
</asp:Panel>

<asp:Panel ID="Panel5" runat="server" Visible="False">
        <tr  style='display: <%# ((bool)Eval("District_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label30" runat="server" Text="Registration label District"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Panel5Visibility" runat="server" 
                    Text='<%# ((bool)Eval("PrimaryLibrary_Prompt") || (bool)Eval("LibraryCard_Prompt") || (bool)Eval("SchoolName_Prompt") || (bool)Eval("District_Prompt")  || (bool)Eval("SDistrict_Prompt") || (bool)Eval("Teacher_Prompt") || (bool)Eval("GroupTeamName_Prompt") || (bool)Eval("SchoolType_Prompt") || (bool)Eval("LiteracyLevel1_Prompt") || (bool)Eval("LiteracyLevel2_Prompt")   ? "1" : "0") %>' 
                    Visible="false"></asp:TextBox> 


                <asp:DropDownList ID="District" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("District_Edit") %>' 
                    AutoPostBack="true"
                    OnSelectedIndexChanged="District_SelectedIndexChanged"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" Enabled='<%# Eval("District_Req") %>'
                    ControlToValidate="District" Display="Dynamic" ErrorMessage="Library District is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
               <asp:CompareValidator ID="CompareValidator6" runat="server" Enabled='<%# Eval("District_Req") %>'
                        ControlToValidate="District" Display="Dynamic" ErrorMessage="Library District is required" 
                        SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>
            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("PrimaryLibrary_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label27" runat="server" Text="Registration label Library"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 


    
                <asp:DropDownList ID="PrimaryLibrary" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("PrimaryLibrary_Prompt") %>' 
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                    ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary Library is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator> 
                <asp:CompareValidator runat="server" Enabled='<%# Eval("PrimaryLibrary_Req") %>'
                    ControlToValidate="PrimaryLibrary" Display="Dynamic" ErrorMessage="Primary Library is required" 
                    SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>

            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("LibraryCard_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label28" runat="server" Text="Registration label LibraryCard"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="LibraryCard" runat="server" Text='' Enabled='<%# (bool)Eval("LibraryCard_Prompt") %>'
                ></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLibraryCard" runat="server" Enabled='<%# Eval("LibraryCard_Req") %>'
                    ControlToValidate="LibraryCard" Display="Dynamic" ErrorMessage="Library Card # is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("SchoolType_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label33" runat="server" Text="Registration label SchoolType"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:DropDownList ID="SchoolType" runat="server" DataSourceID="odsDDSchoolType" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("SchoolType_Prompt") %>'  CssClass="align-right"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="SchoolType_SelectedIndexChanged"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSchoolType" runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                    ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School Type is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
               <asp:CompareValidator ID="CompareValidator4" runat="server" Enabled='<%# Eval("SchoolType_Req") %>'
                        ControlToValidate="SchoolType" Display="Dynamic" ErrorMessage="School Type is required" 
                        SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>

            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("SDistrict_Show")? "normal" : "none") %>'>
            <td><asp:Label ID="Label5" runat="server" Text="Registration label School District"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:DropDownList ID="SDistrict" runat="server" DataSourceID="odsDDSDistrict" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("SDistrict_Edit") %>'
                    AutoPostBack="true" 
                    OnSelectedIndexChanged="SDistrict_SelectedIndexChanged"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                    ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
               <asp:CompareValidator ID="CompareValidator3" runat="server" Enabled='<%# Eval("SDistrict_Req") %>'
                        ControlToValidate="SDistrict" Display="Dynamic" ErrorMessage="School District is required" 
                        SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>

            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("SchoolName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label29" runat="server" Text="Registration label School"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:DropDownList ID="SchoolName" runat="server" DataSourceID="odsDDSchool" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("SchoolName_Edit") %>' 
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSchoolName" runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                    ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
               <asp:CompareValidator ID="CompareValidator5" runat="server" Enabled='<%# Eval("SchoolName_Req") %>'
                        ControlToValidate="SchoolName" Display="Dynamic" ErrorMessage="School Name is required" 
                        SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'> * Required </font></asp:CompareValidator>
            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("Teacher_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label31" runat="server" Text="Registration label Teacher"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Teacher" runat="server" Text='' Enabled='<%# (bool)Eval("Teacher_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTeacher" runat="server" Enabled='<%# Eval("Teacher_Req") %>'
                    ControlToValidate="Teacher" Display="Dynamic" ErrorMessage="Teacher is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("GroupTeamName_Prompt")? "normal" : "none") %>'>
            <td><asp:Label ID="Label32" runat="server" Text="Registration label Group"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="GroupTeamName" runat="server" Text='' Enabled='<%# (bool)Eval("GroupTeamName_Prompt") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGroupTeamName" runat="server" Enabled='<%# Eval("GroupTeamName_Req") %>'
                    ControlToValidate="GroupTeamName" Display="Dynamic" ErrorMessage="Group/Team Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("LiteracyLevel1_Prompt")? "normal" : "none") %>'>
            <td><%# Eval("Literacy1Label")%>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="LiteracyLevel1" runat="server" Text='' 
                     Enabled='<%# (bool)Eval("LiteracyLevel1_Prompt") %>' Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLiteracyLevel1" runat="server" Enabled='<%# Eval("LiteracyLevel1_Req") %>'
                    ControlToValidate="LiteracyLevel1" Display="Dynamic" ErrorMessage='<%# Eval("Literacy1Label")%> + " is required"'
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revLiteracyLevel1"
                    ControlToValidate="LiteracyLevel1"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage='<%# String.Format("<font color=\"red\">{0} must be numeric.</font>", Eval("Literacy1Label")) %>'
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text='<%# String.Format("<font color=\"red\"> * {0} must be numeric.</font>", Eval("Literacy1Label")) %>'
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvLiteracyLevel1"
                    ControlToValidate="LiteracyLevel1"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage='<%# String.Format("<font color=\"red\">{0}must be from 0 to 99!</font>", Eval("Literacy1Label")) %>'
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text='<%# String.Format("<font color=\"red\"> * {0} must be from 0 to 99!</font>", Eval("Literacy1Label")) %>'
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("LiteracyLevel2_Prompt")? "normal" : "none") %>'>
            <td><%# Eval("Literacy1Label")%>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="LiteracyLevel2" runat="server" Text='' 
                     Enabled='<%# (bool)Eval("LiteracyLevel2_Prompt") %>' Width="50px" CssClass="align-right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLiteracyLevel2" runat="server" Enabled='<%# Eval("LiteracyLevel2_Req") %>'
                    ControlToValidate="LiteracyLevel2" Display="Dynamic" ErrorMessage='<%# Eval("Literacy2Label")%> + " is required"' 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revLiteracyLevel2"
                    ControlToValidate="LiteracyLevel2"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage='<%# String.Format("<font color=\"red\">{0} must be numeric.</font>", Eval("Literacy2Label")) %>'
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text='<%# String.Format("<font color=\"red\"> * {0} must be numeric.</font>", Eval("Literacy2Label")) %>'
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvLiteracyLevel2"
                    ControlToValidate="LiteracyLevel2"
                    MinimumValue="0"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                   ErrorMessage='<%# String.Format("<font color=\"red\">{0}must be from 0 to 99!</font>", Eval("Literacy2Label")) %>'
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text='<%# String.Format("<font color=\"red\"> * {0} must be from 0 to 99!</font>", Eval("Literacy2Label")) %>'
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </td>
        <tr/>


</asp:Panel>

<asp:Panel ID="Panel6" runat="server" Visible="False">
<asp:TextBox ID="Panel6Visibility" runat="server" 
                    Text='<%# ((bool)Eval("Custom1_Prompt") || (bool)Eval("Custom2_Prompt") || (bool)Eval("Custom3_Prompt")|| (bool)Eval("Custom4_Prompt") || (bool)Eval("Custom5_Prompt") || (bool)Eval("TermsOfUseflag_Prompt")  ? "1" : "0") %>' 
                    Visible="false"></asp:TextBox> 

        <tr  style='display: <%# ((bool)Eval("Custom1_Show") && CustomRegistrationFields.FetchObject().DDValues1 == "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label1 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Custom1" runat="server"  Enabled='<%# (bool)Eval("Custom1_Edit") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCustom1" runat="server" Enabled='<%# (bool) Eval("Custom1_Req") && CustomRegistrationFields.FetchObject().DDValues1 == "" %>'
                    ControlToValidate="Custom1" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label1 + " is required"%>' 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

       <tr  style='display: <%# ((bool)Eval("Custom1_Show") && CustomRegistrationFields.FetchObject().DDValues1 != "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label1 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                 <asp:DropDownList ID="Custom1DD" runat="server"  DataTextField="Code" DataValueField="Code"
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("Custom1_Edit") %>'  CssClass="align-right"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="Custom1DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom1_Edit") %>' Visible="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Enabled='<%# (bool)Eval("Custom1_Req") && CustomRegistrationFields.FetchObject().DDValues1 != ""  %>'
                    ControlToValidate="Custom1DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label1 + " is required"%>'  
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("Custom2_Show") && CustomRegistrationFields.FetchObject().DDValues2 == "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label2 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Custom2" runat="server"  Enabled='<%# (bool)Eval("Custom2_Edit") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCustom2" runat="server" Enabled='<%# (bool) Eval("Custom2_Req") && CustomRegistrationFields.FetchObject().DDValues2 == "" %>'
                    ControlToValidate="Custom2" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label2 + " is required"%>' 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

       <tr  style='display: <%# ((bool)Eval("Custom2_Show") && CustomRegistrationFields.FetchObject().DDValues2 != "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label2 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                 <asp:DropDownList ID="Custom2DD" runat="server"  DataTextField="Code" DataValueField="Code"
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("Custom2_Edit") %>'  CssClass="align-right"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="Custom2DDTXT" runat="server"  Enabled='<%# (bool)Eval("Custom2_Edit") %>' Visible="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Enabled='<%# (bool)Eval("Custom2_Req") && CustomRegistrationFields.FetchObject().DDValues2 != ""  %>'
                    ControlToValidate="Custom2DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label2 + " is required"%>'  
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("Custom3_Show") && CustomRegistrationFields.FetchObject().DDValues3 == "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label3 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Custom3" runat="server" Enabled='<%# (bool)Eval("Custom3_Edit") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCustom3" runat="server" Enabled='<%# (bool) Eval("Custom3_Req") && CustomRegistrationFields.FetchObject().DDValues3 == "" %>'
                    ControlToValidate="Custom3" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label3 + " is required"%>' 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

       <tr  style='display: <%# ((bool)Eval("Custom3_Show") && CustomRegistrationFields.FetchObject().DDValues3 != "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label3 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                 <asp:DropDownList ID="Custom3DD" runat="server"  DataTextField="Code" DataValueField="Code"
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("Custom3_Edit") %>'  CssClass="align-right"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="Custom3DDTXT" runat="server" Enabled='<%# (bool)Eval("Custom3_Edit") %>' Visible="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Enabled='<%# (bool)Eval("Custom3_Req") && CustomRegistrationFields.FetchObject().DDValues3 != ""  %>'
                    ControlToValidate="Custom3DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label3 + " is required"%>'  
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("Custom4_Show") && CustomRegistrationFields.FetchObject().DDValues4 == "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label4 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Custom4" runat="server"  Enabled='<%# (bool)Eval("Custom4_Edit") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCustom4" runat="server" Enabled='<%# (bool) Eval("Custom4_Req") && CustomRegistrationFields.FetchObject().DDValues4 == "" %>'
                    ControlToValidate="Custom4" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label4 + " is required"%>' 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

       <tr  style='display: <%# ((bool)Eval("Custom4_Show") && CustomRegistrationFields.FetchObject().DDValues4 != "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label4 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                 <asp:DropDownList ID="Custom4DD" runat="server"  DataTextField="Code" DataValueField="Code"
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("Custom4_Edit") %>'  CssClass="align-right"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="Custom4DDTXT" runat="server"  Enabled='<%# (bool)Eval("Custom4_Edit") %>' Visible="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Enabled='<%# (bool)Eval("Custom4_Req") && CustomRegistrationFields.FetchObject().DDValues4 != ""  %>'
                    ControlToValidate="Custom4DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label4 + " is required"%>'  
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("Custom5_Show") && CustomRegistrationFields.FetchObject().DDValues5 == "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label5 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Custom5" runat="server"  Enabled='<%# (bool)Eval("Custom5_Edit") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCustom5" runat="server" Enabled='<%# (bool) Eval("Custom5_Req") && CustomRegistrationFields.FetchObject().DDValues5 == "" %>'
                    ControlToValidate="Custom5" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label5 + " is required"%>' 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>

       <tr  style='display: <%# ((bool)Eval("Custom5_Show") && CustomRegistrationFields.FetchObject().DDValues5 != "" ? "normal" : "none") %>'>
            <td><%# CustomRegistrationFields.FetchObject().Label5 %>:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                 <asp:DropDownList ID="Custom5DD" runat="server"  DataTextField="Code" DataValueField="Code"
                    AppendDataBoundItems="True"
                    Enabled='<%# (bool)Eval("Custom5_Edit") %>'  CssClass="align-right"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="Custom5DDTXT" runat="server"  Enabled='<%# (bool)Eval("Custom5_Edit") %>' Visible="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" Enabled='<%# (bool)Eval("Custom5_Req") && CustomRegistrationFields.FetchObject().DDValues5 != ""  %>'
                    ControlToValidate="Custom5DD" Display="Dynamic" ErrorMessage='<%# CustomRegistrationFields.FetchObject().Label5 + " is required"%>'  
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
            </td>
        <tr/>


        <tr  style='display: <%# ((bool)Eval("TermsOfUseflag_Prompt")? "normal" : "none") %>'>
          <td valign="top"><asp:Label ID="Label34" runat="server" Text="Registration label Terms"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:Label ID="Label4" runat="server" Text="Registration TermsOfUseflag"></asp:Label>   <br />
                <asp:CheckBox ID="TermsOfUseflag" runat="server" ReadOnly="False" Checked="true" CssClass="TermsOfUseflag"></asp:CheckBox>

               <asp:CustomValidator ID="cvTermsOfUseflag" 
                    ClientValidationFunction="TermsOfUseflagValidation" 
                    
                    EnableClientScript="true"
                    runat="server" Enabled='<%# Eval("TermsOfUseflag_Prompt") %>'
                    ErrorMessage="You must accept the terms of use." SetFocusOnError="True" Font-Bold="True"><font color='red'> * Must Accept </font></asp:CustomValidator>
                <br /> <br />
            </td>
        <tr/>

        <tr  style='display: <%# ((bool)Eval("ShareFlag_Prompt")? "normal" : "none") %>'>
          <td valign="top"><asp:Label ID="Label35" runat="server" Text="Registration label InfoShare"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:Label ID="Label6" runat="server" Text="Registration ShareFlag"></asp:Label>   <br />
                <asp:CheckBox ID="ShareFlag" runat="server" ReadOnly="False" Checked="true"></asp:CheckBox>
                <br /><br />
            </td>
        <tr/>

<asp:Panel ID="pnlConsent" runat="server" Visible="False">
        <tr  style='display: <%# ((bool)Eval("ParentPermFlag_Prompt") && int.Parse(  (RegistrationAge.Text.Length==0?"0": RegistrationAge.Text)) < 18 ? "normal" : "none") %>'>
          <td valign="top"><asp:Label ID="Label36" runat="server" Text="Registration label Consent"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:Label ID="lblConsent" runat="server" Text=""></asp:Label>   <br />
                <asp:CheckBox ID="ParentPermFlag" runat="server" ReadOnly="False" Checked="true" CssClass="ParentPermFlag"></asp:CheckBox>
                <asp:CustomValidator ID="cvParentPermFlag" 
                    ClientValidationFunction="ParentPermFlagValidation" 
                    EnableClientScript="true"
                    runat="server" Enabled='<%# (bool)Eval("ParentPermFlag_Prompt") && int.Parse(  (RegistrationAge.Text.Length==0?"0": RegistrationAge.Text)) < 18 %>'
                    ErrorMessage="You must have parental consent." SetFocusOnError="True" Font-Bold="True"><font color='red'> * Must have consent </font></asp:CustomValidator>
                    <br /><br />
            </td>
        <tr/>
</asp:Panel>


</asp:Panel>

<asp:Panel ID="Panel7" runat="server" Visible="False">

        <tr>
            <td>Username:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Username" runat="server" Text='' Enabled="true"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Enabled="true"
                    ControlToValidate="Username" Display="Dynamic" ErrorMessage="Username is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="MessageFailure"
                    runat="server" ControlToValidate="Username" ValidationExpression="^[a-zA-Z0-9_-]{5,25}$"
                    Display="Dynamic" 
                    ErrorMessage="Username must be at least five characters in length.">
                        <font color='red'> Username must be at least five characters in length. </font>
                    </asp:RegularExpressionValidator>
                
            </td>
        <tr/>

        <tr>
            <td>Password:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Password" runat="server" cssclass="pwd" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NPasswordReq" runat="server" 
                    ControlToValidate="Password" ErrorMessage="Password is required"
                    ToolTip="Password required" SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="uxNewPasswordStrengthValidator" CssClass="MessageFailure"
                    runat="server" ControlToValidate="Password" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                    Display="Dynamic" 
                    ErrorMessage="Password must be at least seven characters in length and contain one alpha and one numeric character.&lt;br&gt;">
                        <font color='red'> Password must be at least seven characters in length and contain one alpha and one numeric character. </font>
                    </asp:RegularExpressionValidator>
            </td>
        <tr/>
        <tr>
            <td>Re-Enter Password:&nbsp;&nbsp;&nbsp;</td>
            <td> 
                <asp:TextBox ID="Password2" runat="server" cssclass="pwd2" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                    ControlToValidate="Password2" ErrorMessage="Password Re-entry is required"
                    ToolTip="Password Re-entry required" SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="uxConfirmNewPasswordCustomValidator" CssClass="MessageFailure" 
                    runat="server" ControlToValidate="Password2"
                    ErrorMessage="The Password and Confirmation of Password do not match. &lt;br&gt;" 
                    ClientValidationFunction="ClientValidate"><font color='red'> The Password and Confirmation of Password do not match. </font></asp:CustomValidator>    
            </td>
        <tr/>

        <tr>
            <td><asp:Label ID="Label37" runat="server" Text="Registration label Avatar"></asp:Label></td>
            <td>
                <select id="ddAvatar"></select>
                <input ID="AvatarID" CssClass="avatar" runat="server" Visible="true" type="text" style="display:none;"  value="1"/>
                <script language="javascript" type="text/javascript">
                    var ddData = [<%# Avatar.GetJSONForSelection( 1) %> ];
                    $('#ddAvatar').ddslick({
                        width: 230,
                        background: "transparent",
                        showSelectedHTML: true,
                        selectText: "Select your Avatar",
                        data: ddData,
                        onSelected: function (data) {
                            $('#MainContent_PatronRegistrationCtl_rptr_AvatarID_0').val(data.selectedData.value);
                        }
                    });
                    $('#ddAvatar').ddslick('select', {index: 1 });
                </script>
                <br /><br /><br />
            </td>
        </tr>


</asp:Panel>


        </ItemTemplate>
    </asp:Repeater>

<asp:Panel ID="Panel8" runat="server" Visible="False">
        <tr >
            <td colspan="2"> 
                <asp:Label ID="Label38" runat="server" Text="Registration Account Success"></asp:Label>                
                <br /><br /><br /><br /> 
            </td>
        </tr>

</asp:Panel>

<asp:Panel ID="Panel9" runat="server" Visible="False">
        <tr >
            <td colspan="2"> 
                <asp:Label ID="Label39" runat="server" Text="Registration FamilyAccount Success"></asp:Label>  
                
                <br /><br /> <br /><br /> 
            </td>
        </tr>

</asp:Panel>


        <tr >
            <td width="250px"></td>
            <td> 
                <asp:Button ID="btnPrev" runat="server" Text="Registration btnPrev" 
                    CausesValidation="True" CssClass="btn a" Enabled="False" 
                    onclick="btnPrev_Click"/>
                &nbsp;
                <asp:Button ID="btnNext" runat="server" Text="Registration btnNext" 
                    CausesValidation="True"  CssClass="btn a" onclick="btnNext_Click"/>

                &nbsp;
                <asp:Button ID="btnDone" runat="server" Text="Registration btnDone" 
                    CausesValidation="True"  CssClass="btn a" Visible="False" OnClick="btnDone_Click"/>
            </td>
        </tr>

    </table>

    <asp:TextBox ID="city" runat="server" Visible="false"></asp:TextBox>
    <asp:TextBox ID="district" runat="server" Visible="false" Text="0"></asp:TextBox>
    <asp:TextBox ID="schType" runat="server" Visible="false" Text="0"></asp:TextBox>
    <asp:TextBox ID="sdistrict" runat="server" Visible="false" Text="0"></asp:TextBox>
    <asp:TextBox ID="grade" runat="server" Visible="false" Text="0"></asp:TextBox>
    <asp:TextBox ID="age" runat="server" Visible="false" Text="0"></asp:TextBox>

   <asp:ObjectDataSource ID="odsDDSchoolType" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School Type" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>            

   <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
        SelectMethod="GetAllActive" 
        TypeName="STG.SRP.DAL.Programs">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDDistrict" runat="server" 
        SelectMethod="GetFilteredDistrictDDValues" 
        TypeName="STG.SRP.DAL.LibraryCrosswalk">
        <SelectParameters>
            <asp:ControlParameter ControlID="city" DefaultValue="" Name="city" 
                PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>  


    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetFilteredBranchDDValues" 
        TypeName="STG.SRP.DAL.LibraryCrosswalk">
        <SelectParameters>
            <asp:ControlParameter ControlID="district" DefaultValue="0" Name="districtID" 
                PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="city" DefaultValue="" Name="city" 
                PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>  

    
   <asp:ObjectDataSource ID="odsDDSDistrict" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>              

    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetFilteredSchoolDDValues" 
        TypeName="STG.SRP.DAL.SchoolCrosswalk">
        <SelectParameters>
            <asp:ControlParameter ControlID="schType" DefaultValue="0" Name="schTypeID" 
                PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="sdistrict" DefaultValue="0" Name="districtID" 
                PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="city" DefaultValue="" Name="city" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="grade" DefaultValue="0" Name="grade" 
                PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="age" DefaultValue="0" Name="age" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

   <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAllActive" 
        TypeName="STG.SRP.DAL.Programs">
    </asp:ObjectDataSource>

    </div>
                
</div>



  <script language="javascript" type="text/javascript">
      $(function () {
          $(".datepicker").datepicker();
      });
  </script>