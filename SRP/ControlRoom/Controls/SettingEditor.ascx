<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingEditor.ascx.cs" Inherits="STG.SRP.ControlRoom.Controls.SettingEditor" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:TextBox ID="uxTextBox" runat="server" Width="98%" Visible="False"></asp:TextBox>
<asp:CheckBox ID="uxCheckBox" runat="server" Visible="False" />

<CKEditor:CKEditorControl ID="uxEditor" 
        BasePath="/ckeditor/" 
        runat="server" 
        Skin="office2003" 
         
        BodyId="wrapper" 
        ContentsCss="/css/EditorStyles.css" 
        DisableNativeSpellChecker="False" 
        DisableNativeTableHandles="False" 
        DocType="&lt;!DOCTYPE html&gt;" 
        ForcePasteAsPlainText="True" 
        Height="250px" UIColor="#D3D3D3" 
        Visible="False" 
        Width="98%"
        Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
        / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
        / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
        / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
        "
        AutoGrowOnStartup="True" 
     
        ></CKEditor:CKEditorControl>

        <!--BodyClass="container"
        TemplatesFiles="/Layout/CKEditorTemplates/templates/EditTemplates.js"-->

<asp:TextBox ID="uxMultitext" runat="server" Visible="False" TextMode="MultiLine" Rows="5" Width="98%"></asp:TextBox>
<asp:RadioButtonList ID="uxRadio" runat="server" Visible="False"></asp:RadioButtonList>
<asp:DropDownList ID="uxDrop" runat="server" Visible="False"  Width="98%"></asp:DropDownList>

<asp:TextBox ID="uxDate" runat="server" Width="75px" Visible="false"></asp:TextBox>
    <ajaxToolkit:CalendarExtender ID="cePFRR" runat="server" 
    TargetControlID="uxDate" Format="MM/dd/yyyy">
    </ajaxToolkit:CalendarExtender>
    <ajaxToolkit:MaskedEditExtender ID="meePFRR" runat="server" 
        UserDateFormat="MonthDayYear" TargetControlID="uxDate" MaskType="Date" Mask="99/99/9999">
    </ajaxToolkit:MaskedEditExtender>    


