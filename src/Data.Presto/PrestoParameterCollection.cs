using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Data.Presto
{
    public class PrestoParameterCollection : DbParameterCollection
    {
        private readonly List<PrestoParameter> _parameters = new List<PrestoParameter>();

        protected internal PrestoParameterCollection()
        {
        }

        public new virtual PrestoParameter this[int index]
        {
            get => _parameters[index];
            set
            {
                if (_parameters[index] == value)
                {
                    return;
                }

                _parameters[index] = value;
            }
        }

        public new virtual PrestoParameter this[string parameterName]
        {
            get => this[IndexOfChecked(parameterName)];
            set => this[IndexOfChecked(parameterName)] = value;
        }


        public override int Count => _parameters.Count;

        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add((PrestoParameter)value);

            return Count - 1;
        }

        public virtual PrestoParameter Add(PrestoParameter value)
        {
            _parameters.Add(value);

            return value;
        }

        public override void AddRange(Array values)
        {
            AddRange(values.Cast<PrestoParameter>());
        }

        public virtual void AddRange(IEnumerable<PrestoParameter> values)
            => _parameters.AddRange(values);

        public override void Clear()
        {
            _parameters.Clear();
        }

        public override bool Contains(object value)
            => Contains((PrestoParameter)value);

        public virtual bool Contains(PrestoParameter value)
            => _parameters.Contains(value);

        public override bool Contains(string value) => IndexOf(value) != -1;

        public override void CopyTo(Array array, int index)
            => CopyTo((PrestoParameter[])array, index);

        public virtual void CopyTo(PrestoParameter[] array, int index)
            => _parameters.CopyTo(array, index);

        public override IEnumerator GetEnumerator() => _parameters.GetEnumerator();

        public override int IndexOf(object value)
            => IndexOf((PrestoParameter)value);

        public virtual int IndexOf(PrestoParameter value)
            => _parameters.IndexOf(value);


        public override int IndexOf(string parameterName)
        {
            for (var index = 0; index < _parameters.Count; index++)
            {
                if (_parameters[index].ParameterName == parameterName)
                {
                    return index;
                }
            }

            return -1;
        }

        public override void Insert(int index, object value)
            => Insert(index, (PrestoParameter)value);

        public virtual void Insert(int index, PrestoParameter value)
            => _parameters.Insert(index, value);

        public override void Remove(object value)
            => Remove((PrestoParameter)value);

        public virtual void Remove(PrestoParameter value)
            => _parameters.Remove(value);

        public override void RemoveAt(int index)
            => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName)
            => RemoveAt(IndexOfChecked(parameterName));

        protected override DbParameter GetParameter(int index)
            => this[index];

        protected override DbParameter GetParameter(string parameterName)
            => GetParameter(IndexOfChecked(parameterName));

        protected override void SetParameter(int index, DbParameter value)
            => this[index] = (PrestoParameter)value;

        protected override void SetParameter(string parameterName, DbParameter value)
            => SetParameter(IndexOfChecked(parameterName), value);

        private int IndexOfChecked(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index == -1)
            {
                throw new IndexOutOfRangeException();
            }

            return index;
        }
    }
}
