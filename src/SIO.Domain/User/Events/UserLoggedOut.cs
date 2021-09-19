﻿using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.User.Events
{
    [MessagePackObject]
    public class UserLoggedOut : Event
    {
        public UserLoggedOut(string subject, int version) : base(subject, version)
        {
        }
    }
}