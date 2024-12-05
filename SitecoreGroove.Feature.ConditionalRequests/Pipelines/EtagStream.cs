using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SitecoreGroove.Feature.ConditionalRequests.Pipelines
{
    public class EtagStream : Stream
    {
        private const string EtagNoneMatchHeaderKey = "If-None-Match";
        private const string ContentSecurityPolicyHeaderKey = "Content-Security-Policy";
        private const string EtagHeaderKey = "Etag";

        private Stream _responseStream;
        private MemoryStream _recordStream;
        private HttpContextBase _httpContext;

        public EtagStream(HttpContextBase httpContext)
        {
            _responseStream = httpContext.Response.Filter;
            _recordStream = new MemoryStream();
            _httpContext = httpContext;
        }

        public override bool CanRead
        {
            get
            {
                return _responseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _responseStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _responseStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _responseStream.Length;
            }
        }

        public override long Position { get; set; }

        public override void Flush()
        {
            _responseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _responseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _responseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _responseStream.SetLength(value);
        }

        public override void Close()
        {
            ProcessConditionalRequest();

            _recordStream.Dispose();
            _responseStream.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _responseStream.Write(buffer, offset, count);
            _recordStream.Write(buffer, offset, count);
        }

        private void ProcessConditionalRequest()
        {
            string noneMatchHeaderValue = _httpContext.Request.Headers[EtagNoneMatchHeaderKey];

            string hash = string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] recordStreamBuffer = _recordStream.GetBuffer();

                string contentSecurityPolicyHeaderValue = _httpContext.Response.Headers[ContentSecurityPolicyHeaderKey];

                if (!string.IsNullOrEmpty(contentSecurityPolicyHeaderValue))
                {
                    recordStreamBuffer = recordStreamBuffer.Concat(Encoding.ASCII.GetBytes(contentSecurityPolicyHeaderValue)).ToArray();
                }

                hash = $"\"{Convert.ToBase64String(md5.ComputeHash(recordStreamBuffer))}\"";
            }

            if (!string.IsNullOrEmpty(noneMatchHeaderValue) && noneMatchHeaderValue == hash)
            {
                HttpContext.Current.Response.StatusCode = 304;
                HttpContext.Current.Response.SuppressContent = true;
            }

            HttpContext.Current.Response.Headers[EtagHeaderKey] = hash;
        }
    }
}
