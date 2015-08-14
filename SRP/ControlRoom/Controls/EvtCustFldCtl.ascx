<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EvtCustFldCtl.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.EvtCustFldCtl" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<asp:Label ID="InputVal" runat="server" ViewStateMode="Enabled" Visible='False'></asp:Label>
 <asp:Label ID="Lbl" runat="server" Text=""></asp:Label><br />
<asp:TextBox ID="Txt" runat="server" Text="" Visible='False'></asp:TextBox>
<asp:DropDownList ID="DD" runat="server" DataSourceID="" DataTextField="Code" DataValueField="Code" 
    AppendDataBoundItems="True" Visible='False'
    >
    <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
</asp:DropDownList>
<asp:ObjectDataSource ID="odsDD" runat="server" 
    SelectMethod="GetAlByTypeID" 
    TypeName="GRA.SRP.DAL.Codes">
    <SelectParameters>
        <asp:ControlParameter ControlID="TypeID" DefaultValue="0" Name="id" 
            PropertyName="Text" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:Label ID="TypeID" runat="server" Text="0" Visible="False"></asp:Label>
