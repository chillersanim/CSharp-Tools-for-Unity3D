using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace UnityTools.Collections
{
    public class Table
    {
        [NotNull]
        [ItemNotNull]
        private readonly List<IColumn> columns;

        public IReadOnlyList<IColumn> Columns { get; }

        public int ColumnCount => this.columns.Count;

        public Table(int columnAmount)
        {
            this.columns = new List<IColumn>(columnAmount);
            this.Columns = this.columns.AsReadOnly();

            for (var i = 0; i < columnAmount; i++)
            {
                this.columns.Add(new Column<object>(this, $"Column {i}"));
            }
        }

        public Table([NotNull]params string[] headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            this.columns = new List<IColumn>(headers.Length);
            this.Columns = this.columns.AsReadOnly();

            foreach (var name in headers)
            {
                this.columns.Add(new Column<object>(this, name));
            }
        }

        public void AddColumn(string header)
        {
            this.columns.Add(new Column<object>(this, header));
        }

        public void AddColumn<T>(string header)
        {
            this.columns.Add(new Column<T>(this, header));
        }

        public IColumn GetColumn(int index)
        {
            return this.columns[index];
        }

        public IColumn<T> GetColumn<T>(int index)
        {
            var column = this.columns[index];

            if (column.ContentType == typeof(T))
            {
                return (IColumn<T>) column;
            }

            throw new NotImplementedException();
        }

        public void RemoveColumn(int index)
        {
            var column = (TableChildBase)this.columns[index];
            column.Parent = null;

            this.columns.RemoveAt(index);
        }

        public bool RemoveColumn(string header, StringComparison comparison = StringComparison.CurrentCulture)
        {
            for (var i = 0; i < this.columns.Count; i++)
            {
                if (string.Equals(header, this.columns[i].Header, comparison))
                {
                    this.RemoveColumn(i);
                    return true;
                }
            }

            return false;
        }

        private class TableChildBase
        {
            public Table Parent { get; set; }

            public TableChildBase([NotNull]Table parent)
            {
                this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            }
        }

        private class Column<T> : TableChildBase, IColumn<T>
        {
            private string header;

            public string Header
            {
                get => this.header;
                set => this.header = value ?? string.Empty;
            }

            public Type ContentType { get; }

            public T this[int row]
            {
                get => this.Rows[row];
                set => this.Rows[row] = value;
            }

            /// <inheritdoc />
            object IColumn.this[int row]
            {
                get => this.Rows[row];
                set
                {
                    if (value is T tVal)
                    {
                        this.Rows[row] = tVal;
                    }

                    throw new ArgumentException("The provided value is incompatible with the row type.");
                }
            }

            [NotNull]
            public readonly List<T> Rows;

            public Column([NotNull] Table parent, string header) : base(parent)
            {
                this.Header = header;
                this.ContentType = typeof(T);
                this.Rows = new List<T>();
            }
        }
    }

    public interface ITable
    {
        
    }

    public interface IColumn
    {
        string Header { get; set; }

        Type ContentType { get; }

        object this[int row]
        {
            get;
            set;
        }
    }

    public interface IColumn<T> : IColumn
    {
        new T this[int row]
        {
            get;
            set;
        }
    }

    public interface IRow
    {

    }

    public class TableView : ITable
    {

    }
}
