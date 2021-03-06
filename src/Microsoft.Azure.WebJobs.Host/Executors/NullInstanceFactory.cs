﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal class NullInstanceFactory<TReflected> : IFactory<TReflected>
    {
        private static readonly NullInstanceFactory<TReflected> _instance = new NullInstanceFactory<TReflected>();

        private NullInstanceFactory()
        {
        }

        public static NullInstanceFactory<TReflected> Instance
        {
            get { return _instance; }
        }

        public TReflected Create()
        {
            return default(TReflected);
        }
    }
}
