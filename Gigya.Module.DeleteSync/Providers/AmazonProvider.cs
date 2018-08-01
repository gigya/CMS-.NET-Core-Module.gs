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
        private readonly IAmazonS3 _client;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _prefix;

        public AmazonProvider(string accessKey, string secretKey, string bucketName, string prefix, Logger logger)
        {
            _client = new AmazonS3Client(accessKey, secretKey);
            _logger = logger;
            _accessKey = accessKey;
            _secretKey = secretKey;
            _bucketName = bucketName;
            _prefix = prefix;
        }

        public async Task<List<DeleteSyncFile>> GetUids(Dictionary<string, DeleteSyncLog> processedFiles)
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
                    var response = await _client.ListObjectsAsync(request);
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

        private bool IsFileRequired(string key, Dictionary<string, DeleteSyncLog> processedFiles)
        {
            DeleteSyncLog log;
            if (!processedFiles.TryGetValue(key, out log))
            {
                return true;
            }

            // only need to retry if 100% failure
            return log.Errors == log.Total;
        }

        private async Task<DeleteSyncFile> ReadObjectDataAsync(string key)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                using (GetObjectResponse response = await _client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    var file = new DeleteSyncFile
                    {
                        Key = key,
                        Name = response.Metadata["x-amz-meta-title"]
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

                    file.UIDs.AddRange(lines.Skip(1));
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
