using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        private const string ExtraCredentialKeyword = "ExtraCredentials";
        private const string StreamingKeyword = "Streaming";
        private const string TrinoKeyword = "Trino";
        private const string PasswordKeyword = "Password";
        private const string SslKeyword = "Ssl";

        private static readonly IReadOnlyList<string> _validKeywords;
        private static readonly IReadOnlyDictionary<string, Keywords> _keywords;

        private string _dataSource = string.Empty;
        private string _user = defaultUser;
        private string _catalog = string.Empty;
        private string _schema = string.Empty;
        private bool _streaming = true;
        private bool _trino = false;
        private string _password = string.Empty;
        private bool? _ssl = null;
        private ImmutableList<KeyValuePair<string, string>> _extraCredentials = ImmutableList.Create<KeyValuePair<string, string>>();

        private enum Keywords
        {
            DataSource,
            User,
            Catalog,
            Schema,
            ExtraCredentials,
            Streaming,
            Trino,
            Password,
            Ssl,
        }

        static PrestoConnectionStringBuilder()
        {
            var validKeywords = new string[9];
            validKeywords[(int)Keywords.DataSource] = DataSourceKeyword;
            validKeywords[(int)Keywords.User] = UserKeyword;
            validKeywords[(int)Keywords.Catalog] = CatalogKeyword;
            validKeywords[(int)Keywords.Schema] = SchemaKeyword;
            validKeywords[(int)Keywords.ExtraCredentials] = ExtraCredentialKeyword;
            validKeywords[(int)Keywords.Streaming] = StreamingKeyword;
            validKeywords[(int)Keywords.Trino] = TrinoKeyword;
            validKeywords[(int)Keywords.Password] = PasswordKeyword;
            validKeywords[(int)Keywords.Ssl] = SslKeyword;
            _validKeywords = validKeywords;

            _keywords = new Dictionary<string, Keywords>(10, StringComparer.OrdinalIgnoreCase)
            {
                [DataSourceKeyword] = Keywords.DataSource,
                [DataSourceNoSpaceKeyword] = Keywords.DataSource,
                [UserKeyword] = Keywords.User,
                [CatalogKeyword] = Keywords.Catalog,
                [SchemaKeyword] = Keywords.Schema,
                [ExtraCredentialKeyword] = Keywords.ExtraCredentials,
                [StreamingKeyword] = Keywords.Streaming,
                [TrinoKeyword] = Keywords.Trino,
                [PasswordKeyword] = Keywords.Password,
                [SslKeyword] = Keywords.Ssl
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

        public virtual ImmutableList<KeyValuePair<string, string>> ExtraCredentials
        {
            get => _extraCredentials;
            set
            {
                _extraCredentials = value;
                base[ExtraCredentialKeyword] = WriteExtraCredentials();
            }
        }

        public virtual bool Streaming
        {
            get => _streaming;
            set => base[StreamingKeyword] = _streaming = value;
        }

        public virtual bool Trino
        {
            get => _trino;
            set => base[TrinoKeyword] = _trino = value;
        }

        public virtual bool? Ssl
        {
            get => _ssl;
            set => base[SslKeyword] = _ssl = value;
        }

        public virtual string Password
        {
            get => _password;
            set => base[PasswordKeyword] = _password = value;
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
                    case Keywords.ExtraCredentials:
                        ParseExtraCredentials(Convert.ToString(value, CultureInfo.InvariantCulture));
                        return;
                    case Keywords.Streaming:
                        Streaming = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Trino:
                        Trino = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Password:
                        Password = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Ssl:
                        Ssl = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                        return;
                    default:
                        Debug.Assert(false, "Unexpected keyword: " + keyword);
                        return;
                }
            }
        }

        private void ParseExtraCredentials(string extraCredentials)
        {
            var extraCredentialSplit = extraCredentials.Split(',');

            foreach (var extraCredential in extraCredentialSplit)
            {
                ParseExtraCredential(extraCredential);
            }
            base[ExtraCredentialKeyword] = WriteExtraCredentials();
        }

        private void ParseExtraCredential(string extraCredential)
        {
            var seperatorIndex = extraCredential.IndexOf(':');

            if (seperatorIndex < 0)
            {
                throw new Exception("Missing key value seperator in extra credentials");
            }

            var key = extraCredential.Substring(0, seperatorIndex);
            var value = extraCredential.Substring(seperatorIndex + 1, extraCredential.Length - seperatorIndex - 1);
            _extraCredentials = _extraCredentials.Add(new KeyValuePair<string, string>(key, value));
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
                case Keywords.ExtraCredentials:
                    return WriteExtraCredentials();
                case Keywords.Streaming:
                    return Streaming;
                case Keywords.Trino:
                    return Trino;
                case Keywords.Password:
                    return Password;
                case Keywords.Ssl:
                    return Ssl;
                default:
                    Debug.Assert(false, "Unexpected keyword: " + index);
                    return null;
            }
        }

        private string WriteExtraCredentials()
        {
            return string.Join(",", _extraCredentials.Select(x => $"{x.Key}:{x.Value}"));
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
                case Keywords.ExtraCredentials:
                    _extraCredentials.Clear();
                    return;
                case Keywords.Streaming:
                    _streaming = true;
                    return;
                case Keywords.Trino:
                    _trino = false;
                    return;
                case Keywords.Password:
                    _password = string.Empty;
                    return;
                case Keywords.Ssl:
                    _ssl = null;
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
