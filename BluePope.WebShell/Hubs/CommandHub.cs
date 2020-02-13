using BluePope.WebShell.Lib;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataReceivedEventArgs = BluePope.WebShell.Lib.DataReceivedEventArgs;

namespace BluePope.WebShell.Hubs
{
    public class CommandHub : Hub
    {
        //static Dictionary<string, Process> _cmd = new Dictionary<string, Process>();
        static IHubContext<CommandHub> _hub;
        static StringBuilder cmdOutput = new StringBuilder();
        static FixedProcess cmdProcess;
        static StreamWriter cmdStreamWriter;

        public CommandHub(IHubContext<CommandHub> hub)
        {
            if (_hub == null)
            {
                _hub = hub;

                //Process로 하면 OutputDataReceived 이벤트 발생기준이 NewLine 이라서 마지막줄을 못가져옴
                //Stackoverflow 에서 커스텀 버전을 봄
                //https://stackoverflow.com/questions/1033648/c-sharp-redirect-console-application-output-how-to-flush-the-output

                if (cmdProcess == null)
                {
                    cmdProcess = new FixedProcess();

                    cmdProcess.StartInfo.FileName = "cmd";
                    cmdProcess.StartInfo.UseShellExecute = false;
                    cmdProcess.StartInfo.CreateNoWindow = true;
                    cmdProcess.StartInfo.RedirectStandardOutput = true;
                    cmdProcess.StartInfo.RedirectStandardInput = true;
                    cmdProcess.OutputDataReceived += CmdProcess_OutputDataReceived;
                    //cmdProcess.EnableRaisingEvents = true;
                    //cmdProcess.Exited += CmdProcess_Exited;

                    cmdProcess.Start();

                    cmdStreamWriter = cmdProcess.StandardInput;
                    cmdProcess.BeginOutputReadLine();
                }
            }
        }

        public async Task SendCommand(string cmd)
        {
            await Task.Run(() => {
                cmdStreamWriter.WriteLine(cmd);
            });
            
            //await Clients.All.SendAsync("ReceiveMessage", cmd);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("ReceiveMessage", WebUtility.HtmlEncode(cmdOutput.ToString()));
        }

        private void CmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                var data = e.Data;

                cmdOutput.Append(e.Data);
                
                Task.WaitAll(_hub.Clients.All.SendAsync("ReceiveMessage", WebUtility.HtmlEncode(e.Data)));
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
