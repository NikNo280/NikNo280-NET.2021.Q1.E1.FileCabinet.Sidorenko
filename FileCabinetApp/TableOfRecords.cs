using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace Table
{
    /// <summary>
    /// Prints records to the table.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    public class TableOfRecords<T>
    {
        private IEnumerable<T> items;
        private List<List<string>> table;
        private int[] lenghtRow;
        private string separatoryLine;
        private PropertyInfo[] properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfRecords{T}"/> class.
        /// </summary>
        /// <param name="items">Сollection to print.</param>
        /// <param name="properties">Property to print.</param>
        public TableOfRecords(IEnumerable<T> items, PropertyInfo[] properties)
        {
            this.items = items;
            this.table = new List<List<string>>();
            this.properties = properties;
            this.InitLength();
            this.InitSeparatoryLine();
        }

        /// <summary>
        /// Print Table.
        /// </summary>
        /// <param name="action">Method for print.</param>
        public void PrintTable(Action<string> action)
        {
            this.GetItems(action);
        }

        private void InitSeparatoryLine()
        {
            var line = new StringBuilder();
            line.Append("+-");
            for (int j = 0; j < this.properties.Length; j++)
            {
                line.Append(new string('-', this.lenghtRow[j]) + "-+-");
            }

            this.separatoryLine = line.ToString()[..^1];
        }

        private void InitLength()
        {
            var length = new List<int>();
            foreach (var property in this.properties)
            {
                length.Add(property.Name.Length);
            }

            foreach (var item in this.items)
            {
                for (int i = 0; i < this.properties.Length; i++)
                {
                    var tempLength = this.properties[i].GetValue(item).ToString().Length;
                    if (tempLength > length[i])
                    {
                        length[i] = tempLength;
                    }
                }
            }

            this.lenghtRow = length.ToArray();
        }

        private void GetItems(Action<string> action)
        {
            var line = new StringBuilder();
            action?.Invoke(this.separatoryLine);

            line.Append('|');
            for (int i = 0; i < this.properties.Length; i++)
            {
                line.Append(' ' + new string(' ', this.lenghtRow[i] - this.properties[i].Name.Length) + this.properties[i].Name + " |");
            }

            action?.Invoke(line.ToString());
            action?.Invoke(this.separatoryLine);
            foreach (var item in this.items)
            {
                line.Clear();
                line.Append('|');
                for (int i = 0; i < this.properties.Length; i++)
                {
                    string stringItem;
                    if (this.properties[i].PropertyType == typeof(DateTime))
                    {
                        DateTime dateofbirth = (DateTime)this.properties[i].GetValue(item);
                        stringItem = dateofbirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        stringItem = this.properties[i].GetValue(item).ToString();
                    }

                    line.Append(' ' + new string(' ', this.lenghtRow[i] - stringItem.Length) + stringItem + " |");
                }

                action?.Invoke(line.ToString());
                action?.Invoke(this.separatoryLine);
            }
        }
    }
}