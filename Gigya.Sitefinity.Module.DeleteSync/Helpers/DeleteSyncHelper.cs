using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync.Events;
using Gigya.Module.DeleteSync.Helpers;
using Gigya.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Data;
using Gigya.Sitefinity.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Helpers
{
    public class DeleteSyncHelper
    {
        private readonly Logger _logger = LoggerFactory.Instance();
        private readonly UserManager _userManager = UserManager.GetManager();
        private readonly UserProfileManager _profileManager = UserProfileManager.GetManager();
        private readonly EmailHelper _emailHelper;
        private DeleteSyncEmailModel _emailModel;

        public DeleteSyncHelper() : this (new EmailHelper(new SitefinityEmailProvider()))
        {
        }

        public DeleteSyncHelper(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public Dictionary<string, DeleteSyncLog> GetProcessedFiles()
        {
            using (var context = GigyaDeleteSyncContext.Get())
            {
                var logs = context.Logs.ToDictionary(i => i.Key, Mapper.Map);
                return logs;
            }
        }

        public void Process(SitefinityDeleteSyncSettings settings, List<DeleteSyncFile> files)
        {
            _logger.DebugFormat("Found {0} files to delete.", files.Count);

            _emailModel = new DeleteSyncEmailModel
            {
                DateStarted = DateTime.UtcNow,
                Domain = HttpContext.Current?.Request?.Url?.Host ?? "localhost",
                Subject = "User deletion job completed",
                ProcessedFilenames = files.Select(i => i.Key).ToList()
            };

            using (var context = GigyaDeleteSyncContext.Get())
            {
                foreach (var file in files)
                {
                    for (int i = 0; i < settings.MaxAttempts; i++)
                    {
                        var log = ProcessUids(settings, file);
                        if (log.Errors == log.Total && i < (settings.MaxAttempts - 1))
                        {
                            // complete failure so try again
                            continue;
                        }

                        context.Add(log);
                        break;
                    }
                    
                    context.SaveChanges();
                }
            }

            if (_emailModel.FailedDeletedUids.Any() || _emailModel.FailedUpdatedUids.Any())
            {
                _emailModel.To = settings.EmailsOnFailure;
            }
            else
            {
                _emailModel.To = settings.EmailsOnSuccess;
            }
            _emailModel.DateCompleted = DateTime.UtcNow;
            _emailHelper.SendConfirmation(_emailModel);
        }

        private SitefinityDeleteSyncLog ProcessUids(SitefinityDeleteSyncSettings settings, DeleteSyncFile file)
        {
            var log = new SitefinityDeleteSyncLog
            {
                DateCreated = DateTime.UtcNow,
                Key = file.Key,
                Total = file.UIDs.Count
            };

            foreach (var id in file.UIDs)
            {
                var success = false;

                try
                {
                    success = DeleteOrUpdateUser(id, settings.Action);
                    if (success)
                    {
                        log.Success++;
                    }
                    else
                    {
                        log.Errors++;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("Error occurred deleting users.", e);
                    log.Errors++;
                }
                finally
                {
                    AddLogEntry(success, id, settings.Action);
                }
            }

            return log;
        }

        private void AddLogEntry(bool success, string uid, DeleteSyncAction action)
        {
            switch (action)
            {
                case DeleteSyncAction.FullUserDeletion:
                    if (success)
                    {
                        _emailModel.DeletedUids.Add(uid);
                    }
                    else
                    {
                        _emailModel.FailedDeletedUids.Add(uid);
                    }
                    return;
                case DeleteSyncAction.DeleteNotification:
                    if (success)
                    {
                        _emailModel.UpdatedUids.Add(uid);
                    }
                    else
                    {
                        _emailModel.FailedUpdatedUids.Add(uid);
                    }
                    return;
                default:
                    _logger.Error($"Action: {action} not supported.");
                    return;
            }
        }

        private bool DeleteOrUpdateUser(string uid, DeleteSyncAction action)
        {
            var user = _userManager.GetUser(uid);
            if (user == null)
            {
                _logger.Error(string.Format("User with UID of {0} doesn't exist.", uid));
                return false;
            }

            var eventArgs = new DeleteSyncEventArgs
            {
                Action = action,
                CmsUid = user.Id,
                Uid = uid,
                ContinueWithDeletion = true
            };
            DeleteSyncEventHub.Instance.RaiseDeletingUser(this, eventArgs);

            if (!eventArgs.ContinueWithDeletion)
            {
                _logger.DebugFormat("Deletion not required for user with UID of {0}.", uid);
                return false;
            }

            var success = false;
            switch (action)
            {
                case DeleteSyncAction.FullUserDeletion:
                    success = DeleteUser(user);
                    break;
                case DeleteSyncAction.DeleteNotification:
                    success = MarkUserAsDeleted(user);
                    break;
                default:
                    throw new ArgumentException(string.Format("Action: {0} not supported.", action));
            }

            if (success)
            {
                var deletedUserEventArgs = new DeleteSyncUserDeletedEventArgs
                {
                    Action = action,
                    CmsUid = user.Id,
                    Uid = uid
                };
                DeleteSyncEventHub.Instance.RaiseDeletedUser(this, deletedUserEventArgs);
            }
            return success;
        }

        private bool MarkUserAsDeleted(User user)
        {            
            SitefinityProfile userProfile = _profileManager.GetUserProfile<SitefinityProfile>(user);
            if (userProfile == null)
            {
                _logger.DebugFormat("No profile for user with UID of {0} so couldn't delete.", user.UserName);
                return false;
            }

            var updated = false;
            if (userProfile.DoesFieldExist(Constants.CmsFields.IsDeleted))
            {
                userProfile.SetValue(Constants.CmsFields.IsDeleted, true);
                updated = true;
            }

            if (userProfile.DoesFieldExist(Constants.CmsFields.GigyaDeletedDate))
            {
                userProfile.SetValue(Constants.CmsFields.GigyaDeletedDate, DateTime.UtcNow);
                updated = true;
            }

            if (updated)
            {
                _profileManager.SaveChanges();
                _logger.DebugFormat("User with UID of {0} has been marked as deleted.", user.UserName);
            }
            else
            {
                _logger.Error(string.Format("User profile fields for {0} and {1} don't exist. Please create these fields on the user profile in Sitefinity.", Constants.CmsFields.IsDeleted, Constants.CmsFields.GigyaDeletedDate));
            }
            return updated;
        }

        private bool DeleteUser(User user)
        {
            SitefinityProfile userProfile = _profileManager.GetUserProfile<SitefinityProfile>(user);

            if (userProfile != null)
            {
                _profileManager.Delete(userProfile);
            }

            _userManager.Delete(user);

            _profileManager.SaveChanges();
            _userManager.SaveChanges();

            _logger.DebugFormat("User with UID of {0} has been permanently deleted.", user.UserName);
            return true;
        }
    }
}
