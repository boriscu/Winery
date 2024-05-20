using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Winery
{
    public class WineryContext:DbContext
    {
        public DbSet<Container> Containers { get; set; }
        public DbSet<Wine> Wines { get; set; }

        // singleton pattern with thread safety
        private static WineryContext instance;
        private static readonly object padlock = new object();
        public static WineryContext Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new WineryContext();
                    return instance;
                }
            }
        }
        private WineryContext() { }
    }
}
