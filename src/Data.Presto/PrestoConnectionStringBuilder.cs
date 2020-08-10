using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Data.Presto
{
    public class PrestoConnectionStringBuilder : DbConnectionStringBuilder
    {
        private const string defaultUser = "root";

        private const string DataSourceKeyword = "Data Source";
        private const string DataSourceNoSpaceKeyword = "DataSource";
        private const string UserKeyword = "User";
        private const string CatalogKeyword = "Catalog";
        private const string SchemaKeyword = "Schema";

        private static readonly IReadOnlyList<string> _validKeywords;
        private static readonly IReadOnlyDictionary<string, Keywords> _keywords;

        private string _dataSource = string.Empty;
        private string _user = defaultUser;
        private string _catalog = string.Empty;
        private string _schema = string.Empty;

        private enum Keywords
        {
            DataSource,
            User,
            Catalog,
            Schema
        }

        static PrestoConnectionStringBuilder()
        {
            var validKeywords = new string[4];
            validKeywords[(int)Keywords.DataSource] = DataSourceKeyword;
            validKeywords[(int)Keywords.User] = UserKeyword;
            validKeywords[(int)Keywords.Catalog] = CatalogKeyword;
            validKeywords[(int)Keywords.Schema] = SchemaKeyword;
            _validKeywords = validKeywords;

            _keywords = new Dictionary<string, Keywords>(5, StringComparer.OrdinalIgnoreCase)
            {
                [DataSourceKeyword] = Keywords.DataSource,
                [DataSourceNoSpaceKeyword] = Keywords.DataSource,
                [UserKeyword] = Keywords.User,
                [CatalogKeyword] = Keywords.Catalog,
                [SchemaKeyword] = Keywords.Schema
            };
        }

        public PrestoConnectionStringBuilder()
        {
        }

        public PrestoConnectionStringBuilder(string connectionString)
            => ConnectionString = connectionString;

        public virtual string DataSource
        {
            get => _dataSource;
            set => base[DataSourceKeyword] = _dataSource = value;
        }

        public virtual string User
        {
            get => _user;
            set => base[UserKeyword] = _user = value;
        }

        public virtual string Catalog
        {
            get => _catalog;
            set => base[CatalogKeyword] = _catalog = value;
        }

        public virtual string Schema
        {
            get => _schema;
            set => base[SchemaKeyword] = _schema = value;
        }

        internal virtual string Host
        {
            get
            {
                return _dataSource;
            }
        }

        public override ICollection Values
        {
            get
            {
                var values = new object[_validKeywords.Count];
                for (var i = 0; i < _validKeywords.Count; i++)
                {
                    values[i] = GetAt((Keywords)i);
                }

                return new ReadOnlyCollection<object>(values);
            }
        }

        public override object this[string keyword]
        {
            get => GetAt(GetIndex(keyword));
            set
            {
                if (value == null)
                {
                    Remove(keyword);

                    return;
                }

                switch (GetIndex(keyword))
                {
                    case Keywords.DataSource:
                        DataSource = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Catalog:
                        Catalog = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Schema:
                        Schema = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.User:
                        User = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;

                    default:
                        Debug.Assert(false, "Unexpected keyword: " + keyword);
                        return;
                }
            }
        }

        public override void Clear()
        {
            base.Clear();

            for (var i = 0; i < _validKeywords.Count; i++)
            {
                Reset((Keywords)i);
            }
        }

        public override bool ContainsKey(string keyword)
            => _keywords.ContainsKey(keyword);

        public override bool ShouldSerialize(string keyword)
            => _keywords.TryGetValue(keyword, out var index) && base.ShouldSerialize(_validKeywords[(int)index]);

        public override bool TryGetValue(string keyword, out object value)
        {
            if (!_keywords.TryGetValue(keyword, out var index))
            {
                value = null;

                return false;
            }

            value = GetAt(index);

            return true;
        }

        public override bool Remove(string keyword)
        {
            if (!_keywords.TryGetValue(keyword, out var index)
                || !base.Remove(_validKeywords[(int)index]))
            {
                return false;
            }

            Reset(index);

            return true;
        }

        private object GetAt(Keywords index)
        {
            switch (index)
            {
                case Keywords.DataSource:
                    return DataSource;
                case Keywords.Catalog:
                    return Catalog;
                case Keywords.Schema:
                    return Schema;
                case Keywords.User:
                    return User;

                default:
                    Debug.Assert(false, "Unexpected keyword: " + index);
                    return null;
            }
        }

        private void Reset(Keywords index)
        {
            switch (index)
            {
                case Keywords.DataSource:
                    _dataSource = string.Empty;
                    return;
                case Keywords.Catalog:
                    _catalog = string.Empty;
                    return;
                case Keywords.Schema:
                    _schema = string.Empty;
                    return;
                case Keywords.User:
                    _user = defaultUser;
                    return;

                default:
                    Debug.Assert(false, "Unexpected keyword: " + index);
                    return;
            }
        }

        private static Keywords GetIndex(string keyword)
            => !_keywords.TryGetValue(keyword, out var index)
                ? throw new ArgumentException(nameof(keyword))
                : index;
    }
}
