<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="MiniGameStats.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.MiniGameStats" 

%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

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
    <th colspan="2" ><b>Minigame</b></th>
    <th><b>Start Date</b></th>
    <th><b>End Date</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" 
            DataTextField="AdminName" DataValueField="MGID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        
        <asp:TextBox ID="StartDate" runat="server" Width="75px"                         
            Text=''></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="cetxtDOB" runat="server" TargetControlID="StartDate">
        </ajaxToolkit:CalendarExtender>
        <ajaxToolkit:MaskedEditExtender ID="metxtDOB" runat="server" 
            UserDateFormat="MonthDayYear" TargetControlID="StartDate" MaskType="Date" Mask="99/99/9999">
        </ajaxToolkit:MaskedEditExtender>

    </td>
    <td width="25%">

        <asp:TextBox ID="EndDate" runat="server" Width="75px"                         
            Text=''></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="EndDate">
        </ajaxToolkit:CalendarExtender>
        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" 
            UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999">
        </ajaxToolkit:MaskedEditExtender>        

    </td>
</tr>
<tr>
<td colspan="3"> 

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    <asp:Button ID="btnClear0" runat="server" Text="Export" 
         Width="150px" CSSClass="btn-lg btn-gray" onclick="btnClear0_Click"/>

</td>
<td><input id="chkDetail" type="checkbox" runat="server"/><b>&nbsp;Display Graphs</b></td>
</tr>
    
</table>

    <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Minigame">
    </asp:ObjectDataSource>
    <br />
<div>
    <table cellpadding="5" cellspacing="0" border="1" width="100%">
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <th rowspan=2 align="center">Patron</th>        
            <td rowspan=2 align="center">Email</td>
            <td rowspan=2 align="center">Gender</td>  
            <td rowspan=2 align="center">MiniGame</td>       
            <td rowspan=2 align="center">Type</td>       
            <td colspan=2 align="center">Easy Level</td>
            <td colspan=2 align="center">Medium Level</td>
            <td colspan=2 align="center">Hard Level</td>
        </tr>    
        <tr>
            <td >Started</td>        
            <td >Completed</td>        
            <td >Started</td>        
            <td >Completed</td>        
            <td >Started</td>        
            <td >Completed</td>        
        </tr>    

        <asp:Repeater runat="server" ID="rptr" >
        <ItemTemplate>
            <tr style="font-weight: normal;">
                <td align="left"><%# Eval("Username")%> - <%# Eval("First Name") %> <%# Eval("Last Name")%></td>        
                <td align="left"><%# Eval("Email")%></td>
                <td align="center"><%# Eval("Gender")%></td>  
                <td align="left"><%# Eval("Administrative Name")%></td>       
                <td align="left"><%# Eval("MiniGame Type")%></td>       
                <td align="right"><%# Eval("EasyLevelStated")%></td>        
                <td align="right"><%# Eval("EasyLevelCompleted")%></td>        
                <td align="right"><%# Eval("MediumLevelStated")%></td>        
                <td align="right"><%# Eval("MediumLevelCompleted")%></td>        
                <td align="right"><%# Eval("HardLevelStated")%></td>        
                <td align="right"><%# Eval("HardLevelCompleted")%></td>        
            </tr> 
            <tr style="font-weight: normal; display: none;" id='trGraph_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>' >
                <td colspan="5"></td>       
                <td colspan="2" align="center">
                    <div id='Epie_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>'></div>
                    <script type="text/javascript">

                        $(document).ready(function () {
                            EdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>();
                            
                        })



                        function EdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>() {
                            var data = google.visualization.arrayToDataTable([
          ['', ''],
          ['Not-Completed', <%# Eval("EasyLevelStated")%> - <%# Eval("EasyLevelCompleted")%>],
          ['Completed', <%# Eval("EasyLevelCompleted")%>]
        ]);

                            var options = {
                                width: 'auto',
                                height: '160',
                                backgroundColor: 'transparent',
                                colors: ['#ed6d49', '#74b749'],
                                tooltip: {
                                    textStyle: {
                                        color: '#666666',
                                        fontSize: 11
                                    },
                                    showColorCode: true
                                },
                                legend: {
                                    position: 'none',
                                    textStyle: {
                                        color: 'black',
                                        fontSize: 12
                                    }
                                }
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Epie_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>'));
                            chart.draw(data, options);
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>();
                                
                            });
                        });
                        

                    </script>

                </td>        
                <td colspan="2" align="center">
                    <div id='Mpie_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>'></div>
                    <script type="text/javascript">

                        $(document).ready(function () {
                            MdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>();
                        })



                        function MdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>() {
                            var data = google.visualization.arrayToDataTable([
          ['', ''],
          ['Not-Completed', <%# Eval("MediumLevelStated")%> - <%# Eval("MediumLevelCompleted")%>],
          ['Completed', <%# Eval("MediumLevelCompleted")%>]
        ]);

                            var options = {
                                width: 'auto',
                                height: '160',
                                backgroundColor: 'transparent',
                                colors: ['#ed6d49', '#74b749'],
                                tooltip: {
                                    textStyle: {
                                        color: '#666666',
                                        fontSize: 11
                                    },
                                    showColorCode: true
                                },
                                legend: {
                                    position: 'none',
                                    textStyle: {
                                        color: 'black',
                                        fontSize: 12
                                    }
                                }
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Mpie_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>'));
                            chart.draw(data, options);
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                MdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>();
                            });
                        });

                        
                    </script>

                </td>    
                <td colspan="2" align="center">
                    <div id='Hpie_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>'></div>
                    <script type="text/javascript">

                        $(document).ready(function () {
                            HdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>();
                        })



                        function HdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>() {
                            var data = google.visualization.arrayToDataTable([
          ['', ''],
          ['Not-Completed', <%# Eval("HardLevelStated")%> - <%# Eval("HardLevelCompleted")%>],
          ['Completed', <%# Eval("HardLevelCompleted")%>]
        ]);

                            var options = {
                                width: 'auto',
                                height: '160',
                                backgroundColor: 'transparent',
                                colors: ['#ed6d49', '#74b749'],
                                tooltip: {
                                    textStyle: {
                                        color: '#666666',
                                        fontSize: 11
                                    },
                                    showColorCode: true
                                },
                                legend: {
                                    position: 'none',
                                    textStyle: {
                                        color: 'black',
                                        fontSize: 12
                                    }
                                }
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Hpie_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>'));
                            chart.draw(data, options);
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                HdrawChart<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>();
                                
                            });
                        });
                        
                        if  ($('#<%=chkDetail.ClientID%>').attr('checked')) {
                            $('#trGraph_<%# Eval("Patron ID")%>_<%# Eval("MiniGame ID")%>').css('display', '');
                        }
                    </script>

                </td>                </tr> 

        </ItemTemplate>
        </asp:Repeater>


    </table>

</div>
</asp:Content>
