<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreview.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.QuestionPreview" %>
<div id="Description" runat="server" visible="true">
<asp:Label ID="lblQText" runat="server" Text="" Visible="true"></asp:Label>
</div>
<div id="FreeForm" runat="server" visible="false">
    <asp:TextBox ID="txtAnswer" runat="server" Width="98%"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvFreeForm" ControlToValidate="txtAnswer" runat="server" ErrorMessage="" Display="Dynamic" 
            SetFocusOnError="True" Font-Bold="True"  Enabled="false"><font color='red'> * Required </font></asp:RequiredFieldValidator>
</div>
<div id="MultipleChoice" runat="server" visible="false">
    <asp:Repeater ID="rptrChk" runat="server">
    <ItemTemplate>
        <div class='<%# string.Format("mcdiv{0}", Eval("QID")) %>'>
            <asp:Label ID="SQCID" runat="server" Text='<%# Eval("SQCID") %>' Visible="false"></asp:Label>
            <asp:CheckBox ID="chkAns" runat="server" Checked="False" Text='<%# Eval("ChoiceText") %>' CSSclass='<%# string.Format("ch{0}", Eval("QID")) %>' 
                    AutoPostBack='<%# (bool)Eval("AskClarification") && (bool)Eval("ClarificationRequired") %>'
                    oncheckedchanged="chkAns_CheckedChanged"
            />

            &nbsp;&nbsp;
            <asp:TextBox ID="txtChkClarification" runat="server" Width="150px" Visible='<%# Eval("AskClarification") %>'></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvChkClarification" ControlToValidate="txtChkClarification" runat="server" ErrorMessage="" Display="Dynamic" 
                    Enabled="false"
                    SetFocusOnError="True" Font-Bold="True"  ><font color='red'><br /> * Required <br /></font></asp:RequiredFieldValidator>   
            &nbsp;&nbsp;&nbsp;&nbsp;
        </div>
    </ItemTemplate>
    </asp:Repeater>
    <asp:CustomValidator ID="rfvMCChk" ErrorMessage="" SetFocusOnError="True" Font-Bold="True" Enabled="false" Display="Dynamic" 
        ClientValidationFunction="" runat="server"><font color='red'><br /> * Choice Required <br /></font></asp:CustomValidator>


    <asp:Repeater ID="rptrRadio" runat="server">
    <ItemTemplate>
        <div class='<%# string.Format("mcdiv{0}", Eval("QID")) %>'>
            <asp:Label ID="SQCID" runat="server" Text='<%# Eval("SQCID") %>' Visible="false"></asp:Label>
            <asp:RadioButton ID="rbAns" runat="server" Text='<%# Eval("ChoiceText") %>' 
                Cssclass='<%# string.Format("ch{0}", Eval("QID")) %>'
                Checked='<%# ((int)Eval("ChoiceOrder") == 1 ? true : false) %>' 
                AutoPostBack='True'

                oncheckedchanged="rbAns_CheckedChanged"
            />
            &nbsp;&nbsp;
            <asp:TextBox ID="txtRBClarification" runat="server" Width="150px" Visible='<%# Eval("AskClarification") %>'></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvRBClarification" ControlToValidate="txtRBClarification" runat="server" ErrorMessage="" Display="Dynamic" 
                    SetFocusOnError="True" Font-Bold="True"  
                    Enabled='<%# ((int)Eval("ChoiceOrder") == 1 &&  (bool)Eval("AskClarification") && (bool)Eval("ClarificationRequired") ? true : false) %>' 
                    ><font color='red'> * Required </font></asp:RequiredFieldValidator>   
            &nbsp;&nbsp;&nbsp;&nbsp;
        </div>
    </ItemTemplate>
    </asp:Repeater>
<script type="text/javascript">
$(document).ready(function () {
    $("#<%=MultipleChoice.ClientID %>").each(function () {
        var objRadiosContainer = $(this);
        objRadiosContainer.find("input[type=radio]").each(function () {
            var objRadio = $(this);
            objRadio.change(function () {
                if(objRadio.get(0).checked)
                {
                    objRadiosContainer.find("input[type=radio]").each(function () {
                        if($(this).get(0).id != objRadio.get(0).id)
                        {
                            $(this).get(0).checked = false;
                        }
                    });
                }
            });
        });
    });
});
</script>

    <asp:DropDownList ID="ddMultipleChoice" runat="server" Visible="false" 
        Width="98%" AutoPostBack="true" 
        onselectedindexchanged="ddMultipleChoice_SelectedIndexChanged">
    </asp:DropDownList>
    <asp:CompareValidator ID="rfvddMultipleChoice" runat="server" Enabled='false'
        ControlToValidate="ddMultipleChoice" Display="Dynamic" ErrorMessage="" 
        SetFocusOnError="True" Font-Bold="True" Operator="GreaterThan" ValueToCompare="0"><font color='red'><br /> * Choice Required <br /></font></asp:CompareValidator>

    <asp:TextBox ID="txtDDClarification" runat="server" Width="98%"  Visible="false"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvDDClarification" ControlToValidate="txtDDClarification" runat="server" ErrorMessage="" Display="Dynamic" Visible="true"
            SetFocusOnError="True" Font-Bold="True"  Enabled='false'><font color='red'><br /> * Clarification Required <br /></font></asp:RequiredFieldValidator>
</div>
<div id="Matrix" runat="server" visible="false">
        <asp:ObjectDataSource ID="odsChoiceNames" runat="server" 
            SelectMethod="GetAll" 
            TypeName="GRA.SRP.DAL.SQChoices">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblQID" Name="QID" PropertyName="Text" Type="Int32" />
            
        </SelectParameters>     
	    </asp:ObjectDataSource>

<table>
<tr>
<td></td>
        <asp:Repeater ID="rptrColChoices" runat="server" DataSourceID="odsChoiceNames">
        <ItemTemplate>
            <td align="center" valign="middle">
               <%# Eval("ChoiceText") %>
            </td>
        </ItemTemplate>
        </asp:Repeater>
</tr>
<asp:ObjectDataSource ID="odsLines" runat="server" 
            SelectMethod="GetAll" 
            TypeName="GRA.SRP.DAL.SQMatrixLines">
        <SelectParameters>
        <asp:ControlParameter ControlID="lblQID" Name="QID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>     
	    </asp:ObjectDataSource>

<asp:Repeater ID="rptrMRows" runat="server" DataSourceID="odsLines">
<ItemTemplate>
    <tr id='<%# "Line"+ Eval("LineOrder") %>'>
        <td>
            <%# Eval("LineText") %>
            <asp:Label ID="SQMLID" runat="server" Text='<%# Eval("SQMLID") %>' Visible="false"></asp:Label>
            <asp:Label ID="LineOrder" runat="server" Text='<%# Eval("LineOrder") %>' Visible="false"></asp:Label>
            <asp:CustomValidator ID="rfvMCChk" ErrorMessage="" SetFocusOnError="True" Font-Bold="True" Display="Dynamic" 
                Enabled='<%# lblDisplayControl.Text == "1" && lblIsRequired.Text == "yes" %>' 
                ClientValidationFunction='<%# "Line" + Eval("LineOrder") %>' runat="server"><font color='red'><br /> * Answer Required <br /></font></asp:CustomValidator>

<script type='text/javascript'>
    function <%# "Line" + Eval("LineOrder") %>(sender, args) {
      var isValid = false; 
      $("#<%# "Line"+ Eval("LineOrder") %>").each(function () {
            var objChkContainer = $(this);
            objChkContainer.find("input[type=checkbox]").each(function () {
                var objChk = $(this);
                isValid = isValid || objChk.attr('checked') || objChk[0].attr('checked');  
            });
      });
      args.IsValid = isValid;
    }
</script>
        </td>
        
        <asp:ObjectDataSource ID="odsChoices" runat="server" 
            SelectMethod="GetAllWEcho" 
            TypeName="GRA.SRP.DAL.SQChoices">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblQID" Name="QID" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlId = "LineOrder" Name="Echo" PropertyName="Text" Type="Int32" />
        </SelectParameters>     
	    </asp:ObjectDataSource>

        <asp:Repeater ID="rptrRadioCols" runat="server" DataSourceID="odsChoices" Visible='<%# lblDisplayControl.Text == "2" %>'>
        <ItemTemplate>
            <td align="center" valign="middle">
                <asp:RadioButton ID="rbChoice" runat="server" GroupName='<%# "GP" + Eval("Echo") %>' Checked='<%# ((int)Eval("ChoiceOrder")==1 ? true : false) %>'/>
                <asp:Label ID="ChoiceOrder" runat="server" Text='<%# Eval("ChoiceOrder") %>' Visible="false"></asp:Label>
                <asp:Label ID="SQCID" runat="server" Text='<%# Eval("SQCID") %>' Visible="false"></asp:Label>
            </td>
        </ItemTemplate>
        </asp:Repeater>

        <asp:Repeater ID="rptrCheckCols" runat="server" DataSourceID="odsChoices" Visible='<%# lblDisplayControl.Text == "1" %>'>
        <ItemTemplate>
            <td align="center" valign="middle">
                <asp:CheckBox ID="rbChoice" runat="server" />
                <asp:Label ID="ChoiceOrder" runat="server" Text='<%# Eval("ChoiceOrder") %>' Visible="false"></asp:Label>
                <asp:Label ID="SQCID" runat="server" Text='<%# Eval("SQCID") %>' Visible="false"></asp:Label>
            </td>
        </ItemTemplate>
        </asp:Repeater>

    </tr>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%# "Line"+ Eval("LineOrder") %>").each(function () {
            var objRadiosContainer = $(this);
            objRadiosContainer.find("input[type=radio]").each(function () {
                var objRadio = $(this);
                objRadio.change(function () {
                    if (objRadio.get(0).checked) {
                        objRadiosContainer.find("input[type=radio]").each(function () {
                            if ($(this).get(0).id != objRadio.get(0).id) {
                                $(this).get(0).checked = false;
                            }
                        });
                    }
                });
            });
        });
    });
</script>
</ItemTemplate>
</asp:Repeater>
</table>



</div>
<div id="EndOfPage" runat="server" visible="false">
    <asp:Button ID="btnContinue" runat="server" Text="Continue" CommandName="EOP" CommandArgument="0" CausesValidation="True" />
</div>
<div id="EndOfTest" runat="server" visible="false">
    <asp:Button ID="btnDone" runat="server" Text="Submit" CommandName="EOT" CommandArgument="0" CausesValidation="True"/>
</div>
<asp:Label ID="lblSID" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="lblQID" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="lblQNumber" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="lblQType" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="lblDisplayControl" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="lblDisplayDirection" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="lblIsRequired" runat="server" Text="0" Visible="false"></asp:Label>

<asp:Label ID="lblSRID" runat="server" Text="0" Visible="false"></asp:Label>





