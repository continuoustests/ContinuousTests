using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client.Config
{
    public class ConfigItem<T>
    {
        public bool IsLocal { get; set; }
        public bool Exists { get; set; }
        public bool ShouldMerge { get; set; }
        public bool ShouldExclude { get; set; }
        public T Item { get; set; }
    }
}
