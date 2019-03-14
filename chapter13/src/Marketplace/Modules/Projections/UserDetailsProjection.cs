using System;
using System.Threading.Tasks;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using Serilog;
using static Marketplace.Ads.Messages.UserProfile.Events;

namespace Marketplace.Modules.Projections
{
    public class UserDetailsProjection : RavenDbProjection<ReadModels.UserDetails>
    {
        private static readonly ILogger Log = 
            Serilog.Log.ForContext<ClassifiedAdDetailsProjection>();
        
        public UserDetailsProjection(Func<IAsyncDocumentSession> getSession)
            : base(getSession) { }

        public override async Task Project(object @event)
        {
            Log.Debug("Projecting {event} to UserDetails", @event);
            
            switch (@event)
            {
                case UserRegistered e:
                    await UsingSession(session =>
                        session.StoreAsync(new ReadModels.UserDetails
                        {
                            Id = e.UserId.ToString(),
                            DisplayName = e.DisplayName
                        }));
                    break;
                case UserDisplayNameUpdated e:
                    await UsingSession(session =>
                        UpdateItem(session, e.UserId, x => x.DisplayName = e.DisplayName));
                    break;
                case ProfilePhotoUploaded e:
                    await UsingSession(session =>
                        UpdateItem(session, e.UserId, x => x.PhotoUrl = e.PhotoUrl));
                    break;
            }
        }
    }
}