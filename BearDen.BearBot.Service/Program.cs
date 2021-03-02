// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace BearDen.BearBot.Service
{
    class Program
    {
        /// <summary>
        /// Primary entry point into the BearBot service application
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task Main(string[] args)
            => Service.RunAsync(args);
    }
}
