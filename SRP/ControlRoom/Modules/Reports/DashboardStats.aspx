<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="DashboardStats.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.DashboardStats" 

%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    
</td>
<td>
    <b>Game Level to Report On:</b>&nbsp;
            <asp:TextBox ID="Level" runat="server" Text='' 
                                     ReadOnly="False" Width="75px" CssClass="align-right"></asp:TextBox>
                                <asp:RegularExpressionValidator id="revLevel"
                                    ControlToValidate="Level"
                                    ValidationExpression="\d+"
                                    Display="Dynamic"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Level must be numeric.</font>"
                                    runat="server"
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Level must be numeric. </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />      
                                <asp:RangeValidator ID="rvLevel"
                                    ControlToValidate="Level"
                                    MinimumValue="0"
                                    MaximumValue="9999"
                                    Display="Dynamic"
                                    Type="Integer"
                                    EnableClientScript="true"
                                    ErrorMessage="<font color='red'>Level must be from 0 to 9999!</font>"
                                    runat="server" 
                                    Font-Bold="True" Font-Italic="True" 
                                    Text="<font color='red'> * Level must be from 0 to 9999! </font>" 
                                    EnableTheming="True" 
                                    SetFocusOnError="True" />

</td>
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

    <asp:Panel ID="pnlReport" runat="server" Visible="false">
    asasas
    </asp:Panel>
    <table cellpadding="5" cellspacing="0" border="1" width="100%">
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <td align="center" width="50%">
                <div id='Reg_by_prog'></div>
                    <script type="text/javascript">
                        $(document).ready(function () {
                            EdrawChartReg_by_prog();
                        })

                        function EdrawChartReg_by_prog() {
                        try{
                            var data = google.visualization.arrayToDataTable([
                              ['Program', 'Count' ],
                              <%=ViewState["Pie1"]%>
          
                            ]);

                            var options = {
                                title: 'Registrant Count by Program',
                                width: 'auto',
                                height: '300',
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
                                },
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Reg_by_prog'));
                            chart.draw(data, options); } catch(err) {}
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChartReg_by_prog();
                            });
                        });                       
                    </script>
            </td>        
            <td align="center" width="50%">
                <div id='Finisher_by_prog'></div>
                    <script type="text/javascript">
                        $(document).ready(function () {
                            EdrawChartFinisher_by_prog();
                        })

                        function EdrawChartFinisher_by_prog() {
                        try{
                            var data = google.visualization.arrayToDataTable([
                              ['Program', 'Count' ],
                              <%=ViewState["Pie2"]%>
          
                            ]);

                            var options = {
                                title: 'Game Finisher Count by Program',
                                width: 'auto',
                                height: '300',
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
                                },
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Finisher_by_prog'));
                            chart.draw(data, options); } catch(err) {}
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChartFinisher_by_prog();
                            });
                        });                       
                    </script>            
            </td>
        </tr>    
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <td align="center" width="50%">
                <div id='RegByAge'></div>
                    <script type="text/javascript">
                        $(document).ready(function () {
                            EdrawChartRegByAge();
                        })

                        function EdrawChartRegByAge() {

                        try{
                            var data = google.visualization.arrayToDataTable([
                              ['Age', <%=ViewState["Labels1"]%> ],
                              <%=ViewState["Data1"]%>
                              /*
                              ['Age 0',     4,1,1,1,2],
                              ['Age 4',     0,0,0,0,1],
                              ['Age 7',     0,6,0,0,0],
                              ['Age 33',    0,1,0,0,0],
                              ['Age 43',    1,0,0,0,0],
                              ['Age 100',   2,0,1,0,1],
                              */
                            ]);

                            var options = {
                                title: 'Program Registrants by Age',
                                hAxis: {title: 'Age', titleTextStyle: {color: 'black'}},
                                width: 'auto',
                                height: '300',
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
                                },
                                chartArea: {
                                    left: 60,
                                    //top: 10,
                                    //height: '80%'
                                },
                                bar: { groupWidth: '95%' },
                                //isStacked: true,
                            };

                            var chart = new google.visualization.ColumnChart(document.getElementById('RegByAge'));
                            chart.draw(data, options); } catch(err) {}
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChartRegByAge();
                            });
                        });                       
                    </script>
            </td>
            <td align="center" width="50%">
                <div id='FinisherByAge'></div>
                    <script type="text/javascript">
                        $(document).ready(function () {
                            EdrawChartFinisherByAge();
                        })

                        function EdrawChartFinisherByAge() {
                        try{
                            var data = google.visualization.arrayToDataTable([
                              ['Age', <%=ViewState["Labels2"]%> ],
                              <%=ViewState["Data2"]%>
                              /*
                              ['Age 0',     4,1,1,1,2],
                              ['Age 4',     0,0,0,0,1],
                              ['Age 7',     0,6,0,0,0],
                              ['Age 33',    0,1,0,0,0],
                              ['Age 43',    1,0,0,0,0],
                              ['Age 100',   2,0,1,0,1],
                              */
                            ]);

                            var options = {
                                title: 'Program Game Finishers by Age',
                                hAxis: {title: 'Age', titleTextStyle: {color: 'black'}},
                                width: 'auto',
                                height: '300',
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
                                },
                                chartArea: {
                                    left: 60,
                                    //top: 10,
                                    //height: '80%'
                                },
                                bar: { groupWidth: '95%' },
                                //isStacked: true,
                            };

                            var chart = new google.visualization.ColumnChart(document.getElementById('FinisherByAge'));
                            chart.draw(data, options); } catch(err) {}
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChartFinisherByAge();
                            });
                        });                       
                    </script>
            </td>
        </tr> 
        
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <td align="center" width="50%">
                <div id='RegByGender'></div>
                    <script type="text/javascript">
                        $(document).ready(function () {
                            EdrawChartRegByGender();
                        })

                        function EdrawChartRegByGender() {
                        try{
                            var data = google.visualization.arrayToDataTable([
                              ['', 'Male', 'Female', 'Other', 'N/A' ],
                              <%=ViewState["Stacked1"] %>
                              /*
                              ['Adult Reading',     12,12,0],
                              ['Elementary',     4,1,1],
                              ['High School',     10,20,0,],
                              ['Middle School',     0,6,0],
                              ['Pre K-1st Grade',    0,6,0],
                              */
                            ]);

                            var options = {
                                title: 'Program Registrants by Gender',
                                hAxis: {title: 'Program', titleTextStyle: {color: 'black'}},
                                width: 'auto',
                                height: '300',
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
                                },
                                chartArea: {
                                    left: 60,
                                    //top: 10,
                                    //height: '80%'
                                },
                                //bar: { groupWidth: '75%' },
                                isStacked: true,
                            };

                            var chart = new google.visualization.ColumnChart(document.getElementById('RegByGender'));
                            chart.draw(data, options); } catch(err) {}
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChartRegByGender();
                            });
                        });                       
                    </script>
            </td>
            <td align="center" width="50%">
                <div id='FinisherByGender'></div>
                    <script type="text/javascript">
                        $(document).ready(function () {
                            EdrawChartFinisherByGender();
                        })

                        function EdrawChartFinisherByGender() {
                        try{
                            var data = google.visualization.arrayToDataTable([
                              ['', 'Male', 'Female','Other', 'N/A' ],
                              <%=ViewState["Stacked2"] %>
                              /*
                              ['Adult Reading',     12,12,0],
                              ['Elementary',     4,1,1],
                              ['High School',     10,20,0,],
                              ['Middle School',     0,6,0],
                              ['Pre K-1st Grade',    0,6,0],
                              */
                            ]);

                            var options = {
                                title: 'Program Game Finishers by Gender',
                                hAxis: {title: 'Program', titleTextStyle: {color: 'black'}},
                                width: 'auto',
                                height: '300',
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
                                },
                                chartArea: {
                                    left: 60,
                                    //top: 10,
                                    //height: '80%'
                                },
                                //bar: { groupWidth: '75%' },
                                isStacked: true,
                            };

                            var chart = new google.visualization.ColumnChart(document.getElementById('FinisherByGender'));
                            chart.draw(data, options); } catch(err) {}
                        }

                        //Resize charts and graphs on window resize
                        $(document).ready(function () {
                            $(window).resize(function () {
                                EdrawChartFinisherByGender();
                            });
                        });                       
                    </script>
            </td>
        </tr>            
    </table>
</asp:Content>


