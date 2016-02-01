<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CRMenu.ascx.cs" Inherits="GRA.SRP.ControlRoom.ControlRoomMenu" %>
        <div style="text-align: left; padding-top: 5px; padding-bottom:5px; padding-left: 5px; font-size: 30px;">The Great Reading Adventure</div>
    <div id="cdnavcont">
	    <div id="cdnavheader">
            <ul>
            <li id="current" style="list-style: none!important"><a href="/ControlRoom/PageList.aspx"><span><i class="icon-file"></i> Pages</span></a></li>
            <li id="current" style="list-style: none!important"><a href="/ControlRoom/MenuEdit.aspx"><span ><i class="icon-edit"></i> Site Menu</span></a></li>
            <li id="current" style="list-style: none!important"><a href="/ControlRoom/TemplateList.aspx"><span><i class="icon-flag"></i> Templates</span></a></li>
            <li id="current" style="list-style: none!important"><a href="/ControlRoom/ModuleList.aspx"><span><i class="icon-share"></i> Modules</span></a></li>
            <li id="current" style="list-style: none!important"><a href="/ControlRoom/WidgetList.aspx"><span><i class="icon-check"></i> Widgets</span></a></li>
            <li id="current" style="list-style: none!important"><a href="/ControlRoom/Modules/Security/Default.aspx"><span><i class="icon-user"></i> CMS Users</span></a></li>
            </ul>
            
        </div>
        <a href="/ControlRoom/Logoff.aspx" style="float:right; padding-top:2px;padding-right:3px;"><img src="/ControlRoom/Images/on-off.png" width="24px" border="0"/></a>
        <br />
        <br style="clear:both;" />
    </div>