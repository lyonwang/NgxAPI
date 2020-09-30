using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NgxAPI.Helpers;
using NgxAPI.Models;

namespace NgxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NginxController : ControllerBase
    {
        IConfiguration _config;
        IHostingEnvironment _env;

        public NginxController(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // GET api/nginx/{domain}
        [HttpGet("{domain}")]
        public ActionResult<WhiteList_Req> Get(string domain)
        {
            WhiteList_Req resp = new WhiteList_Req()
            {
                domain = domain,
                ips = new List<WhiteList_Req_Data>()
            };
            Regex regex = new Regex(@"^allow\s+([0-9|\.]+);\s*#{0,1}(.*)$");
            string line = String.Empty;
            string result = String.Empty;
            string whiltListFilePath = $"{this._config["Settings:WhiteListFolder"]}/" +
                                       domain +
                                       this._config["Settings:DomainPostfix"];
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(whiltListFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    var r = regex.Match(line);
                    if (r.Groups.Count > 0)
                    {
                        resp.ips.Add(new WhiteList_Req_Data()
                        {
                            ip = r.Groups[1].Value.Trim(),
                            desc = r.Groups[2].Value.Trim()
                        });
                    }
                }
                file.Close();
            }
            catch (Exception ex)
            {
                result = $"{ex.Message}";
            }

            return resp;
        }

        // POST api/nginx
        [HttpPost]
        public ActionResult Post([FromBody] WhiteList_Req wlist)
        {
            string whiltListFilePath = $"{this._config["Settings:WhiteListFolder"]}/" +
                                       wlist.domain +
                                       this._config["Settings:DomainPostfix"];

            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(whiltListFilePath);
                foreach (var ipitem in wlist.ips)
                {
                    file.WriteLine($"allow {ipitem.ip}; #{ipitem.desc}");
                }
                file.Close();

                //string resultStr = "echo Chuchu0116 | sudo nginx -s reload".Bash();
                // 改成以下，因使用service方式，root帐号启动
                string resultStr = "nginx -s reload".Bash();
                if (!String.IsNullOrEmpty(resultStr))
                {
                    return Content(resultStr);
                }
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message}");
            }

            return Content("Success");
        }

        // GET api/nginx/np/{domain}
        [HttpGet("np/{domain}")]
        public ActionResult<WhiteList_Req> GetNP(string domain)
        {
            WhiteList_Req resp = new WhiteList_Req()
            {
                domain = domain,
                ips = new List<WhiteList_Req_Data>()
            };
            Regex regex = new Regex(@"^allow\s+([0-9|\.]+);\s*#{0,1}(.*)$");
            string line = String.Empty;
            string result = String.Empty;
            string whiltListFilePath = $"{this._config["Settings:WhiteListFolder"]}/" +
                                       domain +
                                       this._config["Settings:NPDomainPostfix"];
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(whiltListFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    var r = regex.Match(line);
                    if (r.Groups.Count > 0)
                    {
                        resp.ips.Add(new WhiteList_Req_Data()
                        {
                            ip = r.Groups[1].Value.Trim(),
                            desc = r.Groups[2].Value.Trim()
                        });
                    }
                }
                file.Close();
            }
            catch (Exception ex)
            {
                result = $"{ex.Message}";
            }

            return resp;
        }

        // POST api/nginx/np
        [HttpPost("np")]
        public ActionResult PostNP([FromBody] WhiteList_Req wlist)
        {
            string whiltListFilePath = $"{this._config["Settings:WhiteListFolder"]}/" +
                                       wlist.domain +
                                       this._config["Settings:NPDomainPostfix"];

            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(whiltListFilePath);
                foreach (var ipitem in wlist.ips)
                {
                    file.WriteLine($"allow {ipitem.ip}; #{ipitem.desc}");
                }
                file.Close();

                //string resultStr = "echo Chuchu0116 | sudo nginx -s reload".Bash();
                // 改成以下，因使用service方式，root帐号启动
                string resultStr = "nginx -s reload".Bash();
                if (!String.IsNullOrEmpty(resultStr))
                {
                    return Content(resultStr);
                }
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message}");
            }

            return Content("Success");
        }

        // POST api/nginx/server
        [HttpPost("server")]
        public ActionResult PostServer([FromBody] Server_Req servers)
        {
            string serverBlockFilesFolder = this._config["Settings:ServerBlockFilesFolder"];
            string serverBlockFilePrefix = this._config["Settings:ServerBlockFilePrefix"];
            string serverBlockMainFile = this._config["Settings:ServerBlockMainFileFullPath"];
            string template = servers.template;
            if (String.IsNullOrEmpty(servers.template) || servers.domains == null || servers.domains.Count == 0)
            {
                return new UnprocessableEntityResult();
            }

            try
            {
                List<string> serverBlockFileNames = new List<string>();
                foreach (var domain in servers.domains)
                {
                    var dFileName = $"{serverBlockFilesFolder}/{serverBlockFilePrefix}{domain}";
                    using (var dfile = new System.IO.StreamWriter(dFileName))
                    {
                        var dContent = template.Replace("$$${ServerName}$$$", domain);
                        dfile.WriteLine(dContent);
                    }
                    serverBlockFileNames.Add(dFileName);
                }
                using (var file = new System.IO.StreamWriter(serverBlockMainFile))
                {
                    foreach (var serverBlockFileName in serverBlockFileNames)
                    {
                        file.WriteLine($"include {serverBlockFileName};");
                    }
                }
                //string resultStr = "echo Chuchu0116 | sudo nginx -s reload".Bash();
                // 改成以下，因使用service方式，root帐号启动
                string resultStr = "openresty -s reload".Bash();
                if (!String.IsNullOrEmpty(resultStr))
                {
                    return Content(resultStr);
                }
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message}");
            }

            return Content("Success");
        }

        // POST api/nginx/server/clean
        [HttpPost("server/clean")]
        public ActionResult CleanServer()
        {
            string serverBlockFilesFolder = this._config["Settings:ServerBlockFilesFolder"];
            string serverBlockMainFile = this._config["Settings:ServerBlockMainFileFullPath"];

            try
            {
                //清空 server_block_main.conf
                using (var file = new System.IO.StreamWriter(serverBlockMainFile))
                {
                    file.WriteLine("");
                }
                //string resultStr = "echo Chuchu0116 | sudo nginx -s reload".Bash();
                // 改成以下，因使用service方式，root帐号启动
                //清除 server_block_domain files
                string resultStr = $"rm -f {serverBlockFilesFolder}/*".Bash();
                //重啟 nginx
                resultStr = "openresty -s reload".Bash();
                if (!String.IsNullOrEmpty(resultStr))
                {
                    return Content(resultStr);
                }
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message}");
            }

            return Content("Success");
        }
    }
}