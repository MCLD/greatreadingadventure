<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingEditor.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.SettingEditor" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:TextBox ID="uxTextBox" runat="server" Width="98%" Visible="False"></asp:TextBox>
<asp:CheckBox ID="uxCheckBox" runat="server" Visible="False" />

<textarea id="uxEditor" runat="server" class="gra-editor" visible="false"></textarea>

<asp:TextBox ID="uxMultitext" runat="server" Visible="False" TextMode="MultiLine" Rows="5" Width="98%"></asp:TextBox>
<asp:RadioButtonList ID="uxRadio" runat="server" Visible="False"></asp:RadioButtonList>
<asp:DropDownList ID="uxDrop" runat="server" Visible="False" Width="98%"></asp:DropDownList>

<asp:TextBox ID="uxDate" runat="server" Width="75px" Visible="false"></asp:TextBox>
<ajaxToolkit:CalendarExtender ID="cePFRR" runat="server"
    TargetControlID="uxDate" Format="MM/dd/yyyy">
</ajaxToolkit:CalendarExtender>
<ajaxToolkit:MaskedEditExtender ID="meePFRR" runat="server"
    UserDateFormat="MonthDayYear" TargetControlID="uxDate" MaskType="Date" Mask="99/99/9999">
</ajaxToolkit:MaskedEditExtender>


