using System;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.UserProfile;
using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.UserProfiles
{
    public class UserProfile : AggregateRoot
    {
        public static UserProfile Create(
            UserId id,
            FullName fullName,
            DisplayName displayName)
        {
            var profile = new UserProfile();

            profile.Apply(
                new Events.UserRegistered
                {
                    UserId = id,
                    FullName = fullName,
                    DisplayName = displayName
                }
            );
            return profile;
        }

        // Aggregate state properties
        public FullName FullName { get; private set; }
        public DisplayName DisplayName { get; private set; }
        public string PhotoUrl { get; private set; }

        public void UpdateFullName(FullName fullName)
            => Apply(
                new Events.UserFullNameUpdated
                {
                    UserId = Id,
                    FullName = fullName
                }
            );

        public void UpdateDisplayName(DisplayName displayName)
            => Apply(
                new Events.UserDisplayNameUpdated
                {
                    UserId = Id,
                    DisplayName = displayName
                }
            );

        public void UpdateProfilePhoto(Uri photoUrl)
            => Apply(
                new Events.ProfilePhotoUploaded
                {
                    UserId = Id,
                    PhotoUrl = photoUrl.ToString()
                }
            );

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.UserRegistered e:
                    SetId(e.UserId);
                    FullName = new FullName(e.FullName);
                    DisplayName = new DisplayName(e.DisplayName);
                    break;
                case Events.UserFullNameUpdated e:
                    FullName = new FullName(e.FullName);
                    break;
                case Events.UserDisplayNameUpdated e:
                    DisplayName = new DisplayName(e.DisplayName);
                    break;
                case Events.ProfilePhotoUploaded e:
                    PhotoUrl = e.PhotoUrl;
                    break;
            }
        }

        protected override void EnsureValidState() { }
    }
}