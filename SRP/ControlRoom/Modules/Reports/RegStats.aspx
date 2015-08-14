<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="RegStats.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.RegStats" 

%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", {
            packages: ["corechart"]
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%"  style="border:double 3px #A3C0E8;">
<tr>
    <th colspan="2"><b>Program</b></th>
    <th><b>Library System</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" 
            DataTextField="AdminName" DataValueField="PID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="LibSys" runat="server" DataSourceID="odsDDLibSys" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <th colspan="2"><b>Library/Branch</b></th>
    <th><b>School</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="School" runat="server" DataSourceID="odsDDSchool" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>

<tr>
<td colspan="2"> 

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    <asp:Button ID="btnClear0" runat="server" Text="Export" 
         Width="150px" CSSClass="btn-lg btn-gray" onclick="btnExport_Click"/>

</td>
<td><input id="chkDetail" type="checkbox" runat="server"/><b>&nbsp;Display Graphs</b></td>
</tr>
    
</table>
    <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>

   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:ObjectDataSource ID="odsDDLibSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <br />

<div>
    <table cellpadding="5" cellspacing="0" border="1" width="100%">
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <th align="center" width="50%">Program</th>        
            <td align="center" width="10%">Age</td>
            <td align="center" width="10%">Male Count</td>  
            <td align="center" width="10%">Female Count</td>       
            <td align="center" width="10%">Other Count</td>       
            <td align="center" width="10%">N/A</td>       
            <td align="center" width="10%">Total</td>
        </tr>    

        <asp:Repeater runat="server" ID="rptr" >
        <ItemTemplate>
            <tr style="font-weight: normal;">
                <td align="left"><%# Eval("AdminName")%></td>        
                <td align="right"><%# Eval("Age")%></td>
                <td align="right"><%# FormatHelper.ToInt((int)Eval("Male"))%></td>  
                <td align="right"><%# FormatHelper.ToInt((int)Eval("Female")) %></td>       
                <td align="right"><%# FormatHelper.ToInt((int)Eval("Other")) %></td>       
                <td align="right"><%# FormatHelper.ToInt((int)Eval("NA")) %></td>       
                <td align="right"><%# FormatHelper.ToInt((int)Eval("Male") + (int)Eval("Female") + (int)Eval("Other") + (int)Eval("NA"))%></td>       
        
            </tr> 
            <tr style="font-weight: normal; display: none;" id='trGraph_<%# Eval("ProgID")%>_<%# Eval("Age")%>' >
                <td colspan="2"></td>       
                <td colspan="5" align="center">
                    <div id='Epie_<%# Eval("ProgID")%>_<%# Eval("Age")%>'></div>
                    <script type="text/javascript">

                        $(document).ready(function () {
                            EdrawChart<%# Eval("ProgID")%>_<%# Eval("Age")%>();
                            
                        })



                        function EdrawChart<%# Eval("ProgID")%>_<%# Eval("Age")%>() {
                            var data = google.visualization.arrayToDataTable([
          ['', ''],
          ['Male', <%# Eval("Male")%> ],
          ['Female', <%# Eval("Female")%> ],
          ['Other', <%# Eval("Other")%> ],
          ['N/A', <%# Eval("NA")%>]
        ]);

                            var options = {
                                width: 'auto',
                                height: '160',
                                backgroundColor: 'transparent',
                                tooltip: {
                                    textStyle: {
                                        color: '#666666',
                                        fontSize: 11
                                    },
                                    showColorCode: true
                                },
                                legend: {
                                    position: 'right',
                                    textStyle: {
                                        color: 'black',
                                        fontSize: 12
                                    }
                                }
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Epie_<%# Eval("ProgID")%>_<%# Eval("Age")%>'));
                            chart.draw(data, options);
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChart<%# Eval("ProgID")%>_<%# Eval("Age")%>();
                                
                            });
                        });
                        
                        if  ($('#<%=chkDetail.ClientID%>').attr('checked')) {
                            $('#trGraph_<%# Eval("ProgID")%>_<%# Eval("Age")%>').css('display', '');
                        }

                    </script>

                </td>        
           </tr> 

        </ItemTemplate>
        </asp:Repeater>


    </table>

</div>
</asp:Content>


