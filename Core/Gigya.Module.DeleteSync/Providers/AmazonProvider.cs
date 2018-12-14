using Amazon.S3;
using Amazon.S3.Model;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Providers
{
    public class AmazonProvider : IDeleteSyncProvider
    {
        private readonly Logger _logger;
        private readonly Lazy<IAmazonS3> _client;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _prefix;

        public AmazonProvider(string accessKey, string secretKey, string bucketName, string prefix, string region, Logger logger)
        {
            _logger = logger;
            _accessKey = accessKey;
            _secretKey = secretKey;
            _bucketName = bucketName;
            _prefix = prefix;

            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);
            _client = new Lazy<IAmazonS3>(() => new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.GetBySystemName(region)));
        }

        public virtual async Task<bool> IsValid()
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                Prefix = _prefix,
                BucketName = _bucketName,
                MaxKeys = 1
            };

            try
            {
                var response = await _client.Value.ListObjectsAsync(request).ConfigureAwait(false);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.Error("Amazon S3 settings are invalid.", e);
                return false;
            }
        }

        public virtual async Task<List<DeleteSyncFile>> GetUids(Dictionary<string, DeleteSyncLog> processedFiles)
        {
            var result = new List<DeleteSyncFile>();

            ListObjectsRequest request = new ListObjectsRequest
            {
                Prefix = _prefix,
                BucketName = _bucketName,
                MaxKeys = 500
            };

            try
            {
                while (true)
                {
                    var response = await _client.Value.ListObjectsAsync(request);
                    if (response == null)
                    {
                        break;
                    }

                    foreach (S3Object entry in response.S3Objects)
                    {
                        if (!IsFileRequired(entry.Key, processedFiles))
                        {
                            continue;
                        }

                        var file = await ReadObjectDataAsync(entry.Key);
                        if (file != null)
                        {
                            result.Add(file);
                        }
                    }

                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Amazon S3 error.", e);
            }

            return result;
        }

        protected virtual bool IsFileRequired(string key, Dictionary<string, DeleteSyncLog> processedFiles)
        {
            return !processedFiles.ContainsKey(key);
        }

        protected virtual async Task<DeleteSyncFile> ReadObjectDataAsync(string key)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                using (GetObjectResponse response = await _client.Value.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    var file = new DeleteSyncFile
                    {
                        Key = key
                    };

                    var responseBody = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(responseBody))
                    {
                        return null;
                    }

                    var lines = responseBody.Split('\n');
                    if (lines.Length < 2)
                    {
                        // first line is always 'UID'
                        return null;
                    }

                    file.UIDs = new List<string>(lines.Skip(1).Where(i => !string.IsNullOrEmpty(i)).Select(i => i.Replace("\r", string.Empty)));
                    return file;
                }
            }
            catch (Exception e)
            {
                _logger.Error("Amazon S3 error.", e);
            }

            return null;
        }
    }
}
