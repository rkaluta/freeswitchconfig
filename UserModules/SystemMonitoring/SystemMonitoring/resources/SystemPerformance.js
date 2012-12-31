CreateNameSpace('Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance');

Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance = {
    AppendPoint: function(array,val){
        if ((array==undefined)||(array==null)){
            array = new Array();
        }
        if(array.length>10){
            array.splice(0,1);
            for(var y=0;y<array.length;y++){
                array[y][0]=y;
            }
        }
        array.push([array.length,val]);
        return array;
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        container.append('<div id="SysInfoContainer" style="width:100%"></div>');
        container.append('<div id="cpuRamContainer" style="width:350px;height:250px;display:inline-block;float:left;"></div><div id="cpuRamLegend" style="width:150px;height:250px;display:inline-block;float:left;"></div>');
        container.append('<div id="netContainer" style="width:350px;height:250px;display:inline-block;float:left;"></div><div id="netLegend" style="width:150px;height:250px;display:inline-block;float:left;"></div>');
        container.append('<div id="partitionsContainer" style="width:350px;height:250px;display:inline-block;float:left;"></div><div id="partitionsLegend" style="width:150px;height:300px;display:inline-block;float:left;"></div>');
        container.append('<div id="callsContainer" style="width:350px;height:250px;display:inline-block;float:left;"></div><div id="callsLegend" style="width:150px;height:250px;display:inline-block;float:left;"></div>');
        container.append('<div style="clear:left;"></div>');
        var sysInfo = $('#SysInfoContainer').html('<h3>System Information</h3><br/>');
        var rows = new Array();
        Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemMonitorService.GetSystemInformation(
            function(msg){
                rows.push(new TableRow([
                    new TableCell('CPU(s):',{'style':'vertical-align:top;','rowspan':msg.CPUs.length}),
                    new TableCell(msg.CPUs[0].SocketNumber),
                    new TableCell(msg.CPUs[0].Speed+' MHz')
                ]));
                for(var y=1;y<msg.CPUs.length;y++){
                    rows.push(new TableRow([
                        new TableCell(msg.CPUs[y].SocketNumber,{'style':'vertical-align:top;'}),
                        new TableCell(msg.CPUs[y].Speed+' MHz')
                    ]));
                }
                rows.push(new TableRow([new TableCell('Memory:',{'rowspan':'2','style':'vertical-align:top'}),new TableCell('RAM:',{'style':'vertical-align:top;'}), new TableCell(msg.RAM)]));
                rows.push(new TableRow([new TableCell('SWAP:',{'style':'vertical-align:top;'}), new TableCell(msg.SWAP)]));
            },
            null,
            null,
            true
        );
        rows.push(new TableRow([new TableCell('Top CPU:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'topCPUName'}),new TableCell('',{'id':'topCPUPercentage'})]));
        rows.push(new TableRow([new TableCell('Top RAM:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'topRAMName'}),new TableCell('',{'id':'topRAMAmount'})]));
        rows.push(new TableRow([new TableCell('Freeswitch:',{'style':'vertical-align:top;','rowspan':'4'}), new TableCell('Uptime'),new TableCell('',{'id':'uptime'})]));
        rows.push(new TableRow([new TableCell('Current Sessions:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'curSessions'})]));
        rows.push(new TableRow([new TableCell('Sessions Since Startup:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'totSessions'})]));
        rows.push(new TableRow([new TableCell('Sessions Per Second:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'perSessions'})]));
        rows.push(new TableRow([new TableCell('Inbound Control Connections:',{'style':'vertical-align:top;','rowspan':'2'}), new TableCell('Max:'),new TableCell('',{'id':'Average_Inbound_Connection_Duration'})]));
        rows.push(new TableRow([new TableCell('Average:',{'style':'vertical-align:top;'}),new TableCell('',{'id':'Max_Inbound_Connection_Duration'})]));
        rows.push(new TableRow([new TableCell('Path Usages:',{'style':'vertical-align:top;','rowspan':'4'}), new TableCell('Logs:'),new TableCell('',{'id':'HD_Logs'})]));
        rows.push(new TableRow([new TableCell('Configurations:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'HD_Configs'})]));
        rows.push(new TableRow([new TableCell('Database:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'HD_Database'})]));
        rows.push(new TableRow([new TableCell('Voicemails:',{'style':'vertical-align:top;'}), new TableCell('',{'id':'HD_Voicemail'})]));
        var tbl = RenderTable(rows);
        AppendToTable(tbl,new TableBody(null,{'id':'hdsContainer'}));
        sysInfo.append(tbl);
        Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData = new Object();
        Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance.LoadStats();
    },
    LoadStats: function(){
        if ($('#cpuRamContainer').length>0){
            FreeswitchConfig.Site.Modals.ShowLoading();
            var cpus = new Array();
            var nets = new Array();
            var parts = new Array();
            var calls = new Array();
            var hds = new Array();
            Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemMonitorService.GetTopProcesses(
                function(msg){
                    $('#topCPUName').html(msg.TOP_Cpu.Name);
                    $('#topCPUPercentage').html(msg.TOP_Cpu.Value);
                    $('#topRAMName').html(msg.TOP_Memory.Name);
                    $('#topRAMAmount').html(msg.TOP_Memory.Value);
                }
            );
            Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemMonitorService.GetInboundConnectionStats(
                function(msg){
                    for(var x=0;x<msg.length;x++){
                        $('#'+msg[x].Name).html(msg[x].Value);
                    }
                }
            );
            Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemMonitorService.GetFreeswitchStatus(
                function(msg){
                    if (msg!=null){
                        $('#uptime').html(msg.UpTimeString);
                        $('#totSessions').html(msg.TotalSessions);
                        $('#curSessions').html(msg.CurrentSessions);
                        $('#perSessions').html(msg.SessionsPerSecond);
                    }
                }
            );
            Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemMonitorService.PathSizeMetrics(
                function(msg){
                    for(var y=0;y<msg.length;y++){
                        $('#'+msg[y].Name).html(msg[y].Value);
                    }
                }
            );
            Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemMonitorService.GetGraphValues(
                function(msg){
                    for (var y=0;y<msg.length;y++){
                        Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData[msg[y].Name] = 
                            Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance.AppendPoint(
                                Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData[msg[y].Name],
                                msg[y].Value
                            );
                        if (msg[y].Name.indexOf('CPU')==0 || msg[y].Name.indexOf('RAM')==0 || msg[y].Name.indexOf('SWAP')==0){
                            cpus.push({
                                lines: {show:true, steps:true},
                                points: {show:true},
                                label: msg[y].Name,
                                data: Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData[msg[y].Name]
                            });
                        }else if (msg[y].Name.indexOf('NET')==0){
                            nets.push({
                                lines: {show:true, steps:true},
                                points: {show:true},
                                label: msg[y].Name,
                                data: Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData[msg[y].Name]
                            });
                        }else if (msg[y].Name.indexOf('Partition')==0){
                            parts.push({
                                lines: {show:true, steps:true},
                                points: {show:true},
                                label: msg[y].Name,
                                data: Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData[msg[y].Name]
                            });    
                        }else if (msg[y].Name.indexOf('HD_Used')==0){
                           hds.push(msg[y]);
                        }else{
                            calls.push({
                                lines: {show:true, steps:true},
                                points: {show:true},
                                label: msg[y].Name,
                                data: Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance._curData[msg[y].Name]
                            });
                        }
                    }
                },
                null,
                null,
                true
            );
            $.plot($('#cpuRamContainer'),cpus,{legend:{container:$('#cpuRamLegend')}});
            $.plot($('#netContainer'),nets,{legend:{container:$('#netLegend')}});
            $.plot($('#partitionsContainer'),parts,{legend:{container:$('#partitionsLegend')}});
            $.plot($('#callsContainer'),calls,{legend:{container:$('#callsLegend')}});
            var hdsContainer = $('#hdsContainer');
            hdsContainer.html('');
            if (hds.length==0){
                hdsContainer.append('<tr><td colspan="3">No Hard Partitions where detected on the system.</td></tr>');
            }else if (hds.length==1){
                hdsContainer.append('<tr><td style="vertical-align:top;" class="Rowed">Space Used:</td><td class="Rowed">'+hds[0].Name.replace('HD_Used - ','')+'</td><td class="Rowed">'+hds[0].Value+'</td></tr>');
            }else{
                hdsContainer.append('<tr><td style="vertical-align:top;" rowspan="'+hds.length+'" class="Rowed">Space Used:</td><td>'+hds[0].Name.replace('HD_Used - ','')+'</td><td>'+hds[0].Value+'</td></tr>');
                for (var y=1;y<hds.length;y++){
                    if (y==hds.length-1){
                        hdsContainer.append('<tr><td class="Rowed">'+hds[y].Name.replace('HD_Used - ','')+'</td><td class="Rowed">'+hds[y].Value+'</td></tr>');
                    }else{
                        hdsContainer.append('<tr><td>'+hds[y].Name.replace('HD_Used - ','')+'</td><td>'+hds[y].Value+'</td></tr>');
                    }
                }
            }
            setTimeout('Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance.LoadStats()',60000);
            FreeswitchConfig.Site.Modals.HideLoading();
        }
    }
}