using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace curdinStoredprocedure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : Controller
    {
        private readonly string? _connectionString;

        private readonly string _videoUploadPath;

        public VideoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            _videoUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedVideos");

            if (!Directory.Exists(_videoUploadPath))
            {
                Directory.CreateDirectory(_videoUploadPath);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest("Invalid video file.");
            }

            string base64Video;
            using (var memoryStream = new MemoryStream())
            {
                await videoFile.CopyToAsync(memoryStream);
                byte[] videoBytes = memoryStream.ToArray();
                base64Video = Convert.ToBase64String(videoBytes);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO Videos (FileName, Base64Data, ContentType, FileSize)
            VALUES (@FileName, @Base64Data, @ContentType, @FileSize)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FileName", videoFile.FileName);
                    command.Parameters.AddWithValue("@Base64Data", base64Video);
                    command.Parameters.AddWithValue("@ContentType", videoFile.ContentType);
                    command.Parameters.AddWithValue("@FileSize", videoFile.Length);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { Message = "Video uploaded and stored as Base64 successfully!" });
        }

        [HttpGet("getallplay")]
        public async Task<IActionResult> AllVideos()
        {
            var videos = new List<object>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Base64Data, ContentType, FileName FROM Videos";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string base64Data = reader.GetString(0);
                            string contentType = reader.GetString(1);
                            string fileName = reader.GetString(2);

                            videos.Add(new
                            {
                                FileName = fileName,
                                ContentType = contentType,
                                Base64Data = base64Data
                            });
                        }
                    }
                }
            }

            if (videos.Count == 0)
            {
                return NotFound("No videos found.");
            }

            return Ok(videos);
        }


        //[HttpGet("getallplay")]
        //public async Task<IActionResult> AllVideo(int page = 1, int size = 2)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        string countQuery = "SELECT COUNT(*) FROM Videos";
        //        int totalRecords;

        //        await connection.OpenAsync();

        //        using (var countCommand = new SqlCommand(countQuery, connection))
        //        {
        //            totalRecords = (int)await countCommand.ExecuteScalarAsync();
        //        }

        //        string dataQuery = "SELECT Base64Data, ContentType, FileName FROM Videos ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //        using (var command = new SqlCommand(dataQuery, connection))
        //        {
        //            command.Parameters.AddWithValue("@Offset", (page - 1) * size);
        //            command.Parameters.AddWithValue("@PageSize", size);

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                var videos = new List<object>();
        //                while (await reader.ReadAsync())
        //                {
        //                    string base64Data = reader.GetString(0);
        //                    string contentType = reader.GetString(1);
        //                    string fileName = reader.GetString(2);

        //                    videos.Add(new
        //                    {
        //                        Base64Data = base64Data,
        //                        ContentType = contentType,
        //                        FileName = fileName
        //                    });
        //                }

        //                // Return data and total count
        //                return Ok(new
        //                {
        //                    TotalRecords = totalRecords,
        //                    Videos = videos
        //                });
        //            }
        //        }
        //    }
        //}


        [HttpGet("play/{id}")]
        public async Task<IActionResult> PlayVideo(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Base64Data, ContentType,FileName FROM Videos WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string base64Data = reader.GetString(0); 
                            string contentType = reader.GetString(1);
                            string fileName = reader.GetString(2);

                            return Ok(new
                            {
                                FileName=fileName,
                                ContentType = contentType,
                                Base64Data = base64Data
                               
                            });
                        }
                    }
                }
            }

            return NotFound("Video metadata not found.");
        }


        //[HttpGet("play/{id}")]
        //public async Task<IActionResult> PlayVideo(int id)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        string query = "SELECT Base64Data, ContentType FROM Videos WHERE Id = @Id";

        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Id", id);

        //            await connection.OpenAsync();
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    string base64Data = reader.GetString(0); // Get Base64 encoded video data
        //                    string contentType = reader.GetString(1); // Get content type (e.g., video/mp4)

        //                    // Decode Base64 data into a byte array
        //                    byte[] videoBytes = Convert.FromBase64String(base64Data);

        //                    // Return the video file as a stream
        //                    var stream = new MemoryStream(videoBytes);
        //                    return File(base64Data, contentType);
        //                }
        //            }
        //        }
        //    }

        //    return NotFound("Video metadata not found.");
        //}


        //[HttpGet("stream/{id}")]
        //public IActionResult StreamVideo(int id)
        //{
        //    var video = _context.Videos.Find(id);
        //    if (video == null)
        //        return NotFound("Video not found.");

        //    var filePath = video.FilePath;
        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound("File not found on disk.");

        //    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    var fileLength = fileStream.Length;
        //    var contentType = video.ContentType;

        //    // Parse the Range header (if present)
        //    var rangeHeader = Request.Headers["Range"].ToString();
        //    if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
        //    {
        //        var range = rangeHeader.Substring("bytes=".Length).Split('-');
        //        long start = long.Parse(range[0]);
        //        long end = range.Length > 1 && !string.IsNullOrEmpty(range[1])
        //            ? long.Parse(range[1])
        //            : fileLength - 1;

        //        if (start < 0 || end >= fileLength || start > end)
        //            return BadRequest("Invalid Range");

        //        fileStream.Seek(start, SeekOrigin.Begin);
        //        var contentLength = end - start + 1;
        //        return File(new FileSegmentStream(fileStream, contentLength), contentType, enableRangeProcessing: true);
        //    }

        //    // Serve the entire file if no range request
        //    return File(fileStream, contentType, enableRangeProcessing: true);
        //}



        [HttpGet("stream/{id}")]
        public async Task<IActionResult> StreamVideo(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Base64Data, ContentType FROM Videos WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string base64Data = reader.GetString(0); 
                            string contentType = reader.GetString(1);

                            byte[] videoBytes = Convert.FromBase64String(base64Data);
                            long fileLength = videoBytes.Length;

                            var rangeHeader = Request.Headers["Range"].ToString();
                            if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
                            {
                                var range = rangeHeader.Substring("bytes=".Length).Split('-');
                                long start = long.Parse(range[0]);
                                long end = range.Length > 1 && !string.IsNullOrEmpty(range[1])
                                    ? long.Parse(range[1])
                                    : fileLength - 1;

                                if (start < 0 || end >= fileLength || start > end)
                                    return BadRequest("Invalid Range");

                                Response.Headers["Content-Range"] = $"bytes {start}-{end}/{fileLength}";
                                Response.Headers["Accept-Ranges"] = "bytes";
                                Response.StatusCode = 206; // Partial content

                                var contentLength = end - start + 1;
                                return File(new MemoryStream(videoBytes, (int)start, (int)contentLength), contentType);
                            }

                            Response.Headers["Accept-Ranges"] = "bytes";
                            return File(new MemoryStream(videoBytes), contentType);
                        }
                    }
                }
            }

            return NotFound("Video metadata not found.");
        }


        //public class FileSegmentStream : Stream
        //{
        //    private readonly Stream _baseStream;
        //    private readonly long _length;
        //    private long _position;

        //    public FileSegmentStream(Stream baseStream, long length)
        //    {
        //        _baseStream = baseStream;
        //        _length = length;
        //        _position = 0;
        //    }

        //    public override bool CanRead => _baseStream.CanRead;
        //    public override bool CanSeek => false;
        //    public override bool CanWrite => false;
        //    public override long Length => _length;
        //    public override long Position
        //    {
        //        get => _position;
        //        set => throw new NotSupportedException();
        //    }

        //    public override void Flush() => _baseStream.Flush();
        //    public override int Read(byte[] buffer, int offset, int count)
        //    {
        //        if (_position >= _length) return 0;

        //        count = (int)Math.Min(count, _length - _position);
        //        int bytesRead = _baseStream.Read(buffer, offset, count);
        //        _position += bytesRead;
        //        return bytesRead;
        //    }

        //    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        //    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        //    public override void SetLength(long value) => throw new NotSupportedException();
        //    protected override void Dispose(bool disposing)
        //    {
        //        base.Dispose(disposing);
        //        if (disposing) _baseStream.Dispose();
        //    }
        //}


    }
}
