using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NPOI.SS.Formula.Functions;
using UalaAccounting.api.ApplicationCore;
using UalaAccounting.api.Models;

namespace UalaAccounting.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecuteController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExecuteController> _logger;
        private readonly IBusinessLogic businessLogic;
        private readonly IConfigurationApp configurationApp;
        private readonly IReclasificationProcess reclasificationProcess;
        private readonly IBackupProcess _logic;
        private readonly ILoanAccountHistoryProcess loanAccountHistoryProcess;
        private readonly IProcessOrchestration _processOrchestration;

        public ExecuteController(ILogger<ExecuteController> logger,
            IBusinessLogic _businessLogic,
            IConfigurationApp _configurationApp,
            IReclasificationProcess _reclasificationProcess,
            IBackupProcess logic,
            ILoanAccountHistoryProcess _loanAccountHistoryProcess,
            IProcessOrchestration processOrchestration,
            IMemoryCache cache) 
        {
            _processOrchestration = processOrchestration;
            _logger = logger;
            businessLogic = _businessLogic;
            configurationApp = _configurationApp;
            reclasificationProcess = _reclasificationProcess;
            _logic = logic;
            loanAccountHistoryProcess = _loanAccountHistoryProcess;
            _cache = cache;
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            try
            {
                return Ok(Assembly.GetEntryAssembly().GetName().Version);
            }
            catch (Exception exc)
            {
                _logger.LogError($"GetVersion(): {exc}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = exc.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
        }
        
        [HttpPost("orchestrate")]
        public async Task<IActionResult> Orchestrate([FromBody] BackupRequestModel backupRequest)
        {
            var processId = Guid.NewGuid().ToString();

            try{
                var processItem =  await _processOrchestration.CheckProcessInProgress();

                if(processItem)
                {
                    var responseCheck = new ApiResponse<string>
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "Process running",
                        Status = 200,
                        Data = $"There is a process running!",
                        TraceId = HttpContext.TraceIdentifier
                    };

                    return StatusCode(200, responseCheck);
                }

                if (backupRequest == null)
                {
                    var errorResponse = new ApiResponse<string>
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "An error occurred",
                        Status = 400,
                        Data = "Invalid data.",
                        TraceId = HttpContext.TraceIdentifier
                    };

                    return StatusCode(400, errorResponse);
                }
                else
                {
                    _ = Task.Run(async () =>
                    {                    
                        await _processOrchestration.Process(processId);
                    });                                        
                }  


            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Process orchestration initiated successfully.",
                Status = 200,
                Data = $"Process id {processId}",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);

            }
            catch(Exception exc)
            {
                _logger.LogError($"GetVersion(): {exc}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = exc.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("statusorchestrate")]
        public async Task<IActionResult> StatusOrchestrate(String processId)
        {
            try{
                if (processId == null)
                {
                    var errorResponse = new ApiResponse<string>
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "An error occurred",
                        Status = 400,
                        Data = "Invalid data.",
                        TraceId = HttpContext.TraceIdentifier
                    };

                    return StatusCode(400, errorResponse);
                }
                else
                {
                    String result = await _processOrchestration.GetProcessStatus(processId);
                    
                    if (result != null)
                    {
                        var response = new ApiResponse<string>
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                            Title = "Status of the process orchestration.",
                            Status = 200,
                            Data = result,
                            TraceId = HttpContext.TraceIdentifier
                        };

                        return StatusCode(200, response);
                    }
                    else
                    {
                        var response = new ApiResponse<string>
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                            Title = "Status of the process orchestration.",
                            Status = 400,
                            Data = $"ProcessId {processId} doesn't exist.",
                            TraceId = HttpContext.TraceIdentifier
                        };

                        return StatusCode(400, response);                        
                    }
                }                
            }
            catch(Exception exc)
            {
                _logger.LogError($"GetVersion(): {exc}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = exc.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("LoanAccountHistory")]
        public async Task<IActionResult> LoanAccountHistory()
        {
            try
            {
                _logger.LogInformation($"Executing API BuildLoanAccountHistory....");

                await loanAccountHistoryProcess.BuildLoanAccountHistory();
            }
            catch (Exception ex)
            {
                _logger.LogError($"BuildLoanAccountHistory(): {ex}");
                
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "LoanAccountHistory.",
                Status = 200,
                Data = "OK",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);

        }        

        [HttpGet("UalaAccountingByDate")]
        public async Task<IActionResult> ExecuteApi(DateTime from, DateTime to)
        {
            try
            {
                _logger.LogInformation($"Executing API ExecuteApi....");
                _logger.LogInformation($"MVP1 v1.0.0");

                if (from == DateTime.MinValue || to == DateTime.MinValue)
                {
                    var errorMessage = "The 'from' or 'to' dates cannot be the minimum value or null.";
                    _logger.LogError(errorMessage);
                    return BadRequest(errorMessage);
                }

                await businessLogic.Process(from, to);                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Process(): {ex}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
            
            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "UalaAccountingByDate",
                Status = 200,
                Data = "OK",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);            
        }

        [HttpGet("ReclassificationLogic")]
        public async Task<IActionResult> ReclassificationLogic(DateTime from, DateTime to)
        {
            try
            {
                _logger.LogInformation($"Executing API ExecuteReclassification....");

                await reclasificationProcess.ExecuteReclasification(from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ExecuteReclasification(): {ex}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }

            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "ReclassificationLogic.",
                Status = 200,
                Data = "OK",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);
        }

        [HttpGet("GetAccountCharts")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountChartList()
        {
            try
            {
                _logger.LogInformation($"Executing API GetAccountChartList....");

                return await configurationApp.GetAccountChartNameAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError($"GetConfigTemplate(): {ex}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("ConfigTemplate")]
        public async Task<IActionResult> GetConfigTemplate(string accountChart)
        {
            try
            {
                _logger.LogInformation($"Executing API GetConfigTemplate....");

                var ms = await configurationApp.GetConfigTemplate(accountChart);

                var currentDateTime = DateTime.Now;

                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", String.Format("ConfigTable-{0}.xlsx", currentDateTime));

            }
            catch (Exception ex)
            {
                _logger.LogError($"GetConfigTemplate(): {ex}");

                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("ConfigTemplate")]
        public async Task<IActionResult> CreateConfigTemplate(IFormFile file)
        {
            try
            {
                _logger.LogInformation($"Executing API CreateConfigTemplate....");

                if (file == null || file.Length == 0)
                {
                    var errorResponse = new ApiResponse<string>
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "An error occurred",
                        Status = 400,
                        Data = "Please upload a valid file.",
                        TraceId = HttpContext.TraceIdentifier
                    };

                    return StatusCode(400, errorResponse);
                }

                await configurationApp.CreateConfigTemplate(file);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateConfigTemplate(): {ex}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }

            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Backup data process successfully.",
                Status = 200,
                Data = "CREATED",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);
        }

        [HttpPut("ConfigTemplate")]
        public async Task<IActionResult> UpdateConfigTemplate(IFormFile file)
        {
            try
            {
                _logger.LogInformation($"Executing API UpdateConfigTemplate....");

                if (file == null || file.Length == 0)
                {
                    var errorResponse = new ApiResponse<string>
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "An error occurred",
                        Status = 400,
                        Data = "Please upload a valid file.",
                        TraceId = HttpContext.TraceIdentifier
                    };

                    return StatusCode(400, errorResponse);
                }

                await configurationApp.UpdateConfigTemplate(file);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateConfigTemplate(): {ex}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = ex.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }

            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Backup data process successfully.",
                Status = 200,
                Data = "UPDATE",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);
        }

        [HttpPost("triggerBackup")]
        public async Task<IActionResult> RunImportAsync()
        {
            try
            {
                _logger.LogInformation($"Executing API BATCH-IMPORT....");
                _logger.LogInformation($"MVP1 v1.0.0");


                await _logic.RunImportAsync();
            }
            catch (Exception exc)
            {
                _logger.LogError($"RunImportAsync(): {exc}");
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = exc.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }

            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Backup data process successfully.",
                Status = 200,
                Data = "QUEUE",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);
        }

        [HttpPost("downloadBackup")]
        public async Task<IActionResult> ReceiveBackup([FromBody] BackupRequestModel backupRequest)
        {
            
            try
            {
                if (backupRequest == null)
                {
                    var errorResponse = new ApiResponse<string>
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "An error occurred",
                        Status = 400,
                        Data = "Invalid data.",
                        TraceId = HttpContext.TraceIdentifier
                    };

                    return StatusCode(400, errorResponse);
                }
                else
                {
                    await _logic.GetAndProcessBackUp();
                }
            }
            catch(Exception exc)
            {
                _logger.LogError($"downloadBackup(): {exc}");
        
                var errorResponse = new ApiResponse<string>
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "An error occurred",
                    Status = 500,
                    Data = exc.Message,
                    TraceId = HttpContext.TraceIdentifier
                };

                return StatusCode(500, errorResponse);
            }

            var response = new ApiResponse<string>
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Backup data process successfully.",
                Status = 200,
                Data = backupRequest.TenantId + " Backup Finish",
                TraceId = HttpContext.TraceIdentifier
            };

            return StatusCode(200, response);
        }
    }
}