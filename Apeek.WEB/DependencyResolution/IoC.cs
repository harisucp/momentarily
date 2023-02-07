// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Core.IocRegistry;
using StructureMap.Configuration.DSL;

namespace Apeek.Web.DependencyResolution {
    using StructureMap;
	
    public static class IoC {
        public static IContainer Initialize()
        {
            var container = Ioc.Instance;
            container.Configure(c =>
            {
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry(new ApeekSingletonRegistry());
                c.AddRegistry(new IocScanerRegistry(AppDomain.CurrentDomain.BaseDirectory + AppSettings.GetInstance().IocScanDirectory));
            });

            container.Configure(x=> x.AddRegistry(new CoreIocRegistry()));
            return container;
        }
    }
}