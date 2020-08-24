using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Presto.Infrastructure.Internal
{
    public class PrestoOptionsExtension : RelationalOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        public PrestoOptionsExtension() { }

        public PrestoOptionsExtension(PrestoOptionsExtension cpy)
        {
            
        }

        public override DbContextOptionsExtensionInfo Info
        {
            get
            {
                if(_info == null)
                {
                    _info = new ExtensionInfo(this);
                }
                return _info;
            }
        }

        public override void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkPresto();
        }

        protected override RelationalOptionsExtension Clone()
        {
            return new PrestoOptionsExtension(this);
        }

        private sealed class ExtensionInfo : RelationalExtensionInfo
        {
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new PrestoOptionsExtension Extension
                => (PrestoOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append(base.LogFragment);

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
                => debugInfo["Presto"] = "1";
        }
    }
}
