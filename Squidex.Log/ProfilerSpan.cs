// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

#pragma warning disable IDE0044 // Add readonly modifier

namespace Squidex.Log
{
    public sealed class ProfilerSpan : IDisposable
    {
        private readonly ProfilerSession session;
        private readonly string key;
        private ValueStopwatch watch = ValueStopwatch.StartNew();
        private List<IDisposable>? hooks;

        public string Key
        {
            get { return key; }
        }

        public ProfilerSpan(ProfilerSession session, string key)
        {
            this.session = session;

            this.key = key;
        }

        public void Listen(IDisposable hook)
        {
            Guard.NotNull(hook, nameof(hook));

            hooks ??= new List<IDisposable>(1);
            hooks.Add(hook);
        }

        public void Dispose()
        {
            var elapsedMs = watch.Stop();

            session.Measured(key, elapsedMs);

            if (hooks != null)
            {
                for (var i = 0; i < hooks.Count; i++)
                {
                    try
                    {
                        hooks[i].Dispose();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
    }
}
