using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Web;

namespace Capsule.Service.ConsoleHost
{
    internal class CapsuleDispatch
    {
        private const string _uploadActionName = "income";
        private const string _downloadActionName = "open";
        private const string _downloadQueryName = "m";
        private const string _lowQ = "_lq";

        private long _seedKey;
        private int _seed;

        private readonly static string _jsonContentTypeProduce = "application/json";
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private readonly static int _imageFileSignatureMaxCount = 10;

        private readonly IEnumerable<ICapsuleInstance> _capsuleInstances;
        private readonly CapsuleOptions _capsuleOptions;
        private readonly ILogger _logger;
        private IDictionary<string, ICapsuleInstance> _capsuleInstancesKV;
        private IDictionary<string, List<byte[]>> _imageFileSignatures;


        public CapsuleDispatch(IOptions<CapsuleOptions> capsuleOptions, IEnumerable<ICapsuleInstance> capsuleInstances, ILogger<CapsuleDispatch> logger)
        {
            _seedKey = _seed = 0;
            _logger = logger;
            _capsuleOptions = capsuleOptions.Value;
            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            if (string.IsNullOrWhiteSpace(_capsuleOptions.StoragePath))
            {
                _capsuleOptions.StoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _uploadActionName);
            }
            _capsuleInstances = capsuleInstances;
            Initialization();
        }

        public Task Doing(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var path = request.Path.ToString();
            var pathInfos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathInfos.Length == 0)
            {
                return Response404(httpContext);
            }
            return pathInfos[0].ToLower() switch
            {
                _uploadActionName => Income(httpContext),
                _downloadActionName => Open(httpContext, pathInfos),
                _ => Response404(httpContext),
            };
        }

        private void Initialization()
        {
            _capsuleInstancesKV = new Dictionary<string, ICapsuleInstance>();
            _imageFileSignatures = new Dictionary<string, List<byte[]>>();

            foreach (var capsuleInstance in _capsuleInstances)
            {
                _capsuleInstancesKV.Add(capsuleInstance.Extension, capsuleInstance);
                _imageFileSignatures.Add(capsuleInstance.Extension, capsuleInstance.HeadBytes);
            }
        }

        private int TenPowInt(int y)
        {
            return (int)Math.Pow(10, y);
        }

        private long TenPowLong(int y)
        {
            return (long)Math.Pow(10, y);
        }

        private bool Validation(IFormFile formFile, IDictionary<string, List<byte[]>> signatures, out string[] fileInfo)
        {
            fileInfo = formFile.FileName.Split('.', StringSplitOptions.RemoveEmptyEntries);
            fileInfo[1] = "." + fileInfo[1];
            if (fileInfo.Length < 2)
            {
                return false;
            }
            var fileExtension = fileInfo[1];
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                return false;
            }
            fileExtension = fileExtension.ToLowerInvariant();
            using var reader = new BinaryReader(formFile.OpenReadStream());
            var headerBytes = reader.ReadBytes(_imageFileSignatureMaxCount);
            if (signatures.TryGetValue(fileExtension, out var extensionSignature))
            {
                foreach (var signature in extensionSignature)
                {
                    if (headerBytes.Take(signature.Length).SequenceEqual(signature))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private CapsuleInfo BuildCapsuleInfo(IFormFile formFile, string[] fileInfo)
        {
            var timestamp = DateTime.Now;
            long year = timestamp.Year;
            long key = year * TenPowInt(8) + DateTime.Now.DayOfYear * TenPowInt(5) + DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;

            var seed = 0;
            lock (_capsuleInstances)
            {
                if (_seedKey == key)
                {
                    _seed++;
                }
                else
                {
                    _seedKey = key;
                    _seed = 1;
                }
                seed = _seed;
            }
            if (_capsuleInstancesKV.TryGetValue(fileInfo[1], out var capsuleInstance))
            {
                var capsuleInfo = capsuleInstance.CreateInfo();
                capsuleInfo.RealName = fileInfo[0];
                capsuleInfo.Name = $"{key}{capsuleInfo.FileTypeValue}{_seed.ToString().PadLeft(3, '0')}";
                capsuleInfo.FileName = $"{capsuleInfo.RealName}_{capsuleInfo.Name}{capsuleInfo.Extension}";
                capsuleInfo.RelativeDirectory = Path.Combine(timestamp.Year.ToString(), timestamp.Month.ToString(), timestamp.Day.ToString());
                capsuleInfo.RelativePath = $"{capsuleInfo.RelativeDirectory}//{capsuleInfo.FileName}";
                capsuleInfo.AbsolutelyDirectory = Path.Combine(_capsuleOptions.StoragePath, capsuleInfo.RelativeDirectory);
                capsuleInfo.AbsolutelyPath = Path.Combine(capsuleInfo.AbsolutelyDirectory, capsuleInfo.FileName);
                return capsuleInfo;
            }
            throw new Exception("不支持的文件格式");

        }

        private bool TryAnalysisCapsuleInfo(string capsuleName, out CapsuleInfo capsuleInfo)
        {
            capsuleName = HttpUtility.UrlDecode(capsuleName);
            var indexOfPoint = capsuleName.LastIndexOf(".");
            if (indexOfPoint > 0)
            {
                capsuleName = capsuleName.Substring(0, indexOfPoint);
            }
            var indexOfCapsuleInfo = capsuleName.LastIndexOf('_');
            var realName = capsuleName;
            if (indexOfCapsuleInfo >= 0)
            {
                realName = capsuleName.Substring(0, indexOfCapsuleInfo);
                capsuleName = capsuleName.Substring(indexOfCapsuleInfo + 1);
            }
            capsuleInfo = null;
            try
            {
                if (capsuleName.Length != 16 && capsuleName.Length != 17)
                {
                    return false;
                }
                if (!long.TryParse(capsuleName, out var capsuleNameValue))
                {
                    return false;
                }
                var year = capsuleName[..4];
                var dayOfYear = capsuleName[4..7];
                var dayOfSeconds = capsuleName[7..12];
                var valueTypeLength = 13 + capsuleName.Length - 16;
                var fileTypeValue = capsuleName[12..valueTypeLength];
                var value = int.Parse(fileTypeValue);
                if (Enum.IsDefined(typeof(FileTypes), value))
                {
                    FileTypes fileType = (FileTypes)value;
                    capsuleInfo = _capsuleInstancesKV[$"." + fileType.ToString().ToLower()].CreateInfo();

                    var timestamp = new DateTime(int.Parse(year), 1, 1).AddDays(int.Parse(dayOfYear) - 1);
                    capsuleInfo.RealName = realName;
                    capsuleInfo.Name = capsuleName;
                    capsuleInfo.FileTypeValue = int.Parse(fileTypeValue);
                    if (capsuleInfo.RealName == capsuleInfo.Name)
                    {
                        capsuleInfo.FileName = capsuleInfo.RealName + capsuleInfo.Extension;
                    }
                    else
                    {
                        capsuleInfo.FileName = $"{capsuleInfo.RealName}_{capsuleInfo.Name}{capsuleInfo.Extension}";
                    }
                    capsuleInfo.RelativeDirectory = Path.Combine(year, timestamp.Month.ToString(), timestamp.Day.ToString());
                    capsuleInfo.RelativePath = $"{capsuleInfo.RelativeDirectory}//{capsuleInfo.FileName}";
                    capsuleInfo.AbsolutelyDirectory = Path.Combine(_capsuleOptions.StoragePath, capsuleInfo.RelativeDirectory);
                    capsuleInfo.AbsolutelyPath = Path.Combine(capsuleInfo.AbsolutelyDirectory, capsuleInfo.FileName);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private async Task<string> SaveCapsule(IFormFile formFile, string[] fileInfo)
        {
            var capsuleInfo = BuildCapsuleInfo(formFile, fileInfo);
            if (!Directory.Exists(capsuleInfo.AbsolutelyDirectory))
            {
                Directory.CreateDirectory(capsuleInfo.AbsolutelyDirectory);
            }
            using (var fileStream = File.Create(capsuleInfo.AbsolutelyPath))
            {
                await formFile.CopyToAsync(fileStream);
            }

            if (_capsuleOptions.Quality < 100 && capsuleInfo.FileGroup == FileGroups.Image)
            {
                using var image = new MagickImage(capsuleInfo.AbsolutelyPath);
                image.Format = capsuleInfo.MagickFormat;
                //var rate = _capsuleOptions.Quality / 100f;
                //var width = (int)(image.Width * rate);
                //var hight = (int)(image.Width * rate);
                //image.Resize(width, hight);
                image.Quality = _capsuleOptions.Quality;
                await image.WriteAsync(Path.Combine(capsuleInfo.AbsolutelyDirectory, $"{capsuleInfo.Name}{_lowQ}{capsuleInfo.Extension}"));
            }
            return capsuleInfo.ToSaveName();
        }


        private Task Response404(HttpContext httpContext, string message = "请求地址异常，无法正常解析")
        {
            return ResponeError(httpContext, message, HttpStatusCode.NotFound);
        }

        private Task ResponeError(HttpContext httpContext, string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return ResponseJson(httpContext, new { message }, httpStatusCode);
        }

        private Task ResponseJson(HttpContext httpContext, object data, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            httpContext.Response.StatusCode = (int)httpStatusCode;
            httpContext.Response.Headers.Clear();
            httpContext.Response.Headers[HeaderNames.ContentType] = _jsonContentTypeProduce;
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "content-type,x-requested-with,authorization,token");
            return httpContext.Response.WriteAsJsonAsync(data, typeof(object), httpContext.RequestAborted);
        }

        private void SetContentType(HttpContext httpContext, CapsuleInfo capsuleInfo)
        {
            if (!_fileExtensionContentTypeProvider.TryGetContentType(capsuleInfo.Extension, out var contentType))
            {
                contentType = capsuleInfo.ContenType;
            }
            httpContext.Response.ContentType = contentType;
        }

        private void SetContentDispositionHeader(HttpContext httpContext, CapsuleInfo capsuleInfo)
        {
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(capsuleInfo.RealName + capsuleInfo.Extension);
            httpContext.Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
        }

        private async Task Income(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;
            if (request.Form != null)
            {
                if (request.Form.Files.Count > 0)
                {
                    var data = new List<string>();
                    foreach (var file in request.Form.Files)
                    {
                        if (Validation(file, _imageFileSignatures, out var fileInfo))
                        {
                            data.Add(await SaveCapsule(file, fileInfo));
                        }
                        else
                        {
                            await ResponeError(httpContext, $"文件{file.Name}验证不通过", HttpStatusCode.Forbidden);
                            return;
                        }
                    }
                    await ResponseJson(httpContext, new { imgs = data });
                }
                else
                {
                    await ResponeError(httpContext, "未解析到form文件", HttpStatusCode.Forbidden);
                }
            }
            else
            {
                await ResponeError(httpContext, "未解析到form文件", HttpStatusCode.Forbidden);
            }
        }

        private async Task Open(HttpContext httpContext, string[] pathInfos)
        {
            if (pathInfos.Length < 2)
            {
                await Response404(httpContext);
                return;
            }
            if (!TryAnalysisCapsuleInfo(pathInfos[1], out var capsuleInfo))
            {
                await ResponeError(httpContext, "文件名解析失败");
                return;
            }
            if (!File.Exists(capsuleInfo.AbsolutelyPath))
            {
                await Response404(httpContext, $"文件{capsuleInfo.FileName}不存在");
                return;
            }
            using var fileStream = new FileStream(capsuleInfo.AbsolutelyPath, FileMode.Open);
            var outputStream = httpContext.Response.Body;
            using (fileStream)
            {
                try
                {
                    SetContentType(httpContext, capsuleInfo);
                    if (pathInfos.Length >= 3 && pathInfos[2] == "download")
                    {
                        SetContentDispositionHeader(httpContext, capsuleInfo);
                    }

                    httpContext.Response.ContentLength = fileStream.Length;
                    await StreamCopyOperation.CopyToAsync(fileStream, outputStream, count: null, bufferSize: (int)fileStream.Length, cancel: httpContext.RequestAborted);
                }
                catch (OperationCanceledException)
                {
                    httpContext.Abort();
                }
            }
        }
    }
}

