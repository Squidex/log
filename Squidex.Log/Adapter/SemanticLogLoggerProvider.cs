// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Squidex.Log.Adapter
{
    public class SemanticLogLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider services;
        private ISemanticLog? log;

        public SemanticLogLoggerProvider(IServiceProvider services)
        {
            Guard.NotNull(services, nameof(services));

            this.services = services;
        }

        internal SemanticLogLoggerProvider(ISemanticLog? log)
        {
            this.log = log;
        }

        public static SemanticLogLoggerProvider ForTesting(ISemanticLog? log)
        {
            return new SemanticLogLoggerProvider(log);
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (log == null && services != null)
            {
                log = services.GetService(typeof(ISemanticLog)) as ISemanticLog;
            }

            if (log == null)
            {
                return NullLogger.Instance;
            }

            return new SemanticLogLogger(log.CreateScope(writer =>
            {
                writer.WriteProperty("category", categoryName);
            }));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
